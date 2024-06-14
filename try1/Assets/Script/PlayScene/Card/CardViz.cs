using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardViz : MonoBehaviour
{
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI healthText;
    public Image dinoImage;
    public Image dinoClass;

    private Card card;  // Assuming you have a Card class defined

    public void LoadCard(Card cardData)
    {
        if (cardData == null)
        {
            Debug.LogWarning("Null card data provided.");
            return;
        }

        card = cardData;
        UpdateCardUI();
    }

    private void UpdateCardUI()
    {
        cardNameText.text = card.cardName;
        costText.text = card.cost.ToString();
        damageText.text = card.damage.ToString();
        healthText.text = card.health.ToString();
        dinoImage.sprite = card.dinoImage;
        dinoClass.sprite = card.cardClass;
    }
}




