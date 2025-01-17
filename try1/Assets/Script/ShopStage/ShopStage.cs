using UnityEngine;

public class ShopStage : MonoBehaviour
{
    public void BuyCard(int cardId, int cost)
    {
        if (GameManager.Instance.CurrentSaveData.money >= cost)
        {
            GameManager.Instance.SubtractMoney(cost);
            GameManager.Instance.AddToPlayerDeck(cardId);
            Debug.Log($"Bought card {cardId}. Remaining money: {GameManager.Instance.CurrentSaveData.money}");
        }
        else
        {
            Debug.LogWarning("Not enough money.");
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.SaveData(); // Save changes when leaving the shop
    }
}
