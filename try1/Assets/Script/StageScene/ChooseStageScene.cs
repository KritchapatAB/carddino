using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseStageScene : MonoBehaviour
{
    public TextMeshProUGUI coinUIText; // Assign CoinUI in the inspector
    public TextMeshProUGUI currentStageText; // Assign CurrentStageText in the inspector

    private void Start()
    {
        GameManager.Instance.LoadSaveData();

        if (!GameManager.Instance.CurrentSaveData.isSaveValid)
        {
            Debug.LogWarning("Invalid save data. Returning to Main Menu.");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        var saveData = GameManager.Instance.CurrentSaveData;

        // Update the money and current stage display
        if (coinUIText != null)
        {
            coinUIText.text = $"{saveData.money}";
        }

        if (currentStageText != null)
        {
            currentStageText.text = $"{saveData.currentStage}";
        }
    }

    public void StartStage()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void OpenShop()
    {
        SceneManager.LoadScene("ShopStage");
    }

    public void ManageDeck()
    {
        SceneManager.LoadScene("PlayerDeckStage");
    }

    public void ReturnMainMenu()
    {
        GameManager.Instance.SaveData();
        SceneManager.LoadScene("MainMenu");
    }

}