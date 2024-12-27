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

    public void LoadCard(Card card)
    {
        cardNameText.text = card.cardName;
        costText.text = card.cost.ToString();
        damageText.text = card.damage.ToString();
        healthText.text = card.health.ToString();
        dinoImage.sprite = card.dinoImage;
        dinoClass.sprite = card.cardClass;
    }
}
