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

    public void DrawInitialHand()
    {
        DrawCardsByType("Normal", 3);
        DrawCardsByType("Attacker", 1, 4);
        DrawCardsByType("Defender", 1, 4);
    }

    private void DrawCardsByType(string dinoType, int count, int maxCost = int.MaxValue)
    {
        for (int i = 0; i < count; i++)
        {
            var card = playerDeckManager.DrawRandomCardByTypeAndCost(dinoType, maxCost);
            if (card != null)
            {
                playerHand.Add(card);
                DisplayCard(card);
            }
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
        // Ignore click if the card is not in the player's hand
        if (clickedCard.transform.parent != handPanel) return;

        if (selectedCard == clickedCard)
        {
            ResetAllCards();
            return;
        }

        selectedCard = clickedCard;
        HighlightSelectedCard(clickedCard);
        Debug.Log($"Card selected: {clickedCard.name}");

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
            cardObject.SetActive(true); 
            cardObject.transform.localScale = Vector3.one;
        }

        if (boardManager != null)
        {
            boardManager.DisableAllSlots();
        }

        Debug.Log("Card selection reset.");
    }

    public void OnCardHover(GameObject hoveredCard)
    {
        // Ignore hover if the card is already placed on the board
        if (hoveredCard.transform.parent != handPanel) return;

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
        // Ignore hover exit if the card is already placed on the board
        if (hoveredCard.transform.parent != handPanel) return;

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
