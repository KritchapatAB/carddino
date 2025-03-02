using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class TurnManager : MonoBehaviour
{
    public enum TurnState { PlayerTurn, EnemyTurn }
    public TurnState currentTurn = TurnState.EnemyTurn;

    private BoardManager boardManager;
    private EnemyManager enemyManager;
    private PlayerHand playerHand;

    private int turnCounter = 0;
    public float enemyTurnDelay = 2.0f;
    private bool hasDrawnCardThisTurn = false;

    public float attackDelay = 1.1f;

    // public bool isPaused = false;

    int ATTCount = 0;

    [SerializeField] private Button attackButton;
    [SerializeField] private TextMeshProUGUI turnCountText;

    [SerializeField] private GameObject background;
    private SpriteRenderer backgroundRenderer;
    private HealthManager healthManager;

    public static TurnManager Instance { get; private set; } // ✅ Singleton

    public bool isPaused = false; // ✅ Instance variable

    private void Awake()
    {
        // Ensure Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {

        healthManager = FindObjectOfType<HealthManager>();
        
        if (background != null)
        {
            backgroundRenderer = background.GetComponent<SpriteRenderer>(); // Get the SpriteRenderer
            if (backgroundRenderer == null)
            {
                Debug.LogError("Background object does not have a SpriteRenderer component!");
            }
        }

        boardManager = FindObjectOfType<BoardManager>();
        enemyManager = FindObjectOfType<EnemyManager>();
        playerHand = FindObjectOfType<PlayerHand>();

        if (enemyManager == null)
        {
            Debug.LogError("EnemyManager not found in scene!");
        }
        else
        {
            enemyManager.Initialize();
        }
        
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(OnAttackButtonClicked);
            attackButton.interactable = false; // Disable at start
        }
        UpdateTurnDisplay();
        if (currentTurn == TurnState.EnemyTurn)
        {
            StartCoroutine(HandleEnemyTurn());
        }
        else
        {
            StartPlayerTurn();
        }
    }

    public void StartPlayerTurn()
    {
        if (isPaused)
        {
            Debug.Log("Game is paused. Player's turn will not start.");
            return;
        }
        HandleBackgroundEffect();
        currentTurn = TurnState.PlayerTurn;
        UpdateTurnDisplay();
        Debug.Log($"Turn {turnCounter}: Player's turn starts.");
        hasDrawnCardThisTurn = false;

        boardManager.EnablePlayerControls();
        playerHand.TogglePlayerInteractions(true);

        if (attackButton != null)
        {
            bool canAttack = hasDrawnCardThisTurn || boardManager.engagePlayerDeck.Count == 0;
            attackButton.interactable = canAttack;
        }
    }

    public void EndPlayerTurn()
    {
        Debug.Log($"Turn {turnCounter}: Player's turn ends.");
        playerHand.DeselectCard();
        boardManager.DisableSacrificePhase();
        boardManager.DisablePlayerControls();
        playerHand.TogglePlayerInteractions(false);

        // ✅ Correct: First process attack phase, THEN switch turn
        StartCoroutine(ExecuteAttackPhase(() =>
        {
            // ✅ Now switch to Enemy Turn AFTER combat
            currentTurn = TurnState.EnemyTurn;
            turnCounter++;
            UpdateTurnDisplay();
            StartCoroutine(HandleEnemyTurn());
        }));
    }

    private IEnumerator HandleEnemyTurn()
    {
        if (isPaused)
        {
            Debug.Log("Game is paused. Enemy's turn will not start.");
            yield break;
        }
        Debug.Log($"Turn {turnCounter}: Enemy's turn starts.");
        yield return new WaitForSeconds(enemyTurnDelay);

        if (enemyManager != null)
        {
            enemyManager.StartTurn(turnCounter);
        }
        else
        {
            Debug.LogError("EnemyManager reference is missing!");
        }

        yield return new WaitForSeconds(enemyTurnDelay);

        // ✅ Skip Attack Phase but still Place Cards on Turn 0
        if (turnCounter == 0)
        {
            Debug.Log($"Turn {turnCounter}: Enemy places cards but skips attack on the first turn.");
            boardManager.MoveEnemyCardsToActiveArea(); // ✅ Move cards from Reserve to Active Area
            boardManager.enemyManager.ReturnHandToDeck();
            currentTurn = TurnState.PlayerTurn;
            turnCounter++;
            StartPlayerTurn();
            yield break; // ✅ Exit the coroutine to skip the attack phase
        }

        EndEnemyTurn(); // ✅ Normal behavior for all other turns
    }


    public void EndEnemyTurn()
    {
        
        Debug.Log($"Turn {turnCounter}: Enemy's turn ends.");
        boardManager.enemyManager.ReturnHandToDeck();

        // ✅ Correct: First process attack phase, THEN switch turn
        StartCoroutine(ExecuteAttackPhase(() =>
        {
            // ✅ Now switch to Player Turn AFTER combat
            currentTurn = TurnState.PlayerTurn;
            turnCounter++;
            boardManager.MoveEnemyCardsToActiveArea();
            StartPlayerTurn();
        }));
    }


    public bool CanPlayerDrawCard()
    {
        return currentTurn == TurnState.PlayerTurn && !hasDrawnCardThisTurn;
    }

    public void NotifyCardDrawn()
    {
        hasDrawnCardThisTurn = true;
        if (attackButton != null)
        {
            bool canAttack = hasDrawnCardThisTurn || boardManager.engagePlayerDeck.Count == 0;
            attackButton.interactable = canAttack;
        }
    }

    public void OnAttackButtonClicked()
    {
        if (!hasDrawnCardThisTurn && boardManager.engagePlayerDeck.Count > 0)
        {
            Debug.LogWarning("Cannot attack without drawing a card first or engage deck is not empty!");
            return;
        }
        
        if (attackButton != null)
        {
            attackButton.interactable = false;
        }
    
        ATTCount = ATTCount +1;
        Debug.Log($"Attack button clicked. Ending player's turn{ATTCount}.");
        
        EndPlayerTurn();
    }

    private IEnumerator ExecuteAttackPhase(System.Action onComplete)
    {
        Debug.Log("Starting Attack Phase...");

        if (currentTurn == TurnState.PlayerTurn)
        {
            Debug.Log("🔹 Player Attacks Enemy");
            yield return StartCoroutine(ProcessAttacks(boardManager.playerSlots, boardManager.enemyActiveSlots, "Player"));
        }
        else if (currentTurn == TurnState.EnemyTurn)
        {
            Debug.Log("🔹 Enemy Attacks Player");
            yield return StartCoroutine(ProcessAttacks(boardManager.enemyActiveSlots, boardManager.playerSlots, "Enemy"));
        }

        Debug.Log("Attack Phase Complete.");
        onComplete?.Invoke(); // Calls next turn transition (e.g., HandleEnemyTurn)
    }


    private IEnumerator ProcessAttacks(List<GameObject> attackers, List<GameObject> defenders, string attackerType)
    {
        Debug.Log("PA1");
        bool isPlayerTurn = attackerType == "Player";

        for (int i = 0; i < attackers.Count; i++)
        {
            GameObject attackerSlot = attackers[i];
            CardInstance attacker = null;

            CardViz attackerCardViz = attackerSlot.GetComponentInChildren<CardViz>();

            if (attackerCardViz == null)
            {
                Debug.LogWarning($"⚠️ No CardViz found in attacker slot: {attackerSlot.name}");
                continue;
            }

            attacker = attackerCardViz.GetCardInstance();
            if (attacker == null)
            {
                Debug.LogWarning($"⚠️ CardInstance is NULL for {attackerSlot.name}!");
                continue;
            }

            Debug.Log($"✅ Attacker Found: {attacker.cardData.cardName} (Slot {i})");

            // ✅ Change to List<CardInstance> for Boss targeting
            List<CardInstance> targets = new List<CardInstance>();

            // Check if it's a Boss or Normal card
            if (attacker.cardData.dinoType == "Boss")
            {
                targets = boardManager.FindBossAttackTarget(attacker, defenders);
            }
            else
            {
                // ✅ Single target for normal cards
                CardInstance target = boardManager.FindNormalAttackTarget(attacker, defenders, i, isPlayerTurn);
                if (target != null)
                {
                    targets.Add(target); // Add single target to list for consistency
                }
            }

            if (targets.Count == 0)
            {
                Debug.Log($"⚠️ No valid target for {attacker.cardData.cardName} in slot {i}. Skipping attack.");
                continue;
            }

            // ✅ Loop through all targets and attack each one
            foreach (var target in targets)
            {
                Debug.Log($"🎯 {attacker.cardData.cardName} attacks {target.cardData.cardName}!");

                if (attacker.cardData.dinoType == "Boss")
                {
                    yield return StartCoroutine(HandleBossAttack(attacker, defenders, attackerType));
                }
                else
                {
                    yield return StartCoroutine(HandleNormalAttack(attacker, defenders, i, attackerType));
                }

                yield return new WaitForSeconds(attackDelay);
            }
        }
        Debug.Log("PA3");
    }


