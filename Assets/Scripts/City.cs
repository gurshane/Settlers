using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : GamePiece {

	private bool metropolis;

    public City() : base(Enums.PieceType.CITY)
    {

        this.metropolis = false;
    }

    public City(Enums.Color color) :
		base(color, Enums.PieceType.CITY)
    {

        this.metropolis = false;
	}

	public bool isMetropolis()
    {
		return this.metropolis;
	}

	public void makeMetropolis()
    {
		this.metropolis = true;
	}

	public void removeMetropolis()
    {
		this.metropolis = false;
	}

	public static City getFreeCity(List<GamePiece> pieces)
    {
		foreach (GamePiece p in pieces)
        {
			if (p.getPieceType () == Enums.PieceType.CITY)
            {
				if (!p.isOnBoard ())
                {
					return (City)p;
				}
			}
		}
		return null;
	}
}
