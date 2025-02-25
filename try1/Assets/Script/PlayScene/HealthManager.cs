using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // Import for Button

public class HealthManager : MonoBehaviour
{
    public int playerHealth = 12;
    public int enemyHealth = 12;

    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI enemyHealthText;

    [Header("Win and Lose Popups")]
    [SerializeField] private GameObject winPopup;
    [SerializeField] private GameObject clearPopup;
    [SerializeField] private GameObject losePopup;
    [SerializeField] private TextMeshProUGUI winText;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button skipBattleButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button clearButton;

    private GameManager gameManager;
    private BoardManager boardManager;
    private TurnManager turnManager;
    private EnemyDeckManager enemyDeckManager;
    private EnemyManager enemyManager;

    public static HealthManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one instance
        }
    }

    private void Start()
    {
        UpdateHealthUI();
        winPopup.SetActive(false);
        clearPopup.SetActive(false);
        losePopup.SetActive(false);

        // 🔥 Add Listeners to Buttons
        // continueButton.onClick.AddListener(HandleWinCondition);
        // quitButton.onClick.AddListener(QuitGame);
        // skipBattleButton.onClick.AddListener(SkipBattle);
        // clearButton.onClick.AddListener(HandleClearCondition);

        skipBattleButton.gameObject.SetActive(false);

        gameManager = FindObjectOfType<GameManager>();
        boardManager = FindObjectOfType<BoardManager>();
        turnManager = FindObjectOfType<TurnManager>();
        enemyDeckManager = FindObjectOfType<EnemyDeckManager>();
        enemyManager = FindObjectOfType<EnemyManager>();

        if (gameManager == null || boardManager == null || turnManager == null || enemyDeckManager == null || enemyManager == null)
        {
            Debug.LogError("Required manager is missing in the scene!");
        }
    }

    private void UpdateHealthUI()
    {
        playerHealthText.text = playerHealth.ToString();
        enemyHealthText.text = enemyHealth.ToString();
    }

    public void DamagePlayer(int damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            ShowLosePopup();
        }
        UpdateHealthUI();
    }

    public void DamageEnemy(int damage)
    {
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            enemyHealth = 0;
            ShowWinPopup();
        }
        UpdateHealthUI();
    }

    private void ShowWinPopup()
    {
        int calStage = GameManager.Instance.CurrentSaveData.currentStage;
        int nextStage = calStage + 0;

        Debug.Log($"nextStage:{nextStage}");
        if (nextStage == 27)
        {
            ShowClearPopup();
            return;
        }

        winPopup.SetActive(true);
        quitButton.gameObject.SetActive(false);
        skipBattleButton.gameObject.SetActive(false);

        if (GameManager.Instance.CurrentStage.stageType == StageType.Challenge)
        {
            winText.text = "+4";
        }
        else
        {
            winText.text = "+2";
        }
        Debug.Log($"(checker: {StageType.Challenge})");
        continueButton.gameObject.SetActive(true);
    }

    private void ShowClearPopup()
    {
        clearPopup.SetActive(true);
        quitButton.gameObject.SetActive(false);
        skipBattleButton.gameObject.SetActive(false);
    }

    private void ShowLosePopup()
    {
        losePopup.SetActive(true);
        quitButton.gameObject.SetActive(false);
        skipBattleButton.gameObject.SetActive(false);

        // Invoke(nameof(HandleLoseCondition), 2.0f);
    }

    public void HandleWinCondition()
    {
        if (GameManager.Instance.CurrentStage.stageType == StageType.Challenge)
        {
            Debug.Log("Player Wins Challenge!");
            gameManager.PlayerWinChallenge();
            SceneManager.LoadScene("ChooseStage");
        }
        else
        {
            Debug.Log("Player Wins Normal Stage!");
            gameManager.PlayerWin();
            SceneManager.LoadScene("ChooseStage");
        }
    }

    public void HandleLoseCondition()
    {
        gameManager.ResetSaveData();
        SceneManager.LoadScene("MainMenu");
    }

    public void HandleClearCondition()
    {
        gameManager.ResetSaveData();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Returning to ChooseStage...");
        SceneManager.LoadScene("ChooseStage");
    }

    public void SkipBattle()
    {
        Debug.Log("Skipping Battle... Calculating Result...");

        winPopup.SetActive(true);
        skipBattleButton.gameObject.SetActive(false);

        if (playerHealth > enemyHealth)
        {
            winText.text = "You Win!";
            Invoke(nameof(HandleWinCondition), 2.0f);
        }
        else
        {
            winText.text = "You Lose!";
            Invoke(nameof(HandleLoseCondition), 2.0f);
        }
    }

    public void CheckWinLoseCondition()
    {
        if (enemyHealth <= 0)
        {
            ShowWinPopup();
            return;
        }

        if (NoEnemyCardsLeft() && enemyManager.enemyHand.Count == 0 && enemyDeckManager.IsDeckEmpty())
        {
            ShowWinPopup();
            return;
        }

        if (playerHealth <= 0)
        {
            ShowLosePopup();
            return;
        }

        if (ShouldShowSkipBattle())
        {
            skipBattleButton.gameObject.SetActive(true);
        }
    }

    private bool NoEnemyCardsLeft()
    {
        return boardManager.enemyActiveSlots.TrueForAll(slot => !slot.GetComponent<EnemyCardSlot>().IsOccupied()) &&
               boardManager.enemyReserveSlots.TrueForAll(slot => !slot.GetComponent<EnemyCardSlot>().IsOccupied());
    }

    private bool ShouldShowSkipBattle()
    {
        return turnManager.GetCurrentTurn() >= 21 || boardManager.engagePlayerDeck.Count == 0;
    }
}
