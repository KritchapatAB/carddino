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
            var selectedCard = playerHand.GetSelectedCard();
            if (CanPlaceCard(selectedCard, logWarnings: true)) // Allow logs for placement attempts
            {
                playerHand.PlaceSelectedCard(gameObject); // Place the card
                SetOccupied(true); // Mark the slot as occupied
            }
            else
            {
                Debug.LogWarning("Cannot place card. Conditions not met.");
            }
        }
    }

    public bool IsOccupied() => isOccupied;

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
        HidePlaceableUI();
    }

    private bool CanPlaceCard(GameObject selectedCard, bool logWarnings = true)
{
    if (selectedCard == null || isOccupied) return false;

    // Validate if the card can be placed
    var cardData = selectedCard.GetComponent<CardInteractionHandler>()?.cardData;
    if (cardData == null)
    {
        if (logWarnings) Debug.LogError("CardData is missing or null.");
        return false;
    }

    if (cardData.cost > 0 && !FindObjectOfType<BoardManager>().AreSacrificesComplete())
    {
        if (logWarnings) Debug.LogWarning("Cannot place card. Sacrifices not complete.");
        return false;
    }
    return true;
}


    private void UpdatePlaceableIcon()
    {
        if (placeableIcon != null && playerHand != null)
        {
            var selectedCard = playerHand.GetSelectedCard();
            bool canPlace = selectedCard != null && CanPlaceCard(selectedCard, logWarnings: false); // Suppress logs
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
