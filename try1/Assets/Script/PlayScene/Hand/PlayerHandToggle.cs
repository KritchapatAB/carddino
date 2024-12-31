using UnityEngine;

public class PlayerHandToggle : MonoBehaviour
{
    public GameObject playerHand;

    // Toggle the visibility of the player's hand
    public void TogglePlayerHand()
    {
        if (playerHand != null)
        {
            playerHand.SetActive(!playerHand.activeSelf);
        }
    }
}
