using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager_MainGame : MonoBehaviour
{

    public GameObject[] EnergyBar;
    public GameObject[] StarBar;
    public GameObject ReadyBar;

    public Button[] AttackButtons;
    public Button[] MoveButtons;

    public GameObject Canves;
    public GameObject pauseWindow;

    //[player][skill][state]
    public Sprite[,,] AttackButtonSprites = new Sprite[2, 4, 2];


    private static float FREEZE_TIME = 2f;

    //private static String[] AttackButtonTextList = { "None", "Charge", "GiantGrowth", "ShockWave" };
    public Image[] AttackButtonImages;

    public static UIManager_MainGame instance = null;

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
    IEnumerator Start()
    {
        pauseWindow.SetActive(false);

        //load sprites
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    AttackButtonSprites[i, j, k] = Resources.Load<Sprite>("AttackButton/SkillButton_Player" + i + j + k);
                }
            }
        }

        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            PlayerController script = GameManager.instance.players[i].GetComponent<PlayerController>();

            setEnergyBar(EnergyBar[i], (float)script.getPowerCount() / PlayerController.POWER_MAX_RANGE);

            setStarBar(StarBar[i], (float)PlayerPrefs.GetInt(GameManager.SCORE_STR[i]) / GameManager.WIN_SCORE);


            //AttackButtons[i].GetComponentInChildren<Text>().text = AttackButtonTextList[script.getSkillLevel() + 1];
        }

        Debug.Log("Start Pause");
        yield return GameManager.instance.PauseGameForSeconds(FREEZE_TIME);
        ReadyBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused || GameManager.instance.isGameOver)
        {
            return;
        }
        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            PlayerController script = GameManager.instance.players[i].GetComponent<PlayerController>();

            setEnergyBar(EnergyBar[i], (float)script.getPowerCount() / PlayerController.POWER_MAX_RANGE);

            setStarBar(StarBar[i], (float)PlayerPrefs.GetInt(GameManager.SCORE_STR[i]) / GameManager.WIN_SCORE);

            setAttackButtonSprite(AttackButtons[i], i, script.getSkillLevel() + 1);

        }
    }

    void setAttackButtonSprite(Button button, int player, int level)
    {
        button.GetComponentInChildren<Image>().sprite = AttackButtonSprites[player, level, 0];
        SpriteState state = new SpriteState();
        state.pressedSprite = AttackButtonSprites[player, level, 1];
        button.spriteState = state;
    }

    void setEnergyBar(GameObject energyBar, float amount)
    {
        energyBar.GetComponent<Image>().fillAmount = amount;
    }

    void setStarBar(GameObject starBar, float amount)
    {
        starBar.GetComponent<Image>().fillAmount = amount;
    }

    public void enbleButtons(bool active)
    {
        for (int i = 0; i < AttackButtons.Length; i++)
        {
            AttackButtons[i].interactable = active;
        }
        for (int i = 0; i < MoveButtons.Length; i++)
        {
            MoveButtons[i].interactable = active;
        }
    }

    public void visibleButtons(bool visible)
    {
        Canves.SetActive(visible);
    }

    public void showPauseWindow()
    {
        pauseWindow.SetActive(true);
    }

    public void showGame()
    {
        pauseWindow.SetActive(false);
    }
}
