// using System.Collections.Generic;
// using UnityEngine;

// public class EnemyManager : MonoBehaviour
// {
//     public EnemyDeckManager enemyDeckManager;
//     public BoardManager boardManager;
//     public int maxCardsPerTurn = 3;
//     private int enemyCostLimit;
//     private int turnCounter = 0;
//     public List<Card> enemyHand = new();
//     private System.Random random = new();

//     public CardDatabase cardDatabase;
//     public GameObject enemyCardPrefab;

//     public void StartTurn()
//     {
//         turnCounter++;
//         enemyHand.Clear();
//         enemyDeckManager.ShuffleDeck(); // Ensure the deck is shuffled before drawing

//         for (int i = 0; i < 5; i++)
//         {
//             if (!enemyDeckManager.IsDeckEmpty())
//             {
//                 enemyHand.Add(enemyDeckManager.TakeNextCard());
//             }
//             else
//             {
//                 Debug.LogWarning("Enemy deck is empty! AI will continue with fewer cards.");
//                 break;
//             }
//         }
//         Debug.Log($"[Enemy AI] Turn {turnCounter}: AI drew {enemyHand.Count} cards.");
//         enemyCostLimit = GetCostLimit();
//         PlaceCards();
//     }

//     private int GetCostLimit()
//     {
//         switch (GameManager.Instance.CurrentStage.difficulty)
//         {
//             case Difficulty.Easy: return 3;
//             case Difficulty.Normal: return 5;
//             case Difficulty.Hard: return 7;
//             default: return 5;
//         }
//     }

//     private void PlaceCards()
//     {
//         int placedCards = 0;
//         int remainingCost = enemyCostLimit;
//         List<GameObject> availableSlots = boardManager.enemyReserveSlots.FindAll(slot => !slot.GetComponent<EnemyCardSlot>().IsOccupied());
        
//         // Get strongest player cards
//         Card strongestAttacker;
//         Card strongestDefender;
//         boardManager.GetStrongestPlayerCards(out strongestAttacker, out strongestDefender);

//         // Decide highest priority target (Attacker or Defender)
//         Card highestPriorityTarget = null;
//         string counterType = "";

//         if (strongestAttacker != null && strongestDefender != null)
//         {
//             if (strongestAttacker.cost > strongestDefender.cost)
//             {
//                 highestPriorityTarget = strongestAttacker;
//                 counterType = "Attacker";
//             }
//             else if (strongestDefender.cost > strongestAttacker.cost)
//             {
//                 highestPriorityTarget = strongestDefender;
//                 counterType = "Defender";
//             }
//             else
//             {
//                 highestPriorityTarget = random.Next(0, 2) == 0 ? strongestAttacker : strongestDefender;
//                 counterType = highestPriorityTarget == strongestAttacker ? "Attacker" : "Defender";
//             }
//         }
//         else if (strongestAttacker != null)
//         {
//             highestPriorityTarget = strongestAttacker;
//             counterType = "Attacker";
//         }
//         else if (strongestDefender != null)
//         {
//             highestPriorityTarget = strongestDefender;
//             counterType = "Defender";
//         }

//         Debug.Log($"[Enemy AI] Prioritizing Counter: {highestPriorityTarget?.cardName ?? "None"} ({counterType})");

