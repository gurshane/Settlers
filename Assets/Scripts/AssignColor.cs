using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssignColor : MonoBehaviour {

    void OnConnectedToServer()
    {
        Debug.Log("Yo");
        GetComponent<GameManager>().UpdateColor();
        //CmdUpdatePlayerNameList(Network.player.ipAddress);
    }

}
