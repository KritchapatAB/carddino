using System.Collections.Generic;
using UnityEngine;

public class EnemyDeckManager : MonoBehaviour
{
    public CardDatabase cardDatabase;
    [SerializeField] private List<Card> enemyDeck = new();
    
    private Dictionary<int, List<Card>> bossAndSpecificCardSchedule = new(); // Turn-based lookup
    public bool hasBossOrSpecificCard { get; private set; } = false; // Flag for EnemyManager

    public void InitializeEnemyDeck(StageConfiguration stageConfig)
    {
        enemyDeck.Clear();
        bossAndSpecificCardSchedule.Clear();
        hasBossOrSpecificCard = false;

        if (stageConfig == null)
        {
            Debug.LogError("Stage configuration is missing!");
            return;
        }

        if (cardDatabase == null)
        {
            Debug.LogError("CardDatabase is not assigned!");
            return;
        }

        Debug.Log($"Initializing deck for stage: {stageConfig.stageName}");

        // Add specific cards and prepare schedule
        ProcessSpecificCards(stageConfig.specificCards);

        // Add random cards based on type and cost
        FillDeckWithRandomCards("Attacker", stageConfig.maxAttackers, stageConfig.minAttackerCost, stageConfig.maxAttackerCost);
        FillDeckWithRandomCards("Defender", stageConfig.maxDefenders, stageConfig.minDefenderCost, stageConfig.maxDefenderCost);
        FillDeckWithRandomCards("Normal", stageConfig.maxNormals, stageConfig.minNormalCost, stageConfig.maxNormalCost);

        // Apply Challenge Modifiers (Boss fights do NOT use these)
        if (stageConfig.stageType == StageType.Challenge)
        {
            ApplyChallengeModifiers();
        }

        TrimDeckToMaxSize(stageConfig.maxDeckSize);
        Debug.Log($"Enemy deck initialized with {enemyDeck.Count} cards.");
    }

    private void ProcessSpecificCards(List<SpecificCardEntry> specificCards)
    {
        foreach (var entry in specificCards)
        {
            Card card = cardDatabase.GetCardById(entry.cardId);
            if (card == null)
            {
                Debug.LogWarning($"Card with ID {entry.cardId} not found in CardDatabase!");
                continue;
            }

            if (!bossAndSpecificCardSchedule.ContainsKey(entry.turn))
            {
                bossAndSpecificCardSchedule[entry.turn] = new List<Card>();
            }

            for (int i = 0; i < entry.quantity; i++)
            {
                bossAndSpecificCardSchedule[entry.turn].Add(card);
                hasBossOrSpecificCard = true; // ✅ Now we know to check turns in EnemyManager
            }

            Debug.Log($"Scheduled {entry.quantity}x {card.cardName} (ID: {card.id}) for Turn {entry.turn}");
        }
    }

    public List<Card> GetCardsForTurn(int turn)
    {
        return bossAndSpecificCardSchedule.ContainsKey(turn) ? bossAndSpecificCardSchedule[turn] : new List<Card>();
    }

    private void ApplyChallengeModifiers()
    {
        for (int i = 0; i < enemyDeck.Count; i++)
        {
            Card modifiedCard = new Card(enemyDeck[i]); // Avoid modifying original database card
            modifiedCard.health += 2;
            modifiedCard.damage += 1;
            enemyDeck[i] = modifiedCard;
        }
        Debug.Log("Applied Challenge modifiers: +2 Health, +1 Damage.");
    }

    private void FillDeckWithRandomCards(string cardType, int maxCount, int minCost, int maxCost)
    {
        List<Card> filteredCards = cardDatabase.cards.FindAll(
            card => card.dinoType == cardType && card.cost >= minCost && card.cost <= maxCost
        );

        for (int i = 0; i < maxCount && filteredCards.Count > 0; i++)
        {
            enemyDeck.Add(filteredCards[Random.Range(0, filteredCards.Count)]);
        }
    }

    private void TrimDeckToMaxSize(int maxSize)
    {
        if (enemyDeck.Count > maxSize)
        {
            enemyDeck.RemoveRange(maxSize, enemyDeck.Count - maxSize);
        }
    }

    public Card TakeNextCard()
    {
        if (enemyDeck.Count == 0) return null;
        Card nextCard = enemyDeck[0];
        enemyDeck.RemoveAt(0);
        return nextCard;
    }

    public void ShuffleDeck()
    {
        for (int i = enemyDeck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (enemyDeck[i], enemyDeck[j]) = (enemyDeck[j], enemyDeck[i]);
        }
    }

    public bool IsDeckEmpty()
    {
        return enemyDeck.Count == 0;
    }

    public void ReturnHandToDeck(List<Card> enemyHand)
    {
        if (enemyHand.Count == 0)
        {
            Debug.Log("No cards to return to enemy deck.");
            return;
        }

        foreach (var card in enemyHand)
        {
            enemyDeck.Add(card);
        }

        enemyHand.Clear(); // ✅ Empty the enemy's hand

        Debug.Log($"Returned {enemyDeck.Count} cards back to the enemy deck.");
    }
}