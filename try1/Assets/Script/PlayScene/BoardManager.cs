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

    public void ShiftEnemyCardToActiveArea()
    {
        foreach (GameObject reserveSlot in enemyReserveSlots)
        {
            if (reserveSlot.transform.childCount > 0) // Check if there's a card
            {
                GameObject card = reserveSlot.transform.GetChild(0).gameObject;
                PlaceCardInSlot(card, enemyActiveSlots); // Move it to active area
                return;
            }
        }
        Debug.Log("No cards in reserve to move.");
    }
}
