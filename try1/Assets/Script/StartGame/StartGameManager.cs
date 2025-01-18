using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartGameManager : MonoBehaviour,ICardSelectionHandler
{
    [Header("References")]
    public CardDatabase cardDatabase;
    public GameObject cardTemplate;
    public GameObject cardDisplay;
    public GameObject continueButton;
    public TextMeshProUGUI uiMessageText;

    [Header("Selection Settings")]
    public int maxAttackers = 3;
    public int maxDefenders = 2;

    private List<Card> selectedAttackers = new();
    private List<Card> selectedDefenders = new();
    private bool selectingAttackers = true;

    private void Start()
    {
        PopulateCardsByType("Attacker");
        UpdateUI();
    }

    public bool SelectCard(Card card)
    {
        if (selectingAttackers && selectedAttackers.Count < maxAttackers)
        {
            selectedAttackers.Add(card);
            Debug.Log($"Selected Attacker: {card.cardName}");
            return true;
        }
        else if (!selectingAttackers && selectedDefenders.Count < maxDefenders)
        {
            selectedDefenders.Add(card);
            Debug.Log($"Selected Defender: {card.cardName}");
            return true;
        }

        Debug.LogWarning("Cannot select more cards.");
        return false;
    }

    public void DeselectCard(Card card)
    {
        if (selectingAttackers)
        {
            selectedAttackers.Remove(card);
        }
        else
        {
            selectedDefenders.Remove(card);
        }
    }


    public void ContinueSelection()
    {
        if (selectingAttackers && selectedAttackers.Count == maxAttackers)
        {
            selectingAttackers = false;
            UpdateUI();
            PopulateCardsByType("Defender");
        }
        else if (!selectingAttackers && selectedDefenders.Count == maxDefenders)
        {
            SavePlayerData();
            SceneManager.LoadScene("ChooseStage");
        }
        else
        {
            Debug.LogWarning("Selection incomplete.");
        }
    }

    private void SavePlayerData()
    {
        List<int> playerDeckIds = new();
        playerDeckIds.AddRange(selectedAttackers.ConvertAll(card => card.id));
        playerDeckIds.AddRange(selectedDefenders.ConvertAll(card => card.id));

        List<Card> normalCards = cardDatabase.cardAssets.FindAll(card => card.dinoType == "Normal");
        for (int i = 0; i < 5; i++)
        {
            int randomIndex = Random.Range(0, normalCards.Count);
            playerDeckIds.Add(normalCards[randomIndex].id);
        }

        PlayerSaveData saveData = new PlayerSaveData
        {
            playerDeckIds = playerDeckIds,
            money = 3,
            currentStage = 1,
            isSaveValid = true
        };

        SaveManager.SaveGame(saveData);
        Debug.Log("Game saved.");
    }

    private void UpdateUI()
    {
        string message = selectingAttackers
            ? "Please Choose 3 Attacker Cards"
            : "Please Choose 2 Defender Cards";

        if (uiMessageText != null)
        {
            uiMessageText.text = message;
        }

        Debug.Log(message);
    }

    public void PopulateCardsByType(string dinoType)
    {
        foreach (Transform child in cardDisplay.transform)
        {
            Destroy(child.gameObject);
        }

        List<Card> filteredCards = cardDatabase.cardAssets.FindAll(card => card.dinoType.ToLower() == dinoType.ToLower());
        int cardsToDisplay = 10;

        for (int i = 0; i < cardsToDisplay; i++)
        {
            Card randomCard = filteredCards[Random.Range(0, filteredCards.Count)];
            GameObject cardObject = Instantiate(cardTemplate, cardDisplay.transform);

            var cardViz = cardObject.GetComponent<CardViz>();
            cardViz?.LoadCard(randomCard);

            var clickable = cardObject.GetComponent<CardVizClickable>();
            if (clickable != null)
            {
                clickable.selectionHandler = this; // Assign StartGameManager as the handler
            }
        }
    }

}

