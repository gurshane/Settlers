using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class Vertex : BoardPiece {

	public List<Edge> neighbouringEdges;
    public bool isOnMainland;
	private bool hasWall;
	private int chits;
	private bool adjacentToRobber;
	private Enums.PortType portType;
	private int visited;

	public Vertex(Enums.TerrainType terrain) : base (terrain) {
		this.hasWall = false;
		this.visited = 0;
		this.neighbouringEdges = new List<Edge> ();
	}

	public int getVisited() {
		return this.visited;
	}

	public void setVisited() {
		this.visited = 1;
	}

	public void resetVisited() {
		this.visited = 0;
	}

	public void clearVisited() {
		this.visited = 2;
	}

	public List<Edge> getNeighbouringEdges() {
		return this.neighbouringEdges;
	}

	public bool getHasWall() {
		return this.hasWall;
	}

	public int getChits() {
		return this.chits;
	}

	public bool isAdjacentToRobber() {
		return this.adjacentToRobber;
	}

	public Enums.PortType getPortType() {
		return this.portType;
	}

	public void addWall() {
		this.hasWall = true;
	}

	public void removeWall() {
		this.hasWall = false;
	}

	public void addRobber() {
		this.adjacentToRobber = true;
	}

	public void removeRobber() {
		this.adjacentToRobber = false;
	}

	public void addEdge(Edge e) {
		this.neighbouringEdges.Add (e);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
