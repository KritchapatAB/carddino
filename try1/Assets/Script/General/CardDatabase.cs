using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public List<Card> cardAssets;
    public List<Card> cards = new();

    void Start()
    {
        cards.AddRange(cardAssets); // Populate cards list from cardAssets
        ShuffleCards(cards); // ✅ Shuffle to ensure true randomness
        Debug.Log($"Cards in database: {cards.Count}");
    }

    // ✅ Helper function to shuffle a list
    private void ShuffleCards(List<Card> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]); // Swap elements
        }
    }

    // Get a card by its unique ID
    public Card GetCardById(int id)
    {
        return cardAssets.Find(card => card.id == id);
    }
}

