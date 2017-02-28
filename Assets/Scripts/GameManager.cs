using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public static class GameManager {

	// Overall Game State
	private static List<Player> players;
	private static List<Enums.TurnOrder> playOrder;
	private static Player currentPlayer;
	private static Enums.GamePhase gamePhase;

	private static Edge edge1;
	private static Vertex vertex1;
	private static List<Hex> hexes;

	// The dice
	private static int firstDie;
	private static int secondDie;
	private static Enums.EventDie eventDie;

	// Some important state info
	private static int longestRouteLength;
	private static string merchantController;
	private static string longestRouteController;

	private static Hex pirateLocation;
	private static Hex robberLocation;
	private static Dictionary<Enums.DevChartType, Vertex> metropolises;
	private static bool barbarianHasAttacked;


	// Begin Game

	// End Game

	// Begin Turn

	// End Turn

	public static List<string> getPlayerNames() {
		List<string> ret = new List<string> ();
		foreach(Player p in players) {
			ret.Add (p.getUserName());
		}
		return ret;
	}

	public static Player getCurrentPlayer() {
		return currentPlayer;
	}

	public static Player getPlayer(string name) {
		foreach (Player p in players) {
			if (p.getUserName().Equals(name)) {
				return p;
			}
		}
		return null;
	}

	public static Enums.GamePhase getGamePhase() {
		return gamePhase;
	}

	public static int getFirstDie() {
		return firstDie;
	}

	public static int getSecondDie() {
		return secondDie;
	}

	public static Enums.EventDie getEventDie() {
		return eventDie;
	}

	public static string getMerchantController() {
		return merchantController;
	}

	public static string getLongestRouteContoller() {
		return longestRouteController;
	}

	public static Hex getPirateLocation() {
		return pirateLocation;
	}

	public static Hex getRobberLocation() {
		return robberLocation;
	}

	public static bool hasBarbarianAttacked() {
		return barbarianHasAttacked;
	}

	public static void barbarianAttackedThisGame() {
		barbarianHasAttacked = true;
	}

	// return true upon success, false upon failure
	// give the given player a metropolis on the chosen city
	// remove it from another player if another player controlled it
	public static bool giveMetropolis(string player, Enums.DevChartType met, Vertex city) {
		GamePiece p = city.getOccupyingPiece ();
		if (Object.ReferenceEquals (p, null)) {
			return false;
		} else if (p.getPieceType() != Enums.PieceType.CITY) {
			return false;
		}

		Vertex current = metropolises [met];

		if (!Object.ReferenceEquals (current, null)) {
			GamePiece opponent = current.getOccupyingPiece ();
			if (Object.ReferenceEquals (opponent, null)) {
				return false;
			} else if (opponent.getPieceType() != Enums.PieceType.CITY) {
				return false;
			} else {
				((City)opponent).removeMetropolis ();
				string opName = opponent.getOwnerName ();
				Player op = getPlayer (opName);
				op.decrementVictoryPoints (2);
			}
		}

		((City)p).makeMetropolis ();
		getPlayer (player).incrementVictoryPoints (2);

		return true;
	}
		
	public static void determineLongestRoute() {
	}

	public static void updateRobberLocation(Hex newLocation) {
		robberLocation = newLocation;
	}

	public static void updatePirateLocation(Hex newLocation) {
		pirateLocation = newLocation;
	}

	// Remove the old merchant controller, and set the new one, assigning victory points
	public static void setMerchantController(Merchant m, string player) {
		if (merchantController != "") {
			Player p = getPlayer (merchantController);
			if (!Object.ReferenceEquals (p, null)) {
				p.decrementVictoryPoints (1);
			}
		}

		Player given = getPlayer(player);
		if (!Object.ReferenceEquals (given, null)) {
			merchantController = player;
			given.incrementVictoryPoints (1);
			m.putOnBoard ();
		}
	}

	// Get a dice roll, randomly
	public static void rollDice() {

		// Get random values for all the dice
		firstDie = (int)Random.Range (1, 7);
		secondDie = (int)Random.Range (1, 7);
		int evnt = (int)Random.Range (1, 7);

		// Assign an event die outcome to the event die
		switch (evnt) {
		case 1:
			eventDie = Enums.EventDie.TRADE;
			break;
		case 2:
			eventDie = Enums.EventDie.POLITICS;
			break;
		case 3:
			eventDie = Enums.EventDie.SCIENCE;
			break;
		default:
			eventDie = Enums.EventDie.BARBARIAN;
			break;
		}

		// Resolve the outcome of the dice roll
		resolveDice ();
	}

	// Pick the dice (for alchemist)
	public static void rollDice(int d1, int d2) {
		
		firstDie = d1;
		secondDie = d2;
		int evnt = (int)Random.Range (1, 7);

		// Assign an event die outcome to the event die
		switch (evnt) {
		case 1:
			eventDie = Enums.EventDie.TRADE;
			break;
		case 2:
			eventDie = Enums.EventDie.POLITICS;
			break;
		case 3:
			eventDie = Enums.EventDie.SCIENCE;
			break;
		default:
			eventDie = Enums.EventDie.BARBARIAN;
			break;
		}

		// Resolve the dice outcome
		resolveDice ();
	}

	// Resolve a dice roll
	private static void resolveDice() {

		// Check if a seven was rolled
		if (firstDie + secondDie == 7) {
			resolveSeven ();
		} else {
			distribute ();
		}

		//Barbarian

		//Event Die

		if (gamePhase == Enums.GamePhase.PHASE_ONE) {
			gamePhase = Enums.GamePhase.PHASE_TWO;
		} else if (gamePhase == Enums.GamePhase.SETUP_ONE) {
			gamePhase = Enums.GamePhase.SETUP_TWO; 
		}
	}

	// Resolve a seven if it is rolled
	private static void resolveSeven() {

		// If the barbarian has not attacked, nothing happens
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

	// Distribute the appropriate resources to all players
	private static void distribute() {
		Graph.vertexReset (vertex1);
		int num = firstDie + secondDie;

		// Make sure there are enough resources and commodities in the bank
		Dictionary<Enums.ResourceType, bool> enoughRes = new Dictionary<Enums.ResourceType, bool> ();
		Dictionary<Enums.CommodityType, bool> enoughComs = new Dictionary<Enums.CommodityType, bool> ();

		for (int i = 0; i < 5; i++) {
			enoughRes.Add ((Enums.ResourceType)i, checkResources ((Enums.ResourceType)i, num));
		}
		for (int i = 0; i < 3; i++) {
			enoughComs.Add ((Enums.CommodityType)i, checkCommodities ((Enums.CommodityType)i, num));
		}

		foreach (Hex h in hexes) {

			// If a hex isn't the right number, or doesn't produce cards, continue
			if (h.getHexNumber () != num) {
				continue;
			}
			Enums.HexType hType = h.getHexType();

			// Check if a hex produces gold
			bool gold = false;
			if (hType == Enums.HexType.GOLD) {
				gold = true;
			}

			Enums.ResourceType res = getResourceFromHex (hType);
			Enums.CommodityType com = getCommodityFromHex (hType);
			if (res == Enums.ResourceType.NONE) {
				continue;
			}

			// Distribute all the resources
			foreach (Vertex v in h.getVertices()) {
				if (v.getVisited () != 0) {
					continue;
				}
				v.setVisited();

				GamePiece current = v.getOccupyingPiece ();
				if(Object.ReferenceEquals(current, null)) {
					continue;
				} 

				// Distribue resources for settlements
				if (current.getPieceType() == Enums.PieceType.SETTLEMENT) {
					string owner = current.getOwnerName ();
					Player p = getPlayer (owner);
					if (res != Enums.ResourceType.NONE && enoughRes[res]) {
						//Bank.withdrawResource (res, 1);
						p.addResource (res, 1);
					} else if (gold) {
						p.incrementGoldCount (2);
					}
				}

				// Distribute resources and commodities for cities
				if (current.getPieceType() == Enums.PieceType.CITY) {
					string owner = current.getOwnerName ();
					Player p = getPlayer (owner);
					if (com != Enums.CommodityType.NONE) {
						if (enoughRes [res]) {
							//Bank.withdrawResource (res, 1);
							p.addResource (res, 1);
						}
						if (enoughComs [com]) {
							//Bank.withdrawCommodity (com, 1);
							p.addCommodity (com, 1);
						}
					} else if (res == Enums.ResourceType.BRICK && enoughRes[res]) {
						//Bank.withdrawResource (res, 2);
						p.addResource (res, 2);
					} else if (res == Enums.ResourceType.GRAIN && enoughRes[res]) {
						//Bank.withdrawResource (res, 2);
						p.addResource (res, 2);
					} else if (gold) {
						p.incrementGoldCount (2);
					}
				}
			}
		}

		//Distribute aqueduct cards
	}

	// get a resource from a hex
	private static Enums.ResourceType getResourceFromHex(Enums.HexType hType) {

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

	// Get a commodity from a hex
	private static Enums.CommodityType getCommodityFromHex(Enums.HexType hType) {

		switch (hType) {
		case Enums.HexType.FOREST:
			return Enums.CommodityType.PAPER;
		case Enums.HexType.MOUNTAIN:
			return Enums.CommodityType.COIN;
		case Enums.HexType.PASTURE:
			return Enums.CommodityType.CLOTH;
		default:
			return Enums.CommodityType.NONE;
		}
	}
				
	// Make sure there are enough resources in the bank for a given dice roll
	private static bool checkResources(Enums.ResourceType res, int n) {
		Graph.vertexReset (vertex1);
		int total = 0;

		foreach (Hex h in hexes) {

			// If a hex isn't the right type or number, continue
			if (h.getHexNumber () != n) {
				continue;
			}

			Enums.HexType hType = h.getHexType();
			if (getResourceFromHex (hType) != res) {
				continue;
			}

			// Get all the resources accumulated by all players
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
					total++;
				}
				if (current.getPieceType() == Enums.PieceType.CITY) {
					if (res == Enums.ResourceType.BRICK) {
						total += 2;
					} else if (res == Enums.ResourceType.GRAIN) {
						total += 2;
					} else if (res != Enums.ResourceType.NONE) {
						total++;
					}
				}
			}
		}

		// Check the amount against the bank
		int bankAmount = 100; //Bank.getResourceAmount (res);
		if (bankAmount >= total) {
			return true;
		} else {
			return false;
		}
	}

	// Make sure there are enough commodities in the bank for a given dice roll
	private static bool checkCommodities(Enums.CommodityType com, int n) {
		Graph.vertexReset (vertex1);
		int total = 0;

		foreach (Hex h in hexes) {

			// If a hex isn't the right type or number, continue
			if (h.getHexNumber () != n) {
				continue;
			}

			Enums.HexType hType = h.getHexType();
			if (getCommodityFromHex (hType) != com) {
				continue;
			}

			// Get all the commodities accumulated by all players
			foreach (Vertex v in h.getVertices()) {
				if (v.getVisited () != 0) {
					continue;
				}
				v.setVisited();

				GamePiece current = v.getOccupyingPiece ();
				if(Object.ReferenceEquals(current, null)) {
					continue;
				} 
				if (current.getPieceType() == Enums.PieceType.CITY) {
					switch (com) {
					case Enums.CommodityType.NONE:
						break;
					default:
						total++;
						break;
					}
				}
			}
		}

		// Check the amount against the bank
		int bankAmount = 100; //Bank.getCommodityAmount (com);
		if (bankAmount >= total) {
			return true;
		} else {
			return false;
		}
	}
}
