using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

public class BoardManager : MonoBehaviour
{
    public List<GameObject> playerSlots = new();
    public TextMeshProUGUI sacrificeProgressText;

    private List<GameObject> cardsToSacrifice = new();
    private int requiredSacrifices;
    private GameObject cardToPlace;
    private bool isSacrificePhaseActive = false;

    public List<Card> engagePlayerDeck = new(); // Deck for the current fight
    public PlayerDeckManager playerDeckManager;

    public List<GameObject> enemySlots = new(); // Add enemy card slots
    public EnemyDeckManager enemyDeckManager;

    public event Action OnDeckChanged;

//region StartGame
    void Start()
    {
        PrepareEngageDeck();
    }

    private void Awake()
    {
        // Find all slots in the scene and populate playerSlots
        playerSlots.Clear();
        foreach (Transform child in transform)
        {
            var slot = child.GetComponent<CardSlot>();
            if (slot != null)
            {
                playerSlots.Add(child.gameObject);
            }
        }
        Debug.Log($"Player slots populated: {playerSlots.Count}");
    }

    public void PrepareEngageDeck()
    {
        if (playerDeckManager == null || playerDeckManager.playerDeck.Count == 0)
        {
            Debug.LogError("Cannot prepare engage deck. Player deck is not initialized.");
            return;
        }

        engagePlayerDeck.Clear();
        engagePlayerDeck.AddRange(playerDeckManager.playerDeck);

        Debug.Log($"Engage deck prepared with {engagePlayerDeck.Count} cards.");
    }
//endregion

//region DrawCard
    //Draw Ramdom Card
    public Card DrawCardFromEngageDeck()
    {
        if (engagePlayerDeck == null || engagePlayerDeck.Count == 0)
        {
            Debug.LogWarning("No cards left in the Engage Deck to draw!");
            return null;
        }

        // Draw a random card
        int randomIndex = UnityEngine.Random.Range(0, engagePlayerDeck.Count);
        Card drawnCard = engagePlayerDeck[randomIndex];

        // Remove the card from the Engage Deck
        engagePlayerDeck.RemoveAt(randomIndex);

        Debug.Log($"Card drawn randomly: {drawnCard.cardName}");

        OnDeckChanged?.Invoke();

        return drawnCard;
    }

    //Draw Card Filter
    public Card DrawCardFromEngageDeck(string cardType, int maxCost)
    {
        if (engagePlayerDeck == null || engagePlayerDeck.Count == 0)
        {
            Debug.LogWarning("No cards left in the Engage Deck to draw!");
            return null;
        }

        // Filter cards by type and cost
        var filteredCards = engagePlayerDeck.FindAll(card => card.dinoType == cardType && card.cost <= maxCost);

        if (filteredCards.Count == 0)
        {
            Debug.LogWarning($"No cards of type {cardType} with cost <= {maxCost} found.");
            return null;
        }

        // Draw a random card from the filtered list
        int randomIndex = UnityEngine.Random.Range(0, filteredCards.Count);
        Card drawnCard = filteredCards[randomIndex];

        // Remove the card from the Engage Deck
        engagePlayerDeck.Remove(drawnCard);


        OnDeckChanged?.Invoke();
        return drawnCard;
    }
//endregion

//region Sacrifire
    public bool IsSacrificePhaseActive() => isSacrificePhaseActive;
    public bool AreSacrificesComplete() => cardsToSacrifice.Count >= requiredSacrifices;

    public void EnableSacrificePhase(int cost, GameObject card)
    {
        requiredSacrifices = cost;
        cardToPlace = card;
        isSacrificePhaseActive = true;

        HighlightAvailableCards();
        UpdateSacrificeProgressText();
    }


    public void DisableSacrificePhase()
    {
        Debug.Log("Disabling sacrifice phase.");
        ResetOnBoardCardsScale(); // Reset all on-board cards
        ClearSacrificeData();
    }

    public void ProceedToPlacement()
    {
        if (cardsToSacrifice.Count < requiredSacrifices)
        {
            Debug.LogWarning("Not enough sacrifices made.");
            return;
        }

        HighlightEmptySlots();
        ClearSacrificeProgressText();
    }

