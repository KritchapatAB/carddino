using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>(); // Logical state of the player's hand
    public GameObject cardPrefab;                   // Prefab for card UI
    public Transform handPanel;                     // Parent container for card UI in the hand
    public PlayerDeckManager playerDeckManager;     // Reference to the PlayerDeckManager

    private List<GameObject> instantiatedCards = new List<GameObject>(); // Instantiated card objects

    private GameObject selectedCard; // The currently selected card
    private Vector3 originalScale; // Original scale of the selected card
    public BoardManager boardManager; // Reference to the BoardManager script

    void Start()
    {
        if (playerDeckManager != null)
        {
            // Subscribe to the OnDeckInitialized event
            playerDeckManager.OnDeckInitialized += DrawInitialHand;
        }
        else
        {
            Debug.LogError("PlayerDeckManager is not assigned in PlayerHand.");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (playerDeckManager != null)
        {
            playerDeckManager.OnDeckInitialized -= DrawInitialHand;
        }
    }

    //region starthand
    public void DrawInitialHand()
    {
        // Draw initial cards
        DrawCardsByType("Normal", 3);    // Draw 3 Normal cards
        DrawCardsByType("Attacker", 1, 4); // Draw 1 Attacker card with max cost of 4
        DrawCardsByType("Defender", 1, 4); // Draw 1 Defender card with max cost of 4
    }

    private void DrawCardsByType(string dinoType, int count, int maxCost = int.MaxValue)
    {
        for (int i = 0; i < count; i++)
        {
            var card = playerDeckManager.DrawRandomCardByTypeAndCost(dinoType, maxCost);

            if (card != null)
            {
                playerHand.Add(card);  // Add to the player's hand list
                DrawCard(card);        // Instantiate and display the card
            }
            else
            {
                Debug.LogWarning($"Failed to draw a card of type {dinoType} with cost <= {maxCost}.");
            }
        }
    }

    public void DrawCard(Card card)
    {
        GameObject newCard = Instantiate(cardPrefab, handPanel); // Create the card in the hand
        CardViz cardViz = newCard.GetComponent<CardViz>();

        if (cardViz != null)
        {
            cardViz.LoadCard(card); // Populate the visual details from Card data
        }

        CardInteractionHandler interactionHandler = newCard.GetComponent<CardInteractionHandler>();
        if (interactionHandler != null)
        {
            interactionHandler.cardData = card; // Assign the CardData to the interaction handler
            interactionHandler.SetCardState(CardInteractionHandler.CardState.InHand);
        }

        instantiatedCards.Add(newCard); // Add the card to the list of instantiated cards
    }

    //endregion

    public void PlaceSelectedCard(GameObject slot)
    {
        if (selectedCard != null)
        {
            Card cardData = selectedCard.GetComponent<CardInteractionHandler>()?.cardData;

            if (cardData == null)
            {
                Debug.LogError("CardData is null for the selected card.");
                return;
            }

            // Check if sacrifices are required and complete
            if (cardData.cost > 0 && boardManager != null && !boardManager.AreSacrificesComplete())
            {
                Debug.LogWarning("Cannot place card. Sacrifices not complete.");
                return;
            }

            // Place the card
            slot.GetComponent<CardSlot>().SetOccupied(true); // Mark the slot as occupied
            selectedCard.transform.SetParent(slot.transform);
            selectedCard.transform.localPosition = Vector3.zero;

            // Reset the card's scale to its original size
            selectedCard.transform.localScale = Vector3.one;

            selectedCard.GetComponent<CardInteractionHandler>().SetCardState(CardInteractionHandler.CardState.OnBoard);
            selectedCard = null; // Clear the selection
        }
    }

    public void SelectCard(GameObject card)
{
    // If the card is already selected, deselect it
    if (selectedCard == card)
    {
        Debug.Log("Card already selected. Deselecting it.");
        DeselectCard();
        return;
    }

    // If another card is selected, deselect it first
    if (selectedCard != null)
    {
        Debug.Log("Switching selection. Deselecting current card.");
        DeselectCard();
    }

    // Set the new card as the selected card
    selectedCard = card;

    // Update visuals for the selected card
    CardInteractionHandler interactionHandler = selectedCard.GetComponent<CardInteractionHandler>();
    if (interactionHandler != null)
    {
        interactionHandler.DisableHoverEffect(); // Disable hover effects
        interactionHandler.ScaleUpForSelection(); // Scale up for selection
    }

    Card cardData = selectedCard.GetComponent<CardInteractionHandler>()?.cardData;

    if (cardData == null)
    {
        Debug.LogError("CardData is null for the selected card.");
        return;
    }

    // Check the card's cost and trigger the appropriate phase
    if (cardData.cost == 0)
    {
        Debug.Log("Selected card cost is 0, enabling direct placement.");
        boardManager?.HighlightEmptySlots();
    }
    else if (cardData.cost > 0)
    {
        Debug.Log($"Selected card cost is {cardData.cost}, starting sacrifice phase.");
        boardManager?.EnableSacrificePhase(cardData.cost, selectedCard);
    }
}



private void DeselectCard()
{
    if (selectedCard != null)
    {
        Debug.Log($"Deselecting card: {selectedCard.name}");

        // Reset the card's visual state
        CardInteractionHandler interactionHandler = selectedCard.GetComponent<CardInteractionHandler>();
        if (interactionHandler != null)
        {
            interactionHandler.EnableHoverEffect(); // Re-enable hover effects
            interactionHandler.Deselect(); // Reset the visual state
        }

        // Clear the selected card
        selectedCard = null;
    }
}

}
