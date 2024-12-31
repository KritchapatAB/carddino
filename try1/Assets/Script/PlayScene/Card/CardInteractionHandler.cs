using UnityEngine;
using UnityEngine.EventSystems;

public class CardInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Card cardData; // The card's data
    public enum CardState { InHand, OnBoard, Placing }
    public CardState currentState;

    private Vector3 originalScale;
    private bool hoverEnabled = true; // Determines whether hover effects are enabled

    [Header("Scale Settings")]
    public float hoverScaleMultiplier = 1.2f; // Scale multiplier for hover
    public float selectedScaleMultiplier = 1.2f; // Scale multiplier for selection

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void SetCardState(CardState newState)
    {
        currentState = newState;
        Debug.Log($"Card state updated to: {newState}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverEnabled && currentState == CardState.InHand)
        {
            HoverEffect();
        }
        else if (hoverEnabled && currentState == CardState.OnBoard)
        {
            BoardHoverEffect();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverEnabled)
        {
            ResetScale();
        }
    }

    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     if (currentState == CardState.InHand)
    //     {
    //         FindObjectOfType<PlayerHand>().SelectCard(gameObject);
    //     }
    //     else if (currentState == CardState.OnBoard)
    //     {
    //         FindObjectOfType<BoardManager>().SelectCardForSacrifice(gameObject);
    //     }
    // }
    public void OnPointerClick(PointerEventData eventData)
{
    if (currentState == CardState.InHand)
    {
        FindObjectOfType<PlayerHand>().SelectCard(gameObject);
    }
    else if (currentState == CardState.OnBoard)
    {
        FindObjectOfType<BoardManager>().SelectCardForSacrifice(gameObject);
    }
}


    private void HoverEffect()
    {
        transform.localScale = originalScale * hoverScaleMultiplier;
    }

    private void BoardHoverEffect()
    {
        transform.localScale = originalScale * 1.1f; // Slightly smaller than selection or hover
    }

    private void ResetScale()
    {
        transform.localScale = originalScale;
    }

    public void DisableHoverEffect()
    {
        hoverEnabled = false;
    }

    public void EnableHoverEffect()
    {
        hoverEnabled = true;
    }

    public void Highlight()
    {
        // Change size or color for sacrifice highlight
        GetComponent<SpriteRenderer>().color = Color.red; // Change color to red
        transform.localScale = Vector3.one * 0.8f; // Scale down slightly
    }

    public void Deselect()
    {
        // Reset size or color
        GetComponent<SpriteRenderer>().color = Color.white; // Reset color
        transform.localScale = originalScale; // Reset scale
}


    public void ScaleUpForSelection()
    {
        transform.localScale = originalScale * selectedScaleMultiplier;
    }
}
