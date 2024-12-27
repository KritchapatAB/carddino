using UnityEngine; 
using UnityEngine.EventSystems;

public class CardEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
<<<<<<< Updated upstream
    private PlayerHand playerHand;
    private BoardManager boardManager;

    private void Start()
    {
        playerHand = FindObjectOfType<PlayerHand>();
        boardManager = FindObjectOfType<BoardManager>();

        if (playerHand == null)
        {
            Debug.LogError("PlayerHand component not found in the scene.");
        }

        if (boardManager == null)
        {
            Debug.LogError("BoardManager component not found in the scene.");
        }
    }

    private bool IsInPlayerHand()
    {
        // Check if this card's parent is the player's hand panel
        return transform.parent == playerHand.handPanel;
    }

    private bool IsOnBoard()
    {
        // Check if this card's parent is part of the board
        return boardManager != null && boardManager.IsCardOnBoard(transform);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsInPlayerHand() && playerHand != null)
        {
            playerHand.OnCardHover(gameObject); // Handle hover enter for hand cards
        }
        else if (IsOnBoard() && boardManager != null)
        {
            boardManager.OnBoardCardHover(gameObject); // Handle hover enter for onboard cards
        }
=======
    public void OnPointerEnter(PointerEventData eventData)
    {
        FindObjectOfType<PlayerHand>()?.OnCardHover(gameObject);
>>>>>>> Stashed changes
    }

    public void OnPointerExit(PointerEventData eventData)
    {
<<<<<<< Updated upstream
        if (IsInPlayerHand() && playerHand != null)
        {
            playerHand.OnCardHoverExit(gameObject); // Handle hover exit for hand cards
        }
        else if (IsOnBoard() && boardManager != null)
        {
            boardManager.OnBoardCardHoverExit(gameObject); // Handle hover exit for onboard cards
        }
=======
        FindObjectOfType<PlayerHand>()?.OnCardHoverExit(gameObject);
>>>>>>> Stashed changes
    }

    public void OnPointerClick(PointerEventData eventData)
    {
<<<<<<< Updated upstream
        if (IsInPlayerHand() && playerHand != null)
        {
            playerHand.OnCardClick(gameObject); // Handle click for hand cards
        }
        else if (IsOnBoard() && boardManager != null)
        {
            boardManager.OnBoardCardClick(gameObject); // Handle click for onboard cards
        }
=======
        FindObjectOfType<PlayerHand>()?.OnCardClick(gameObject);
>>>>>>> Stashed changes
    }
}
