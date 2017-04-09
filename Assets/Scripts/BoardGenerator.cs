using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BoardGenerator : NetworkBehaviour {

    //Spawn Locations
    public List<GameObject> lakeSpawnLocations;

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

    public GameObject robber;

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

    private Dictionary<char, int> numberTiles;

    private Hex robberHex;
    private Hex extraHex;

    void Start()
    {
        doOnce = true;
        numberTiles = new Dictionary<char, int>();
        numberTiles.Add('A', 5);
        numberTiles.Add('B', 2);
        numberTiles.Add('C', 6);
        numberTiles.Add('D', 3);
        numberTiles.Add('E', 8);
        numberTiles.Add('F', 10);
        numberTiles.Add('G', 9);
        numberTiles.Add('H', 12);
        numberTiles.Add('I', 11);
        numberTiles.Add('J', 4);
        numberTiles.Add('K', 8);
        numberTiles.Add('L', 10);
        numberTiles.Add('M', 9);
        numberTiles.Add('N', 4);
        numberTiles.Add('O', 5);
        numberTiles.Add('P', 6);
        numberTiles.Add('Q', 3);
        numberTiles.Add('R', 11);
        numberTiles.Add('S', 11);
    }
    

    [ServerCallback]
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
        //Spawn Mountain 
        spawnHex(mainBoardHexSpawns, mountainHex, 3, true);
        //Spawn Pasture 
        spawnHex(mainBoardHexSpawns, pastureHex, 4, true);


        //Spawn Island Hexes
        //------------------
        //Spawn Gold
        spawnHex(islandHexSpawns, goldHex, 2, false);
        //Spawn Forest
        spawnHex(islandHexSpawns, forestHex, 1, false);
        //Spawn Fields
        spawnHex(islandHexSpawns, fieldHex, 1, false);
        //Spawn Hills
        spawnHex(islandHexSpawns, hillHex, 1, false);
        //Spawn Mountain 
        spawnHex(islandHexSpawns, mountainHex, 2, false);
        //Spawn Pasture 
        spawnHex(islandHexSpawns, pastureHex, 1, false);


        spawnHex(lakeSpawnLocations, dessertHex, 1, true);
    }

    void spawnHex(List<GameObject> spawnPositions, GameObject hexToSpawn, int numToSpawn, bool isOnMainBoard)
    {
        for ( int i = 0; i < numToSpawn; i++)
        {
            GameObject targetSpawn = (spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)]);
            Transform targetTransform = targetSpawn.transform;
            GameObject spawnedHex = Instantiate(hexToSpawn, targetTransform.position, Quaternion.identity, targetTransform);

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

            if (isOnMainBoard)
            {
                hex.hexNumber = numberTiles[hex.hexLetter];
                spawnedMainBoardHexes.Add(spawnedHex);
            }
            else
            {
                spawnedIslandHexes.Add(spawnedHex);
            }

            NetworkServer.Spawn(spawnedHex);
            
            spawnPositions.Remove(targetSpawn);
        }
    }
}
