// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class StageManagerV2 : MonoBehaviour
// {
//     [Header("Stage Settings")]
//     public int maxStages = 12;
//     public int bossInterval = 4;

//     [Header("Button References")]
//     public Button normalStageButton;
//     public Button challengeStageButton;
//     public Button bossButton;
//     public Button shopButton;
//     public Button chooseCardButton;
//     public Button deleteCardButton;

//     private GameManager gameManager;
//     private EnemyDeckManager enemyDeckManager;
//     private int currentStage => GameManager.Instance.CurrentSaveData.currentStage;
//     private Difficulty currentDifficulty = Difficulty.Easy;

//     private void Start()
//     {
//         gameManager = GameManager.Instance;
//         enemyDeckManager = FindObjectOfType<EnemyDeckManager>();

//         if (gameManager == null)
//         {
//             Debug.LogError("GameManager is not found in the scene!");
//             return;
//         }

//         if (enemyDeckManager == null)
//         {
//             Debug.LogError("EnemyDeckManager is not found in the scene!");
//             return;
//         }

//         InitializeStage();
//         SetupButtonListeners();
//     }

//     private void InitializeStage()
//     {
//         Debug.Log($"Starting Stage: {currentStage}");

//         // Check and update difficulty
//         UpdateDifficulty();
        
//         // Setup stage configuration
//         SetupStageConfiguration();
        
//         // Update button visibility
//         UpdateButtonVisibility();
//     }

//     private void SetupButtonListeners()
//     {
//         normalStageButton.onClick.AddListener(() => StartStage(StageType.Normal));
//         challengeStageButton.onClick.AddListener(() => StartStage(StageType.Challenge));
//         bossButton.onClick.AddListener(() => StartStage(StageType.Boss));
//         shopButton.onClick.AddListener(OpenShop);
//         chooseCardButton.onClick.AddListener(ChooseCard);
//         deleteCardButton.onClick.AddListener(DeleteCard);
//     }

//     private void UpdateButtonVisibility()
//     {
//         bool isBossFight = IsBossFight();
        
//         normalStageButton.gameObject.SetActive(!isBossFight);
//         challengeStageButton.gameObject.SetActive(!isBossFight);
//         bossButton.gameObject.SetActive(isBossFight);
        
//         shopButton.gameObject.SetActive(true);
//         chooseCardButton.gameObject.SetActive(true);
//         deleteCardButton.gameObject.SetActive(true);
//     }

//     private void SetupStageConfiguration()
//     {
//         StageType stageType = IsBossFight() ? StageType.Boss : StageType.Normal;

//         // Setup stage configuration
//         StageConfiguration stageConfig = new StageConfiguration
//         {
//             stageType = stageType,
//             difficulty = currentDifficulty,
//             stageName = $"{stageType} Stage {currentStage}"
//         };
//         // Set and initialize the enemy deck
//         gameManager.SetCurrentStage(stageConfig);
//         enemyDeckManager.InitializeEnemyDeck(stageConfig);

//         Debug.Log($"Initialized Stage: {stageConfig.stageName} with Difficulty: {currentDifficulty}");
//     }

//     private void UpdateDifficulty()
//     {
//         if (currentStage > 0 && currentStage % bossInterval == 0)
//         {
//             currentDifficulty = currentDifficulty switch
//             {
//                 Difficulty.Easy => Difficulty.Normal,
//                 Difficulty.Normal => Difficulty.Hard,
//                 _ => Difficulty.Hard
//             };

//             Debug.Log($"Difficulty Increased to: {currentDifficulty}");
//         }
//     }

//     private bool IsBossFight()
//     {
//         return currentStage % bossInterval == 0;
//     }

//     public void AdvanceStage()
//     {
//         gameManager.AdvanceStage();

//         if (currentStage > maxStages)
//         {
//             Debug.Log("Congratulations! You completed the game.");
//             SceneManager.LoadScene("MainMenu");
//         }
//         else
//         {
//             InitializeStage();
//         }
//     }

//     public void OpenShop()
//     {
//         GameManager.Instance.CurrentSaveData.stageTypeSelected = "Shop";
//         GameManager.Instance.CurrentSaveData.hasChosenBreak = true;
//         GameManager.Instance.SaveData();
//         SceneManager.LoadScene("ShopStage");
//     }

//     public void ChooseCard()
//     {
//         GameManager.Instance.CurrentSaveData.stageTypeSelected = "GetCard";
//         GameManager.Instance.CurrentSaveData.hasChosenBreak = true;
//         GameManager.Instance.SaveData();
//         SceneManager.LoadScene("ChooseCard");
//     }

//     public void DeleteCard()
//     {
//         GameManager.Instance.CurrentSaveData.stageTypeSelected = "DeleteCard";
//         GameManager.Instance.CurrentSaveData.hasChosenBreak = true;
//         GameManager.Instance.SaveData();
//         SceneManager.LoadScene("DeleteCard");
//     }


//     public void StartStage(StageType stageType)
//     {
//         GameManager.Instance.CurrentSaveData.stageTypeSelected = stageType.ToString();
//         GameManager.Instance.CurrentSaveData.currentStageType = stageType;
//         GameManager.Instance.CurrentSaveData.hasChosenStageType = true;
//         GameManager.Instance.SaveData();

//         SceneManager.LoadScene("PlayScene");
//     }

//     void OnComplete()
//     {
//         GameManager.Instance.CurrentSaveData.hasChosenBreak = false;
//         GameManager.Instance.SaveData();
//         SceneManager.LoadScene("PlayScene");
//     }


// }


// //playscene
// // void Start()
// // {
// //     var currentStageType = GameManager.Instance.CurrentSaveData.currentStageType;
// //     var hasChosenStageType = GameManager.Instance.CurrentSaveData.hasChosenStageType;

// //     if (hasChosenStageType)
// //     {
// //         Debug.Log($"Loading {currentStageType} Stage...");
// //         if (currentStageType == StageType.Boss)
// //         {
// //             // Load Boss
// //         }
// //         else
// //         {
// //             // Load Normal
// //         }
// //     }
// // }
