using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Trades : MonoBehaviour {

    public int[] resourcesOffered;
    public int[] commoditiesOffered;
    public int[] resourcesWanted;
    public int[] commoditiesWanted;
    public int goldOffered;
    public int goldWanted;
    public int offering;

    public int [] getResourcesOffered()
    {
        return resourcesOffered;
    }

    public int [] getCommoditiesOffered()
    {
        return commoditiesOffered;
    }

    public int[] getResourcesWanted()
    {
        return resourcesWanted;
    }

    public int[] getCommoditiesWanted()
    {
        return commoditiesWanted;
    }

    public int getGoldOffered()
    {
        return goldOffered;
    }

    public int getGoldWanted()
    {
        return goldWanted;
    }

    public int getPlayerOffering()
    {
        return offering;
    }

    public void init()
    {

    }
    /*SyncListInt resourcesOffered;
    SyncListInt commoditiesOffered;
    SyncListInt resourcesWanted;
    SyncListInt commoditiesWanted;
    [SyncVar]
    private int goldOffered;
    [SyncVar]
    private int goldWanted;
    private Player offering;

    public SyncListInt getResourcesOffered()
    {
        return resourcesOffered;
    }

    public SyncListInt getCommoditiesOffered()
    {
        return commoditiesOffered;
    }

    public SyncListInt getResourcesWanted()
    {
        return resourcesWanted;
    }

    public SyncListInt getCommoditiesWanted()
    {
        return commoditiesWanted;
    }

    public int getGoldOffered()
    {
        return goldOffered;
    }

    public int getGoldWanted()
    {
        return goldWanted;
    }

    public Player getPlayerOffering()
    {
        return offering;
    }

    public void init(int[] resourcesOffer, int[] resourcesDemand, int[] commoditiesOffer, int[] commoditiesDemand, int goldOffer, int goldDemand, Player offer)
    {
        for (int i = 0; i < resourcesOffer.Length; i++)
        {
            resourcesOffered.Add(resourcesOffer[i]);
            resourcesWanted.Add(resourcesDemand[i]);
        }
        for (int j = 0; j < commoditiesOffer.Length; j++)
        {
            commoditiesOffered.Add(commoditiesOffer[j]);
            commoditiesWanted.Add(commoditiesDemand[j]);
        }
        goldOffered = goldOffer;
        goldWanted = goldDemand;
        offering = offer;
    }

    [ClientRpc]
    public void RpcSetPlayer(NetworkInstanceId iD)
    {
        Player theObject = NetworkServer.FindLocalObject(netID).GetComponent<Player>();
        offering = theObject;
    }

    [Command]
    public void CmdDestroy(NetworkInstanceId netID)
    {
        GameObject theObject = NetworkServer.FindLocalObject(netID);
        NetworkServer.Destroy(theObject);
    }*/
}
