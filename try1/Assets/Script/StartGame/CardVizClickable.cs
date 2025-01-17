using UnityEngine;
using UnityEngine.EventSystems;

public class CardVizClickable : MonoBehaviour, IPointerClickHandler
{
    public StartGameManager startGameManager;
    public GameObject highlight; 

    private void Start()
    {
        if (highlight != null)
        {
            highlight.SetActive(false); // Hide the highlight initially
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (startGameManager == null) return;

        var card = GetComponent<CardViz>().GetCardData();

        if (highlight.activeSelf)
        {
            startGameManager.DeselectCard(card);
            highlight.SetActive(false);
        }
        else
        {
            if (startGameManager.SelectCard(card))
            {
                highlight.SetActive(true);
            }
        }
    }
}
