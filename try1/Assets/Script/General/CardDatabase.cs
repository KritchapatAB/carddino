using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public List<Card> cardAssets;
    public List<Card> cards = new();

    void Start()
    {
        cards.AddRange(cardAssets); // Populate cards list from cardAssets
        Debug.Log($"Cards in database: {cards.Count}");
    }

    // Get a card by its unique ID
    public Card GetCardById(int id)
    {
        return cardAssets.Find(card => card.id == id);
    }
}

