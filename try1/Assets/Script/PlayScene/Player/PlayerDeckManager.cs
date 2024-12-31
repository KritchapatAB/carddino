using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerDeckManager : MonoBehaviour
{
    public List<Card> playerDeck = new List<Card>();
    public CardDatabase cardDatabase;
    public RandomCard randomCard;

    public event Action OnDeckInitialized;
    
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
        List<Card> availableCards = new List<Card>(cardDatabase.cards);
        playerDeck.Clear();

        if (availableCards.Count == 0)
        {
            Debug.LogWarning("No cards available in the card database!");
            return;
        }

        // Populate the deck with cards (allow duplicates)
        while (playerDeck.Count < 30)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableCards.Count); // Explicitly use UnityEngine.Random
            playerDeck.Add(availableCards[randomIndex]);
        }

        Debug.Log("Player deck initialized.");

        // Notify listeners that the deck is ready
        OnDeckInitialized?.Invoke();
    }

    Card DrawRandomCardFromList(List<Card> cards)
    {
        if (cards.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, cards.Count); // Explicitly use UnityEngine.Random
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
