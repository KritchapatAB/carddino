using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public CardDatabase cardDatabase;
    public RandomCard randomCard;
    public int handSize = 5;

    public GameObject draggableCardPrefab; // Reference to the draggable card prefab
    public Transform handPanel; // Drag the UI panel for hand here

    void Start()
    {
        if (randomCard == null)
        {
            Debug.LogError("RandomCard component not found! Please assign it in the inspector.");
            return;
        }

        if (cardDatabase == null)
        {
            Debug.LogError("CardDatabase component not found! Please assign it in the inspector.");
            return;
        }

        cardDatabase.OnDatabaseReady += DrawInitialHand;
    }

    void DrawInitialHand()
    {
        // Draw first three "Normal" cards
        for (int i = 0; i < 3; i++)
        {
            DrawSpecificCard(cardDatabase.cards, "Normal");
        }

        // Draw one "Attacker" card with cost less than 4
        DrawSpecificCard(cardDatabase.cards, "Attacker", 4);

        // Draw one "Defender" card with cost less than 4
        DrawSpecificCard(cardDatabase.cards, "Defender", 4);

        Debug.Log("Initial hand drawn. Card count in hand: " + playerHand.Count);
    }

    public void DrawRandomCard()
    {
        if (cardDatabase != null && randomCard != null)
        {
            Card drawnCard = randomCard.DrawAndRemoveRandomCard(cardDatabase.cards);
            if (drawnCard != null)
            {
                playerHand.Add(drawnCard);
                Debug.Log("Card added to hand: " + drawnCard.cardName);
                DisplayCard(drawnCard);
            }
            else
            {
                Debug.LogWarning("No card was drawn from the deck.");
            }
        }
        else
        {
            Debug.LogError("CardDatabase or RandomCard component is missing.");
        }
    }

    void DrawSpecificCard(List<Card> cards, string dinoType, int maxCost = int.MaxValue)
    {
        List<Card> filteredCards = cards.FindAll(card => card.dinoType == dinoType && card.cost < maxCost);
        if (filteredCards.Count > 0)
        {
            Card drawnCard = randomCard.DrawAndRemoveRandomCard(filteredCards);
            if (drawnCard != null)
            {
                playerHand.Add(drawnCard);
                cards.Remove(drawnCard); // Remove from main deck
                Debug.Log("Specific card added to hand: " + drawnCard.cardName);
                DisplayCard(drawnCard);
            }
            else
            {
                Debug.LogWarning("No specific card was drawn from the deck.");
            }
        }
        else
        {
            Debug.LogWarning($"No cards found with dinoType {dinoType} and cost less than {maxCost}.");
        }
    }

    void DisplayCard(Card card)
    {
        GameObject newCard = Instantiate(draggableCardPrefab, handPanel);

        // Debug log to check if the prefab is instantiated
        if (newCard == null)
        {
            Debug.LogError("Failed to instantiate card prefab.");
            return;
        }

        // Use CardViz to load the card data
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
    }
}
