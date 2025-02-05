using System.Collections.Generic;
using UnityEngine;

public class StageSelectionPopup : MonoBehaviour
{
    public GameObject stageCardPrefab;
    public Transform stageCardContainer;

    public void GenerateStageChoices(List<StageConfiguration> stageOptions)
    {
        foreach (Transform child in stageCardContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (StageConfiguration stage in stageOptions)
        {
            GameObject stageCard = Instantiate(stageCardPrefab, stageCardContainer);
            StageCardUI cardUI = stageCard.GetComponent<StageCardUI>();
            cardUI.Setup(stage, this);
        }

        gameObject.SetActive(true); // Show popup
    }

    public void DeselectAllCards()
    {
        foreach (Transform child in stageCardContainer)
        {
            StageCardUI card = child.GetComponent<StageCardUI>();
            if (card != null)
            {
                card.Deselect();
            }
        }
    }
}


