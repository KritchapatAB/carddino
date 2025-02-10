using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum TurnState { PlayerTurn, EnemyTurn }
    public TurnState currentTurn = TurnState.EnemyTurn;

    private BoardManager boardManager;
    private EnemyManager enemyManager;  // ✅ NEW: Reference EnemyManager
    private PlayerHand playerHand;


    public float enemyTurnDelay = 2.0f;
    private bool hasDrawnCardThisTurn = false;

    void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
        enemyManager = FindObjectOfType<EnemyManager>();  // ✅ Assign EnemyManager
        playerHand = FindObjectOfType<PlayerHand>();

        if (enemyManager == null)
        {
            Debug.LogError("EnemyManager not found in scene!");
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
        Debug.Log("Player's turn starts.");
        boardManager.MoveEnemyCardsToActiveArea();
        hasDrawnCardThisTurn = false;

        boardManager.EnablePlayerControls();
        playerHand.TogglePlayerInteractions(true);
    }

    public void EndPlayerTurn()
    {
        currentTurn = TurnState.EnemyTurn;
        Debug.Log("Player's turn ends.");

        boardManager.DisableSacrificePhase();
        boardManager.DisablePlayerControls();
        playerHand.TogglePlayerInteractions(false);

        StartCoroutine(HandleEnemyTurn());
    }

    private IEnumerator HandleEnemyTurn()
    {
        Debug.Log("Enemy's turn starts.");
        yield return new WaitForSeconds(enemyTurnDelay);

        if (enemyManager != null)
        {
            enemyManager.StartTurn();  // ✅ This actually makes the enemy AI move
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
        Debug.Log("Enemy's turn ends.");

        boardManager.MoveEnemyCardsToActiveArea(); // ✅ Ensure this runs here

        boardManager.enemyManager.ReturnHandToDeck();
        StartPlayerTurn();
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
}
