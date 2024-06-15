using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 initialPosition;
    private Transform originalParent;
    private Canvas canvas;
    private bool isHovering = false;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        initialPosition = transform.position;
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = transform.position;
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsValidDropArea())
        {
            transform.position = initialPosition;
            transform.SetParent(originalParent, true);
        }
        else
        {
            initialPosition = transform.position;
        }
    }

    private bool IsValidDropArea()
    {
        // Example logic to check if dropped in a valid area
        return false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Implement hover effect here, e.g., change scale or color
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Remove hover effect here, e.g., revert scale or color
        transform.localScale = Vector3.one;
        isHovering = false;
    }
}


