using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    public GameObject player;
    public Transform playerPosition;
    public Animator lockAnim;
    public Animator eyeAnim;

    public bool inFullScreen;

    public List<string> roundMovement = new List<string>();
    //--------------------------------------------------------
    // Game variables

    public static int Level = 0;
    public static int lives = 1;

	public enum GameState { Init, Game, Dead, Scores }
	public static GameState gameState;

    private GameObject pacman;
    private GameObject blinky;
    private GameObject pinky;
    private GameObject inky;
    private GameObject clyde;
    private GameGUINavigation gui;

	public static bool scared;
    static public int score;

	public float scareLength;
	private float _timeToCalm;

    public float SpeedPerLevel;

    //-------------------------------------------------------------------
    // analytics variables
    
    public string userInitial;
    public int trialNum;
    public string trialName;
    public List<string> trials;
    public int winningScore; //set this value in both scenes!

    private string sceneName;

    
    private int _playerEP; //energizer pellet collect
    private bool _isGameOver = false;
    
    //-------------------------------------------------------------------
    // singleton implementation
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    //-------------------------------------------------------------------
    // function definitions

    void Awake()
    {
        inFullScreen = true;

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if(this != _instance)   
                Destroy(this.gameObject);
        }

        AssignGhosts();
    }

	void Start () 
	{
		gameState = GameState.Init;

        trialNum = GlobalControl.Instance.trialNum;
        trials = GlobalControl.Instance.trials;
        userInitial = GlobalControl.Instance.userInitial;
        trialName = GlobalControl.Instance.trialName;
        
        InvokeRepeating("CatchPosition", 0f, 1);  //0 delay, repeat every 1s 0f, 0.25f
	}

    public void PlayerHasClearedLevel()
    {   
        _isGameOver = true;
        Time.timeScale = 0;
        Tinylytics.AnalyticsManager.LogCustomMetric(userInitial + "_LEVELCLEARED", "ROUND_" + trialNum + "0/6_TIMEINSECONDS_" + Time.time);
        ResetRound();
    }

    public void PlayerGetsEnergizer(Vector2 _PelletPos)
    {
        _playerEP++;

        if(_playerEP <= 3)
            {
                lockAnim = GameObject.Find("lock_" + _playerEP.ToString()).GetComponent<Animator>();
                lockAnim.SetTrigger("open");
                Debug.Log(trialName + lockAnim.name); 

                if (_playerEP == 3 && trialName == "harkness")
                {
                    eyeAnim = GameObject.Find("spooky").GetComponent<Animator>(); 
                    Debug.Log(trialName + eyeAnim.name); 
                    eyeAnim.SetTrigger("zoom");
                }
            }
        
        Tinylytics.AnalyticsManager.LogCustomMetric(userInitial + "_PELLETCOLLECTED", "PELLET_" + _playerEP.ToString()+ "/4_ATPOSITION_ " + _PelletPos + "_TIMEINSECONDS_" + Time.time);
    }

    public void GhostHitsPlayer(string _GhostName, Vector2 _GhostPos)
    {   
        _isGameOver = true;
        Time.timeScale = 0;
        Tinylytics.AnalyticsManager.LogCustomMetric(userInitial + "_HASDIED", "COLLIDEDWITH_" + _GhostName + "_ATPOSITION_ " + _GhostPos + "_TIMEINSECONDS_" + Time.time);
        Debug.Log( _playerEP + "/4 POWER PELLET" + _GhostPos);
        ResetRound();

    }

     public void CatchPosition() {
        float pX = playerPosition.transform.position.x;
        float pY = playerPosition.transform.position.z;
        roundMovement.Add("[" + pX.ToString() + "," + pY.ToString() + "]");
    }

    public void StopTracking()
    {
        string str = string.Join(", ", roundMovement);
        Tinylytics.AnalyticsManager.LogCustomMetric("TEST_PACMOVEMENT", str);
        Debug.Log(str);
    } 


    public void SaveGame()
    {
        GlobalControl.Instance.trialNum = trialNum;
        GlobalControl.Instance.trialName = trialName;
        GlobalControl.Instance.trials = trials;
        GlobalControl.Instance.userInitial = userInitial;
    }

    private void ResetRound()
    {
        if (_isGameOver == true)
        {
            _isGameOver = false;
            _playerEP = 0;
            StopTracking();
            newTrial();
            StartCoroutine(CheckSceneOrWait());
        }
    }

    private IEnumerator CheckSceneOrWait()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        
        if(currentScene.name != "interstitial") ResetScene();
        else 
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(CheckSceneOrWait());
        }
    }

    void newTrial()
    {
        //Tinylytics.AnalyticsManager.LogCustomMetric(userInitial + "_ROUNDSTARTED", trialNum.ToString() + "/6 at " + trialName.ToString() + System.DateTime.Now);
        
        if (trialNum < trials.Count)
        {
            trialNum = trialNum + 1;
            trialName = trials[trialNum];
            
            SaveGame();

            sceneName = "interstitial"; //this name is used in the Coroutine, which is basically just a pause timer for 3 seconds.

            StartCoroutine(WaitForSceneLoad());
        }
        else { endGame(); }
    }

    void endGame()
    {
        //if you want to know how lond the entire set of trials took, you can add your tinyLytics call here
        sceneName = "ending"; //this name is used in the Coroutine, which is basically just a pause timer for 3 seconds.
        Tinylytics.AnalyticsManager.LogCustomMetric(userInitial + "_TRIALENDED", "LOCALENDTIME_" + System.DateTime.Now);
        StartCoroutine(WaitForSceneLoad());

    }
      private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
    }

    void OnLevelWasLoaded()
    {
        if (Level == 0) lives = 1;

        Debug.Log("Level " + Level + " Loaded!");
        AssignGhosts();
        ResetVariables();

        // Adjust Ghost variables! SpeedPerLevel was 0.025
        clyde.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        blinky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        pinky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        inky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        pacman.GetComponent<PlayerController>().speed += Level*SpeedPerLevel/2;
    }

    private void ResetVariables()
    {
        _timeToCalm = 0.0f;
        scared = false;
        PlayerController.killstreak = 0;
    }

    // Update is called once per frame
	void Update () 
	{
        if (Screen.fullScreen == false) inFullScreen = false;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log(inFullScreen);
            Tinylytics.AnalyticsManager.LogCustomMetric("FullSreen Mode is " , inFullScreen.ToString());
            endGame();
        }

		if(scared && _timeToCalm <= Time.time)
			CalmGhosts();
	}

	public void ResetScene()
	{
        Time.timeScale = 1;
        
        CalmGhosts();

		pacman.transform.position = new Vector3(15f, 11f, 0f);
		blinky.transform.position = new Vector3(15f, 20f, 0f);
		pinky.transform.position = new Vector3(14.5f, 17f, 0f);
		inky.transform.position = new Vector3(16.5f, 17f, 0f);
		clyde.transform.position = new Vector3(12.5f, 17f, 0f);
        Debug.Log("Moving Ghost!");

		pacman.GetComponent<PlayerController>().ResetDestination();
		blinky.GetComponent<GhostMove>().InitializeGhost();
		pinky.GetComponent<GhostMove>().InitializeGhost();
		inky.GetComponent<GhostMove>().InitializeGhost();
		clyde.GetComponent<GhostMove>().InitializeGhost();

        player = GameObject.Find("pacman").GetComponent<GameObject>();
        playerPosition = GameObject.Find("pacman").GetComponent<Transform>();

        if(player == null) Debug.Log("Player Null on Reset!");
        if(playerPosition == null) Debug.Log("Player Null on Reset!");

        gameState = GameState.Init;

        gui.H_ShowReadyScreen();
        

	}

	public void ToggleScare()
	{
		if(!scared)	ScareGhosts();
		else 		CalmGhosts();
	}

	public void ScareGhosts()
	{
		scared = true;
		blinky.GetComponent<GhostMove>().Frighten();
		pinky.GetComponent<GhostMove>().Frighten();
		inky.GetComponent<GhostMove>().Frighten();
		clyde.GetComponent<GhostMove>().Frighten();
		_timeToCalm = Time.time + scareLength;

        Debug.Log("Ghosts Scared");
	}

	public void CalmGhosts()
	{
		scared = false;
		blinky.GetComponent<GhostMove>().Calm();
		pinky.GetComponent<GhostMove>().Calm();
		inky.GetComponent<GhostMove>().Calm();
		clyde.GetComponent<GhostMove>().Calm();
	    PlayerController.killstreak = 0;
    }

    void AssignGhosts()
    {
        // find and assign ghosts
        clyde = GameObject.Find("clyde");
        pinky = GameObject.Find("pinky");
        inky = GameObject.Find("inky");
        blinky = GameObject.Find("blinky");
        pacman = GameObject.Find("pacman");

        if (clyde == null || pinky == null || inky == null || blinky == null)
        {
            Debug.Log("One of ghosts are NULL");
            AssignGhosts();
        }
        if (pacman == null)
        {
            Debug.Log("Pacman is NULL");
            AssignGhosts();
        } 

        gui = GameObject.FindObjectOfType<GameGUINavigation>();

        if(gui == null) Debug.Log("GUI Handle Null!");
    }

    public void LoseLife()
    {
        lives--;
        gameState = GameState.Dead;
    
        // update UI too
        UIScript ui = GameObject.FindObjectOfType<UIScript>();
        Destroy(ui.lives[ui.lives.Count - 1]);
        ui.lives.RemoveAt(ui.lives.Count - 1);
        
        gui.H_ShowGameOverScreen();
    }

    public static void DestroySelf()
    {
        Debug.Log("DestroySelf()");
        score = 0;
        Level = 0;
        lives = 2;
        Destroy(GameObject.Find("Game Manager"));
    }

}
