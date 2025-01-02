using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    public RectTransform endGameContainer;

    private Vector2 ShowEndGameContainerPosition;
    private Vector2 hiddenEndGamePosition;
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
       // originalEndGameContainerPosition = endGameContainer.anchoredPosition;
        ShowEndGameContainerPosition = new Vector2 (0, 0);
        hiddenEndGamePosition = ShowEndGameContainerPosition + new Vector2(0, -1100);

        // hiddenEndGamePosition = originalEndGameContainerPosition + new Vector2(0, -1100);

        HideEndGame();
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
        SceneManager.LoadScene("MainMenu");
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
