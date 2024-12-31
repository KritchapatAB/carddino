using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IPointerClickHandler
{
    private bool isOccupied = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerHand playerHand = FindObjectOfType<PlayerHand>();
        if (playerHand != null && !isOccupied) // Ensure the slot is not occupied
        {
            playerHand.PlaceSelectedCard(gameObject);
            ResetSlot(); // Reset visual feedback after placement
        }
    }

    public void HighlightSlot()
    {
        GetComponent<SpriteRenderer>().color = Color.green; // Add highlight
    }

    public void ResetSlot()
    {
        GetComponent<SpriteRenderer>().color = Color.white; // Reset highlight
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
}
