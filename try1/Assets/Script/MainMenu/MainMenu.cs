using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        
        Debug.Log("Play button clicked"); 
        SceneManager.LoadScene("PlayScene", LoadSceneMode.Single);
        SceneManager.UnloadSceneAsync("MainMenu"); // Force unload the MainMenu

    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}
