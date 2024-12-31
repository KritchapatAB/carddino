using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerDeckManager : MonoBehaviour
{
    public List<Card> playerDeck = new();
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

    // Initialize the player's deck
    public void InitializePlayerDeck()
    {
        var availableCards = new List<Card>(cardDatabase.cards);
        playerDeck.Clear();

        if (availableCards.Count == 0)
        {
            Debug.LogWarning("No cards available in the card database!");
            return;
        }

        while (playerDeck.Count < 30) // Fill the deck with 30 cards
        {
            int randomIndex = UnityEngine.Random.Range(0, availableCards.Count);
            playerDeck.Add(availableCards[randomIndex]);
        }

        Debug.Log("Player deck initialized.");
        OnDeckInitialized?.Invoke();
    }

    // Draw a random card from the deck by type and cost
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
