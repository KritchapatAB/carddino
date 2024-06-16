using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    public CardDatabase cardDatabase;
    public RandomCard randomCard;
    public int handSize = 5;
    public GameObject draggableCardPrefab;
    public Transform handPanel;
    private List<GameObject> instantiatedCards = new List<GameObject>();

    void Start()
    {
        if (randomCard == null)
        {
            Debug.LogError("RandomCard component not found! Please assign it in the inspector.");
            return;
        }

        if (cardDatabase == null)
        {
            Debug.LogError("CardDatabase component not found! Please assign it in the inspector.");
            return;
        }

        cardDatabase.OnDatabaseReady += DrawInitialHand;

        DraggableCard.OnDragStart += HideHand;
        DraggableCard.OnDragEnd += ShowHand;
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
            DrawSpecificCard(cardDatabase.cards, "Normal");
        }

        DrawSpecificCard(cardDatabase.cards, "Attacker", 4);
        DrawSpecificCard(cardDatabase.cards, "Defender", 4);

        Debug.Log("Initial hand drawn. Card count in hand: " + playerHand.Count);
    }

    public void DrawRandomCard()
    {
        if (cardDatabase != null && randomCard != null)
        {
            Card drawnCard = randomCard.DrawAndRemoveRandomCard(cardDatabase.cards);
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
            Debug.LogError("CardDatabase or RandomCard component is missing.");
        }
    }

    void DrawSpecificCard(List<Card> cards, string dinoType, int maxCost = int.MaxValue)
    {
        List<Card> filteredCards = cards.FindAll(card => card.dinoType == dinoType && card.cost < maxCost);
        if (filteredCards.Count > 0)
        {
            Card drawnCard = randomCard.DrawAndRemoveRandomCard(filteredCards);
            if (drawnCard != null)
            {
                playerHand.Add(drawnCard);
                cards.Remove(drawnCard);
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
