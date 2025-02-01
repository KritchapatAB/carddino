// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;

// public class CardVizClickable : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler
// {
//     public ICardSelectionHandler selectionHandler; // Reference to any script implementing ICardSelectionHandler
//     public GameObject highlight; // Highlight GameObject for visual feedback

//     private Card cardData;

//     private bool isDragging;

//     private bool hasScrollRect;

//     private void Start()
//     {
//         // Check if there's a ScrollRect parent in the hierarchy
//         hasScrollRect = GetComponentInParent<ScrollRect>() != null;
//         Debug.LogWarning($"TestScroll {hasScrollRect}.");


//         if (highlight != null)
//         {
//             highlight.SetActive(false); // Hide highlight initially
//         }

//         cardData = GetComponent<CardViz>().GetCardData(); // Load card data
//     }

//     public void OnPointerClick(PointerEventData eventData)
//     {
//         Debug.LogWarning($"TestDrag {isDragging}.");
//         Debug.LogWarning($"TestScroll {hasScrollRect}.");
//         if (hasScrollRect && isDragging) return; // Ignore clicks if dragging is active in a scrollable context

//         if (selectionHandler == null || cardData == null) return;

//         if (highlight.activeSelf)
//         {
//             selectionHandler.DeselectCard(cardData);
//             Debug.LogWarning($"TestDelected {cardData}.");
//             highlight.SetActive(false);
//         }
//         else
//         {
//             if (selectionHandler.SelectCard(cardData))
//             {
//                 Debug.LogWarning($"TestSelected {cardData}.");
//                 highlight.SetActive(true);
//             }
//         }
//     }

//     public void OnBeginDrag(PointerEventData eventData)
//     {
//         isDragging = true; // Set dragging to true when a drag starts
//     }

//     public void OnEndDrag(PointerEventData eventData)
//     {
//         isDragging = false; // Reset dragging when the drag ends
//     }

// }
 
    // private void Start()
    // {
    //     if (highlight != null)
    //     {
    //         highlight.SetActive(false); // Hide highlight initially
    //     }

    //     cardData = GetComponent<CardViz>().GetCardData(); // Get card data from CardViz
    // }

    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     if (selectionHandler == null || cardData == null) return;

    //     if (highlight.activeSelf)
    //     {
    //         selectionHandler.DeselectCard(cardData); // Call DeselectCard
    //         highlight.SetActive(false); // Hide highlight
    //     }
    //     else
    //     {
    //         if (selectionHandler.SelectCard(cardData)) // Call SelectCard
    //         {
    //             highlight.SetActive(true); // Show highlight if selection succeeds
    //         }
    //     }
    // }
using UnityEngine;
using UnityEngine.EventSystems;

public class CardVizClickable : MonoBehaviour, IPointerClickHandler
{
    public ICardSelectionHandler selectionHandler; // Reference to any script implementing ICardSelectionHandler
    public GameObject highlight; // Highlight GameObject for visual feedback

    private Card cardData;

    private void Start()
    {
        if (highlight != null)
        {
            highlight.SetActive(false); // Hide highlight initially
        }

        cardData = GetComponent<CardViz>().GetCardData(); // Get card data from CardViz
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectionHandler == null || cardData == null) return;

        if (highlight.activeSelf)
        {
            selectionHandler.DeselectCard(cardData); // Call DeselectCard
            highlight.SetActive(false); // Hide highlight
        }
        else
        {
            if (selectionHandler.SelectCard(cardData)) // Call SelectCard
            {
                highlight.SetActive(true); // Show highlight if selection succeeds
            }
        }
    }
}