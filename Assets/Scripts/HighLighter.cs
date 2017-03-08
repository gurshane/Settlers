using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class HighLighter : NetworkBehaviour {

    private GameObject currentlyHighlighted;
    private GameObject gameManager;
    public bool firstTurn;
    public bool placedFirstSettlement;
    public bool placedFirstEdge;
    private List<Edge> validEdges;
    private List<Vertex> validVertexes;
    private bool doOnce;
    public bool secondTurn;
    public Enums.Color myColor;
    public List<Enums.Color> myColors;
    public List<int> myDieNumbers;

    private PrefabHolder prefabHolder;
    private BoardState boardState;
    private Player p;

    public int numPlayers;
    public int numPlayersReady;

    public int firstDieNum;
    public int secondDieNum;
    public int resourceDieNum;

    public turnOrder currentTurn;

    public bool placedFirstCity;
    public bool placedSecondEdge;


    public int red;
    public int green;
    public int blue;
    public bool iAmTheSpawner;
    void Start()
    {
        placedFirstCity = false;
        placedSecondEdge = false;
        iAmTheSpawner = false;
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        firstTurn = true;
        placedFirstSettlement = false;
        placedFirstEdge = false;
        doOnce = true;
        secondTurn = false;
        validEdges = new List<Edge>();
        validVertexes = new List<Vertex>();
        prefabHolder = GetComponent<PrefabHolder>();
        myColors = new List<Enums.Color>();
        myDieNumbers = new List<int>();
        boardState = GetComponent<BoardState>();
        p = GetComponent<Player>();
        numPlayers = 2;
        numPlayersReady = 0;
        currentTurn = GetComponent<turnOrder>();

        red = UnityEngine.Random.Range(0, 255);
        green = UnityEngine.Random.Range(0, 255);
        blue = UnityEngine.Random.Range(0, 255);


        StartCoroutine(pickColor());
    }

    // Update is called once per frame
    void Update() {
        
        if(!isLocalPlayer)
        {
            return;
        }
        

        if(myDieNumbers.Count >= 3)
        {
            firstDieNum = myDieNumbers[0];
            secondDieNum = myDieNumbers[1];
            resourceDieNum = myDieNumbers[2];
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit impact;
            if (Physics.Raycast(ray, out impact))
            {
                //if ((int)GetComponent<turnOrder>().currentTurn != (int)myColor)
                //{
                //    Debug.Log("bollocks");
                //    return;
                //}
                GameObject pieceHit = impact.collider.gameObject;
                //CmdHighlightThis( impact.collider.gameObject);
                if (firstTurn)
                {
                    //if ((int)currentTurn != (int)myColor)
                    //{
                    //    Debug.Log((int)currentTurn + " " + (int)myColor);
                    //    Debug.Log("not my turn");
                    //    return;
                    //}
                    if (!placedFirstSettlement)
                    {
                        //Didnt hit a vertex, gtfo
                        if (!pieceHit.tag.Equals("Vertex"))
                        {
                            return;
                        }
                        //Have to place a settlement first
                        Vertex v = pieceHit.GetComponent<Vertex>();
                        if (v.getOccupyingPiece() != null)
                        {
                            return;
                        }
                        if (!v.isOnMainland)
                        {
                            return;
                        }
                        //If not on land, gtfo
                        if ((int)v.terrainType != (int)Enums.TerrainType.LAND)
                        {
                            return;
                        }
                        placedFirstSettlement = true;
                        //Keep track of valid positions to spawn the next edge
                        validEdges = pieceHit.GetComponent<Vertex>().neighbouringEdges;
                        //GameObject newSettlement = Instantiate<GameObject>(GetComponent<PrefabHolder>().settlement, pieceHit.transform.position, Quaternion.identity);
                        //newSettlement.transform.localScale = newSettlement.transform.localScale * 1.5f;
                        //newSettlement.transform.Rotate(new Vector3(-90f, 0f, 0f));
                        //newSettlement.transform.Translate(0f, 0f, 10f);
                        //iAmTheSpawner = true;
                        p.incrementVictoryPoints(1);
                        CmdSpawnSettlement(pieceHit.transform.position, pieceHit.transform.rotation, (int)myColor, red, green, blue);

                    }
                    else if (!placedFirstEdge) //always have to do settlement then edge
                    {
                        //Didnt hit an edge, gtfo
                        if (!pieceHit.tag.Equals("Edge"))
                        {
                            return;
                        }
                        Edge e = pieceHit.GetComponent<Edge>();
                        if (e.getOccupyingPiece() != null)
                        {
                            return;
                        }
                        bool validE = false;
                        //Has to place edge adjacent to 
                        foreach (Edge currentEdge in validEdges)
                        {
                            validE = pieceHit.name.Equals(currentEdge.gameObject.name);
                            if (validE)
                            {
                                break;
                            }
                        }
                        //Not adjacent to the v you placed, gtfo
                        if (!validE)
                        {
                            return;
                        }
                        placedFirstEdge = true;
                        validVertexes.Add(e.getLeftVertex());
                        validVertexes.Add(e.getRightVertex());
                        if ((int)e.terrainType == (int)Enums.TerrainType.LAND)
                        {
                            //GameObject newRoad = Instantiate<GameObject>(GetComponent<PrefabHolder>().road, pieceHit.transform.position, pieceHit.transform.rotation);
                            //newRoad.transform.localScale = newRoad.transform.localScale * 1.5f;
                            //iAmTheSpawner = true;
                            //newRoad.transform.Rotate(new Vector3(-90f, 0f, 0f));
                            //newRoad.transform.Translate(0f, 0f, 5f);
                            CmdSpawnRoad(pieceHit.transform.position, pieceHit.transform.rotation.eulerAngles.y, false, (int)myColor, red, green, blue);

                        }
                        else
                        {
                            //GameObject newRoad = Instantiate<GameObject>(GetComponent<PrefabHolder>().boat, pieceHit.transform.position, pieceHit.transform.rotation);
                            //newRoad.transform.localScale = newRoad.transform.localScale*1.5f;
                            //iAmTheSpawner = true;
                            //newRoad.transform.Rotate(new Vector3(-90f, 0f, 0f));
                            //newRoad.transform.Translate(0f, 0f, 5f);
                            CmdSpawnRoad(pieceHit.transform.position, pieceHit.transform.rotation.eulerAngles.y, true, (int)myColor, red, green, blue);
                        }
                    }
                    else if (!placedFirstCity)
                    {
                        if (!pieceHit.tag.Equals("Vertex"))
                        {
                            return;
                        }
                        //Have to place a settlement first
                        Vertex v = pieceHit.GetComponent<Vertex>();
                        if (v.getOccupyingPiece() != null)
                        {
                            return;
                        }
                        if (!v.isOnMainland)
                        {
                            return;
                        }
                        //If not on land, gtfo
                        if ((int)v.terrainType != (int)Enums.TerrainType.LAND)
                        {
                            return;
                        }
                        placedFirstCity = true;
                        //Keep track of valid positions to spawn the next edge
                        foreach(Edge e in pieceHit.GetComponent<Vertex>().neighbouringEdges)
                        {
                            validEdges.Add(e);
                        }
                        //GameObject newSettlement = Instantiate<GameObject>(GetComponent<PrefabHolder>().settlement, pieceHit.transform.position, Quaternion.identity);
                        //newSettlement.transform.localScale = newSettlement.transform.localScale * 1.5f;
                        //newSettlement.transform.Rotate(new Vector3(-90f, 0f, 0f));
                        //newSettlement.transform.Translate(0f, 0f, 10f);
                        //iAmTheSpawner = true;
                        p.incrementVictoryPoints(2);
                        CmdSpawnCity(pieceHit.transform.position, pieceHit.transform.rotation, (int)myColor, red, green, blue);

                        foreach(Hex h in boardState.hexPoisition.Values)
                        {
                            if (h.gameObject.tag.Equals("MainHex"))
                            {
                                if(h.adjacentToVertex(v))
                                {
                                    p.addResource(h.resourceType, 1);
                                }
                            }
                        }
                    }
                    else if (!placedSecondEdge)
                    {
                        //Didnt hit an edge, gtfo
                        if (!pieceHit.tag.Equals("Edge"))
                        {
                            return;
                        }
                        Edge e = pieceHit.GetComponent<Edge>();
                        if (e.getOccupyingPiece() != null)
                        {
                            return;
                        }
                        bool validE = false;
                        //Has to place edge adjacent to 
                        foreach (Edge currentEdge in validEdges)
                        {
                            validE = pieceHit.name.Equals(currentEdge.gameObject.name);
                            if (validE)
                            {
                                break;
                            }
                        }
                        //Not adjacent to the v you placed, gtfo
                        if (!validE)
                        {
                            return;
                        }
                        placedSecondEdge = true;
                        validVertexes.Add(e.getLeftVertex());
                        validVertexes.Add(e.getRightVertex());
                        if ((int)e.terrainType == (int)Enums.TerrainType.LAND)
                        {
                            //GameObject newRoad = Instantiate<GameObject>(GetComponent<PrefabHolder>().road, pieceHit.transform.position, pieceHit.transform.rotation);
                            //newRoad.transform.localScale = newRoad.transform.localScale * 1.5f;
                            //iAmTheSpawner = true;
                            //newRoad.transform.Rotate(new Vector3(-90f, 0f, 0f));
                            //newRoad.transform.Translate(0f, 0f, 5f);
                            CmdSpawnRoad(pieceHit.transform.position, pieceHit.transform.rotation.eulerAngles.y, false, (int)myColor, red, green, blue);

                        }
                        else
                        {
                            //GameObject newRoad = Instantiate<GameObject>(GetComponent<PrefabHolder>().boat, pieceHit.transform.position, pieceHit.transform.rotation);
                            //newRoad.transform.localScale = newRoad.transform.localScale*1.5f;
                            //iAmTheSpawner = true;
                            //newRoad.transform.Rotate(new Vector3(-90f, 0f, 0f));
                            //newRoad.transform.Translate(0f, 0f, 5f);
                            CmdSpawnRoad(pieceHit.transform.position, pieceHit.transform.rotation.eulerAngles.y, true, (int)myColor, red, green, blue);
                        }
                    }
                    //Turn over
                    if(placedFirstEdge && placedFirstSettlement && placedSecondEdge && placedFirstCity)
                    {
                        firstTurn = false;
                        secondTurn = true; //Remove this eventuallys
                        CmdPlayerDoneFirstTurn();
                    }


                }
                else if (secondTurn)
                {
                    //let people build cities or more settlements
                    //let people roll the die
                    if(pieceHit.tag.Equals("Vertex"))
                    {
                        Vertex v = pieceHit.GetComponent<Vertex>();
                        GamePiece g = v.getOccupyingPiece();
                        if(!validPlaceForVertex(pieceHit))
                        {
                            return;
                        }
                        if(g == null)
                        {
                            foreach(Edge e in v.getNeighbouringEdges())
                            {
                                validEdges.Add(e);
                            }
                            iAmTheSpawner = true;
                            makeSettlmentHere(v);
                            return;
                        }

                        ////Same colour as me
                        //if ((int)g.getColor() != (int)myColor)
                        //{
                        //    return;
                        //}

                        try
                        {
                            City c = (City) v.getOccupyingPiece();
                        }
                        catch(Exception e)
                        {
                            foreach (Edge ed in v.getNeighbouringEdges())
                            {
                                validEdges.Add(ed);
                            }
                            iAmTheSpawner = true;
                            makeCityHere(v);
                        }
                    }
                    else if(pieceHit.tag.Equals("Edge"))
                    {
                        Edge e = pieceHit.GetComponent<Edge>();
                        GamePiece g = e.getOccupyingPiece();

                        if (!validPlaceForEdge(pieceHit))
                        {
                            return;
                        }

                        if (g == null)
                        {
                            validVertexes.Add(e.getRightVertex());
                            validVertexes.Add(e.getLeftVertex());
                            iAmTheSpawner = true;
                            makeRoadHere(e);
                        }
                    }
                }

                //if ((numPlayersReady == numPlayers) && !secondTurn)
                //{
                //    secondTurn = true;
                //}

                
            }
        }
    }

    bool validPlaceForVertex(GameObject pieceHit)
    {
        bool validV = false;

        foreach(Vertex v in validVertexes)
        {
            if(v == null)
            {
                continue;
            }
            validV = pieceHit.name.Equals(v.gameObject.name);
            if(validV)
            {
                break;
            }
        }

        return validV;
    }

    bool validPlaceForEdge(GameObject pieceHit)
    {
        bool validE = false;
        //Has to place edge adjacent to some 
        foreach (Edge currentEdge in validEdges)
        {
            if(currentEdge == null)
            {
                continue;
            }
            validE = pieceHit.name.Equals(currentEdge.gameObject.name);
            if (validE)
            {
                break;
            }
        }

        return validE;
    }
    
    void makeRoadHere(Edge e)
    {
        if ((p.getResources()[(int)Enums.ResourceType.BRICK] >= 1) && (p.getResources()[(int)Enums.ResourceType.LUMBER] >= 1))
        {
            p.discardResource(Enums.ResourceType.BRICK, 1);
            p.discardResource(Enums.ResourceType.LUMBER, 1);
            if((int)e.terrainType == (int)Enums.TerrainType.LAND)
            {
                //GameObject go = Instantiate<GameObject>(GetComponent<PrefabHolder>().road, e.gameObject.transform.position, e.gameObject.transform.rotation);
                //go.transform.localScale = 1.5f * go.transform.localScale;
                //go.transform.Rotate(new Vector3(-90f, 0f, 0f));
                //go.transform.Translate(0f, 0f, 5f);
                CmdSpawnRoad(e.gameObject.transform.position, e.gameObject.transform.rotation.eulerAngles.y, false, (int)myColor, red, green, blue);
            }
            else
            {
                //GameObject go = Instantiate<GameObject>(GetComponent<PrefabHolder>().boat, e.gameObject.transform.position, e.gameObject.transform.rotation);
                //go.transform.localScale = go.transform.localScale * 1.5f;
                ////go.transform.Rotate(new Vector3(-90f, 0f, 0f));
                //go.transform.Translate(0f, 0f, 5f);
                CmdSpawnRoad(e.gameObject.transform.position, e.gameObject.transform.rotation.eulerAngles.y, true, (int)myColor, red, green, blue);
            }
        }
    }

    void makeSettlmentHere(Vertex v)
    {
        if((p.getResources()[(int)Enums.ResourceType.BRICK] >= 1) && (p.getResources()[(int)Enums.ResourceType.GRAIN] >= 1) 
            && (p.getResources()[(int)Enums.ResourceType.LUMBER] >= 1) && (p.getResources()[(int)Enums.ResourceType.ORE] >= 1))
        {
            p.discardResource(Enums.ResourceType.BRICK, 1);
            p.discardResource(Enums.ResourceType.GRAIN, 1);
            p.discardResource(Enums.ResourceType.LUMBER, 1);
            p.discardResource(Enums.ResourceType.ORE, 1);
            p.incrementVictoryPoints(1);

            //GameObject go = Instantiate<GameObject>(GetComponent<PrefabHolder>().settlement, v.gameObject.transform.position, v.gameObject.transform.rotation);
            //go.transform.localScale = go.transform.localScale * 1.5f;
            //go.transform.Rotate(new Vector3(-90.0f, 0f, 0f));
            //go.transform.Translate(0f, 0f, 10f);
            CmdSpawnSettlement(v.gameObject.transform.position, v.gameObject.transform.rotation, (int)myColor, red, green, blue);
        }
    }

    void makeCityHere(Vertex v)
    {
        if ((p.getResources()[(int)Enums.ResourceType.GRAIN] >= 2) && (p.getResources()[(int)Enums.ResourceType.ORE] >= 3))
        {
            p.discardResource(Enums.ResourceType.GRAIN, 2);
            p.discardResource(Enums.ResourceType.ORE, 3);
            p.incrementVictoryPoints(1);//because you lose one point for removing the settlement and gain two for the new city
            //GameObject go = Instantiate<GameObject>(GetComponent<PrefabHolder>().city, v.gameObject.transform.position, v.gameObject.transform.rotation);
            //go.transform.localScale = go.transform.localScale * 1.5f;
            //go.transform.Rotate(new Vector3(-90.0f, 0f, 0f));
            //go.transform.Translate(0f, 0f, 10f);
            CmdSpawnCity(v.gameObject.transform.position, v.gameObject.transform.rotation, (int)myColor, red, green, blue);
        }
    }

    [Command]
    void CmdPlayerDoneFirstTurn()
    {
        RpcPlayerDoneFirstTurn();
    }

    [ClientRpc]
    void RpcPlayerDoneFirstTurn()
    {
        GetComponent<turnOrder>().nextTurn();
    }

    [Command]
    void CmdSpawnCity(Vector3 v, Quaternion q, int mymat, int r, int g, int b)
    {
        RpcSpawnCity(v, q, mymat, r, g, b);
    }

    [ClientRpc]
    void RpcSpawnCity(Vector3 v, Quaternion q, int mymat, int r, int g, int b)
    {
        //if(iAmTheSpawner)
        //{
        //    iAmTheSpawner = false;

        //    return;
        //}
        //Spawn city
        GameObject go = Instantiate<GameObject>(GetComponent<PrefabHolder>().city, v, q);
        go.transform.Rotate(new Vector3(-90.0f, 0f, 0f));
        go.transform.Translate(0f, 0f, 10f);
        Color col = new Color(r, g, b);
        Debug.Log(col);
        go.GetComponent<MeshRenderer>().material = prefabHolder.materials[mymat];
        //Destory whats already there
        GameObject oldGo;
        try
        {
            oldGo = boardState.spawnedObjects[v];
            boardState.spawnedObjects.Remove(v);
            Destroy(oldGo);
        }
        catch(Exception e)
        {

        }

        Vertex source = boardState.vertexPosition[v];
        City c = new City(myColor, false);
        c.putOnBoard();
        source.setOccupyingPiece(c);
        boardState.vertexPosition.Remove(v);
        boardState.vertexPosition.Add(v, source);

        boardState.spawnedObjects.Add(v, go);
    }
    

    [Command]
    void CmdSpawnSettlement(Vector3 v, Quaternion q, int mymat, int r, int g, int b)
    {
        RpcSpawnSettlement(v, q, mymat, r, g, b);
    }

    [ClientRpc]
    void RpcSpawnSettlement(Vector3 v, Quaternion q, int mymat, int r, int g, int b)
    {
        GameObject go = Instantiate<GameObject>(GetComponent<PrefabHolder>().settlement, v, q);
        go.transform.Rotate(new Vector3(-90.0f, 0f, 0f));
        go.transform.Translate(0f, 0f, 10f);
        Color col = new Color(r, g, b);
        Debug.Log(col);
        go.GetComponent<MeshRenderer>().material = prefabHolder.materials[mymat];

        Vertex source = boardState.vertexPosition[v];
        // Put a settlement on the board
        Settlement settlement = new Settlement(myColor);
        source.setOccupyingPiece(settlement);
        settlement.putOnBoard();
        boardState.vertexPosition.Remove(v);
        boardState.vertexPosition.Add(v, source);

        boardState.spawnedObjects.Add(v, go);
    }

    [Command]
    void CmdSpawnRoad(Vector3 v, float q, bool isBoat, int mymat, int r, int g, int b)
    {
        RpcSpawnRoad(v, q, isBoat, mymat, r, g, b);
    }

    [ClientRpc]
    void RpcSpawnRoad(Vector3 v, float q, bool isBoat, int mymat, int r, int g, int b)
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
        Color col = new Color(r, g, b);
        Debug.Log(col);
        go.GetComponent<MeshRenderer>().material = prefabHolder.materials[mymat];
        Edge source = boardState.edgePosition[v];
        // Put a road on the board
        Road road = new Road(myColor, isBoat);
        source.setOccupyingPiece(road);
        road.putOnBoard();

        boardState.edgePosition.Remove(v);
        boardState.edgePosition.Add(v, source);

        boardState.spawnedObjects.Add(v, go);
    }

    public void makeMaritimeTrade()
    {

    }

    public void makeMaritimeTrade(Enums.ResourceType from, Enums.ResourceType to)
    {
        if(!secondTurn)
        {
            return;
        }
        if(p.getResources()[(int)from] < 4)
        {
            return;
        }
        p.discardResource(from, 4);
        p.addResource(to, 1);
    }

    IEnumerator pickColor()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.0f, 3.0f)+UnityEngine.Random.Range(1f, 3f) + UnityEngine.Random.Range(1f, 4f));
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

    public void rollDice()
    {
        int firstDie = UnityEngine.Random.Range(1, 6);
        int secondDie = UnityEngine.Random.Range(1, 6);
        int resourceDie = UnityEngine.Random.Range(1, 6);

        CmdRollDie(firstDie, secondDie, resourceDie);
    }

    [Command]
    void CmdRollDie(int first, int second, int resource)
    {
        RpcRollDie(first, second, resource);
    }

    [ClientRpc]
    void RpcRollDie(int first, int second, int resource)
    {
        
        myDieNumbers.Clear();
        myDieNumbers.Add(first);
        myDieNumbers.Add(second);
        myDieNumbers.Add(resource);

        int sum = first + second;
        Debug.Log(sum);
        if(sum == 7)
        {
            return;
        }
        
        //Go through all hexes
        foreach (Hex hex in boardState.hexPoisition.Values)
        {
            if(((int)hex.hexType == (int)Enums.HexType.WATER) || ((int)hex.hexType == (int)Enums.HexType.DESERT))
            {
                continue;
            }
            //If this hex has the right hex number
            if(hex.hexNumber == sum)
            {
                Debug.Log("right hex number");
                //Go through all its vertices
                foreach(Vertex vertex in hex.vertices)
                {
                    GamePiece gp = boardState.vertexPosition[vertex.gameObject.transform.position].getOccupyingPiece();
                    //If this vertex has a piece on it
                    if(gp != null)
                    {
                        Debug.Log("right piece");
                        //And this piece is my colour
                        //if((int)gp.getColor() == (int)myColor)
                        //{
                            Debug.Log("yahtzee");
                        try
                        {
                            City city = (City)gp;
                            p.addResource(hex.resourceType, 2);
                        }
                        catch(Exception e)
                        {
                            p.addResource(hex.resourceType, 1);
                        }
                        //}
                    }
                }
            }
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
