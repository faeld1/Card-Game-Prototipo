using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public LevelData currentLevel;
    public int currentSubLevelIndex = 0; // subnivel atual
    [SerializeField] private int subLevelTimes = 5;

    //Cartas para serem enviadas para o MainDeck da proxima rodada
    public List<CardData> mainDeck = new List<CardData>(); // Baralho principal do jogador

    private Dictionary<CardData, CardData> evolvedCards = new Dictionary<CardData, CardData>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Garante que o primeiro nível está desbloqueado por padrão
            if (!ES3.KeyExists("Level_1_Unlocked"))
            {
                ES3.Save("Level_1_Unlocked", true);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //subLevelTimes -= 1;
    }

    public void AdavanceSubLevel()
    {
        currentSubLevelIndex++;

        if (currentSubLevelIndex > subLevelTimes) // Terminou todos os subníveis - Ganhou o level
        {
            Debug.Log("Level Complete!");
            SaveLevelProgress(currentLevel.levelNumber);

            // Limpa cartas temporárias antes de retornar ao menu principal
            ClearTemporaryCards();
            RevertEvolutions();
            // Carrega o menu principal após concluir o último subnível
            SceneManager.LoadScene("MainMenu");

            return;
        }
        //Ganhou o subnivel

        UI_Manager.instance.ShowEndGame();
        DeckManager.instance.HideHand();
      
    }

    public void GoToUpgradeScene()
    {
        SceneManager.LoadScene("UpgradeScene");
    }

    public void ResetSubLevelProgress()
    {
        currentSubLevelIndex = 0;
    }

    public void SaveLevelProgress(int levelNumber)
    {
        // Salva o progresso do Level no EasySave
        ES3.Save($"Level_{levelNumber}_Unlocked", true);

        // Desbloqueia o próximo nível
        int nextLevel = levelNumber + 1;
        if (nextLevel <= subLevelTimes) // Substitua 5 pelo número total de níveis no jogo
        {
            ES3.Save($"Level_{nextLevel}_Unlocked", true);
        }
    }

    /// Verifica se um nível está desbloqueado.
    /// <returns>Retorna verdadeiro se o nível estiver desbloqueado.</returns>
    public bool IsLevelUnlocked(int levelNumber)
    {
        return ES3.KeyExists($"Level_{levelNumber}_Unlocked") && ES3.Load<bool>($"Level_{levelNumber}_Unlocked");
    }

    /// Define o nível atual e carrega a cena de batalha.
    /// Este método deve ser associado aos botões de seleção de nível.
    public void SelectLevel(LevelData levelData)
    {
        if (levelData.isUnlocked)
        {
            currentLevel = levelData;
            currentSubLevelIndex = 0; // Reinicia no subnível 0
            SceneManager.LoadScene("BattleScene"); // Carrega a cena de batalha
        }
        else
        {
            Debug.LogWarning("Level is locked!");
        }
    }

    //Relacionado a passagem das cartas ganhas.
    public void AddCardToMainDeck(CardData card)
    {
        mainDeck.Add(card);
    }

    public void UpgradeCardInMainDeck(CardData originalCard, CardData upgradedCard)
    {
        int index = mainDeck.IndexOf(originalCard);
        if (index >= 0)
        {
            mainDeck[index] = upgradedCard;
        }
        else
        {
            Debug.LogWarning($"Carta {originalCard.cardName} não encontrada no mainDeck.");
        }
    }

    public void ClearTemporaryCards()
    {
        // Remove todas as cartas marcadas como temporárias
        mainDeck.RemoveAll(card => card.isTemporary); // Supondo que `isTemporary` é um campo booleano em CardData
        //Debug.Log("Cartas temporárias removidas do mainDeck.");
    }
    public void TrackEvolution(CardData originalCard, CardData evolvedCard)
    {
        if (!evolvedCards.ContainsKey(originalCard))
        {
            evolvedCards.Add(originalCard, evolvedCard);
            Debug.Log($"Evolução rastreada: {originalCard.cardName} > {evolvedCard.cardName}");
        }
    }
    public void RevertEvolutions()
    {
        foreach (var pair in evolvedCards)
        {
            CardData evolvedCard = pair.Value;
            CardData originalCard = pair.Key;

            // Substituir a carta evoluída pela original no mainDeck
            int index = mainDeck.IndexOf(evolvedCard);
            if (index >= 0)
            {
                mainDeck[index] = originalCard;
                Debug.Log($"Evolução revertida: {evolvedCard.cardName} > {originalCard.cardName}");
            }
        }

        // Limpar o dicionário após reverter
        evolvedCards.Clear();
    }
    public void ReturnToMenu()
    {
        // Limpa cartas temporárias e reverte evoluções antes de retornar ao menu
        ClearTemporaryCards();
        RevertEvolutions();

        // Carrega o menu principal
        SceneManager.LoadScene("MainMenu");
    }
    //
}
