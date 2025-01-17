using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for Button reference

public class MainMenu : MonoBehaviour
{
  
    public Button continueButton; 

    private void Start()
    {
        UpdateContinueButtonState();
    }

    public void PlayGame()
    {
        Debug.Log("Play button clicked.");
        SceneManager.LoadScene("StartGame"); // Load the StartGame scene
    }

    public void ContinueGame()
    {
        Debug.Log("Continue button clicked.");
        SceneManager.LoadScene("ChooseStage"); // Load the ChooseStage scene
    }

    private void UpdateContinueButtonState()
    {
        // Hide the Continue button if no save file or invalid save data
        if (!File.Exists(SaveManager.GetSaveFilePath()))
        {
            continueButton.interactable = false; // Disable the button
            continueButton.gameObject.SetActive(false); // Optionally hide it
            Debug.Log("No save file found. Disabling Continue button.");
            return;
        }

        PlayerSaveData saveData = SaveManager.LoadGame(null);
        if (saveData == null || !saveData.isSaveValid)
        {
            continueButton.interactable = false; // Disable the button
            continueButton.gameObject.SetActive(false); // Optionally hide it
            Debug.Log("Invalid save file found. Disabling Continue button.");
        }
        else
        {
            continueButton.interactable = true; // Enable the button
            continueButton.gameObject.SetActive(true); // Optionally show it
            Debug.Log("Valid save file found. Continue button enabled.");
        }
    }
}

