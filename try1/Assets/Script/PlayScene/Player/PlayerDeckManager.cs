using System.Collections.Generic;
using UnityEngine;

public class PlayerDeckManager : MonoBehaviour
{
    public List<Card> playerDeck = new List<Card>(); // List to hold the player's deck of cards
    public int desiredDeckSize = 30; // Number of cards desired in the player's deck
    public CardDatabase cardDatabase; // Reference to the CardDatabase script

    void Start()
    {
        InitializePlayerDeck();
    }

    public void InitializePlayerDeck()
    {
        List<Card> availableCards = new List<Card>(cardDatabase.cards); // Get all available cards from CardDatabase

        playerDeck.Clear(); // Clear existing playerDeck before initialization

        // Ensure there are cards to draw from the database
        if (availableCards.Count == 0)
        {
            Debug.LogWarning("No cards available in the card database to initialize the player deck.");
            return;
        }

        // Draw cards until desiredDeckSize is reached, allowing duplicates
        while (playerDeck.Count < desiredDeckSize)
        {
            Card randomCard = DrawRandomCardFromList(availableCards); // Draw a random card from available cards
            if (randomCard != null)
            {
                playerDeck.Add(randomCard);
                Debug.Log("Added card to player deck: " + randomCard.cardName);
            }
            else
            {
                Debug.LogWarning("Failed to draw a random card. Check RandomCard script.");
                break; // Exit the loop if drawing a card fails repeatedly
            }
        }

        Debug.Log("Player deck initialized with " + playerDeck.Count + " cards.");
    }

    // Method to draw a random card from the given list
    Card DrawRandomCardFromList(List<Card> cards)
    {
        if (cards.Count > 0)
        {
            int randomIndex = Random.Range(0, cards.Count);
            Card randomCard = cards[randomIndex];
            return randomCard;
        }
        else
        {
            Debug.LogWarning("No more cards left in the list to draw from!");
            return null;
        }
    }

    // Method to draw a random card from playerDeck
    public Card DrawRandomCard()
    {
        if (playerDeck.Count > 0)
        {
            int randomIndex = Random.Range(0, playerDeck.Count);
            Card randomCard = playerDeck[randomIndex];
            playerDeck.RemoveAt(randomIndex); // Remove the card from the player deck
            Debug.Log("Random card drawn from player deck: " + randomCard.cardName);
            return randomCard;
        }
        else
        {
            Debug.LogWarning("No more cards left in player deck!");
            return null;
        }
    }

    // Method to add a card back to playerDeck (if needed)
    public void ReturnCardToDeck(Card card)
    {
        playerDeck.Add(card);
        Debug.Log("Returned card to player deck: " + card.cardName);
    }

    // Method to clear all cards from playerDeck (if needed)
    public void ClearDeck()
    {
        playerDeck.Clear();
        Debug.Log("Player deck cleared.");
    }

    // Method to remove a specific card from the deck
    public void RemoveCardFromDeck(Card card)
    {
        if (playerDeck.Contains(card))
        {
            playerDeck.Remove(card);
            Debug.Log("Removed card from player deck: " + card.cardName);
        }
        else
        {
            Debug.LogWarning("Card not found in player deck.");
        }
    }
}
