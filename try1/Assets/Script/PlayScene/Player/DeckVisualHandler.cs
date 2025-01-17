using UnityEngine;
using System.Collections;

public class DeckVisualHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] deckLayers; // Assign BackCard objects (e.g., BackCard(4), BackCard(3), etc.)
    [SerializeField] private float blinkDuration = 1.0f;
    [SerializeField] private Color darkGreen = new Color(0.0f, 0.5f, 0.0f);
    [SerializeField] private Color lightGreen = new Color(0.0f, 1.0f, 0.0f);

    private TurnManager turnManager;
    private BoardManager boardManager;
    private bool isBlinking;
    private int initialDeckSize;

    private readonly float[] thresholds = { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f }; // Thresholds for each layer (0% to 80%)

    private void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        boardManager = FindObjectOfType<BoardManager>();

        if (boardManager == null)
        {
            Debug.LogError("BoardManager not found! Deck visualization will not work.");
            return;
        }

        // Subscribe to deck changes
        boardManager.OnDeckChanged += UpdateDeckVisualization;

        // Initialize the engage deck size dynamically
        PlayerHand playerHand = FindObjectOfType<PlayerHand>();
        initialDeckSize = boardManager.engagePlayerDeck.Count + (playerHand?.playerHand.Count ?? 0);

        UpdateDeckVisualization(); // Update visualization at the start
    }

    private void OnDestroy()
    {
        if (boardManager != null)
        {
            boardManager.OnDeckChanged -= UpdateDeckVisualization;
        }
    }

    private void Update()
    {
        if (ShouldBlink() && !isBlinking)
        {
            StartCoroutine(BlinkDeck());
        }
        else if (!ShouldBlink() && isBlinking)
        {
            StopBlinking();
        }
    }

    /// <summary>
    /// Updates the visibility of deck layers based on remaining card percentage.
    /// </summary>
    private void UpdateDeckVisualization()
    {
        if (boardManager == null || boardManager.engagePlayerDeck == null || initialDeckSize == 0) return;

        int totalCards = boardManager.engagePlayerDeck.Count;
        float currentPercentage = (float)totalCards / initialDeckSize;

        Debug.Log($"Updating Deck Visualization: {currentPercentage * 100}% remaining");

        for (int i = 0; i < deckLayers.Length; i++)
        {
            bool shouldShowLayer = currentPercentage > thresholds[i];
            if (deckLayers[i].activeSelf != shouldShowLayer) // Only toggle if there's a state change
            {
                deckLayers[i].SetActive(shouldShowLayer);
                Debug.Log($"Layer {i} {(shouldShowLayer ? "ACTIVE" : "HIDDEN")}. Threshold: {thresholds[i] * 100}%");
            }
        }
    }

    /// <summary>
    /// Checks if the deck should blink.
    /// </summary>
    private bool ShouldBlink()
    {
        return turnManager?.currentTurn == TurnManager.TurnState.PlayerTurn && turnManager.CanPlayerDrawCard();
    }

    /// <summary>
    /// Handles the blinking effect on the topmost visible layer.
    /// </summary>
    private IEnumerator BlinkDeck()
    {
        isBlinking = true;

        GameObject topLayer = GetTopVisibleLayer();
        if (topLayer == null)
        {
            Debug.LogWarning("No visible deck layers to blink!");
            yield break;
        }

        var deckImage = topLayer.GetComponent<UnityEngine.UI.Image>();
        if (deckImage == null)
        {
            Debug.LogWarning("No Image component found on the top visible layer!");
            yield break;
        }

        float timer = 0;
        while (isBlinking)
        {
            timer += Time.deltaTime / blinkDuration;
            deckImage.color = Color.Lerp(darkGreen, lightGreen, Mathf.PingPong(timer, 1));
            yield return null;
        }
    }

    /// <summary>
    /// Stops the blinking effect and resets the color of the top layer.
    /// </summary>
    private void StopBlinking()
    {
        isBlinking = false;

        GameObject topLayer = GetTopVisibleLayer();
        if (topLayer != null)
        {
            var deckImage = topLayer.GetComponent<UnityEngine.UI.Image>();
            if (deckImage != null)
            {
                deckImage.color = Color.white; // Reset to default color
            }
        }
    }

    /// <summary>
    /// Finds the topmost visible deck layer.
    /// </summary>
    private GameObject GetTopVisibleLayer()
    {
        for (int i = deckLayers.Length - 1; i >= 0; i--) // Start from the highest layer
        {
            if (deckLayers[i].activeSelf)
            {
                return deckLayers[i];
            }
        }
        return null;
    }
}