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
            selectedCards.Add(card);
            Debug.Log($"Selected Card: {card.cardName}");
            return true;
        }

        Debug.LogWarning("Cannot select more cards.");
        return false;
    }

    public void DeselectCard(Card card)
    {
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            Debug.Log($"Deselected Card: {card.cardName}");
        }
    }


    private void PopulateCardsExcludingBoss()
    {
        // Clear existing card objects
        foreach (Transform child in cardDisplay.transform)
        {
            Destroy(child.gameObject);
        }

        List<Card> filteredCards = cardDatabase.cardAssets.FindAll(card => card.dinoType.ToLower() != "boss");
        int cardsToDisplay = 5;

        for (int i = 0; i < cardsToDisplay; i++)
        {
            Card randomCard = filteredCards[Random.Range(0, filteredCards.Count)];
            GameObject cardObject = Instantiate(cardTemplate, cardDisplay.transform);

            var cardViz = cardObject.GetComponent<CardViz>();
            cardViz?.LoadCard(randomCard);

            var clickable = cardObject.GetComponent<CardVizClickable>();
            if (clickable != null)
            {
                clickable.selectionHandler = this; // Assign the current manager as the handler
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

            GameManager.Instance.AdvanceStage();
            GameManager.Instance.AddMoney(2);
            GameManager.Instance.ContinueGame();
        }
        else
        {
            Debug.LogWarning("You must select exactly 1 card to proceed.");
        }
    }
}