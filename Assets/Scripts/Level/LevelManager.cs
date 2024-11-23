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

            // Garante que o primeiro n�vel est� desbloqueado por padr�o
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

        if (currentSubLevelIndex >= 5) // Terminou todos os subn�veis
        {
            Debug.Log("Level Complete!");
            SaveLevelProgress(currentLevel.levelNumber);

            // Carrega o menu principal ap�s concluir o �ltimo subn�vel
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            // Carrega a pr�xima cena de melhoria
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

        // Desbloqueia o pr�ximo n�vel
        int nextLevel = levelNumber + 1;
        if (nextLevel <= 5) // Substitua 5 pelo n�mero total de n�veis no jogo
        {
            ES3.Save($"Level_{nextLevel}_Unlocked", true);
        }
    }

    /// Verifica se um n�vel est� desbloqueado.
    /// <returns>Retorna verdadeiro se o n�vel estiver desbloqueado.</returns>
    public bool IsLevelUnlocked(int levelNumber)
    {
        return ES3.KeyExists($"Level_{levelNumber}_Unlocked") && ES3.Load<bool>($"Level_{levelNumber}_Unlocked");
    }

    /// Define o n�vel atual e carrega a cena de batalha.
    /// Este m�todo deve ser associado aos bot�es de sele��o de n�vel.
    public void SelectLevel(LevelData levelData)
    {
        if (levelData.isUnlocked)
        {
            currentLevel = levelData;
            currentSubLevelIndex = 0; // Reinicia no subn�vel 0
            SceneManager.LoadScene("BattleScene"); // Carrega a cena de batalha
        }
        else
        {
            Debug.LogWarning("Level is locked!");
        }
    }
}
