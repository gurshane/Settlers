using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssignColor : NetworkBehaviour {

    void Update()
    {
        if(isServer && isLocalPlayer)
        {
            //GetComponent<GameManager>().UpdateColor();
        }
    }

}
