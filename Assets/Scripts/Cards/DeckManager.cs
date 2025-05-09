using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class DeckManager : MonoBehaviour
{
    public static Action<CardData> OnCardUsed;
    public static Action<Enemy_Stats,CardData> OnCardAttackUsed;

    public static DeckManager instance;

    public List<CardData> mainDeck;
   [SerializeField] private List<CardData> discardPile = new List<CardData>();
    public List<CardData> hand = new List<CardData>();
    public int handSize = 3;

    private Color textColor;

    //REFERENCIAS UI
    public Image mainDeckSlot;
    public Image discardPileSlot;
    public Image[] handSlots;
    public RectTransform handContainer;

    [SerializeField] private TextMeshProUGUI mainDeckCountText;
    [SerializeField] private TextMeshProUGUI discardPileCountText;

    private Vector2 originalHandContainerPosition;
    private Vector2 hiddenPosition;

    public TextMeshProUGUI[] attackValues; // Usando TextMeshProUGUI para valores de ataque
    public TextMeshProUGUI[] defenseValues; // Usando TextMeshProUGUI para valores de defesa

    // Sprite padrão de carta para quando o slot estiver vazio
    public Sprite cardBackSprite;
    public Sprite emptySlotSprite;

    private bool drawedInitialHand = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        originalHandContainerPosition = handContainer.anchoredPosition;

        hiddenPosition = originalHandContainerPosition + new Vector2(0, -400);

        mainDeck = new List<CardData>(LevelManager.Instance.mainDeck);
        ShuffleDeck();
        DrawInitialHand();
        UpdateUI();

    }

    public void HideHand()
    {
        StartCoroutine(MoveHand(hiddenPosition));
    }

    public void ShowHand()
    {
        StartCoroutine(MoveHand(originalHandContainerPosition));
    }

    private IEnumerator MoveHand(Vector2 targetPosition)
    {
        Vector2 startPosition = handContainer.anchoredPosition;
        float elapsedTime = 0;
        float transitionDuration = 0.5f;

        while (elapsedTime < transitionDuration)
        {
            // Interpola entre a posição inicial e o alvo
            handContainer.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que a posição final seja exata
        handContainer.anchoredPosition = targetPosition;
    }

    // Embaralhar o baralho principal
    private void ShuffleDeck()
    {
        for (int i = 0; i < mainDeck.Count; i++)
        {
            CardData temp = mainDeck[i];
            int randomIndex = UnityEngine.Random.Range(i, mainDeck.Count);
            mainDeck[i] = mainDeck[randomIndex];
            mainDeck[randomIndex] = temp;
        }
    }

    // Desenhar as cartas iniciais
    private void DrawInitialHand()
    {
        for (int i = 0; i < handSize; i++)
        {
            DrawCard();
        }
        drawedInitialHand = true;
    }

    // Desenhar uma carta do baralho principal para a mão
    public void DrawCard()
    {
        if (hand.Count >= handSize)
        {
            Debug.Log("Mão cheia!");
            return;
        }

        if (mainDeck.Count == 0)
        {
            RefillDeckFromDiscardPile();
        }

        if (mainDeck.Count > 0 && !drawedInitialHand)
        {
            CardData drawnCard = mainDeck[0];
            hand.Add(drawnCard);
            mainDeck.RemoveAt(0);
        }

        UpdateUI();

    }

    // Reabastecer o baralho principal com o baralho de descarte e embaralhar
    private void RefillDeckFromDiscardPile()
    {
        mainDeck.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
    }

    public void UseCard(CardData usedCard)
    {
        if (hand.Contains(usedCard)/* && usedCard.cardType != CardType.Attack*/)
        {
            OnCardUsed?.Invoke(usedCard);

            int index = hand.IndexOf(usedCard); // Armazena o índice da carta usada
            hand.RemoveAt(index); // Remove a carta da mão
            discardPile.Add(usedCard); // Adiciona a carta ao descarte

            DrawCard();

            // Move a nova carta para o lugar da carta usada
            if (hand.Count < handSize && mainDeck.Count > 0 ) // Verifica se há cartas no baralho e se o índice é válido
            {
                CardData drawnCard = mainDeck[0];
                hand.Insert(index, drawnCard); // Insere a nova carta no lugar da usada
                mainDeck.RemoveAt(0); // Remove a nova carta do baralho
            }

            UpdateUI();
        }
    }
    public void UseCardAttack(Enemy_Stats enemyTarget,CardData usedCard)
    {
        if (hand.Contains(usedCard) && usedCard.cardType == CardType.Attack)
        {
            OnCardAttackUsed?.Invoke(enemyTarget, usedCard);

            int index = hand.IndexOf(usedCard); // Armazena o índice da carta usada
            hand.RemoveAt(index); // Remove a carta da mão
            discardPile.Add(usedCard); // Adiciona a carta ao descarte

            DrawCard();

            // Move a nova carta para o lugar da carta usada
            if (hand.Count < handSize && mainDeck.Count > 0) // Verifica se há cartas no baralho e se o índice é válido
            {
                CardData drawnCard = mainDeck[0];
                hand.Insert(index, drawnCard); // Insere a nova carta no lugar da usada
                mainDeck.RemoveAt(0); // Remove a nova carta do baralho
            }

            UpdateUI();
        }
    }
    public void SetHandVisibility(bool isVisible)
    {
        foreach (var slot in handSlots)
        {
            slot.gameObject.SetActive(isVisible);
        }
    }

    // Move todas as cartas da mão para a pilha de descarte
    public void DiscardHand()
    {
        foreach (var card in hand)
        {
            discardPile.Add(card); // Adiciona a carta à pilha de descarte
        }
        hand.Clear(); // Limpa a mão após mover as cartas
        drawedInitialHand = false;
        DrawInitialHand(); // Desenha novas cartas para a mão
    }

    public void UpdateUI()
    {
        // Verifica e atualiza mainDeckSlot e discardPileSlot com base em cardBack ou emptySlotSprite
        mainDeckSlot.sprite = mainDeck.Count > 0 ? cardBackSprite : emptySlotSprite;
        discardPileSlot.sprite = discardPile.Count > 0 ? cardBackSprite : emptySlotSprite;

        for (int i = 0; i < handSlots.Length; i++)
        {
              var slot = handSlots[i];
              if (slot == null) continue;

              CardHandSlotUI slotUI = slot.GetComponent<CardHandSlotUI>();
              if (slotUI == null)
              {
                  Debug.LogWarning($"Slot {i} não possui CardHandSlotUI.");
                  continue;
              }

              if (i < hand.Count)
                  slotUI.Setup(hand[i]);
              else
                  slotUI.ClearSlot();
        }

        // Atualizar os card count
        if (mainDeckCountText != null)
            mainDeckCountText.text = mainDeck.Count.ToString();

        if (discardPileCountText != null)
            discardPileCountText.text = discardPile.Count.ToString();
    }

    public void RemoveTemporaryCards(List<CardData> temporaryCards)
    {
        mainDeck.RemoveAll(card => temporaryCards.Contains(card));
        Debug.Log("Cartas temporárias removidas do baralho.");
    }
}
