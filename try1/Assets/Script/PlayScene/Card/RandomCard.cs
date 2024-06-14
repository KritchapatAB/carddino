using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCard : MonoBehaviour
{
    public Card DrawRandomCard(List<Card> cards)
    {
        if (cards.Count > 0)
        {
            int randomIndex = Random.Range(0, cards.Count);
            Card randomCard = cards[randomIndex];
            Debug.Log("Random card drawn: " + randomCard.cardName);
            return randomCard;
        }
        else
        {
            Debug.LogWarning("No more cards left to draw!");
            return null;
        }
    }

    public Card DrawAndRemoveRandomCard(List<Card> cards)
    {
        if (cards.Count > 0)
        {
            int randomIndex = Random.Range(0, cards.Count);
            Card randomCard = cards[randomIndex];
            cards.RemoveAt(randomIndex);
            Debug.Log("Random card drawn and removed: " + randomCard.cardName);
            return randomCard;
        }
        else
        {
            Debug.LogWarning("No more cards left to draw!");
            return null;
        }
    }
}
