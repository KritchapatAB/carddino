// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class PlayScene : MonoBehaviour
// {
//     public void WinStage()
//     {
//         GameManager.Instance.CurrentSaveData.isSaveValid = true;
//         GameManager.Instance.AddMoney(10);
//         GameManager.Instance.AdvanceStage();
//         GameManager.Instance.SaveData();

//         Debug.Log("Stage completed. Returning to ChooseStage.");
//         SceneManager.LoadScene("ChooseStage");
//     }

//     public void LoseStage()
//     {
//         GameManager.Instance.CurrentSaveData.isSaveValid = false;
//         GameManager.Instance.SaveData();

//         Debug.Log("Stage failed. Save invalidated.");
//         GameManager.Instance.ResetSaveData(); // Optional: delete save file
//         SceneManager.LoadScene("MainMenu");
//     }
// }

