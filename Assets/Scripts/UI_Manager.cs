using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    public TextMeshProUGUI rageText;
    public Button specialAttackButton;

    public RectTransform endGameContainer;
    [SerializeField]private RectTransform blockHandCardsContainer;
    [SerializeField] private RectTransform victoryContainer;

    [SerializeField]private float endGameCallDelay = 1f;
    private Vector2 ShowEndGameContainerPosition;
    private Vector2 hiddenEndGamePosition;

    private Vector2 blockHandCardsPosition;
    private Vector2 hiddenBlockHandCardsPosition;

    private Vector2 victoryContainerPosition;
    private Vector2 hiddenVictoryContainerPosition;
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
        SetupUIContainers();

        HideEndGame();
        blockHandCardsContainer.anchoredPosition = hiddenBlockHandCardsPosition; //esconde as cartas de bloqueio

        specialAttackButton.onClick.AddListener(() => BattleManager.instance.playerStats.player.UseSpecialAttack());

        UpdateRageUI(0);
    }
    public void UpdateRageUI(int rage)
    {
        rageText.text = "Raiva: " + rage;
        specialAttackButton.interactable = rage >= 3;
    }

    private void SetupUIContainers()
    {
        ShowEndGameContainerPosition = new Vector2(0, 0);
        hiddenEndGamePosition = ShowEndGameContainerPosition + new Vector2(0, -1100);

        blockHandCardsPosition = blockHandCardsContainer.anchoredPosition;
        hiddenBlockHandCardsPosition = blockHandCardsPosition + new Vector2(0, -1100);

        victoryContainerPosition = victoryContainer.anchoredPosition;
        hiddenVictoryContainerPosition = new Vector2(0, 700);
        victoryContainer.anchoredPosition = hiddenVictoryContainerPosition;
    }

    public void HideBlockHandCards()
    {
        //Debug.Log("HideBlockHandsCards chamado");
        StartCoroutine(HideBlockHandCardsDelay());
    }
    public void ShowBlockHandCards()
    {
        //Debug.Log("ShowBlockHandsCards chamado");
        blockHandCardsContainer.anchoredPosition = blockHandCardsPosition;
    }
    private IEnumerator HideBlockHandCardsDelay()
    {
        //yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(0.01f);
        //Debug.Log("HideBlockHandsCards chamado");
        blockHandCardsContainer.anchoredPosition = hiddenBlockHandCardsPosition;
    }
    private void ShowVictoryContainer()
    {
        victoryContainer.anchoredPosition = victoryContainerPosition;
    }

    public void ExitScene()
    {
        //HideEndGame();
        
        StartCoroutine(ExitAfterDelay());
        //criar depois uma solução melhor para isso aqui:   
    }

    private IEnumerator ExitAfterDelay()
    {
        yield return new WaitForSeconds(0.01f);
        LevelManager.Instance.ReturnToMenu();
        //SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator ShowEndGameDelay()
    {
        yield return new WaitForSeconds(endGameCallDelay);
        ShowVictoryContainer();

        yield return new WaitForSeconds(1.5f);
        MoveEndGame(ShowEndGameContainerPosition);
        endGameContainer.gameObject.SetActive(true);
    }

    public void ShowEndGame()
    {
        StartCoroutine(ShowEndGameDelay());
    }

    public void HideEndGame()
    {
        MoveEndGame(hiddenEndGamePosition);
        endGameContainer.gameObject.SetActive(false);
    }

    private void MoveEndGame(Vector2 targetPosition)
    {
        endGameContainer.anchoredPosition = targetPosition;
    }

    public void DeleteSaves()
    {
        PlayerPrefs.DeleteAll();
        ES3.DeleteFile();
    }
}
