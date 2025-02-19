// using UnityEngine;

// public class CardInstance
// {
//     public Card baseCard; // Reference to the original Card ScriptableObject

//     public int currentHealth;
//     public int currentDamage;
//     public int currentCost;

//     public CardInstance(Card card)
//     {
//         baseCard = card;
//         currentHealth = card.health;
//         currentDamage = card.damage;
//         currentCost = card.cost;
//     }

//     public bool IsDead()
//     {
//         return currentHealth <= 0;
//     }

//     public void TakeDamage(int amount)
//     {
//         currentHealth -= amount;
//         Debug.Log($"[{baseCard.cardName}] Took {amount} damage. Remaining HP: {currentHealth}");
//     }

//     public void ApplyBuff(int damageBoost, int healthBoost)
//     {
//         currentDamage += damageBoost;
//         currentHealth += healthBoost;
//         Debug.Log($"[{baseCard.cardName}] Buffed! Damage: {currentDamage}, Health: {currentHealth}");
//     }
// }
