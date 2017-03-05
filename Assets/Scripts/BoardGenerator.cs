using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BoardGenerator : NetworkBehaviour {

    //Spawn Locations
    private List<GameObject> mainBoardHexSpawns;
    private List<GameObject> islandHexSpawns;

    //Hex Prefabs
    public GameObject goldHex;
    public GameObject forestHex;
    public GameObject pastureHex;
    public GameObject mountainHex;
    public GameObject hillHex;
    public GameObject dessertHex;
    public GameObject fieldHex;

    //Number Tiles
    public List<GameObject> numberPieces;

    //Edges
    private List<GameObject> edges;

    //Vertices
    private List<GameObject> vertices;

    //Spawned versions of the hexes, number tiles, water hexes
    private List<GameObject> spawned;
    private List<GameObject> spawnedNumbers;
    private List<GameObject> spawnedMainBoardHexes;
    private List<GameObject> spawnedIslandHexes;

    private bool doOnce;

    void Start()
    {
        doOnce = true;
    }
    // Use this for initialization
    void LateUpdate ()
    {
        if(doOnce && isServer)
        {
            doOnce = false;

            spawned = new List<GameObject>();
            spawnedNumbers = new List<GameObject>();
            spawnedMainBoardHexes = new List<GameObject>();
            spawnedIslandHexes = new List<GameObject>();

            edges = new List<GameObject>(GameObject.FindGameObjectsWithTag("Edge"));
            vertices = new List<GameObject>(GameObject.FindGameObjectsWithTag("Vertex"));

            mainBoardHexSpawns = new List<GameObject>(GameObject.FindGameObjectsWithTag("MainHex"));
            islandHexSpawns = new List<GameObject>(GameObject.FindGameObjectsWithTag("IslandHex"));


            makeBoard();
        }
	}

	void makeBoard()
    {
        //Spawn Main Board Hexes
        //----------------------
        //Spawn Forest
        spawnHex(mainBoardHexSpawns, forestHex, 4, true);
        //Spawn Fields
        spawnHex(mainBoardHexSpawns, fieldHex, 4, true);
        //Spawn Hills
        spawnHex(mainBoardHexSpawns, hillHex, 3, true);
        //Spawn Dessert
        spawnHex(mainBoardHexSpawns, dessertHex, 1, true);
        //Spawn Mountain 
        spawnHex(mainBoardHexSpawns, mountainHex, 3, true);
        //Spawn Pasture 
        spawnHex(mainBoardHexSpawns, pastureHex, 4, true);


        //Spawn Island Hexes
        //------------------
        //Spawn Gold
        spawnHex(islandHexSpawns, goldHex, 1, false);
        //Spawn Forest
        spawnHex(islandHexSpawns, forestHex, 1, false);
        //Spawn Fields
        spawnHex(islandHexSpawns, fieldHex, 1, false);
        //Spawn Hills
        spawnHex(islandHexSpawns, hillHex, 1, false);
        //Spawn Dessert
        spawnHex(islandHexSpawns, dessertHex, 1, false);
        //Spawn Mountain 
        spawnHex(islandHexSpawns, mountainHex, 1, false);
        //Spawn Pasture 
        spawnHex(islandHexSpawns, pastureHex, 2, false);
    }

    void spawnHex(List<GameObject> spawnPositions, GameObject hexToSpawn, int numToSpawn, bool isOnMainBoard)
    {
       for( int i = 0; i < numToSpawn; i++)
        {
            GameObject targetSpawn = (spawnPositions[Random.Range(0, spawnPositions.Count)]);
            Transform targetTransform = targetSpawn.transform;
            GameObject spawnedHex = Instantiate(hexToSpawn, targetTransform.position, Quaternion.identity, targetTransform);

            GameObject targetNumber = numberPieces[Random.Range(0, numberPieces.Count)];
            GameObject spawnedNumber = Instantiate(targetNumber, targetTransform.position, Quaternion.identity, targetTransform);

            Hex hex = targetSpawn.gameObject.GetComponent<Hex>();
            string name = hexToSpawn.name;
            switch (name)
            {
                case "waterTile":
                    hex.hexType = Enums.HexType.WATER;
                    hex.resourceType = Enums.ResourceType.NONE;
                    break;
                case "brickTile":
                    hex.hexType = Enums.HexType.HILL;
                    hex.resourceType = Enums.ResourceType.BRICK;
                    break;
                case "desertTile":
                    hex.hexType = Enums.HexType.DESERT;
                    hex.resourceType = Enums.ResourceType.NONE;
                    break;
                case "goldTile":
                    hex.hexType = Enums.HexType.GOLD;
                    hex.resourceType = Enums.ResourceType.NONE;
                    break;
                case "oreTile":
                    hex.hexType = Enums.HexType.MOUNTAIN;
                    hex.resourceType = Enums.ResourceType.ORE;
                    break;
                case "sheepTile":
                    hex.hexType = Enums.HexType.PASTURE;
                    hex.resourceType = Enums.ResourceType.WOOL;
                    break;
                case "wheatTile":
                    hex.hexType = Enums.HexType.FIELD;
                    hex.resourceType = Enums.ResourceType.GRAIN;
                    break;
                case "woodTile":
                    hex.hexType = Enums.HexType.FOREST;
                    hex.resourceType = Enums.ResourceType.LUMBER;
                    break;
            }

            string number = targetNumber.name;
            switch (number)
            {
                case "one":
                    hex.hexNumber = 1;
                    break;
                case "two":
                    hex.hexNumber = 2;
                    break;
                case "three":
                    hex.hexNumber = 3;
                    break;
                case "four":
                    hex.hexNumber = 4;
                    break;
                case "five":
                    hex.hexNumber = 5;
                    break;
                case "six":
                    hex.hexNumber = 6;
                    break;
                case "seven":
                    hex.hexNumber = Random.Range(1, 6);
                    break;
                case "eight":
                    hex.hexNumber = 8;
                    break;
                case "nine":
                    hex.hexNumber = 9;
                    break;
                case "ten":
                    hex.hexNumber = 10;
                    break;
                case "eleven":
                    hex.hexNumber = 11;
                    break;
                case "twelve":
                    hex.hexNumber = 12;
                    break;
            }

            if (isOnMainBoard)
            {
                spawnedMainBoardHexes.Add(spawnedHex);
            }
            else
            {
                spawnedIslandHexes.Add(spawnedHex);
            }

            NetworkServer.Spawn(spawnedHex);
            NetworkServer.Spawn(targetNumber);
            spawnPositions.Remove(targetSpawn);
        }
    }
}
