using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeckManager : MonoBehaviour
{
    public CardDatabase cardDatabase; // Reference to the CardDatabase script
    public RandomCard randomCardScript; // Reference to the RandomCard script
    public List<Card> playerDeck = new List<Card>(); // List to hold the player's deck
    public int desiredDeckSize = 30; // Number of cards desired in the player's deck

    void Start()
    {
        InitializePlayerDeck();
    }

    void InitializePlayerDeck()
    {
        List<Card> availableCards = new List<Card>(cardDatabase.cards); // Get all available cards from CardDatabase

        // While loop to draw cards until desiredDeckSize is reached or no more cards available
        while (playerDeck.Count < desiredDeckSize && availableCards.Count > 0)
        {
            Card randomCard = randomCardScript.DrawAndRemoveRandomCard(availableCards);
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
}
