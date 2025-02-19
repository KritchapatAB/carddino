using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class CardViz : MonoBehaviour
{
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI healthText;
    public Image dinoImage;
    public Image dinoClass;

    private CardInstance cardInstance; 

    public void LoadCard(Card card)
    {
        if (card == null)
        {
            Debug.LogWarning("Null card data provided.");
            return;
        }

        cardInstance = new CardInstance(card, gameObject);
        UpdateCardUI();
    }

    public void LoadCard(CardInstance instance)
    {
        if (instance == null)
        {
            Debug.LogWarning("Null card instance provided.");
            return;
        }

        cardInstance = instance;
        UpdateCardUI();
    }

    private void UpdateCardUI()
    {
        if (cardInstance == null) return;

        cardNameText.text = cardInstance.cardData.cardName;
        costText.text = cardInstance.cardData.cost.ToString();
        damageText.text = cardInstance.currentDamage.ToString(); 
        healthText.text = cardInstance.currentHealth.ToString(); 
        dinoImage.sprite = cardInstance.cardData.dinoImage;
        dinoClass.sprite = cardInstance.cardData.cardClass;
    }

    public void UpdateHealthUI(int newHealth)
    {
        healthText.text = newHealth.ToString(); // ✅ Fix: Update health display
    }

    public void FlashDamageEffect()
    {
        StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        healthText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        healthText.color = Color.white;
    }

    public CardInstance GetCardInstance()
    {
        if (cardInstance == null)
        {
            Debug.LogWarning($"⚠️ CardInstance is NULL in CardViz on {gameObject.name}");
        }
        else
        {
            Debug.Log($"✅ CardViz returning: {cardInstance.cardData.cardName}");
        }
        return cardInstance;
    }
}
