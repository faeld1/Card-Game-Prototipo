using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public GameObject newCardContainer; // Container com os 3 botões para novas cartas
    public GameObject upgradeCardContainer; // Container para instanciar os botões de upgrade
    public GameObject upgradeButtonPrefab; // Prefab do botão de upgrade
    public Button skipButton; // Botão de pular

    public List<CardData> availableCards; // Cartas que podem ser oferecidas como novas

    private bool offerUpgrade; // Alterna entre upgrade e nova carta

    private void Start()
    {
        offerUpgrade = LevelManager.Instance.currentSubLevelIndex % 2 == 0;

        if (offerUpgrade)
        {
            ShowUpgradeOptions();
        }
        else
        {
            ShowNewCardOptions();
        }

        skipButton.onClick.AddListener(Skip);
    }

    private void ShowNewCardOptions()
    {
        newCardContainer.SetActive(true);
        upgradeCardContainer.SetActive(false);

        // Seleciona 3 cartas aleatórias
        List<CardData> randomCards = new List<CardData>();
        List<CardData> availableCopy = new List<CardData>(availableCards);

        for (int i = 0; i < 3; i++)
        {
            if(availableCopy.Count > 0)
            {
                int randomIndex = Random.Range(0, availableCopy.Count);
                randomCards.Add(availableCopy[randomIndex]);
                availableCopy.RemoveAt(randomIndex); // Remove para evitar duplicadas
            }
            
        }

        // Preenche os botões do container
        Button[] cardButtons = newCardContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (i < randomCards.Count)
            {
                CardData card = randomCards[i];
                cardButtons[i].gameObject.SetActive(true);
                cardButtons[i].onClick.RemoveAllListeners();
                cardButtons[i].onClick.AddListener(() => AddNewCard(card));

                // Atualize as informações visuais do botão aqui
                // Exemplo: cardButtons[i].GetComponentInChildren<Text>().text = card.cardName;
                if(card.attackValue > 0)
                cardButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = card.attackValue.ToString();
                else if(card.defenseValue > 0)
                cardButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = card.defenseValue.ToString();

                cardButtons[i].image.sprite = card.cardSprite;
            }
            else
            {
                cardButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ShowUpgradeOptions()
    {
        newCardContainer.SetActive(false);
        upgradeCardContainer.SetActive(true);

        // Limpa o container
        foreach (Transform child in upgradeCardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Filtra cartas upgradáveis
        List<CardData> upgradableCards = LevelManager.Instance.mainDeck.FindAll(card => card.evolvedCard != null);
        Debug.Log($"Cartas disponíveis para upgrade: {upgradableCards.Count}");

        foreach (CardData card in upgradableCards)
        {
            GameObject upgradeButton = Instantiate(upgradeButtonPrefab, upgradeCardContainer.transform);
            Button button = upgradeButton.GetComponent<Button>();

            // Configure as informações da carta no botão
            // Exemplo: Mostrar nome e descrição da carta atual e evoluída
            TextMeshProUGUI[] texts = upgradeButton.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = $"Atual: {card.cardName}";
            texts[1].text = $"Evolução: {card.evolvedCard.cardName}";

            button.image.sprite = card.cardSprite;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => UpgradeCard(card));
        }
    }

    public void AddNewCard(CardData newCard)
    {
        if(!newCard.isTemporary)
        {
        newCard.isTemporary = true; // Marca como temporaria
        }
        LevelManager.Instance.AddCardToMainDeck(newCard);
        GoToNextBattle();
    }

    public void UpgradeCard(CardData card)
    {
        if (LevelManager.Instance.mainDeck.Contains(card) && card.evolvedCard != null)
        {
            LevelManager.Instance.TrackEvolution(card, card.evolvedCard);
            LevelManager.Instance.UpgradeCardInMainDeck(card, card.evolvedCard);
        }

        GoToNextBattle();
    }

    public void Skip()
    {
        Debug.Log("O jogador escolheu pular o upgrade.");
        GoToNextBattle();
    }

    private void GoToNextBattle()
    {
        SceneManager.LoadScene("BattleScene");
    }
}
