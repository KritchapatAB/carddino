using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public List<Card> playerHand = new();
    public GameObject cardPrefab;
    public Transform handPanel;
    public PlayerDeckManager playerDeckManager;
    public BoardManager boardManager;

    private List<GameObject> instantiatedCards = new();
    private GameObject selectedCard;

    void Start()
    {
        if (playerDeckManager == null)
        {
            Debug.LogError("PlayerDeckManager is not assigned in PlayerHand.");
            return;
        }

        playerDeckManager.OnDeckInitialized += DrawInitialHand;
    }

    void OnDestroy()
    {
        if (playerDeckManager != null)
        {
            playerDeckManager.OnDeckInitialized -= DrawInitialHand;
        }
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
            var card = playerDeckManager.DrawRandomCardByTypeAndCost(cardType, maxCost);
            if (card != null)
            {
                playerHand.Add(card);
                DrawCard(card);
            }
            else
            {
                Debug.LogWarning($"Failed to draw a card of type {cardType} with cost <= {maxCost}.");
            }
        }
    }

    private void DrawCard(Card card)
    {
        var newCard = Instantiate(cardPrefab, handPanel);
        var cardViz = newCard.GetComponent<CardViz>();
        cardViz?.LoadCard(card);

        var interactionHandler = newCard.GetComponent<CardInteractionHandler>();
        if (interactionHandler != null)
        {
            interactionHandler.cardData = card;
            interactionHandler.SetCardState(CardInteractionHandler.CardState.InHand);
        }

        instantiatedCards.Add(newCard);
    }

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

            slot.GetComponent<CardSlot>().SetOccupied(true); // Mark slot as occupied
            selectedCard.transform.SetParent(slot.transform);
            selectedCard.transform.localPosition = Vector3.zero;
            selectedCard.transform.localScale = Vector3.one;

            selectedCard.GetComponent<CardInteractionHandler>().SetCardState(CardInteractionHandler.CardState.OnBoard);
            selectedCard = null; // Clear selection

            boardManager.DestroySacrificedCards(); // Remove sacrificed cards
            boardManager.DisableSacrificePhase(); // End the sacrifice phase
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
}

