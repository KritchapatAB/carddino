using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThisCard : MonoBehaviour
{
    public List<Card> thisCard = new List<Card>();
    public int thisId;

    public int id;
    public string cardname;
    public int cost;
    public int damage;
    public int health;
    public Sprite dinoImage;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI healthText;
    public Image dinoArtImage;

    public bool cardBack;
    public static bool staticCardBack;

    public GameObject Hand;

    public int numberOfCardInDeck;

    public bool canBeSummon;
    public bool summoned;
    public GameObject battleZone;

    private TurnSystem turnSystem; // Reference to the TurnSystem instance

    void Start()
    {
        thisCard[0] = CardDatabase.cardList[thisId];
        numberOfCardInDeck = PlayerDeck.deckSize;

        canBeSummon = false;
        summoned = false;

        // Try finding the TurnSystem object and handle potential absence
        turnSystem = GameObject.FindObjectOfType<TurnSystem>();
        if (turnSystem == null)
        {
            Debug.LogError("TurnSystem not found in the scene. Please ensure it exists.");
            return; // Exit Start() if TurnSystem is missing
        }
    }

    void Update()
    {
        // Try finding the Hand object and handle potential absence
        Hand = GameObject.Find("Hand");
        if (Hand == null)
        {
            Debug.LogError("Hand object not found in the scene. Please ensure it exists.");
            return; // Exit Update() if Hand is missing
        }

        if (this.transform.parent == Hand.transform)
        {
            cardBack = false;
        }

        id = thisCard[0].id;
        cardname = thisCard[0].cardname;
        cost = thisCard[0].cost;
        damage = thisCard[0].damage;
        health = thisCard[0].health;
        dinoImage = thisCard[0].dinoImage;

        nameText.text = " " + cardname;
        costText.text = " " + cost;
        damageText.text = " " + damage;
        healthText.text = " " + health;
        dinoArtImage.sprite = dinoImage;

        staticCardBack = cardBack;

        if (this.tag == "Clone")
        {
            thisCard[0] = PlayerDeck.staticDeck[numberOfCardInDeck - 1];
            numberOfCardInDeck -= 1;
            PlayerDeck.deckSize -= 1;
            cardBack = false;
            this.tag = "Untagged";
        }

        // Access currentMana only if turnSystem is not null (prevents error)

    }

    public void Summon()
    {
        if (turnSystem != null) // Check for turnSystem before accessing currentMana
        {
            turnSystem.currentMana -= cost;
        }
        summoned = true;
    }
}
