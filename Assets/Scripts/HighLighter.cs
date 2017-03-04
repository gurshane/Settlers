using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HighLighter : NetworkBehaviour {

    private GameObject currentlyHighlighted;
    private GameObject gameManager;
    public bool firstTurn;
    public bool placedFirstSettlement;
    public bool placedFirstEdge;
    private List<Edge> validEdges;
    
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        firstTurn = true;
        placedFirstSettlement = false;
        placedFirstEdge = false;
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
                //CmdHighlightThis( impact.collider.gameObject);
                if (firstTurn)
                {
                    GameObject pieceHit = impact.collider.gameObject;
                    if (!placedFirstSettlement)
                    {
                        //Didnt hit a vertex, gtfo
                        if (!pieceHit.tag.Equals("Vertex"))
                        {
                            return;
                        }
                        //Have to place a settlement first
                        Vertex v = pieceHit.GetComponent<Vertex>();
                        //If not on land, gtfo
                        if((int)v.terrainType != (int)Enums.TerrainType.LAND)
                        {
                            return;
                        }
                        placedFirstSettlement = true;
                        //Keep track of valid positions to spawn the next edge
                        validEdges = pieceHit.GetComponent<Vertex>().neighbouringEdges;
                        GameObject newSettlement = Instantiate<GameObject>(GetComponent<PrefabHolder>().settlement, pieceHit.transform.position, Quaternion.identity);
                        

                        CmdSpawnSettlement(pieceHit.transform.position);

                    }
                    else if (!placedFirstEdge) //always have to do settlement then edge
                    {
                        //Didnt hit an edge, gtfo
                        if(!pieceHit.tag.Equals("Edge"))
                        {
                            return;
                        }
                        Edge e = pieceHit.GetComponent<Edge>();
                        bool validE = false;
                        //Has to place edge adjacent to 
                        foreach(Edge currentEdge in validEdges)
                        {
                            validE = pieceHit.name.Equals(currentEdge.gameObject.name);
                            if(validE)
                            {
                                break;
                            }
                        }
                        //Not adjacent to the v you placed, gtfo
                        if(!validE)
                        {
                            return;
                        }
                        placedFirstEdge = true;
                        if((int)e.terrainType == (int)Enums.TerrainType.LAND)
                        {
                            GameObject newRoad = Instantiate<GameObject>(GetComponent<PrefabHolder>().road, pieceHit.transform.position, pieceHit.transform.rotation);

                            CmdSpawnRoad(pieceHit.transform.position, pieceHit.transform.rotation, false);

                        }
                        else
                        {
                            GameObject newRoad = Instantiate<GameObject>(GetComponent<PrefabHolder>().boat, pieceHit.transform.position, pieceHit.transform.rotation);

                            CmdSpawnRoad(pieceHit.transform.position, pieceHit.transform.rotation, true);
                        }
                    }

                    //Turn over
                    if(placedFirstEdge && placedFirstSettlement)
                    {
                        firstTurn = false;
                    }


                }
            }
        }
    }

    [Command]
    void CmdSpawnSettlement(Vector3 v)
    {
        RpcSpawnSettlement(v);
    }

    [ClientRpc]
    void RpcSpawnSettlement(Vector3 v)
    {
        Instantiate<GameObject>(GetComponent<PrefabHolder>().settlement, v, Quaternion.identity);
    }

    [Command]
    void CmdSpawnRoad(Vector3 v, Quaternion q, bool isBoat)
    {
        RpcSpawnRoad(v, q, isBoat);
    }

    [ClientRpc]
    void RpcSpawnRoad(Vector3 v, Quaternion q, bool isBoat)
    {
        if(isBoat)
        {
            Instantiate<GameObject>(GetComponent<PrefabHolder>().boat, v, q);
        }
        else
        {
            Instantiate<GameObject>(GetComponent<PrefabHolder>().road, v, q);
        }
    }

    public void makeMaritimeTrade()
    {

    }

    //[ClientRpc]
    //public void RpcHighlightThis(GameObject target)
    //{
    //    if (target.tag == "Edge" || target.tag == "Vertex")
    //    {
    //        if (currentlyHighlighted != null)
    //        {
    //            currentlyHighlighted.GetComponent<MeshRenderer>().enabled = false;
    //        }
    //        target.GetComponent<MeshRenderer>().enabled = true;
    //        currentlyHighlighted = target;
    //    }
    //}

    //[Command]
    //public void CmdHighlightThis(GameObject target)
    //{
    //    RpcHighlightThis(target);
    //}
}
