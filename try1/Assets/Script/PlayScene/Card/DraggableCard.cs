using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 initialPosition;
    private Transform originalParent;
    private Canvas canvas;

    public delegate void DragAction();
    public static event DragAction OnDragStart;
    public static event DragAction OnDragEnd;

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
        OnDragStart?.Invoke();
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
        OnDragEnd?.Invoke();
    }

    private bool IsValidDropArea()
    {
        return false; // Replace with your logic for valid drop areas
    }
}





