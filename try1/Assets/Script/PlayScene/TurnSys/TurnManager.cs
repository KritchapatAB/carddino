using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum TurnState { PlayerTurn, EnemyTurn }
    public TurnState currentTurn = TurnState.EnemyTurn;

    private BoardManager boardManager;
    private EnemyManager enemyManager;
    private PlayerHand playerHand;

    private int turnCounter = 0; // ✅ Ensure turnCounter is initialized
    public float enemyTurnDelay = 2.0f;
    private bool hasDrawnCardThisTurn = false;

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
            enemyManager.Initialize(); // ✅ Ensure EnemyManager is initialized
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
        currentTurn = TurnState.EnemyTurn;
        Debug.Log($"Turn {turnCounter}: Player's turn ends.");

        boardManager.DisableSacrificePhase();
        boardManager.DisablePlayerControls();
        playerHand.TogglePlayerInteractions(false);

        turnCounter++; 
        StartCoroutine(HandleEnemyTurn());
    }

    private IEnumerator HandleEnemyTurn()
    {
        Debug.Log($"Turn {turnCounter}: Enemy's turn starts.");
        yield return new WaitForSeconds(enemyTurnDelay);

        if (enemyManager != null)
        {
            enemyManager.StartTurn(turnCounter); // ✅ Pass turnCounter to fix error
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

        turnCounter++; // ✅ Increment turn count AFTER enemy's turn
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
