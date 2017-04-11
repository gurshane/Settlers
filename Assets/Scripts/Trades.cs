using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
