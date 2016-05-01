using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager_StartMenu : MonoBehaviour
{

    public GameObject startWindow;
    public GameObject guideWindow;

    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        showStartWindow();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showGuidWindow()
    {
        startWindow.SetActive(false);
        guideWindow.SetActive(true);
    }

    public void showStartWindow()
    {
        startWindow.SetActive(true);
        guideWindow.SetActive(false);
    }

    public void startNewGame()
    {
        SceneManager.LoadScene("MainGame");
    }
}
