using System.Collections.Generic;
using UnityEngine;

public class EnemyDeckManager : MonoBehaviour
{
    public CardDatabase cardDatabase;  // Reference to the CardDatabase

    [SerializeField]
    private List<Card> enemyDeck = new();

    public void InitializeEnemyDeck(StageConfiguration stageConfig)
    {
        enemyDeck.Clear();

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

        // Add specific cards by ID
        AddSpecificCards(stageConfig.specificCards);

        // Add random cards based on type and cost
        FillDeckWithRandomCards("Attacker", stageConfig.maxAttackers, stageConfig.minAttackerCost, stageConfig.maxAttackerCost);
        FillDeckWithRandomCards("Defender", stageConfig.maxDefenders, stageConfig.minDefenderCost, stageConfig.maxDefenderCost);
        FillDeckWithRandomCards("Normal", stageConfig.maxNormals, stageConfig.minNormalCost, stageConfig.maxNormalCost);

        // Ensure deck doesn't exceed maximum size
        TrimDeckToMaxSize(stageConfig.maxDeckSize);

        Debug.Log($"Enemy deck initialized with {enemyDeck.Count} cards.");
    }

    private void AddSpecificCards(List<SpecificCardEntry> specificCards)
    {
        foreach (var entry in specificCards)
        {
            Card card = cardDatabase.GetCardById(entry.cardId);
            if (card == null)
            {
                Debug.LogWarning($"Card with ID {entry.cardId} not found in CardDatabase!");
                continue;
            }

            for (int i = 0; i < entry.quantity; i++)
            {
                enemyDeck.Add(card);
                Debug.Log($"Added card {card.cardName} (ID: {card.id}) to enemy deck.");
            }
        }
    }

    // Add random cards of a specific type and cost to the deck
    private void FillDeckWithRandomCards(string cardType, int maxCount, int minCost, int maxCost)
    {
        // Filter cards by type, cost, and CardID range
        List<Card> filteredCards = cardDatabase.cards.FindAll(
            card => card.dinoType == cardType &&
                    card.cost >= minCost &&
                    card.cost <= maxCost &&
                    card.id >= 0 && card.id <= 19); // Ensure CardID is between 0 and 19

        int cardsToAdd = maxCount;

        while (cardsToAdd > 0 && filteredCards.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredCards.Count);
            enemyDeck.Add(filteredCards[randomIndex]);
            cardsToAdd--;
        }
    }


    private void TrimDeckToMaxSize(int maxSize)
    {
        if (enemyDeck.Count > maxSize)
        {
            enemyDeck.RemoveRange(maxSize, enemyDeck.Count - maxSize);
            Debug.Log($"Trimmed enemy deck to max size: {maxSize}.");
        }
    }

    public Card PeekNextCard()
    {
        if (enemyDeck.Count == 0)
        {
            Debug.LogWarning("Enemy deck is empty! Cannot peek any cards.");
            return null;
        }

        return enemyDeck[0];
    }

    public Card TakeNextCard()
    {
        if (enemyDeck.Count == 0)
        {
            Debug.LogWarning("Enemy deck is empty! Cannot take any cards.");
            return null;
        }

        Card nextCard = enemyDeck[0];
        enemyDeck.RemoveAt(0);
        return nextCard;
    }

    public bool IsDeckEmpty()
    {
        return enemyDeck.Count == 0;
    }
}
