using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trades : MonoBehaviour {

    private int[] resourcesOffered;
    private int[] commoditiesOffered;
    private int[] resourcesWanted;
    private int[] commoditiesWanted;
    private int goldOffered;
    private int goldWanted;
    private Player offering;

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

    public Player getPlayerOffering()
    {
        return offering;
    }

    public void init()
    {

    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
