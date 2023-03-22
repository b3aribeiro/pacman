using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ResetTrials : MonoBehaviour
{
    public string userInitial;
    public int trialNum;
    public string trialName;
    public List<string> trials;

    void Start()
    {
        trialNum = GlobalControl.Instance.trialNum;
        trialName = GlobalControl.Instance.trialName;
        trials = GlobalControl.Instance.trials;

        userInitial = GlobalControl.Instance.userInitial;
    }

    public void SaveGame()
    {
        GlobalControl.Instance.trialNum = trialNum;
        GlobalControl.Instance.trialName = trialName;
        GlobalControl.Instance.trials = trials;
        GlobalControl.Instance.userInitial = userInitial;
    }

}
