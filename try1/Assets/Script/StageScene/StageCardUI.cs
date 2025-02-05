// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using UnityEngine.EventSystems;

// public class StageCardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
// {
//     public Image frontCard; // Reference to the card background
//     public TextMeshProUGUI stageNameText;

//     private StageConfiguration stageData;
//     private StageSelectionPopup stageSelectionPopup;
//     private bool isSelected = false;

//     private static Color defaultColor = Color.white;
//     private static Color selectedColor = new Color(0f, 1f, 0.65f, 1f); // 00FFA7
//     private static Color hoverColor = new Color(0.8f, 1f, 0.8f, 1f); // Light green effect when hovering

//     public void Setup(StageConfiguration stage, StageSelectionPopup popup)
//     {
//         stageData = stage;
//         stageSelectionPopup = popup;

//         stageNameText.text = stage.stageName;
//         frontCard.color = defaultColor;
//     }

//     public void OnPointerClick(PointerEventData eventData)
//     {
//         if (!isSelected)
//         {
//             stageSelectionPopup.DeselectAllCards(); // Deselect other cards
//             frontCard.color = selectedColor; // Change to highlight color
//             isSelected = true;

//             GameManager.Instance.SetCurrentStage(stageData);
//             LoadNextScene();
//         }
//     }

//     public void OnPointerEnter(PointerEventData eventData)
//     {
//         if (!isSelected) frontCard.color = hoverColor; // Highlight on hover
//     }

//     public void OnPointerExit(PointerEventData eventData)
//     {
//         if (!isSelected) frontCard.color = defaultColor; // Reset when cursor leaves
//     }

//     private void LoadNextScene()
//     {
//         switch (stageData.stageType)
//         {
//             case StageType.Normal:
//             case StageType.Challenge:
//                 UnityEngine.SceneManagement.SceneManager.LoadScene("PlayScene");
//                 break;
//             case StageType.Boss:
//                 UnityEngine.SceneManagement.SceneManager.LoadScene("BossScene");
//                 break;
//             case StageType.Shop:
//                 UnityEngine.SceneManagement.SceneManager.LoadScene("ShopStage");
//                 break;
//             case StageType.ChooseCard:
//                 UnityEngine.SceneManagement.SceneManager.LoadScene("ChooseCard");
//                 break;
//             default:
//                 Debug.LogError("Unknown stage type!");
//                 break;
//         }
//     }

//     public void Deselect()
//     {
//         frontCard.color = defaultColor; // Reset color
//         isSelected = false;
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StageCardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image frontCard;
    public TextMeshProUGUI stageNameText;

    private StageConfiguration stageData;
    private StageSelectionPopup stageSelectionPopup;
    private bool isSelected = false;

    private static readonly Color defaultColor = Color.white;
    private static readonly Color selectedColor = new(0f, 1f, 0.65f, 1f);
    private static readonly Color hoverColor = new(0.8f, 1f, 0.8f, 1f);

    public void Setup(StageConfiguration stage, StageSelectionPopup popup)
    {
        stageData = stage;
        stageSelectionPopup = popup;
        stageNameText.text = stage.stageName;
        frontCard.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSelected)
        {
            stageSelectionPopup.DeselectAllCards();
            frontCard.color = selectedColor;
            isSelected = true;
            GameManager.Instance.SetCurrentStage(stageData);
            LoadNextScene();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) { if (!isSelected) frontCard.color = hoverColor; }
    public void OnPointerExit(PointerEventData eventData) { if (!isSelected) frontCard.color = defaultColor; }
    public void Deselect() { frontCard.color = defaultColor; isSelected = false; }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(stageData.stageType switch
        {
            StageType.Normal or StageType.Challenge => "PlayScene",
            StageType.Boss => "BossScene",
            StageType.Shop => "ShopStage",
            StageType.ChooseCard => "ChooseCard",
            _ => throw new System.Exception("Unknown stage type!")
        });
    }
}
