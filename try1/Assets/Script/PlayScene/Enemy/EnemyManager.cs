using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyDeckManager enemyDeckManager;
    public BoardManager boardManager; // Reference to BoardManager
    public int maxCardsPerTurn = 3;
    private int enemyCostLimit;
    private int turnCounter = 0;
    public List<Card> enemyHand = new();
    private System.Random random = new();

    public CardDatabase cardDatabase;
    public GameObject enemyCardPrefab;
    
    public void StartTurn()
    {
        Debug.Log($"[Enemy AI] Turn {turnCounter}: AI drew {enemyHand.Count} cards.");
        Debug.Log("Test1");
        turnCounter++;
        enemyHand.Clear();
        for (int i = 0; i < 5; i++)
        {
        if (!enemyDeckManager.IsDeckEmpty())
        {
            enemyHand.Add(enemyDeckManager.TakeNextCard());
        }
        else
        {
            Debug.LogWarning("Enemy deck is empty! AI will continue with fewer cards.");
            break; // Prevent infinite loop
        }
        }
        enemyCostLimit = GetCostLimit();
        Debug.Log("Test5");
        PlaceCards();
        Debug.Log("Test6");
        Debug.Log($"[Enemy AI] Turn {turnCounter}: AI drew {enemyHand.Count} cards.");
    }

    private int GetCostLimit()
    {
        switch (GameManager.Instance.CurrentStage.difficulty)
        {
            case Difficulty.Easy: return 3;
            case Difficulty.Normal: return 5;
            case Difficulty.Hard: return 7;
            default: return 5;
        }
    }

    private void PlaceCards()
    {
        int placedCards = 0;
        int remainingCost = enemyCostLimit;
        List<GameObject> availableSlots = boardManager.enemyReserveSlots.FindAll(slot => slot.GetComponent<EnemyCardSlot>().IsOccupied() == false);

        // Boss fights - Ensure forced placement
        if (GameManager.Instance.CurrentStage.stageType == StageType.Boss)
        {
            foreach (var entry in GameManager.Instance.CurrentStage.specificCards)
            {
                if (entry.turn == turnCounter && availableSlots.Count > 0)
                {
                    Card bossCard = cardDatabase.GetCardById(entry.cardId);
                    if (bossCard != null)
                    {
                        PlaceCardInSlot(bossCard, availableSlots[0]);
                        availableSlots.RemoveAt(0);
                        return;
                    }
                }
            }
        }

        // AI placement strategy
        while (placedCards < maxCardsPerTurn && remainingCost > 0 && availableSlots.Count > 0)
        {
            Card selectedCard = SelectCardBasedOnPlayerField();
            if (selectedCard == null) break;

            if (selectedCard.cost > remainingCost) continue;

            PlaceCardInSlot(selectedCard, availableSlots[0]);
            availableSlots.RemoveAt(0);

            remainingCost -= selectedCard.cost;
            placedCards++;
        }
    }

    private Card SelectCardBasedOnPlayerField()
    {
        bool playerHasAttackers = boardManager.PlayerFieldHasAttackers();
        bool playerHasDefenders = boardManager.PlayerFieldHasDefenders();

        if (playerHasAttackers && random.NextDouble() < 0.7) 
        {
            var defenders = enemyHand.FindAll(c => c.dinoType == "Defender");
            if (defenders.Count > 0) return defenders[random.Next(defenders.Count)];
        }

        if (playerHasDefenders && random.NextDouble() < 0.7) 
        {
            var attackers = enemyHand.FindAll(c => c.dinoType == "Attacker");
            if (attackers.Count > 0) return attackers[random.Next(attackers.Count)];
        }

        return enemyHand.Count > 0 ? enemyHand[random.Next(enemyHand.Count)] : null;
    }

    private void PlaceCardInSlot(Card card, GameObject slot)
    {
        var enemySlot = slot.GetComponent<EnemyCardSlot>();

        if (enemySlot != null && !enemySlot.IsOccupied())
        {
            // 🔹 Create a new GameObject for the card
            GameObject cardObject = InstantiateCardObject(card);

            // 🔹 Place the GameObject in the slot
            enemySlot.PlaceCard(cardObject);

            enemyHand.Remove(card);
            Debug.Log($"[Enemy AI] Placed {card.cardName} in enemy slot.");
        }
    }

    private GameObject InstantiateCardObject(Card card)
    {
        if (enemyCardPrefab == null)
        {
            Debug.LogError("EnemyCardPrefab is not assigned in the Inspector!");
            return null;
        }

        // 🔹 Instantiate the enemy card prefab
        GameObject newCard = Instantiate(enemyCardPrefab);

        // 🔹 Assign card data to CardViz
        CardViz cardViz = newCard.GetComponent<CardViz>();
        if (cardViz != null)
        {
            cardViz.LoadCard(card); // ✅ Update the UI with card data
        }
        else
        {
            Debug.LogWarning("CardViz component not found on the prefab!");
        }

        return newCard;
    }

    public Card GetCardById(int cardId)
    {
        return cardDatabase.GetCardById(cardId);
    }

    public void ReturnHandToDeck()
    {
        enemyDeckManager.ReturnHandToDeck(enemyHand);
    }

}
