using UnityEngine;

public class CardInstance
{
    public Card cardData { get; private set; }
    public int currentHealth { get; private set; }
    public int currentDamage { get; private set; }
    private GameObject cardObject; // ✅ Store the actual GameObject
    private CardViz cardViz; // ✅ Store reference to CardViz

    public CardInstance(Card card, GameObject obj)
    {
        cardData = card;
        currentHealth = card.health;
        currentDamage = card.damage;
        cardObject = obj;
        cardViz = cardObject.GetComponent<CardViz>(); // ✅ Store CardViz reference
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{cardData.cardName} took {amount} damage! Remaining HP: {currentHealth}");

        if (cardViz != null)
        {
            cardViz.UpdateHealthUI(currentHealth); // ✅ Fix: Use CardViz function
            cardViz.FlashDamageEffect(); // ✅ Fix: Run coroutine in MonoBehaviour
        }

        if (currentHealth <= 0)
        {
            Debug.Log($"{cardData.cardName} has been destroyed!");
            DestroyCard();
        }
    }

    private void DestroyCard()
    {
        if (cardObject != null)
        {
            var slot = cardObject.transform.parent.GetComponent<CardSlot>();
            if (slot != null)
            {
                slot.SetOccupied(false); // ✅ Mark the slot as unoccupied
            }
            GameObject.Destroy(cardObject);
        }
    }
}

