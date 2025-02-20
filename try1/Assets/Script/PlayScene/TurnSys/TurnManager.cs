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

    for (int i = 0; i < attackers.Count; i++)
    {
        GameObject attackerSlot = attackers[i]; // ✅ The slot where the attacker is
        GameObject targetSlot = defenders[i];  // ✅ The corresponding enemy slot

        bool isPlayerTurn = attackerType == "Player";
        CardInstance attacker = null;

        // ✅ Find the attacking card inside the attacker's slot (Fixed logic)
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
        CardInstance target = null;
        CardViz defenderCardViz = null;

        foreach (Transform child in targetSlot.transform)
        {
            defenderCardViz = child.GetComponent<CardViz>();
            if (defenderCardViz != null) break; // Found the card, exit loop
        }

        if (defenderCardViz != null)
        {
            target = defenderCardViz.GetCardInstance();
        }

        if (target == null)
        {
            Debug.Log($"⚠️ No valid target in slot {i}. Skipping attack.");
            continue;
        }

        Debug.Log($"🎯 {attackerType} Attacks {target.cardData.cardName} in Slot {i}");

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

        if (defenders.Count > 0)
        {
            // ✅ Ensure Boss targets correctly based on turn
            CardInstance target = boardManager.FindAttackTarget(boss, defenders, 0, isPlayerTurn);
            int bossDamage = boss.currentDamage * 2; // ✅ Boss does double damage

            Debug.Log($"[{attackerType} BOSS] {boss.cardData.cardName} attacks {target.cardData.cardName} for {bossDamage} damage!");
            target.TakeDamage(bossDamage);
        }
        else
        {
            Debug.Log($"[{attackerType} BOSS] {boss.cardData.cardName} damages all Player Cards!");
        }

        yield return null;
    }


}
