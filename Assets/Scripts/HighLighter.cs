using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HighLighter : NetworkBehaviour {

    private GameObject currentlyHighlighted;
    private GameObject gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        
    }

    // Update is called once per frame
    void Update() {
        if(!isLocalPlayer)
        {
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit impact;
            if (Physics.Raycast(ray, out impact))
            {
                gameManager.GetComponent<GlobalNetworkManager>().CmdHighlightThis( impact.collider.gameObject);
            }
        }
    }

    [ClientRpc]
    public void RpcHighlightThis(GameObject target)
    {
        if (target.tag == "Edge" || target.tag == "Vertex")
        {
            if (currentlyHighlighted != null)
            {
                currentlyHighlighted.GetComponent<MeshRenderer>().enabled = false;
            }
            target.GetComponent<MeshRenderer>().enabled = true;
            currentlyHighlighted = target;
        }
    }
}