//         if (highestPriorityTarget != null)
//         {
//             if (counterType == "Attacker")
//             {
//                 Card counterCard = enemyHand.Find(c => c.dinoType == "Attacker" && c.damage > highestPriorityTarget.health && c.cost <= remainingCost);
//                 if (counterCard != null)
//                 {
//                     Debug.Log($"[Enemy AI] Countering {highestPriorityTarget.cardName} with {counterCard.cardName}");
//                     PlaceCardInCorrectSlot(counterCard, availableSlots, highestPriorityTarget);
//                     remainingCost -= counterCard.cost;
//                     placedCards++;
//                     enemyHand.Remove(counterCard);
//                 }
//                 else
//                 {
//                     Debug.Log($"[Enemy AI] No strong Attacker found, switching to defense.");
//                     Card counterDefender = enemyHand.Find(c => c.dinoType == "Defender" && c.cost <= remainingCost);
//                     if (counterDefender != null)
//                     {
//                         Debug.Log($"[Enemy AI] Placing Defender instead: {counterDefender.cardName}");
//                         PlaceCardInCorrectSlot(counterDefender, availableSlots, highestPriorityTarget);
//                         remainingCost -= counterDefender.cost;
//                         placedCards++;
//                         enemyHand.Remove(counterDefender);
//                     }
//                 }
//             }
//             else if (counterType == "Defender")
//             {
//                 Card bestAttacker = enemyHand.Find(c => c.dinoType == "Attacker" && c.cost <= remainingCost);
//                 if (bestAttacker != null)
//                 {
//                     Debug.Log($"[Enemy AI] Countering {highestPriorityTarget.cardName} with {bestAttacker.cardName}");
//                     PlaceCardInCorrectSlot(bestAttacker, availableSlots, highestPriorityTarget);
//                     remainingCost -= bestAttacker.cost;
//                     placedCards++;
//                     enemyHand.Remove(bestAttacker);
//                 }
//             }
//         }

//         if (placedCards >= maxCardsPerTurn)
//         {
//             Debug.Log($"[Enemy AI] Stopping after placing {placedCards} cards.");
//             return;
//         }

//         // Place remaining random cards
//         Debug.Log($"[Enemy AI] Placing remaining cards.");
//         ShuffleList(availableSlots);
//         while (placedCards < maxCardsPerTurn && remainingCost > 0 && availableSlots.Count > 0)
//         {
//             Card selectedCard = GetRandomCardFromHand(remainingCost);
//             if (selectedCard == null) break;
//             PlaceCardInSlot(selectedCard, availableSlots[0]);
//             availableSlots.RemoveAt(0);
//             remainingCost -= selectedCard.cost;
//             placedCards++;
//             enemyHand.Remove(selectedCard);
//         }
//     }

// private void PlaceCardInCorrectSlot(Card card, List<GameObject> availableSlots, Card playerCard)
// {
//     if (availableSlots.Count == 0)
//     {
//         Debug.LogWarning($"[Enemy AI] No available slots to place {card.cardName}.");
//         return;
//     }

//     Debug.Log($"[Enemy AI] Checking for player slot of {playerCard.cardName}");

//     // 🔹 If the AI is countering a Defender, place the Attacker **ANYWHERE** in enemyReserveArea
//     if (playerCard.dinoType == "Defender")
//     {
//         Debug.Log($"[Enemy AI] {card.cardName} is attacking a Defender. Placing in ANY slot.");
//         PlaceCardInRandomSlot(card, availableSlots);
//         return;
//     }

//     // 🔹 If the AI is using a Defender to counter an Attacker, place **ANYWHERE** in enemyReserveArea
//     if (card.dinoType == "Defender")
//     {
//         Debug.Log($"[Enemy AI] {card.cardName} is a Defender. Placing it ANYWHERE.");
//         PlaceCardInRandomSlot(card, availableSlots);
//         return;
//     }

//     // 🔹 Find the correct slot where the player's card is placed (Only needed if attacking another Attacker)
//     int playerCardIndex = boardManager.playerSlots.FindIndex(slot => 
//     {
//         CardViz placedCard = null;

//         foreach (Transform child in slot.transform)
//         {
//             placedCard = child.GetComponent<CardViz>();
//             if (placedCard != null) break;
//         }

//         return placedCard?.GetCardData() == playerCard;
//     });

//     // 🔹 If AI cannot find the correct slot, pick a random slot instead
//     if (playerCardIndex == -1 || playerCardIndex >= boardManager.enemyReserveSlots.Count)
//     {
//         Debug.LogWarning($"[Enemy AI] Could not find matching slot for {playerCard.cardName}. Placing in a random slot instead.");
//         PlaceCardInRandomSlot(card, availableSlots);
//         return;
//     }

//     Debug.Log($"[Enemy AI] Targeting player slot {playerCardIndex} for {playerCard.cardName}");

//     // 🔹 Match the enemy slot to the player slot index
//     GameObject matchingEnemySlot = boardManager.enemyReserveSlots[playerCardIndex];

