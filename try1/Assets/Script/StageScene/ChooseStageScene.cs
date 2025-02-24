using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class ChooseStageScene : MonoBehaviour
{
    public TextMeshProUGUI coinUIText, currentStageText;
    public StageSelectionPopup stageSelectionPopup;

    private void Start()
    {
        if (!GameManager.Instance.CurrentSaveData.isSaveValid)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        UpdateUI();
        LoadOrGenerateStageOptions();
    }

    private void UpdateUI()
    {
        var saveData = GameManager.Instance.CurrentSaveData;
        coinUIText.text = $"{saveData.money}";
        currentStageText.text = $"{saveData.currentStage}";
    }

    private void LoadOrGenerateStageOptions()
    {
        if (GameManager.Instance.LastStageChoices.Count > 0)
            stageSelectionPopup.GenerateStageChoices(GameManager.Instance.LastStageChoices);
        else
            GenerateStageOptions();
    }

private void GenerateStageOptions()
{
    int currentStage = GameManager.Instance.CurrentSaveData.currentStage;
    List<StageConfiguration> possibleStages = GetValidStages(currentStage);
    List<StageConfiguration> selectedStages = new();

    if (currentStage % 9 == 0) // ✅ Boss Fight (One Choice Only)
    {
        StageConfiguration bossStage = possibleStages.Find(stage => stage.stageType == StageType.Boss);
        if (bossStage != null) selectedStages.Add(bossStage);
    }
    else
    {
        possibleStages.RemoveAll(stage => stage.stageType == StageType.Boss); // Ensure Boss doesn't appear outside 9th stage
        
        while (selectedStages.Count < 3 && possibleStages.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleStages.Count);
            selectedStages.Add(possibleStages[randomIndex]); // ✅ Allows duplicate stages
        }
    }

    GameManager.Instance.SetLastStageChoices(selectedStages);
    GameManager.Instance.SaveData();
    stageSelectionPopup.GenerateStageChoices(selectedStages);
}


private List<StageConfiguration> GetValidStages(int currentStage)
{
    var allStages = GameManager.Instance.stageDatabase?.stageConfigs;
    if (allStages == null) return new();

    List<StageConfiguration> validStages = new();

    // ✅ Boss Fight (Every 9th Stage) → Selects One Boss Based on Difficulty
    if (currentStage % 9 == 0)
    {
        return allStages.FindAll(stage => 
            stage.stageType == StageType.Boss &&
            ((currentStage == 9 && stage.difficulty == Difficulty.Easy) ||
             (currentStage == 18 && stage.difficulty == Difficulty.Normal) ||
             (currentStage == 27 && stage.difficulty == Difficulty.Hard)));
    }

    // ✅ First, filter all stages based on difficulty
    if (currentStage <= 9) 
        validStages = allStages.FindAll(stage => stage.difficulty == Difficulty.Easy);
    else if (currentStage <= 18) 
        validStages = allStages.FindAll(stage => stage.difficulty == Difficulty.Normal);
    else if (currentStage <= 27) 
        validStages = allStages.FindAll(stage => stage.difficulty == Difficulty.Hard);

    // ✅ If it's a multiple of 4, force Normal/Challenge from the filtered stages
    if (currentStage % 4 == 0)
    {
        return validStages.FindAll(stage => stage.stageType == StageType.Normal || stage.stageType == StageType.Challenge);
    }

    // ✅ Add Shop & ChooseCard randomly (based on number of stages in difficulty)
    int stageCount = validStages.Count;
    if (stageCount > 0)
    {
        float chance = 1f / stageCount; // Dynamic probability based on available stages
        if (Random.value < chance)
        {
            StageConfiguration shopStage = allStages.Find(stage => stage.stageType == StageType.Shop);
            if (shopStage != null) validStages.Add(shopStage);
        }

        if (Random.value < chance)
        {
            StageConfiguration chooseCardStage = allStages.Find(stage => stage.stageType == StageType.ChooseCard);
            if (chooseCardStage != null) validStages.Add(chooseCardStage);
        }
    }

    return validStages;
}

    public void ReturnMainMenu()
    {
        GameManager.Instance.SaveData();
        SceneManager.LoadScene("MainMenu");
    }

    public void StageOverride()
    {
        SceneManager.LoadScene("ShopStage");
    }

    public void StagePlayer()
    {
        SceneManager.LoadScene("PlayerDeckStage");
    }
}

