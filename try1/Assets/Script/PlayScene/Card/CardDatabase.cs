using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public List<Card> cardAssets;
    public List<Card> cards = new List<Card>();

    public Card GetCardById(int id)
    {
        return cardAssets.Find(card => card.id == id);
    }

    void Start()
    {
        cards = new List<Card>(cardAssets); // Populate cards list from cardAssets.
        Debug.Log("Cards in database: " + cards.Count);
    }
}

