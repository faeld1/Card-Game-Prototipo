using UnityEngine;

public class UI_Menu : MonoBehaviour
{
    public void DeleteSaves()
    {
        PlayerPrefs.DeleteAll();
        ES3.DeleteFile();
    }
}
