using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // Import for Button
using System.Collections;

public class HealthManager : MonoBehaviour
{
    public int playerHealth = 12;
    public int enemyHealth = 12;

    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI enemyHealthText;
    [SerializeField] private TextMeshProUGUI stageInfoText;

    [Header("Win and Lose Popups")]
    [SerializeField] private GameObject winPopup;
    [SerializeField] private GameObject clearPopup;
    [SerializeField] private GameObject losePopup;

    [SerializeField] private GameObject ExitCheckPopup;
    [SerializeField] private GameObject ContinueCheckPopup;

    [SerializeField] private GameObject bossInfoText;

    [SerializeField] private TextMeshProUGUI winText;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button getCardButton;
    [SerializeField] private Button deleteCardButton;
    [SerializeField] private Button shopButton;

    [Header("Heart Images")]
    [SerializeField] private Image playerHeartImage;
    [SerializeField] private Image enemyHeartImage;

    [Header("Heart Sprites")]
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite threeQuarterHeart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite lowHeart;

    private GameManager gameManager;
    private BoardManager boardManager;
    private TurnManager turnManager;
    private EnemyDeckManager enemyDeckManager;
    private EnemyManager enemyManager;

    public PlayerSaveData CurrentSaveData { get; private set; }

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
        ExitCheckPopup.SetActive(false);
        bossInfoText.SetActive(false);
        ContinueCheckPopup.SetActive(false);

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
        UpdateHeartDisplay();
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
        TurnManager.Instance.isPaused = true;
        int nextStage = GameManager.Instance.CurrentSaveData.currentStage + 0;

        Debug.Log($"nextStage:{nextStage}");
        if (nextStage == 12)
        {
            ShowClearPopup();
            return;
        }

        winPopup.SetActive(true);
        quitButton.gameObject.SetActive(false);
        winText.text = "+2";
        Debug.Log($"(checker: {StageType.Normal})");
        continueButton.gameObject.SetActive(true);
        GetCurrentStagenow();
        UpdateWinButtons();
    }

    private void UpdateWinButtons()
    {
        // Show all options
        shopButton.gameObject.SetActive(true);
        getCardButton.gameObject.SetActive(true);
        deleteCardButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);

        // Add listeners to each button
        shopButton.onClick.RemoveAllListeners();
        getCardButton.onClick.RemoveAllListeners();
        deleteCardButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();

        shopButton.onClick.AddListener(() => GoToShop());
        getCardButton.onClick.AddListener(() => GoToGetCard());
        deleteCardButton.onClick.AddListener(() => GoToDeleteCard());
        continueButton.onClick.AddListener(() =>
        {
            GameManager.Instance.AdvanceStage();
            GameManager.Instance.AddMoney(2);
            GameManager.Instance.ContinueGame();
            TurnManager.Instance.isPaused = false;
        });
    }

    private void GoToShop()
    {
        SceneManager.LoadScene("ShopStage");
    }

    private void GoToGetCard()
    {
        SceneManager.LoadScene("ChooseCard");
    }

    private void GoToDeleteCard()
    {
        SceneManager.LoadScene("DeleteCard");
    }

    private void ShowClearPopup()
    {
        TurnManager.Instance.isPaused = true;
        clearPopup.SetActive(true);
        quitButton.gameObject.SetActive(false);
        GameManager.Instance.ResetSaveData();
    }

    private void ShowLosePopup()
    {
        TurnManager.Instance.isPaused = true;
        losePopup.SetActive(true);
        quitButton.gameObject.SetActive(false);
        GameManager.Instance.ResetSaveData();
    }

    public void AlertBoxQuitON()
    {
        ExitCheckPopup.SetActive(true);
    }

    public void AlertBoxQuitOFF()
    {
        ExitCheckPopup.SetActive(false);
    }

    public void AlertBoxContinueON()
    {
        ContinueCheckPopup.SetActive(true);
    }

    public void AlertBoxContinueOFF()
    {
        ContinueCheckPopup.SetActive(false);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Mainmenu");
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

    private void GetCurrentStagenow()
    {
        int currentStageNow = GameManager.Instance.CurrentSaveData.currentStage;

        if ((currentStageNow + 1) % 4 == 0)
        {
            bossInfoText.SetActive(true);
        }
        stageInfoText.text = $"You Pass Stage ({currentStageNow}/12)";
    }
    
    public int GetPlayerHealth()
    {
        return playerHealth;
    }

     private void UpdateHeartDisplay()
    {
        // Update Player Heart UI
        if (playerHealth <= 3)
            playerHeartImage.sprite = lowHeart;
        else if (playerHealth <= 6)
            playerHeartImage.sprite = halfHeart;
        else if (playerHealth <= 9)
            playerHeartImage.sprite = threeQuarterHeart;
        else
            playerHeartImage.sprite = fullHeart;

        // Update Enemy Heart UI
        if (enemyHealth <= 3)
            enemyHeartImage.sprite = lowHeart;
        else if (enemyHealth <= 6)
            enemyHeartImage.sprite = halfHeart;
        else if (enemyHealth <= 9)
            enemyHeartImage.sprite = threeQuarterHeart;
        else
            enemyHeartImage.sprite = fullHeart;
    }

}
