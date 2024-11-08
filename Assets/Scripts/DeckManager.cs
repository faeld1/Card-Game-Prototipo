using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Collections;

public class DeckManager : MonoBehaviour
{
    public static Action<CardData> OnCardUsed;

    public static DeckManager instance;

    public List<CardData> mainDeck;
    private List<CardData> discardPile = new List<CardData>();
    public List<CardData> hand = new List<CardData>();
    public int handSize = 3;

    private Color textColor;

    //REFERENCIAS UI
    public Image mainDeckSlot;
    public Image discardPileSlot;
    public Image[] handSlots;
    public RectTransform handContainer;

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
        if (hand.Contains(usedCard))
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

    public void SetHandVisibility(bool isVisible)
    {
        foreach (var slot in handSlots)
        {
            slot.gameObject.SetActive(isVisible);
        }
    }

    public void UpdateUI()
    {
        // Verifica e atualiza mainDeckSlot e discardPileSlot com base em cardBack ou emptySlotSprite
        mainDeckSlot.sprite = mainDeck.Count > 0 ? cardBackSprite : emptySlotSprite;
        discardPileSlot.sprite = discardPile.Count > 0 ? cardBackSprite : emptySlotSprite;

        for (int i = 0; i < handSlots.Length; i++)
        {
            if (i < hand.Count)
            {
                CardData card = hand[i];
                handSlots[i].sprite = card.cardSprite;

                // Verificação do tipo da carta para exibir o valor correspondente
                string cardValueText = "";
                
                switch (card.cardType)
                {
                    case CardType.Attack:
                        cardValueText = card.attackValue.ToString();
                        textColor = Color.red;
                        break;
                    case CardType.Defense:
                        cardValueText = card.defenseValue.ToString();
                        textColor = Color.blue;
                        break;
                    case CardType.Support:
                        cardValueText = "Suporte"; // Ou outro valor ou símbolo específico para suporte
                        break;
                }


                handSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = cardValueText;
                handSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = textColor;
                

                // Adiciona o CardDragHandler e atribui o CardData automaticamente
                CardDragHandler dragHandler = handSlots[i].GetComponent<CardDragHandler>();
                if (dragHandler == null)
                    dragHandler = handSlots[i].gameObject.AddComponent<CardDragHandler>();

                dragHandler.cardData = card; // Atribui o CardData automaticamente
            }
            else
            {
                handSlots[i].sprite = emptySlotSprite;
                handSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";

                // Remove o CardDragHandler se o slot está vazio
                CardDragHandler dragHandler = handSlots[i].GetComponent<CardDragHandler>();
                if (dragHandler != null)
                    dragHandler.cardData = null;
            }
        }
    }
}
