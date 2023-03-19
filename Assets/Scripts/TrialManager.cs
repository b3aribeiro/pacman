using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrialManager : MonoBehaviour
{
   //public Ball ball;

    public Text playerScoreText;
    public Text computerScoreText;

    private int _playerScore;
    private int _computerScore;

    public int trialNum;
    public string trialName;
    public List<string> trials;
    public int winningScore; //set this value in both scenes!

    private string sceneName;

    void Start()
    {
        trialNum = GlobalControl.Instance.trialNum;
        trialName = GlobalControl.Instance.trialName;
        trials = GlobalControl.Instance.trials;
    }

    public void SaveGame()
    {
        GlobalControl.Instance.trialNum = trialNum;
        GlobalControl.Instance.trialName = trialName;
        GlobalControl.Instance.trials = trials;

    }


    public void PlayerScores()
    {
        //Tinylytics.AnalyticsManager.LogCustomMetric("Player Score", _playerScore.ToString());
        _playerScore++;
        this.playerScoreText.text = _playerScore.ToString();
        ResetRound();
        
    }

    public void ComputerScores()
    {
        //Tinylytics.AnalyticsManager.LogCustomMetric("Computer Score", _computerScore.ToString());
        _computerScore++;
        this.computerScoreText.text = _computerScore.ToString();
        ResetRound();

    }


    private void ResetRound()
    {
        if (_playerScore == winningScore || _computerScore == winningScore)
        {
            trialNum = trialNum + 1;
            SaveGame();
            newTrial();
            //this.ball.ResetPosition(); //ball should stop moving once game is over
        }
        else
        {
            //this.ball.ResetPosition();
            //StartCoroutine(pauseBall()); //waits .5 sec to serve the ball
        }

        
    }

    void newTrial()
    {
        Tinylytics.AnalyticsManager.LogCustomMetric("TrialNumber_" + trialNum.ToString() + "/10" + " TrialName_" + trialName.ToString(), "PS_" + _playerScore.ToString() + "PCS_" + _computerScore.ToString());
        
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
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator pauseBall()
    {
        yield return new WaitForSeconds(0.5f);
        //this.ball.AddStartingForce();

    }
}
