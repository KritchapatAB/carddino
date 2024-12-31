// using UnityEngine;
// using System.Collections.Generic;
// using TMPro;

// public class BoardManager : MonoBehaviour
// {
//     public List<GameObject> playerSlots = new List<GameObject>(); // Player slots
//     private List<GameObject> cardsToSacrifice = new List<GameObject>(); // Cards selected for sacrifice
//     private int requiredSacrifices;
//     private GameObject cardToPlace; // Card being placed after sacrifices
//     public TextMeshProUGUI sacrificeProgressText;

//     public void EnableSacrificePhase(int cost, GameObject card)
//     {
//         requiredSacrifices = cost;
//         cardToPlace = card;
//         Debug.Log($"Sacrifice phase started. Need {cost} sacrifices for card: {card.name}");
//         HighlightAvailableCards(); // Highlight cards on the board that can be sacrificed
//     }

//     private void HighlightAvailableCards()
//     {
//         foreach (var card in FindObjectsOfType<CardInteractionHandler>())
//         {
//             if (card.currentState == CardInteractionHandler.CardState.OnBoard)
//             {
//                 card.Highlight(); // Visual feedback for sacrifice availability
//             }
//         }
//     }

//     public void ProceedToPlacement()
// {
//     if (cardsToSacrifice.Count < requiredSacrifices)
//     {
//         Debug.LogWarning("Not enough sacrifices made. Cannot proceed to placement.");
//         return;
//     }

//     Debug.Log($"Proceeding to placement. Sacrificing {cardsToSacrifice.Count} cards.");

//     foreach (var card in cardsToSacrifice)
//     {
//         Destroy(card); // Remove sacrificed cards
//     }

//     cardsToSacrifice.Clear();

//     if (cardToPlace != null)
//     {
//         HighlightEmptySlots(); // Allow placement of the card
//     }

//     if (sacrificeProgressText != null)
//     {
//         sacrificeProgressText.text = ""; // Clear sacrifice progress text
//     }
// }


//     public void HighlightEmptySlots()
//     {
//         foreach (var slot in playerSlots)
//         {
//             if (!slot.GetComponent<CardSlot>().IsOccupied())
//             {
//                 slot.GetComponent<CardSlot>().HighlightSlot(); // Visual feedback for empty slots
//             }
//         }
//     }

//     public void SelectCardForSacrifice(GameObject card)
//     {
//         CardInteractionHandler interactionHandler = card.GetComponent<CardInteractionHandler>();

//         // Ensure only on-board cards can be sacrificed
//         if (interactionHandler == null || interactionHandler.currentState != CardInteractionHandler.CardState.OnBoard)
//         {
//             Debug.LogWarning("Only cards on the board can be selected for sacrifice.");
//             return;
//         }

//         if (cardsToSacrifice.Contains(card))
//         {
//             cardsToSacrifice.Remove(card);
//             interactionHandler.Deselect(); // Reset visual feedback
//         }
//         else if (cardsToSacrifice.Count < requiredSacrifices)
//         {
//             cardsToSacrifice.Add(card);
//             interactionHandler.Highlight(); // Add visual feedback
//         }

//         // Update progress text
//         if (sacrificeProgressText != null)
//         {
//             sacrificeProgressText.text = $"{cardsToSacrifice.Count}/{requiredSacrifices} sacrifices selected";
//         }

//         // Proceed to placement if sacrifices are complete
//         if (cardsToSacrifice.Count == requiredSacrifices)
//         {
//             ProceedToPlacement();
//         }
//     }


// public bool AreSacrificesComplete()
// {
//     return cardsToSacrifice.Count >= requiredSacrifices;
// }

// }
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

    // Begin the sacrifice phase for a card with a specified cost
    public void EnableSacrificePhase(int cost, GameObject card)
    {
        requiredSacrifices = cost;
        cardToPlace = card;
        HighlightAvailableCards();
        UpdateSacrificeProgressText();
    }

    // Highlight cards available for sacrifice
    private void HighlightAvailableCards()
    {
        foreach (var cardHandler in FindObjectsOfType<CardInteractionHandler>())
        {
            if (cardHandler.currentState == CardInteractionHandler.CardState.OnBoard)
            {
                cardHandler.Highlight();
            }
        }
    }

    // Attempt to proceed to card placement
    public void ProceedToPlacement()
    {
        if (cardsToSacrifice.Count < requiredSacrifices)
        {
            Debug.LogWarning("Not enough sacrifices made.");
            return;
        }

        foreach (var card in cardsToSacrifice)
        {
            Destroy(card);
        }

        cardsToSacrifice.Clear();
        HighlightEmptySlots();
        ClearSacrificeProgressText();
    }

    // Highlight all empty slots for card placement
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

    // Select a card for sacrifice
    public void SelectCardForSacrifice(GameObject card)
    {
        var interactionHandler = card.GetComponent<CardInteractionHandler>();
        if (interactionHandler == null || interactionHandler.currentState != CardInteractionHandler.CardState.OnBoard)
        {
            Debug.LogWarning("Only on-board cards can be sacrificed.");
            return;
        }

        if (cardsToSacrifice.Contains(card))
        {
            cardsToSacrifice.Remove(card);
            interactionHandler.Deselect();
        }
        else if (cardsToSacrifice.Count < requiredSacrifices)
        {
            cardsToSacrifice.Add(card);
            interactionHandler.Highlight();
        }

        UpdateSacrificeProgressText();

        if (cardsToSacrifice.Count == requiredSacrifices)
        {
            ProceedToPlacement();
        }
    }

    // Check if the required sacrifices are complete
    public bool AreSacrificesComplete() => cardsToSacrifice.Count >= requiredSacrifices;

    // Update the UI text for sacrifice progress
    private void UpdateSacrificeProgressText()
    {
        if (sacrificeProgressText != null)
        {
            sacrificeProgressText.text = $"{cardsToSacrifice.Count}/{requiredSacrifices} sacrifices selected";
        }
    }

    // Clear the sacrifice progress text
    private void ClearSacrificeProgressText()
    {
        if (sacrificeProgressText != null)
        {
            sacrificeProgressText.text = string.Empty;
        }
    }
}

