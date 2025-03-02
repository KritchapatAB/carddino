using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class StartGameManager : MonoBehaviour, ICardSelectionHandler
{
    [Header("References")]
    public CardDatabase cardDatabase;
    public GameObject cardTemplate;
    public GameObject cardDisplay;
    public Button continueButton;
    public TextMeshProUGUI uiMessageText;

    [Header("Selection Settings")]
    public int maxAttackers = 3;
    public int maxDefenders = 2;

    private List<Card> selectedAttackers = new();
    private List<Card> selectedDefenders = new();
    private List<Card> starterCards = new();

    private enum SelectionPhase { Starter, Attacker, Defender }
    private SelectionPhase currentPhase = SelectionPhase.Starter;

    private void Start()
    {
        ShowStarterCards();
        continueButton.interactable = true; // ✅ Allow user to proceed
    }

    private void ShowStarterCards()
    {
        uiMessageText.text = "Your Starter Cards";

        foreach (Transform child in cardDisplay.transform)
        {
            Destroy(child.gameObject);
        }

        List<Card> normalCards = cardDatabase.cardAssets.FindAll(card => card.dinoType == "Normal");
        for (int i = 0; i < 8; i++)
        {
            int randomIndex = Random.Range(0, normalCards.Count);
            starterCards.Add(normalCards[randomIndex]);
        }

        foreach (Card card in starterCards)
        {
            GameObject cardObject = Instantiate(cardTemplate, cardDisplay.transform);
            var cardViz = cardObject.GetComponent<CardViz>();
            cardViz?.LoadCard(card);
        }
    }

    public void ContinueSelection()
    {
        switch (currentPhase)
        {
            case SelectionPhase.Starter:
                currentPhase = SelectionPhase.Attacker;
                PopulateCardsByType("Attacker");
                uiMessageText.text = $"Select {maxAttackers} Attacker Cards (0/{maxAttackers})";
                continueButton.interactable = false;
                break;

            case SelectionPhase.Attacker:
                if (selectedAttackers.Count == maxAttackers)
                {
                    currentPhase = SelectionPhase.Defender;
                    PopulateCardsByType("Defender");
                    uiMessageText.text = $"Select {maxDefenders} Defender Cards (0/{maxDefenders})";
                    continueButton.interactable = false;
                }
                break;

            case SelectionPhase.Defender:
                if (selectedDefenders.Count == maxDefenders)
                {
                    GameManager.Instance.ResetSaveData();
                    GameManager.Instance.StartNewGame();
                    SavePlayerData();
                    GameManager.Instance.ContinueGame();
                }
                break;
        }
    }

    private void SavePlayerData()
    {
        List<int> playerDeckIds = new();
        playerDeckIds.AddRange(selectedAttackers.ConvertAll(card => card.id));
        playerDeckIds.AddRange(selectedDefenders.ConvertAll(card => card.id));
        playerDeckIds.AddRange(starterCards.ConvertAll(card => card.id));

        GameManager.Instance.CurrentSaveData.playerDeckIds = playerDeckIds;
        GameManager.Instance.CurrentSaveData.money = 3;
        GameManager.Instance.CurrentSaveData.currentStage = 1;
        GameManager.Instance.CurrentSaveData.isSaveValid = true;
        GameManager.Instance.SaveData();
    }

    public bool SelectCard(Card card)
    {
        if (currentPhase == SelectionPhase.Attacker && selectedAttackers.Count < maxAttackers)
        {
            selectedAttackers.Add(card);
            UpdateUI();
            return true;
        }
        else if (currentPhase == SelectionPhase.Defender && selectedDefenders.Count < maxDefenders)
        {
            selectedDefenders.Add(card);
            UpdateUI();
            return true;
        }
        return false;
    }

    public void DeselectCard(Card card)
    {
        if (currentPhase == SelectionPhase.Attacker)
        {
            selectedAttackers.Remove(card);
        }
        else if (currentPhase == SelectionPhase.Defender)
        {
            selectedDefenders.Remove(card);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        switch (currentPhase)
        {
            case SelectionPhase.Attacker:
                uiMessageText.text = $"Select {maxAttackers} Attacker Cards ({selectedAttackers.Count}/{maxAttackers})";
                continueButton.interactable = (selectedAttackers.Count == maxAttackers);
                break;

            case SelectionPhase.Defender:
                uiMessageText.text = $"Select {maxDefenders} Defender Cards ({selectedDefenders.Count}/{maxDefenders})";
                continueButton.interactable = (selectedDefenders.Count == maxDefenders);
                break;
        }
    }

    public void PopulateCardsByType(string dinoType)
    {
        foreach (Transform child in cardDisplay.transform)
        {
            Destroy(child.gameObject);
        }

        List<Card> filteredCards = cardDatabase.cardAssets.FindAll(card => card.dinoType == dinoType);
        ShuffleList(filteredCards);

        int cardsToDisplay = Mathf.Min(filteredCards.Count, 10);
        for (int i = 0; i < cardsToDisplay; i++)
        {
            Card randomCard = filteredCards[i];
            GameObject cardObject = Instantiate(cardTemplate, cardDisplay.transform);

            var cardViz = cardObject.GetComponent<CardViz>();
            cardViz?.LoadCard(randomCard);

            var clickable = cardObject.GetComponent<CardVizClickable>();
            if (clickable != null)
            {
                clickable.selectionHandler = this;
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
