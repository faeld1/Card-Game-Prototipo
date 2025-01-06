using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons; // Botões do menu de níveis
    [SerializeField] private LevelData[] levelsData; // Referências aos dados dos níveis
    [SerializeField] private TextMeshProUGUI levelNameText; // Texto para o nome da fase no painel esquerdo
    [SerializeField] private Button startLevelButton; // Botão "Começar" no painel esquerdo
    [SerializeField] private Color selectedColor = Color.green; // Cor para a fase selecionada
    [SerializeField] private Color defaultColor = Color.white; // Cor padrão dos botões

    private int selectedLevelIndex = -1; // Índice do nível selecionado

    private void Start()
    {
        SetupLevelButtons();
        SelectLastUnlockedLevel();
    }

    public void SetupLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i;
            LevelData levelData = levelsData[i];

            bool isUnlocked = LevelManager.Instance.IsLevelUnlocked(levelData.levelNumber);
           // Debug.Log($"Configuring Level {levelData.levelNumber}: Unlocked = {isUnlocked}");

            levelButtons[i].interactable = isUnlocked;

            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => OnLevelButtonClick(levelIndex));

            // Resetando a cor padrão dos botões
            levelButtons[i].GetComponent<Image>().color = defaultColor;
        }
    }

    private void OnLevelButtonClick(int levelIndex)
    {
        if (selectedLevelIndex != -1)
        {
            // Reseta a cor do botão previamente selecionado
            levelButtons[selectedLevelIndex].GetComponent<Image>().color = defaultColor;
        }

        // Define o novo índice selecionado
        selectedLevelIndex = levelIndex;

        // Muda a cor do botão selecionado
        levelButtons[selectedLevelIndex].GetComponent<Image>().color = selectedColor;

        // Atualiza o painel esquerdo
        UpdateLeftPanel(levelsData[levelIndex]);
    }
    private void UpdateLeftPanel(LevelData levelData)
    {
        // Atualiza o nome da fase no painel esquerdo
        levelNameText.text = $"Fase: {levelData.levelName}";

        // Atualiza o botão "Começar" para iniciar a fase selecionada
        startLevelButton.onClick.RemoveAllListeners();
        startLevelButton.onClick.AddListener(() => LevelManager.Instance.SelectLevel(levelData));
    }
    private void SelectLastUnlockedLevel()
    {
        // Seleciona automaticamente a última fase desbloqueada
        for (int i = levelsData.Length - 1; i >= 0; i--)
        {
            if (LevelManager.Instance.IsLevelUnlocked(levelsData[i].levelNumber))
            {
                OnLevelButtonClick(i);
                break;
            }
        }
    }
}
