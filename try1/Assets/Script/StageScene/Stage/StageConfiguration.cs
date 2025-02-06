// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public enum StageType
// {
//     Normal,
//     Challenge,
//     Boss,
//     Shop,
//     ChooseCard
// }

// [System.Serializable]
// public class SpecificCardEntry
// {
//     public int cardId;   // ID of the card to add
//     public int quantity; // Quantity to add
//     public int turn;     // Turn to add these cards (optional)
// }

// [System.Serializable]
// public class StageConfiguration
// {
//     public string stageName;                     // Name of the stage
//     public StageType stageType;                  // NEW FIELD: What type of stage is this?
//     public int maxDeckSize;                      // Maximum cards in the deck
//     public int maxAttackers, maxDefenders, maxNormals; // Max cards per type
//     public int minAttackerCost, maxAttackerCost; // Min-Max cost for Attackers
//     public int minDefenderCost, maxDefenderCost; // Min-Max cost for Defenders
//     public int minNormalCost, maxNormalCost;     // Min-Max cost for Normals
//     public List<SpecificCardEntry> specificCards; // Specific cards to add by ID


//     public int bonusHealth;
//     public int bonusDamage; 

//     public StageConfiguration()
//     {
//         bonusHealth = 2;
//         bonusDamage = 1;
//     }
// }

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

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

[System.Serializable]
public class SpecificCardEntry
{
    public int cardId;  
    public int quantity; 
    public int turn;     
}

[System.Serializable]
public class StageConfiguration
{
    public string stageName;                 
    public StageType stageType;              
    public Difficulty difficulty; // ✅ Only used for Combat Stages

    public int maxDeckSize;                      
    public int maxAttackers, maxDefenders, maxNormals; 
    public int minAttackerCost, maxAttackerCost; 
    public int minDefenderCost, maxDefenderCost; 
    public int minNormalCost, maxNormalCost;     

    public List<SpecificCardEntry> specificCards; 

    public int bonusHealth;  
    public int bonusDamage;  

    public StageConfiguration()
    {
        // ✅ Default bonuses based on stage type
        if (stageType == StageType.Challenge)
        {
            bonusHealth = 2;
            bonusDamage = 1;
        }
        else if (stageType == StageType.Boss)
        {
            bonusHealth = 0; // No extra health for bosses
            bonusDamage = 0;
        }
        else
        {
            bonusHealth = 0;
            bonusDamage = 0;
        }

        // ✅ Only set difficulty for Combat stages
        if (stageType == StageType.Normal || stageType == StageType.Challenge || stageType == StageType.Boss)
        {
            difficulty = Difficulty.Easy; // Default to Easy, will be modified in filtering logic
        }
    }
}

