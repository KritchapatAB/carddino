using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IPointerClickHandler
{
    private PlayerHand playerHand;
    private GameObject placedCard = null;

    void Start()
    {
        playerHand = FindObjectOfType<PlayerHand>();
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
        // Placeholder for enabling placement mode (if needed in the future)
    }

    public void DisablePlacementMode()
    {
        // Placeholder for disabling placement mode (if needed in the future)
    }

    void PlaceCard(GameObject card)
    {
        placedCard = card;
        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;
        card.transform.localScale = Vector3.one;
    }
}
