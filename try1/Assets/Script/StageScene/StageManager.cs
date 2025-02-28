using UnityEngine;

public class StageManager : MonoBehaviour
{
    private EnemyDeckManager enemyDeckManager;

    void Start()
    {
        enemyDeckManager = FindObjectOfType<EnemyDeckManager>();

        if (enemyDeckManager == null)
        {
            Debug.LogError("EnemyDeckManager is not assigned!");
            return;
        }

        if (GameManager.Instance.CurrentStage == null)
        {
            Debug.LogError("No stage selected in GameManager!");
            return;
        }

        Debug.Log($"Starting Stage: {GameManager.Instance.CurrentStage.stageName}");
        enemyDeckManager.InitializeEnemyDeck(GameManager.Instance.CurrentStage);
    }
}