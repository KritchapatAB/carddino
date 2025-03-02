using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerSaveData CurrentSaveData { get; private set; }
    public StageConfiguration CurrentStage { get; private set; }
    public StageDatabase stageDatabase;
    public List<StageConfiguration> LastStageChoices { get; private set; } = new();

    private bool saveDataLoaded = false;
    private Difficulty currentDifficulty = Difficulty.Easy;

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
        SaveManager.SaveGame(CurrentSaveData);
    }

    public void LoadSaveData()
    {
        if (saveDataLoaded) return;
        CurrentSaveData = SaveManager.LoadGame(null);
        if (CurrentSaveData == null) return;
        saveDataLoaded = true;
    }

    public void ResetSaveData()
    {
        // Reset Difficulty to Easy when clearing save data
        currentDifficulty = Difficulty.Easy;

        SaveManager.DeleteSave();
        CurrentSaveData = new PlayerSaveData
        {
            currentStage = 1,
            money = 0,
            playerDeckIds = new List<int>(),
            isSaveValid = false
        };
        saveDataLoaded = false; // Reset the flag to allow reloading on new game
        Debug.Log("Save data has been reset. Difficulty reset to Easy.");
        ResetCurrentStage();
    }


    public void AddToPlayerDeck(int cardId) => CurrentSaveData.playerDeckIds.Add(cardId);
    public void RemoveFromPlayerDeck(int cardId) => CurrentSaveData.playerDeckIds.Remove(cardId);
    public void AddMoney(int amount) => CurrentSaveData.money += amount;
    public void SubtractMoney(int amount) => CurrentSaveData.money -= amount;
    public void ClearMoney() => CurrentSaveData.money = 0;

    public void ResetCurrentStage()
    {
        CurrentSaveData.currentStage = 1;
    }

    public void AdvanceStage()
    {
        CurrentSaveData.currentStage++;
        SaveData();
    }

    public void SetCurrentStage(StageConfiguration stageConfig)
    {
        CurrentStage = stageConfig;
    }

    public void ContinueGame()
    {
        Debug.Log("Continuing Game...");
        int currentStage = CurrentSaveData.currentStage;

        // Update Difficulty before determining Stage Type
        UpdateDifficulty(currentStage);

        // Determine Stage Type
        StageType stageType = GetStageType(currentStage);

        // Log current stage, difficulty, and type
        Debug.Log($"Stage: {currentStage}, Difficulty: {currentDifficulty}, Type: {stageType}");

        // Get corresponding StageConfiguration from StageDatabase
        StageConfiguration stageConfig = stageDatabase.stageConfigs.Find(config =>
            config.difficulty == currentDifficulty && 
            config.stageType == stageType);

        if (stageConfig == null)
        {
            Debug.LogError($"No StageConfiguration found for Difficulty: {currentDifficulty} and StageType: {stageType}");
            return;
        }

        // Save the configuration in GameManager
        SetCurrentStage(stageConfig);

        SaveData();
        SceneManager.LoadScene("PlayScene");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayScene")
        {
            EnemyDeckManager enemyDeckManager = FindObjectOfType<EnemyDeckManager>();
            if (enemyDeckManager != null)
            {
                enemyDeckManager.InitializeEnemyDeck(CurrentStage);
            }
            else
            {
                Debug.LogError("EnemyDeckManager not found! Deck initialization failed.");
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void StartNewGame()
    {
        // Reset Difficulty to Easy for a fresh start
        currentDifficulty = Difficulty.Easy;

        // Initialize a fresh PlayerSaveData
        CurrentSaveData = new PlayerSaveData
        {
            currentStage = 1,
            money = 0,
            playerDeckIds = new List<int>(), // Clear deck explicitly
            isSaveValid = true
        };

        saveDataLoaded = true; // Flag that new game is initialized
        SaveData(); // Save the initialized state

        Debug.Log("New game started. Difficulty reset to Easy.");
    }


    private void UpdateDifficulty(int currentStage)
    {
        // Difficulty should be calculated based on the number of 4-stage cycles completed
        int completedBlocks = (currentStage - 1) / 4;

        // Set difficulty based on completed blocks (instead of incrementing every time)
        currentDifficulty = completedBlocks switch
        {
            0 => Difficulty.Easy,   // Stages 1-4 → Easy
            1 => Difficulty.Normal, // Stages 5-8 → Normal
            _ => Difficulty.Hard    // Stages 9+ → Hard
        };

        Debug.Log($"Difficulty Set to: {currentDifficulty} at Stage {currentStage}");
    }


    private StageType GetStageType(int currentStage)
    {
        // Every 4th stage is a Boss Fight
        return (currentStage % 4 == 0) ? StageType.Boss : StageType.Normal;
    }

    public void PlayerWin()
    {
        CurrentSaveData.isSaveValid = true;
        AddMoney(2);
    }

    public void ForceReloadSaveData()
    {
        LoadSaveData(); // Force reload of save data
        Debug.Log("Save data reloaded in GameManager.");
    }

}