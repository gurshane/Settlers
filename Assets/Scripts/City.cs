using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : GamePiece {

	private bool metropolis;

	public City(Enums.Color color, bool met) :
		base(color, Enums.PieceType.CITY) {

		this.metropolis = met;
	}

	public bool isMetropolis() {
		return this.metropolis;
	}

	public void makeMetropolis() {
		this.metropolis = true;
	}

	public void removeMetropolis() {
		this.metropolis = false;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
