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
            // ✅ Separate variables for each type of slot
            var enemySlot = cardObject.transform.parent.GetComponent<EnemyCardSlot>();
            var playerSlot = cardObject.transform.parent.GetComponent<CardSlot>();

            // ✅ Check and clear EnemyCardSlot first
            if (enemySlot != null)
            {
                enemySlot.ClearSlot(); // ✅ Mark the Enemy slot as unoccupied
            }
            // ✅ If not an EnemyCardSlot, check for Player CardSlot
            else if (playerSlot != null)
            {
                playerSlot.SetOccupied(false); // ✅ Mark the Player slot as unoccupied
            }

            // ✅ Destroy the card GameObject
            GameObject.Destroy(cardObject);
            Debug.Log($"[DestroyCard] Destroyed {cardData.cardName}");
        }
    }

}

