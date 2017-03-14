using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bank : NetworkBehaviour {
    
	public int[] resources;
    public int[] commodities;

	private List<Enums.ProgressCardName> tradeCards;
	private List<Enums.ProgressCardName> politicsCards;
	private List<Enums.ProgressCardName> scienceCards;


    void Start()
    {
        resources = new int[GameManager.instance.getNumberResources()];
        for (int i = 0; i < resources.Length; i++)
        {
            resources[i] = 19;
        }

        commodities = new int[GameManager.instance.getNumberCommodities()];
        for (int i = 0; i < commodities.Length; i++)
        {
            commodities[i] = 12;
        }

        scienceCards = new List<Enums.ProgressCardName>();
        scienceCards.Add(Enums.ProgressCardName.ALCHEMIST);
        scienceCards.Add(Enums.ProgressCardName.ALCHEMIST);
        scienceCards.Add(Enums.ProgressCardName.CRANE);
        scienceCards.Add(Enums.ProgressCardName.CRANE);
        scienceCards.Add(Enums.ProgressCardName.ENGINEER);
        scienceCards.Add(Enums.ProgressCardName.INVENTOR);
        scienceCards.Add(Enums.ProgressCardName.INVENTOR);
        scienceCards.Add(Enums.ProgressCardName.IRRIGATION);
        scienceCards.Add(Enums.ProgressCardName.IRRIGATION);
        scienceCards.Add(Enums.ProgressCardName.MEDICINE);
        scienceCards.Add(Enums.ProgressCardName.MEDICINE);
        scienceCards.Add(Enums.ProgressCardName.MINING);
        scienceCards.Add(Enums.ProgressCardName.MINING);
        scienceCards.Add(Enums.ProgressCardName.SMITH);
        scienceCards.Add(Enums.ProgressCardName.SMITH);
        scienceCards.Add(Enums.ProgressCardName.PRINTER);
        scienceCards.Add(Enums.ProgressCardName.ROADBUILDING);
        scienceCards.Add(Enums.ProgressCardName.ROADBUILDING);

        politicsCards = new List<Enums.ProgressCardName>();
        politicsCards.Add(Enums.ProgressCardName.BISHOP);
        politicsCards.Add(Enums.ProgressCardName.BISHOP);
        politicsCards.Add(Enums.ProgressCardName.CONSTITUTION);
        politicsCards.Add(Enums.ProgressCardName.DESERTER);
        politicsCards.Add(Enums.ProgressCardName.DESERTER);
        politicsCards.Add(Enums.ProgressCardName.DIPLOMAT);
        politicsCards.Add(Enums.ProgressCardName.DIPLOMAT);
        politicsCards.Add(Enums.ProgressCardName.INTRIGUE);
        politicsCards.Add(Enums.ProgressCardName.INTRIGUE);
        politicsCards.Add(Enums.ProgressCardName.SABOTEUR);
        politicsCards.Add(Enums.ProgressCardName.SABOTEUR);
        politicsCards.Add(Enums.ProgressCardName.SPY);
        politicsCards.Add(Enums.ProgressCardName.SPY);
        politicsCards.Add(Enums.ProgressCardName.SPY);
        politicsCards.Add(Enums.ProgressCardName.WARLORD);
        politicsCards.Add(Enums.ProgressCardName.WARLORD);
        politicsCards.Add(Enums.ProgressCardName.WEDDING);
        politicsCards.Add(Enums.ProgressCardName.WEDDING);

        tradeCards = new List<Enums.ProgressCardName>();
        tradeCards.Add(Enums.ProgressCardName.COMMERCIALHARBOR);
        tradeCards.Add(Enums.ProgressCardName.COMMERCIALHARBOR);
        tradeCards.Add(Enums.ProgressCardName.MASTERMERCHANT);
        tradeCards.Add(Enums.ProgressCardName.MASTERMERCHANT);
        tradeCards.Add(Enums.ProgressCardName.MERCHANT);
        tradeCards.Add(Enums.ProgressCardName.MERCHANT);
        tradeCards.Add(Enums.ProgressCardName.MERCHANT);
        tradeCards.Add(Enums.ProgressCardName.MERCHANT);
        tradeCards.Add(Enums.ProgressCardName.MERCHANT);
        tradeCards.Add(Enums.ProgressCardName.MERCHANT);
        tradeCards.Add(Enums.ProgressCardName.MERCHANTFLEET);
        tradeCards.Add(Enums.ProgressCardName.MERCHANTFLEET);
        tradeCards.Add(Enums.ProgressCardName.RESOURCEMONOPOLY);
        tradeCards.Add(Enums.ProgressCardName.RESOURCEMONOPOLY);
        tradeCards.Add(Enums.ProgressCardName.RESOURCEMONOPOLY);
        tradeCards.Add(Enums.ProgressCardName.RESOURCEMONOPOLY);
        tradeCards.Add(Enums.ProgressCardName.TRADEMONOPOLY);
        tradeCards.Add(Enums.ProgressCardName.TRADEMONOPOLY);

        scienceCards.Sort(delegate (Enums.ProgressCardName a, Enums.ProgressCardName b) {
            return (Random.Range(-10.0f, 10.0f).CompareTo(Random.Range(-10.0f, 10.0f)));
        });
        politicsCards.Sort(delegate (Enums.ProgressCardName a, Enums.ProgressCardName b) {
            return (Random.Range(-10.0f, 10.0f).CompareTo(Random.Range(-10.0f, 10.0f)));
        });
        tradeCards.Sort(delegate (Enums.ProgressCardName a, Enums.ProgressCardName b) {
            return (Random.Range(-10.0f, 10.0f).CompareTo(Random.Range(-10.0f, 10.0f)));
        });
    }

    public int getResourceAmount(Enums.ResourceType res)
    {
		return resources [(int)res];
	}

	public int getCommodityAmount(Enums.CommodityType com)
    {
		return commodities [(int)com];
	}

	public bool withdrawResource(Enums.ResourceType res, int amount)
    {
		if (resources [(int)res] < amount)
        {
			return false;
		}
        CmdDecrementResources(res, amount);
		return true;
	}

    [Command]
    void CmdDecrementResources(Enums.ResourceType res, int amount)
    {
        RpcDecrementResources(res, amount);
    }

    [ClientRpc]
    void RpcDecrementResources(Enums.ResourceType res, int amount)
    {
        resources[(int)res] -= amount;
    }

	public bool withdrawCommodity(Enums.CommodityType com, int amount)
    {
		if (commodities [(int)com] < amount)
        {
			return false;
		}
        CmdDecrementCommodities(com, amount);
		return true;
	}

    [Command]
    void CmdDecrementCommodities(Enums.CommodityType com, int amount)
    {
        RpcDecrementCommodities(com, amount);
    }

    [ClientRpc]
    void RpcDecrementCommodities(Enums.CommodityType com, int amount)
    {
        commodities[(int)com] -= amount;
    }

    public void depositResource(Enums.ResourceType res, int amount)
    {
        CmdIncreaseResources(res, amount);
	}

    [Command]
    void CmdIncreaseResources(Enums.ResourceType res, int amount)
    {
        RpcIncreaseResources(res, amount);
    }

    [ClientRpc]
    void RpcIncreaseResources(Enums.ResourceType res, int amount)
    {
        resources[(int)res] += amount;
    }

	public void depositCommodity(Enums.CommodityType com, int amount)
    {
        CmdIncreaseCommodities(com, amount);
	}

    [Command]
    void CmdIncreaseCommodities(Enums.CommodityType com, int amount)
    {
        RpcIncreaseCommodities(com, amount);
    }

    [ClientRpc]
    void RpcIncreaseCommodities(Enums.CommodityType com, int amount)
    {
        commodities[(int)com] += amount;
    }

	// Put the given progress card on the bottom of a progress card pile
	public void depositProgressCard(Enums.DevChartType progressType, 
		Enums.ProgressCardName progressCard)
    {
        CmdAddProgressCard(progressType, progressCard);
	}

    [Command]
    void CmdAddProgressCard(Enums.DevChartType progressType, Enums.ProgressCardName progressCard)
    {
        RpcAddProgressCard(progressType, progressCard);
    }

    [ClientRpc]
    void RpcAddProgressCard(Enums.DevChartType progressType, Enums.ProgressCardName progressCard)
    {
        if (progressType == Enums.DevChartType.TRADE)
        {
            tradeCards.Add(progressCard);
        }
        else if (progressType == Enums.DevChartType.POLITICS)
        {
            politicsCards.Add(progressCard);
        }
        else if (progressType == Enums.DevChartType.SCIENCE)
        {
            scienceCards.Add(progressCard);
        }
    }

	// Draw and return a progress card from the requested pile
	public Enums.ProgressCardName withdrawProgressCard(Enums.DevChartType progressType)
    {
		Enums.ProgressCardName ret;

		if (progressType == Enums.DevChartType.TRADE)
        {
			ret = tradeCards [0];
		}
        else if (progressType == Enums.DevChartType.POLITICS)
        {
			ret = politicsCards [0];
		}
        else
        {
			ret = scienceCards [0];
		}

        CmdDrawProgressCard(progressType);

        return ret;
    }

    [Command]
    void CmdDrawProgressCard(Enums.DevChartType progressType)
    {
        RpcDrawProgressCard(progressType);
    }

    [ClientRpc]
    void RpcDrawProgressCard(Enums.DevChartType progressType)
    {
        if (progressType == Enums.DevChartType.TRADE)
        {
            tradeCards.RemoveAt(0);
        }
        else if (progressType == Enums.DevChartType.POLITICS)
        {
            politicsCards.RemoveAt(0);
        }
        else
        {
            scienceCards.RemoveAt(0);
        }
    }
		
	// Make sure a given trade is valid
	public bool isValidBankTrade(int[] resRatios, int[] comRatios, Trades trade)
    {
		int totalAvailable = 0;
		int totalWanted = 0;

		// Extract the information from the trade
		int[] resOffered = trade.getResourcesOffered ();
		int[] resWanted = trade.getResourcesWanted ();
		int[] comOffered = trade.getCommoditiesOffered ();
		int[] comWanted = trade.getCommoditiesWanted ();

		// Find the total offered amount
		for (int i = 0; i < resOffered.Length; i++)
        {
			totalAvailable += resOffered [i] / resRatios [i];
		}
		for (int i = 0; i < comOffered.Length; i++)
        {
			totalAvailable += comOffered [i] / comRatios [i];
		}
		totalAvailable += trade.getGoldOffered () / 2;

		// Find the total requested amount
		for (int i = 0; i < resWanted.Length; i++)
        {
			if (resWanted [i] > resources [i])
            {
				return false;
			}
			totalWanted += resWanted [i];
		}
		for (int i = 0; i < comWanted.Length; i++)
        {
			if (comWanted [i] > commodities [i])
            {
				return false;
			}
			totalWanted += comWanted [i];
		}

		// Return true if the requested amount is valid
		if (totalWanted != 0 && totalWanted <= totalAvailable)
        {
			return true;
		}
        else
        {
			return false;
		}
	}

    // Make a trade with the bank
    public bool tradeWithBank(int[] resRatios, int[] comRatios, Trades trade)
    {
        if (!isValidBankTrade(resRatios, comRatios, trade))
        {
            return false;
        }

		// Extract the information from the trade
		int[] resOffered = trade.getResourcesOffered();
		int[] resWanted = trade.getResourcesWanted();
		int[] comOffered = trade.getCommoditiesOffered();
		int[] comWanted = trade.getCommoditiesWanted();

		Player trader = trade.getPlayerOffering();
		int tradeId = trader.getID ();
		int gold = trade.getGoldOffered ();
			
		CmdTradeWithBank (resOffered, resWanted, comOffered, comWanted, tradeId, gold);
        return true;
    }

	[Command]
	public void CmdTradeWithBank (int[] resOffered, int[] resWanted,
		int[] comOffered, int[]comWanted, int tradeId, int gold) 
	{
		RpcTradeWithBank (resOffered, resWanted, comOffered, comWanted, tradeId, gold);
    }

	[ClientRpc]
	public void RpcTradeWithBank (int[] resOffered, int[] resWanted,
		int[] comOffered, int[]comWanted, int tradeId, int gold) 
	{

		Player trader = GameManager.instance.getPlayer (tradeId);

		// Update all relevent fields
		for (int i = 0; i < resOffered.Length; i++)
		{
			trader.changeResource((Enums.ResourceType)i, resOffered[i]);
			depositResource((Enums.ResourceType)i, resOffered[i]);
		}
		for (int i = 0; i < comOffered.Length; i++)
		{
			trader.changeCommodity((Enums.CommodityType)i, comOffered[i]);
			depositCommodity((Enums.CommodityType)i, comOffered[i]);
		}
		for (int i = 0; i < resWanted.Length; i++)
		{
			trader.changeResource((Enums.ResourceType)i, resWanted[i]);
			withdrawResource((Enums.ResourceType)i, resWanted[i]);
		}
		for (int i = 0; i < comWanted.Length; i++)
		{
			trader.changeCommodity((Enums.CommodityType)i, comWanted[i]);
			withdrawCommodity((Enums.CommodityType)i, comWanted[i]);
		}
		trader.changeGoldCount(gold);
	}
}
