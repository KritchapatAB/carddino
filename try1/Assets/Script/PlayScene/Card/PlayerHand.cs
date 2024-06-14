using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHand : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public CardDatabase cardDatabase;
    public RandomCard randomCard;
    public int handSize = 5;

    public GameObject cardPrefab; // Drag your card prefab here
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
        for (int i = 0; i < handSize; i++)
        {
            DrawRandomCard();
        }
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

    void DisplayCard(Card card)
    {
        GameObject newCard = Instantiate(cardPrefab, handPanel);

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
