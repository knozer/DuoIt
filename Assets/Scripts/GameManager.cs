using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    [Serializable]
    public class Count
    {
        public float minimum;             //Minimum value for our Count class.
        public float maximum;             //Maximum value for our Count class.

        //Assignment constructor.
        public Count(float min, float max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public GameObject[] players;
    public static string[] SCORE_STR = { "Blue", "Red" };
    //
    public bool isFallen = false;
    public GameObject fallenPlayer;

    public static int WIN_SCORE = 5;

    //basic set up
    public static GameManager instance = null;
    public GameObject[] PickUps;
    //public GameObject OutterShockWave;
    //public GameObject InnerShockWave;

    private static float pi = (float)Math.PI;

    //generate item
    private float radius;
    private static float[] ItemProbability = { 0f, 0.85f, 0.95f };
    private Count spawnInterval = new Count(4f, 5f);
    private List<ItemController> itemsOnBoard;
    private int MaxItemsOnBoard = 3;
    private float nextSpawnTime;
    private Vector3 boardCenter;
    private Vector3 boardSize;

    private static GameObject fallingPlayer;
    //private float EndPauseTime=0;

    public bool isPaused = false;
    public bool isGameOver = false;

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
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        boardCenter = sr.bounds.center;
        boardSize = sr.bounds.size * 0.8f;
        radius = boardSize.x / 3;

        nextSpawnTime = Random.Range(spawnInterval.minimum, spawnInterval.maximum);
        itemsOnBoard = new List<ItemController>();
    }

    public float getRadius()
    {
        return radius;
    }

    public void addItemToList(ItemController item)
    {
        itemsOnBoard.Add(item);
    }

    public void removeItemFromList(ItemController item)
    {
        itemsOnBoard.Remove(item);
    }


    // Update is called once per frame
    void Update()
    {
        if (isPaused || isGameOver)
        {
            return;
        }
        //generate items if not reach maximum
        if (itemsOnBoard.Count < MaxItemsOnBoard)
        {
            nextSpawnTime -= Time.deltaTime;
            if (nextSpawnTime <= 0)
            {
                float x = 0, y = 0, r = 0, colliderRadius = 0;
                GameObject toInstantiate = null;
                Vector3 instantiatePosition = new Vector3();
                CircleCollider2D BackGroundCollider = GetComponent<CircleCollider2D>();
                bool isValidPosition = false;
                while (toInstantiate == null || !isValidPosition)
                {
                    nextSpawnTime = Random.Range(spawnInterval.minimum, spawnInterval.maximum);
                    float theta = Random.Range(0f, 2f * pi);
                    r = (float)Math.Sqrt(Random.Range(0f, 1f)) * radius;
                    x = r * (float)Math.Cos(theta);
                    y = r * (float)Math.Sin(theta);
                    float p = Random.Range(0f, 1f);
                    for (int i = PickUps.Length - 1; i >= 0; i--)
                    {
                        if (p >= ItemProbability[i])
                        {
                            toInstantiate = PickUps[i];
                            break;
                        }
                    }
                    colliderRadius = toInstantiate.transform.localScale.x * toInstantiate.GetComponent<CircleCollider2D>().radius;
                    instantiatePosition = boardCenter + new Vector3(x, y, 0);

                    Collider2D[] hits = Physics2D.OverlapCircleAll(instantiatePosition, colliderRadius);
                    isValidPosition = hits.Length == 1 && hits[0] == BackGroundCollider;
                }
                Instantiate(toInstantiate, instantiatePosition, Quaternion.identity);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //player off stage
        if (!isPaused && (other.tag == "Player"))
        {
            UIManager_MainGame.instance.visibleButtons(false);
            isGameOver = true;
            fallingPlayer = other.gameObject;
            isFallen = true;
            fallenPlayer = fallingPlayer;
            //focusCamera(fallingPlayer);
            PlayerController script = other.GetComponent<PlayerController>();
            script.PlayerStop();
            script.Opponent.GetComponent<PlayerController>().PlayerStop();

            script.triggerPlayerFall();
        }
    }

    public void JudgeGame()
    {
        fallingPlayer.SetActive(false);
        String winner = fallingPlayer.name == "Blue" ? "Red" : "Blue";
        int score = PlayerPrefs.GetInt(winner) + 1;

        if (score >= WIN_SCORE)
        {
            PlayerPrefs.SetInt("Blue", 0);
            PlayerPrefs.SetInt("Red", 0);
            PlayerPrefs.SetString("Winner", winner);
            //endWindow.winner = winner;
            SceneManager.LoadScene("WinMenu");
        }
        else {
            PlayerPrefs.SetInt(winner, score);
            SceneManager.LoadScene("MainGame");
        }
    }

    // public void startNewGame()
    // {
    //     SceneManager.LoadScene("Main");
    // }

    public void PauseGameAndShowMenu()
    {
        PauseGame();
        UIManager_MainGame.instance.showPauseWindow();
    }

    public void ResumeGameAndShowGame()
    {
        UIManager_MainGame.instance.showGame();
        ResumeGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        UIManager_MainGame.instance.enbleButtons(false);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        isPaused = false;
        UIManager_MainGame.instance.enbleButtons(true);
        Time.timeScale = 1;
    }


    public IEnumerator PauseGameForSeconds(float pauseTime)
    {
        PauseGame();
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + pauseTime)
        {
            yield return null;
        }
        ResumeGame();
        //if (!GenericWindow.newgame) 
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }

    public void OnMouseUpAsButton()
    {
        if (isPaused)
        {
            return;
        }
        PauseGameAndShowMenu();
    }
}
