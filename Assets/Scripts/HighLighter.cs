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

    private bool doOnce;
    public bool secondTurn;
    public Enums.Color myColor;
    public List<Enums.Color> myColors;

    private PrefabHolder prefabHolder;
    private BoardState boardState;

    public int numPlayers;
    public int numPlayersReady;

    public Enums.TurnOrder currentTurn;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        firstTurn = true;
        placedFirstSettlement = false;
        placedFirstEdge = false;
        doOnce = true;
        secondTurn = false;
        prefabHolder = GetComponent<PrefabHolder>();
        myColors = new List<Enums.Color>();
        boardState = GetComponent<BoardState>();
        currentTurn = Enums.TurnOrder.FIRST;
        StartCoroutine(pickColor());
        numPlayers = 2;
        numPlayersReady = 0;
       
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
                    if ((int)currentTurn != (int)myColor)
                    {
                        Debug.Log((int)currentTurn + " " + (int)myColor);
                        Debug.Log("not my turn");
                        return;
                    }
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
                        if(v.getOccupyingPiece() != null)
                        {
                            return;
                        }
                        if(!v.isOnMainland)
                        {
                            return;
                        }
                        //If not on land, gtfo
                        if((int)v.terrainType != (int)Enums.TerrainType.LAND)
                        {
                            return;
                        }
                        placedFirstSettlement = true;
                        //Keep track of valid positions to spawn the next edge
                        validEdges = pieceHit.GetComponent<Vertex>().neighbouringEdges;
                        //GameObject newSettlement = Instantiate<GameObject>(GetComponent<PrefabHolder>().settlement, pieceHit.transform.position, Quaternion.identity);
                        

                        CmdSpawnSettlement(pieceHit.transform.position, pieceHit.transform.rotation, (int) myColor);

                    }
                    else if (!placedFirstEdge) //always have to do settlement then edge
                    {
                        //Didnt hit an edge, gtfo
                        if(!pieceHit.tag.Equals("Edge"))
                        {
                            return;
                        }
                        Edge e = pieceHit.GetComponent<Edge>();
                        if(e.getOccupyingPiece() != null)
                        {
                            return;
                        }
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
                            //GameObject newRoad = Instantiate<GameObject>(GetComponent<PrefabHolder>().road, pieceHit.transform.position, pieceHit.transform.rotation);

                            CmdSpawnRoad(pieceHit.transform.position, pieceHit.transform.rotation.eulerAngles.y, false, (int) myColor);

                        }
                        else
                        {
                            //GameObject newRoad = Instantiate<GameObject>(GetComponent<PrefabHolder>().boat, pieceHit.transform.position, pieceHit.transform.rotation);

                            CmdSpawnRoad(pieceHit.transform.position, pieceHit.transform.rotation.eulerAngles.y, true, (int) myColor);
                        }
                    }

                    //Turn over
                    if(placedFirstEdge && placedFirstSettlement)
                    {
                        firstTurn = false;
                        Debug.Log("done");
                        CmdPlayerDoneFirstTurn(((int)currentTurn) + 1);
                    }


                }
                else if (secondTurn)
                {
                    //let people build cities or more settlements
                    //let people roll the die
                }

                if ((numPlayersReady == numPlayers) && !secondTurn)
                {
                    secondTurn = true;
                }

                
            }
        }
    }

    [Command]
    void CmdPlayerDoneFirstTurn(int turn)
    {
        RpcPlayerDoneFirstTurn(turn);
    }

    [ClientRpc]
    void RpcPlayerDoneFirstTurn(int turn)
    {
        currentTurn = (Enums.TurnOrder)turn;

    }

    [Command]
    void CmdSpawnSettlement(Vector3 v, Quaternion q, int mymat)
    {
        RpcSpawnSettlement(v, q, mymat);
    }

    [ClientRpc]
    void RpcSpawnSettlement(Vector3 v, Quaternion q, int mymat)
    {
        GameObject go = Instantiate<GameObject>(GetComponent<PrefabHolder>().settlement, v, q);
        go.transform.Rotate(new Vector3(-90.0f, 0f, 0f));
        go.transform.Translate(0f, 0f, 10f);
        go.GetComponent<MeshRenderer>().material = prefabHolder.materials[mymat];

        Vertex source = boardState.vertexPosition[v];
        // Put a settlement on the board
        Settlement settlement = new Settlement(myColor);
        source.setOccupyingPiece(settlement);
        settlement.putOnBoard();
        boardState.vertexPosition.Remove(v);
        boardState.vertexPosition.Add(v, source);
    }

    [Command]
    void CmdSpawnRoad(Vector3 v, float q, bool isBoat, int mymat)
    {
        RpcSpawnRoad(v, q, isBoat, mymat);
    }

    [ClientRpc]
    void RpcSpawnRoad(Vector3 v, float q, bool isBoat, int mymat)
    {
        GameObject go;
        if(isBoat)
        {
            go = Instantiate<GameObject>(GetComponent<PrefabHolder>().boat, v, Quaternion.identity);
        }
        else
        {
            go = Instantiate<GameObject>(GetComponent<PrefabHolder>().road, v, Quaternion.identity);
        }

        go.transform.Rotate(new Vector3(-90f, 0f, 0f));
        go.transform.Translate(0f, 0f, 5f);
        go.GetComponent<MeshRenderer>().material = prefabHolder.materials[mymat];

        Edge source = boardState.edgePosition[v];
        // Put a road on the board
        Road road = new Road(myColor, isBoat);
        source.setOccupyingPiece(road);
        road.putOnBoard();

        boardState.edgePosition.Remove(v);
        boardState.edgePosition.Add(v, source);
    }

    public void makeMaritimeTrade()
    {

    }

    IEnumerator pickColor()
    {
        yield return new WaitForSeconds(Random.Range(3.0f, 5.0f)+Random.Range(0.1f, 0.3f));
        if (!myColors.Contains(Enums.Color.WHITE))
        {
            Debug.Log("I'm white");
            myColor = Enums.Color.WHITE;
            CmdUpdateColorList((int)Enums.Color.WHITE);
        }
        else if (!myColors.Contains(Enums.Color.ORANGE))
        {
            Debug.Log("I'm orange");
            myColor = Enums.Color.ORANGE;
            CmdUpdateColorList((int)Enums.Color.ORANGE);
        }
        else if (!myColors.Contains(Enums.Color.RED))
        {
            Debug.Log("I'm red");
            myColor = Enums.Color.RED;
            CmdUpdateColorList((int)Enums.Color.RED);
        }
        else if (!myColors.Contains(Enums.Color.BLUE))
        {
            Debug.Log("I'm blue");
            myColor = Enums.Color.BLUE;
            CmdUpdateColorList((int)Enums.Color.BLUE);
        }
    }


    [Command]
    void CmdUpdateColorList(int color)
    {
        RpcUpdateColorList(color);
    }

    [ClientRpc]
    void RpcUpdateColorList(int color)
    {
        myColors.Add((Enums.Color)color);
    }

    public void tradeMaritimeWool()
    {

    }

    public void tradeMaritimeLumber()
    {

    }

    public void tradeMaritimeOre()
    {
       
    }

    public void tradeMaritimeBrikc()
    {

    }

    public void tradeMaritimeGrain()
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
