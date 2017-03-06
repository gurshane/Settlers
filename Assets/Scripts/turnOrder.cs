using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class turnOrder : MonoBehaviour {

    public Enums.Color currentTurn;

	// Use this for initialization
	void Start () {
        currentTurn = Enums.Color.WHITE;
	}
	
	public void nextTurn()
    {
        currentTurn = (Enums.Color)(((int)currentTurn) + 1);
    }
}
