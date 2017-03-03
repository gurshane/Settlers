using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	// Overall Game State
	private List<Player> players;
	private List<Enums.TurnOrder> playOrder;
	private Player currentPlayer;

    [SyncVar]
	public Enums.GamePhase gamePhase;

    [SyncVar]
	public int pointsToWin;

	private Edge edge1;
	private Vertex vertex1;
	private List<Hex> hexes;

	// The dice
    [SyncVar]
	public int firstDie;

    [SyncVar]
	public int secondDie;

    [SyncVar]
	public Enums.EventDie eventDie;

	// Some important state info
    [SyncVar]
	public int longestRouteLength;

    [SyncVar]
	public string merchantController;

    [SyncVar]
	public string longestRouteController;

	private Hex pirateLocation;
	private Hex robberLocation;
	private Dictionary<Enums.DevChartType, Vertex> metropolises;

    [SyncVar]
	public bool barbarianHasAttacked;

	private const int numCommodities = 3;
	private const int numResources = 5;
	private const int numDevChartType = 3;
	private const int progressCardLimit = 4;

    TradeManager tradeManager;
    Bank bank;
    BoardState boardState;
    MoveManager moveManager;

    void Start()
    {
        tradeManager = GetComponent<TradeManager>();
        bank = GetComponent<Bank>();
        boardState = GetComponent<BoardState>();
        moveManager = GetComponent<MoveManager>();
    }

    void Update()
    {

    }

    public int getNumberResources() {
		return numResources;
	}

	public int getNumberCommodities() {
		return numCommodities;
	}

	public int getNumberDevChartType() {
		return numDevChartType;
	}

	public int getProgressCardLimit() {
		return progressCardLimit;
	}

	public int getPointsToWin() {
		return pointsToWin;
	}

	// Complete an initial first turn
	public void initialFirstTurn() {
		currentPlayer = getNextPlayer (false);
		gamePhase = Enums.GamePhase.SETUP_ONE;

		// Current player places pieces

		if (getCurrentTurn () == getLastTurn ()) {
			initialSecondTurn (true);
		} else {
			initialFirstTurn ();
		}
	}
		
	// Complete an initial second turn
	public void initialSecondTurn(bool first) {
		if (first && getCurrentTurn () == getLastTurn ()) {
			currentPlayer = currentPlayer;
		} else {
			currentPlayer = getNextPlayer (true);
		}
		gamePhase = Enums.GamePhase.SETUP_TWO;

		// Current player places pieces

		if (getCurrentTurn () == Enums.TurnOrder.FIRST) {
			beginTurn (true);
		} else {
			initialSecondTurn (false);
		}
	}

	// Begin Game

	// End Game

	public void endGame() {
	}

	// Begin a Turn
	public void beginTurn(bool first) {
		if (first) {
			currentPlayer = currentPlayer;
		} else {
			currentPlayer = getNextPlayer (false);
		}
		gamePhase = Enums.GamePhase.PHASE_ONE;

		// current player rolls dice
	}

	// End a turn
	public void endTurn() {

		// If a player has won, end the game
		foreach (Player p in players) {
			if (p.getVictoryCounts () >= pointsToWin) {
				endGame ();
				return;
			}
		}

		// Do some cleanup steps
		foreach (Player p in players) {
			p.roadNotMoved ();

			// If any player has more than 4 progress cards, they must discard
			if (p.getProgressCards ().Count > progressCardLimit) {

				// Those players discard down to 4 progress cards

			}

			// Set some turn specific booleans to false
			List<GamePiece> pieceList = p.getGamePieces ();
			foreach (GamePiece piece in pieceList) {
				if (piece.getPieceType () == Enums.PieceType.KNIGHT) {
					Knight k = (Knight)piece;
					k.notActivatedThisTurn ();
					k.notUpgradedThisTurn ();
				} else if (piece.getPieceType () == Enums.PieceType.ROAD) {
					Road r = (Road)piece;
					r.notBuiltThisTurn ();
				}
			}
		}

		// Begin the next turn
		beginTurn (false);
	}

	// Find the next player
	private Player getNextPlayer(bool reverse) {

		// If current player is null, get the first player in the turn order
		if (Object.ReferenceEquals(currentPlayer, null)) {
			for (int i = 0; i < playOrder.Count; i++) {
				if (playOrder[i] == Enums.TurnOrder.FIRST) {
					return players[i];
				}
			}
		}

		Enums.TurnOrder currentTurn = getCurrentTurn();
		Enums.TurnOrder nextTurn;
		Enums.TurnOrder lastTurn = getLastTurn ();

		// Check if turns are moving in reverse order
		if (!reverse) {
			if (currentTurn == lastTurn) {
				nextTurn = Enums.TurnOrder.FIRST;
			} else {
				int next = ((int)currentTurn) + 1;
				nextTurn = (Enums.TurnOrder)next;
			}
		} else {
			if (currentTurn == Enums.TurnOrder.FIRST) {
				nextTurn = lastTurn;
			} else {
				int next = ((int)currentTurn) - 1;
				nextTurn = (Enums.TurnOrder)next;
			}
		}

		// Find the next player based on the next turn
		int index = 0;
		for (int i = 0; i < playOrder.Count; i++) {
			if (playOrder [i] == nextTurn) {
				index = i;
			}
		}

		// Return the next player
		return players [index];
	}

	// Get the current turn based on the current player
	private Enums.TurnOrder getCurrentTurn() {
		int index = 0;
		for (int i = 0; i < players.Count; i++) {
			if (Object.ReferenceEquals (currentPlayer, players [i])) {
				index = i;
			}
		}
		return playOrder [index];
	}

	// Get the last turn based on the number of people playing
	private Enums.TurnOrder getLastTurn() {
		if (playOrder.Count == 1) {
			return Enums.TurnOrder.FIRST;
		} else if (playOrder.Count == 2) {
			return Enums.TurnOrder.SECOND;
		} else if (playOrder.Count == 3) {
			return Enums.TurnOrder.THIRD;
		} else {
			return Enums.TurnOrder.FOURTH;
		}
	}

	public List<string> getPlayerNames() {
		List<string> ret = new List<string> ();
		foreach(Player p in players) {
			ret.Add (p.getUserName());
		}
		return ret;
	}

	public Player getCurrentPlayer() {
		return currentPlayer;
	}

	public Player getPlayer(string name) {
		foreach (Player p in players) {
			if (p.getUserName().Equals(name)) {
				return p;
			}
		}
		return null;
	}

	public Enums.GamePhase getGamePhase() {
		return gamePhase;
	}

	public int getFirstDie() {
		return firstDie;
	}

	public int getSecondDie() {
		return secondDie;
	}

	public Enums.EventDie getEventDie() {
		return eventDie;
	}

	public string getMerchantController() {
		return merchantController;
	}

	public string getLongestRouteContoller() {
		return longestRouteController;
	}

	public Hex getPirateLocation() {
		return pirateLocation;
	}

	public Hex getRobberLocation() {
		return robberLocation;
	}

	public bool hasBarbarianAttacked() {
		return barbarianHasAttacked;
	}

	public void barbarianAttackedThisGame() {
		barbarianHasAttacked = true;
	}

	// return true upon success, false upon failure
	// give the given player a metropolis on the chosen city
	// remove it from another player if another player controlled it
	public bool giveMetropolis(string player, Enums.DevChartType met, Vertex city) {
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
		
	public void determineLongestRoute() {
	}

	public void updateRobberLocation(Hex newLocation) {
		robberLocation = newLocation;
	}

	public void updatePirateLocation(Hex newLocation) {
		pirateLocation = newLocation;
	}

	// Remove the old merchant controller, and set the new one, assigning victory points
	public void setMerchantController(Merchant m, string player) {
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
	public void rollDice() {

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
	public void rollDice(int d1, int d2) {
		
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
	private void resolveDice() {

		// Check if a seven was rolled
		if (firstDie + secondDie == 7) {
			resolveSeven ();
		} else {
			distribute ();
		}

		//Barbarian

		//Event Die

		gamePhase = Enums.GamePhase.PHASE_TWO;
	}

	// Resolve a seven if it is rolled
	private void resolveSeven() {

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
	private void distribute() {
		Graph.vertexReset (vertex1);
		int num = firstDie + secondDie;

		// Make sure there are enough resources and commodities in the bank
		Dictionary<Enums.ResourceType, bool> enoughRes = new Dictionary<Enums.ResourceType, bool> ();
		Dictionary<Enums.CommodityType, bool> enoughComs = new Dictionary<Enums.CommodityType, bool> ();

		for (int i = 0; i < numResources; i++) {
			enoughRes.Add ((Enums.ResourceType)i, checkResources ((Enums.ResourceType)i, num));
		}
		for (int i = 0; i < numCommodities; i++) {
			enoughComs.Add ((Enums.CommodityType)i, checkCommodities ((Enums.CommodityType)i, num));
		}

		foreach (Hex h in hexes) {

			// If a hex isn't the right number, or doesn't produce cards, continue
			if (h.getHexNumber () != num) {
				continue;
			}

			if (Object.ReferenceEquals(h, robberLocation)) {
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
						bank.withdrawResource (res, 1);
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
							bank.withdrawResource (res, 1);
							p.addResource (res, 1);
						}
						if (enoughComs [com]) {
							bank.withdrawCommodity (com, 1);
							p.addCommodity (com, 1);
						}
					} else if (res == Enums.ResourceType.BRICK && enoughRes[res]) {
						bank.withdrawResource (res, 2);
						p.addResource (res, 2);
					} else if (res == Enums.ResourceType.GRAIN && enoughRes[res]) {
						bank.withdrawResource (res, 2);
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
	public Enums.ResourceType getResourceFromHex(Enums.HexType hType) {

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
	public Enums.CommodityType getCommodityFromHex(Enums.HexType hType) {

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
	private bool checkResources(Enums.ResourceType res, int n) {
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
		int bankAmount = bank.getResourceAmount (res);
		if (bankAmount >= total) {
			return true;
		} else {
			return false;
		}
	}

	// Make sure there are enough commodities in the bank for a given dice roll
	private bool checkCommodities(Enums.CommodityType com, int n) {
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
		int bankAmount = bank.getCommodityAmount (com);
		if (bankAmount >= total) {
			return true;
		} else {
			return false;
		}
	}

}
