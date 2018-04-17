using System.Collections;
using System.Collections.Generic;                       //Allows us to use Lists. 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = .1f;
    public static GameManager instance = null;                          //Static instance of GameManager which allows it to be accessed by any other script.
    public BoardManager boardScript;                            //Store a reference to our BoardManager which will set up the level.
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;                          //Current level number, expressed in game as "Day 1".
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    void Awake()                            //Awake is always called before any Start functions
    {
        if (instance == null)                           //Check if instance already exists
        {
            instance = this;                            //if not, set instance to this
        }
        else if (instance != this)                           //If instance already exists and it's not this:
        {
            Destroy(gameObject);                            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
        }
        DontDestroyOnLoad(gameObject);                          //Sets this to not be destroyed when reloading scene
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();                         //Get a component reference to the attached BoardManager script
        InitGame();                         //Call the InitGame function to initialize the first level 
    }

    //This is called each time a scene is loaded.
    void OnLevelWasLoaded(int index)
    {
        //Add one to our level number.
        level++;
        //Call InitGame to initialize our level.
        InitGame();
    }

    //Initializes the game for each level.
    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);
        enemies.Clear();
        boardScript.SetupScene(level);                          //Call the SetupScene function of the BoardManager script, pass it current level number.
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }


    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved and died.";
        levelImage.SetActive(true);
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
