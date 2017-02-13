using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour {

    //Hexes
    public List<Transform> hexSpawnLocations;
    public List<GameObject> hexPieces;

    //Number Tiles
    public List<Transform> numberSpawnLocations;
    public List<GameObject> numberPieces;

    //Edges
    public List<GameObject> edges;

    //Vertices
    public List<GameObject> vertices;

    //Spawned versions of the hexes, number tiles, water hexes
    private List<GameObject> spawned;
    private List<GameObject> spawnedNumbers;
    private List<GameObject> spawnedWaterTiles;

	// Use this for initialization
	void Start ()
    {
        spawned = new List<GameObject>();
        spawnedNumbers = new List<GameObject>();

        makeBoard();
	}

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            makeBoard();
        }
    }
	
	void makeBoard()
    {
        if(spawned.Count > 0)
        {
            foreach(GameObject spawnedObject in spawned)
            {
                Destroy(spawnedObject);
            }
            
            //foreach(GameObject numberTile in spawnedNumbers)
            //{
            //    Destroy(numberTile);
            //}
        }
        
        foreach (Transform spawnLocation in hexSpawnLocations)
        {
            if(spawnLocation != null)
            {
                GameObject spawnedHex = Instantiate(hexPieces[Random.Range(0, hexPieces.Count)], spawnLocation.position, Quaternion.identity, spawnLocation);
                spawned.Add(spawnedHex);
            }
           
        }
        
    }
}
