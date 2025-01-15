using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Required for Image component

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

    [Header("Highlight Settings")]
    public Image highlightOverlay; // Reference to the HighlightOverlay Image

    private void Start()
    {
        originalScale = transform.localScale;

        // Ensure the highlight overlay is hidden by default
        if (highlightOverlay != null)
        {
            highlightOverlay.enabled = false;
        }
    }

    public void SetCardState(CardState newState)
    {
        currentState = newState;
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
        var turnManager = FindObjectOfType<TurnManager>();

        if (turnManager.currentTurn != TurnManager.TurnState.PlayerTurn)
        {
            Debug.LogWarning("It's not the player's turn! Card interaction is disabled.");
            return;
        }

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
        if (highlightOverlay != null)
        {
            highlightOverlay.enabled = true;
            highlightOverlay.color = Color.yellow; // Indicate it's sacrificial
        }

        transform.localScale = originalScale * 0.9f;
    }

    public void HighlightSelectedForSacrifice()
    {
        if (highlightOverlay != null)
        {
            highlightOverlay.enabled = true;
            highlightOverlay.color = Color.red; // Strong highlight for selected
        }

        transform.localScale = originalScale * 0.8f;
    }

    public void DeselectSacrifireAble()
    {
        // Find the HighlightOverlay under CardTemplate
        var highlightOverlay = transform.Find("CardTemplate/HighlightOverlay")?.GetComponent<UnityEngine.UI.Image>();

        if (highlightOverlay == null)
        {
            Debug.LogError("HighlightOverlay or its Image component is missing!");
            return;
        }

        // Update the highlight color to "sacrifice-able"
        highlightOverlay.color = Color.yellow; 
        highlightOverlay.enabled = true; // Ensure the overlay is visible

        // Revert to "sacrifice-able" scaling
        transform.localScale = originalScale * 0.9f;
    }



    public void ResetToDefault()
    {
        if (highlightOverlay != null)
        {
            highlightOverlay.enabled = false;
        }

        ResetScale(); // Return to normal
    }

    public void ScaleUpForSelection()
    {
        transform.localScale = originalScale * selectedScaleMultiplier;
        DisableHoverEffect();
    }
}
