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
        // CurrentSaveData = null;
        // saveDataLoaded = false;
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
    public void ResetCurrentStage()
    {
        CurrentSaveData.currentStage = 1;
    }
    public void ClearMoney() => CurrentSaveData.money = 0;
    
    public void PlayerWin()
    {
        CurrentSaveData.isSaveValid = true;
        AddMoney(2);
        AdvanceStage();
        SaveData();
        LastStageChoicesClear();
        // SceneManager.LoadScene("ChooseStage");
    }

    public void PlayerWinChallenge()
    {
        CurrentSaveData.isSaveValid = true;
        AddMoney(4);
        AdvanceStage();
        SaveData();
        LastStageChoicesClear();
    }
}