    public void DestroySacrificedCards()
    {
        foreach (var card in cardsToSacrifice)
        {
            var cardSlot = card.transform.parent?.GetComponent<CardSlot>();
            if (cardSlot != null)
            {
                cardSlot.SetOccupied(false); // Mark the slot as unoccupied
            }

            Destroy(card); // Remove the card from the scene
            Debug.Log($"Destroyed sacrificed card: {card.name}");
        }

        cardsToSacrifice.Clear(); // Clear the sacrifice list
    }


    public void SelectCardForSacrifice(GameObject card)
    {
        if (!isSacrificePhaseActive)
        {
            Debug.LogWarning("Cannot select card for sacrifice. Sacrifice phase is not active.");
            return;
        }

        var interactionHandler = card.GetComponent<CardInteractionHandler>();
        if (interactionHandler == null || interactionHandler.currentState != CardInteractionHandler.CardState.OnBoard)
        {
            Debug.LogWarning("Only on-board cards can be sacrificed.");
            return;
        }

        if (cardsToSacrifice.Contains(card))
        {
            DeselectCardForSacrifice(card);
        }
        else if (cardsToSacrifice.Count < requiredSacrifices)
        {
            cardsToSacrifice.Add(card);
            interactionHandler.HighlightSelectedForSacrifice();
        }

        UpdateSacrificeProgressText();

        if (cardsToSacrifice.Count == requiredSacrifices)
        {
            ProceedToPlacement();
        }
    }

    
    private void ClearSacrificeData()
    {
        cardsToSacrifice.Clear();
        ClearSacrificeProgressText();
        isSacrificePhaseActive = false;
        cardToPlace = null;
    }

    private void ResetOnBoardCardsScale()
    {
        foreach (var handler in FindObjectsOfType<CardInteractionHandler>())
        {
            handler.ResetToDefault();
        }
    }

    public void DeselectCardForSacrifice(GameObject card)
    {
        if (card == null)
        {
            Debug.LogError("Attempted to deselect a null card.");
            return;
        }

        // Check if the card is in the sacrifice list
        if (!cardsToSacrifice.Contains(card))
        {
            Debug.LogWarning($"Card '{card.name}' is not in the sacrifice list.");
            return;
        }

        // Remove the card from the sacrifice list
        cardsToSacrifice.Remove(card);
        Debug.Log($"Card '{card.name}' removed from sacrifice list.");

        // Reset the card's visuals
        var interactionHandler = card.GetComponent<CardInteractionHandler>();
        if (interactionHandler != null)
        {
            interactionHandler.DeselectSacrifireAble();
        }
        else
        {
            Debug.LogError($"Card '{card.name}' is missing a CardInteractionHandler component.");
        }
        UpdateSacrificeProgressText();
    }

//endregion

//region Highlight
    private void HighlightAvailableCards()
    {
        foreach (var handler in FindObjectsOfType<CardInteractionHandler>())
        {
            if (handler.currentState == CardInteractionHandler.CardState.OnBoard)
            {
                handler.HighlightSacrifireAble();
            }
        }
    }

    public void HighlightEmptySlots()
    {
        foreach (var slot in playerSlots)
        {
            if (slot == null) continue;

            var cardSlot = slot.GetComponent<CardSlot>();
            if (cardSlot != null && !cardSlot.IsOccupied())
            {
                cardSlot.ShowPlaceableUI();
            }
        }
    }
//endregion

//region SacrifireText
    private void UpdateSacrificeProgressText()
    {
        if (sacrificeProgressText != null)
        {
            sacrificeProgressText.text = $"{cardsToSacrifice.Count}/{requiredSacrifices} sacrifices selected";
        }
    }

    private void ClearSacrificeProgressText()
    {
        if (sacrificeProgressText != null)
        {
            sacrificeProgressText.text = string.Empty;
        }
    }
    
//endregion

//region Enemy
    public void HandleEnemyTurn()
    {
        Debug.Log("EnemyTurn");
    }
//endregion

    public void ClearEngageDeck()
    {
        engagePlayerDeck.Clear();
        Debug.Log("Engage deck cleared after the fight.");
    }
    
    public void EnablePlayerControls()
    {
        Debug.Log("Player controls enabled.");
        // Enable UI interactions and highlight slots, etc.
    }

    public void DisablePlayerControls()
    {
        Debug.Log("Player controls disabled.");
        // Disable UI interactions and reset any highlights
    }

}