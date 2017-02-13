using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : GamePiece {

	private bool isShip;
	private bool builtThisTurn;

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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
