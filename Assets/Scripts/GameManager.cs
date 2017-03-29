using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;
using Prototype.NetworkLobby; 

public class GameManager : NetworkBehaviour {

    [SyncVar]
	public Enums.GamePhase gamePhase;

    [SyncVar]
	public int pointsToWin;
    
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
    public int playerTurn;

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
	private NetworkIdentity objNetId;

    // Overall Game State
    private List<Player> players;
    Graph graph;
    
    static public GameManager instance = null;

    public void Init() //initializer
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
        if (objects.Length != GameObject.FindObjectOfType<LobbyManager>()._playerNumber)
        {
            Debug.Log("Objects length is: " + objects.Length + " _playerNumber is: " + GameObject.FindObjectOfType<LobbyManager>()._playerNumber + "...returning");
            return;
        }
        Debug.Log("Objects length is: " + objects.Length + " _playerNumber is: " + GameObject.FindObjectOfType<LobbyManager>()._playerNumber + "...continuing");
        players = new List<Player>();
        ServerInitPlayers(objects);
        ClientInitPlayers(objects);
        Debug.Log("Players count: " + players.Count);
        //firstTurn();
        //secondTurn();
    }

    [Server]
    private void ServerInitPlayers(GameObject [] objects)
    {
        Debug.Log(objects.Length);
        for (int i = 0; i < objects.Length; i++)
        {
            Player player = objects[i].GetComponent<Player>();
            player.CmdInit(i);
			player.Init ();
            players.Add (player);
            
        }
    }

    [Client]
    private void ClientInitPlayers(GameObject[] objects)
    {
        if (isServer && isClient)
        {
            return;
        }
        Debug.Log("Number of objects tagged Player: " + objects.Length);//why only 1 player object found?
        //client connections delay issues, how to delay until all connections present?
        //why isnt syncvar properly working on player objects for iD number?????    
        Player[] temp = new Player[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            Player player = objects[i].GetComponent<Player>();
            //player.getGm();
            temp[player.getID()] = player;//error because -1 
        }
        for (int i = 0; i<temp.Length; i++)
        {
			temp [i].Init ();
            players.Add(temp[i]);
        }
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (isClient)
        {
            Debug.Log("Client game manager start");
        }
        
        players = new List<Player>();
        gamePhase = Enums.GamePhase.SETUP_ONE;
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

        Init();
    }

    public int getPlayerTurn()
    {
        return this.playerTurn;
    }
	
    // Move to next turn based on game phase
	public void SetPlayerTurn(bool server)
    {

        // Assign client authority
        assignAuthority(server);
	
        // Increment gamephase
		if (this.gamePhase == Enums.GamePhase.SETUP_ONE) {
			playerTurn++;
			if (this.playerTurn >= players.Count) {
				playerTurn = players.Count - 1;
				gamePhase = Enums.GamePhase.SETUP_TWO;
			}
		} else if (this.gamePhase == Enums.GamePhase.SETUP_TWO) {
			playerTurn--;
			if (this.playerTurn < 0) {
				playerTurn = 0;
				gamePhase = Enums.GamePhase.PHASE_ONE;
			}
		} else {
			playerTurn++;
			if (this.playerTurn >= players.Count) {
				playerTurn = 0;
			}
            gamePhase = Enums.GamePhase.PHASE_ONE;
		}

        removeAuthority(server);
    }

    public void DiceRolled(bool server)
    {
        assignAuthority(server);
        firstDie = UnityEngine.Random.Range(1, 7);
        secondDie = UnityEngine.Random.Range(1, 7);
        int thirdDie = UnityEngine.Random.Range(0, 2);//eventDie
        if (thirdDie == 1) {
            eventDie = Enums.EventDie.BARBARIAN;
        } else {
            thirdDie = UnityEngine.Random.Range(1, 4);
            eventDie = (Enums.EventDie)thirdDie;
        }
        removeAuthority(server);
        resolveDice();
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

    public List<Player> getPlayers()
    {
        return this.players;
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

   

    // Resolve a dice roll
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
        int num = firstDie + secondDie;

        // Make sure there are enough resources and commodities in the bank
        Dictionary<Enums.ResourceType, bool> enoughRes = new Dictionary<Enums.ResourceType, bool>();
        Dictionary<Enums.CommodityType, bool> enoughComs = new Dictionary<Enums.CommodityType, bool>();

        for (int i = 0; i < numResources; i++)
        {
            enoughRes.Add((Enums.ResourceType)i, checkResources((Enums.ResourceType)i, num));
        }
        for (int i = 0; i < numCommodities; i++)
        {
            enoughComs.Add((Enums.CommodityType)i, checkCommodities((Enums.CommodityType)i, num));
        }

        foreach (Hex h in BoardState.instance.hexPoisition.Values)
        {
            // If a hex isn't the right number, or doesn't produce cards, continue
            if (h.getHexNumber() != num)
            {
                continue;
            }

            if (Object.ReferenceEquals(h, robberLocation))
            {
                continue;
            }
            Enums.HexType hType = h.getHexType();

            // Check if a hex produces gold
            bool gold = false;
            if (hType == Enums.HexType.GOLD)
            {
                gold = true;
            }

            Enums.ResourceType res = getResourceFromHex(hType);
            Enums.CommodityType com = getCommodityFromHex(hType);
            if (res == Enums.ResourceType.NONE)
            {
                continue;
            }

            // Distribute all the resources
            foreach (Vertex v in h.getVertices())
            {
                GamePiece current = v.getOccupyingPiece();
                if (Object.ReferenceEquals(current, null))
                {
                    continue;
                }

                // Distribue resources for settlements
                if (current.getPieceType() == Enums.PieceType.SETTLEMENT)
                {
                    Debug.Log("Hex type: " + h.getHexType() + ", enough: " + enoughRes[res]);
                    Enums.Color ownerColor = current.getColor();
                    Player p = getPlayer(ownerColor);
                    if (res != Enums.ResourceType.NONE && enoughRes[res])
                    {
                        Bank.instance.withdrawResource(res, 1, p.isServer);
                        p.changeResource(res, 1);
                    }
                    else if (gold)
                    {
                        p.changeGoldCount(2);
                    }
                }

                // Distribute resources and commodities for cities
                if (current.getPieceType() == Enums.PieceType.CITY)
                {
                    Enums.Color ownerColor = current.getColor();
                    Player p = getPlayer(ownerColor);
                    if (com != Enums.CommodityType.NONE)
                    {
                        if (enoughRes[res])
                        {
                            Bank.instance.withdrawResource(res, 1, p.isServer);
                            p.changeResource(res, 1);
                        }
                        if (enoughComs[com])
                        {
                            Bank.instance.withdrawCommodity(com, 1, p.isServer);
                            p.changeCommodity(com, 1);
                        }
                    }
                    else if (res == Enums.ResourceType.BRICK && enoughRes[res])
                    {
                        Bank.instance.withdrawResource(res, 2, p.isServer);
                        p.changeResource(res, 2);
                    }
                    else if (res == Enums.ResourceType.GRAIN && enoughRes[res])
                    {
                        Bank.instance.withdrawResource(res, 2, p.isServer);
                        p.changeResource(res, 2);
                    }
                    else if (gold)
                    {
                        p.changeGoldCount(2);
                    }
                }
            }
        }
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
        int total = 0;

        foreach (Hex h in BoardState.instance.hexPoisition.Values)
        {

            // If a hex isn't the right type or number, continue
            if (h.getHexNumber() != n)
            {
                continue;
            }

            Enums.HexType hType = h.getHexType();
            if (getResourceFromHex(hType) != res)
            {
                continue;
            }

            // Get all the resources accumulated by all players
            foreach (Vertex v in h.getVertices())
            {
                GamePiece current = v.getOccupyingPiece();
                if (Object.ReferenceEquals(current, null))
                {
                    continue;
                }
                if (current.getPieceType() == Enums.PieceType.SETTLEMENT)
                {
                    total++;
                }
                if (current.getPieceType() == Enums.PieceType.CITY)
                {
                    if (res == Enums.ResourceType.BRICK)
                    {
                        total += 2;
                    }
                    else if (res == Enums.ResourceType.GRAIN)
                    {
                        total += 2;
                    }
                    else if (res != Enums.ResourceType.NONE)
                    {
                        total++;
                    }
                }
            }
        }

        // Check the amount against the bank
        int bankAmount = Bank.instance.getResourceAmount(res);
        if (bankAmount >= total)
        {
            return true;
        }
        else
        {
            return false;
        }
        return true;
    }

    // Make sure there are enough commodities in the bank for a given dice roll
    private bool checkCommodities(Enums.CommodityType com, int n)
    {
        int total = 0;

        foreach (Hex h in BoardState.instance.hexPoisition.Values)
        {

            // If a hex isn't the right type or number, continue
            if (h.getHexNumber() != n)
            {
                continue;
            }

            Enums.HexType hType = h.getHexType();
            if (getCommodityFromHex(hType) != com)
            {
                continue;
            }

            // Get all the commodities accumulated by all players
            foreach (Vertex v in h.getVertices())
            {
                GamePiece current = v.getOccupyingPiece();
                if (Object.ReferenceEquals(current, null))
                {
                    continue;
                }
                if (current.getPieceType() == Enums.PieceType.CITY)
                {
                    switch (com)
                    {
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
        int bankAmount = Bank.instance.getCommodityAmount(com);
        if (bankAmount >= total)
        {
            return true;
        }
        else
        {
            return false;
        }
        return true;
    }

    private void assignAuthority(bool server) {
        if (!server) {
			objNetId = this.GetComponent<NetworkIdentity> ();     
			objNetId.AssignClientAuthority (connectionToClient);   
		}
    }

    private void removeAuthority(bool server) {
        if (!server) {
			objNetId.RemoveClientAuthority (connectionToClient); 
		}
    }
    
}
