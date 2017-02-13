using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public abstract class BoardPiece : MonoBehaviour {

	private GamePiece occupyingPiece;
	private Enums.TerrainType terrainType;

	public BoardPiece (Enums.TerrainType terrain) {
		this.terrainType = terrain;
	}

	public GamePiece getOccupyingPiece() {
		return this.occupyingPiece;
	}

	public Enums.TerrainType getTerrainType() {
		return this.terrainType;
	}

	public void setOccupyingPiece(GamePiece gamePiece) {
		this.occupyingPiece = gamePiece;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
