using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class Edge : BoardPiece {

	public Vertex leftVertex;
	public Vertex rightVertex;
	public Hex leftHex;
	public Hex rightHex;
	private int visited;

    public bool isForwardPointing;
    public bool isLeftPointing;
    public bool isRightPointing;

	public Edge(Enums.TerrainType terrain) : base(terrain)
    {
		this.visited = 0;
	}
		
	public int getVisited()
    {
		return this.visited;
	}

	public void setVisited()
    {
		this.visited = 1;
	}

	public void resetVisited()
    {
		this.visited = 0;
	}

	public void clearVisited()
    {
		this.visited = 2;
	}

	public Vertex getLeftVertex()
    {
		return this.leftVertex;
	}

	public Vertex getRightVertex()
    {
		return this.rightVertex;
	}

	public Hex getLeftHex()
    {
		return this.leftHex;
	}

	public Hex getRightHex()
    {
		return this.rightHex;
	}

	public void setLeftVertex(Vertex v)
    {
		this.leftVertex = v;
	}

	public void setRightVertex(Vertex v)
    {
		this.rightVertex = v;
	}

	public void setLeftHex(Hex v)
    {
		this.leftHex = v;
	}

	public void setRightHex(Hex v)
    {
		this.rightHex = v;
	}
    
}
