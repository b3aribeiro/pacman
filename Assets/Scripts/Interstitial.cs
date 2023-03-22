using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class Interstitial : MonoBehaviour
{
    public string userInitial;
    public int trialNum;
    public string trialName;
    public List<string> trials;

    public TextMeshProUGUI message;

    void Start()
    {
        trialNum = GlobalControl.Instance.trialNum;
        trialName = GlobalControl.Instance.trialName;
        trials = GlobalControl.Instance.trials;
        userInitial = GlobalControl.Instance.userInitial;

        MessagePlayer();
    }

    public void SaveGame()
    {
        GlobalControl.Instance.trialNum = trialNum;
        GlobalControl.Instance.trialName = trialName;
        GlobalControl.Instance.trials = trials;
        GlobalControl.Instance.userInitial = userInitial;
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SceneManager.LoadScene(trialName);
        }

    }

    void MessagePlayer()
    {
        var _trialNumberForHumans = trialNum + 1; 
        message.text = "trial " + _trialNumberForHumans + "\n" +"Press Space to play again";
    }
}
