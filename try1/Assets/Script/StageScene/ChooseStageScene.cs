// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using TMPro;
// using System.Collections;

// public class ChooseStageScene : MonoBehaviour
// {
//     public TextMeshProUGUI coinUIText;
//     public TextMeshProUGUI currentStageText;
//     public StageSelectionPopup stageSelectionPopup;
//     public StageDatabase stageDatabase;

//     private void Start()
//     {
//         if (!GameManager.Instance.CurrentSaveData.isSaveValid)
//         {
//             Debug.LogWarning("Invalid save data. Returning to Main Menu.");
//             SceneManager.LoadScene("MainMenu");
//             return; // Stop execution
//         }

//         UpdateUI();
//         LoadOrGenerateStageOptions();
//     }




//     private void UpdateUI()
//     {
//         var saveData = GameManager.Instance.CurrentSaveData;
//         coinUIText.text = $"{saveData.money}";
//         currentStageText.text = $"{saveData.currentStage}";
//     }

//     private void LoadOrGenerateStageOptions()
//     {
//         if (GameManager.Instance.LastStageChoices.Count > 0)
//         {
//             Debug.Log("Loading previously saved stage choices.");
//             stageSelectionPopup.GenerateStageChoices(GameManager.Instance.LastStageChoices);
//         }
//         else
//         {
//             Debug.Log("i fuked");
//             GenerateStageOptions();
//         }
//     }


//     private void GenerateStageOptions()
//     {
//         int currentStage = GameManager.Instance.CurrentSaveData.currentStage;
//         List<StageConfiguration> possibleStages = GetValidStages(currentStage);
//         List<StageConfiguration> selectedStages = new List<StageConfiguration>();

//         if (currentStage % 9 == 0) // Boss Stage
//         {
//             StageConfiguration bossStage = possibleStages.Find(stage => stage.stageType == StageType.Boss);
//             if (bossStage != null)
//             {
//                 selectedStages.Add(bossStage);
//             }
//         }
//         else
//         {
//             possibleStages.RemoveAll(stage => stage.stageType == StageType.Boss);
            
//             while (selectedStages.Count < 3 && possibleStages.Count > 0)
//             {
//                 int randomIndex = Random.Range(0, possibleStages.Count);
//                 StageConfiguration chosenStage = possibleStages[randomIndex];

//                 // Prevent duplicates
//                 if (!selectedStages.Contains(chosenStage))
//                 {
//                     selectedStages.Add(chosenStage);
//                 }
//                 possibleStages.RemoveAt(randomIndex);
//             }
//         }

//         GameManager.Instance.SetLastStageChoices(selectedStages);
//         GameManager.Instance.SaveData();

//         stageSelectionPopup.GenerateStageChoices(selectedStages);
//     }

//     private List<StageConfiguration> GetValidStages(int currentStage)
//     {
//         if (GameManager.Instance.stageDatabase == null)
//         {
//             Debug.LogError("StageDatabase is NULL in GameManager!");
//             return new List<StageConfiguration>();
//         }

//         List<StageConfiguration> allStages = GameManager.Instance.stageDatabase.stageConfigs;
//         List<StageConfiguration> validStages = new List<StageConfiguration>();

//         if (currentStage % 9 == 0)
//         {
//             return allStages.FindAll(stage => stage.stageType == StageType.Boss);
//         }

//         foreach (StageConfiguration stage in allStages)
//         {
//             if (currentStage % 4 == 0 && (stage.stageType == StageType.Normal || stage.stageType == StageType.Challenge))
//             {
//                 validStages.Add(stage);
//             }
//             else if (currentStage % 9 != 0)
//             {
//                 validStages.Add(stage);
//             }
//         }
//         return validStages;
//     }

//     public void ReturnMainMenu()
//     {
//         GameManager.Instance.SaveData();
//         SceneManager.LoadScene("MainMenu");
//     }
// }

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

        if (currentStage % 9 == 0)
            selectedStages.Add(possibleStages.Find(stage => stage.stageType == StageType.Boss));
        else
        {
            possibleStages.RemoveAll(stage => stage.stageType == StageType.Boss);
            while (selectedStages.Count < 3 && possibleStages.Count > 0)
            {
                int randomIndex = Random.Range(0, possibleStages.Count);
                if (!selectedStages.Contains(possibleStages[randomIndex]))
                    selectedStages.Add(possibleStages[randomIndex]);
                possibleStages.RemoveAt(randomIndex);
            }
        }

        GameManager.Instance.SetLastStageChoices(selectedStages);
        GameManager.Instance.SaveData();
        stageSelectionPopup.GenerateStageChoices(selectedStages);
    }

    // private List<StageConfiguration> GetValidStages(int currentStage)
    // {
    //     var allStages = GameManager.Instance.stageDatabase?.stageConfigs;
    //     if (allStages == null) return new();

    //     return currentStage % 9 == 0
    //         ? allStages.FindAll(stage => stage.stageType == StageType.Boss)
    //         : allStages.FindAll(stage => currentStage % 4 == 0
    //             ? stage.stageType == StageType.Normal || stage.stageType == StageType.Challenge
    //             : stage.stageType != StageType.Boss);
    // }

private List<StageConfiguration> GetValidStages(int currentStage)
{
    var allStages = GameManager.Instance.stageDatabase?.stageConfigs;
    if (allStages == null) return new();

    List<StageConfiguration> validStages = new();

    // ✅ Boss Fight (Every 9th stage) → Selects Boss Based on Difficulty
    if (currentStage % 9 == 0)
    {
        return allStages.FindAll(stage => 
            stage.stageType == StageType.Boss &&
            ((currentStage == 9 && stage.difficulty == Difficulty.Easy) ||
             (currentStage == 18 && stage.difficulty == Difficulty.Normal) ||
             (currentStage == 27 && stage.difficulty == Difficulty.Hard)));
    }

    // ✅ Force Combat Every 4th Stage (Scales by Difficulty)
    if (currentStage % 4 == 0)
    {
        return allStages.FindAll(stage => 
            (stage.stageType == StageType.Normal || stage.stageType == StageType.Challenge) &&
            ((currentStage <= 8 && stage.difficulty == Difficulty.Easy) ||
             (currentStage <= 17 && stage.difficulty == Difficulty.Normal) ||
             (currentStage <= 26 && stage.difficulty == Difficulty.Hard)));
    }

    // ✅ Difficulty Scaling for Regular Stages
    if (currentStage <= 8) // Stages 1-8 → Easy
        validStages = allStages.FindAll(stage => stage.difficulty == Difficulty.Easy);
    else if (currentStage <= 17) // Stages 10-17 → Normal
        validStages = allStages.FindAll(stage => stage.difficulty == Difficulty.Normal);
    else if (currentStage <= 26) // Stages 19-26 → Hard
        validStages = allStages.FindAll(stage => stage.difficulty == Difficulty.Hard);

    // ✅ Add Shop & ChooseCard (Duplicates Allowed)
    StageConfiguration shopStage = allStages.Find(stage => stage.stageType == StageType.Shop);
    StageConfiguration chooseCardStage = allStages.Find(stage => stage.stageType == StageType.ChooseCard);

    if (shopStage != null) validStages.Add(shopStage);
    if (chooseCardStage != null) validStages.Add(chooseCardStage);

    return validStages; // ✅ Returns all possible stages (including duplicates)
}



    public void ReturnMainMenu()
    {
        GameManager.Instance.SaveData();
        SceneManager.LoadScene("MainMenu");
    }
}
