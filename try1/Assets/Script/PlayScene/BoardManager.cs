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


    public EnemyDeckManager enemyDeckManager;

    public EnemyManager enemyManager;

    public event Action OnDeckChanged;
    public CardDatabase cardDatabase;

    public EnemyCardSlot EnemyCardSlot;
    public GameObject enemyCardPrefab;
    public List<GameObject> enemyReserveSlots = new(); // Where AI places new cards
    public List<GameObject> enemyActiveSlots = new();

//region StartGame
    void Start()
    {
        PrepareEngageDeck();
    }

    private void Awake()
    {
        FindEnemySlots();
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
        if (GameManager.Instance == null || GameManager.Instance.CurrentSaveData == null)
        {
            Debug.LogError("GameManager or SaveData is missing! Cannot prepare engage deck.");
            return;
        }

        engagePlayerDeck.Clear();

        // Get saved player deck from GameManager
        List<int> playerDeckIds = GameManager.Instance.CurrentSaveData.playerDeckIds;

        if (playerDeckIds == null || playerDeckIds.Count == 0)
        {
            Debug.LogError("No saved player deck found! Engage deck cannot be prepared.");
            return;
        }

        foreach (int cardId in playerDeckIds)
        {
            Card card = cardDatabase.cards.Find(c => c.id == cardId);
            if (card != null)
            {
                engagePlayerDeck.Add(card);
            }
            else
            {
                Debug.LogWarning($"Card with ID {cardId} not found in CardDatabase!");
            }
        }

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

    public bool PlayerFieldHasAttackers()
{
    foreach (var slot in playerSlots)
    {
        var cardSlot = slot.GetComponent<CardSlot>();
        if (cardSlot != null && cardSlot.IsOccupied())
        {
            var placedCard = EnemyCardSlot.GetPlacedCard();
            if (placedCard != null)
            {
                var cardViz = placedCard.GetComponent<CardViz>();
                if (cardViz != null)
                {
                    var card = cardViz.GetCardData();
                    if (card != null && card.dinoType == "Attacker")
                    {
                        return true;
                    }
                }
            }
        }
    }
    return false;
}


public bool PlayerFieldHasDefenders()
{
    foreach (var slot in playerSlots)
    {
        var cardSlot = slot.GetComponent<CardSlot>();
        if (cardSlot != null && cardSlot.IsOccupied())
        {
            var placedCard = EnemyCardSlot.GetPlacedCard();
            if (placedCard != null)
            {
                var cardViz = placedCard.GetComponent<CardViz>();
                if (cardViz != null)
                {
                    var card = cardViz.GetCardData();
                    if (card != null && card.dinoType == "Defender")
                    {
                        return true;
                    }
                }
            }
        }
    }
    return false;
}


public void MoveEnemyCardsToActiveArea()
{
    List<EnemyCardSlot> reserveSlots = new();
    List<EnemyCardSlot> activeSlots = new();

    // 🔹 Get all slots in EnemyReserveArea & EnemyActiveArea
    foreach (Transform child in GameObject.Find("EnemyReserveArea").transform)
    {
        var slot = child.GetComponent<EnemyCardSlot>();
        if (slot != null) reserveSlots.Add(slot);
    }

    foreach (Transform child in GameObject.Find("EnemyActiveArea").transform)
    {
        var slot = child.GetComponent<EnemyCardSlot>();
        if (slot != null) activeSlots.Add(slot);
    }

    // 🔹 Move cards ONLY IF Active Area has empty slots
    for (int i = 0; i < activeSlots.Count; i++)
    {
        if (!activeSlots[i].IsOccupied() && i < reserveSlots.Count)
        {
            GameObject cardToMove = reserveSlots[i].GetPlacedCard();
            if (cardToMove != null)
            {
                reserveSlots[i].ClearSlot();
                activeSlots[i].PlaceCard(cardToMove);

                // ✅ Move UI and position correctly
                cardToMove.transform.SetParent(activeSlots[i].transform, false);
                cardToMove.transform.localPosition = Vector3.zero; // Reset position to fit new slot
            }
        }
    }
}


private void FindEnemySlots()
{
    enemyReserveSlots.Clear();
    enemyActiveSlots.Clear();

    GameObject reserveArea = GameObject.Find("EnemyReserveArea");
    GameObject activeArea = GameObject.Find("EnemyActiveArea");

    if (reserveArea != null)
    {
        foreach (Transform child in reserveArea.transform)
        {
            var slot = child.GetComponent<EnemyCardSlot>();
            if (slot != null) enemyReserveSlots.Add(slot.gameObject);
        }
    }
    else
    {
        Debug.LogError("EnemyReserveArea not found in scene!");
    }

    if (activeArea != null)
    {
        foreach (Transform child in activeArea.transform)
        {
            var slot = child.GetComponent<EnemyCardSlot>();
            if (slot != null) enemyActiveSlots.Add(slot.gameObject);
        }
    }
    else
    {
        Debug.LogError("EnemyActiveArea not found in scene!");
    }

    Debug.Log($"[BoardManager] Found {enemyReserveSlots.Count} reserve slots & {enemyActiveSlots.Count} active slots.");
}



}