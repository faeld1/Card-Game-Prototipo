using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    [Header("Special Settings")]
    public TextMeshProUGUI rageText;
    public Button specialAttackButton;
    [SerializeField] private GameObject[] specialMarkers; // Referencie as bolinhas (de 0 a 2)
    [SerializeField] private Image specialSwordGlow;
    [SerializeField] private RectTransform specialSword;
    [SerializeField] private GameObject specialBGYellow;
    [SerializeField] private GameObject flameAnim;
    [SerializeField] private float markerPulseScale = 1.2f;
    [SerializeField] private float pulseDuration = 0.5f;
    private Quaternion swordInitialRotation;

    public Button endTurnButton;

    [SerializeField] private TextMeshProUGUI playerDamageText;
    [SerializeField] private TextMeshProUGUI playerDefenseText;

    [Header("Arrow Advise")]
    [SerializeField] private RectTransform endTurnArrow;
    [SerializeField] private float arrowMoveSpeed = 8f; // Velocidade do movimento
    [SerializeField] private float arrowMoveRange = 10f; // Distância do movimento no eixo Y
    private Vector3 arrowStartPos;// Posição inicial da seta
    private bool isArrowVisible = false;

    [Header("Who's Turn")]
    [SerializeField] private RectTransform whoTurnContainer;
    [SerializeField] private RectTransform whoTurnStartPos;
    [SerializeField] private RectTransform whoTurnEndPos;
    private Coroutine whoTurnCoroutine;

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
        ResetSpecialMarkers();

        DisableArrow();

        UpdatePlayerStatsUI();

        SpecialAttackSetupStart();

        StartCoroutine(MoveUI(whoTurnContainer, whoTurnEndPos, whoTurnStartPos, 0f));
    }



    private IEnumerator MoveUI(RectTransform target, RectTransform startRef, RectTransform endRef, float duration, float delayBefore = 0f, float delayAfter = 0f)
    {
        if (startRef == null || endRef == null || target == null)
        {
            Debug.LogWarning("Alguma referência de RectTransform está nula.");
            yield break;
        }

        if (delayBefore > 0f)
            yield return new WaitForSeconds(delayBefore);

        Vector2 startPos = startRef.anchoredPosition;
        Vector2 endPos = endRef.anchoredPosition;

        float elapsedTime = 0f;
        target.anchoredPosition = startPos;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.anchoredPosition = endPos;

        if (delayAfter > 0f)
            yield return new WaitForSeconds(delayAfter);
    }

    //Settings do Who's Turn abaixo no Region
    #region Who's Turn
    public void MoveWhoTurn(bool inOrOut, float delay)
    {
        if (whoTurnCoroutine != null)
            StopCoroutine(whoTurnCoroutine);

        whoTurnCoroutine = StartCoroutine(MoveWhoTurnSequence(inOrOut, delay));
    }
    private IEnumerator MoveWhoTurnSequence(bool inOrOut, float delay)
    {
        if (inOrOut)
        {
            // Entra
            yield return StartCoroutine(MoveUI(whoTurnContainer, whoTurnStartPos, whoTurnEndPos, .5f));
            // Espera 5s
            yield return new WaitForSeconds(4f);
            // Sai
            yield return StartCoroutine(MoveUI(whoTurnContainer, whoTurnEndPos, whoTurnStartPos, .5f));
        }
        else
        {
            // Sai direto com delay
            yield return new WaitForSeconds(delay);
            yield return StartCoroutine(MoveUI(whoTurnContainer, whoTurnEndPos, whoTurnStartPos, .5f));
        }
    }
    #endregion

    private void UpdatePlayerStatsUI()
    {
        playerDamageText.text = "" + BattleManager.instance.playerStats.damage.GetValue();
        playerDefenseText.text = "" + BattleManager.instance.playerStats.armor.GetValue();
    }
    public void UpdateRageUI(int rage)
    {
        rageText.text = rage.ToString();
        specialAttackButton.interactable = rage >= 3;
        if (rage >= 3)
        {
            SpecialSwordGlow(1f);
        }

        UpdateSpecialMarkers(rage);
    }
    //Settings do Special Attack Abaixo no Region
    #region Special Attack
    private void SpecialAttackSetupStart()
    {
        swordInitialRotation = specialSword.rotation;
        SpecialSwordGlow(0f);
        specialSwordGlow.gameObject.SetActive(false);
        flameAnim.SetActive(false);
        specialBGYellow.SetActive(false);
    }
    // Escalamento animado de marcador
    private IEnumerator AnimateMarker(GameObject marker)
    {
        Vector3 originalScale = marker.transform.localScale;
        Vector3 targetScale = originalScale * markerPulseScale;
        float elapsed = 0;

        while (elapsed < pulseDuration / 2f)
        {
            marker.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / (pulseDuration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }
        marker.transform.localScale = targetScale;

        elapsed = 0;
        while (elapsed < pulseDuration / 2f)
        {
            marker.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / (pulseDuration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }
        marker.transform.localScale = originalScale;
    }
    private IEnumerator RotateSword(float targetZ, float duration)
    {
        Quaternion startRot = specialSword.rotation;
        Quaternion endRot = Quaternion.Euler(0, 0, targetZ);
        float elapsed = 0;

        while (elapsed < duration)
        {
            specialSword.rotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        specialSword.rotation = endRot;
    }
    private IEnumerator FadeImageAlpha(Image image, float from, float to, float duration)
    {
        float elapsed = 0;
        Color color = image.color;
        color.a = from;
        image.color = color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(from, to, elapsed / duration);
            image.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        color.a = to;
        image.color = color;
    }
    private void UpdateSpecialMarkers(int rage)
    {
        for (int i = 0; i < specialMarkers.Length; i++)
        {
            bool shouldBeActive = rage > i;
            GameObject marker = specialMarkers[i];

            if (marker.activeSelf != shouldBeActive)
            {
                marker.SetActive(shouldBeActive);
                if (shouldBeActive)
                    StartCoroutine(AnimateMarker(marker));
            }
        }

        if (rage >= 3)
        {
            StartCoroutine(SpecialChargeUpSequence());
        }
    }

    private IEnumerator SpecialChargeUpSequence()
    {
        yield return StartCoroutine(RotateSword(180f, 0.35f));
        specialBGYellow.SetActive(true);
        specialSwordGlow.gameObject.SetActive(true);
        flameAnim.SetActive(true);
        yield return StartCoroutine(FadeImageAlpha(specialSwordGlow, 0f, .2f, 0.2f));
    }

    public void ResetSpecialMarkers()
    {
        foreach (var marker in specialMarkers)
            if (marker != null) marker.SetActive(false);

        StartCoroutine(SpecialResetSequence());
    }
    public void ResetSpecialEffects()
    {
        StartCoroutine(SpecialResetSequence());
    }
    private IEnumerator SpecialResetSequence()
    {
        yield return StartCoroutine(RotateSword(0f, 0.5f));
        yield return StartCoroutine(FadeImageAlpha(specialSwordGlow, 1f, 0f, 0.2f));
        specialSwordGlow.gameObject.SetActive(false);
        specialBGYellow.SetActive(false);
        flameAnim.SetActive(false);
    }
    private void SpecialSwordGlow(float glowAlpha)
    {
        if (specialSwordGlow != null)
        {
            Color color = specialSwordGlow.color;
            color.a = glowAlpha; // Alpha em 0 = totalmente transparente
            specialSwordGlow.color = color;
        }
    }
    #endregion

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

        specialAttackButton.gameObject.SetActive(false); //Some com o botao de Special quando ganha ou perde o sub-level(Stage)

        if (wonOrLose)
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
        if (wonOrLose)
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
