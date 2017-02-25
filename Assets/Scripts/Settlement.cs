using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : GamePiece {

	public Settlement(Enums.Color color) : base(color, Enums.PieceType.SETTLEMENT) {}

	public static Settlement getFreeSettlement (List<GamePiece> pieces) {
		foreach (GamePiece p in pieces) {
			if (p.getPieceType () == Enums.PieceType.SETTLEMENT) {
				if (!p.isOnBoard ()) {
					return (Settlement)p;
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
