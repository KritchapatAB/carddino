using UnityEngine;

public class EnemyCardSlot : MonoBehaviour
{
    private bool isOccupied;
    private GameObject placedCard;

    public bool IsOccupied() => isOccupied;

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }

    public void PlaceCard(GameObject card)
    {
        if (isOccupied) return;

        placedCard = card;
        isOccupied = true;  // ✅ This must be set!
        placedCard.transform.SetParent(transform, false);
    }


    // 🔹 Moves the card from Reserve Area to Active Area
    public void MoveToActiveField(Transform activeSlot)
    {
        if (!isOccupied || placedCard == null) return;

        placedCard.transform.SetParent(activeSlot, false); // Move card
        isOccupied = false; // Free up the reserve slot
    }

    public GameObject GetPlacedCard()
    {
        Debug.Log($"[EnemyCardSlot] Returning {placedCard?.name ?? "null"} from GetPlacedCard()");
        return placedCard;
    }


    public void ClearSlot()
    {
        if (placedCard != null)
        {
            Destroy(placedCard); // Remove the card object
            placedCard = null;
        }
        isOccupied = false;
    }

}
