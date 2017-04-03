using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BoardState : NetworkBehaviour
{
    static public BoardState instance;

    public Dictionary<Vector3, Vertex> vertexPosition;
    public Dictionary<Vector3, Edge> edgePosition;
    public Dictionary<Vector3, Hex> hexPosition;

    public Dictionary<Vector3, GameObject> spawnedObjects;

    bool doOnce;

    void Awake()
    {
        instance = this;
    }
	// Use this for initialization
	void Start ()
    {
        doOnce = true;
        vertexPosition = new Dictionary<Vector3, Vertex>();
        edgePosition = new Dictionary<Vector3, Edge>();
        hexPosition = new Dictionary<Vector3, Hex>();
        spawnedObjects = new Dictionary<Vector3, GameObject>();
        
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        if(vertexPosition.Count < 112)
        {
            vertexPosition.Clear();
            foreach (GameObject vertex in GameObject.FindGameObjectsWithTag("Vertex"))
            {
                vertexPosition.Add(vertex.transform.position, vertex.GetComponent<Vertex>());
            }
        }
        if(edgePosition.Count < 155)
        {
            edgePosition.Clear();
            foreach (GameObject edge in GameObject.FindGameObjectsWithTag("Edge"))
            {
                edgePosition.Add(edge.transform.position, edge.GetComponent<Edge>());
            }
        }
        if(hexPosition.Count < 19)
        {
            hexPosition.Clear();
            foreach (GameObject hex in GameObject.FindGameObjectsWithTag("MainHex"))
            {
                hexPosition.Add(hex.transform.position, hex.GetComponent<Hex>());
            }
        }
            
            
	}
}
