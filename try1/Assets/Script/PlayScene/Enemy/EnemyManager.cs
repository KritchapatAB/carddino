using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyDeckManager enemyDeckManager;
    public BoardManager boardManager;
    public int maxCardsPerTurn = 3;
    private int enemyCostLimit;
    public List<Card> enemyHand = new();
    private System.Random random = new();

    public CardDatabase cardDatabase;
    public GameObject enemyCardPrefab;
    
    private bool shouldCheckBossOrSpecificCard;

    public void Initialize()
    {
        shouldCheckBossOrSpecificCard = enemyDeckManager.hasBossOrSpecificCard;
    }

    public void StartTurn(int turnCounter)
    {
        enemyHand.Clear();
        enemyDeckManager.ShuffleDeck();


        for (int i = 0; i < 5; i++)
        {
            if (!enemyDeckManager.IsDeckEmpty())
            {
                enemyHand.Add(enemyDeckManager.TakeNextCard());
            }
        }

        // ✅ First, check if there are Boss/Specific Cards to place
        if (shouldCheckBossOrSpecificCard)
        {
            Debug.Log("Check?");
            List<Card> specialCards = enemyDeckManager.GetCardsForTurn(turnCounter);
            if (specialCards.Count > 0)
            {
                Debug.Log($"[Enemy AI] Placing special cards for Turn {turnCounter}");
                PlaceSpecialCards(specialCards);
                return; // ✅ Skip normal AI logic if special cards were placed
            }
        }

        Debug.Log($"[Enemy AI] Turn {turnCounter}: AI drew {enemyHand.Count} cards.");
        enemyCostLimit = GetCostLimit();
        PlaceCards();
    }

    private void PlaceSpecialCards(List<Card> specialCards)
    {
        // ✅ Only consider slots in Reserved Area that are NOT occupied and NOT mapped to occupied Active slots
        List<GameObject> availableSlots = boardManager.enemyReserveSlots.FindAll(slot =>
            !slot.GetComponent<EnemyCardSlot>().IsOccupied() &&
            !boardManager.enemyActiveSlots.Exists(activeSlot => 
                activeSlot.GetComponent<EnemyCardSlot>().IsOccupied() && 
                boardManager.enemyReserveSlots.IndexOf(slot) == boardManager.enemyActiveSlots.IndexOf(activeSlot)) // Prevent placing in slots that map to occupied Active Area slots
        );

        foreach (var card in specialCards)
        {
            if (availableSlots.Count == 0) break;

            // ✅ Pick a slot from the cleaned-up list
            GameObject slot = availableSlots[Random.Range(0, availableSlots.Count)];
            PlaceCardInSlot(card, slot);
            availableSlots.Remove(slot); // Remove slot after placement
        }
    }



    private int GetCostLimit()
    {
        return GameManager.Instance.CurrentStage.difficulty switch
        {
            Difficulty.Easy => 3,
            Difficulty.Normal => 5,
            Difficulty.Hard => 7,
            _ => 5
        };
    }

    private void PlaceCards()
    {
        int placedCards = 0;
        int remainingCost = enemyCostLimit;
        
        // Declare availableSlots locally
        List<GameObject> availableSlots = boardManager.enemyReserveSlots.FindAll(slot => 
            !slot.GetComponent<EnemyCardSlot>().IsOccupied()
        );

        // Get strongest player cards
        boardManager.GetStrongestPlayerCards(out Card strongestAttacker, out Card strongestDefender);
        Card highestPriorityTarget = DeterminePriorityTarget(strongestAttacker, strongestDefender, out string counterType);

        if (highestPriorityTarget != null)
        {
            Card counterCard = GetCounterCard(highestPriorityTarget, counterType, remainingCost);
            if (counterCard != null)
            {
                Debug.Log($"[Enemy AI] Countering {highestPriorityTarget.cardName} with {counterCard.cardName}");
                PlaceCard(counterCard, availableSlots, highestPriorityTarget);
                remainingCost -= counterCard.cost;
                placedCards++;
                enemyHand.Remove(counterCard);
            }
        }

        if (placedCards < maxCardsPerTurn)
        {
            Debug.Log($"[Enemy AI] Placing remaining random cards.");
            PlaceRandomCards(remainingCost, maxCardsPerTurn - placedCards, availableSlots);
        }
    }


    private Card DeterminePriorityTarget(Card strongestAttacker, Card strongestDefender, out string counterType)
    {
        counterType = "";
        if (strongestAttacker != null && strongestDefender != null)
        {
            if (strongestAttacker.cost > strongestDefender.cost)
            {
                counterType = "Attacker";
                return strongestAttacker;
            }
            else if (strongestDefender.cost > strongestAttacker.cost)
            {
                counterType = "Defender";
                return strongestDefender;
            }
            else
            {
                counterType = random.Next(0, 2) == 0 ? "Attacker" : "Defender";
                return counterType == "Attacker" ? strongestAttacker : strongestDefender;
            }
        }
        else if (strongestAttacker != null)
        {
            counterType = "Attacker";
            return strongestAttacker;
        }
        else if (strongestDefender != null)
        {
            counterType = "Defender";
            return strongestDefender;
        }
        return null;
    }

    private Card GetCounterCard(Card playerCard, string counterType, int remainingCost)
    {
        if (counterType == "Attacker")
        {
            return enemyHand.Find(c => c.dinoType == "Attacker" && c.damage > playerCard.health && c.cost <= remainingCost)
                ?? enemyHand.Find(c => c.dinoType == "Defender" && c.cost <= remainingCost); // If no strong attacker, place a defender
        }
        else if (counterType == "Defender")
        {
            return enemyHand.Find(c => c.dinoType == "Attacker" && c.cost <= remainingCost);
        }
        return null;
    }

    private void PlaceCard(Card card, List<GameObject> availableSlots, Card playerCard)
    {
        if (availableSlots.Count == 0)
        {
            Debug.LogWarning($"[Enemy AI] No available slots for {card.cardName}. Skipping placement.");
            return;
        }

        if (playerCard.dinoType == "Defender" || card.dinoType == "Defender")
        {
            Debug.Log($"[Enemy AI] {card.cardName} is a Defender or attacking a Defender. Placing anywhere.");
            PlaceCardInSlot(card, GetRandomSlot(availableSlots));
            return;
        }

        int playerCardIndex = boardManager.playerSlots.FindIndex(slot =>
        {
            CardViz placedCard = null;
            foreach (Transform child in slot.transform)
            {
                placedCard = child.GetComponent<CardViz>();
                if (placedCard != null) break;
            }
            return placedCard?.GetCardInstance().cardData == playerCard;
        });

        if (playerCardIndex == -1 || playerCardIndex >= boardManager.enemyReserveSlots.Count)
        {
            Debug.LogWarning($"[Enemy AI] No matching slot for {playerCard.cardName}. Placing in random slot.");
            PlaceCardInSlot(card, GetRandomSlot(availableSlots));
        }
        else
        {
            GameObject matchingSlot = boardManager.enemyReserveSlots[playerCardIndex];
            if (!matchingSlot.GetComponent<EnemyCardSlot>().IsOccupied())
            {
                Debug.Log($"[Enemy AI] Placing {card.cardName} in slot {matchingSlot.name} to counter {playerCard.cardName}");
                PlaceCardInSlot(card, matchingSlot);
            }
            else
            {
                Debug.LogWarning($"[Enemy AI] Correct slot for {playerCard.cardName} is occupied. Placing randomly.");
                PlaceCardInSlot(card, GetRandomSlot(availableSlots));
            }
        }
    }

    private void PlaceRandomCards(int remainingCost, int slotsToFill, List<GameObject> availableSlots)
    {
        Debug.Log($"[PlaceRandomCards] Starting - Remaining Cost: {remainingCost}, Slots to Fill: {slotsToFill}, Available Slots: {availableSlots.Count}");

        while (slotsToFill > 0 && availableSlots.Count > 0)
        {
            // 🔥 Update available slots dynamically
            availableSlots = boardManager.enemyReserveSlots.FindAll(slot => !slot.GetComponent<EnemyCardSlot>().IsOccupied());

            Card selectedCard = GetRandomCardFromHand(remainingCost);

            if (selectedCard == null)
            {
                Debug.LogWarning($"[PlaceRandomCards] No card found for cost: {remainingCost}. Stopping placement.");
                break;
            }

            GameObject selectedSlot = GetRandomSlot(availableSlots);

            if (selectedSlot == null)
            {
                Debug.LogWarning($"[PlaceRandomCards] No available slot found. Breaking loop.");
                break;
            }

            Debug.Log($"[PlaceRandomCards] Placing {selectedCard.cardName} in {selectedSlot.name}");
            
            // Attempt to place the card
            bool placedSuccessfully = PlaceCardInSlot(selectedCard, selectedSlot);

            if (placedSuccessfully)
            {
                enemyHand.Remove(selectedCard);
                remainingCost -= selectedCard.cost;
                slotsToFill--;
            }
            else
            {
                Debug.LogWarning($"[PlaceRandomCards] Failed to place {selectedCard.cardName}. Returning to hand.");
            }
        }

        Debug.Log($"[PlaceRandomCards] Completed. Cards in hand: {enemyHand.Count}, Available Slots: {availableSlots.Count}");
    }



    private Card GetRandomCardFromHand(int remainingCost)
    {
        List<Card> affordableCards = enemyHand.FindAll(c => c.cost <= remainingCost);
        return affordableCards.Count > 0 ? affordableCards[UnityEngine.Random.Range(0, affordableCards.Count)] : null;
    }

    private bool PlaceCardInSlot(Card card, GameObject slot)
    {
        Debug.Log($"[PlaceCardInSlot] Attempting to place {card.cardName} in {slot.name}");

        var enemySlot = slot.GetComponent<EnemyCardSlot>();
        if (enemySlot != null && !enemySlot.IsOccupied())
        {
            GameObject cardObject = InstantiateCardObject(card);

            if (cardObject == null)
            {
                Debug.LogError($"[PlaceCardInSlot] Failed to instantiate card: {card.cardName}");
                return false;
            }

            enemySlot.PlaceCard(cardObject);

            Debug.Log($"[PlaceCardInSlot] Placed {card.cardName} in {slot.name}");
            return true;
        }
        else
        {
            Debug.LogWarning($"[PlaceCardInSlot] Slot {slot.name} is already occupied or invalid.");
            return false;
        }
    }

    private GameObject GetRandomSlot(List<GameObject> availableSlots)
    {
        return availableSlots[UnityEngine.Random.Range(0, availableSlots.Count)];
    }

    private GameObject InstantiateCardObject(Card card)
    {
        GameObject newCard = Instantiate(enemyCardPrefab);
        newCard.GetComponent<CardViz>().LoadCard(card);
        return newCard;
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private bool ShouldCounterPlayer()
    {
        return Random.value < 0.9f;
    }

    public void ReturnHandToDeck()
    {
        enemyDeckManager.ReturnHandToDeck(enemyHand);
    }
}
