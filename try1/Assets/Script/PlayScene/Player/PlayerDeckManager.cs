using System.Collections.Generic;
using UnityEngine;

public class PlayerDeckManager : MonoBehaviour
{
    public List<Card> playerDeck = new List<Card>();
    public CardDatabase cardDatabase;
    public RandomCard randomCard;

    void Start()
    {
        if (cardDatabase == null || randomCard == null)
        {
            Debug.LogError("CardDatabase or RandomCard is not assigned!");
            return;
        }

        InitializePlayerDeck();
    }

    public void InitializePlayerDeck()
    {
        if (cardDatabase.cards.Count == 0)
        {
            Debug.LogWarning("No cards available in the card database to initialize the player deck.");
            return;
        }

        List<Card> availableCards = new List<Card>(cardDatabase.cards);
        playerDeck.Clear();

        while (playerDeck.Count < 30)
        {
            Card randomCard = DrawRandomCardFromList(availableCards);
            if (randomCard != null)
            {
                playerDeck.Add(randomCard);
            }
            else
            {
                Debug.LogWarning("Failed to draw a random card.");
                break;
            }
        }

        Debug.Log("Player deck initialized.");
    }

    Card DrawRandomCardFromList(List<Card> cards)
    {
        if (cards.Count > 0)
        {
            int randomIndex = Random.Range(0, cards.Count);
            return cards[randomIndex];
        }
        Debug.LogWarning("No cards left in the list!");
        return null;
    }

    public Card DrawRandomCardByTypeAndCost(string dinoType, int maxCost)
    {
        if (randomCard == null)
        {
            Debug.LogError("RandomCard component is not assigned!");
            return null;
        }

        var filteredCards = playerDeck.FindAll(card => card.dinoType == dinoType && card.cost <= maxCost);
        return randomCard.DrawAndRemoveRandomCard(filteredCards);
    }
}
