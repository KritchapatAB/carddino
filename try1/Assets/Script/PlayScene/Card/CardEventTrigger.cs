using UnityEngine;
using UnityEngine.EventSystems;

public class CardEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private PlayerHand playerHand;

    private void Start()
    {
        playerHand = FindObjectOfType<PlayerHand>();
        if (playerHand == null)
        {
            Debug.LogError("PlayerHand component not found in the scene.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playerHand != null)
        {
            playerHand.OnCardHover(gameObject); // Handle hover enter
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (playerHand != null)
        {
            playerHand.OnCardHoverExit(gameObject); // Handle hover exit
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playerHand != null)
        {
            playerHand.OnCardClick(gameObject); // Handle card click
        }
    }
}
