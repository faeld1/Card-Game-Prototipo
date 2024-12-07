using UnityEngine;

public class UI_Menu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject playPanel;
    public GameObject characterPanel;
    public GameObject craftPanel;
    public GameObject dungeonsPanel;
    public GameObject settingsPanel;

    [Header("Dependencies")]
    public LevelMenuManager levelMenuManager; // Referência ao LevelMenuManager

    private void Start()
    {
        ShowPlayPanel(); // Exibe Play por padrão
    }

    public void ShowPlayPanel()
    {
        CloseAllPanels();
        playPanel.SetActive(true);

        // Configurar botões de nível sempre que abrir a aba Play
        if (levelMenuManager != null)
        {
            levelMenuManager.SetupLevelButtons();
        }
    }

    public void ShowCharacterPanel()
    {
        CloseAllPanels();
        characterPanel.SetActive(true);
    }

    public void ShowCraftPanel()
    {
        CloseAllPanels();
        craftPanel.SetActive(true);
    }

    public void ShowDungeonsPanel()
    {
        CloseAllPanels();
        dungeonsPanel.SetActive(true);
    }

    public void ShowSettingsPanel()
    {
        CloseAllPanels();
        settingsPanel.SetActive(true);
    }

    private void CloseAllPanels()
    {
        playPanel.SetActive(false);
        characterPanel.SetActive(false);
        craftPanel.SetActive(false);
        dungeonsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
}
