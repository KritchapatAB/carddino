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
    if (isOccupied)
    {
        Debug.LogError($"⚠️ [EnemyCardSlot] Tried to place {card?.name ?? "null"} in {gameObject.name}, but it's already occupied!");
        return;
    }

    placedCard = card;
    isOccupied = true;

    placedCard.transform.SetParent(transform, false);
    placedCard.transform.localPosition = Vector3.zero;

    Debug.Log($"✅ [EnemyCardSlot] Placed {placedCard.name} in {gameObject.name}");
}

    // 🔹 Moves the card from Reserve Area to Active Area
    public void MoveToActiveField(EnemyCardSlot activeSlot)
    {
        if (!isOccupied || placedCard == null) return;

        activeSlot.PlaceCard(placedCard); // ✅ Update target slot’s reference
        placedCard.transform.SetParent(activeSlot.transform, false);
        placedCard.transform.localPosition = Vector3.zero;

        placedCard = null; // ✅ Remove reference from old slot
        isOccupied = false;
    }

    public GameObject GetPlacedCard()
    {
        return placedCard;
    }

    public void ClearSlot()
{
    Debug.Log($"[ClearSlot] Releasing {gameObject.name}, clearing {placedCard?.name ?? "null"}");

    // ✅ Properly clear the slot and remove card reference
    if (placedCard != null)
    {
        // Destroy(placedCard);
        placedCard = null;
    }

    isOccupied = false;
}

}
