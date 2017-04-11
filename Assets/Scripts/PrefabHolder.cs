using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabHolder : MonoBehaviour {

    public GameObject settlement;
    public GameObject city;
    public GameObject levelOneKnight;
    public GameObject levelTwoKnight;
    public GameObject levelThreeKnight;
    public GameObject cityWall;
    public GameObject road;
    public GameObject metropolis;
    public GameObject boat;
    public GameObject pirate;
    public GameObject robber;
    public GameObject cityWithCityWall;
    public GameObject metropolisWithCityWall;

    public List<Material> materials;

    static public PrefabHolder instance = null;

	void Awake()
    {
        instance = this;
    }

}
