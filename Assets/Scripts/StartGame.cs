using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartGame : MonoBehaviour
{

    public int trialNum;
    public string trialName;
    public List<string> trials = new();
    public int winningScore;

    private void Start()
    {

        //shuffle the list when the game starts
        for (int i = 0; i < trials.Count; i++)
        {
            string temp = trials[i];
            int randomIndex = Random.Range(i, trials.Count);
            trials[i] = trials[randomIndex];
            trials[randomIndex] = temp;
        }

        GlobalControl.Instance.trials = trials; //set a global list of trials we can use in all of the scenes


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            gameStart(); 
        }
    }

    void gameStart()
    {
        trialNum = 0; //set the trial number to the first item in the list
        GlobalControl.Instance.trialNum = trialNum; //this will set the trialNum to the first in the list
        Tinylytics.AnalyticsManager.LogCustomMetric("Game Start", "Start " + System.DateTime.Now);
        trialName = GlobalControl.Instance.trials[trialNum];
        SceneManager.LoadScene(trialName);
    }
}
