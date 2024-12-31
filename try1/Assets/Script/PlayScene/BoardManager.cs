// using UnityEngine;
// using System.Collections.Generic;

// public class BoardManager : MonoBehaviour
// {
//     public List<GameObject> playerSlots = new List<GameObject>();
//     private List<GameObject> cardsToSacrifice = new List<GameObject>();
//     private int requiredSacrifices;

//     private void Start()
//     {
//         InitializePlayerSlots();
//     }

//     private void InitializePlayerSlots()
//     {
//         // Automatically populate playerSlots from child GameObjects
//         Transform playerArea = GameObject.Find("PlayerArea").transform; // Ensure PlayerArea is named correctly
//         foreach (Transform slot in playerArea)
//         {
//             CardSlot cardSlot = slot.GetComponent<CardSlot>();
//             if (cardSlot != null)
//             {
//                 playerSlots.Add(slot.gameObject);
//             }
//         }
//         Debug.Log($"Player slots initialized: {playerSlots.Count} slots found.");
//     }

//     public void HighlightEmptySlots()
//     {
//         foreach (var slot in playerSlots)
//         {
//             if (!slot.GetComponent<CardSlot>().IsOccupied())
//             {
//                 slot.GetComponent<CardSlot>().HighlightSlot(); // Add visual feedback
//             }
//         }
//     }

//     public void EnableSacrificePhase(int cost)
//     {
//         requiredSacrifices = cost;
//         Debug.Log($"Sacrifice phase started. Need {cost} cards.");
//         HighlightAvailableCards();
//     }

//     private void HighlightAvailableCards()
//     {
//         foreach (var card in FindObjectsOfType<CardInteractionHandler>())
//         {
//             if (card.currentState == CardInteractionHandler.CardState.OnBoard)
//             {
//                 card.Highlight(); // Highlight available cards for sacrifice
//             }
//         }
//     }

//     public void SelectCardForSacrifice(GameObject card)
//     {
//         if (cardsToSacrifice.Contains(card))
//         {
//             cardsToSacrifice.Remove(card);
//             card.GetComponent<CardInteractionHandler>().Deselect();
//         }
//         else if (cardsToSacrifice.Count < requiredSacrifices)
//         {
//             cardsToSacrifice.Add(card);
//             card.GetComponent<CardInteractionHandler>().Highlight();
//         }

//         if (cardsToSacrifice.Count == requiredSacrifices)
//         {
//             ProceedToPlacement();
//         }
//     }

//     public void ProceedToPlacement()
//     {
//         foreach (var card in cardsToSacrifice)
//         {
//             Destroy(card);
//         }
//         cardsToSacrifice.Clear();
//         HighlightEmptySlots();
//     }
// }
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BoardManager : MonoBehaviour
{
    public List<GameObject> playerSlots = new List<GameObject>(); // Player slots
    private List<GameObject> cardsToSacrifice = new List<GameObject>(); // Cards selected for sacrifice
    private int requiredSacrifices;
    private GameObject cardToPlace; // Card being placed after sacrifices
    public TextMeshProUGUI sacrificeProgressText;

    public void EnableSacrificePhase(int cost, GameObject card)
    {
        requiredSacrifices = cost;
        cardToPlace = card;
        Debug.Log($"Sacrifice phase started. Need {cost} sacrifices for card: {card.name}");
        HighlightAvailableCards(); // Highlight cards on the board that can be sacrificed
    }

    private void HighlightAvailableCards()
    {
        foreach (var card in FindObjectsOfType<CardInteractionHandler>())
        {
            if (card.currentState == CardInteractionHandler.CardState.OnBoard)
            {
                card.Highlight(); // Visual feedback for sacrifice availability
            }
        }
    }


    public void ProceedToPlacement()
    {
        Debug.Log($"Proceeding to placement. Sacrificing {cardsToSacrifice.Count} cards.");
        
        foreach (var card in cardsToSacrifice)
        {
            Destroy(card); // Remove sacrificed cards
        }

        cardsToSacrifice.Clear();

        if (cardToPlace != null)
        {
            HighlightEmptySlots(); // Allow placement of the card
        }
    }

    public void HighlightEmptySlots()
    {
        foreach (var slot in playerSlots)
        {
            if (!slot.GetComponent<CardSlot>().IsOccupied())
            {
                slot.GetComponent<CardSlot>().HighlightSlot(); // Visual feedback for empty slots
            }
        }
    }

    public void SelectCardForSacrifice(GameObject card)
{
    if (cardsToSacrifice.Contains(card))
    {
        cardsToSacrifice.Remove(card);
        card.GetComponent<CardInteractionHandler>().Deselect();
    }
    else if (cardsToSacrifice.Count < requiredSacrifices)
    {
        cardsToSacrifice.Add(card);
        card.GetComponent<CardInteractionHandler>().Highlight();
    }

    if (sacrificeProgressText != null)
    {
        sacrificeProgressText.text = $"{cardsToSacrifice.Count}/{requiredSacrifices} sacrifices selected";
    }

    if (cardsToSacrifice.Count == requiredSacrifices)
    {
        ProceedToPlacement();
    }
}
}
