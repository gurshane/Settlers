using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class BoardPiece : NetworkBehaviour {

	private GamePiece occupyingPiece;
	public Enums.TerrainType terrainType;

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
}
