using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBackPrefab : MonoBehaviour
{
    public GameObject Deck;
    public GameObject It;

    void Start()
    {
    }

    void Update()
    {
        Deck = GameObject.Find("DeckPanel");
        It.transform.SetParent(Deck.transform);
        It.transform.localScale = new Vector3(1f, 1.1f, 1.05f);
        It.transform.position = new Vector3(transform.position.x, transform.position.y, -48);
        It.transform.eulerAngles = new Vector3(25, 0, 0);
    }
}
