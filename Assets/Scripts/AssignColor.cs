using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssignColor : NetworkBehaviour {

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player  connected from " + player.ipAddress + ":" + player.port);
    }
}