private IEnumerator HandleNormalAttack(CardInstance attacker, List<GameObject> defenders, int slotIndex, string attackerType)
    {
        bool isPlayerAttacking = (attackerType == "Player");

        // ✅ Clear target state to avoid overlap
        CardInstance target = null;

        // ✅ Use the new FindNormalAttackTarget() method
        target = boardManager.FindNormalAttackTarget(attacker, defenders, slotIndex, isPlayerAttacking);

        if (target != null)
        {
            // ✅ Calculate Damage with special rule for Player attacking Boss
            int attackDamage = (attacker.cardData.dinoType == "Attacker") ? attacker.currentDamage + 1 : attacker.currentDamage;

            if (isPlayerAttacking && target.cardData.dinoType == "Boss")
            {
                attackDamage -= 1; // ✅ Player deals 1 less damage to Boss
                if (attackDamage < 0) attackDamage = 0; // ✅ No negative damage
                Debug.Log($"[{attackerType} Turn] {attacker.cardData.cardName} attacks {target.cardData.cardName} (Boss) for {attackDamage} damage (Reduced by 1)!");
            }
            else
            {
                Debug.Log($"[{attackerType} Turn] {attacker.cardData.cardName} attacks {target.cardData.cardName} for {attackDamage} damage!");
            }

            target.TakeDamage(attackDamage);

            // ✅ Clear target state after attack
            target = null;
        }
        else
        {
            Debug.Log($"[{attackerType} Turn] {attacker.cardData.cardName} has no valid target.");
        }

        yield return null;
    }

