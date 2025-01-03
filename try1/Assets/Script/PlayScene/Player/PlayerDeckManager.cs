using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerDeckManager : MonoBehaviour
{
    public List<Card> playerDeck = new(); // Player's full deck
    public CardDatabase cardDatabase;

    public event Action OnPlayerDeckReady; // Fires when playerDeck is ready

    private bool isPlayerDeckInitialized = false;

    void Start()
    {
        if (cardDatabase == null)
        {
            Debug.LogError("CardDatabase is not assigned!");
            return;
        }

        // For development only
        InitializePlayerDeck();
    }

    // Initialize the player's deck
    public void InitializePlayerDeck()
    {
        if (isPlayerDeckInitialized) return;

        var availableCards = new List<Card>(cardDatabase.cards);
        playerDeck.Clear();

        if (availableCards.Count == 0)
        {
            Debug.LogWarning("No cards available in the card database!");
            return;
        }

        while (playerDeck.Count < 30)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableCards.Count);
            playerDeck.Add(availableCards[randomIndex]);
        }

        Debug.Log($"Player deck initialized with {playerDeck.Count} cards.");
        isPlayerDeckInitialized = true;
        OnPlayerDeckReady?.Invoke();
    }
}
