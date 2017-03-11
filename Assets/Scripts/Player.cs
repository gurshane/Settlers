using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;
using System;

public class Player : NetworkBehaviour {

    private int iD;

    private List<GamePiece> pieces;
    private List<ProgressCardName> progressCards;

    private int[] resources;//dont need syncVar can update on method call;
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

    public void init(int iD, string name, string pass)
    {
        this.iD = iD;
        this.userName = name;
        this.password = pass;
        this.resources = new int[4] { 0, 0, 0, 0 };
        this.commodities = new int[3] { 0, 0, 0 };
        this.goldCount = 0;
        this.devFlipChart = new int[3] { 0, 0, 0 };
        this.resourceRatios = new int[4] { 4, 4, 4, 4 };
        this.commodityRatios = new int[4] { 4, 4, 4, 4 };
        this.myColor = (Enums.Color) Enum.Parse(typeof(Enums.Color), this.iD.ToString());
        this.victoryPoints = 0;
        this.safeCardCount = 7;
        this.cityWallsLeft = 0;
        this.movedRoad = false; 
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

	public bool changeVictoryPoints(int num) {//decrease victory points
        if (!isLocalPlayer)
            return false;
        if (this.victoryPoints + num < 0) {
			return false;
		}
		this.victoryPoints += num;
        CmdChangeVP(num);
		return true;
	}

	/*public void incrementVictoryPoints(int num) {//increase vicctory points
        if (!isLocalPlayer)
            return;
        this.victoryPoints += num;
        CmdIncrementVP(num);
	}*/

    [Command]
    public void CmdChangeVP(int num)
    {
        this.victoryPoints += num;
        RpcChangeVP(num);
    }

    [ClientRpc]
    public void RpcChangeVP(int num)
    {
        this.victoryPoints += num;
    }


    public int getGoldCount()//return gold count
    {
        return this.goldCount;
    }

	public bool changeGoldCount(int num) {//decrease gold count
		if (this.goldCount + num < 0) {
			return false;
		}
		this.goldCount += num;
        CmdChangeGold(num);
		return true;
	}

    [Command]
    public void CmdChangeGold(int num)
    {
        this.goldCount += num;
        RpcChangeGold(num);
    }

    [ClientRpc]
    public void RpcChangeGold(int num)
    {
        this.goldCount += num;
    }
	/*public void incrementGoldCount(int num) {//increase gold count
		this.goldCount += num;
	}*/

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
        CmdSetStatus(newStatus);
    }

    [Command]
    public void CmdSetStatus(Status newStatus)
    {
        this.status = newStatus;
        RpcSetStatus(newStatus);
    }

    [ClientRpc]
    public void RpcSetStatus(Status newStatus)
    {
        this.status = newStatus;
    }

    public void upgradeDevChart(Enums.DevChartType devChartType)
    {
        int devPosition = (int)devChartType;//casting a enum into an int returns the 0 based position of that enum specific
        this.devFlipChart[devPosition]++;//access the devFlipChart at the position found above and increment it
        CmdUpgradeDevChart(devChartType);
    }

    [Command]
    public void CmdUpgradeDevChart(Enums.DevChartType devChartType)
    {
        int devPosition = (int)devChartType;
        this.devFlipChart[devPosition]++;
        RpcUpgradeDevChart(devChartType);

    }

    [ClientRpc]
    public void RpcUpgradeDevChart(Enums.DevChartType devChartType)
    {
        int devPosition = (int)devChartType;
        this.devFlipChart[devPosition]++;
    }

    public void addProgressCard(ProgressCardName cardName)
    {
        this.progressCards.Add(cardName);
        CmdAddProgressCard(cardName);
    }

