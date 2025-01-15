using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IPointerClickHandler
{
    private bool isOccupied;
    [SerializeField] private GameObject placeableIcon; // Assign the PlaceableIcon in the Inspector

    private PlayerHand playerHand;

    private void Start()
    {
        playerHand = FindObjectOfType<PlayerHand>();
        UpdatePlaceableIcon();
    }

    private void Update()
    {
        // Continuously check the slot's eligibility when a card is selected
        UpdatePlaceableIcon();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOccupied) return;

        if (playerHand != null && playerHand.HasSelectedCard())
        {
            var selectedCard = playerHand.GetSelectedCard(); // Assume this method fetches the selected card
            if (CanPlaceCard(selectedCard))
            {
                playerHand.PlaceSelectedCard(gameObject); // Place the card
                SetOccupied(true); // Mark the slot as occupied
            }
        }
    }

    public bool IsOccupied() => isOccupied;

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
        HidePlaceableUI();
    }

    // Check if the selected card can be placed in this slot
    private bool CanPlaceCard(GameObject selectedCard)
    {
        if (selectedCard == null || isOccupied)
        {
            return false;
        }

        // Add more conditions if necessary (e.g., checking card type, field-specific rules)
        return true;
    }

    // Update the visibility of the PlaceableIcon
    private void UpdatePlaceableIcon()
    {
        if (placeableIcon != null && playerHand != null)
        {
            var selectedCard = playerHand.GetSelectedCard();
            bool canPlace = selectedCard != null && CanPlaceCard(selectedCard);
            placeableIcon.SetActive(canPlace);
        }
    }
    
    public void ShowPlaceableUI()
    {
        var placeableUI = transform.Find("PlaceableUI");
        if (placeableUI != null)
        {
            placeableUI.gameObject.SetActive(true);
        }
    }

    public void HidePlaceableUI()
    {
        var placeableUI = transform.Find("PlaceableUI");
        if (placeableUI != null)
        {
            placeableUI.gameObject.SetActive(false);
        }
    }
}
