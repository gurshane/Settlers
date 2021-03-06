﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public abstract class GamePiece {

	private Enums.Color myColor;
	private Enums.PieceType pieceType;
	private bool onBoard;

    public GamePiece(Enums.PieceType pieceType)
    {
        this.myColor = Enums.Color.NONE;
        this.pieceType = pieceType;
        this.onBoard = false;
    }

    public GamePiece(Enums.Color color, Enums.PieceType pieceType)
    {
		this.myColor = color;
		this.pieceType = pieceType;
		this.onBoard = false;
	}

	public Enums.Color getColor()
    {
		return this.myColor;
	}

	public void setColor(Enums.Color color)
	{
		this.myColor = color;
	}

	public Enums.PieceType getPieceType()
    {
		return this.pieceType;
	}

	public bool isOnBoard()
    {
		return onBoard;
	}

	public void putOnBoard()
    {
		onBoard = true;
	}

	public void takeOffBoard()
    {
		onBoard = false;
	}

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
