using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopStage : MonoBehaviour, ICardSelectionHandler
{
    [Header("References")]
    public CardDatabase cardDatabase; // Reference to the card database
    public GameObject cardTemplate; // Prefab for card visualization
    public GameObject cardDisplay; // Parent object for displaying cards
    public TextMeshProUGUI coinUIText;

    [Header("Settings")]
    public int maxSelectableCards = 1; // Limit player to selecting only 1 card
    private int totalCost = 0; // Track the total cost of selected cards
    private List<Card> selectedCards = new(); // Tracks selected cards

    private void Start()
    {
        PopulateCardsExcludingBoss(); // Populate the cards, excluding Boss type
        UpdateUI();
    }

    private void UpdateUI()
    {
        var saveData = GameManager.Instance.CurrentSaveData;
        if (coinUIText != null)
        {
            int remainingMoney = saveData.money - totalCost;
            coinUIText.text = remainingMoney.ToString();
             coinUIText.color = totalCost > 0 ? Color.red : Color.black;
        }
    }

    public bool SelectCard(Card card)
    {
        // Calculate card cost based on its properties
        int cardCost = card.cost >= 3 ? 2 : 1;

        // Check if adding the card exceeds the player's limit
        if (selectedCards.Count < maxSelectableCards)
        {
            selectedCards.Add(card);
            totalCost += cardCost; // Add card cost to total
            Debug.Log($"Selected Card: {card.cardName}, Cost: {cardCost}");
            UpdateUI();
            return true;
        }

        Debug.LogWarning("Cannot select more cards.");
        return false;
    }

    public void DeselectCard(Card card)
    {
        // Remove card and subtract its cost
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            int cardCost = card.cost >= 3 ? 2 : 1;
            totalCost -= cardCost;
            Debug.Log($"Deselected Card: {card.cardName}, Cost: {cardCost}");
            UpdateUI();
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
        int cardsToDisplay = 8;

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
        var saveData = GameManager.Instance.CurrentSaveData;

        // Prevent confirmation if overspending
        if (saveData.money - totalCost < 0)
        {
            Debug.LogWarning("Not enough money to confirm selection.");
            return;
        }

        // Add selected card(s) to player's deck and subtract money
        foreach (var card in selectedCards)
        {
            GameManager.Instance.AddToPlayerDeck(card.id);
        }

        saveData.money -= totalCost; // Deduct total cost from player money
        GameManager.Instance.SaveData();

        Debug.Log($"Confirmed purchase. Total cost: {totalCost}. Returning to ChooseStage.");
        GameManager.Instance.AdvanceStage();
        SceneManager.LoadScene("ChooseStage");
    }
}
