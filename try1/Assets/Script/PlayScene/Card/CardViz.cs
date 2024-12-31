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

    private Card cardData; // Reference to the card data

    public void LoadCard(Card card)
    {
        if (card == null)
        {
            Debug.LogWarning("Null card data provided.");
            return;
        }

        cardData = card; // Store the card data reference
        UpdateCardUI();
    }

    private void UpdateCardUI()
    {
        if (cardData == null) return;

        cardNameText.text = cardData.cardName;
        costText.text = cardData.cost.ToString();
        damageText.text = cardData.damage.ToString();
        healthText.text = cardData.health.ToString();
        dinoImage.sprite = cardData.dinoImage;
        dinoClass.sprite = cardData.cardClass;
    }

    public Card GetCardData()
    {
        return cardData; // Provide access to the card data
    }
}
