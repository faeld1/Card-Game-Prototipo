using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public LevelData currentLevel;
    public int currentSubLevelIndex = 0; // subnivel atual

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

    public void AdavanceSubLevel()
    {
        currentSubLevelIndex++;

        if (currentSubLevelIndex >= 5) // Terminou todos os subníveis
        {
            Debug.Log("Level Complete!");
            SaveLevelProgress(currentLevel.levelNumber);

            // Carrega o menu principal após concluir o último subnível
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            // Carrega a próxima cena de melhoria
            SceneManager.LoadScene("UpgradeScene");
        }
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
        if (nextLevel <= 5) // Substitua 5 pelo número total de níveis no jogo
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
}
