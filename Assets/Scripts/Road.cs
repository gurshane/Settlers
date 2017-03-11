using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : GamePiece {

	private bool isShip;
	private bool builtThisTurn;

	public Road(bool isShip) :
		base(Enums.PieceType.ROAD) {

		this.isShip = isShip;
		this.builtThisTurn = true;
	}

	public Road(Enums.Color color, bool isShip) :
		base(color, Enums.PieceType.ROAD) {

		this.isShip = isShip;
		this.builtThisTurn = true;
	}

	public bool getIsShip() {
		return isShip;
	}

	public bool getBuiltThisTurn() {
		return this.builtThisTurn;
	}

	public void wasBuiltThisTurn() {
		this.builtThisTurn = true;
	}

	public void notBuiltThisTurn() {
		this.builtThisTurn = false;
	}

	public static Road getFreeRoad(List<GamePiece> pieces) {
		foreach (GamePiece p in pieces) {
			if (p.getPieceType () == Enums.PieceType.ROAD) {
				if ( !((Road)p).getIsShip () ) {
					if (!p.isOnBoard ()) {
						return (Road)p;
					}
				}
			}
		}
		return null;
	}

	public static Road getFreeShip(List<GamePiece> pieces) {
		foreach (GamePiece p in pieces) {
			if (p.getPieceType () == Enums.PieceType.ROAD) {
				if ( ((Road)p).getIsShip () ) {
					if (!p.isOnBoard ()) {
						return (Road)p;
					}
				}
			}
		}
		return null;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
