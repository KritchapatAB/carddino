using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseCardStageManager : MonoBehaviour, ICardSelectionHandler
{
    [Header("References")]
    public CardDatabase cardDatabase; // Reference to the card database
    public GameObject cardTemplate; // Prefab for card visualization
    public GameObject cardDisplay; // Parent object for displaying cards

    [Header("Settings")]
    public int maxSelectableCards = 1; // Limit player to selecting only 1 card

    private List<Card> selectedCards = new(); // Tracks selected cards

    private void Start()
    {
        PopulateCardsExcludingBoss(); // Populate the cards, excluding Boss type
    }

    public bool SelectCard(Card card)
    {
        if (selectedCards.Count < maxSelectableCards)
        {
            selectedCards.Add(card); // Add card to selection
            Debug.Log($"Selected Card: {card.cardName}");
            return true; // Successful selection
        }

        Debug.LogWarning("Cannot select more cards.");
        return false; // Selection failed (max reached)
    }

    public void DeselectCard(Card card)
    {
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card); // Remove card from selection
            Debug.Log($"Deselected Card: {card.cardName}");
        }
    }

    private void PopulateCardsExcludingBoss()
    {
        // Clear any existing card objects
        foreach (Transform child in cardDisplay.transform)
        {
            Destroy(child.gameObject);
        }

        List<Card> filteredCards = cardDatabase.cardAssets.FindAll(card => card.dinoType.ToLower() != "boss");
        int cardsToDisplay = 5; // Show a fixed number of cards

        for (int i = 0; i < cardsToDisplay; i++)
        {
            Card randomCard = filteredCards[Random.Range(0, filteredCards.Count)]; // Get a random card
            GameObject cardObject = Instantiate(cardTemplate, cardDisplay.transform);

            var cardViz = cardObject.GetComponent<CardViz>();
            cardViz?.LoadCard(randomCard); // Load card data into CardViz

            var clickable = cardObject.GetComponent<CardVizClickable>();
            if (clickable != null)
            {
                clickable.selectionHandler = this; // Assign this manager as the handler
            }
        }
    }

    public void ConfirmSelection()
    {
        if (selectedCards.Count == maxSelectableCards)
        {
            // Add selected card(s) to player's deck
            foreach (var card in selectedCards)
            {
                GameManager.Instance.AddToPlayerDeck(card.id); // Add to player's deck via GameManager
            }

            // Save changes and return to ChooseStage
            GameManager.Instance.SaveData();
            Debug.Log("Card added and saved. Returning to ChooseStage.");
            SceneManager.LoadScene("ChooseStage");
        }
        else
        {
            Debug.LogWarning("You must select exactly 1 card to proceed.");
        }
    }
}
