using System.Collections.Generic;
using UnityEngine;

public enum StageType
{
    Normal,
    Boss
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

[CreateAssetMenu(fileName = "StageConfig", menuName = "Stage System/Stage Configuration")]
public class StageConfiguration : ScriptableObject
{
    [Header("Stage Settings")]
    public string stageName;
    public StageType stageType;
    public Difficulty difficulty;

    [Header("Deck Settings")]
    public int maxDeckSize = 10;
    public int maxAttackers = 2;
    public int maxDefenders = 2;
    public int maxNormals = 3;
    public int minAttackerCost = 1;
    public int maxAttackerCost = 3;
    public int minDefenderCost = 1;
    public int maxDefenderCost = 2;
    public int minNormalCost = 1;
    public int maxNormalCost = 3;

    [Header("Specific Cards")]
    public List<SpecificCardEntry> specificCards = new List<SpecificCardEntry>();
}
