using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public Transform playerArea;       // Player slots container
    public Transform enemyActiveArea; // Enemy active slots container
    public Transform enemyReserveArea; // Enemy reserve slots container

    public GameObject cardPrefab;     // Prefab for cards
    public List<GameObject> playerSlots = new List<GameObject>();
    public List<GameObject> enemyActiveSlots = new List<GameObject>();
    public List<GameObject> enemyReserveSlots = new List<GameObject>();

    void Start()
    {
        InitializeSlots(playerArea, playerSlots);
        InitializeSlots(enemyActiveArea, enemyActiveSlots);
        InitializeSlots(enemyReserveArea, enemyReserveSlots);
    }

    void InitializeSlots(Transform area, List<GameObject> slotList)
    {
        foreach (Transform slot in area)
        {
            slotList.Add(slot.gameObject);
        }
    }

    public void PlaceCardInSlot(GameObject card, List<GameObject> slots)
    {
        foreach (GameObject slot in slots)
        {
            if (slot.transform.childCount == 0) // Check if the slot is empty
            {
                card.transform.SetParent(slot.transform);
                card.transform.localPosition = Vector3.zero; // Center card in the slot
                card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;
            }
        }
        Debug.LogWarning("No empty slots available.");
    }

    public void EnableBoardForPlacement(GameObject cardToPlace)
    {
        foreach (GameObject slot in playerSlots)
        {
            if (slot.transform.childCount == 0) // Only allow empty slots
            {
                var slotComponent = slot.GetComponent<CardSlot>();
                if (slotComponent != null)
                {
                    slotComponent.EnablePlacementMode();
                }
            }
        }
    }

    public void DisableAllSlots()
    {
        foreach (GameObject slot in playerSlots)
        {
            var slotComponent = slot.GetComponent<CardSlot>();
            if (slotComponent != null)
            {
                slotComponent.DisablePlacementMode();
            }
        }
    }

    public void ConfirmCardPlacement(GameObject card)
    {
        PlaceCardInSlot(card, playerSlots); // Use the existing method to handle placement
        DisableAllSlots();                 // Disable placement mode after card is placed
    }
}
