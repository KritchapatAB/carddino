using UnityEngine;

public class DeleteCard : MonoBehaviour
{
    public void DeleteCardFromDeck(int cardId)
    {
        GameManager.Instance.RemoveFromPlayerDeck(cardId);
        Debug.Log($"Deleted card {cardId} from deck.");
    }

    private void OnDisable()
    {
        GameManager.Instance.SaveData(); // Save changes when leaving
    }
}

