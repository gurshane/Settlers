using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bank : NetworkBehaviour
{

    public int[] resources;
    public int[] commodities;

    public List<FishToken> fishTokens;

    public List<Enums.ProgressCardName> tradeCards;
    public List<Enums.ProgressCardName> politicsCards;
    public List<Enums.ProgressCardName> scienceCards;
    private NetworkIdentity objNetId;

    static public Bank instance = null;

    void Awake()
    {
        instance = this;
    }

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

        generateFishTokens();
    }

    public void generateFishTokens()
    {
        fishTokens = new List<FishToken>();
        fishTokens.Add(new FishToken(false, 1));
        fishTokens.Add(new FishToken(false, 1));
        fishTokens.Add(new FishToken(false, 1));
        fishTokens.Add(new FishToken(false, 1));
        fishTokens.Add(new FishToken(false, 1));
        fishTokens.Add(new FishToken(false, 1));
        fishTokens.Add(new FishToken(false, 1));
        fishTokens.Add(new FishToken(false, 1));
        fishTokens.Add(new FishToken(false, 1));
        fishTokens.Add(new FishToken(false, 1));
        fishTokens.Add(new FishToken(false, 1));

        fishTokens.Add(new FishToken(false, 2));
        fishTokens.Add(new FishToken(false, 2));
        fishTokens.Add(new FishToken(false, 2));
        fishTokens.Add(new FishToken(false, 2));
        fishTokens.Add(new FishToken(false, 2));
        fishTokens.Add(new FishToken(false, 2));
        fishTokens.Add(new FishToken(false, 2));
        fishTokens.Add(new FishToken(false, 2));
        fishTokens.Add(new FishToken(false, 2));
        fishTokens.Add(new FishToken(false, 2));

        fishTokens.Add(new FishToken(false, 3));
        fishTokens.Add(new FishToken(false, 3));
        fishTokens.Add(new FishToken(false, 3));
        fishTokens.Add(new FishToken(false, 3));
        fishTokens.Add(new FishToken(false, 3));
        fishTokens.Add(new FishToken(false, 3));
        fishTokens.Add(new FishToken(false, 3));
        fishTokens.Add(new FishToken(false, 3));

        fishTokens.Add(new FishToken(true, -7));
    }

    public int getFishToken(bool server)
    {
        if (fishTokens.Count < 3)
        {
            generateFishTokens();
        }

        int numFish = fishTokens[0].value;
        
        assignAuthority(server);
        RpcWithdrawFishToken();
        removeAuthority(server);

        return numFish;
    }

    [ClientRpc]
    public void RpcWithdrawFishToken()
    {
        fishTokens.RemoveAt(0);
    }

    public int getFishTokens(bool server)
    {
        if (fishTokens.Count < 3)
        {
            generateFishTokens();
        }

        int numFish = fishTokens[0].value;
        if(numFish != -7)
        {
            numFish += fishTokens[1].value;
        }
        else{}

        assignAuthority(server);
        RpcWithdrawFishTokens();
        removeAuthority(server);

        return numFish;
    }

    [ClientRpc]
    public void RpcWithdrawFishTokens()
    {
        fishTokens.RemoveAt(0);
        fishTokens.RemoveAt(0);
    }

    public int getResourceAmount(Enums.ResourceType res)
    {
        return resources[(int)res];
    }

    public int getCommodityAmount(Enums.CommodityType com)
    {
        return commodities[(int)com];
    }

    public bool withdrawResource(Enums.ResourceType res, int amount, bool server)
    {
        if (resources[(int)res] < amount)
        {
            return false;
        }

        assignAuthority(server);
        RpcDecrementResources(res, amount);
        removeAuthority(server);
        return true;
    }

    [ClientRpc]
    void RpcDecrementResources(Enums.ResourceType res, int amount)
    {
        resources[(int)res] -= amount;
    }

    public bool withdrawCommodity(Enums.CommodityType com, int amount, bool server)
    {
        if (commodities[(int)com] < amount)
        {
            return false;
        }

        assignAuthority(server);
        RpcDecrementCommodities(com, amount);
        removeAuthority(server);
        return true;
    }

    [ClientRpc]
    void RpcDecrementCommodities(Enums.CommodityType com, int amount)
    {
        commodities[(int)com] -= amount;
    }

    public void depositResource(Enums.ResourceType res, int amount, bool server)
    {
        assignAuthority(server);
        RpcIncreaseResources(res, amount);
        removeAuthority(server);
    }

    [ClientRpc]
    void RpcIncreaseResources(Enums.ResourceType res, int amount)
    {
        resources[(int)res] += amount;
    }

    public void depositCommodity(Enums.CommodityType com, int amount, bool server)
    {
        assignAuthority(server);
        RpcIncreaseCommodities(com, amount);
        removeAuthority(server);
    }

    [ClientRpc]
    void RpcIncreaseCommodities(Enums.CommodityType com, int amount)
    {
        commodities[(int)com] += amount;
    }

    // Put the given progress card on the bottom of a progress card pile
    public void depositProgressCard(Enums.DevChartType progressType,
        Enums.ProgressCardName progressCard, bool server)
    {
        assignAuthority(server);
        RpcAddProgressCard(progressType, progressCard);
        removeAuthority(server);
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
    public void withdrawProgressCard(Enums.DevChartType progressType, int player)
    {
        Enums.ProgressCardName prog;
        if (progressType == Enums.DevChartType.TRADE)
        {
            int rand = Random.Range(0, tradeCards.Count);
            prog = tradeCards[rand];
            tradeCards.RemoveAt(rand);
        }
        else if (progressType == Enums.DevChartType.POLITICS)
        {
            int rand = Random.Range(0, politicsCards.Count);
            prog = politicsCards[rand];
            politicsCards.RemoveAt(rand);
        }
        else
        {
            int rand = Random.Range(0, scienceCards.Count);
            prog = scienceCards[rand];
            scienceCards.RemoveAt(rand);
        }
        Player current = GameManager.instance.getPlayer(player);
        GameManager.instance.getPersonalPlayer().addProgressCard(prog, current.getID());
    }

    // Make sure a given trade is valid
    public bool isValidBankTrade(int[] resRatios, int[] comRatios, Trades trade)
    {
        int totalAvailable = 0;
        int totalWanted = 0;

        // Extract the information from the trade
        SyncListInt resOffered = trade.getResourcesOffered();
        SyncListInt resWanted = trade.getResourcesWanted();
        SyncListInt comOffered = trade.getCommoditiesOffered();
        SyncListInt comWanted = trade.getCommoditiesWanted();

        // Find the total offered amount
        for (int i = 0; i < resOffered.Count; i++)
        {
            totalAvailable += resOffered[i] / resRatios[i];
        }
        for (int i = 0; i < comOffered.Count; i++)
        {
            totalAvailable += comOffered[i] / comRatios[i];
        }
        totalAvailable += trade.getGoldOffered() / 2;

        // Find the total requested amount
        for (int i = 0; i < resWanted.Count; i++)
        {
            if (resWanted[i] > resources[i])
            {
                return false;
            }
            totalWanted += resWanted[i];
        }
        for (int i = 0; i < comWanted.Count; i++)
        {
            if (comWanted[i] > commodities[i])
            {
                return false;
            }
            totalWanted += comWanted[i];
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

    public bool isValidBankTrade(int[] resRatios, int[] comRatios, Trade trade)
    {
        int totalAvailable = 0;
        int totalWanted = 0;

        // Extract the information from the trade
        int[] resOffered = trade.resourcesOffered;
        int[] resWanted = trade.resourcesWanted;
        int[] comOffered = trade.commoditiesOffered;
        int[] comWanted = trade.commoditiesWanted;

        // Find the total offered amount
        for (int i = 0; i < resOffered.Length; i++)
        {
            totalAvailable += resOffered[i] / resRatios[i];
        }
        for (int i = 0; i < comOffered.Length; i++)
        {
            totalAvailable += comOffered[i] / comRatios[i];
        }
        totalAvailable += trade.goldOffered / 2;

        // Find the total requested amount
        for (int i = 0; i < resWanted.Length; i++)
        {
            if (resWanted[i] > resources[i])
            {
                return false;
            }
            totalWanted += resWanted[i];
        }
        for (int i = 0; i < comWanted.Length; i++)
        {
            if (comWanted[i] > commodities[i])
            {
                return false;
            }
            totalWanted += comWanted[i];
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
        SyncListInt resOffered = trade.getResourcesOffered();
        SyncListInt resWanted = trade.getResourcesWanted();
        SyncListInt comOffered = trade.getCommoditiesOffered();
        SyncListInt comWanted = trade.getCommoditiesWanted();

        Player trader = GameManager.instance.getCurrentPlayer();
        int tradeId = trader.getID();
        int gold = trade.getGoldOffered();

        // Update all relevent fields
        for (int i = 0; i < resOffered.Count; i++)
        {
            GameManager.instance.getPersonalPlayer().changeResource((Enums.ResourceType)i, resOffered[i], trader.getID());
            depositResource((Enums.ResourceType)i, resOffered[i], trader.isServer);
        }
        for (int i = 0; i < comOffered.Count; i++)
        {
            GameManager.instance.getPersonalPlayer().changeCommodity((Enums.CommodityType)i, comOffered[i], trader.getID());
            depositCommodity((Enums.CommodityType)i, comOffered[i], trader.isServer);
        }
        for (int i = 0; i < resWanted.Count; i++)
        {
            GameManager.instance.getPersonalPlayer().changeResource((Enums.ResourceType)i, resWanted[i], trader.getID());
            withdrawResource((Enums.ResourceType)i, resWanted[i], trader.isServer);
        }
        for (int i = 0; i < comWanted.Count; i++)
        {
            GameManager.instance.getPersonalPlayer().changeCommodity((Enums.CommodityType)i, comWanted[i], trader.getID());
            withdrawCommodity((Enums.CommodityType)i, comWanted[i], trader.isServer);
        }
        GameManager.instance.getPersonalPlayer().changeGoldCount(gold, trader.getID());

        return true;
    }

    public bool tradeWithBank(int[] resRatios, int[] comRatios, Trade trade)
    {
        if (!isValidBankTrade(resRatios, comRatios, trade))
        {
            return false;
        }

        // Extract the information from the trade
        int[] resOffered = trade.resourcesOffered;
        int[] resWanted = trade.resourcesWanted;
        int[] comOffered = trade.commoditiesOffered;
        int[] comWanted = trade.commoditiesWanted;

        Player trader = GameManager.instance.getCurrentPlayer();
        int tradeId = trader.getID();
        int gold = trade.goldOffered;

        // Update all relevent fields
        for (int i = 0; i < resOffered.Length; i++)
        {
            GameManager.instance.getPersonalPlayer().changeResource((Enums.ResourceType)i, -1 * resOffered[i], trader.getID());
            depositResource((Enums.ResourceType)i, resOffered[i], trader.isServer);
        }
        for (int i = 0; i < comOffered.Length; i++)
        {
            GameManager.instance.getPersonalPlayer().changeCommodity((Enums.CommodityType)i, -1 * comOffered[i], trader.getID());
            depositCommodity((Enums.CommodityType)i, comOffered[i], trader.isServer);
        }

        for (int i = 0; i < resWanted.Length; i++)
        {
            GameManager.instance.getPersonalPlayer().changeResource((Enums.ResourceType)i, resWanted[i], trader.getID());
            withdrawResource((Enums.ResourceType)i, resWanted[i], trader.isServer);
        }
        for (int i = 0; i < comWanted.Length; i++)
        {
            GameManager.instance.getPersonalPlayer().changeCommodity((Enums.CommodityType)i, comWanted[i], trader.getID());
            withdrawCommodity((Enums.CommodityType)i, comWanted[i], trader.isServer);
        }
        GameManager.instance.getPersonalPlayer().changeGoldCount(gold, trader.getID());

        return true;
    }

    private void assignAuthority(bool server)
    {
        if (!server)
        {
            objNetId = this.GetComponent<NetworkIdentity>();
            objNetId.AssignClientAuthority(connectionToClient);
        }
    }

    private void removeAuthority(bool server)
    {
        if (!server)
        {
            objNetId.RemoveClientAuthority(connectionToClient);
        }
    }
}
