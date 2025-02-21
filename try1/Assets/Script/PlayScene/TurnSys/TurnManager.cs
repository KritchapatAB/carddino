using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
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
        currentTurn = TurnState.PlayerTurn;
        Debug.Log($"Turn {turnCounter}: Player's turn starts.");
        hasDrawnCardThisTurn = false;

        boardManager.EnablePlayerControls();
        playerHand.TogglePlayerInteractions(true);
    }

    public void EndPlayerTurn()
    {
        Debug.Log($"Turn {turnCounter}: Player's turn ends.");

        boardManager.DisableSacrificePhase();
        boardManager.DisablePlayerControls();
        playerHand.TogglePlayerInteractions(false);

        // ✅ Correct: First process attack phase, THEN switch turn
        StartCoroutine(ExecuteAttackPhase(() =>
        {
            // ✅ Now switch to Enemy Turn AFTER combat
            currentTurn = TurnState.EnemyTurn;
            turnCounter++;
            StartCoroutine(HandleEnemyTurn());
        }));
    }

    private IEnumerator HandleEnemyTurn()
    {
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
        EndEnemyTurn();
    }

    public void EndEnemyTurn()
    {
        Debug.Log($"Turn {turnCounter}: Enemy's turn ends.");

        boardManager.MoveEnemyCardsToActiveArea();
        boardManager.enemyManager.ReturnHandToDeck();

        // ✅ Correct: First process attack phase, THEN switch turn
        StartCoroutine(ExecuteAttackPhase(() =>
        {
            // ✅ Now switch to Player Turn AFTER combat
            currentTurn = TurnState.PlayerTurn;
            turnCounter++;
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
    }

    public void OnAttackButtonClicked()
    {
        Debug.Log("Attack button clicked. Ending player's turn.");
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

        // ✅ Find the attacking card inside the attacker's slot
        CardViz attackerCardViz = null;
        foreach (Transform child in attackerSlot.transform)
        {
            attackerCardViz = child.GetComponent<CardViz>();
            if (attackerCardViz != null) break; // Found the card, exit loop
        }

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

        // ✅ Find the correct **defending card** based on turn
        CardInstance target = boardManager.FindAttackTarget(attacker, defenders, i, isPlayerTurn);

        if (target == null)
        {
            Debug.Log($"⚠️ No valid target for {attacker.cardData.cardName} in slot {i}. Skipping attack.");
            continue;
        }

        Debug.Log($"🎯 {attacker.cardData.cardName} attacks {target.cardData.cardName}!");

        // ✅ Execute Attack
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
    Debug.Log("PA3");
}




    private IEnumerator HandleNormalAttack(CardInstance attacker, List<GameObject> defenders, int slotIndex, string attackerType)
    {
        bool isPlayerAttacking = (attackerType == "Player");

        // ✅ Clear target state to avoid overlap
        CardInstance target = null;

        // ✅ Correctly targets Player or Enemy based on turn
        target = boardManager.FindAttackTarget(attacker, defenders, slotIndex, isPlayerAttacking);

        if (target != null)
        {
            int attackDamage = (attacker.cardData.dinoType == "Attacker") ? attacker.currentDamage + 1 : attacker.currentDamage;
            Debug.Log($"[{attackerType} Turn] {attacker.cardData.cardName} attacks {target.cardData.cardName} for {attackDamage} damage!");
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
    int bossDamage = boss.currentDamage * 2; // ✅ Boss does double damage
    bool targetFound = false;

    // Step 1: Prioritize Defenders
    if (defenders.Count > 0)
    {
        // ✅ Target the leftmost Defender
        CardInstance target = boardManager.FindDefenderTarget(defenders, isPlayerTurn);
        if (target != null)
        {
            Debug.Log($"[{attackerType} BOSS] {boss.cardData.cardName} attacks Defender {target.cardData.cardName} for {bossDamage} damage!");
            target.TakeDamage(bossDamage);
            targetFound = true;
        }
    }

    // Step 2: Closest Card in Same Column
    if (!targetFound)
    {
        // ✅ Find closest card in the same column as the Boss
        int bossSlotIndex = boardManager.GetSlotIndex(boss);
        CardInstance target = boardManager.FindFrontCardTarget(defenders, bossSlotIndex, isPlayerTurn);

        if (target != null)
        {
            Debug.Log($"[{attackerType} BOSS] {boss.cardData.cardName} attacks {target.cardData.cardName} for {bossDamage} damage!");
            target.TakeDamage(bossDamage);
            targetFound = true;
        }
    }

    // Step 3: Attack All Cards (if no Defenders and no cards in the same column)
    if (!targetFound)
    {
        Debug.Log($"[{attackerType} BOSS] {boss.cardData.cardName} attacks ALL cards on the opposing field!");
        List<GameObject> targetSlots = isPlayerTurn ? boardManager.enemyActiveSlots : boardManager.playerSlots;

        foreach (GameObject slot in targetSlots)
        {
            CardViz cardViz = null;
            foreach (Transform child in slot.transform)
            {
                cardViz = child.GetComponent<CardViz>();
                if (cardViz != null) break;
            }

            if (cardViz != null)
            {
                CardInstance target = cardViz.GetCardInstance();
                Debug.Log($"[{attackerType} BOSS] {boss.cardData.cardName} deals {bossDamage} damage to {target.cardData.cardName}!");
                target.TakeDamage(bossDamage);
            }
        }
    }

    yield return null;
}


}
