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

     private bool isInteractionEnabled = true;

    public int maxHandSize = 7; 

    private void Start()
    {
        DrawInitialHand();
    }

//region DrawCard
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

    public void TryDrawCard()
    {
        if (!FindObjectOfType<TurnManager>().CanPlayerDrawCard())
        {
            Debug.LogWarning("Cannot draw more cards this turn!");
            return;
        }

        if (playerHand.Count >= maxHandSize)
        {
            Debug.LogWarning("Cannot draw more cards. Hand is full!");
            return;
        }

        // Use the deck to draw a card
        Card drawnCard = boardManager.DrawCardFromEngageDeck();

        if (drawnCard == null)
        {
            Debug.LogWarning("No cards left in the deck to draw!");
            return;
        }

        // Instantiate card into hand
        GameObject newCard = Instantiate(cardPrefab, handPanel);
        CardViz cardViz = newCard.GetComponent<CardViz>();

        if (cardViz != null)
        {
            cardViz.LoadCard(drawnCard);
        }

        CardInteractionHandler interactionHandler = newCard.GetComponent<CardInteractionHandler>();
        if (interactionHandler != null)
        {
            interactionHandler.cardData = drawnCard;
            interactionHandler.SetCardState(CardInteractionHandler.CardState.InHand);
        }

        // Add card to player hand
        instantiatedCards.Add(newCard);
        playerHand.Add(drawnCard);

        Debug.Log($"Player drew card: {drawnCard.cardName}");

        // Notify TurnManager that a card has been drawn
        FindObjectOfType<TurnManager>().NotifyCardDrawn();
    }


//endregion

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


    public void DeselectCard()
    {
        if (selectedCard != null)
        {
            var interactionHandler = selectedCard.GetComponent<CardInteractionHandler>();
            interactionHandler?.ResetToDefault();
            interactionHandler?.EnableHoverEffect(); // Re-enable hover effects
        }
        selectedCard = null;
    }

//endregion

    public void TogglePlayerInteractions(bool enable)
    {
        isInteractionEnabled = enable;

        foreach (var card in instantiatedCards)
        {
            var interactionHandler = card.GetComponent<CardInteractionHandler>();
            if (interactionHandler != null)
            {
                if (enable)
                    interactionHandler.EnableHoverEffect();
                else
                    interactionHandler.DisableHoverEffect();
            }
        }
    }


    public bool HasSelectedCard()
    {
        return selectedCard != null;
    }

    public GameObject GetSelectedCard()
    {
        return selectedCard;
    }

}