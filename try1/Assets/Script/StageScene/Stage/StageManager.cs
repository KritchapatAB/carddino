using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<StageConfigSO> Stages = new List<StageConfigSO>(); // List of Stage Configurations
    public int CurrentStageIndex = 0;                              // Current stage index

    private EnemyDeckManager enemyDeckManager;

    void Start()
    {
        enemyDeckManager = FindObjectOfType<EnemyDeckManager>();

        if (enemyDeckManager == null)
        {
            Debug.LogError("EnemyDeckManager is not assigned or not found in the scene!");
            return;
        }
        AssignStageToEnemyDeck();
    }

    public void AssignStageToEnemyDeck()
    {
        if (CurrentStageIndex < 0 || CurrentStageIndex >= Stages.Count)
        {
            Debug.LogError($"Invalid stage index {CurrentStageIndex}. Please set a valid stage index.");
            return;
        }
     
        StageConfigSO currentStageConfigSO = Stages[CurrentStageIndex];
        Debug.Log($"Assigned stage '{currentStageConfigSO.name}' to EnemyDeckManager.");
        if (currentStageConfigSO == null)
        {
            Debug.LogError($"StageConfigSO is null for index {CurrentStageIndex}. Check your setup.");
            return;
        }

        if (currentStageConfigSO.stageConfigurations.Count == 0)
        {
            Debug.LogError($"StageConfigSO '{currentStageConfigSO.name}' has no stage configurations.");
            return;
        }
        var stageConfig = currentStageConfigSO.stageConfigurations[0]; // Assuming single-stage configuration per SO
        enemyDeckManager.InitializeEnemyDeck(stageConfig);
    }
}