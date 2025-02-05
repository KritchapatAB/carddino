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

    private List<StageConfiguration> GetValidStages(int currentStage)
    {
        var allStages = GameManager.Instance.stageDatabase?.stageConfigs;
        if (allStages == null) return new();

        return currentStage % 9 == 0
            ? allStages.FindAll(stage => stage.stageType == StageType.Boss)
            : allStages.FindAll(stage => currentStage % 4 == 0
                ? stage.stageType == StageType.Normal || stage.stageType == StageType.Challenge
                : stage.stageType != StageType.Boss);
    }

    public void ReturnMainMenu()
    {
        GameManager.Instance.SaveData();
        SceneManager.LoadScene("MainMenu");
    }
}
