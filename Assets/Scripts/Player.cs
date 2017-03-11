using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public GameObject myHud;

    public GameManager gm;

    [SyncVar]
    private int iD;

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

    private Dictionary<Vector3, GamePiece> spawnedPieces;

    public void Init(int iD)
    {
        this.iD = iD;
        Debug.Log("Initiated Player: " + iD);
        gm = GameObject.FindObjectOfType<GameManager>();
        Debug.Log(NetworkClient.active);
        if (NetworkClient.active)
        {
            gm.EventDiceRolled += DiceRolled;
            gm.EventBarbarianAttack += BarbarianAttacked;
            gm.EventNextPlayer += NextPlayerTurn;
        }
    }

    void Start()
    {
        spawnedPieces = new Dictionary<Vector3, GamePiece>();

        iD = -1;

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


        if (isLocalPlayer)
        {
            gameObject.name = Network.player.ipAddress;
            Instantiate(myHud);
        }
    }

    public void Update()
    {
        if(isLocalPlayer)
        {
            
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
        if (!isLocalPlayer)
            return false;
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

    public void startTurn()
    {
        Status status = Status.ACTIVE;
        gm.CmdStartTurn();
        //enable HUD actions 
    }
    [Command]
    public void CmdEndTurn()
    {
        gm.CmdSetPlayerTurn();
    }

    [ClientRpc]
    public void RpcDiceRoll(int iD)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (iD == this.iD)
        {
            //call UI element forcing dice roll
            //upon dice roll click return call the following method to release event catched in DiceRolled
            Debug.Log("Your Turn");
            gm.CmdDiceRolled();
        }
        else
        {
            Debug.Log("Player " + iD + "'s turn");
            //UI element notifying other players that the game is waiting for the dice to be rolled
        }
    }

    public void DiceRolled(int first, int second, int third)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //call UI element displaying results of die roll  
        Debug.Log(" " + first + " " + second + " " + third);
        this.CmdEndTurn();
    }

    public void NextPlayerTurn()
    {
        if (!isLocalPlayer)
            return;
        Debug.Log(gm.getPlayerTurn());
        if (gm.getPlayerTurn() == iD)
        {
            startTurn();
        }
    }

    public void BarbarianAttacked(bool win, int[] winners)
    {
        Debug.Log("Winners");
    }
}
