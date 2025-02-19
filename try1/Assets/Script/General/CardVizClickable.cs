using UnityEngine;
using UnityEngine.EventSystems;

public class CardVizClickable : MonoBehaviour, IPointerClickHandler
{
    public ICardSelectionHandler selectionHandler;
    public GameObject highlight;

    private CardInstance cardInstance; // ✅ Using CardInstance

    private void Start()
    {
        if (highlight != null)
        {
            highlight.SetActive(false);
        }

        cardInstance = GetComponent<CardViz>().GetCardInstance(); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectionHandler == null || cardInstance == null) return;

        if (highlight.activeSelf)
        {
            selectionHandler.DeselectCard(cardInstance.cardData); // ✅ Pass only `cardData`
            highlight.SetActive(false);
        }
        else
        {
            if (selectionHandler.SelectCard(cardInstance.cardData)) // ✅ Pass only `cardData`
            {
                highlight.SetActive(true);
            }
        }
    }
}
