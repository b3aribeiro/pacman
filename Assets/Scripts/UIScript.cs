﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {

	public int high, score;

	public List<Image> lives = new List<Image>(3);

	Text txt_score, txt_high, txt_level;
	
	// Use this for initialization
	void Start () 
	{
        txt_level = GetComponentsInChildren<Text>()[0];
		txt_score = GetComponentsInChildren<Text>()[1];
		//txt_high = GetComponentsInChildren<Text>()[0];

	    for (int i = 0; i < 3 - GameManager.lives; i++)
	    {
	        Destroy(lives[lives.Count-1]);
            lives.RemoveAt(lives.Count-1);
	    }
	}
	
	// Update is called once per frame
	void Update () 
	{

        high = GameObject.Find("Game Manager").GetComponent<ScoreManager>().High();

        // update score text
        score = GameManager.score;
	    txt_level.text = "Trial\n" + (GlobalControl.Instance.trialNum + 1);
		txt_score.text = "Score\n" + score;
		//txt_high.text = "High Score\n" + high;

	}


}
