using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class SavedStates : NetworkBehaviour {
    

	// Update is called once per frame
	void Update ()
    {
		if(!isLocalPlayer)
        {
            return;
        }

        //give a lot of resources
        //barbarian
        //winning
        Debug.Log("bruh");
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Years of Plenty");
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Progress Card Saved Game");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Metropolis Aqueduct, MarketPlace, Fortress");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Knight Saved Game");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Barbarian Saved Game");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Winning Saved Game");
        }
    }

    void yearsOfPlenty()
    {

    }

    void progressCardSavedGame()
    {

    }

    void devChartSavedGame()
    {

    }

    void knightSavedGame()
    {

    }

    void barbarianSavedGame()
    {

    }

    void winningSavedGame()
    {

    }
}
