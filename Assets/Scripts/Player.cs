using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public GameObject myHud;

    private List<GamePiece> pieces;
    private List<ProgressCardName> progressCards;

    private int[] resources;
    private int[] commodities;
    private int goldCount;

    private int[] devFlipChart;
    private int[] resourceRatios;
    private int[] commodityRatios;

    private string userName;
    private string password;
	private Enums.Status status;
    public Enums.Color myColor;
    
    public int victoryPoints;

    private int safeCardCount;
	private int cityWallsLeft;
    private bool movedRoad;
    private bool myTurn;

    private Dictionary<Vector3, GamePiece> spawnedPieces;

    void Start()
    {
        spawnedPieces = new Dictionary<Vector3, GamePiece>();

        //initialize gamePieces list with 
        pieces = new List<GamePiece>();
        /*
         * 4 cities
5 settlement
15 roads
3 city walls (represented by an integer in player)
15 ships*/

        progressCards = new List<ProgressCardName>();
        
        resources = new int[7];
        resources[(int)Enums.ResourceType.BRICK] = 0;
        resources[(int)Enums.ResourceType.WOOL] = 0;
        resources[(int)Enums.ResourceType.GRAIN] = 0;
        resources[(int)Enums.ResourceType.ORE] = 0;
        resources[(int)Enums.ResourceType.LUMBER] = 0;

        commodities = new int[5];
        goldCount = 0;
        
        devFlipChart = new int[4];
        resourceRatios = new int[7];
        commodityRatios = new int[5];
        
        
        password = "";
        
        status = Enums.Status.ACTIVE;

        safeCardCount = 7;
        cityWallsLeft = 3;

        movedRoad = false;

        if(isLocalPlayer)
        {
            gameObject.name = Network.player.ipAddress;
            userName = gameObject.name;
            Instantiate(myHud);
            myTurn = false;
        }
    }

    public void Update()
    {
        if(isLocalPlayer)
        {
            //myTurn = GetComponent<GameManager>().getCurrentPlayer().Equals(gameObject.name);
            
        }
    }

    public void SetColor(Enums.Color color)
    {
        myColor = color;
        Debug.Log(color);
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

	public bool decrementVictoryPoints(int num) {//decrease victory points
		if (this.victoryPoints < num) {
			return false;
		}
		this.victoryPoints -= num;
		return true;
	}

	public void incrementVictoryPoints(int num) {//increase vicctory points
		this.victoryPoints += num;
	}

    public int getGoldCount()//return gold count
    {
        return this.goldCount;
    }

	public bool decrementGoldCount(int num) {//decrease gold count
		if (this.goldCount < num) {
			return false;
		}
		this.goldCount -= num;
		return true;
	}

	public void incrementGoldCount(int num) {//increase gold count
		this.goldCount += num;
	}

    public int getSafeCardCount()//get safe card count (number of cards possible to carry in a hand)
    {
        return this.safeCardCount;
    }

    public string getUserName()//return username
    {
        return this.userName;
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
    }

    void upgradeDevChart(Enums.DevChartType devChartType)
    {
        int devPosition = (int)devChartType;//casting a enum into an int returns the 0 based position of that enum specific
        this.devFlipChart[devPosition]++;//access the devFlipChart at the position found above and increment it
    }

    public void addProgressCard(ProgressCardName cardName)
    {
        progressCards.Add(cardName);
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

	public void incrementCityWallCount() {
		this.cityWallsLeft++;
	}

	public bool decrementCityWallCount() {
		if (this.cityWallsLeft <= 0) {
			return false;
		}
		this.cityWallsLeft++;
		return true;
	}

    public void updateResourceRatio(ResourceType resourceType, int newRatio)
    {
        int resPosition = (int)resourceType;//find index of resource Enum and set an int to this index
        resourceRatios[resPosition] = newRatio;//access the array at this index and set the new ratio
        return;
    }

    public void updateResoureRatios(int [] newRatios)
    {
        resourceRatios = newRatios;
    }

    public void updateCommodityRatio(CommodityType commodity, int newRatio)
    {
        int comPosition = (int)commodity;
        commodityRatios[comPosition] = newRatio;//same as method above
        return;
    }

    public void updateCommodityRatios(int [] newRatios)
    {
        commodityRatios = newRatios;
    }

    public bool discardCommodity(CommodityType commodityType, int numToRemove)
    {
		int comPosition = (int)commodityType;
		if (commodities[comPosition] < numToRemove)//check if there are enough commodities
		{
			return false;//if there arent return false to denote an error
		}
		commodities[comPosition] -= numToRemove;//if there are decrease the number of commodities
		return true;
    }

    public void addCommodity(CommodityType commodityType, int numToAdd)
    {
        int comPosition = (int)commodityType;//casting to find index in enum 
        commodities[comPosition] += numToAdd;//access index of enum and add numToAdd number of commodities
    }

    public void addResource(Enums.ResourceType resourceType, int numToAdd)
    {
        int resPosition = (int)resourceType;//same as above function
        resources[resPosition] += numToAdd;
    }

    public bool discardProgressCard(ProgressCardName cardName)
    {
        for (int i = 0; i<progressCards.Count; i++)//cycle through progress cards
        {
            if ((int)cardName == (int)progressCards[i])//find progress card that has been selected to be discarded
            {
                progressCards.RemoveAt(i);//discard from list
                return true;// return confirmation of found progress card
            }
        }
        return false;// if did not find progress card of the parameter type, return false denoting error
    }

    public bool discardResource(ResourceType resource, int numToRemove)
    {
        int resPosition = (int)resource;
        if (resources[resPosition] < numToRemove)
        {
            return false;
        }
        resources[resPosition] -= numToRemove;//ykno
        return true;
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
    
}
