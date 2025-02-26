using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManagerV2 : MonoBehaviour
{
    [Header("Stage Settings")]
    public int maxStages = 12;
    public int bossInterval = 4;

    [Header("Button References")]
    public Button normalStageButton;
    public Button challengeStageButton;
    public Button bossButton;
    public Button shopButton;
    public Button chooseCardButton;
    public Button deleteCardButton;

    private GameManager gameManager;
    private EnemyDeckManager enemyDeckManager;
    private int currentStage => GameManager.Instance.CurrentSaveData.currentStage;
    private Difficulty currentDifficulty = Difficulty.Easy;

    private void Start()
    {
        gameManager = GameManager.Instance;
        enemyDeckManager = FindObjectOfType<EnemyDeckManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager is not found in the scene!");
            return;
        }

        if (enemyDeckManager == null)
        {
            Debug.LogError("EnemyDeckManager is not found in the scene!");
            return;
        }

        InitializeStage();
        SetupButtonListeners();
    }

    private void InitializeStage()
    {
        Debug.Log($"Starting Stage: {currentStage}");

        // Check and update difficulty
        UpdateDifficulty();
        
        // Setup stage configuration
        SetupStageConfiguration();
        
        // Update button visibility
        UpdateButtonVisibility();
    }

    private void SetupButtonListeners()
    {
        normalStageButton.onClick.AddListener(() => StartStage(StageType.Normal));
        challengeStageButton.onClick.AddListener(() => StartStage(StageType.Challenge));
        bossButton.onClick.AddListener(() => StartStage(StageType.Boss));
        shopButton.onClick.AddListener(OpenShop);
        chooseCardButton.onClick.AddListener(ChooseCard);
        deleteCardButton.onClick.AddListener(DeleteCard);
    }

    private void UpdateButtonVisibility()
    {
        // Boss Fight Condition
        bool isBossFight = IsBossFight();
        
        // Combat Stages (Normal or Challenge)
        normalStageButton.gameObject.SetActive(!isBossFight);
        challengeStageButton.gameObject.SetActive(!isBossFight);

        // Boss Button Visibility
        bossButton.gameObject.SetActive(isBossFight);

        // Short Break Buttons (Always Active After Fight)
        shopButton.gameObject.SetActive(true);
        chooseCardButton.gameObject.SetActive(true);
        deleteCardButton.gameObject.SetActive(true);
    }

    private void SetupStageConfiguration()
    {
        StageType stageType = IsBossFight() ? StageType.Boss : StageType.Normal;

        // Setup stage configuration
        StageConfiguration stageConfig = new StageConfiguration
        {
            stageType = stageType,
            difficulty = currentDifficulty,
            stageName = $"{stageType} Stage {currentStage}"
        };

        if (stageType == StageType.Challenge)
        {
            stageConfig.bonusHealth = 2;
            stageConfig.bonusDamage = 1;
        }

        // Set and initialize the enemy deck
        gameManager.SetCurrentStage(stageConfig);
        enemyDeckManager.InitializeEnemyDeck(stageConfig);

        Debug.Log($"Initialized Stage: {stageConfig.stageName} with Difficulty: {currentDifficulty}");
    }

    private void UpdateDifficulty()
    {
        if (currentStage > 0 && currentStage % bossInterval == 0)
        {
            currentDifficulty = currentDifficulty switch
            {
                Difficulty.Easy => Difficulty.Normal,
                Difficulty.Normal => Difficulty.Hard,
                _ => Difficulty.Hard
            };

            Debug.Log($"Difficulty Increased to: {currentDifficulty}");
        }
    }

    private bool IsBossFight()
    {
        return currentStage % bossInterval == 0;
    }

    public void StartStage(StageType stageType)
    {
        Debug.Log($"Starting {stageType} Stage...");
        SceneManager.LoadScene("PlayScene");
    }

    public void OpenShop()
    {
        Debug.Log("Entering Shop...");
        SceneManager.LoadScene("ShopStage");
    }

    public void ChooseCard()
    {
        Debug.Log("Choosing Card...");
        SceneManager.LoadScene("ChooseCard");
    }

    public void DeleteCard()
    {
        Debug.Log("Deleting Card...");
        SceneManager.LoadScene("DeleteCard");
    }

    public void AdvanceStage()
    {
        gameManager.AdvanceStage();

        if (currentStage > maxStages)
        {
            Debug.Log("Congratulations! You completed the game.");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            InitializeStage();
        }
    }
}