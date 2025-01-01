using UnityEngine;
using UnityEngine.EventSystems;

public class CardInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Card cardData;
    public enum CardState { InHand, OnBoard, Placing }
    public CardState currentState;

    private Vector3 originalScale;
    private bool hoverEnabled = true;

    [Header("Scale Settings")]
    public float hoverScaleMultiplier = 1.2f;
    public float selectedScaleMultiplier = 1.2f;

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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverEnabled)
        {
            ResetScale();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var boardManager = FindObjectOfType<BoardManager>();
        var playerHand = FindObjectOfType<PlayerHand>();

        if (boardManager != null && boardManager.IsSacrificePhaseActive())
        {
            if (currentState == CardState.OnBoard)
            {
                boardManager.SelectCardForSacrifice(gameObject);
            }
            else
            {
                boardManager.DisableSacrificePhase();
                playerHand.SelectCard(gameObject);
            }
        }
        else if (currentState == CardState.InHand)
        {
            playerHand?.SelectCard(gameObject);
        }
    }

    private void HoverEffect()
    {
        transform.localScale = originalScale * hoverScaleMultiplier;
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

    public void HighlightSacrifireAble()
    {
        GetComponent<SpriteRenderer>().color = Color.yellow; // Indicate it's sacrificial
        transform.localScale = originalScale * 0.9f;
    }

    public void HighlightSelectedForSacrifice()
    {
        GetComponent<SpriteRenderer>().color = Color.red; // Strong highlight for selected
        transform.localScale = originalScale * 0.8f;
    }

    public void DeselectSacrifireAble()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        transform.localScale = originalScale * 0.9f; // Back to sacrificial highlight
    }

    public void ResetToDefault()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        ResetScale(); // Return to normal
    }

    public void ScaleUpForSelection()
    {
        transform.localScale = originalScale * selectedScaleMultiplier;
        DisableHoverEffect();
    }
}


