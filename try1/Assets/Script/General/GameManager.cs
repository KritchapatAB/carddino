// using UnityEngine;
// using UnityEngine.SceneManagement;
// using System.Collections.Generic;
// using System.Collections;

// public class GameManager : MonoBehaviour
// {
//     public static GameManager Instance { get; private set; }
//     public PlayerSaveData CurrentSaveData { get; private set; }
//     public StageConfiguration CurrentStage { get; private set; }
//     private bool saveDataLoaded = false;

//     public StageDatabase stageDatabase;

//     // Stores last generated stage choices
//     public List<StageConfiguration> LastStageChoices { get; private set; } = new List<StageConfiguration>();

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject); // Keep GameManager across scenes
//             StartCoroutine(WaitForStageDatabaseAndLoadData());
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     // Ensure StageDatabase exists before loading save data
//     private IEnumerator WaitForStageDatabaseAndLoadData()
//     {
//         while (stageDatabase == null)
//         {
//             stageDatabase = FindObjectOfType<StageDatabase>(); // Find StageDatabase globally
//             yield return null; // Wait for the next frame
//         }

//         Debug.Log("StageDatabase is now initialized!");
//         LoadSaveData(); // Now it's safe to load save data
//     }

//     public void SaveData()
//     {
//         if (CurrentSaveData != null)
//         {
//             CurrentSaveData.lastStageNames.Clear();
//             foreach (var stage in LastStageChoices)
//             {
//                 CurrentSaveData.lastStageNames.Add(stage.stageName);
//             }

//             SaveManager.SaveGame(CurrentSaveData);
//             Debug.Log("Game data saved.");
//         }
//         else
//         {
//             Debug.LogWarning("No save data to save.");
//         }
//     }

//     public void LoadSaveData()
//     {
//         if (!saveDataLoaded) // Load once
//         {
//             CurrentSaveData = SaveManager.LoadGame(null);
//             if (CurrentSaveData == null)
//             {
//                 Debug.LogWarning("No valid save data found.");
//                 return;
//             }

//             LastStageChoices.Clear();

//             if (stageDatabase != null && CurrentSaveData.lastStageNames.Count > 0)
//             {
//                 foreach (var stageName in CurrentSaveData.lastStageNames)
//                 {
//                     StageConfiguration stage = stageDatabase.stageConfigs.Find(stage => stage.stageName == stageName);
//                     if (stage != null)
//                     {
//                         LastStageChoices.Add(stage);
//                     }
//                 }
//                 Debug.Log($" Loaded {LastStageChoices.Count} saved stage choices.");
//             }
//             else
//             {
//                 Debug.Log("No saved stage choices found. Generating new ones.");
//             }

//             saveDataLoaded = true;
//         }
//     }

//         // Reset save file (e.g., on game over)
//     public void ResetSaveData()
//     {
//         SaveManager.DeleteSave();
//         CurrentSaveData = null;
//         saveDataLoaded = false;
//     }

//     // Example utility functions for scenes
//     public void AddToPlayerDeck(int cardId)
//     {
//         CurrentSaveData.playerDeckIds.Add(cardId);
//     }

//     public void RemoveFromPlayerDeck(int cardId)
//     {
//         CurrentSaveData.playerDeckIds.Remove(cardId);
//     }

//     public void AdvanceStage()
//     {
//         CurrentSaveData.currentStage++;
//     }

//     public void AddMoney(int amount)
//     {
//         CurrentSaveData.money += amount;
//     }

//     public void SubtractMoney(int amount)
//     {
//         CurrentSaveData.money -= amount;
//     }

//     public void LastStageChoicesClear()
//     {
//         LastStageChoices.Clear();
//     }
    
//     // Store selected stage
//     public void SetCurrentStage(StageConfiguration stage)
//     {
//         CurrentStage = stage;
//     }

//     // Save last generated stage choices
//     public void SetLastStageChoices(List<StageConfiguration> stages)
//     {
//         LastStageChoices = new List<StageConfiguration>(stages);
//     }

//     // Get saved stage choices
//     public List<StageConfiguration> GetLastStageChoices()
//     {
//         return LastStageChoices.Count > 0 ? new List<StageConfiguration>(LastStageChoices) : null;

//     }

//     // Called when player wins a stage
//     public void PlayerWin()
//     {
//         CurrentSaveData.isSaveValid = true;
//         AddMoney(2); // Reward money
//         AdvanceStage();
//         SaveData();

//         // Clear last stage choices so the player gets new ones
//         LastStageChoicesClear();
//         SceneManager.LoadScene("ChooseStage"); // Return to stage selection
//     }
// }

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerSaveData CurrentSaveData { get; private set; }
    public StageConfiguration CurrentStage { get; private set; }
    private bool saveDataLoaded = false;

    public StageDatabase stageDatabase;
    public List<StageConfiguration> LastStageChoices { get; private set; } = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(WaitForStageDatabaseAndLoadData());
        }
        else Destroy(gameObject);
    }

    private IEnumerator WaitForStageDatabaseAndLoadData()
    {
        while (stageDatabase == null)
        {
            stageDatabase = FindObjectOfType<StageDatabase>();
            yield return null;
        }
        LoadSaveData();
    }

    public void SaveData()
    {
        if (CurrentSaveData == null) return;

        CurrentSaveData.lastStageNames.Clear();
        LastStageChoices.ForEach(stage => CurrentSaveData.lastStageNames.Add(stage.stageName));
        SaveManager.SaveGame(CurrentSaveData);
    }

    public void LoadSaveData()
    {
        if (saveDataLoaded) return;

        CurrentSaveData = SaveManager.LoadGame(null);
        if (CurrentSaveData == null) return;

        LastStageChoices.Clear();
        if (stageDatabase != null && CurrentSaveData.lastStageNames.Count > 0)
        {
            CurrentSaveData.lastStageNames.ForEach(stageName =>
            {
                StageConfiguration stage = stageDatabase.stageConfigs.Find(s => s.stageName == stageName);
                if (stage != null) LastStageChoices.Add(stage);
            });
        }
        saveDataLoaded = true;
    }

    public void ResetSaveData()
    {
        SaveManager.DeleteSave();
        CurrentSaveData = null;
        saveDataLoaded = false;
    }

    public void AddToPlayerDeck(int cardId) => CurrentSaveData.playerDeckIds.Add(cardId);
    public void RemoveFromPlayerDeck(int cardId) => CurrentSaveData.playerDeckIds.Remove(cardId);
    public void AdvanceStage() => CurrentSaveData.currentStage++;
    public void AddMoney(int amount) => CurrentSaveData.money += amount;
    public void SubtractMoney(int amount) => CurrentSaveData.money -= amount;
    public void LastStageChoicesClear() => LastStageChoices.Clear();
    public void SetCurrentStage(StageConfiguration stage) => CurrentStage = stage;
    public void SetLastStageChoices(List<StageConfiguration> stages) => LastStageChoices = new List<StageConfiguration>(stages);
    public List<StageConfiguration> GetLastStageChoices() => LastStageChoices.Count > 0 ? new(LastStageChoices) : null;

    public void PlayerWin()
    {
        CurrentSaveData.isSaveValid = true;
        AddMoney(2);
        AdvanceStage();
        SaveData();
        LastStageChoicesClear();
        SceneManager.LoadScene("ChooseStage");
    }
}