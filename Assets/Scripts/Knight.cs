using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : GamePiece {

	private int level;
	private bool active;
	private bool hasBeenUpgraded;
	private bool activatedThisTurn;

	public Knight(Enums.Color color) :
		base(color, Enums.PieceType.KNIGHT) {

		this.level = 1;
		this.active = false;
		this.hasBeenUpgraded = false;
		this.activatedThisTurn = false;
	}

	public void updateLevel(int level) {
		this.level = level;
	}

	public void upgrade() {
		this.level++;
		this.hasBeenUpgraded = true;
	}

	public void activateKnight() {
		this.active = true;
		this.activatedThisTurn = true;
	}

	public void deactivateKnight() {
		this.active = false;
	}

	public int getLevel() {
		return this.level;
	}

	public bool isActive() {
		return this.active;
	}

	public bool wasUpgraded() {
		return this.hasBeenUpgraded;
	}

	public bool wasActivatedThisTurn() {
		return this.activatedThisTurn;
	}

	public void notActivatedThisTurn() {
		this.activatedThisTurn = false;
	}

	public void notUpgradedThisTurn() {
		this.hasBeenUpgraded = false;
	}

	public static Knight getFreeKnight(List<GamePiece> pieces) {
		foreach (GamePiece p in pieces) {
			if (p.getPieceType () == Enums.PieceType.KNIGHT) {
				if (!p.isOnBoard ()) {
					return (Knight)p;
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
