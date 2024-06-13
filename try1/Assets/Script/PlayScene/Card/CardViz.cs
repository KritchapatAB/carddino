using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardViz : MonoBehaviour
{
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI healthText;
    public Image dinoImage;
    public Image dinoClass;

    public Card card;

    private void Start(){
        LoadCard(card);
    }

    private void LoadCard(Card c)
    {
        if (c== null)
            return;

        card = c;
        cardName.text = c.cardName;
        costText.text = c.cost.ToString();
        //if(string.IsNullorEmpty(c.cardEffect))
        //{effect.gameObject.SetActive(false);
        //} else {
        //wffect.gameObject.SetActive(true)
        //cardEffect.text = C.cardEffect;}
        damageText.text = c.damage.ToString();
        healthText.text = c.health.ToString();
        dinoImage.sprite = c.dinoImage;


    }

    void Update(){
   
    }
}