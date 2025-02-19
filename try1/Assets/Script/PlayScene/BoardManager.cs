using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;
using System.Collections;


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
        ShuffleList(engagePlayerDeck);
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

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]); // Swap elements
        }
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

//EnemyAI

public void GetStrongestPlayerCards(out Card strongestAttacker, out Card strongestDefender)
{
    strongestAttacker = null;
    strongestDefender = null;
    int highestAttackDamage = -1;
    int highestDefenderHealth = -1;

    foreach (var slot in playerSlots)
    {
        var cardSlot = slot.GetComponent<CardSlot>();
        if (cardSlot != null && cardSlot.IsOccupied())
        {
            // ✅ Loop through children to find the actual card
            CardViz placedCard = null;
            foreach (Transform child in slot.transform)
            {
                placedCard = child.GetComponent<CardViz>();
                if (placedCard != null) break; // Found the card, exit loop
            }

            if (placedCard == null)
            {
                Debug.LogWarning($"⚠️ No CardViz found in occupied slot: {slot.name}");
                continue; // Skip this slot
            }

            var card = placedCard.GetCardInstance()?.cardData;
            if (card != null)
            {
                if (card.dinoType == "Attacker" && card.damage > highestAttackDamage)
                {
                    highestAttackDamage = card.damage;
                    strongestAttacker = card;
                }
                if (card.dinoType == "Defender" && card.health > highestDefenderHealth)
                {
                    highestDefenderHealth = card.health;
                    strongestDefender = card;
                }
            }
        }
    }
    
    Debug.Log($"✅ Strongest Attacker: {strongestAttacker?.cardName ?? "None"}");
    Debug.Log($"✅ Strongest Defender: {strongestDefender?.cardName ?? "None"}");
}

public void MoveEnemyCardsToActiveArea()
{
    List<EnemyCardSlot> reserveSlots = new();
    List<EnemyCardSlot> activeSlots = new();

    // ✅ Find all slots in Reserve and Active areas
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

    // ✅ Move cards correctly
    int minSlots = Mathf.Min(reserveSlots.Count, activeSlots.Count); // Make sure we don’t go out of bounds

    for (int i = 0; i < minSlots; i++) 
    {
        if (!activeSlots[i].IsOccupied()) 
        {
            GameObject cardToMove = reserveSlots[i].GetPlacedCard();

            Debug.Log($"[MoveEnemyCardsToActiveArea] Moving {cardToMove?.name ?? "null"} from {reserveSlots[i].name} to {activeSlots[i].name}");

            if (cardToMove != null)
            {
                // ✅ Step 1: Move the card to the new slot
                activeSlots[i].PlaceCard(cardToMove);
                cardToMove.transform.SetParent(activeSlots[i].transform, false);
                cardToMove.transform.localPosition = Vector3.zero;

                // ✅ Step 2: Only clear the old slot AFTER moving the card
                reserveSlots[i].ClearSlot();
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


public CardInstance FindAttackTarget(CardInstance attacker, List<GameObject> opponentSlots, int slotIndex, bool isPlayerAttacking) 
{
    Debug.Log($"🔎 FindAttackTarget - Attacker: {attacker.cardData.cardName} (Slot {slotIndex}) | PlayerAttacking: {isPlayerAttacking}");

    List<CardInstance> defenders = new List<CardInstance>();
    List<CardInstance> otherEnemies = new List<CardInstance>();

    // ✅ Step 1: Identify Defenders and other enemies (Fix for AI tracking player slots)
    for (int i = 0; i < opponentSlots.Count; i++)
    {
        GameObject targetSlot = opponentSlots[i];

        // ✅ Check correct slot type based on attacker type
        if (isPlayerAttacking)
        {
            EnemyCardSlot enemySlot = targetSlot.GetComponent<EnemyCardSlot>();
            if (enemySlot == null || !enemySlot.IsOccupied()) continue;
        }
        else
        {
            CardSlot playerSlot = targetSlot.GetComponent<CardSlot>();
            if (playerSlot == null || !playerSlot.IsOccupied()) continue;
        }

        // ✅ Find the actual card inside the slot
        CardViz cardViz = null;
        foreach (Transform child in targetSlot.transform)
        {
            cardViz = child.GetComponent<CardViz>();
            if (cardViz != null) break;
        }

        if (cardViz == null) continue;
        CardInstance targetCard = cardViz.GetCardInstance();
        if (targetCard == null) continue;

        // ✅ Categorize defenders and other targets
        if (targetCard.cardData.dinoType == "Defender")
        {
            defenders.Add(targetCard);
        }
        else
        {
            otherEnemies.Add(targetCard);
        }
    }

    // ✅ Step 2: Attack Defenders first (Target the leftmost defender)
    if (defenders.Count > 0)
    {
        Debug.Log($"🛡 Targeting Defender: {defenders[0].cardData.cardName}");
        return defenders[0];
    }

    // ✅ Step 3: If no Defenders exist, attack the unit in the same column (SlotIndex)
    if (slotIndex < opponentSlots.Count)
    {
        GameObject targetSlot = opponentSlots[slotIndex];

        // ✅ Ensure the correct slot type is checked
        if (isPlayerAttacking)
        {
            EnemyCardSlot enemySlot = targetSlot.GetComponent<EnemyCardSlot>();
            if (enemySlot != null && enemySlot.IsOccupied())
            {
                CardViz cardViz = targetSlot.transform.GetChild(0).GetComponent<CardViz>();
                if (cardViz != null)
                {
                    Debug.Log($"🎯 Attacking Enemy in Slot {slotIndex}");
                    return cardViz.GetCardInstance();
                }
            }
        }
        else
        {
            CardSlot playerSlot = targetSlot.GetComponent<CardSlot>();
            if (playerSlot != null && playerSlot.IsOccupied())
            {
                CardViz cardViz = targetSlot.transform.GetChild(0).GetComponent<CardViz>();
                if (cardViz != null)
                {
                    Debug.Log($"🎯 Attacking Player Card in Slot {slotIndex}");
                    return cardViz.GetCardInstance();
                }
            }
        }
    }

    // ✅ Step 4: If no enemies are in front, return null (Prepare for direct HP damage)
    Debug.Log($"⚠️ No valid target in slot {slotIndex}. Skipping attack.");
    return null;
}





}