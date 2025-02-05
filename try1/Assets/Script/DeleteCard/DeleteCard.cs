using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeleteCard : MonoBehaviour, ICardSelectionHandler
{
    public CardDatabase cardDatabase; // Reference to the card database
    public GameObject cardTemplate; // Prefab for a single card UI
    public Transform cardDisplayArea; // Parent object to hold card UIs

    [Header("Settings")]
    public int maxSelectableCards = 1; // Limit player to selecting only 1 card

    private List<Card> selectedCards = new(); // Tracks selected cards

    private void Start()
    {
        LoadAndDisplayPlayerDeck();
    }

    private void LoadAndDisplayPlayerDeck()
{
    List<int> playerDeckIds = GameManager.Instance.CurrentSaveData.playerDeckIds;

    if (playerDeckIds == null || playerDeckIds.Count == 0)
    {
        Debug.LogWarning("No cards in the player's deck.");
        return;
    }

    // Clear existing cards
    foreach (Transform child in cardDisplayArea)
    {
        Destroy(child.gameObject);
    }

    // Instantiate cards and assign selection handler
    foreach (int cardId in playerDeckIds)
    {
        Card cardData = cardDatabase.GetCardById(cardId);

        if (cardData != null)
        {
            GameObject cardObject = Instantiate(cardTemplate, cardDisplayArea);
            
            // Load card data
            CardViz cardViz = cardObject.GetComponent<CardViz>();
            cardViz.LoadCard(cardData);

            // Assign the selection handler
            var clickable = cardObject.GetComponent<CardVizClickable>();
            if (clickable != null)
            {
                clickable.selectionHandler = this; // Assign the current DeleteCard instance as the handler
            }
        }
        else
        {
            Debug.LogWarning($"Card with ID {cardId} not found in database.");
        }
    }
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

    private void OnDisable()
    {
        GameManager.Instance.SaveData(); // Save changes when leaving
    }

    public void ChooseCard()
    {
        SceneManager.LoadScene("ChooseCard");
    }

    public void ConfirmSelection()
    {
        if (selectedCards.Count == maxSelectableCards)
        {
            // Add selected card(s) to player's deck
            foreach (var card in selectedCards)
            {
                GameManager.Instance.RemoveFromPlayerDeck(card.id); // Add to player's deck via GameManager
            }

            GameManager.Instance.LastStageChoicesClear();
            GameManager.Instance.AdvanceStage();
            GameManager.Instance.SaveData();
            Debug.Log($"Deleted card from deck.");
            Debug.Log("Card Deleted and saved. Returning to ChooseStage.");
            SceneManager.LoadScene("ChooseStage");
        }
        else
        {
            Debug.LogWarning("You must select exactly 1 card to proceed.");
        }
    }

}

