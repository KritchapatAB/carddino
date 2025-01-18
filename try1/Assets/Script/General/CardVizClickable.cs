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
