using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class Hex : BoardPiece {

	public List<Vertex> vertices;

	/// <summary>
	/// The hex value displayed above this instance
	/// </summary>
	public UIHexValue hexVal;

    [SyncVar]
	public Enums.HexType hexType;

    [SyncVar]
    public Enums.ResourceType resourceType;

    [SyncVar]
	public int hexNumber;

    [SyncVar]
    public char hexLetter;

    public Hex(Enums.TerrainType terrain, Enums.HexType hexType) : base(terrain)
    {
        this.vertices = new List<Vertex>();
        if (terrain == Enums.TerrainType.WATER)
        {
            this.hexType = Enums.HexType.WATER;
        }
        else if (hexType == Enums.HexType.WATER)
        {
            this.hexType = Enums.HexType.DESERT;
        }
        else
        {
            this.hexType = hexType;
        }
    }

    public Hex(Enums.TerrainType terrain, Enums.HexType hexType, Enums.ResourceType resType) : base(terrain) {
		this.vertices = new List<Vertex> ();
		if (terrain == Enums.TerrainType.WATER) {
			this.hexType = Enums.HexType.WATER;
		} else if (hexType == Enums.HexType.WATER) {
			this.hexType = Enums.HexType.DESERT;
		} else {
			this.hexType = hexType;
		}
        this.resourceType = resType;
	}

	public List<Vertex> getVertices() {
		return this.vertices;
	}

	public bool adjacentToVertex(Vertex v) {
		foreach (Vertex neighbour in vertices) {
			if (Object.ReferenceEquals(neighbour, v)) {
				return true;
			}
		}
		return false;
	}

	public Enums.HexType getHexType() {
		return this.hexType;
	}

	public int getHexNumber() {
		return hexNumber;
	}

	public void addVertex(Vertex v) {
		this.vertices.Add (v);
	}

	public void setHexNumber(int hexNumber) {
		this.hexNumber = hexNumber;
	}
    
}
