using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    public RectTransform endGameContainer;
    [SerializeField]private RectTransform blockHandCardsContainer;

    private Vector2 ShowEndGameContainerPosition;
    private Vector2 hiddenEndGamePosition;

    private Vector2 blockHandCardsPosition;
    private Vector2 hiddenBlockHandCardsPosition;
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
        ShowEndGameContainerPosition = new Vector2 (0, 0);
        hiddenEndGamePosition = ShowEndGameContainerPosition + new Vector2(0, -1100);

        blockHandCardsPosition = blockHandCardsContainer.anchoredPosition;
        hiddenBlockHandCardsPosition = blockHandCardsPosition + new Vector2(0, -1100);

        HideEndGame();
        blockHandCardsContainer.anchoredPosition = hiddenBlockHandCardsPosition; //esconde as cartas de bloqueio
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
        yield return new WaitForSeconds(1f);
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
