using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageType
{
    Normal,
    Challenge,
    Boss,
    Shop,
    ChooseCard
}

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
    public StageType stageType;                  // NEW FIELD: What type of stage is this?
    public int maxDeckSize;                      // Maximum cards in the deck
    public int maxAttackers, maxDefenders, maxNormals; // Max cards per type
    public int minAttackerCost, maxAttackerCost; // Min-Max cost for Attackers
    public int minDefenderCost, maxDefenderCost; // Min-Max cost for Defenders
    public int minNormalCost, maxNormalCost;     // Min-Max cost for Normals
    public List<SpecificCardEntry> specificCards; // Specific cards to add by ID


    public int bonusHealth;
    public int bonusDamage; 

    public StageConfiguration()
    {
        bonusHealth = 2;
        bonusDamage = 1;
    }
}
