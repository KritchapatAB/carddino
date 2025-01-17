using UnityEngine;
using UnityEngine.EventSystems;

public class DeckClickHandler : MonoBehaviour, IPointerClickHandler
{
    public PlayerHand playerHand;

    public void OnPointerClick(PointerEventData eventData)
    {   
        if (playerHand == null)
        {
            Debug.LogError("PlayerHand is not assigned to DeckClickHandler!");
            return;
        }
        playerHand.TryDrawCard();
    }
}
