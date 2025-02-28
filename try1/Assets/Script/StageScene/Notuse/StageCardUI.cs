// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using UnityEngine.EventSystems;
// using UnityEngine.SceneManagement;
// using System.Collections.Generic;

// public class StageCardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
// {
//     public Image frontCard;
//     public TextMeshProUGUI stageNameText;

//     private StageConfiguration stageData;
//     private StageSelectionPopup stageSelectionPopup;
//     private bool isSelected = false;

//     private static readonly Color defaultColor = Color.white;
//     private static readonly Color selectedColor = new(0f, 1f, 0.65f, 1f);
//     private static readonly Color hoverColor = new(0.8f, 1f, 0.8f, 1f);

//     private static HashSet<string> displayedStages = new(); // ✅ Tracks displayed stage names

//     public void Setup(StageConfiguration stage, StageSelectionPopup popup)
//     {
//         stageData = stage;
//         stageSelectionPopup = popup;

//         // ✅ Check if the stage has already been displayed
//         if (displayedStages.Contains(stage.stageName))
//         {
//             gameObject.SetActive(false); // Hide duplicate
//             return;
//         }

//         // ✅ Register this stage as displayed
//         displayedStages.Add(stage.stageName);

//         stageNameText.text = stage.stageName;
//         frontCard.color = defaultColor;
//     }

//     public void OnPointerClick(PointerEventData eventData)
//     {
//         if (!isSelected)
//         {
//             stageSelectionPopup.DeselectAllCards();
//             frontCard.color = selectedColor;
//             isSelected = true;
//             GameManager.Instance.SetCurrentStage(stageData);
//             LoadNextScene();
//         }
//     }

//     public void OnPointerEnter(PointerEventData eventData) { if (!isSelected) frontCard.color = hoverColor; }
//     public void OnPointerExit(PointerEventData eventData) { if (!isSelected) frontCard.color = defaultColor; }
//     public void Deselect() { frontCard.color = defaultColor; isSelected = false; }

//     private void LoadNextScene()
//     {
//         SceneManager.LoadScene(stageData.stageType switch
//         {
//             StageType.Normal or StageType.Challenge => "PlayScene",
//             StageType.Boss => "PlayScene", //BossScene
//             StageType.Shop => "ShopStage",
//             StageType.ChooseCard => "ChooseCard",
//             _ => throw new System.Exception("Unknown stage type!")
//         });
//     }

//     private void OnDestroy()
//     {
//         if (stageData != null) displayedStages.Remove(stageData.stageName); // ✅ Cleanup on destroy
//     }
// }
