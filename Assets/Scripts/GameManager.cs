using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    [SyncVar]
	public Enums.GamePhase gamePhase;

    [SyncVar]
	public int pointsToWin;

    [SyncVar]
    public Enums.Color colorToPick;
    
    [SyncVar]
    public int firstDie;

    [SyncVar]
    public int secondDie;

    [SyncVar]
    public Enums.EventDie eventDie;
    
    [SyncVar]
	public int longestRouteLength;
    
    [SyncVar]
    public int color = 0;

    [SyncVar]
    private int playerTurn;

    [SyncVar]
    private int merchantController;

    [SyncVar]
    private int longestRouteController;

    [SyncVar]
    public bool barbarianHasAttacked;

    [SyncVar]
    private int barbarianPos;

    private Hex pirateLocation;
	private Hex robberLocation;
	private Dictionary<Enums.DevChartType, Vertex> metropolises;

	private const int numCommodities = 3;
	private const int numResources = 5;
	private const int numDevChartType = 3;
	private const int progressCardLimit = 4;

    // Overall Game State
    private List<Player> players;

    TradeManager tradeManager;
    Bank bank;
    MoveManager moveManager;
    Graph graph;
    
    static public GameManager instance = null;

    public void init() //initializer
    {
        Debug.Log("Started Init");
        players = new List<Player>();
        Debug.Log("network connection: " + Network.connections.Length);
        ServerInitPlayers();
        ClientInitPlayers();
        this.CmdSetPlayerTurn();
    }

    [Server]
    private void ServerInitPlayers()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(objects.Length);
        for (int i = 0; i < objects.Length; i++)
        {
            Player player = objects[i].GetComponent<Player>();
            player.Init(i);
            player.RpcInit();
            players.Add (player);

        }
    }

    [Client]
    private void ClientInitPlayers()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(objects.Length);
        for (int i = 0; i < objects.Length; i++)
        {
            Player player = objects[i].GetComponent<Player>();
            players[player.getID()] = player;
        }
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        tradeManager = GetComponent<TradeManager>();
        bank = GetComponent<Bank>();
        moveManager = GetComponent<MoveManager>();
        
        players = new List<Player>();
        gamePhase = Enums.GamePhase.SETUP_ONE;
        playerTurn = 5;
        pointsToWin = 16;
        firstDie = 0;
        secondDie = 0;
        longestRouteLength = 0;
        merchantController = -1;
        longestRouteController = -1;

        metropolises = new Dictionary<DevChartType, Vertex>();
        metropolises.Add(Enums.DevChartType.POLITICS, null);
        metropolises.Add(Enums.DevChartType.TRADE, null);
        metropolises.Add(Enums.DevChartType.SCIENCE, null);

        barbarianHasAttacked = false;

        graph = new Graph();

        bank = GetComponent<Bank>();

        init();

    }

    public int getPlayerTurn()
    {
        return this.playerTurn;
    }

    [Command]
    public void CmdSetPlayerTurn()
    {
        playerTurn++;
        if (this.playerTurn >= players.Count)
        {
            playerTurn = 0;
        }
		Debug.Log ("turn = " + playerTurn);
        EventNextPlayer();
    }

    public delegate void DiceRolledDelegate(int firstDie, int secondDie, int thirdDie);

    [SyncEvent]
    public event DiceRolledDelegate EventDiceRolled; //event that syncs to all clients

    [Command]
    public void CmdStartTurn()//control flow function
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].RpcDiceRoll(playerTurn);
        }
        resolveDice();
    }

    [Command]
    public void CmdDiceRolled()
    {
        firstDie = UnityEngine.Random.Range(1, 7);
        secondDie = UnityEngine.Random.Range(1, 7);
        int thirdDie = UnityEngine.Random.Range(0, 4);//eventDie 
        eventDie = (Enums.EventDie)System.Enum.Parse(typeof(Enums.EventDie), thirdDie.ToString());
        EventDiceRolled(firstDie, secondDie, thirdDie);//call the event on all client objects
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

    // Begin Game

    // End Game

    public void endGame()
    {
    }

    // End a turn
    public void endTurn()
    {

        // If a player has won, end the game
        foreach (Player p in players)
        {
            if (p.getVictoryCounts() >= pointsToWin)
            {
                endGame();
                return;
            }
        }

        // Do some cleanup steps
        foreach (Player p in players)
        {
            p.roadNotMoved();

            // If any player has more than 4 progress cards, they must discard
            if (p.getProgressCards().Count > progressCardLimit)
            {

                // Those players discard down to 4 progress cards

            }

            // Set some turn specific booleans to false
            List<GamePiece> pieceList = p.getGamePieces();
            foreach (GamePiece piece in pieceList)
            {
                if (piece.getPieceType() == Enums.PieceType.KNIGHT)
                {
                    Knight k = (Knight)piece;
                    k.notActivatedThisTurn();
                    k.notUpgradedThisTurn();
                }
                else if (piece.getPieceType() == Enums.PieceType.ROAD)
                {
                    Road r = (Road)piece;
                    r.notBuiltThisTurn();
                }
            }
        }

        // Begin the next turn
    }

    // Find the next player
    private Player getNextPlayer(bool reverse)
    {

        // If current player is null, get the first player in the turn order
		if (Object.ReferenceEquals(getCurrentPlayer(), null))
        {
            return players[0];
        }

        // Check if turns are moving in reverse order
        if (!reverse)
        {
			if (Object.ReferenceEquals(getCurrentPlayer(), players[players.Count - 1]))
            {
                return players[0];
            }
            else
            {
                for (int i = 0; i < players.Count - 1; i++)
                {
					if (Object.ReferenceEquals(players[i], getCurrentPlayer()))
                    {
                        return players[i + 1];
                    }
                }
            }
        }
        else
        {
			if (Object.ReferenceEquals(getCurrentPlayer(), players[0]))
            {
                return players[players.Count - 1];
            }
            else
            {
                for (int i = 1; i < players.Count; i++)
                {
					if (Object.ReferenceEquals(players[i], getCurrentPlayer()))
                    {
                        return players[i - 1];
                    }
                }
            }
        }

        // Return the first player by default
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

	public int getMerchantController() {
		return merchantController;
	}

	public int getLongestRouteContoller() {
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

	public delegate void FirstTurnDelegate();

	[SyncEvent]
	public event FirstTurnDelegate EventFirstTurn;

	public delegate void SecondTurnDelegate();

	[SyncEvent]
	public event SecondTurnDelegate EventSecondTurn;

    // return true upon success, false upon failure
    // give the given player a metropolis on the chosen city
    // remove it from another player if another player controlled it
    public bool giveMetropolis(int player, Enums.DevChartType met, Vertex city)
    {
        GamePiece p = city.getOccupyingPiece();
        if (Object.ReferenceEquals(p, null))
        {
            return false;
        }
        else if (p.getPieceType() != Enums.PieceType.CITY)
        {
            return false;
        }

        Vertex current = metropolises[met];

        if (!Object.ReferenceEquals(current, null))
        {
            GamePiece opponent = current.getOccupyingPiece();
            if (Object.ReferenceEquals(opponent, null))
            {
                return false;
            }
            else if (opponent.getPieceType() != Enums.PieceType.CITY)
            {
                return false;
            }
            else
            {
                ((City)opponent).removeMetropolis();
                Enums.Color opColor = opponent.getColor();
                Player op = getPlayer(opColor);
                op.changeVictoryPoints(-2);
            }
        }

        ((City)p).makeMetropolis();
        getPlayer(player).changeVictoryPoints(2);

        return true;
    }

    public void determineLongestRoute()
    {
    }

    public Player getPlayer(int id)
    {
        if (id < 0 || id >= players.Count)
        {
            return null;
        }

        return players[id];
    }
    
    public Player getCurrentPlayer()
    {
		return players[getPlayerTurn()];
    }

    public Player getPlayer(Enums.Color color)
    {
        foreach (Player p in players)
        {
            if (p.getColor() == color)
            {
                return p;
            }
        }

        return null;
    }

    public void updateRobberLocation(Hex newLocation)
    {
        robberLocation = newLocation;
    }

    public void updatePirateLocation(Hex newLocation)
    {
        pirateLocation = newLocation;
    }

    // Remove the old merchant controller, and set the new one, assigning victory points
    public void setMerchantController(Merchant m, int player)
    {
        if (merchantController != -1)
        {
            Player p = getPlayer(merchantController);
            if (!Object.ReferenceEquals(p, null))
            {
                p.changeVictoryPoints(-1);
            }
        }

        Player given = getPlayer(player);
        if (!Object.ReferenceEquals(given, null))
        {
            merchantController = player;
            given.changeVictoryPoints(1);
            m.putOnBoard();
        }
    }

    // Get a dice roll, randomly
    public void rollDice()
    {

        // Get random values for all the dice
        firstDie = (int)Random.Range(1, 7);
        secondDie = (int)Random.Range(1, 7);
        int evnt = (int)Random.Range(1, 7);

        // Assign an event die outcome to the event die
        switch (evnt)
        {
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
        resolveDice();
    }

    // Pick the dice (for alchemist)
    public void rollDice(int d1, int d2)
    {

        firstDie = d1;
        secondDie = d2;
        int evnt = (int)Random.Range(1, 7);

        // Assign an event die outcome to the event die
        switch (evnt)
        {
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
        resolveDice();
    }

    // Resolve a dice roll
    [Server]
    private void resolveDice()
    {

        // Check if a seven was rolled
        if (firstDie + secondDie == 7)
        {
            resolveSeven();
        }
        else
        {
            distribute();
        }

        //Barbarian

        //Event Die

        gamePhase = Enums.GamePhase.PHASE_TWO;
    }

    // Resolve a seven if it is rolled
    [Server]
    private void resolveSeven()
    {

        // If the barbarian has not attacked, nothing happens
        if (!barbarianHasAttacked)
        {
            return;
        }

        foreach (Player p in players)
        {

            //Discard cards

        }

        //Choose pirate or robber

        //Move pirate or robber

        //vertices adjacent to robber
    }


    [Server]
    private void resolveBarbarian()
    {
        barbarianPos++;
        if (barbarianPos > 6)
        {
            barbarianPos = 0;
            barbarianAttack();
        }
    }

    private void barbarianAttack()
    {
        int knightNum = 0;
        int citiesCount = 0;
        List<City> cities = new List<City>();
        foreach(Vertex v in BoardState.instance.vertexPosition.Values)
        {
            GamePiece gp = v.getOccupyingPiece();
            if(gp != null)
            {
                try
                {
                    City c = (City)gp;
                    citiesCount++;
                }
                catch
                {

                }
            }
        }

        int[] pKnights = new int[4];
        for (int i = 0; i < players.Count; i++)
        {
            pKnights[i] = 0;
            List<GamePiece> pieces = players[i].getGamePieces();
            for (int j = 0; j < pieces.Count; j++)
            {
                Knight knight = (Knight)pieces[i];
                if (knight != null)
                {
                    if (knight.isActive())
                    {
                        pKnights[i] += knight.getLevel();
                        knight.deactivateKnight();
                    }
                }
            }
            knightNum += pKnights[i];
        }
        if (knightNum >= citiesCount)
        {
            EventBarbarianAttack(true, pKnights);
        }
        else
        {
            EventBarbarianAttack(false, pKnights);
        }
    }

    public delegate void BarbarianAttackDelegate(bool win, int[] winners);

    [SyncEvent]
    public event BarbarianAttackDelegate EventBarbarianAttack; //event that syncs to all clients

    public delegate void NextPlayerDelegate();

    [SyncEvent]
    public event NextPlayerDelegate EventNextPlayer;


    // Distribute the appropriate resources to all players
    private void distribute()
    {
        //graph.vertexReset(vertex1);
        //int num = firstDie + secondDie;

        //// Make sure there are enough resources and commodities in the bank
        //Dictionary<Enums.ResourceType, bool> enoughRes = new Dictionary<Enums.ResourceType, bool>();
        //Dictionary<Enums.CommodityType, bool> enoughComs = new Dictionary<Enums.CommodityType, bool>();

        //for (int i = 0; i < numResources; i++)
        //{
        //    enoughRes.Add((Enums.ResourceType)i, checkResources((Enums.ResourceType)i, num));
        //}
        //for (int i = 0; i < numCommodities; i++)
        //{
        //    enoughComs.Add((Enums.CommodityType)i, checkCommodities((Enums.CommodityType)i, num));
        //}

        //foreach (Hex h in boardState.hexPoisition.Values)
        //{

        //    // If a hex isn't the right number, or doesn't produce cards, continue
        //    if (h.getHexNumber() != num)
        //    {
        //        continue;
        //    }

        //    if (Object.ReferenceEquals(h, robberLocation))
        //    {
        //        continue;
        //    }
        //    Enums.HexType hType = h.getHexType();

        //    // Check if a hex produces gold
        //    bool gold = false;
        //    if (hType == Enums.HexType.GOLD)
        //    {
        //        gold = true;
        //    }

        //    Enums.ResourceType res = getResourceFromHex(hType);
        //    Enums.CommodityType com = getCommodityFromHex(hType);
        //    if (res == Enums.ResourceType.NONE)
        //    {
        //        continue;
        //    }

        //    // Distribute all the resources
        //    foreach (Vertex v in h.getVertices())
        //    {
        //        if (v.getVisited() != 0)
        //        {
        //            continue;
        //        }
        //        v.setVisited();

        //        GamePiece current = v.getOccupyingPiece();
        //        if (Object.ReferenceEquals(current, null))
        //        {
        //            continue;
        //        }

        //        // Distribue resources for settlements
        //        if (current.getPieceType() == Enums.PieceType.SETTLEMENT)
        //        {
        //            Enums.Color ownerColor = current.getColor();
        //            Player p = getPlayer(ownerColor);
        //            if (res != Enums.ResourceType.NONE && enoughRes[res])
        //            {
        //                bank.withdrawResource(res, 1);
        //                p.changeResource(res, 1);
        //            }
        //            else if (gold)
        //            {
        //                p.changeGoldCount(2);
        //            }
        //        }

        //        // Distribute resources and commodities for cities
        //        if (current.getPieceType() == Enums.PieceType.CITY)
        //        {
        //            Enums.Color ownerColor = current.getColor();
        //            Player p = getPlayer(ownerColor);
        //            if (com != Enums.CommodityType.NONE)
        //            {
        //                if (enoughRes[res])
        //                {
        //                    bank.withdrawResource(res, 1);
        //                    p.changeResource(res, 1);
        //                }
        //                if (enoughComs[com])
        //                {
        //                    bank.withdrawCommodity(com, 1);
        //                    p.changeCommodity(com, 1);
        //                }
        //            }
        //            else if (res == Enums.ResourceType.BRICK && enoughRes[res])
        //            {
        //                bank.withdrawResource(res, 2);
        //                p.changeResource(res, 2);
        //            }
        //            else if (res == Enums.ResourceType.GRAIN && enoughRes[res])
        //            {
        //                bank.withdrawResource(res, 2);
        //                p.changeResource(res, 2);
        //            }
        //            else if (gold)
        //            {
        //                p.changeGoldCount(2);
        //            }
        //        }
        //    }
        //}
        //Distribute aqueduct cards
    }

    // get a resource from a hex
    public Enums.ResourceType getResourceFromHex(Enums.HexType hType)
    {

        switch (hType)
        {
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
    public Enums.CommodityType getCommodityFromHex(Enums.HexType hType)
    {

        switch (hType)
        {
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
    private bool checkResources(Enums.ResourceType res, int n)
    {
        //graph.vertexReset(vertex1);
        //int total = 0;

        //foreach (Hex h in hexes)
        //{

        //    // If a hex isn't the right type or number, continue
        //    if (h.getHexNumber() != n)
        //    {
        //        continue;
        //    }

        //    Enums.HexType hType = h.getHexType();
        //    if (getResourceFromHex(hType) != res)
        //    {
        //        continue;
        //    }

        //    // Get all the resources accumulated by all players
        //    foreach (Vertex v in h.getVertices())
        //    {
        //        if (v.getVisited() != 0)
        //        {
        //            continue;
        //        }
        //        v.setVisited();

        //        GamePiece current = v.getOccupyingPiece();
        //        if (Object.ReferenceEquals(current, null))
        //        {
        //            continue;
        //        }
        //        if (current.getPieceType() == Enums.PieceType.SETTLEMENT)
        //        {
        //            total++;
        //        }
        //        if (current.getPieceType() == Enums.PieceType.CITY)
        //        {
        //            if (res == Enums.ResourceType.BRICK)
        //            {
        //                total += 2;
        //            }
        //            else if (res == Enums.ResourceType.GRAIN)
        //            {
        //                total += 2;
        //            }
        //            else if (res != Enums.ResourceType.NONE)
        //            {
        //                total++;
        //            }
        //        }
        //    }
        //}

        //// Check the amount against the bank
        //int bankAmount = bank.getResourceAmount(res);
        //if (bankAmount >= total)
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
        return true;
    }

    // Make sure there are enough commodities in the bank for a given dice roll
    private bool checkCommodities(Enums.CommodityType com, int n)
    {
        //graph.vertexReset(vertex1);
        //int total = 0;

        //foreach (Hex h in hexes)
        //{

        //    // If a hex isn't the right type or number, continue
        //    if (h.getHexNumber() != n)
        //    {
        //        continue;
        //    }

        //    Enums.HexType hType = h.getHexType();
        //    if (getCommodityFromHex(hType) != com)
        //    {
        //        continue;
        //    }

        //    // Get all the commodities accumulated by all players
        //    foreach (Vertex v in h.getVertices())
        //    {
        //        if (v.getVisited() != 0)
        //        {
        //            continue;
        //        }
        //        v.setVisited();

        //        GamePiece current = v.getOccupyingPiece();
        //        if (Object.ReferenceEquals(current, null))
        //        {
        //            continue;
        //        }
        //        if (current.getPieceType() == Enums.PieceType.CITY)
        //        {
        //            switch (com)
        //            {
        //                case Enums.CommodityType.NONE:
        //                    break;
        //                default:
        //                    total++;
        //                    break;
        //            }
        //        }
        //    }
        //}

        //// Check the amount against the bank
        //int bankAmount = bank.getCommodityAmount(com);
        //if (bankAmount >= total)
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
        return true;
    }
    
}
