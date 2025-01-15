using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecificCardEntry
{
    public int cardId;   // ID of the card to add
    public int quantity; // Quantity to add
    public int turn;     // Turn to add these cards (optional)
}

[System.Serializable]
public class StageConfiguration
{
    public string stageName;                     // Name of the stage
    public int maxDeckSize;                      // Maximum cards in the deck
    public int maxAttackers, maxDefenders, maxNormals; // Max cards per type
    public int minAttackerCost, maxAttackerCost; // Min-Max cost for Attackers
    public int minDefenderCost, maxDefenderCost; // Min-Max cost for Defenders
    public int minNormalCost, maxNormalCost;     // Min-Max cost for Normals
    public List<SpecificCardEntry> specificCards; // Specific cards to add by ID
}