//     if (!matchingEnemySlot.GetComponent<EnemyCardSlot>().IsOccupied())
//     {
//         Debug.Log($"[Enemy AI] Placing {card.cardName} in slot {matchingEnemySlot.name} to counter {playerCard.cardName}");
//         PlaceCardInSlot(card, matchingEnemySlot);
//         availableSlots.Remove(matchingEnemySlot);
//     }
//     else
//     {
//         Debug.LogWarning($"[Enemy AI] Correct slot for {playerCard.cardName} is occupied. Placing in a random slot.");
//         PlaceCardInRandomSlot(card, availableSlots);
//     }
// }



//     private Card GetRandomCardFromHand(int remainingCost) 
//     {
//         List<Card> affordableCards = enemyHand.FindAll(c => c.cost <= remainingCost);
//         return affordableCards.Count > 0 ? affordableCards[UnityEngine.Random.Range(0, affordableCards.Count)] : null;
//     }

//     private void PlaceCardInSlot(Card card, GameObject slot)
//     {
//         var enemySlot = slot.GetComponent<EnemyCardSlot>();
//         if (enemySlot != null && !enemySlot.IsOccupied())
//         {
//             GameObject cardObject = InstantiateCardObject(card);
//             enemySlot.PlaceCard(cardObject);
//             Debug.Log($"[Enemy AI] Placing {card.cardName} in {slot.name} (Correct Position?)");
//         }
//     }

//     private void PlaceCardInRandomSlot(Card card, List<GameObject> availableSlots) 
// {
//     if (availableSlots.Count == 0)
//     {
//         Debug.LogWarning($"[Enemy AI] No available slots for {card.cardName}.");
//         return;
//     }

//     int randomIndex = UnityEngine.Random.Range(0, availableSlots.Count);
//     GameObject randomSlot = availableSlots[randomIndex];

//     Debug.Log($"[Enemy AI] Placing {card.cardName} in a random slot: {randomSlot.name}");
//     PlaceCardInSlot(card, randomSlot);
//     availableSlots.RemoveAt(randomIndex);
// }


//     private GameObject InstantiateCardObject(Card card)
//     {
//         GameObject newCard = Instantiate(enemyCardPrefab);
//         newCard.GetComponent<CardViz>().LoadCard(card);
//         return newCard;
//     }

//     private void ShuffleList<T>(List<T> list)
//     {
//         for (int i = list.Count - 1; i > 0; i--)
//         {
//             int j = UnityEngine.Random.Range(0, i + 1);
//             (list[i], list[j]) = (list[j], list[i]);
//         }
//     }
    
//     private bool ShouldCounterPlayer()
//     {
//         return Random.value < 0.9f;
//     }

//     public void ReturnHandToDeck()
//     {
//         enemyDeckManager.ReturnHandToDeck(enemyHand);
//     }
// }

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
        List<GameObject> availableSlots = boardManager.enemyReserveSlots.FindAll(slot => !slot.GetComponent<EnemyCardSlot>().IsOccupied());

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
            return placedCard?.GetCardData() == playerCard;
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
        while (slotsToFill > 0 && remainingCost > 0 && availableSlots.Count > 0)
        {
            Card selectedCard = GetRandomCardFromHand(remainingCost);
            if (selectedCard == null) break;

            PlaceCardInSlot(selectedCard, GetRandomSlot(availableSlots));
            remainingCost -= selectedCard.cost;
            slotsToFill--;
            enemyHand.Remove(selectedCard);
        }
    }

    private Card GetRandomCardFromHand(int remainingCost)
    {
        List<Card> affordableCards = enemyHand.FindAll(c => c.cost <= remainingCost);
        return affordableCards.Count > 0 ? affordableCards[UnityEngine.Random.Range(0, affordableCards.Count)] : null;
    }

    private void PlaceCardInSlot(Card card, GameObject slot)
    {
        var enemySlot = slot.GetComponent<EnemyCardSlot>();
        if (enemySlot != null && !enemySlot.IsOccupied())
        {
            GameObject cardObject = InstantiateCardObject(card);
            enemySlot.PlaceCard(cardObject);
            Debug.Log($"[Enemy AI] Placed {card.cardName} in {slot.name}");
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
