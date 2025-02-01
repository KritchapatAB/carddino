using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeckStage : MonoBehaviour
{
    public CardDatabase cardDatabase; // Reference to the card database
    public GameObject cardTemplate; // Prefab for a single card UI
    public Transform cardDisplayArea; // Parent object to hold card UIs

    private void Start()
    {
        LoadAndDisplayPlayerDeck();
    }

    private void LoadAndDisplayPlayerDeck()
    {
        List<int> playerDeckIds = GameManager.Instance.CurrentSaveData.playerDeckIds;

        if (playerDeckIds == null || playerDeckIds.Count == 0)
        {
            Debug.LogWarning("No cards in the player's deck.");
            return;
        }

        // Clear existing cards
        foreach (Transform child in cardDisplayArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate cards and add to the scrollable content
        foreach (int cardId in playerDeckIds)
        {
            Card cardData = cardDatabase.GetCardById(cardId);

            if (cardData != null)
            {
                GameObject cardObject = Instantiate(cardTemplate, cardDisplayArea);
                CardViz cardViz = cardObject.GetComponent<CardViz>();
                cardViz.LoadCard(cardData);
            }
            else
            {
                Debug.LogWarning($"Card with ID {cardId} not found in database.");
            }
        }
    }


    public void ReturnToChooseStage()
    {
        SceneManager.LoadScene("ChooseStage");
    }
}
