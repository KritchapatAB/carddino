using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandToggle : MonoBehaviour
{
    public GameObject playerHand; // Assign your PlayerHand GameObject here in the Inspector

    public void TogglePlayerHand()
    {
        playerHand.SetActive(!playerHand.activeSelf); // Toggle the active state of PlayerHand
    }
}
