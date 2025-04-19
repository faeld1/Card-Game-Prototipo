using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    public TextMeshProUGUI rageText;
    public Button specialAttackButton;

    public Button endTurnButton;

    [SerializeField] private TextMeshProUGUI playerDamageText;
    [SerializeField] private TextMeshProUGUI playerDefenseText;

    [Header("Seta Advise")]
    [SerializeField] private RectTransform endTurnArrow;
    [SerializeField] private float arrowMoveSpeed = 8f; // Velocidade do movimento
    [SerializeField] private float arrowMoveRange = 10f; // Distância do movimento no eixo Y
    private Vector3 arrowStartPos;// Posição inicial da seta
    private bool isArrowVisible = false;

    public RectTransform endGameContainer;
    [SerializeField] private RectTransform loseGameContainer;
    [SerializeField] private RectTransform blockHandCardsContainer;
    [SerializeField] private RectTransform victoryContainer;

    [SerializeField] private float endGameCallDelay = 1f;
    private Vector2 ShowEndGameContainerPosition;
    private Vector2 hiddenEndGamePosition;

    private Vector2 ShowLoseGameContainerPosition;
    private Vector2 hiddenLoseGamePosition;

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
        HideLoseGame();

        blockHandCardsContainer.anchoredPosition = hiddenBlockHandCardsPosition; //esconde as cartas de bloqueio

        specialAttackButton.onClick.AddListener(() => BattleManager.instance.playerStats.player.UseSpecialAttack());
        //endTurnButton.onClick.AddListener(() => BattleManager.instance.ForceEndTurn());

        UpdateRageUI(0);

        DisableArrow();

        UpdatePlayerStatsUI();
    }

    private void UpdatePlayerStatsUI()
    {
        playerDamageText.text = "Dano: " + BattleManager.instance.playerStats.damage.GetValue();
        playerDefenseText.text = "Defesa: " + BattleManager.instance.playerStats.armor.GetValue();
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

        ShowLoseGameContainerPosition = new Vector2(0, 0);
        hiddenLoseGamePosition = ShowLoseGameContainerPosition + new Vector2(0, -1100);

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
    private void ShowVictoryContainer(bool wonOrLose)
    {
        TextMeshProUGUI victoryText = victoryContainer.GetComponentInChildren<TextMeshProUGUI>();

        if(wonOrLose) 
            victoryText.text = "Victory!";
        else
            victoryText.text = "Lose!";
        
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

    private IEnumerator ShowEndGameDelay(bool wonOrLose)
    {
        yield return new WaitForSeconds(endGameCallDelay);
        ShowVictoryContainer(wonOrLose);

        yield return new WaitForSeconds(1.5f);
        if(wonOrLose)
        {
            MoveEndGame(ShowEndGameContainerPosition);
            endGameContainer.gameObject.SetActive(true);
        }
        else
        {
            MoveLoseGame(ShowLoseGameContainerPosition);
            loseGameContainer.gameObject.SetActive(true);
        }
        
    }
    public void ShowEndGame(bool wonOrLose)
    {
        StartCoroutine(ShowEndGameDelay(wonOrLose));
    }
    //Won Game
    private void HideEndGame()
    {
        MoveEndGame(hiddenEndGamePosition);
        endGameContainer.gameObject.SetActive(false);
    }

    private void MoveEndGame(Vector2 targetPosition)
    {
        endGameContainer.anchoredPosition = targetPosition;
    }
    //Lose Game
    private void HideLoseGame()
    {
        MoveLoseGame(hiddenLoseGamePosition);
        loseGameContainer.gameObject.SetActive(false);
    }
    private void MoveLoseGame(Vector2 targetPosition)
    {
        loseGameContainer.anchoredPosition = targetPosition;
    }
    // ARROW ADVISE
    public IEnumerator ShowEndTurnArrow()
    {
        if (isArrowVisible) yield break; // Se já estiver visível, não faz nada

        isArrowVisible = true;
        endTurnArrow.gameObject.SetActive(true);

        arrowStartPos = endTurnArrow.anchoredPosition;

        while (isArrowVisible)
        {
            float newY = arrowStartPos.y + Mathf.Sin(Time.time * arrowMoveSpeed) * arrowMoveRange;
            endTurnArrow.anchoredPosition = new Vector2(arrowStartPos.x, newY);
            yield return null;
        }
    }

    public void DisableArrow()
    {
        isArrowVisible = false;
        endTurnArrow.gameObject.SetActive(false);
    }

    public void DeleteSaves()
    {
        PlayerPrefs.DeleteAll();
        ES3.DeleteFile();
    }

    private void OnEnable()
    {
        PlayerStats.OnPlayerLevelUp += UpdatePlayerStatsUI;
    }

    private void OnDisable()
    {
        PlayerStats.OnPlayerLevelUp -= UpdatePlayerStatsUI;
    }
}
