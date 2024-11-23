using UnityEngine;
using UnityEngine.UI;
public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons; // Botões do menu de níveis
    [SerializeField] private LevelData[] levelsData; // Referências aos dados dos níveis

    private void Start()
    {
        SetupLevelButtons();
    }

    private void SetupLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i;
            LevelData levelData = levelsData[i];

            bool isUnlocked = LevelManager.Instance.IsLevelUnlocked(levelData.levelNumber);
            Debug.Log($"Configuring Level {levelData.levelNumber}: Unlocked = {isUnlocked}");

            levelButtons[i].interactable = isUnlocked;

            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => LevelManager.Instance.SelectLevel(levelData));
        }
    }
}
