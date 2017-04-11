using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Trades : NetworkBehaviour
{

    private SyncListInt resourcesOffered = new SyncListInt();
    private SyncListInt commoditiesOffered = new SyncListInt();
    private SyncListInt resourcesWanted = new SyncListInt();
    private SyncListInt commoditiesWanted = new SyncListInt();
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

    [Server]
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
        Player theObject = NetworkServer.FindLocalObject(iD).GetComponent<Player>();
        offering = theObject;
    }

    void Start()//spawn checker 
    {
        if (isClient || isServer)
        {
            Debug.Log("Spawned Trade on Client; Gold Offered: " + goldOffered + " Gold Demanded: " + goldWanted);
        }
    }
}
