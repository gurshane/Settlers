using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public GameObject myHud;

	private MoveAuthorizer ma;
    private Graph graph;

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
    private int numFish;

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

    private Dictionary<Vector3, GamePiece> spawnedPieces;

    //For moves
    public Vertex v1 = null;
    public Edge e1 = null;

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
	public void Init() {
		this.myColor = (Enums.Color)iD;
		foreach (GamePiece piece in pieces) {
			piece.setColor (myColor);
		}
	}

    public void OnLoad()//
    {
        GameManager.instance.Init();
    }

    public int getTotalResources() {
        int ret = 0;
        for (int i = 0; i < 5; i++) {
            ret += resources[i];
        }
        return ret;
    }

    public int getTotalCommodities() {
        int ret = 0;
        for (int i = 0; i < 3; i++) {
            ret += commodities[i];
        }
        return ret;
    }

    public int getHandSize() {
        return getTotalResources() + getTotalCommodities();
    }

    public int maxHandSize() {
        return (3-cityWallsLeft)*2 + 7;
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

        this.resources = new int[5] { 10, 10, 10, 10, 10 };
        this.commodities = new int[3] { 0, 0, 0 };
        this.goldCount = 0;
        this.devFlipChart = new int[3] { 1, 1, 1 };
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

		this.ma = new MoveAuthorizer ();
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
		if(!isLocalPlayer || GameManager.instance.getPlayerTurn() != iD)
        {
			return;   
        }

        if (moveType != MoveType.MOVE_KNIGHT && moveType != MoveType.DISPLACE_KNIGHT && moveType != MoveType.CHASE_ROBBER) {
            v1 = null;
        }

        if (moveType != MoveType.MOVE_SHIP) {
            e1 = null;
        }

		Ray ray;
		RaycastHit impact;
		GameObject pieceHit = null;

        // If game phase is phase one
		if (GameManager.instance.getGamePhase () == Enums.GamePhase.SETUP_ONE) {

            // Get a mouse click
			if (Input.GetButtonDown ("Fire1")) {
				ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out impact)) {
					pieceHit = impact.collider.gameObject;
				}
			}

			if (Object.ReferenceEquals(pieceHit, null)) {
				return;
			}
				
            // Must place first settlement if it hasn't been placed yet
			if (moveType == Enums.MoveType.PLACE_INITIAL_SETTLEMENT) {

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
				Vertex v = pieceHit.GetComponent<Vertex>();

				if (ma.canPlaceInitialTownPiece (v)) {
					CmdPlaceInitialSettlement (v.transform.position);
                    moveType = Enums.MoveType.NONE;
				}

            // Otherwise place first road
			} else if (moveType == Enums.MoveType.PLACE_INITIAL_ROAD) {

				if (!pieceHit.tag.Equals("Edge")) {
					return;
				}
				Edge e = pieceHit.GetComponent<Edge>();

				if (ma.canPlaceInitialRoad(e, this.myColor)) {
					CmdPlaceInitialRoad (e.transform.position);
                    moveType = Enums.MoveType.PLACE_INITIAL_CITY;
				}
			} else if (moveType == Enums.MoveType.PLACE_INITIAL_SHIP) {

				if (!pieceHit.tag.Equals("Edge")) {
					return;
				}
				Edge e = pieceHit.GetComponent<Edge>();

				if (ma.canPlaceInitialShip(e, this.myColor)) {
					CmdPlaceInitialShip (e.transform.position);
                    moveType = Enums.MoveType.PLACE_INITIAL_CITY;
				}
			}
		}

        // Game phase 2
		if (GameManager.instance.getGamePhase () == Enums.GamePhase.SETUP_TWO) {

            // Get mouse click
			if (Input.GetButtonDown ("Fire1")) {
				ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out impact)) {
					pieceHit = impact.collider.gameObject;
				}
			}

			if (Object.ReferenceEquals(pieceHit, null)) {
				return;
			}

            // Must place first settlement if it hasn't been placed yet
			if (moveType == Enums.MoveType.PLACE_INITIAL_CITY) {

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
				Vertex v = pieceHit.GetComponent<Vertex>();

				if (ma.canPlaceInitialTownPiece (v)) {
					CmdPlaceInitialCity (v.transform.position);
                    moveType = Enums.MoveType.NONE;
				}

            // Otherwise place first road
			} else if (moveType == Enums.MoveType.PLACE_INITIAL_ROAD) {

				if (!pieceHit.tag.Equals("Edge")) {
					return;
				}
				Edge e = pieceHit.GetComponent<Edge>();

				if (ma.canPlaceInitialRoad(e, this.myColor)) {
					CmdPlaceInitialRoad (e.transform.position);
                    moveType = Enums.MoveType.NONE;
				}
			} else if (moveType == Enums.MoveType.PLACE_INITIAL_SHIP) {

				if (!pieceHit.tag.Equals("Edge")) {
					return;
				}
				Edge e = pieceHit.GetComponent<Edge>();

				if (ma.canPlaceInitialShip(e, this.myColor)) {
					CmdPlaceInitialShip (e.transform.position);
                    moveType = Enums.MoveType.NONE;
				}
			}
		}

        // Main game phase one
		if (GameManager.instance.getGamePhase () == Enums.GamePhase.PHASE_ONE) {

            // Get mouse click
			if (Input.GetButtonDown ("Fire1")) {
				ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out impact)) {
					pieceHit = impact.collider.gameObject;
				}
			}

			if (Object.ReferenceEquals(pieceHit, null)) {
				return;
			}

            if (special == Enums.Special.MOVE_ROBBER) {
                Debug.Log("robber1");
                if (!pieceHit.tag.Equals("MainHex") && !pieceHit.tag.Equals("IslandHex")) {
					return;
				}
                Hex h = pieceHit.GetComponent<Hex>();
                Debug.Log("robber2");
                if (ma.canMoveRobber(h)) {
                    Debug.Log("robber3");
					CmdMoveRobber (h.transform.position);

                    bool stealable = false;
                    foreach (Hex hex in BoardState.instance.hexPosition.Values) {
                        GamePiece hexPiece = hex.getOccupyingPiece();
                        if (!Object.ReferenceEquals(hexPiece, null)) {
                            if (hexPiece.getPieceType() == PieceType.ROBBER) {
                                foreach (Vertex vert in hex.getVertices()){
                                    GamePiece vertPiece = vert.getOccupyingPiece();
                                    if (!Object.ReferenceEquals(vertPiece, null)) {
                                        if (vertPiece.getColor() != myColor){
                                            if (vertPiece.getPieceType() == PieceType.CITY ||
                                                vertPiece.getPieceType() == PieceType.SETTLEMENT) {

                                                stealable = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (stealable) {
                        setSpecial(Special.STEAL_RESOURCES_ROBBER, getID());
                    } else {
                        setSpecial(Special.NONE, getID());
                        foreach(Player p in GameManager.instance.getPlayers()) {
                            GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                        }
                        revertTurn();
                        endPhaseOne();
                    }
				}
            } else if (special == Enums.Special.MOVE_PIRATE) {
                if (!pieceHit.tag.Equals("WaterHex")) {
					return;
				}
                Hex h = pieceHit.GetComponent<Hex>();

                if (ma.canMovePirate(h)) {
					CmdMovePirate (h.transform.position);

                    bool stealable = false;
                    foreach (Edge edge in BoardState.instance.edgePosition.Values) {
                        GamePiece edgePiece = edge.getOccupyingPiece();
                        if (!Object.ReferenceEquals(edgePiece, null)) {
                            if (edgePiece.getPieceType() == PieceType.ROAD && ((Road)edgePiece).getIsShip()) {
                                Hex leftHex = edge.getLeftHex();
                                if (!Object.ReferenceEquals(leftHex, null)) {
                                    GamePiece leftHexPiece = leftHex.getOccupyingPiece();
                                    if (!Object.ReferenceEquals(leftHexPiece, null)) {
                                        if (leftHexPiece.getPieceType() == PieceType.PIRATE) {

                                            stealable = true;
                                            break;
                                        }
                                    }
                                }
                                Hex rightHex = edge.getRightHex();
                                if (!Object.ReferenceEquals(rightHex, null)) {
                                    GamePiece rightHexPiece = rightHex.getOccupyingPiece();
                                    if (!Object.ReferenceEquals(rightHexPiece, null)) {
                                        if (rightHexPiece.getPieceType() == PieceType.PIRATE) {

                                            stealable = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (stealable) {
                        setSpecial(Special.STEAL_RESOURCES_PIRATE, getID());
                    } else {
                        setSpecial(Special.NONE, getID());
                        foreach(Player p in GameManager.instance.getPlayers()) {
                            GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                        }
                        revertTurn();
                        endPhaseOne();
                    }
				}
            } else if (special == Enums.Special.STEAL_RESOURCES_ROBBER) { 

                if (pieceHit.tag.Equals("Vertex")) {

                    Vertex v = pieceHit.GetComponent<Vertex>();

                    if (ma.canStealRobber(v, myColor)) {
                        int opp = (int)v.getOccupyingPiece().getColor();
                        Player oppo  = GameManager.instance.getPlayer(opp);
                        int hSize = oppo.getHandSize();
                        bool taken = false;
                        for (int i = 0; i < 5; i++) {
                            oppo.changeResource((ResourceType)i, -1);
                            if (oppo.getHandSize() < hSize) {
                                taken = true;
                                changeResource((ResourceType)i, 1);
                                break;
                            }
                        }
                        if (!taken) {
                            for (int i = 0; i < 3; i++) {
                                oppo.changeCommodity((CommodityType)i, -1);
                                if (oppo.getHandSize() < hSize) {
                                    changeCommodity((CommodityType)i, 1);
                                    break;
                                }
                            }
                        }
                        setSpecial(Special.NONE, getID());
                        foreach(Player p in GameManager.instance.getPlayers()) {
                            GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                        }
                        revertTurn();
                        endPhaseOne();
                    }
				}
            } else if (special == Enums.Special.STEAL_RESOURCES_PIRATE) { 
                if (pieceHit.tag.Equals("Edge")) {

                    Edge e = pieceHit.GetComponent<Edge>();

                    if (ma.canStealPirate(e, myColor)) {
                        int opp = (int)e.getOccupyingPiece().getColor();
                        Player oppo  = GameManager.instance.getPlayer(opp);
                        int hSize = oppo.getHandSize();
                        bool taken = false;
                        for (int i = 0; i < 5; i++) {
                            oppo.changeResource((ResourceType)i, -1);
                            if (oppo.getHandSize() < hSize) {
                                taken = true;
                                changeResource((ResourceType)i, 1);
                                break;
                            }
                        }
                        if (!taken) {
                            for (int i = 0; i < 3; i++) {
                                oppo.changeCommodity((CommodityType)i, -1);
                                if (oppo.getHandSize() < hSize) {
                                    changeCommodity((CommodityType)i, 1);
                                    break;
                                }
                            }
                        }
                        setSpecial(Special.NONE, getID());
                        foreach(Player p in GameManager.instance.getPlayers()) {
                            GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                        }
                        revertTurn();
                        endPhaseOne();
                    }
				} 
            }
		}

        // Main game phase two
		if (GameManager.instance.getGamePhase () == Enums.GamePhase.PHASE_TWO) {
            // Get mouse click
			if (Input.GetButtonDown ("Fire1")) {
				ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out impact)) {
					pieceHit = impact.collider.gameObject;
				}
			}

			if (Object.ReferenceEquals(pieceHit, null)) {
				return;
			}

            if (special == Enums.Special.KNIGHT_DISPLACED) {
                bool chosen = false;
                if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (graph.areConnectedVertices (v, v1, myColor))
                {
                    if (Object.ReferenceEquals (v.getOccupyingPiece (), null))
                    {
                        chosen = true;
                    }
                }

                if (chosen) {
					CmdAlternateDisplaceKnight (v.transform.position);

                    foreach (Player p in GameManager.instance.players) {
                        GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                    }
                    revertTurn();
                    moveType = Enums.MoveType.NONE;
				}
            }

            // Must place first settlement if it hasn't been placed yet
			if (moveType == Enums.MoveType.BUILD_SETTLEMENT) {

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
				Vertex v = pieceHit.GetComponent<Vertex>();

				if (ma.canBuildSettlement (v, this.resources, this.pieces, this.myColor)) {
					CmdBuildSettlement (v.transform.position);
                    moveType = Enums.MoveType.NONE;
				}

            // Otherwise place first road
			} else if (moveType == Enums.MoveType.BUILD_ROAD) {

				if (!pieceHit.tag.Equals("Edge")) {
					return;
				}
				Edge e = pieceHit.GetComponent<Edge>();

				if (ma.canBuildRoad(e, this.resources, this.pieces, this.myColor)) {
					CmdBuildRoad (e.transform.position);
                    moveType = Enums.MoveType.NONE;
				}
			} else if (moveType == Enums.MoveType.MOVE_KNIGHT) {

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (v1 == null) {
                    v1 = v;
                } else {
                    if (ma.canKnightMove(v1, v, this.myColor)) {
                        CmdMoveKnight (v.transform.position);
                        moveType = Enums.MoveType.NONE;
                    }
                }
			} else if (moveType == Enums.MoveType.DISPLACE_KNIGHT) {

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
                Vertex v = pieceHit.GetComponent<Vertex>();

                if (v1 == null) {
                    v1 = v;
                } else {
                    if (ma.canKnightDisplace(v1, v, this.myColor)) {
                        CmdDisplaceKnight (v.transform.position);
                        moveType = Enums.MoveType.NONE;
                    }
                }
			} else if (moveType == Enums.MoveType.UPGRADE_KNIGHT) {

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
				Vertex v = pieceHit.GetComponent<Vertex>();

				if (ma.canUpgradeKnight (this.resources, this.devFlipChart, v, this.pieces, this.myColor)) {
					CmdUpgradeKnight (v.transform.position);
                    moveType = Enums.MoveType.NONE;
				}
			} else if (moveType == Enums.MoveType.ACTIVATE_KNIGHT) {

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
				Vertex v = pieceHit.GetComponent<Vertex>();

				if (ma.canActivateKnight (this.resources, v, this.myColor)) {
					CmdActivateKnight (v.transform.position);
                    moveType = Enums.MoveType.NONE;
				}
			} else if (moveType == Enums.MoveType.BUILD_CITY) {

                Debug.Log("hello4");

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
				Vertex v = pieceHit.GetComponent<Vertex>();
                Debug.Log("hello5");

				if (ma.canBuildCity (v, this.resources, this.pieces, this.myColor)) {
					CmdBuildCity (v.transform.position);
                    moveType = Enums.MoveType.NONE;
				}
			} else if (moveType == Enums.MoveType.BUILD_CITY_WALL) {

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
				Vertex v = pieceHit.GetComponent<Vertex>();

				if (ma.canBuildCityWall (v, this.resources, this.cityWallsLeft, this.myColor)) {
					CmdBuildCityWall (v.transform.position);
                    moveType = Enums.MoveType.NONE;
				}
			} else if (moveType == Enums.MoveType.BUILD_KNIGHT) {

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
				Vertex v = pieceHit.GetComponent<Vertex>();

				if (ma.canBuildKnight (v, this.resources, this.pieces, this.myColor)) {
					CmdBuildKnight (v.transform.position);
                    moveType = Enums.MoveType.NONE;
				}
			} else if (moveType == Enums.MoveType.BUILD_SHIP) {

				if (!pieceHit.tag.Equals("Edge")) {
					return;
				}
				Edge e = pieceHit.GetComponent<Edge>();

				if (ma.canBuildShip(e, this.resources, this.pieces, this.myColor)) {
                    Debug.Log("ship1");
					CmdBuildShip (e.transform.position);
                    moveType = Enums.MoveType.NONE;
				}
			} else if (moveType == Enums.MoveType.CHASE_ROBBER) {

                if (v1 == null) {
                    if (!pieceHit.tag.Equals("Vertex")) {
                        return;
                    }
                    Vertex v = pieceHit.GetComponent<Vertex>();
                    v1 = v;
                } else {
                    if (!pieceHit.tag.Equals("MainHex") && !pieceHit.tag.Equals("MainHex")) {
                        return;
                    }
                    Hex h = pieceHit.GetComponent<Hex>();
                    if (ma.canChaseRobber(v1, this.myColor)) {
                        CmdChaseRobber (h.transform.position);
                        moveType = Enums.MoveType.NONE;
                    }
                }
			} else if (moveType == Enums.MoveType.MOVE_SHIP) {

				if (!pieceHit.tag.Equals("Edge")) {
					return;
				}
                Edge e = pieceHit.GetComponent<Edge>();

                if (e1 == null) {
                    e1 = e;
                } else {
                    if (ma.canShipMove(e1, e, this.myColor)) {
                        Debug.Log("heywhatsup");
                        CmdMoveShip (e.transform.position);
                        moveType = Enums.MoveType.NONE;
                    }
                }
			}
        }
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
            if (pieces[i].isOnBoard()==false)
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

    public int [] getDevFlipChart()//return array of progress in the development flip chart
    {
        return this.devFlipChart;
    }

    public int getVictoryCounts()//return victory points
    {
        return this.victoryPoints;
    }

    public bool changeVictoryPoints(int num)
    {//decrease victory points
        //if (!isLocalPlayer)
            //return false;
        if (this.victoryPoints + num < 0)
        {
            return false;
        }
        CmdChangeVP(num);
        return true;
    }

    [Command]
    public void CmdChangeVP(int num)
    {
        this.victoryPoints += num;
    }

    public bool changeFishCount(int num)
    {
        if((this.numFish + num) < 0)
        {
            return false;
        }
        CmdChangeFishCount(num);
        return true;
    }

    [Command]
    public void CmdChangeFishCount(int num)
    {
        this.numFish += num;
    }

    public int getGoldCount()//return gold count
    {
        return this.goldCount;
    }

    public bool changeGoldCount(int num)
    {//decrease gold count
        if (this.goldCount + num < 0)
        {
            return false;
        }
        CmdChangeGold(num);
        return true;
    }

    [Command]
    public void CmdChangeGold(int num)
    {
        this.goldCount += num;
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

    public void setMoveType(Enums.MoveType mType, int plyr) {
        CmdSetMoveType(mType, plyr);
    }

    [Command]
    public void CmdSetMoveType(Enums.MoveType mType, int plyr)
    {
        GameManager.instance.getPlayer(plyr).moveType = mType; 
    }

    public Enums.Special getSpecial()
    {
        return this.special; 
    }

    public void setSpecial(Enums.Special spec, int plyr) {
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

    public void setOldTurn(int turn) {
        CmdSetOldTurn(turn);
    }

    [Command]
    public void CmdSetOldTurn(int turn)
    {
        foreach (Player p in GameManager.instance.getPlayers()) {
            p.oldTurn = turn;
         }
    }

    public int getI1()
    {
        return this.i1; 
    }

    public void setI1(int i, int plyr) {
        CmdSetI1(i, plyr);
    }

    [Command]
    public void CmdSetI1(int i, int plyr)
    {
        GameManager.instance.getPlayer(plyr).i1 = i;
    }

    public int getOpponent()
    {
        return this.opponent; 
    }

    public void setOpponent(int i, int plyr) {
        CmdSetOpponent(i, plyr);
    }

    [Command]
    public void CmdSetOpponent(int i, int plyr)
    {
        GameManager.instance.getPlayer(plyr).opponent = i;
    }

    public bool getB1()
    {
        return this.b1; 
    }

    public void setB1(bool b, int plyr) {
        CmdSetB1(b, plyr);
    }

    [Command]
    public void CmdSetB1(bool b, int plyr)
    {
        GameManager.instance.getPlayer(plyr).b1 = b;
    }

    public void setSpecialTurn(int turn) {
        CmdSetSpecialTurn(turn);
    }

    [Command]
    public void CmdSetSpecialTurn(int turn)
    {
        RpcSetSpecialTurn(turn);
    }

    [ClientRpc]
    public void RpcSetSpecialTurn(int turn) {
        GameManager.instance.setSpecialTurn(turn, isServer);
    }

    public void revertTurn() {
        Debug.Log("prevert1" + GameManager.instance.getPlayerTurn());
        CmdRevertTurn();
    }

    [Command]
    public void CmdRevertTurn()
    {
        RpcRevertTurn();
    }

    [ClientRpc]
    public void RpcRevertTurn() {
        GameManager.instance.setSpecialTurn(oldTurn, isServer);
        Debug.Log("prevert" + GameManager.instance.getPlayerTurn()); 
    }

    public void endPhaseOne() {
        Debug.Log("epo1");
        CmdEndPhaseOne(isServer);
    }

    [Command]
    public void CmdEndPhaseOne(bool server)
    {
        Debug.Log("epo2");
        RpcEndPhaseOne(server);
    }

    [ClientRpc]
    public void RpcEndPhaseOne(bool server) {
        Debug.Log("epo3");
        GameManager.instance.setGamePhase(GamePhase.PHASE_TWO, server);
    }

    public void SetV1(Vertex vReplace)
    {
        CmdSetV1(vReplace.transform.position);
    }

    [Command]
    public void CmdSetV1(Vector3 vReplace)
    {
        RpcSetV1(vReplace);
    }

    [ClientRpc]
    public void RpcSetV1(Vector3 vReplace)
    {
        v1 = BoardState.instance.vertexPosition[vReplace];
    }

    public void SetE1(Edge eReplace)
    {
        CmdSetE1(eReplace.transform.position);
    }

    [Command]
    public void CmdSetE1(Vector3 eReplace)
    {
        RpcSetE1(eReplace);
    }

    [ClientRpc]
    public void RpcSetE1(Vector3 eReplace)
    {
        e1 = BoardState.instance.edgePosition[eReplace];
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

	public int getCityWallCount() {
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

    public void makeAqueduct()
    {
        this.aqueduct = true;
    }

    public void upgradeDevChart(Enums.DevChartType devChartType)
    {
        CmdUpgradeDevChart(devChartType);
    }

    [Command]
    public void CmdUpgradeDevChart(Enums.DevChartType devChartType)
    {
        RpcUpgradeDevChart(devChartType);

    }

    [ClientRpc]
    public void RpcUpgradeDevChart(Enums.DevChartType devChartType)
    {
        int devPosition = (int)devChartType;
        this.devFlipChart[devPosition]++;
    }

    public void updateResourceRatios(int[] newRatios)
    {
        resourceRatios = newRatios;
        CmdUpdateResourceRatios(newRatios);
    }

    [Command]
    void CmdUpdateResourceRatios(int[] newRatios)
    {
        RpcUpdateResourceRatios(newRatios);
    }

    [ClientRpc]
    void RpcUpdateResourceRatios(int[] newRatios)
    {
        this.resourceRatios = newRatios;
    }

    public bool changeResource(ResourceType resource, int num)
    {
        int resPosition = (int)resource;
        if (resources[resPosition] + num < 0)
        {
            return false;
        }
        CmdChangeResource(resource, num);
        return true;
    }

    [Command]
    public void CmdChangeResource(ResourceType resourceType, int num)
    {
        RpcChangeResource(resourceType, num);
    }

    [ClientRpc]
    public void RpcChangeResource(ResourceType resourceType, int num)
    {
        int resP = (int)resourceType;
        this.resources[resP] += num;
    }

    public void addProgressCard(ProgressCardName cardName)
    {
        CmdAddProgressCard(cardName);
    }

    [Command]
    public void CmdAddProgressCard(ProgressCardName cardName)
    {
        RpcAddProgressCard(cardName);
    }

    [ClientRpc]
    public void RpcAddProgressCard(ProgressCardName cardName)
    {
        this.progressCards.Add(cardName);
    }

    public void changeSafeCardCount(int count)
    {
        this.safeCardCount += count;
    }

    public bool changeCityWallCount(int num)
    {
        if (this.cityWallsLeft <= 0)
        {
            return false;
        }
        CmdChangeCityWalls(num);
        return true;
    }

    [Command]
    public void CmdChangeCityWalls(int num)
    {
        this.cityWallsLeft += num;
    }

    public void updateResourceRatio(ResourceType resourceType, int newRatio)
    {
        CmdUpdateResourceRatio(resourceType, newRatio);
        return;
    }

    [Command]
    public void CmdUpdateResourceRatio(ResourceType resourceType, int newRatio)
    {
        RpcUpdateResourceRatio(resourceType, newRatio);
    }

    [ClientRpc]
    public void RpcUpdateResourceRatio(ResourceType resourceType, int newRatio)
    {
        int resP = (int)resourceType;
        this.resourceRatios[resP] = newRatio;
    }

    public void updateCommodityRatio(CommodityType commodity, int newRatio)
    {
        CmdUpdateCommodityRatio(commodity, newRatio);
        return;
    }

    [Command]
    public void CmdUpdateCommodityRatio(CommodityType commodityType, int newRatio)
    {
        RpcUpdateCommodityRatio(commodityType, newRatio);
    }

    [ClientRpc]
    public void RpcUpdateCommodityRatio(CommodityType commodityType, int newRatio)
    {
        int comP = (int)commodityType;
        this.commodityRatios[comP] = newRatio;
    }
    

    public bool changeCommodity(CommodityType commodityType, int num)
    {
        int comPosition = (int)commodityType;
        if (commodities[comPosition] + num < 0)//check if there are enough commodities
        {
            return false;//if there arent return false to denote an error
        }
        CmdChangeCommodity(commodityType, num);
        return true;
    }

    [Command]
    public void CmdChangeCommodity(CommodityType commodityType, int num)
    {
        RpcChangeCommodity(commodityType, num);
    }

    [ClientRpc]
    public void RpcChangeCommodity(CommodityType commodityType, int num)
    {
        int comP = (int)commodityType;
        this.commodities[comP] += num;
    }

    public void endTurn() {
        if (GameManager.instance.getPlayerTurn() != iD) { return; }
        if (GameManager.instance.getGamePhase() == GamePhase.PHASE_ONE) { return; }
        if (moveType == MoveType.SPECIAL) { return; }

        if (moveType != MoveType.PLACE_INITIAL_CITY) {
            moveType = Enums.MoveType.NONE;
        }
        special = Enums.Special.NONE;
        v1 = null;
        e1 = null;
        i1 = 0;
        b1 = false;
        movedRoad = false;

        if (progressCards.Count > 4) {

            // Those players discard down to 4 progress cards

        }

        // Set some turn specific booleans to false
        foreach (GamePiece piece in pieces) {
            if (piece.getPieceType () == Enums.PieceType.KNIGHT) {
                Knight k = (Knight)piece;
                k.notActivatedThisTurn ();
                k.notUpgradedThisTurn ();
            } else if (piece.getPieceType () == Enums.PieceType.ROAD) {
                Road r = (Road)piece;
                r.notBuiltThisTurn ();
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
	public void CmdPlaceInitialSettlement(Vector3 location) {
		MoveManager.instance.placeInitialSettlement (BoardState.instance.vertexPosition [location], this.pieces, this.isServer);
	}

	[Command]
	public void CmdPlaceInitialCity(Vector3 location) {
		MoveManager.instance.placeInitialCity (BoardState.instance.vertexPosition [location], this.pieces, this.isServer);
	}

	[Command]
	public void CmdPlaceInitialRoad(Vector3 location) {
		MoveManager.instance.placeInitialRoad (BoardState.instance.edgePosition [location], this.myColor, this.pieces, this.isServer);
	}

	[Command]
	public void CmdPlaceInitialShip(Vector3 location) {
		MoveManager.instance.placeInitialShip (BoardState.instance.edgePosition [location], this.myColor, this.pieces, this.isServer);
	}

    [Command]
	public void CmdBuildSettlement(Vector3 location) {
		MoveManager.instance.buidSettlement (BoardState.instance.vertexPosition [location], this.resources, this.pieces, this.myColor, this.isServer);
	}

	[Command]
	public void CmdBuildRoad(Vector3 location) {
		MoveManager.instance.buildRoad (BoardState.instance.edgePosition [location], this.resources, this.pieces, this.myColor, this.isServer);
	}

    [Command]
	public void CmdMoveKnight(Vector3 location) {
		MoveManager.instance.moveKnight(v1, BoardState.instance.vertexPosition [location], this.myColor, this.isServer);
	}

    [Command]
	public void CmdDisplaceKnight(Vector3 location) {
		MoveManager.instance.displaceKnight(v1, BoardState.instance.vertexPosition [location], this.myColor, this.isServer);
	}

    [Command]
	public void CmdUpgradeKnight(Vector3 location) {
		MoveManager.instance.upgradeKnight(this.resources, this.devFlipChart, BoardState.instance.vertexPosition [location], this.pieces, this.myColor, this.isServer);
	}

    [Command]
	public void CmdActivateKnight(Vector3 location) {
		MoveManager.instance.activateKnight(this.resources, BoardState.instance.vertexPosition [location], this.myColor, this.isServer);
	}

    [Command]
    public void CmdUpgradeDevelopmentChart(Enums.DevChartType dev) {
        MoveManager.instance.upgradeDevChart(dev, this.commodities, this.pieces, this.devFlipChart, this.isServer);
    }

    [Command]
	public void CmdBuildCity(Vector3 location) {
        Debug.Log("hello3");
		MoveManager.instance.buildCity (BoardState.instance.vertexPosition [location], this.resources, this.pieces, this.myColor, this.isServer);
	}

    [Command]
	public void CmdBuildCityWall(Vector3 location) {
		MoveManager.instance.buildCityWall (BoardState.instance.vertexPosition [location], this.resources, this.cityWallsLeft, this.myColor, this.isServer);
	}

    [Command]
	public void CmdBuildKnight(Vector3 location) {
		MoveManager.instance.buildKnight (BoardState.instance.vertexPosition [location], this.resources, this.pieces, this.myColor, this.isServer);
	}

    [Command]
	public void CmdBuildShip(Vector3 location) {
		MoveManager.instance.buildShip (BoardState.instance.edgePosition [location], this.resources, this.pieces, this.myColor, this.isServer);
	}

    [Command]
	public void CmdMoveShip(Vector3 location) {
		MoveManager.instance.moveShip (e1, BoardState.instance.edgePosition [location], this.myColor, this.isServer);
	}

    [Command]
    public void CmdChaseRobber(Vector3 location) {
		MoveManager.instance.chaseRobber (v1, BoardState.instance.hexPosition [location], this.myColor, this.isServer);        
    }

    [Command]
    public void CmdAlternateDisplaceKnight(Vector3 location) {
		MoveManager.instance.alternateDisplaceKnight (BoardState.instance.vertexPosition [location], i1, b1, myColor, isServer);        
    }

    [Command]
    public void CmdMoveRobber(Vector3 location) {
		MoveManager.instance.moveRobber (BoardState.instance.hexPosition [location], isServer);        
    }

    [Command]
    public void CmdMovePirate(Vector3 location) {
		MoveManager.instance.movePirate (BoardState.instance.hexPosition [location], isServer);        
    }
}
