using UnityEngine;
using UnityEngine.UI;
public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons; // Bot�es do menu de n�veis
    [SerializeField] private LevelData[] levelsData; // Refer�ncias aos dados dos n�veis

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
