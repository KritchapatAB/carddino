// using UnityEngine;
// using UnityEngine.EventSystems;

// public class CardInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
// {
//     public void OnPointerEnter(PointerEventData eventData)
//     {
//         transform.localScale = Vector3.one * 1.1f; // Slightly enlarge the card on hover
//     }

//     public void OnPointerExit(PointerEventData eventData)
//     {
//         transform.localScale = Vector3.one; // Reset size on hover exit
//     }

//     public void OnPointerClick(PointerEventData eventData)
//     {
//         Debug.Log($"Card clicked: {name}");
//         // Call a method in your PlayerHand script to handle card placement
//         PlayerHand.Instance.PlaceCardOnField(gameObject);
//     }
// }
