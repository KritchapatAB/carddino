using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerSaveData CurrentSaveData { get; private set; }
    public StageConfiguration CurrentStage { get; private set; } 
    private bool saveDataLoaded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep GameManager across scenes
        }
        else
        {
            Destroy(gameObject); // Enforce singleton
        }
    }

    // Load save data
    public void LoadSaveData()
    {
        if (!saveDataLoaded) // Load once
        {
            CurrentSaveData = SaveManager.LoadGame(null);
            if (CurrentSaveData == null)
            {
                Debug.LogWarning("No valid save data found.");
                return;
            }
            saveDataLoaded = true;
        }
    }

    // Save data
    public void SaveData()
    {
        if (CurrentSaveData != null)
        {
            SaveManager.SaveGame(CurrentSaveData);
            Debug.Log("Game data saved.");
        }
        else
        {
            Debug.LogWarning("No save data to save.");
        }
    }

    // Reset save file (e.g., on game over)
    public void ResetSaveData()
    {
        SaveManager.DeleteSave();
        CurrentSaveData = null;
        saveDataLoaded = false;
    }

    // Example utility functions for scenes
    public void AddToPlayerDeck(int cardId)
    {
        CurrentSaveData.playerDeckIds.Add(cardId);
    }

    public void RemoveFromPlayerDeck(int cardId)
    {
        CurrentSaveData.playerDeckIds.Remove(cardId);
    }

    public void AdvanceStage()
    {
        CurrentSaveData.currentStage++;
    }

    public void AddMoney(int amount)
    {
        CurrentSaveData.money += amount;
    }

    public void SubtractMoney(int amount)
    {
        CurrentSaveData.money -= amount;
    }

    // ✅ Store selected stage
    public void SetCurrentStage(StageConfiguration stage)
    {
        CurrentStage = stage;
    }

    // ✅ Called when player wins a stage
    public void PlayerWin()
    {
        CurrentSaveData.isSaveValid = true;
        AddMoney(2); // Reward money
        AdvanceStage();
        SaveData();

        // Instead of unlocking stages, show a new random selection
        SceneManager.LoadScene("ChooseStage"); // Return to stage selection
    }
}
