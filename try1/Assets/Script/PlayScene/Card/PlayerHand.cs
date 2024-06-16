using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public PlayerDeckManager playerDeckManager;
    public int handSize = 5;
    public GameObject draggableCardPrefab;
    public Transform handPanel;
    private List<GameObject> instantiatedCards = new List<GameObject>();

    void Start()
    {
        if (playerDeckManager == null)
        {
            Debug.LogError("PlayerDeckManager component not found! Please assign it in the inspector.");
            return;
        }

        playerDeckManager.InitializePlayerDeck();

        DraggableCard.OnDragStart += HideHand;
        DraggableCard.OnDragEnd += ShowHand;

        DrawInitialHand();
    }

    void OnDestroy()
    {
        DraggableCard.OnDragStart -= HideHand;
        DraggableCard.OnDragEnd -= ShowHand;
    }

    void DrawInitialHand()
    {
        for (int i = 0; i < 3; i++)
        {
            DrawSpecificCard("Normal");
        }

        DrawSpecificCard("Attacker", 4);
        DrawSpecificCard("Defender", 4);

        Debug.Log("Initial hand drawn. Card count in hand: " + playerHand.Count);
    }

    public void DrawRandomCard()
    {
        if (playerDeckManager != null)
        {
            Card drawnCard = playerDeckManager.DrawRandomCard();
            if (drawnCard != null)
            {
                playerHand.Add(drawnCard);
                Debug.Log("Card added to hand: " + drawnCard.cardName);
                DisplayCard(drawnCard);
            }
            else
            {
                Debug.LogWarning("No card was drawn from the deck.");
            }
        }
        else
        {
            Debug.LogError("PlayerDeckManager component is missing.");
        }
    }

    void DrawSpecificCard(string dinoType, int maxCost = int.MaxValue)
    {
        List<Card> filteredCards = playerDeckManager.playerDeck.FindAll(card => card.dinoType == dinoType && card.cost < maxCost);
        if (filteredCards.Count > 0)
        {
            Card drawnCard = DrawAndRemoveRandomCard(filteredCards);
            if (drawnCard != null)
            {
                playerHand.Add(drawnCard);
                playerDeckManager.RemoveCardFromDeck(drawnCard);
                Debug.Log("Specific card added to hand: " + drawnCard.cardName);
                DisplayCard(drawnCard);
            }
            else
            {
                Debug.LogWarning("No specific card was drawn from the deck.");
            }
        }
        else
        {
            Debug.LogWarning($"No cards found with dinoType {dinoType} and cost less than {maxCost}.");
        }
    }

    void DisplayCard(Card card)
    {
        GameObject newCard = Instantiate(draggableCardPrefab, handPanel);
        if (newCard == null)
        {
            Debug.LogError("Failed to instantiate card prefab.");
            return;
        }

        // Set unique identifier for each card instance in the UI
        newCard.name = $"Card_{card.id}_{Random.Range(0, 10000)}"; // Using a random number to ensure unique names

        CardViz cardViz = newCard.GetComponent<CardViz>();
        if (cardViz != null)
        {
            cardViz.LoadCard(card);
            Debug.Log("Card visualization updated for: " + card.cardName);
        }
        else
        {
            Debug.LogError("CardViz component not found on card prefab.");
        }

        instantiatedCards.Add(newCard);
    }

    Card DrawAndRemoveRandomCard(List<Card> cards)
    {
        if (cards.Count > 0)
        {
            int randomIndex = Random.Range(0, cards.Count);
            Card randomCard = cards[randomIndex];
            cards.RemoveAt(randomIndex);
            return randomCard;
        }
        else
        {
            Debug.LogWarning("No more cards left to draw!");
            return null;
        }
    }

    public void OnCardHover(GameObject hoveredCard)
    {
        foreach (GameObject cardObject in instantiatedCards)
        {
            if (cardObject == hoveredCard)
            {
                cardObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
            else
            {
                cardObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
        }
    }

    public void OnCardHoverExit(GameObject hoveredCard)
    {
        foreach (GameObject cardObject in instantiatedCards)
        {
            cardObject.transform.localScale = Vector3.one;
        }
    }

    public void HideHand()
    {
        handPanel.gameObject.SetActive(false);
    }

    public void ShowHand()
    {
        handPanel.gameObject.SetActive(true);
    }
}
