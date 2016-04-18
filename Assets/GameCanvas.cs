using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCanvas : MonoBehaviour 
{
    public bool IsPause { private set; get; }

    [SerializeField]
    private Text levelDisplay = null;
    [SerializeField]
    private GameObject menuPanel = null;
    [SerializeField]
    private GameObject helpPanel = null;
    [SerializeField]
    private GameObject readyPanel = null;

    void Start()
    {
        this.menuPanel.SetActive(false);
        this.helpPanel.SetActive(false);
        this.levelDisplay.text = string.Format("Level: {0}", GameManager.Level);
#if UNITY_ANDROID && !UNITY_EDITOR
        this.OpenReady();
#else
        this.readyPanel.SetActive(false);
#endif
    }


    public void OpenMenu()
    {
        this.menuPanel.SetActive(true);
        Time.timeScale = 0;
        this.IsPause = true;
    }

    public void CloseMenu()
    {
        this.menuPanel.SetActive(false);
        Time.timeScale = 1;
        this.IsPause = false;
    }

    public void Quit()
    {
        CloseMenu();
        SceneManager.LoadScene("main_menu");
    }

    public void OpenHelp()
    {
        this.helpPanel.SetActive(true);
        this.menuPanel.SetActive(false);
    }
    public void CloseHelp()
    {
        this.helpPanel.SetActive(false);
        this.menuPanel.SetActive(true);
    }

    public void CloseReady()
    {
        this.readyPanel.SetActive(false);
        Time.timeScale = 1;
        this.IsPause = false;
    }

    public void OpenReady()
    {
        this.readyPanel.SetActive(true);
        Time.timeScale = 0;
        this.IsPause = true;
    }


}
