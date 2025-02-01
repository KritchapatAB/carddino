using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollRectBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEngine.UI.ScrollRect scrollRect;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("YES");
        scrollRect.enabled = false; // Disable scrolling when pointer is over the card
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("NO");
        scrollRect.enabled = true; // Re-enable scrolling when pointer leaves the card
    }
}
