using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public PlayerDeckManager playerDeckManager;
    public RandomCard randomCard; // Reference to RandomCard component
    public int handSize = 5;
    public GameObject cardPrefab;
    public Transform handPanel;

    private List<GameObject> instantiatedCards = new List<GameObject>();
    private GameObject selectedCard = null; // Tracks the currently selected card
    private BoardManager boardManager;

    void Start()
    {
        // Ensure all required components are assigned
        if (playerDeckManager == null || randomCard == null)
        {
            Debug.LogError("PlayerDeckManager or RandomCard component is missing. Please assign them in the inspector.");
            return;
        }

        boardManager = FindObjectOfType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager not found in the scene. Make sure it is added and active.");
            return;
        }

        // Initialize deck and draw cards
        playerDeckManager.InitializePlayerDeck();
        DrawInitialHand();
    }

    void DrawInitialHand()
    {
        for (int i = 0; i < 3; i++) DrawSpecificCard("Normal");
        DrawSpecificCard("Attacker", 4);
        DrawSpecificCard("Defender", 4);
    }

    void DrawSpecificCard(string dinoType, int maxCost = int.MaxValue)
    {
        var filteredCards = playerDeckManager.playerDeck.FindAll(card => card.dinoType == dinoType && card.cost <= maxCost);

        if (filteredCards.Count > 0)
        {
            Card drawnCard = DrawAndRemoveRandomCard(filteredCards);
            if (drawnCard != null)
            {
                playerHand.Add(drawnCard);
                playerDeckManager.RemoveCardFromDeck(drawnCard);
                DisplayCard(drawnCard);
            }
        }
        else
        {
            Debug.LogWarning($"No cards found with dinoType '{dinoType}' and cost <= {maxCost}.");
        }
    }

    Card DrawAndRemoveRandomCard(List<Card> cards)
    {
        return randomCard != null ? randomCard.DrawAndRemoveRandomCard(cards) : null;
    }

    void DisplayCard(Card card)
    {
        GameObject newCard = Instantiate(cardPrefab, handPanel);
        CardViz cardViz = newCard.GetComponent<CardViz>();

        if (cardViz != null)
        {
            cardViz.LoadCard(card);
            Debug.Log($"Displayed card: {card.cardName}");
        }
        else
        {
            Debug.LogError("CardViz component missing on card prefab.");
        }

        instantiatedCards.Add(newCard);
    }

    public void OnCardClick(GameObject clickedCard)
    {
        // Check if the card is already selected
        if (selectedCard == clickedCard)
        {
            ResetAllCards();
            return;
        }

        // Select a new card
        selectedCard = clickedCard;
        HighlightSelectedCard(clickedCard);
        Debug.Log($"Card selected: {clickedCard.name}");

        // Enable board slots for placement
        if (boardManager != null)
        {
            boardManager.EnableBoardForPlacement(selectedCard);
        }
    }

    void HighlightSelectedCard(GameObject clickedCard)
    {
        foreach (GameObject cardObject in instantiatedCards)
        {
            if (cardObject == clickedCard)
            {
                cardObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); // Highlight selected card
            }
            else
            {
                cardObject.SetActive(false); // Hide other cards
            }
        }
    }

    public void ResetAllCards()
    {
        selectedCard = null;

        foreach (GameObject cardObject in instantiatedCards)
        {
            cardObject.SetActive(true); // Make all cards visible again
            cardObject.transform.localScale = Vector3.one; // Reset scale
        }

        Debug.Log("Card selection reset.");

        // Disable all board slots
        if (boardManager != null)
        {
            boardManager.DisableAllSlots();
        }
    }

    public void OnCardHover(GameObject hoveredCard)
    {
        // Only scale up hovered card if no card is selected
        if (selectedCard != null && selectedCard != hoveredCard) return;

        foreach (GameObject cardObject in instantiatedCards)
        {
            cardObject.transform.localScale = cardObject == hoveredCard
                ? new Vector3(1.2f, 1.2f, 1.2f) // Hover scale
                : new Vector3(0.8f, 0.8f, 0.8f); // Default scale
        }
    }

    public void OnCardHoverExit(GameObject hoveredCard)
    {
        // Reset scale unless a card is selected
        if (selectedCard != null && selectedCard != hoveredCard) return;

        foreach (GameObject cardObject in instantiatedCards)
        {
            cardObject.transform.localScale = selectedCard == cardObject
                ? new Vector3(1.2f, 1.2f, 1.2f) // Maintain selected scale
                : Vector3.one; // Default scale
        }
    }

    public GameObject GetSelectedCard()
    {
        return selectedCard;
    }
}
