using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public List<Card> playerHand = new(); // Cards currently in hand
    public GameObject cardPrefab;
    public Transform handPanel;
    public PlayerDeckManager playerDeckManager;
    public BoardManager boardManager;

    private List<GameObject> instantiatedCards = new();
    private GameObject selectedCard;

    public int maxHandSize = 20; 

    private void Start()
    {
        if (boardManager == null)
        {
            Debug.LogError("BoardManager is not assigned in PlayerHand.");
            return;
        }

        boardManager.OnEngageDeckReady += DrawInitialHand;
    }

    private void OnDestroy()
    {
        if (boardManager != null)
        {
            boardManager.OnEngageDeckReady -= DrawInitialHand;
        }
    }

    public void DrawCard(Card card)
    {
        GameObject newCard = Instantiate(cardPrefab, handPanel);
        CardViz cardViz = newCard.GetComponent<CardViz>();

        if (cardViz != null)
        {
            cardViz.LoadCard(card); // Load the card's visual representation
        }

        CardInteractionHandler interactionHandler = newCard.GetComponent<CardInteractionHandler>();
        if (interactionHandler != null)
        {
            interactionHandler.cardData = card; // Assign the card data to the interaction handler
            interactionHandler.SetCardState(CardInteractionHandler.CardState.InHand);
        }

        instantiatedCards.Add(newCard); // Add the card object to the visual hand
        playerHand.Add(card);          // Add the card to the logical hand
        Debug.Log($"Card drawn: {card.cardName}");
    }

    public void DrawInitialHand()
    {
        DrawCardsByType("Normal", 3);
        DrawCardsByType("Attacker", 1, 4);
        DrawCardsByType("Defender", 1, 4);
    }

    private void DrawCardsByType(string cardType, int count, int maxCost = int.MaxValue)
    {
        for (int i = 0; i < count; i++)
        {
            var card = boardManager.DrawCardFromEngageDeck(cardType, maxCost); // Fetch a card from the engage deck
            if (card != null)
            {
                DrawCard(card); // Call the updated DrawCard method with the card data
            }
            else
            {
                Debug.LogWarning($"No {cardType} card available with cost <= {maxCost}.");
            }
        }
    }

    public void DrawCardFromDeck()
    {
        // Ensure the actual hand size is tracked from instantiated cards
        int currentHandSize = instantiatedCards.Count;

        if (currentHandSize >= maxHandSize)
        {
            Debug.LogWarning("Cannot draw more cards. Hand is full!");
            return;
        }

        // Use the new random draw method
        Card card = boardManager.DrawCardFromEngageDeck();

        if (card == null)
        {
            Debug.LogWarning("No cards left in the Engage Deck to draw!");
            return;
        }

        // Instantiate the new card and add it to the hand
        GameObject newCard = Instantiate(cardPrefab, handPanel);
        CardViz cardViz = newCard.GetComponent<CardViz>();

        if (cardViz != null)
        {
            cardViz.LoadCard(card); // Load card visuals
        }

        CardInteractionHandler interactionHandler = newCard.GetComponent<CardInteractionHandler>();
        if (interactionHandler != null)
        {
            interactionHandler.cardData = card; // Assign card data
            interactionHandler.SetCardState(CardInteractionHandler.CardState.InHand);
        }

        // Add the new card to the instantiatedCards list
        instantiatedCards.Add(newCard);

        // Add to the logical `playerHand` list
        playerHand.Add(card);

        Debug.Log($"Card drawn: {card.cardName}");
    }

//region PlacingCard
    public void SelectCard(GameObject card)
    {
        if (selectedCard == card)
        {
            DeselectCard();
            return;
        }

        if (selectedCard != null)
        {
            DeselectCard();
        }

        selectedCard = card;
        var interactionHandler = card.GetComponent<CardInteractionHandler>();
        interactionHandler?.ScaleUpForSelection();

        var cardData = card.GetComponent<CardInteractionHandler>()?.cardData;
        if (cardData == null)
        {
            Debug.LogError("CardData is null for the selected card.");
            return;
        }

        if (cardData.cost == 0)
        {
            boardManager.HighlightEmptySlots();
        }
        else
        {
            boardManager.EnableSacrificePhase(cardData.cost, card);
        }
    }

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

            if (cardData.cost > 0 && boardManager != null && !boardManager.AreSacrificesComplete())
            {
                Debug.LogWarning("Cannot place card. Sacrifices not complete.");
                return;
            }

            // Place the card visually
            slot.GetComponent<CardSlot>().SetOccupied(true);
            selectedCard.transform.SetParent(slot.transform);
            selectedCard.transform.localPosition = Vector3.zero;
            selectedCard.transform.localScale = Vector3.one;

            selectedCard.GetComponent<CardInteractionHandler>().SetCardState(CardInteractionHandler.CardState.OnBoard);

            // Remove the card from the hand tracking
            instantiatedCards.Remove(selectedCard);
            playerHand.Remove(cardData);

            selectedCard = null;

            // Handle board manager state
            boardManager.DestroySacrificedCards();
            boardManager.DisableSacrificePhase();

            Debug.Log("Card placed successfully.");
        }
        else
        {
            Debug.LogWarning("No card is selected for placement.");
        }
    }

    private void DeselectCard()
    {
        var interactionHandler = selectedCard?.GetComponent<CardInteractionHandler>();
        if (interactionHandler != null)
        {
            interactionHandler.ResetToDefault();
            interactionHandler.EnableHoverEffect(); // Re-enable hover effects
        }
        selectedCard = null;
    }
//endregion
}