private IEnumerator HandleBossAttack(CardInstance boss, List<GameObject> defenders, string attackerType)
{
    bool isPlayerTurn = (attackerType == "Player");

    // ✅ Use the updated FindBossAttackTarget() method
    List<CardInstance> targets = boardManager.FindBossAttackTarget(boss, defenders);

    if (targets.Count > 0)
    {
        // ✅ Boss attacks each target found
        foreach (var target in targets)
        {
            Debug.Log($"[{attackerType} BOSS] {boss.cardData.cardName} attacks {target.cardData.cardName}!");

            // ✅ Boss Damage Calculation (No reduction, normal damage)
            int bossDamage = boss.currentDamage;
            target.TakeDamage(bossDamage);

            yield return new WaitForSeconds(0.5f); // Add delay between attacks for better visualization
        }
    }
    else
    {
        // ✅ If no targets, Boss attacks the player directly (handled in FindBossAttackTarget)
        // Debug.Log($"[{attackerType} BOSS] {boss.cardData.cardName} attacks the player directly!");
    }

    yield return null;
}

public int GetCurrentTurn()
{
    return turnCounter;
}

private void UpdateTurnDisplay()
{
    if (turnCountText != null)
    {
        if (currentTurn == TurnState.PlayerTurn)
        {
            turnCountText.text = "Player";
        }
        else if (currentTurn == TurnState.EnemyTurn)
        {
            turnCountText.text = "Enemy";
        }
    }
    else
    {
        Debug.LogWarning("Turn Count Text is not assigned in the Inspector.");
    }
}

 private void HandleBackgroundEffect()
    {
        if (healthManager == null || backgroundRenderer == null)
        {
            Debug.LogError("HealthManager or Background Renderer is missing!");
            return;
        }

        int playerHealth = healthManager.GetPlayerHealth();

        if (playerHealth < 4)
        {
            StartCoroutine(BlinkBackgroundEffect());
        }
    }

    private IEnumerator BlinkBackgroundEffect()
    {
        Color normalColor = Color.white; // White
        Color blinkColor = new Color(1f, 0.57f, 0.53f); // #FF9188
        Color finalColor = new Color(1f, 0.70f, 0.68f); // #FFB2AD

        for (int i = 0; i < 3; i++) // Blink 3 times
        {
            backgroundRenderer.color = blinkColor;
            yield return new WaitForSeconds(0.3f);
            backgroundRenderer.color = normalColor;
            yield return new WaitForSeconds(0.3f);
        }
        backgroundRenderer.color = finalColor; // Set to permanent color after blinking
    }
}
