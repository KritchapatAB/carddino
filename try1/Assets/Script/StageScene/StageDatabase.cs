using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDatabase", menuName = "Stage System/Stage Database")]
public class StageDatabase : ScriptableObject
{
    [Header("Stage Configurations")]
    public List<StageConfiguration> stageConfigs;
}
