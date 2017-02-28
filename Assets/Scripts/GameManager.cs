using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public static class GameManager {
	/*
	 
	// Overall Game State
	private List<Player> players;
	private List<Enums.TurnOrder> playOrder;
	private Player currentPlayer;
	private Enums.GamePhase gamePhase;

	private List<Edge> edges;
	private List<Vertex> vertices;
	private List<Hex> hexes;

	// The dice
	private int firstDie;
	private int secondDie;
	private Enums.EventDie eventDie;

	// Some important state info
	private int longestRouteLength;
	private int merchantController;
	private int longestRouteController;
	private Hex pirateLocation;
	private Hex robberLocation;
	private Dictionary<Enums.DevChartType, Vertex> metropolises;
	private bool barbarianHasAttacked;

	public List<string> getPlayerNames() {
		List<string> ret = new List<string> ();
		foreach(Player p in players) {
			ret.Add (p);
		}
		return ret;
	}

	public Player getCurrentPlayer() {
		return this.currentPlayer;
	}


	public Player getPlayer(string name) {
		foreach (Player p in players) {
			if p.getUserName().Equals(name) {
				return p;
			}
		}
		return null;
	}

	public Enums.GamePhase getGamePhase() {
		return this.gamePhase;
	}

	public int getFirstDie() {
		return this.firstDie;
	}

	public int getSecondDie() {
		return this.secondDie;
	}

	public Enums.EventDie getEventDie() {
		return this.eventDie;
	}

	public string getMerchantController() {
		if (merchantController < 0) {
			return null;
		}
		return players [merchantController].getUserName ();
	}

	public Hex getPirateLocation() {
		return this.pirateLocation;
	}

	public Hex getRobberLocation() {
		return this.robberLocation;
	}

	public bool hasBarbarianAttacked() {
		return this.barbarianHasAttacked;
	}

	// return true upon success, false upon failure
	// give the given player a metropolis on the chosen city
	// remove it from another player if another player controlled it
	public bool giveMetropolis(string player, Enums.DevChartType met, Vertex city) {
		GamePiece p = city.getOccupyingPiece ();
		if (Object.ReferenceEquals (p, null)) {
			return false;
		} else if (p.getPieceType != Enums.PieceType.CITY) {
			return false;
		}

		Vertex current = metropolises [met];

		if (!Object.ReferenceEquals (current, null)) {
			GamePiece opponent = current.getOccupyingPiece ();
			if (Object.ReferenceEquals (opponent, null)) {
				return false;
			} else if (p.getPieceType != Enums.PieceType.CITY) {
				return false;
			} else {
				((City)current).removeMetropolis ();
				string opName = current.getOwnerName ();
				Player op = getPlayer (opName);
				op.decrementVictoryPoints (2);
			}
		}

		((City)p).makeMetropolis ();
		getPlayer (player).incrementVictoryPoints (2);

		return true;
	}
		
	public void determineLongestRoad() {
	}

	public void rollDice() {

		this.firstDie = (int)Random.Range (1, 7);
		this.secondDie = (int)Random.Range (1, 7);
		int evnt = (int)Random.Range (1, 7);

		switch (evnt) {
		case 1:
			this.eventDie = Enums.EventDie.TRADE;
			break;
		case 2:
			this.eventDie = Enums.EventDie.POLITICS;
			break;
		case 3:
			this.eventDie = Enums.EventDie.SCIENCE;
			break;
		default:
			this.eventDie = Enums.EventDie.BARBARIAN;
			break;
		}

		resolveDice ();
	}

	public void rollDice(int d1, int d2) {
		
		this.firstDie = d1;
		this.secondDie = d2;
		int evnt = (int)Random.Range (1, 7);

		switch (evnt) {
		case 1:
			this.eventDie = Enums.EventDie.TRADE;
			break;
		case 2:
			this.eventDie = Enums.EventDie.POLITICS;
			break;
		case 3:
			this.eventDie = Enums.EventDie.SCIENCE;
			break;
		default:
			this.eventDie = Enums.EventDie.BARBARIAN;
			break;
		}

		resolveDice ();
	}

	private void resolveDice() {
		
		if (firstDie + secondDie == 7) {
			resolveSeven ();
		} else {
			distribute ();
		}

		//Barbarian

		//Event Die
	}

	private void resolveSeven() {

		if (!barbarianHasAttacked) {
			return;
		}

		foreach (Player p in players) {

			//Discard cards

		}

		//Choose pirate or robber

		//Move pirate or robber

		//vertices adjacent to robber
	}

	private void distribute() {

		Graph.vertexReset ();

		int num = this.firstDie + this.secondDie;

		// Check enough resources in bank

		foreach (Hex h in hexes) {

			if (h.getHexNumber () != num) {
				continue;
			}

			Enums.HexType hType = h.getHexType;

			bool gold = false;
			if (hType == Enums.HexType.GOLD) {
				gold = true;
			}

			Enums.ResourceType res = getResourceFromHex (hType);
			Enums.CommodityType com = getCommodityFromHex (hType);

			foreach (Vertex v in h.getVertices()) {
				if (v.getVisited () != 0) {
					continue;
				}

				v.setVisited();

				GamePiece current = v.getOccupyingPiece ();
				if(Object.ReferenceEquals(current, null)) {
					continue;
				} 

				if (current.getPieceType() == Enums.PieceType.SETTLEMENT) {
					string owner = current.getOwnerName ();
					Player p = getPlayer (owner);
					if (res != Enums.ResourceType.NONE) {
						Bank.withdrawResource (res, 1);
						p.addResource (res, 1);
					} else if (gold) {
						p.incrementGoldCount (2);
					}
				}

				if (current.getPieceType() == Enums.PieceType.CITY) {
					string owner = current.getOwnerName ();
					Player p = getPlayer (owner);
					if (com != Enums.ResourceType.NONE) {
						Bank.withdrawResource (res, 1);
						Bank.withdrawResource (com, 1);
						p.addResource (res, 1);
						p.addResource (com, 1);
					} else if (res == Enums.ResourceType.BRICK) {
						Bank.withdrawResource (res, 2);
						p.addResource (res, 2);
					} else if (res == Enums.ResourceType.GRAIN) {
						Bank.withdrawResource (res, 2);
						p.addResource (res, 2);
					} else if (gold) {
						p.incrementGoldCount (2);
					}
				}
			}
		}

		//Distribute aqueduct cards
	}

	private Enums.ResourceType getResourceFromHex(Enums.HexType hType) {

		switch (hType) {
		case Enums.HexType.FIELD:
			return Enums.ResourceType.GRAIN;
		case Enums.HexType.FOREST:
			return Enums.ResourceType.LUMBER;
		case Enums.HexType.HILL:
			return Enums.ResourceType.BRICK;
		case Enums.HexType.MOUNTAIN:
			return Enums.ResourceType.ORE;
		case Enums.HexType.PASTURE:
			return Enums.ResourceType.WOOL;
		default:
			return Enums.ResourceType.NONE;
		}
	}

	private Enums.CommodityType getCommodityFromHex(Enums.HexType hType) {

		switch (hType) {
		case Enums.HexType.FOREST:
			return Enums.CommodityType.PAPER;
		case Enums.HexType.MOUNTAIN:
			return Enums.CommodityType.COIN;
		case Enums.HexType.PASTURE:
			return Enums.CommodityType.CLOTH;
		default:
			return Enums.ResourceType.NONE;
		}
	}
				      
		*/	
}
