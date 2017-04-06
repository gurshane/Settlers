using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TradeManager : NetworkBehaviour {

    public GameObject tradePrefab; //set to a Trades object
    List<Trades> active;

	// Use this for initialization
	void Start () {
        active = new List<Trades>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    [Command]//call from Player on client to spawn a trade on all machines 
    public void CmdSpawnTrade(int [] resourcesOffered, int [] resourcesDemanded, int [] commoditiesOffered, int [] commoditiesDemanded, int goldOffered, int goldDemanded, int playerId)
    {
        Player[] players = GameObject.FindObjectsOfType<Player>();
        Player offering = null;
        foreach (Player player in players)
        {
            if (player.iD == playerId)
            {
                offering = player;
                break;
            }
        }
        GameObject trade = (GameObject) GameObject.Instantiate(tradePrefab);
        trade.GetComponent<Trades>().init(resourcesOffered, resourcesDemanded, commoditiesOffered, commoditiesDemanded, goldOffered, goldDemanded, offering);
        NetworkServer.Spawn(trade);
    }


}