    [Command]
    public void CmdAddProgressCard(ProgressCardName cardName)
    {
        this.progressCards.Add(cardName);
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

    /*public void decreaseSafeCardCount(int count)
    {
        this.safeCardCount -= count;
    }*/

	public int getCityWallCount() {
		return this.cityWallsLeft;
	}

/*	public void incrementCityWallCount() {
		this.cityWallsLeft++;
	}*/

	public bool changeCityWallCount(int num) {
		if (this.cityWallsLeft <= 0) {
			return false;
		}
		this.cityWallsLeft+=num;
        CmdChangeCityWalls(num);
		return true;
	}

    [Command]
    public void CmdChangeCityWalls(int num)
    {
        this.cityWallsLeft += num;
        RpcChangeCityWalls(num);
    }

    [ClientRpc]
    public void RpcChangeCityWalls(int num)
    {
        this.cityWallsLeft += num;
    }

    public void updateResourceRatio(ResourceType resourceType, int newRatio)
    {
        int resPosition = (int)resourceType;//find index of resource Enum and set an int to this index
        this.resourceRatios[resPosition] = newRatio;//access the array at this index and set the new ratio
        CmdUpdateResourceRatio(resourceType, newRatio);
        return;
    }

    [Command]
    public void CmdUpdateResourceRatio(ResourceType resourceType, int newRatio)
    {
        int resP = (int)resourceType;
        this.resourceRatios[resP] = newRatio;
        RpcUpdateResourceRatio(resourceType, newRatio);
    }

    [ClientRpc]
    public void RpcUpdateResourceRatio(ResourceType resourceType, int newRatio)
    {
        int resP = (int)resourceType;
        this.resourceRatios[resP] = newRatio;
    }

    public void updateResoureRatios(int [] newRatios)
    {
        resourceRatios = newRatios;
    }

    public void updateCommodityRatio(CommodityType commodity, int newRatio)
    {
        int comPosition = (int)commodity;
        commodityRatios[comPosition] = newRatio;//same as method above
        CmdUpdateCommodityRatio(commodity, newRatio);
        return;
    }

    [Command]
    public void CmdUpdateCommodityRatio(CommodityType commodityType, int newRatio)
    {
        int comP = (int)commodityType;
        this.commodityRatios[comP] = newRatio;
        RpcUpdateCommodityRatio(commodityType, newRatio);
    }

    [ClientRpc]
    public void RpcUpdateCommodityRatio(CommodityType commodityType, int newRatio)
    {
        int comP = (int)commodityType;
        this.commodityRatios[comP] = newRatio;
    }

    public void updateCommodityRatios(int [] newRatios)
    {
        commodityRatios = newRatios;
    }

    public bool changeCommodity(CommodityType commodityType, int num)
    {
		int comPosition = (int)commodityType;
		if (commodities[comPosition] + num < 0)//check if there are enough commodities
		{
			return false;//if there arent return false to denote an error
		}
		commodities[comPosition] += num;//if there are decrease the number of commodities
        CmdChangeCommodity(commodityType, num);
		return true;
    }

    [Command]
    public void CmdChangeCommodity(CommodityType commodityType, int num)
    {
        int comP = (int)commodityType;
        this.commodities[comP] += num;
        RpcChangeCommodity(commodityType, num);
    }

    [ClientRpc]
    public void RpcChangeCommodity(CommodityType commodityType, int num)
    {
        int comP = (int)commodityType;
        this.commodities[comP] += num;
    }

    /*public void addCommodity(CommodityType commodityType, int numToAdd)
    {
        int comPosition = (int)commodityType;//casting to find index in enum 
        commodities[comPosition] += numToAdd;//access index of enum and add numToAdd number of commodities
    }*/

    /*public void addResource(Enums.ResourceType resourceType, int numToAdd)
    {
        int resPosition = (int)resourceType;//same as above function
        resources[resPosition] += numToAdd;
    }*/

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

    public bool changeResource(ResourceType resource, int num)
    {
        int resPosition = (int)resource;
        if (resources[resPosition] + num < 0)
        {
            return false;
        }
        resources[resPosition] += num;//ykno
        CmdChangeResource(resource, num);
        return true;
    }

    [Command]
    public void CmdChangeResource(ResourceType resourceType, int num)
    {
        int resP = (int)resourceType;
        this.resources[resP] += num;
        RpcChangeResource(resourceType, num);
    }

    [ClientRpc]
    public void RpcChangeResource(ResourceType resourceType, int num)
    {
        int resP = (int)resourceType;
        this.resources[resP] += num;
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
