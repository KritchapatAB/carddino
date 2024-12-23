using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image slotHighlight;
    private PlayerHand playerHand;
    private GameObject placedCard = null;

    void Start()
    {
        playerHand = FindObjectOfType<PlayerHand>();
        slotHighlight = GetComponent<Image>();
        if (slotHighlight == null)
        {
            Debug.LogError("CardSlot is missing an Image component for highlighting!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (placedCard == null && slotHighlight != null)
        {
            slotHighlight.color = Color.green; // Highlight the slot
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (slotHighlight != null)
        {
            slotHighlight.color = Color.white; // Reset highlight
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (placedCard != null)
        {
            Debug.Log("Slot already occupied!");
            return;
        }

        if (playerHand != null)
        {
            GameObject selectedCard = playerHand.GetSelectedCard();
            if (selectedCard != null)
            {
                PlaceCard(selectedCard);
                playerHand.ResetAllCards();
                Debug.Log("Card placed on the slot.");
            }
            else
            {
                Debug.Log("No card selected. Please select a card before clicking a slot.");
            }
        }
    }

    public void EnablePlacementMode()
    {
        if (placedCard == null && slotHighlight != null)
        {
            slotHighlight.color = Color.green; // Highlight the slot for placement
        }
    }

    public void DisablePlacementMode()
    {
        if (slotHighlight != null)
        {
            slotHighlight.color = Color.white; // Reset highlight
        }
    }

    void PlaceCard(GameObject card)
    {
        placedCard = card;
        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;
        card.transform.localScale = Vector3.one;

        if (slotHighlight != null)
        {
            slotHighlight.color = Color.white; // Reset the highlight
        }
    }
}
