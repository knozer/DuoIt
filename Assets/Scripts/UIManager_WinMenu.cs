using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager_WinMenu : MonoBehaviour
{

    public Text WinLoseText;

    public static UIManager_WinMenu instance = null;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

    }

    // Use this for initialization
    void Start()
    {
        WinLoseText.text = PlayerPrefs.GetString("Winner")+" Wins";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startNewGame()
    {
        //GenericWindow.newgame = false;
        SceneManager.LoadScene("MainGame");
    }

}
