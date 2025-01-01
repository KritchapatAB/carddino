using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BoardManager : MonoBehaviour
{
    public List<GameObject> playerSlots = new();
    public TextMeshProUGUI sacrificeProgressText;

    private List<GameObject> cardsToSacrifice = new();
    private int requiredSacrifices;
    private GameObject cardToPlace;
    private bool isSacrificePhaseActive = false;

    public void EnableSacrificePhase(int cost, GameObject card)
    {
        requiredSacrifices = cost;
        cardToPlace = card;
        isSacrificePhaseActive = true;

        HighlightAvailableCards();
        UpdateSacrificeProgressText();
    }


    public void DisableSacrificePhase()
{
    Debug.Log("Disabling sacrifice phase.");
    ResetOnBoardCardsScale(); // Reset all on-board cards
    ClearSacrificeData();
}

    public void ProceedToPlacement()
    {
        if (cardsToSacrifice.Count < requiredSacrifices)
        {
            Debug.LogWarning("Not enough sacrifices made.");
            return;
        }

        HighlightEmptySlots();
        ClearSacrificeProgressText();
    }



public void DestroySacrificedCards()
{
    foreach (var card in cardsToSacrifice)
    {
        var cardSlot = card.transform.parent?.GetComponent<CardSlot>();
        if (cardSlot != null)
        {
            cardSlot.SetOccupied(false); // Mark the slot as unoccupied
        }

        Destroy(card); // Remove the card from the scene
        Debug.Log($"Destroyed sacrificed card: {card.name}");
    }

    cardsToSacrifice.Clear(); // Clear the sacrifice list
}


    public void SelectCardForSacrifice(GameObject card)
    {
        if (!isSacrificePhaseActive)
        {
            Debug.LogWarning("Cannot select card for sacrifice. Sacrifice phase is not active.");
            return;
        }

        var interactionHandler = card.GetComponent<CardInteractionHandler>();
        if (interactionHandler == null || interactionHandler.currentState != CardInteractionHandler.CardState.OnBoard)
        {
            Debug.LogWarning("Only on-board cards can be sacrificed.");
            return;
        }

        if (cardsToSacrifice.Contains(card))
        {
            cardsToSacrifice.Remove(card);
            interactionHandler.DeselectSacrifireAble();
        }
        else if (cardsToSacrifice.Count < requiredSacrifices)
        {
            cardsToSacrifice.Add(card);
            interactionHandler.HighlightSelectedForSacrifice();
        }

        UpdateSacrificeProgressText();

        if (cardsToSacrifice.Count == requiredSacrifices)
        {
            ProceedToPlacement();
        }
    }


    public bool AreSacrificesComplete() => cardsToSacrifice.Count >= requiredSacrifices;



    private void ResetOnBoardCardsScale()
{
    foreach (var handler in FindObjectsOfType<CardInteractionHandler>())
    {
        handler.ResetToDefault();
    }
}


    private void HighlightAvailableCards()
{
    foreach (var handler in FindObjectsOfType<CardInteractionHandler>())
    {
        if (handler.currentState == CardInteractionHandler.CardState.OnBoard)
        {
            handler.HighlightSacrifireAble();
        }
    }
}


    

    public void HighlightEmptySlots()
    {
        foreach (var slot in playerSlots)
        {
            var cardSlot = slot.GetComponent<CardSlot>();
            if (cardSlot != null && !cardSlot.IsOccupied())
            {
                cardSlot.HighlightSlot();
            }
        }
    }

    private void ClearSacrificeData()
    {
        cardsToSacrifice.Clear();
        ClearSacrificeProgressText();
        isSacrificePhaseActive = false;
        cardToPlace = null;
    }

    private void UpdateSacrificeProgressText()
    {
        if (sacrificeProgressText != null)
        {
            sacrificeProgressText.text = $"{cardsToSacrifice.Count}/{requiredSacrifices} sacrifices selected";
        }
    }

    private void ClearSacrificeProgressText()
    {
        if (sacrificeProgressText != null)
        {
            sacrificeProgressText.text = string.Empty;
        }
    }

    public bool IsSacrificePhaseActive() => isSacrificePhaseActive;
}
