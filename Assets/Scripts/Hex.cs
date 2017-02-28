﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class Hex : BoardPiece {

	public List<Vertex> vertices;

    [SyncVar]
	public Enums.HexType hexType;

	private int hexNumber;

	public Hex(Enums.TerrainType terrain, Enums.HexType hexType) : base(terrain) {
		this.vertices = new List<Vertex> ();
		if (terrain == Enums.TerrainType.WATER) {
			this.hexType = Enums.HexType.WATER;
		} else if (hexType == Enums.HexType.WATER) {
			this.hexType = Enums.HexType.DESERT;
		} else {
			this.hexType = hexType;
		}
	}

	public List<Vertex> getVertices() {
		return this.vertices;
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

    void Start()
    {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
