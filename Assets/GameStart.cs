using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameStart : NetworkBehaviour {

    public GameObject boardGenerator;
    
	// Use this for initialization
	void Start ()
    {
        if (this.isServer)
        {
            Instantiate<GameObject>(boardGenerator);
            //Create the players
            //Update their guis
            //Start the first turn
        }
    }

}
