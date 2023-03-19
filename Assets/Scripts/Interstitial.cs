using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class Interstitial : MonoBehaviour
{
    public int trialNum;
    public string trialName;
    public List<string> trials;

    public TextMeshProUGUI message;

    void Start()
    {
        trialNum = GlobalControl.Instance.trialNum;
        trialName = GlobalControl.Instance.trialName;
        trials = GlobalControl.Instance.trials;

        MessagePlayer();
    }

    public void SaveGame()
    {
        GlobalControl.Instance.trialNum = trialNum;
        GlobalControl.Instance.trialName = trialName;
        GlobalControl.Instance.trials = trials;
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            newTrial();
        }

    }

    void MessagePlayer()
    {
        var _trialNumberForHumans = trialNum + 1; //lol this is because trialNum starts at 0 (as do all array values) but people don't start counting with zero...mostly...
        message.text = "trial " + _trialNumberForHumans + "\n" +"Press Space to play again";
    }

    void newTrial()
    {

        //Tinylytics.AnalyticsManager.LogCustomMetric("New Trial Number: ", trialNum.ToString());
        //Tinylytics.AnalyticsManager.LogCustomMetric("New Trial Name: ", trialName);

        SceneManager.LoadScene(trialName);

    }
}
