using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IPointerClickHandler
{
    private bool isOccupied;

    // Handle click event on the card slot
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOccupied) return;

        var playerHand = FindObjectOfType<PlayerHand>();
        if (playerHand != null)
        {
            playerHand.PlaceSelectedCard(gameObject);
            ResetSlotVisual();
        }
    }

    // Highlight the slot with a green color
    public void HighlightSlot() => SetSlotColor(Color.green);

    // Reset the slot's visual to its default state
    public void ResetSlotVisual() => SetSlotColor(Color.white);

    // Check if the slot is currently occupied
    public bool IsOccupied() => isOccupied;

    // Set the occupation status of the slot
    public void SetOccupied(bool occupied) => isOccupied = occupied;

    // Update the slot's color
    private void SetSlotColor(Color color)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}
