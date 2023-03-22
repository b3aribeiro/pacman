﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{

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
        trialName = GlobalControl.Instance.trialName;
        trials = GlobalControl.Instance.trials;
        userInitial = GlobalControl.Instance.userInitial;
	}

    public void PlayerHasClearedLevel()
    {   
        _isGameOver = true;
        Tinylytics.AnalyticsManager.LogCustomMetric(userInitial + "_LEVELCLEARED", "ROUND_" + trialNum + "0/6_TIMEINSECONDS_" + Time.time);
        ResetRound();
    }

    public void PlayerGetsEnergizer(Vector2 _PelletPos)
    {
        _playerEP++;
        Tinylytics.AnalyticsManager.LogCustomMetric(userInitial + "_PELLETCOLLECTED", "PELLET_" + _playerEP.ToString()+ "/4_ATPOSITION_ " + _PelletPos + "_TIMEINSECONDS_" + Time.time);
        Debug.Log( _playerEP + "/4 POWER PELLET" + _PelletPos);
    }

    public void GhostHitsPlayer(string _GhostName, Vector2 _GhostPos)
    {   
        _isGameOver = true;
        Tinylytics.AnalyticsManager.LogCustomMetric(userInitial + "_HASDIED", "COLLIDEDWITH_" + _GhostName + "_ATPOSITION_ " + _GhostPos + "_TIMEINSECONDS_" + Time.time);
        Debug.Log( _playerEP + "/4 POWER PELLET" + _GhostPos);
        ResetRound();

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
        if (_playerEP == winningScore || _isGameOver == true)
        {
            _isGameOver = false;
            _playerEP = 0;
            trialNum = trialNum + 1;
            ResetScene();
            newTrial();
        }
    }

    void newTrial()
    {
        //Tinylytics.AnalyticsManager.LogCustomMetric(userInitial + "_ROUNDSTARTED", trialNum.ToString() + "/6 at " + trialName.ToString() + System.DateTime.Now);
        
        if (trialNum < trials.Count)
        {
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
        StartCoroutine(WaitForSceneLoad());

    }
      private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(sceneName);
    }

    void OnLevelWasLoaded()
    {
        if (Level == 0) lives = 1;

        Debug.Log("Level " + Level + " Loaded!");
        AssignGhosts();
        ResetVariables();

        // Adjust Ghost variables! SpeedPerLevel was 0.025
        //clyde.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        //blinky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        //pinky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        //inky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        //pacman.GetComponent<PlayerController>().speed += Level*SpeedPerLevel/2;
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
		if(scared && _timeToCalm <= Time.time)
			CalmGhosts();
	}

	public void ResetScene()
	{
        CalmGhosts();

		pacman.transform.position = new Vector3(15f, 11f, 0f);
		blinky.transform.position = new Vector3(15f, 20f, 0f);
		pinky.transform.position = new Vector3(14.5f, 17f, 0f);
		inky.transform.position = new Vector3(16.5f, 17f, 0f);
		clyde.transform.position = new Vector3(12.5f, 17f, 0f);

		pacman.GetComponent<PlayerController>().ResetDestination();
		blinky.GetComponent<GhostMove>().InitializeGhost();
		pinky.GetComponent<GhostMove>().InitializeGhost();
		inky.GetComponent<GhostMove>().InitializeGhost();
		clyde.GetComponent<GhostMove>().InitializeGhost();

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

        if (clyde == null || pinky == null || inky == null || blinky == null) Debug.Log("One of ghosts are NULL");
        if (pacman == null) Debug.Log("Pacman is NULL");

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
