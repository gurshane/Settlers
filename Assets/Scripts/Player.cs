using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums; 

public class Player : MonoBehaviour {

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
    private Enums.Color myColor;

    private int victoryPoints;
    private int safeCardCount;
	private int cityWallsLeft;
    private bool movedRoad;
    
    public List<GamePiece> getNotOnBoardPiece()
    {
        List<GamePiece> notOnBoard = new List<GamePiece>();
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].isOnBoard()==false)
            {
                notOnBoard.Add(pieces[i]);
            }
        }
        return notOnBoard;
    }

    public List<GamePiece> getGamePieces()
    {
        return this.pieces;
    }

    public List<ProgressCardName> getProgressCards()
    {
        return this.progressCards;
    }

    public int[] getResources()
    {
        return this.resources; 
    }

    public int[] getCommodities()
    {
        return this.commodities;
    }

	public int[] getResourceRatios()
	{
		return this.resourceRatios; 
	}

	public int[] getCommodityRatios()
	{
		return this.commodityRatios;
	}

    public int [] getDevFlipChart()
    {
        return this.devFlipChart;
    }

    public int getVictoryCounts()
    {
        return this.victoryPoints;
    }

	public bool decrementVictoryPoints(int num) {
		if (this.victoryPoints < num) {
			return false;
		}
		this.victoryPoints -= num;
		return true;
	}

	public void incrementVictoryPoints(int num) {
		this.victoryPoints += num;
	}

    public int getGoldCount()
    {
        return this.goldCount;
    }

	public bool decrementGoldCount(int num) {
		if (this.goldCount < num) {
			return false;
		}
		this.goldCount -= num;
		return true;
	}

	public void incrementGoldCount(int num) {
		this.goldCount += num;
	}

    public int getSafeCardCount()
    {
        return this.safeCardCount;
    }

    public string getUserName()
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
        int devPosition = (int)devChartType;
        this.devFlipChart[devPosition]++;
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
        int resPosition = (int)resourceType;
        resourceRatios[resPosition] = newRatio;
        return;
    }

    public void updateResoureRatios(int [] newRatios)
    {
        resourceRatios = newRatios;
    }

    public void updateCommodityRatio(CommodityType commodity, int newRatio)
    {
        int comPosition = (int)commodity;
        commodityRatios[comPosition] = newRatio;
        return;
    }

    public void updateCommodityRatios(int [] newRatios)
    {
        commodityRatios = newRatios;
    }

    public bool discardCommodity(CommodityType commodityType, int numToRemove)
    {
		int comPosition = (int)commodityType;
		if (commodities[comPosition] < numToRemove)
		{
			return false;
		}
		commodities[comPosition] -= numToRemove;
		return true;
    }

    public void addCommodity(CommodityType commodityType, int numToAdd)
    {
        int comPosition = (int)commodityType;
        commodities[comPosition] += numToAdd;
    }

    public void addResource(Enums.ResourceType resourceType, int numToAdd)
    {
        int resPosition = (int)resourceType;
        resources[resPosition] += numToAdd;
    }

    public bool discardProgressCard(ProgressCardName cardName)
    {
        for (int i = 0; i<progressCards.Count; i++)
        {
            if ((int)cardName == (int)progressCards[i])
            {
                progressCards.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public bool discardResource(ResourceType resource, int numToRemove)
    {
        int resPosition = (int)resource;
        if (resources[resPosition] < numToRemove)
        {
            return false;
        }
        resources[resPosition] -= numToRemove;
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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
