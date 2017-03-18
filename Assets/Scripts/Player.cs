using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public GameObject myHud;

    public GameManager gm;

	public MoveManager mm;

	private MoveAuthorizer ma;

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
    private int safeCardCount;

    private List<GamePiece> pieces;
    private List<ProgressCardName> progressCards;

    private int[] resources;
    private int[] commodities;
    private int[] devFlipChart;
    private int[] resourceRatios;
    private int[] commodityRatios;
    
    private int cityWallsLeft;
    private bool movedRoad;
    private bool aqueduct;

	[SyncVar]
	public bool placedFirstTown;

	[SyncVar]
	public bool placedFirstEdge;

    private Dictionary<Vector3, GamePiece> spawnedPieces;

	[Command]
    public void CmdInit(int iD)//call this if server is loaded after player
    {
        this.iD = iD;
		this.myColor = (Enums.Color)iD;
        Debug.Log("Initiated Player: " + iD);
		Debug.Log ("MYCOLOR: " + this.myColor);
    }

    // Final initialization step, assign colors, game manager, movemanager
	public void Init() {
		getGm();
		mm = GameObject.FindWithTag("MoveManager").GetComponent<MoveManager>();
		mm.Init ();
		this.myColor = (Enums.Color)iD;
		foreach (GamePiece piece in pieces) {
			piece.setColor (myColor);
		}
	}
		
    public void getGm()
    {
        Debug.Log("is Client: " + isClient);
        Debug.Log("Is Server: " + isServer);
        Debug.Log(iD + "iD isLocal: " + isLocalPlayer);
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        Debug.Log(gm);
        Debug.Log("networkclient" + NetworkClient.active);
        /*if (NetworkClient.active)
        {
            gm.EventDiceRolled += DiceRolled;
            gm.EventBarbarianAttack += BarbarianAttacked;
            gm.EventNextPlayer += NextPlayerTurn;
            gm.EventFirstTurn += FirstTurn;
            gm.EventSecondTurn += SecondTurn;
        }*/
    }

    public void OnLoad()//
    {
        getGm();
		if (gm == null)//check if gameManager has been spawned yet
        {
            return;
        }
        
        gm.Init();
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

        this.resources = new int[5] { 0, 0, 0, 0, 0 };
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
		this.placedFirstTown = false;
		this.placedFirstEdge = false;

		this.ma = new MoveAuthorizer ();


        if (isLocalPlayer)
        {
            
            gameObject.name = Network.player.ipAddress;
            Instantiate(myHud);
        }
        OnLoad();

    }

    public void Update()
    {
		if(!isLocalPlayer || gm.getPlayerTurn() != iD)
        {
			return;   
        }

		Ray ray;
		RaycastHit impact;
		GameObject pieceHit = null;

        // Space means end turn
		if (Input.GetKeyDown (KeyCode.Space)) {
			placedFirstEdge = false;
			placedFirstTown = false;
			CmdEndTurn ();
		}

        // I = initialize if its necessary to do it again
		if (Input.GetKeyDown (KeyCode.I)) {
			Init ();
		}

        // If game phase is phase one
		if (gm.getGamePhase () == Enums.GamePhase.SETUP_ONE) {

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
			if (!placedFirstTown) {

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
				Vertex v = pieceHit.GetComponent<Vertex>();

				if (ma.canPlaceInitialTownPiece (v)) {
					CmdPlaceInitialSettlement (v.transform.position);
					placedFirstTown = true;
				}

            // Otherwise place first road
			} else if (!placedFirstEdge) {

				if (!pieceHit.tag.Equals("Edge")) {
					return;
				}
				Edge e = pieceHit.GetComponent<Edge>();

				if (ma.canPlaceInitialRoad(e, this.myColor)) {
					CmdPlaceInitialRoad (e.transform.position);
					placedFirstEdge = true;
				}
			}
		}

        // Game phase 2
		if (gm.getGamePhase () == Enums.GamePhase.SETUP_TWO) {

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

            // Must place city if not done yet
			if (!placedFirstTown) {

				if (!pieceHit.tag.Equals("Vertex")) {
					return;
				}
				Vertex v = pieceHit.GetComponent<Vertex>();

				if (ma.canPlaceInitialTownPiece (v)) {
					CmdPlaceInitialCity (v.transform.position);
					placedFirstTown = true;
				}

            // Otherwise place second road
			} else if (!placedFirstEdge) {
				Debug.Log ("Before Check");
				if (!pieceHit.tag.Equals("Edge")) {
					return;
				}
				Debug.Log ("After Check");

				Edge e = pieceHit.GetComponent<Edge>();

				if (ma.canPlaceInitialRoad (e, this.myColor)) {
					CmdPlaceInitialRoad (e.transform.position);
					placedFirstEdge = true;
				}
			}
		}

        // Main game phase one
		if (gm.getGamePhase () == Enums.GamePhase.PHASE_ONE) {
		}

        // Main game phase two
		if (gm.getGamePhase () == Enums.GamePhase.PHASE_TWO) {
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
		this.victoryPoints += num;
        //CmdChangeVP(num);
        return true;
    }

    [Command]
    public void CmdChangeVP(int num)
    {
        this.victoryPoints += num;
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

    [Command]
    public void CmdStartTurn()
    {
        Status status = Status.ACTIVE;
        gm.StartTurn();
    }



    [Command]
    public void CmdEndTurn()
    {
		gm.SetPlayerTurn(this.isServer);
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
        gm.DiceRolled();
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

    public void NextPlayerTurn()
    {
        if (!isLocalPlayer || gm.getPlayerTurn() != iD)
        {
            return;
        }
        Debug.Log("nextplayerturn" + gm.getPlayerTurn());
        CmdStartTurn();
    }

    public void BarbarianAttacked(bool win, int[] winners)
    {
        Debug.Log("Winners");
    }

    public void FirstTurn()
    {
        if (!isLocalPlayer || gm.getPlayerTurn() != iD)
        {
            return;
        }
        Debug.Log("Player first turn for " + iD);
        //place Piece
        //place Road
    }

    public void SecondTurn()
    {
        if (!isLocalPlayer || gm.getPlayerTurn() != iD)
        {
            return;
        }
        Debug.Log("Player second turn for " + iD);
        //place City
        //place Road 
    }
		
    // Commands to move manager
	[Command]
	public void CmdPlaceInitialSettlement(Vector3 location) {
		mm.placeInitialSettlement (BoardState.instance.vertexPosition [location], this.pieces, this.isServer);
	}

	[Command]
	public void CmdPlaceInitialCity(Vector3 location) {
		mm.placeInitialCity (BoardState.instance.vertexPosition [location], this.pieces, this.isServer);
	}

	[Command]
	public void CmdPlaceInitialRoad(Vector3 location) {
		mm.placeInitialRoad (BoardState.instance.edgePosition [location], this.myColor, this.pieces, this.isServer);
	}

	[Command]
	public void CmdPlaceInitialShip(Vector3 location) {
		mm.placeInitialShip (BoardState.instance.edgePosition [location], this.myColor, this.pieces, this.isServer);
	}
}
