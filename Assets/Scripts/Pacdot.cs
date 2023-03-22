using UnityEngine;
using System.Collections;

public class Pacdot : MonoBehaviour {

	private GameManager gm;

	// Use this for initialization
	void Start ()
	{
	    gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if( gm == null )    Debug.Log("Energizer did not find Game Manager!");
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name == "pacman")
		{
			GameManager.score += 10;
		    GameObject[] pacdots = GameObject.FindGameObjectsWithTag("pacdot");
            Destroy(gameObject);

		    if (pacdots.Length == 1)
		    {
				gm.PlayerHasClearedLevel();
		        //GameObject.FindObjectOfType<GameGUINavigation>().LoadLevel();
		    }
		}
	}
}
