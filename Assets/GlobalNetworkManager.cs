using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GlobalNetworkManager : NetworkBehaviour {

    [Command]
    public void CmdHighlightThis(GameObject target)
    {
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<HighLighter>().RpcHighlightThis(target);
        }
    }

}
