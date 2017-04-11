using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    public GameObject myHud;

    public GameObject tradePrefab;

    private MoveAuthorizer ma;
    private ProgressAuthroizor pa;
    private Graph graph;

    public int[] resourcesOffered;
    public int[] commoditiesOffered;

    public int goldOffered;

    public int[] resourcesWanted;
    public int[] commoditiesWanted;

    public int goldWanted;

    [SyncVar]
    public int iD;

    [SyncVar]
    public Enums.Color myColor;

    [SyncVar]
    public int victoryPoints;

    [SyncVar]
    private Enums.Status status;

    [SyncVar]
    private int goldCount;

    [SyncVar]
    public int numFish;

    [SyncVar]
    public bool ownsBoot;

    [SyncVar]
    private int safeCardCount;

    public List<GamePiece> pieces;
    public List<ProgressCardName> progressCards;

    public int[] resources;
    public int[] commodities;
    public int[] devFlipChart;
    public int[] resourceRatios;
    public int[] commodityRatios;


    public int cityWallsLeft;
    public bool movedRoad;
    public bool aqueduct;

    [SyncVar]
    public Enums.Special special;

    [SyncVar]
    public Enums.MoveType moveType;

    [SyncVar]
    public int oldTurn;

    [SyncVar]
    public Enums.DevChartType metropType;

    private Dictionary<Vector3, GamePiece> spawnedPieces;

    //For moves
    public Vertex v1 = null;
    public Edge e1 = null;
    public Hex h1 = null;

    [SyncVar]
    public int i1 = 0;

    [SyncVar]
    public bool b1 = false;

    [SyncVar]
    public int opponent;

    [Command]
    public void CmdInit(int iD)//call this if server is loaded after player
    {
        this.iD = iD;
        this.myColor = (Enums.Color)iD;
    }

    // Final initialization step, assign colors, game manager, movemanager
    public void Init()
    {
        this.myColor = (Enums.Color)iD;
        foreach (GamePiece piece in pieces)
        {
            piece.setColor(myColor);
        }
    }

    public void OnLoad()//
    {
        GameManager.instance.Init();
    }

    public int getTotalResources()
    {
        int ret = 0;
        for (int i = 0; i < 5; i++)
        {
            ret += resources[i];
        }
        return ret;
    }

    public int getTotalCommodities()
    {
        int ret = 0;
        for (int i = 0; i < 3; i++)
        {
            ret += commodities[i];
        }
        return ret;
    }

    public int getHandSize()
    {
        return getTotalResources() + getTotalCommodities();
    }

    public int maxHandSize()
    {
        return (3 - cityWallsLeft) * 2 + 7;
    }

    void Start()
    {

        spawnedPieces = new Dictionary<Vector3, GamePiece>();

        // Create pieces
        pieces = new List<GamePiece>();
        for (int i = 0; i < 5; i++)
        {
            Settlement s = new Settlement();
            pieces.Add(s);
        }
        for (int i = 0; i < 4; i++)
        {
            City c = new City();
            pieces.Add(c);
        }
        for (int i = 0; i < 15; i++)
        {
            Road r = new Road(false);
            Road s = new Road(true);
            pieces.Add(r);
            pieces.Add(s);
        }
        for (int i = 0; i < 6; i++)
        {
            Knight k = new Knight();
            pieces.Add(k);
        }

        progressCards = new List<ProgressCardName>();
        status = Enums.Status.ACTIVE;
        this.resourcesOffered = new int[5] { 0, 0, 0, 0, 0 };
        this.resourcesWanted = new int[5] { 0, 0, 0, 0, 0, };
        this.commoditiesOffered = new int[3] { 0, 0, 0 };
        this.commoditiesWanted = new int[3] { 0, 0, 0 };
        this.resources = new int[5] { 10, 10, 10, 10, 10 };
        this.commodities = new int[3] { 10, 10, 10 };
        this.goldCount = 0;
        this.devFlipChart = new int[3] { 0, 0, 0 };
        this.resourceRatios = new int[5] { 4, 4, 4, 4, 4 };
        this.commodityRatios = new int[3] { 4, 4, 4 };
        this.myColor = Enums.Color.NONE;
        this.victoryPoints = 0;
        this.safeCardCount = 7;
        this.cityWallsLeft = 3;
        this.movedRoad = false;
        this.aqueduct = false;
        this.moveType = Enums.MoveType.PLACE_INITIAL_SETTLEMENT;
        this.special = Enums.Special.NONE;
        this.opponent = -1;

        this.ma = new MoveAuthorizer();
        this.pa = new ProgressAuthroizor();
        this.graph = new Graph();


        if (isLocalPlayer)
        {

            gameObject.name = Network.player.ipAddress;
            Instantiate(myHud);
        }
        OnLoad();
    }

    public void Update()
    {
        if (!isLocalPlayer || GameManager.instance.getPlayerTurn() != iD)
        {
            return;
        }

        Ray ray;
        RaycastHit impact;
        GameObject pieceHit = null;



        // If game phase is phase one
        if (GameManager.instance.getGamePhase() == Enums.GamePhase.SETUP_ONE)
        {

            // Get a mouse click
            if (Input.GetButtonDown("Fire1"))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out impact))
                {
                    pieceHit = impact.collider.gameObject;
                }
            }

            if (Object.ReferenceEquals(pieceHit, null))
            {
                return;
            }

            // Must place first settlement if it hasn't been placed yet
            if (moveType == Enums.MoveType.PLACE_INITIAL_SETTLEMENT)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (ma.canPlaceInitialTownPiece(v))
                {
                    CmdPlaceInitialSettlement(v.transform.position);
                    moveType = Enums.MoveType.NONE;
                }

                // Otherwise place first road
            }
            else if (moveType == Enums.MoveType.PLACE_INITIAL_ROAD)
            {

                if (!pieceHit.tag.Equals("Edge"))
                {
                    return;
                }
                Edge e = pieceHit.GetComponent<Edge>();

                if (ma.canPlaceInitialRoad(e, this.myColor))
                {
                    CmdPlaceInitialRoad(e.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.PLACE_INITIAL_SHIP)
            {

                if (!pieceHit.tag.Equals("Edge"))
                {
                    return;
                }
                Edge e = pieceHit.GetComponent<Edge>();

                if (ma.canPlaceInitialShip(e, this.myColor))
                {
                    CmdPlaceInitialShip(e.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
        }

        // Game phase 2
        if (GameManager.instance.getGamePhase() == Enums.GamePhase.SETUP_TWO)
        {

            // Get mouse click
            if (Input.GetButtonDown("Fire1"))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out impact))
                {
                    pieceHit = impact.collider.gameObject;
                }
            }

            if (Object.ReferenceEquals(pieceHit, null))
            {
                return;
            }

            // Must place first settlement if it hasn't been placed yet
            if (moveType == Enums.MoveType.PLACE_INITIAL_CITY)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (ma.canPlaceInitialTownPiece(v))
                {
                    CmdPlaceInitialCity(v.transform.position);
                    moveType = Enums.MoveType.NONE;
                }

                // Otherwise place first road
            }
            else if (moveType == Enums.MoveType.PLACE_INITIAL_ROAD)
            {

                if (!pieceHit.tag.Equals("Edge"))
                {
                    return;
                }
                Edge e = pieceHit.GetComponent<Edge>();

                if (ma.canPlaceInitialRoad(e, this.myColor))
                {
                    CmdPlaceInitialRoad(e.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.PLACE_INITIAL_SHIP)
            {

                if (!pieceHit.tag.Equals("Edge"))
                {
                    return;
                }
                Edge e = pieceHit.GetComponent<Edge>();

                if (ma.canPlaceInitialShip(e, this.myColor))
                {
                    CmdPlaceInitialShip(e.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
        }

        // Main game phase one
        if (GameManager.instance.getGamePhase() == Enums.GamePhase.PHASE_ONE)
        {

            if (Input.GetKeyDown("t"))
            {
                int goldO = 0;
                int goldW = 0;
                if (Input.GetKeyDown("1"))
                {
                    goldO = 1;
                }
                else if (Input.GetKeyDown("2"))
                {
                    goldO = 2;
                }
                if (Input.GetKeyDown("3"))
                {
                    goldW = 1;
                }
                else if (Input.GetKeyDown("4"))
                {
                    goldW = 2;
                }

                CmdSpawnTrade(this.resources, this.resources, this.commodities, this.commodities, goldO, goldW, this.iD);
            }

            // Get mouse click
            if (Input.GetButtonDown("Fire1"))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out impact))
                {
                    pieceHit = impact.collider.gameObject;
                }
            }

            if (Object.ReferenceEquals(pieceHit, null))
            {
                return;
            }

            if (special == Enums.Special.MOVE_ROBBER)
            {

                Debug.Log("robber1");
                if (!pieceHit.tag.Equals("MainHex") && !pieceHit.tag.Equals("IslandHex"))
                {
                    return;
                }
                Hex h = pieceHit.GetComponent<Hex>();
                Debug.Log("robber2");
                if (ma.canMoveRobber(h))
                {
                    Debug.Log("robber3");
                    CmdMoveRobber(h.transform.position);

                    bool stealable = false;
                    foreach (Vertex vert in h.getVertices())
                    {
                        GamePiece vertPiece = vert.getOccupyingPiece();
                        if (!Object.ReferenceEquals(vertPiece, null))
                        {
                            Debug.Log("step2");
                            if (vertPiece.getColor() != myColor)
                            {
                                Debug.Log("step3");
                                if (vertPiece.getPieceType() == PieceType.CITY ||
                                    vertPiece.getPieceType() == PieceType.SETTLEMENT)
                                {

                                    stealable = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (stealable)
                    {
                        setSpecial(Special.STEAL_RESOURCES_ROBBER, getID());
                    }
                    else
                    {
                        if (!b1)
                        {
                            setSpecial(Special.NONE, getID());
                            foreach (Player p in GameManager.instance.getPlayers())
                            {
                                GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                            }
                            revertTurn();
                            endPhaseOne();
                        }
                        else
                        {
                            GameManager.instance.barbarianAttack();
                        }
                    }
                }
            }
            else if (special == Enums.Special.MOVE_PIRATE)
            {
                if (!pieceHit.tag.Equals("WaterHex"))
                {
                    return;
                }
                Hex h = pieceHit.GetComponent<Hex>();

                if (ma.canMovePirate(h))
                {
                    CmdMovePirate(h.transform.position);

                    bool stealable = false;
                    foreach (Edge edge in BoardState.instance.edgePosition.Values)
                    {
                        GamePiece edgePiece = edge.getOccupyingPiece();
                        if (!Object.ReferenceEquals(edgePiece, null))
                        {
                            if (edgePiece.getPieceType() == PieceType.ROAD && ((Road)edgePiece).getIsShip() && edgePiece.getColor() != myColor)
                            {
                                Hex leftHex = edge.getLeftHex();
                                if (Object.ReferenceEquals(leftHex, h))
                                {
                                    stealable = true;
                                    break;
                                }
                                Hex rightHex = edge.getRightHex();
                                if (!Object.ReferenceEquals(rightHex, null))
                                {
                                    stealable = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (stealable)
                    {
                        setSpecial(Special.STEAL_RESOURCES_PIRATE, getID());
                    }
                    else
                    {
                        if (!b1)
                        {
                            setSpecial(Special.NONE, getID());
                            foreach (Player p in GameManager.instance.getPlayers())
                            {
                                GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                            }
                            revertTurn();
                            endPhaseOne();
                        }
                        else
                        {
                            GameManager.instance.barbarianAttack();
                        }
                    }
                }
            }
            else if (special == Enums.Special.STEAL_RESOURCES_ROBBER)
            {

                if (pieceHit.tag.Equals("Vertex"))
                {

                    Vertex v = pieceHit.GetComponent<Vertex>();

                    if (ma.canStealRobber(v, myColor))
                    {
                        int opp = (int)v.getOccupyingPiece().getColor();
                        Player oppo = GameManager.instance.getPlayer(opp);
                        bool taken = false;
                        for (int i = 0; i < 5; i++)
                        {
                            if (oppo.getResources()[i] > 0)
                            {
                                GameManager.instance.getPersonalPlayer().changeResource((ResourceType)i, -1, oppo.getID());
                                taken = true;
                                changeResource((ResourceType)i, 1, getID());
                                break;
                            }
                        }
                        if (!taken)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (oppo.getCommodities()[i] > 0)
                                {
                                    GameManager.instance.getPersonalPlayer().changeCommodity((CommodityType)i, -1, oppo.getID());
                                    changeCommodity((CommodityType)i, 1, getID());
                                    break;
                                }
                            }
                        }
                        if (!b1)
                        {
                            setSpecial(Special.NONE, getID());
                            foreach (Player p in GameManager.instance.getPlayers())
                            {
                                GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                            }
                            revertTurn();
                            endPhaseOne();
                        }
                        else
                        {
                            GameManager.instance.barbarianAttack();
                        }
                    }
                }
            }
            else if (special == Enums.Special.STEAL_RESOURCES_PIRATE)
            {
                if (pieceHit.tag.Equals("Edge"))
                {

                    Edge e = pieceHit.GetComponent<Edge>();

                    if (ma.canStealPirate(e, myColor))
                    {
                        int opp = (int)e.getOccupyingPiece().getColor();
                        Player oppo = GameManager.instance.getPlayer(opp);
                        bool taken = false;
                        for (int i = 0; i < 5; i++)
                        {
                            if (oppo.getResources()[i] > 0)
                            {
                                GameManager.instance.getPersonalPlayer().changeResource((ResourceType)i, -1, oppo.getID());
                                taken = true;
                                changeResource((ResourceType)i, 1, getID());
                                break;
                            }
                        }
                        if (!taken)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (oppo.getCommodities()[i] > 0)
                                {
                                    GameManager.instance.getPersonalPlayer().changeCommodity((CommodityType)i, -1, oppo.getID());
                                    changeCommodity((CommodityType)i, 1, getID());
                                    break;
                                }
                            }
                        }
                        if (!b1)
                        {
                            setSpecial(Special.NONE, getID());
                            foreach (Player p in GameManager.instance.getPlayers())
                            {
                                GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                            }
                            revertTurn();
                            endPhaseOne();
                        }
                        else
                        {
                            GameManager.instance.barbarianAttack();
                        }
                    }
                }
            }
            else if (special == Enums.Special.CHOOSE_DESTROYED_CITY)
            {
                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (ma.canDestroyCity(v, myColor))
                {
                    CmdDestroyCity(v.transform.position);

                    GameManager.instance.barbarianLossShortcut(getID() + 1);
                }
            }
        }

        // Main game phase two
        if (GameManager.instance.getGamePhase() == Enums.GamePhase.PHASE_TWO)
        {
            // Get mouse click

            if (Input.GetButtonDown("Fire1"))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out impact))
                {
                    pieceHit = impact.collider.gameObject;
                }
            }

            if (Object.ReferenceEquals(pieceHit, null))
            {
                return;
            }

            if (special == Enums.Special.KNIGHT_DISPLACED)
            {
                bool chosen = false;
                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (graph.areConnectedVertices(v, v1, myColor))
                {
                    if (Object.ReferenceEquals(v.getOccupyingPiece(), null))
                    {
                        chosen = true;
                    }
                }

                if (Object.ReferenceEquals(v, v1)) chosen = false;

                if (chosen)
                {
                    CmdAlternateDisplaceKnight(v.transform.position);

                    foreach (Player p in GameManager.instance.players)
                    {
                        GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                    }
                    revertTurn();
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (special == Enums.Special.CHOOSE_METROPOLIS)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (ma.canChooseMetropolis(v, this.myColor))
                {

                    setSpecial(Special.NONE, getID());
                    foreach (Player p in GameManager.instance.getPlayers())
                    {
                        GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                    }

                    CmdChooseMetropolis(v.transform.position);
                }

                // Otherwise place first road
            }
            else if (moveType == Enums.MoveType.BUILD_SETTLEMENT)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (ma.canBuildSettlement(v, this.resources, this.pieces, this.myColor))
                {
                    CmdBuildSettlement(v.transform.position);
                    moveType = Enums.MoveType.NONE;
                }

                // Otherwise place first road
            }
            else if (moveType == Enums.MoveType.BUILD_ROAD)
            {

                if (!pieceHit.tag.Equals("Edge"))
                {
                    return;
                }
                Edge e = pieceHit.GetComponent<Edge>();

                if (ma.canBuildRoad(e, this.resources, this.pieces, this.myColor))
                {
                    CmdBuildRoad(e.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.MOVE_KNIGHT)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (Object.ReferenceEquals(v1, null))
                {
                    SetV1(v, getID());
                }
                else
                {
                    if (ma.canKnightMove(v1, v, this.myColor))
                    {
                        CmdMoveKnight(v.transform.position);
                        moveType = Enums.MoveType.NONE;
                    }
                }
            }
            else if (moveType == Enums.MoveType.DISPLACE_KNIGHT)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (Object.ReferenceEquals(v1, null))
                {
                    SetV1(v, getID());
                }
                else
                {
                    if (ma.canKnightDisplace(v1, v, this.myColor))
                    {
                        Debug.Log("knight displaced");
                        Debug.Log(v1 + " " + v);
                        CmdDisplaceKnight(v.transform.position);
                        moveType = Enums.MoveType.NONE;
                    }
                }
            }
            else if (moveType == Enums.MoveType.UPGRADE_KNIGHT)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (ma.canUpgradeKnight(this.resources, this.devFlipChart, v, this.pieces, this.myColor))
                {
                    CmdUpgradeKnight(v.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.ACTIVATE_KNIGHT)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (ma.canActivateKnight(this.resources, v, this.myColor))
                {
                    CmdActivateKnight(v.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.BUILD_CITY)
            {

                Debug.Log("hello4");

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();
                Debug.Log("hello5");

                if (ma.canBuildCity(v, this.resources, this.pieces, this.myColor))
                {
                    CmdBuildCity(v.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.BUILD_CITY_WALL)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (ma.canBuildCityWall(v, this.resources, this.cityWallsLeft, this.myColor))
                {
                    CmdBuildCityWall(v.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.BUILD_KNIGHT)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (ma.canBuildKnight(v, this.resources, this.pieces, this.myColor))
                {
                    CmdBuildKnight(v.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.BUILD_SHIP)
            {

                if (!pieceHit.tag.Equals("Edge"))
                {
                    return;
                }
                Edge e = pieceHit.GetComponent<Edge>();

                if (ma.canBuildShip(e, this.resources, this.pieces, this.myColor))
                {
                    Debug.Log("ship1");
                    CmdBuildShip(e.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.CHASE_ROBBER)
            {

                if (Object.ReferenceEquals(v1, null))
                {
                    if (!pieceHit.tag.Equals("Vertex"))
                    {
                        return;
                    }
                    Vertex v = pieceHit.GetComponent<Vertex>();
                    SetV1(v, getID());
                }
                else
                {
                    if (!pieceHit.tag.Equals("MainHex") && !pieceHit.tag.Equals("MainHex"))
                    {
                        return;
                    }
                    Hex h = pieceHit.GetComponent<Hex>();
                    if (ma.canChaseRobber(v1, this.myColor))
                    {
                        CmdChaseRobber(h.transform.position);
                        moveType = Enums.MoveType.NONE;
                    }
                }
            }
            else if (moveType == Enums.MoveType.MOVE_SHIP)
            {

                if (!pieceHit.tag.Equals("Edge"))
                {
                    return;
                }
                Edge e = pieceHit.GetComponent<Edge>();

                if (Object.ReferenceEquals(e1, null))
                {
                    SetE1(e, getID());
                }
                else
                {
                    if (ma.canShipMove(e1, e, this.myColor))
                    {
                        Debug.Log("heywhatsup");
                        CmdMoveShip(e.transform.position);
                        moveType = Enums.MoveType.NONE;
                    }
                }
            }
            else if (moveType == Enums.MoveType.FISH_2)
            {

                if (!pieceHit.tag.Equals("LandHex") && !pieceHit.tag.Equals("IslandHex") && !pieceHit.tag.Equals("WaterHex"))
                {
                    return;
                }
                Hex h = pieceHit.GetComponent<Hex>();

                bool robber = false;
                bool pirate = false;

                GamePiece thief = h.getOccupyingPiece();
                if (!Object.ReferenceEquals(thief, null))
                {
                    if (thief.getPieceType() == PieceType.ROBBER) robber = true;
                    else if (thief.getPieceType() == PieceType.PIRATE) pirate = true;
                }

                if (numFish < 2) return;

                if (robber)
                {
                    CmdRemoveRobber();
                    moveType = Enums.MoveType.NONE;
                }
                else if (pirate)
                {
                    CmdRemovePirate();
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.FISH_5)
            {

                if (!pieceHit.tag.Equals("Edge"))
                {
                    return;
                }
                Edge e = pieceHit.GetComponent<Edge>();

                if (numFish < 5) return;

                if (ma.canFishRoad(e, this.numFish, this.pieces, this.myColor))
                {
                    CmdFishRoad(e.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.PROGRESS_BISHOP)
            {

                if (!pieceHit.tag.Equals("MainHex") && !pieceHit.tag.Equals("IslandHex"))
                {
                    return;
                }
                Hex h = pieceHit.GetComponent<Hex>();

                if (ma.canMoveRobber(h))
                {
                    CmdBishop(h.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.PROGRESS_DIPLOMAT)
            {
                if (!pieceHit.tag.Equals("Edge"))
                {
                    return;
                }
                Edge e = pieceHit.GetComponent<Edge>();

                if (Object.ReferenceEquals(e1, null))
                {
                    SetE1(e, getID());
                }
                else
                {
                    if (pa.canRoadMove(e1, e, this.myColor))
                    {
                        CmdDiplomat(e.transform.position);
                        moveType = Enums.MoveType.NONE;
                    }
                }
            }
            else if (moveType == Enums.MoveType.PROGRESS_INTRIGUE)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();
                if (pa.canIntrigueKnight(v, this.myColor))
                {
                    CmdIntrigue(v.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.PROGRESS_ENGINEER)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();
                if (pa.canEngineer(v, this.cityWallsLeft, this.myColor))
                {
                    CmdIntrigue(v.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.PROGRESS_INVENTOR)
            {

                if (!pieceHit.tag.Equals("MainHex") && !pieceHit.tag.Equals("IslandHex"))
                {
                    return;
                }
                Hex h = pieceHit.GetComponent<Hex>();

                if (Object.ReferenceEquals(h1, null))
                {
                    SetH1(h, getID());
                }
                else
                {
                    if (pa.canInventor(h1, h))
                    {
                        CmdInventor(h.transform.position);
                        moveType = Enums.MoveType.NONE;
                    }
                }
            }
            else if (moveType == Enums.MoveType.PROGRESS_MEDICINE)
            {

                if (!pieceHit.tag.Equals("Vertex"))
                {
                    return;
                }
                Vertex v = pieceHit.GetComponent<Vertex>();
                if (pa.canMedicine(v, this.resources, this.pieces, this.myColor))
                {
                    Debug.Log("medicine " + v);
                    CmdMedicine(v.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
            else if (moveType == Enums.MoveType.PROGRESS_ROAD_BUILDING_1)
            {

                if (!pieceHit.tag.Equals("Edge"))
                {
                    return;
                }
                Edge e = pieceHit.GetComponent<Edge>();
                if (pa.canRoadBuilding(e, this.pieces, this.myColor))
                {
                    CmdRoadBuilding(e.transform.position);
                    moveType = Enums.MoveType.PROGRESS_ROAD_BUILDING_2;
                }
            }
            else if (moveType == Enums.MoveType.PROGRESS_ROAD_BUILDING_2)
            {

                if (!pieceHit.tag.Equals("Edge"))
                {
                    return;
                }
                Edge e = pieceHit.GetComponent<Edge>();
                if (pa.canRoadBuilding(e, this.pieces, this.myColor))
                {
                    CmdRoadBuilding(e.transform.position);
                    moveType = Enums.MoveType.NONE;
                }
            }
        }
    }

    /* public void tradeWithBank()
     {
         Trades newTrade = new Trades();
         newTrade.resourcesOffered = this.resourcesOffered;
         newTrade.commoditiesOffered = this.commoditiesOffered;
         newTrade.resourcesWanted = this.resourcesWanted;
         newTrade.commoditiesWanted = this.commoditiesWanted;
         newTrade.goldOffered = this.goldOffered;
         newTrade.goldWanted = this.goldWanted;
         newTrade.offering = this.iD;
         MoveManager.instance.tradeWithBank(this.resourceRatios, this.commodityRatios, newTrade);
     }*/


    [Command]
    public void CmdTradeWithBank(int[] resourcesOffered, int[] resourcesDemanded, int[] commoditiesOffered, int[] commoditiesDemanded, int goldOffered, int goldDemanded, int playerId)
    {
        GameObject trade = (GameObject)GameObject.Instantiate(tradePrefab);
        trade.GetComponent<Trades>().init(resourcesOffered, resourcesDemanded, commoditiesOffered, commoditiesDemanded, goldOffered, goldDemanded, this);
        NetworkServer.Spawn(trade);
        trade.GetComponent<Trades>().RpcSetPlayer(this.netId);
        MoveManager.instance.tradeWithBank(this.resourceRatios, this.commodityRatios, trade.GetComponent<Trades>());
        CmdDestroyObject(trade.GetComponent<Trades>().netId);
    }



    public int getID()
    {
        return this.iD;
    }

    public void SetColor(Enums.Color color)
    {
        myColor = color;
        CmdSetColor(color);
    }

    [Command]
    void CmdSetColor(Enums.Color color)
    {
        myColor = color;
    }

    public List<GamePiece> getNotOnBoardPiece()//iterate through list of player's gamePieces
    {//for each piece that isOnBoard equals false, add it to a new List
        List<GamePiece> notOnBoard = new List<GamePiece>();
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].isOnBoard() == false)
            {
                notOnBoard.Add(pieces[i]);
            }
        }
        return notOnBoard;//return list of pieces that are not on board
    }

    public List<GamePiece> getGamePieces()//return player's pieces
    {
        return this.pieces;
    }

    public List<ProgressCardName> getProgressCards()//return player's progress cards
    {
        return this.progressCards;
    }

    public int[] getResourcesWanted()
    {
        return this.resourcesWanted;
    }

    public int[] getResourcesOffered()
    {
        return this.resourcesOffered;
    }

    public int[] getCommoditiesWanted()
    {
        return this.commoditiesWanted;
    }

    public int[] getCommoditiesOffered()
    {
        return this.commoditiesOffered;
    }

    public int[] getResources()//return player's resources
    {
        return this.resources;
    }

    public int[] getCommodities()//return player's commodities
    {
        return this.commodities;
    }

    public int[] getResourceRatios()//return player's trade ratios with bank for resources (ie ratios obtained from ports)
    {
        return this.resourceRatios;
    }

    public int[] getCommodityRatios()//return player's trade ratios with bank for commodities
    {
        return this.commodityRatios;
    }

    public int[] getDevFlipChart()//return array of progress in the development flip chart
    {
        return this.devFlipChart;
    }

    public int getVictoryCounts()//return victory points
    {
        return this.victoryPoints;
    }

    public bool changeVictoryPoints(int num, int plyr)
    {//decrease victory points
     //if (!isLocalPlayer)
     //return false;
        if (GameManager.instance.getPlayer(plyr).victoryPoints + num < 0)
        {
            return false;
        }
        CmdChangeVP(num, plyr);
        return true;
    }

    [Command]
    public void CmdChangeVP(int num, int plyr)
    {
        RpcChangeVP(num, plyr);
    }

    [ClientRpc]
    public void RpcChangeVP(int num, int plyr)
    {
        GameManager.instance.getPlayer(plyr).victoryPoints += num;
    }

    public void hasBoot(bool doesHaveBoot, int player)
    {
        CmdHasBoot(doesHaveBoot, player);
    }

    [Command]
    public void CmdHasBoot(bool doesHaveBoot, int player)
    {
        RpcHasBoot(doesHaveBoot, player);
    }

    [ClientRpc]
    public void RpcHasBoot(bool doesHaveBoot, int player)
    {
        GameManager.instance.getPlayer(player).ownsBoot = doesHaveBoot;
    }

    public bool changeFishCount(int num, int plyr)
    {
        if ((GameManager.instance.getPlayer(plyr).numFish + num) < 0)
        {
            return false;
        }
        CmdChangeFishCount(num, plyr);
        return true;
    }

    public bool changeGoldOffered(int num, int player)
    {
        if (GameManager.instance.getPlayer(player).goldOffered + num < 0)
        {
            return false;
        }

        CmdChangeGoldOffered(num, player);
        return true;
    }

    [Command]
    public void CmdChangeGoldOffered(int num, int player)
    {
        RpcChangeGoldOffered(num, player);
    }

    [ClientRpc]
    public void RpcChangeGoldOffered(int num, int player)
    {
        GameManager.instance.getPlayer(player).goldOffered += num;
    }

    public bool changeGoldWanted(int num, int player)
    {
        if (GameManager.instance.getPlayer(player).goldWanted + num < 0)
        {
            return false;
        }

        CmdChangeGoldWanted(num, player);
        return true;
    }

    [Command]
    public void CmdChangeGoldWanted(int num, int player)
    {
        RpcChangeGoldWanted(num, player);
    }

    [ClientRpc]
    public void RpcChangeGoldWanted(int num, int player)
    {
        GameManager.instance.getPlayer(player).goldWanted += num;
    }

    [Command]
    public void CmdChangeFishCount(int num, int plyr)
    {
        RpcChangeFishCount(num, plyr);
    }

    [ClientRpc]
    public void RpcChangeFishCount(int num, int plyr)
    {
        GameManager.instance.getPlayer(plyr).numFish += num;
    }

    public int getGoldCount()//return gold count
    {
        return this.goldCount;
    }

    public bool changeGoldCount(int num, int plyr)
    {//decrease gold count
        if (GameManager.instance.getPlayer(plyr).goldCount + num < 0)
        {
            return false;
        }
        CmdChangeGold(num, plyr);
        return true;
    }

    [Command]
    public void CmdChangeGold(int num, int plyr)
    {
        RpcChangeGold(num, plyr);
    }

    [ClientRpc]
    public void RpcChangeGold(int num, int plyr)
    {
        GameManager.instance.getPlayer(plyr).goldCount += num;
    }

    public int getSafeCardCount()//get safe card count (number of cards possible to carry in a hand)
    {
        return this.safeCardCount;
    }


    public Enums.Color getColor()
    {
        return this.myColor;
    }



    public Enums.Status getStatus()
    {
        return this.status;
    }

    public Enums.MoveType getMoveType()
    {
        return this.moveType;
    }

    public void setMoveType(Enums.MoveType mType, int plyr)
    {
        CmdSetMoveType(mType, plyr);
    }

    [Command]
    public void CmdSetMoveType(Enums.MoveType mType, int plyr)
    {
        RpcSetMoveType(mType, plyr);
    }

    [ClientRpc]
    public void RpcSetMoveType(Enums.MoveType mType, int plyr)
    {
        Debug.Log(mType);
        GameManager.instance.getPlayer(plyr).moveType = mType;
    }

    public Enums.Special getSpecial()
    {
        return this.special;
    }

    public void setSpecial(Enums.Special spec, int plyr)
    {
        CmdSetSpecial(spec, plyr);
        Debug.Log("spec1");
    }

    [Command]
    public void CmdSetSpecial(Enums.Special spec, int plyr)
    {
        RpcSetSpecial(spec, plyr);
        Debug.Log("spec2");
    }

    [ClientRpc]
    public void RpcSetSpecial(Enums.Special spec, int plyr)
    {
        Debug.Log(spec);
        GameManager.instance.getPlayer(plyr).special = spec;
        Debug.Log("spec3");
    }

    public int getOldTurn()
    {
        return this.oldTurn;
    }

    public void setOldTurn(int turn)
    {
        CmdSetOldTurn(turn);
    }

    [Command]
    public void CmdSetOldTurn(int turn)
    {
        RpcSetOldTurn(turn);
    }

    [ClientRpc]
    public void RpcSetOldTurn(int turn)
    {
        foreach (Player p in GameManager.instance.getPlayers())
        {
            p.oldTurn = turn;
        }
    }

    public int getI1()
    {
        return this.i1;
    }

    public void setI1(int i, int plyr)
    {
        CmdSetI1(i, plyr);
    }

    [Command]
    public void CmdSetI1(int i, int plyr)
    {
        RpcSetI1(i, plyr);
    }

    [ClientRpc]
    public void RpcSetI1(int i, int plyr)
    {
        GameManager.instance.getPlayer(plyr).i1 = i;
    }

    public Enums.DevChartType getMetropolis()
    {
        return this.metropType;
    }

    public void setMetropolis(Enums.DevChartType d, int plyr)
    {
        CmdSetMetropolis(d, plyr);
    }

    [Command]
    public void CmdSetMetropolis(Enums.DevChartType d, int plyr)
    {
        RpcSetMetropolis(d, plyr);
    }

    [ClientRpc]
    public void RpcSetMetropolis(Enums.DevChartType d, int plyr)
    {
        GameManager.instance.getPlayer(plyr).metropType = d;
    }

    public int getOpponent()
    {
        return this.opponent;
    }

    public void setOpponent(int i, int plyr)
    {
        CmdSetOpponent(i, plyr);
    }

    [Command]
    public void CmdSetOpponent(int i, int plyr)
    {
        RpcSetOpponent(i, plyr);
    }

    [ClientRpc]
    public void RpcSetOpponent(int i, int plyr)
    {
        GameManager.instance.getPlayer(plyr).opponent = i;
    }

    public bool getB1()
    {
        return this.b1;
    }

    public void setB1(bool b, int plyr)
    {
        CmdSetB1(b, plyr);
    }

    [Command]
    public void CmdSetB1(bool b, int plyr)
    {
        RpcSetB1(b, plyr);
    }

    [ClientRpc]
    public void RpcSetB1(bool b, int plyr)
    {
        GameManager.instance.getPlayer(plyr).b1 = b;
    }

    public void setSpecialTurn(int turn)
    {
        CmdSetSpecialTurn(turn);
    }

    [Command]
    public void CmdSetSpecialTurn(int turn)
    {
        RpcSetSpecialTurn(turn);
    }

    [ClientRpc]
    public void RpcSetSpecialTurn(int turn)
    {
        GameManager.instance.setSpecialTurn(turn, isServer);
    }

    public void setMetropolisPlayer(Enums.DevChartType d, Vertex v)
    {
        CmdSetMetropolisPlayer(d, v.transform.position);
    }

    [Command]
    public void CmdSetMetropolisPlayer(Enums.DevChartType d, Vector3 v)
    {
        RpcSetMetropolisPlayer(d, v);
    }

    [ClientRpc]
    public void RpcSetMetropolisPlayer(Enums.DevChartType d, Vector3 v)
    {
        GameManager.instance.setMetropolisPlayer(d, BoardState.instance.vertexPosition[v]);
    }

    public void activateKnights(int plyr)
    {
        CmdActivateKnights(plyr);
    }

    [Command]
    public void CmdActivateKnights(int plyr)
    {
        RpcActivateKnights(plyr);
    }

    [ClientRpc]
    public void RpcActivateKnights(int plyr)
    {
        foreach (GamePiece piece in GameManager.instance.getPlayer(plyr).getGamePieces())
        {
            if (piece.getPieceType() == PieceType.KNIGHT && piece.isOnBoard())
            {
                Knight k = (Knight)piece;
                k.activateKnight();
            }
        }
    }

    public void deactivateKnights()
    {
        CmdDeactivateKnights();
    }

    [Command]
    public void CmdDeactivateKnights()
    {
        RpcDeactivateKnights();
    }

    [ClientRpc]
    public void RpcDeactivateKnights()
    {
        foreach (Player p in GameManager.instance.getPlayers())
        {
            foreach (GamePiece piece in p.getGamePieces())
            {
                if (piece.getPieceType() == PieceType.KNIGHT)
                {
                    Knight k = (Knight)piece;
                    k.deactivateKnight();
                }
            }
        }
    }

    public void revertTurn()
    {
        Debug.Log("prevert1" + GameManager.instance.getPlayerTurn());
        CmdRevertTurn();
    }

    [Command]
    public void CmdRevertTurn()
    {
        RpcRevertTurn();
    }

    [ClientRpc]
    public void RpcRevertTurn()
    {
        GameManager.instance.setSpecialTurn(oldTurn, isServer);
        Debug.Log("prevert" + GameManager.instance.getPlayerTurn());
    }

    public void endPhaseOne()
    {
        Debug.Log("epo1");
        if (GameManager.instance.getGamePhase() == GamePhase.PHASE_ONE) CmdEndPhaseOne(isServer);
    }

    [Command]
    public void CmdEndPhaseOne(bool server)
    {
        Debug.Log("epo2");
        RpcEndPhaseOne(server);
    }

    [ClientRpc]
    public void RpcEndPhaseOne(bool server)
    {
        Debug.Log("epo3");
        GameManager.instance.setGamePhase(GamePhase.PHASE_TWO, server);
    }

    public void moveBarbarian()
    {
        CmdMoveBarbarian();
    }

    [Command]
    public void CmdMoveBarbarian()
    {
        RpcMoveBarbarian();
    }

    [ClientRpc]
    public void RpcMoveBarbarian()
    {
        if (GameManager.instance.getBarbarianPosition() + 1 == 7)
        {
            GameManager.instance.barbarianPos = 0;
            GameManager.instance.barbarianHasAttacked = true;
        }
        else
        {
            GameManager.instance.barbarianPos++;
        }
    }

    public void SetV1(Vertex vReplace, int plyr)
    {
        CmdSetV1(vReplace.transform.position, plyr);
    }

    [Command]
    public void CmdSetV1(Vector3 vReplace, int plyr)
    {
        RpcSetV1(vReplace, plyr);
    }

    [ClientRpc]
    public void RpcSetV1(Vector3 vReplace, int plyr)
    {
        GameManager.instance.getPlayer(plyr).v1 = BoardState.instance.vertexPosition[vReplace];
    }

    public void ResetV1(int plyr)
    {
        CmdResetV1(plyr);
    }

    [Command]
    public void CmdResetV1(int plyr)
    {
        RpcResetV1(plyr);
    }

    [ClientRpc]
    public void RpcResetV1(int plyr)
    {
        GameManager.instance.getPlayer(plyr).v1 = null;
    }

    public void SetH1(Hex hReplace, int plyr)
    {
        CmdSetH1(hReplace.transform.position, plyr);
    }

    [Command]
    public void CmdSetH1(Vector3 hReplace, int plyr)
    {
        RpcSetH1(hReplace, plyr);
    }

    [ClientRpc]
    public void RpcSetH1(Vector3 hReplace, int plyr)
    {
        GameManager.instance.getPlayer(plyr).h1 = BoardState.instance.hexPosition[hReplace];
    }

    public void ResetH1(int plyr)
    {
        CmdResetV1(plyr);
    }

    [Command]
    public void CmdResetH1(int plyr)
    {
        RpcResetV1(plyr);
    }

    [ClientRpc]
    public void RpcResetH1(int plyr)
    {
        GameManager.instance.getPlayer(plyr).h1 = null;
    }

    public void SetE1(Edge eReplace, int plyr)
    {
        CmdSetE1(eReplace.transform.position, plyr);
    }

    [Command]
    public void CmdSetE1(Vector3 eReplace, int plyr)
    {
        RpcSetE1(eReplace, plyr);
    }

    [ClientRpc]
    public void RpcSetE1(Vector3 eReplace, int plyr)
    {
        GameManager.instance.getPlayer(plyr).e1 = BoardState.instance.edgePosition[eReplace];
    }

    public void ResetE1(int plyr)
    {
        CmdResetE1(plyr);
    }

    [Command]
    public void CmdResetE1(int plyr)
    {
        RpcResetE1(plyr);
    }

    [ClientRpc]
    public void RpcResetE1(int plyr)
    {
        GameManager.instance.getPlayer(plyr).e1 = null;
    }

    public void setStatus(Status newStatus)
    {
        this.status = newStatus;
        CmdSetStatus(newStatus);
    }

    [Command]
    public void CmdSetStatus(Status newStatus)
    {
        this.status = newStatus;
    }

    public void increaseSafeCardCount(int count)
    {
        this.safeCardCount += count;
    }

    public void decreaseSafeCardCount(int count)
    {
        this.safeCardCount -= count;
    }

    public int getCityWallCount()
    {
        return this.cityWallsLeft;
    }

    public bool hasMovedRoad()
    {
        return this.movedRoad;
    }

    public void movesRoad()
    {
        this.movedRoad = true;
    }

    public void roadNotMoved()
    {
        this.movedRoad = false;
    }

    public bool getAqueduct()
    {
        return this.aqueduct;
    }

    public void makeAqueduct(int plyr)
    {
        CmdMakeAqueduct(plyr);
    }

    [Command]
    void CmdMakeAqueduct(int plyr)
    {
        RpcMakeAqueduct(plyr);
    }

    [ClientRpc]
    void RpcMakeAqueduct(int plyr)
    {
        GameManager.instance.getPlayer(plyr).aqueduct = true;
    }

    public void createAqueductArray(bool[] array)
    {
        CmdCreateAqueductArray(array);
    }

    [Command]
    void CmdCreateAqueductArray(bool[] array)
    {
        RpcCreateAqueductArray(array);
    }

    [ClientRpc]
    void RpcCreateAqueductArray(bool[] array)
    {
        GameManager.instance.aqueducts = array;
    }

    public void upgradeDevChart(Enums.DevChartType devChartType, int plyr)
    {
        CmdUpgradeDevChart(devChartType, plyr);
    }

    [Command]
    public void CmdUpgradeDevChart(Enums.DevChartType devChartType, int plyr)
    {
        RpcUpgradeDevChart(devChartType, plyr);

    }

    [ClientRpc]
    public void RpcUpgradeDevChart(Enums.DevChartType devChartType, int plyr)
    {
        int devPosition = (int)devChartType;
        GameManager.instance.getPlayer(plyr).devFlipChart[devPosition]++;
    }

    public void updateResourceRatios(int[] newRatios, int plyr)
    {
        CmdUpdateResourceRatios(newRatios, plyr);
    }

    [Command]
    void CmdUpdateResourceRatios(int[] newRatios, int plyr)
    {
        RpcUpdateResourceRatios(newRatios, plyr);
    }

    [ClientRpc]
    void RpcUpdateResourceRatios(int[] newRatios, int plyr)
    {
        GameManager.instance.getPlayer(plyr).resourceRatios = newRatios;
    }

    [Command]
    public void CmdAlchemistRoll(int a, int b)
    {
        GameManager.instance.alchemistRolled(a, b, isServer);
    }

    public bool changeResourceOffer(ResourceType resource, int num, int player)
    {
        int resPosition = (int)resource;
        if (GameManager.instance.getPlayer(player).resourcesOffered[resPosition] + num < 0)
        {
            return false;
        }

        CmdChangeResourceOffer(resource, num, player);
        return true;
    }

    [Command]
    public void CmdChangeResourceOffer(ResourceType resourceType, int num, int player)
    {
        RpcChangeResourceOffer(resourceType, num, player);
    }

    [ClientRpc]
    public void RpcChangeResourceOffer(ResourceType resource, int num, int player)
    {
        int resP = (int)resource;
        GameManager.instance.getPlayer(player).resourcesOffered[resP] += num;
    }

    public bool changeResourceWanted(ResourceType resource, int num, int player)
    {
        int resPosition = (int)resource;
        if (GameManager.instance.getPlayer(player).resourcesWanted[resPosition] + num < 0)
        {
            return false;
        }

        CmdChangeResourceWanted(resource, num, player);
        return true;
    }

    [Command]
    public void CmdChangeResourceWanted(ResourceType resourceType, int num, int player)
    {
        RpcChangeResourceWanted(resourceType, num, player);
    }

    [ClientRpc]
    public void RpcChangeResourceWanted(ResourceType resource, int num, int player)
    {
        int resP = (int)resource;
        GameManager.instance.getPlayer(player).resourcesWanted[resP] += num;
    }

    public bool changeCommodityOffered(CommodityType commodity, int num, int player)
    {
        int comPosition = (int)commodity;
        if (GameManager.instance.getPlayer(player).commoditiesOffered[comPosition] + num < 0)
        {
            return false;
        }

        CmdChangeCommodityOffered(commodity, num, player);
        return true;
    }

    [Command]
    public void CmdChangeCommodityOffered(CommodityType commodity, int num, int player)
    {
        RpcChangeCommodityOffered(commodity, num, player);
    }

    [ClientRpc]
    public void RpcChangeCommodityOffered(CommodityType commodity, int num, int player)
    {
        int comP = (int)commodity;
        GameManager.instance.getPlayer(player).commoditiesOffered[comP] += num;
    }

    public bool changeCommodityWanted(CommodityType commodity, int num, int player)
    {
        int comPosition = (int)commodity;
        if (GameManager.instance.getPlayer(player).commoditiesWanted[comPosition] + num < 0)
        {
            return false;
        }

        CmdChangeCommodityWanted(commodity, num, player);
        return true;
    }

    [Command]
    public void CmdChangeCommodityWanted(CommodityType commodity, int num, int player)
    {
        RpcChangeCommodityWanted(commodity, num, player);
    }

    [ClientRpc]
    public void RpcChangeCommodityWanted(CommodityType commodity, int num, int player)
    {
        int comP = (int)commodity;
        GameManager.instance.getPlayer(player).commoditiesWanted[comP] += num;
    }

    public bool changeResource(ResourceType resource, int num, int plyr)
    {
        int resPosition = (int)resource;
        if (GameManager.instance.getPlayer(plyr).resources[resPosition] + num < 0)
        {
            return false;
        }
        CmdChangeResource(resource, num, plyr);
        return true;
    }

    [Command]
    public void CmdChangeResource(ResourceType resourceType, int num, int plyr)
    {
        RpcChangeResource(resourceType, num, plyr);
    }

    [ClientRpc]
    public void RpcChangeResource(ResourceType resourceType, int num, int plyr)
    {
        int resP = (int)resourceType;
        GameManager.instance.getPlayer(plyr).resources[resP] += num;
    }

    public void addProgressCard(ProgressCardName cardName, int plyr)
    {
        CmdAddProgressCard(cardName, plyr);
    }

    [Command]
    public void CmdAddProgressCard(ProgressCardName cardName, int plyr)
    {
        RpcAddProgressCard(cardName, plyr);
    }

    [ClientRpc]
    public void RpcAddProgressCard(ProgressCardName cardName, int plyr)
    {
        GameManager.instance.getPlayer(plyr).progressCards.Add(cardName);
    }

    public void removeProgressCard(ProgressCardName cardName, int plyr)
    {
        List<ProgressCardName> progs = GameManager.instance.getPlayer(plyr).getProgressCards();
        bool exists = false;
        foreach (ProgressCardName prog in progs)
        {
            if (cardName == prog) exists = true;
        }

        if (!exists) return;
        CmdRemoveProgressCard(cardName, plyr);
    }

    [Command]
    public void CmdRemoveProgressCard(ProgressCardName cardName, int plyr)
    {
        RpcRemoveProgressCard(cardName, plyr);
    }

    [ClientRpc]
    public void RpcRemoveProgressCard(ProgressCardName cardName, int plyr)
    {
        GameManager.instance.getPlayer(plyr).progressCards.Remove(cardName);
    }

    public void changeSafeCardCount(int count)
    {
        this.safeCardCount += count;
    }

    public bool changeCityWallCount(int num, int plyr)
    {
        if (GameManager.instance.getPlayer(plyr).cityWallsLeft <= 0)
        {
            return false;
        }
        CmdChangeCityWalls(num, plyr);
        return true;
    }

    [Command]
    public void CmdChangeCityWalls(int num, int plyr)
    {
        RpcChangeCityWalls(num, plyr);
    }

    [ClientRpc]
    public void RpcChangeCityWalls(int num, int plyr)
    {
        GameManager.instance.getPlayer(plyr).cityWallsLeft += num;
    }

    public void updateResourceRatio(ResourceType resourceType, int newRatio, int plyr)
    {
        CmdUpdateResourceRatio(resourceType, newRatio, plyr);
        return;
    }

    [Command]
    public void CmdUpdateResourceRatio(ResourceType resourceType, int newRatio, int plyr)
    {
        RpcUpdateResourceRatio(resourceType, newRatio, plyr);
    }

    [ClientRpc]
    public void RpcUpdateResourceRatio(ResourceType resourceType, int newRatio, int plyr)
    {
        int resP = (int)resourceType;
        GameManager.instance.getPlayer(plyr).resourceRatios[resP] = newRatio;
    }

    public void updateCommodityRatio(CommodityType commodity, int newRatio, int plyr)
    {
        CmdUpdateCommodityRatio(commodity, newRatio, plyr);
        return;
    }

    [Command]
    public void CmdUpdateCommodityRatio(CommodityType commodityType, int newRatio, int plyr)
    {
        RpcUpdateCommodityRatio(commodityType, newRatio, plyr);
    }

    [ClientRpc]
    public void RpcUpdateCommodityRatio(CommodityType commodityType, int newRatio, int plyr)
    {
        int comP = (int)commodityType;
        GameManager.instance.getPlayer(plyr).commodityRatios[comP] = newRatio;
    }


    public bool changeCommodity(CommodityType commodityType, int num, int plyr)
    {
        int comPosition = (int)commodityType;
        if (GameManager.instance.getPlayer(plyr).commodities[comPosition] + num < 0)//check if there are enough commodities
        {
            return false;//if there arent return false to denote an error
        }
        CmdChangeCommodity(commodityType, num, plyr);
        return true;
    }

    [Command]
    public void CmdChangeCommodity(CommodityType commodityType, int num, int plyr)
    {
        RpcChangeCommodity(commodityType, num, plyr);
    }

    [ClientRpc]
    public void RpcChangeCommodity(CommodityType commodityType, int num, int plyr)
    {
        int comP = (int)commodityType;
        GameManager.instance.getPlayer(plyr).commodities[comP] += num;
    }

    public void endTurn()
    {
        if (GameManager.instance.getPlayerTurn() != iD) { return; }
        if (GameManager.instance.getGamePhase() == GamePhase.PHASE_ONE) { return; }
        if (moveType == MoveType.SPECIAL) { return; }
        if ((GameManager.instance.getGamePhase() == GamePhase.SETUP_ONE ||
                GameManager.instance.getGamePhase() == GamePhase.SETUP_TWO) &&
                (moveType != MoveType.PLACE_INITIAL_ROAD &&
                moveType != MoveType.PLACE_INITIAL_SHIP &&
                moveType != MoveType.NONE)) { return; }

        moveType = Enums.MoveType.NONE;

        if (GameManager.instance.getGamePhase() == GamePhase.SETUP_ONE)
        {
            moveType = Enums.MoveType.PLACE_INITIAL_CITY;
        }
        special = Enums.Special.NONE;
        movedRoad = false;

        if (progressCards.Count > 4)
        {

            // Those players discard down to 4 progress cards

        }

        // Set some turn specific booleans to false
        foreach (GamePiece piece in pieces)
        {
            if (piece.getPieceType() == Enums.PieceType.KNIGHT)
            {
                Knight k = (Knight)piece;
                k.notActivatedThisTurn();
                k.notUpgradedThisTurn();
            }
            else if (piece.getPieceType() == Enums.PieceType.ROAD)
            {
                Road r = (Road)piece;
                r.notBuiltThisTurn();
            }
        }

        CmdEndTurn();
    }

    [Command]
    public void CmdEndTurn()
    {

        GameManager.instance.SetPlayerTurn(this.isServer);
    }

    [ClientRpc]
    public void RpcDiceRoll(int iD)
    {
        if (!isLocalPlayer || iD != this.iD)
        {
            return;
        }
        if (iD == this.iD)
        {
            //call UI element forcing dice roll
            //upon dice roll click return call the following method to release event catched in DiceRolled
            Debug.Log("Your Turn");
            CmdDiceRoll();
        }
        else
        {
            Debug.Log("Player " + iD + "'s turn");
            //UI element notifying other players that the game is waiting for the dice to be rolled
        }
    }

    [Command]
    public void CmdDiceRoll()
    {
        GameManager.instance.DiceRolled(isServer);
    }

    public void DiceRolled(int first, int second, int third)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //call UI element displaying results of die roll  
        Debug.Log("Dicerolled " + first + " " + second + " " + third);
    }

    public void BarbarianAttacked(bool win, int[] winners)
    {
        Debug.Log("Winners");
    }

    // Commands to move manager
    [Command]
    public void CmdPlaceInitialSettlement(Vector3 location)
    {
        MoveManager.instance.placeInitialSettlement(BoardState.instance.vertexPosition[location], this.pieces, this.isServer);
    }

    [Command]
    public void CmdPlaceInitialCity(Vector3 location)
    {
        MoveManager.instance.placeInitialCity(BoardState.instance.vertexPosition[location], this.pieces, this.isServer);
    }

    [Command]
    public void CmdPlaceInitialRoad(Vector3 location)
    {
        MoveManager.instance.placeInitialRoad(BoardState.instance.edgePosition[location], this.myColor, this.pieces, this.isServer);
    }

    [Command]
    public void CmdPlaceInitialShip(Vector3 location)
    {
        MoveManager.instance.placeInitialShip(BoardState.instance.edgePosition[location], this.myColor, this.pieces, this.isServer);
    }

    [Command]
    public void CmdBuildSettlement(Vector3 location)
    {
        MoveManager.instance.buidSettlement(BoardState.instance.vertexPosition[location], this.resources, this.pieces, this.myColor, this.isServer);
    }

    [Command]
    public void CmdBuildRoad(Vector3 location)
    {
        MoveManager.instance.buildRoad(BoardState.instance.edgePosition[location], this.resources, this.pieces, this.myColor, this.isServer);
    }

    [Command]
    public void CmdFishRoad(Vector3 location)
    {
        MoveManager.instance.fishRoad(BoardState.instance.edgePosition[location], this.numFish, this.pieces, this.myColor, this.isServer);
    }

    [Command]
    public void CmdMoveKnight(Vector3 location)
    {
        MoveManager.instance.moveKnight(v1, BoardState.instance.vertexPosition[location], this.myColor, this.isServer);
    }

    [Command]
    public void CmdDisplaceKnight(Vector3 location)
    {
        MoveManager.instance.displaceKnight(v1, BoardState.instance.vertexPosition[location], this.myColor, this.isServer);
    }

    [Command]
    public void CmdUpgradeKnight(Vector3 location)
    {
        MoveManager.instance.upgradeKnight(this.resources, this.devFlipChart, BoardState.instance.vertexPosition[location], this.pieces, this.myColor, this.isServer);
    }

    [Command]
    public void CmdActivateKnight(Vector3 location)
    {
        MoveManager.instance.activateKnight(this.resources, BoardState.instance.vertexPosition[location], this.myColor, this.isServer);
    }

    [Command]
    public void CmdUpgradeDevelopmentChart(Enums.DevChartType dev)
    {
        MoveManager.instance.upgradeDevChart(dev, this.commodities, this.pieces, this.devFlipChart, this.isServer);
    }

    [Command]
    public void CmdCrane(Enums.DevChartType dev)
    {
        ProgressCards.instance.crane(dev, this.commodities, this.pieces, this.devFlipChart, this.isServer);
    }

    [Command]
    public void CmdBuildCity(Vector3 location)
    {
        Debug.Log("hello3");
        MoveManager.instance.buildCity(BoardState.instance.vertexPosition[location], this.resources, this.pieces, this.myColor, this.isServer);
    }

    [Command]
    public void CmdBuildCityWall(Vector3 location)
    {
        MoveManager.instance.buildCityWall(BoardState.instance.vertexPosition[location], this.resources, this.cityWallsLeft, this.myColor, this.isServer);
    }

    [Command]
    public void CmdBuildKnight(Vector3 location)
    {
        MoveManager.instance.buildKnight(BoardState.instance.vertexPosition[location], this.resources, this.pieces, this.myColor, this.isServer);
    }

    [Command]
    public void CmdBuildShip(Vector3 location)
    {
        MoveManager.instance.buildShip(BoardState.instance.edgePosition[location], this.resources, this.pieces, this.myColor, this.isServer);
    }

    [Command]
    public void CmdMoveShip(Vector3 location)
    {
        MoveManager.instance.moveShip(e1, BoardState.instance.edgePosition[location], this.myColor, this.isServer);
    }

    [Command]
    public void CmdChaseRobber(Vector3 location)
    {
        MoveManager.instance.chaseRobber(v1, BoardState.instance.hexPosition[location], this.myColor, this.isServer);
    }

    [Command]
    public void CmdAlternateDisplaceKnight(Vector3 location)
    {
        MoveManager.instance.alternateDisplaceKnight(BoardState.instance.vertexPosition[location], i1, b1, myColor, isServer);
    }

    [Command]
    public void CmdMoveRobber(Vector3 location)
    {
        MoveManager.instance.moveRobber(BoardState.instance.hexPosition[location], isServer);
    }

    [Command]
    public void CmdBishop(Vector3 location)
    {
        ProgressCards.instance.bishop(BoardState.instance.hexPosition[location], this.myColor, isServer);
    }

    [Command]
    public void CmdDiplomat(Vector3 location)
    {
        ProgressCards.instance.diplomat(e1, BoardState.instance.edgePosition[location], this.myColor, isServer);
    }

    [Command]
    public void CmdRoadBuilding(Vector3 location)
    {
        ProgressCards.instance.roadBuilding(BoardState.instance.edgePosition[location], this.pieces, this.myColor, isServer);
    }

    [Command]
    public void CmdIntrigue(Vector3 location)
    {
        ProgressCards.instance.intrigue(BoardState.instance.vertexPosition[location], this.myColor, isServer);
    }

    [Command]
    public void CmdEngineer(Vector3 location)
    {
        ProgressCards.instance.engineer(BoardState.instance.vertexPosition[location], this.cityWallsLeft, this.myColor, isServer);
    }

    [Command]
    public void CmdMedicine(Vector3 location)
    {
        Debug.Log("cmdmedicine " + location);
        Debug.Log("cmdmedicine " + BoardState.instance.vertexPosition[location]);
        ProgressCards.instance.medicine(BoardState.instance.vertexPosition[location], this.resources, this.pieces, this.myColor, isServer);
    }

    [Command]
    public void CmdInventor(Vector3 location)
    {
        ProgressCards.instance.inventor(h1, BoardState.instance.hexPosition[location], isServer);
    }

    [Command]
    public void CmdMovePirate(Vector3 location)
    {
        MoveManager.instance.movePirate(BoardState.instance.hexPosition[location], isServer);
    }

    [Command]
    public void CmdDestroyCity(Vector3 location)
    {
        MoveManager.instance.destroyCity(BoardState.instance.vertexPosition[location], myColor, isServer);
    }

    [Command]
    public void CmdChooseMetropolis(Vector3 location)
    {
        MoveManager.instance.chooseMetropolis(BoardState.instance.vertexPosition[location], myColor, getMetropolis(), isServer);
    }

    [Command]
    public void CmdRemoveRobber()
    {
        MoveManager.instance.removeRobber(isServer);
    }

    [Command]
    public void CmdRemovePirate()
    {
        MoveManager.instance.removePirate(isServer);
    }

    [Command]//call from Player on client to spawn a trade on all machines 
    public void CmdSpawnTrade(int[] resourcesOffered, int[] resourcesDemanded, int[] commoditiesOffered, int[] commoditiesDemanded, int goldOffered, int goldDemanded, int playerId)
    {
        List<Player> players = GameManager.instance.getPlayers();
        Player offering = null;
        foreach (Player player in players)
        {
            if (player.iD == playerId)
            {
                offering = player;
                break;
            }
        }
        GameObject trade = (GameObject)GameObject.Instantiate(tradePrefab);
        trade.GetComponent<Trades>().init(resourcesOffered, resourcesDemanded, commoditiesOffered, commoditiesDemanded, goldOffered, goldDemanded, offering);
        NetworkServer.Spawn(trade);
        trade.GetComponent<Trades>().RpcSetPlayer(offering.netId);
    }

    public void acceptTrade(Trades trade)
    {
        Player offered = trade.getPlayerOffering();
        for (int i = 0; i < trade.getResourcesOffered().Count; i++)
        {
            offered.changeResource((Enums.ResourceType)i, -trade.getResourcesOffered()[i], offered.iD);
            offered.changeResource((Enums.ResourceType)i, trade.getResourcesWanted()[i], offered.iD);
            this.changeResource((Enums.ResourceType)i, trade.getResourcesOffered()[i], this.iD);
            this.changeResource((Enums.ResourceType)i, -trade.getResourcesWanted()[i], this.iD);
        }
        for (int i = 0; i < trade.getCommoditiesOffered().Count; i++)
        {
            offered.changeCommodity((Enums.CommodityType)i, -trade.getCommoditiesOffered()[i], offered.iD);
            offered.changeCommodity((Enums.CommodityType)i, trade.getCommoditiesWanted()[i], offered.iD);
            this.changeCommodity((Enums.CommodityType)i, trade.getCommoditiesOffered()[i], this.iD);
            this.changeCommodity((Enums.CommodityType)i, -trade.getCommoditiesWanted()[i], this.iD);
        }
        offered.changeGoldCount(trade.getGoldWanted() - trade.getGoldOffered(), offered.iD);
        this.changeGoldCount(trade.getGoldOffered() - trade.getGoldWanted(), this.iD);
        CmdDestroyObject(trade.netId);//destroy the trade 
    }

    [Command]
    public void CmdDestroyObject(NetworkInstanceId netId)
    {
        GameObject theObject = NetworkServer.FindLocalObject(netId);
        NetworkServer.Destroy(theObject);
    }

}