using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerHand : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public PlayerDeckManager playerDeckManager;
    public RandomCard randomCard; // Reference to RandomCard component
    public int handSize = 5;
    public GameObject cardPrefab;
    public Transform handPanel;
    private List<GameObject> instantiatedCards = new List<GameObject>();

    private GameObject selectedCard = null; // Track the currently selected card

    void Start()
    {
        if (playerDeckManager == null)
        {
            Debug.LogError("PlayerDeckManager component not found! Please assign it in the inspector.");
            return;
        }

        if (randomCard == null)
        {
            Debug.LogError("RandomCard component not found! Please assign it in the inspector.");
            return;
        }

        playerDeckManager.InitializePlayerDeck();
        DrawInitialHand();
    }

    void DrawInitialHand()
    {
        for (int i = 0; i < 3; i++)
        {
            DrawSpecificCard("Normal");
        }

        DrawSpecificCard("Attacker", 4);
        DrawSpecificCard("Defender", 4);

        Debug.Log("Initial hand drawn. Card count in hand: " + playerHand.Count);
    }

    void DrawSpecificCard(string dinoType, int maxCost = int.MaxValue)
    {
        List<Card> filteredCards = playerDeckManager.playerDeck.FindAll(card => card.dinoType == dinoType && card.cost < maxCost);

        if (filteredCards.Count > 0)
        {
            Card drawnCard = DrawAndRemoveRandomCard(filteredCards);
            if (drawnCard != null)
            {
                playerHand.Add(drawnCard);
                playerDeckManager.RemoveCardFromDeck(drawnCard);
                Debug.Log("Specific card added to hand: " + drawnCard.cardName);
                DisplayCard(drawnCard);
            }
        }
        else
        {
            Debug.LogWarning($"No cards found with dinoType '{dinoType}' and cost less than {maxCost}.");
        }
    }

    Card DrawAndRemoveRandomCard(List<Card> cards)
    {
        if (randomCard != null)
        {
            return randomCard.DrawAndRemoveRandomCard(cards);
        }
        else
        {
            Debug.LogError("RandomCard reference is null!");
            return null;
        }
    }

    void DisplayCard(Card card)
    {
        GameObject newCard = Instantiate(cardPrefab, handPanel);
        if (newCard == null)
        {
            Debug.LogError("Failed to instantiate card prefab.");
            return;
        }

        newCard.name = $"Card_{card.id}_{Random.Range(0, 10000)}";

        CardViz cardViz = newCard.GetComponent<CardViz>();
        if (cardViz != null)
        {
            cardViz.LoadCard(card);
            Debug.Log("Card visualization updated for: " + card.cardName);
        }
        else
        {
            Debug.LogError("CardViz component not found on card prefab.");
        }

        instantiatedCards.Add(newCard);
    }

    public void OnCardHover(GameObject hoveredCard)
    {
        // Only allow hover effects if no card is currently selected
        if (selectedCard != null && selectedCard != hoveredCard)
            return;

        foreach (GameObject cardObject in instantiatedCards)
        {
            if (cardObject == hoveredCard)
            {
                cardObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); // Hover scale
            }
            else if (selectedCard == null)
            {
                cardObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f); // Non-hovered scale
            }
        }
    }

    public void OnCardHoverExit(GameObject hoveredCard)
    {
        // Only reset hover effects if no card is currently selected
        if (selectedCard != null && selectedCard != hoveredCard)
            return;

        foreach (GameObject cardObject in instantiatedCards)
        {
            // Reset to default scale unless it's the selected card
            if (cardObject == selectedCard)
            {
                cardObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); // Maintain selected scale
            }
            else
            {
                cardObject.transform.localScale = Vector3.one; // Default scale
            }
        }
    }

    public void OnCardClick(GameObject clickedCard)
    {
        Debug.Log("Card clicked: " + clickedCard.name);

        // Check if the clicked card is currently selected
        bool isCardSelected = (selectedCard == clickedCard);

        if (isCardSelected)
        {
            // If the card is already selected, reset all cards to their default state
            ResetAllCards();
        }
        else
        {
            // Select the clicked card and hide all others
            selectedCard = clickedCard;
            foreach (GameObject cardObject in instantiatedCards)
            {
                if (cardObject == clickedCard)
                {
                    cardObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); // Highlight the clicked card
                }
                else
                {
                    cardObject.SetActive(false); // Hide other cards
                }
            }
        }
    }

    private void ResetAllCards()
    {
        selectedCard = null;
        foreach (GameObject cardObject in instantiatedCards)
        {
            cardObject.SetActive(true); // Show all cards
            cardObject.transform.localScale = Vector3.one; // Reset scale
        }
    }
}
