using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();

    void Awake()
    {
        cardList.Add(new Card(0, "-", "-", 0, 0, 0, Resources.Load<Sprite>("0")));
        cardList.Add(new Card(1, "Dodo", "Normal", 0, 1, 1, Resources.Load<Sprite>("CardImage/Raptor")));
        cardList.Add(new Card(2, "Raptor", "Attacker", 1, 3, 1, Resources.Load<Sprite>("CardImage/Raptor"))); 
        cardList.Add(new Card(3, "Parasaur", "Defender", 2, 0, 4, Resources.Load<Sprite>("CardImage/parasaur")));  
    }
}
