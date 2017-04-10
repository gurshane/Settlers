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
    public int barbarianPos;

    private Hex pirateLocation;
	private Hex robberLocation;
	public Dictionary<Enums.DevChartType, Vertex> metropolises;

    public bool[] aqueducts;

	private const int numCommodities = 3;
	private const int numResources = 5;
	private const int numDevChartType = 3;
	private const int progressCardLimit = 4;
	private NetworkIdentity objNetId;

    // Overall Game State
    public List<Player> players;
    Graph graph;
    
    static public GameManager instance = null;

    public void Init() //initializer
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Objects length is: " + objects.Length + " _playerNumber is: " + GameObject.FindObjectOfType<LobbyManager>()._playerNumber + "...continuing");
        players = new List<Player>();
        ServerInitPlayers(objects);
        ClientInitPlayers(objects);
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
        playerTurn = 0;

        metropolises = new Dictionary<DevChartType, Vertex>();
        metropolises.Add(Enums.DevChartType.POLITICS, null);
        metropolises.Add(Enums.DevChartType.TRADE, null);
        metropolises.Add(Enums.DevChartType.SCIENCE, null);

        barbarianHasAttacked = false;
        barbarianPos = 0;
        graph = new Graph();

        Init();
    }

    public int getPlayerTurn()
    {
        return this.playerTurn;
    }

    public void setSpecialTurn(int turn, bool server) {
        // Assign client authority
        playerTurn = turn;
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
        resolveDice();
        removeAuthority(server);
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

    public void setGamePhase(GamePhase gPhase, bool server) {
        this.gamePhase = gPhase;
    }

    public void setMetropolisPlayer(Enums.DevChartType dev, Vertex v) {
        this.metropolises[dev] = v;
    }

    public Vertex getMetropolisAt(Enums.DevChartType dev) {
		return metropolises[dev];
	}

	public Hex getRobberLocation() {
		return robberLocation;
	}

    public int getBarbarianPosition() {
        return this.barbarianPos;
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
                getPersonalPlayer().changeVictoryPoints(-2, op.getID());
            }
        }

        ((City)p).makeMetropolis();
        getPersonalPlayer().changeVictoryPoints(2, getPlayer(player).getID());

        return true;
    }

    public void determineLongestRoute()
    {
    }

    public List<Player> getPlayers()
    {
        return this.players;
    }

    public Player getPersonalPlayer() {
        return GameObject.Find(Network.player.ipAddress).GetComponent<Player>();

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
                getPersonalPlayer().changeVictoryPoints(-1, p.getID());
            }
        }

        Player given = getPlayer(player);
        if (!Object.ReferenceEquals(given, null))
        {
            merchantController = player;
            getPersonalPlayer().changeVictoryPoints(1, given.getID());
            m.putOnBoard();
        }
    }

    public void sevenShortcut(int start){
        resolveSeven(start, getPersonalPlayer().getB1());
    }
   

    // Resolve a dice roll
    private void resolveDice()
    {

        bool attack = false;
        if ((eventDie == EventDie.BARBARIAN) && (barbarianPos + 1 == 7)) attack = true;

        getPersonalPlayer().setOldTurn(playerTurn);
        if (eventDie == EventDie.BARBARIAN) getPersonalPlayer().moveBarbarian();
        coloredEventDie();

        if (firstDie + secondDie == 7)
        {
            if (!attack) {                
                resolveSeven(0, false);
            } else {
                resolveSeven(0, true);
            }
        }
        else
        {
            if (!attack) {
                distribute(false);
            } else {
                distribute(true);
            }
        }
    }

    private void coloredEventDie() {
        if (eventDie == EventDie.BARBARIAN) return;
        foreach (Player p in players) {
            int[] devs = p.getDevFlipChart();
            if (firstDie <= devs[(int)eventDie-1] + 1) {
                Bank.instance.withdrawProgressCard((DevChartType)((int)eventDie - 1), p.getID());
            }
        }
    }



    // Resolve a seven if it is rolled
    private void resolveSeven(int start, bool barbarianAfter)
    {
        int old = getCurrentPlayer().getOldTurn();

        // If the barbarian has not attacked, nothing happens
        if (!barbarianHasAttacked)
        {
            if (!getPersonalPlayer().getB1()) {
                getPersonalPlayer().endPhaseOne();
            } else {
                GameManager.instance.barbarianAttack();
            }
            return;
        }

        for (int i = start; i < players.Count; i++)
        {
            Player p = players[i];
            if (p.getHandSize() > p.maxHandSize() ){

                foreach (Player p2 in players) {
                    getPersonalPlayer().setMoveType(MoveType.SPECIAL, p2.getID());
                }

                Debug.Log("old turn" + p.getOldTurn());

                getPersonalPlayer().setSpecial(Special.DISCARD_RESOURCE_SEVEN, p.getID());
                foreach (Player p2 in players) {
                    if(!Object.ReferenceEquals(p, p2)) getPersonalPlayer().setSpecial(Special.NONE, p2.getID());
                    if (barbarianAfter) getPersonalPlayer().setB1(true, p2.getID());
                    else getPersonalPlayer().setB1(false, p2.getID());
                }
                getPersonalPlayer().setSpecialTurn(p.getID());
                return;
            }
        }

        foreach (Player p2 in players) {
            getPersonalPlayer().setMoveType(MoveType.SPECIAL, p2.getID());
        }
        getPersonalPlayer().setSpecialTurn(old);
        getPersonalPlayer().setSpecial(Special.CHOOSE_PIRATE_OR_ROBBER, old);

        foreach (Player p2 in players) {
            if(!Object.ReferenceEquals(getPlayer(old), p2)) getPersonalPlayer().setSpecial(Special.NONE, p2.getID());
            if (barbarianAfter) getPersonalPlayer().setB1(true, p2.getID());
            else getPersonalPlayer().setB1(false, p2.getID());
        }
    }

    public void barbarianAttack()
    {
        int knightNum = 0;
        int citiesCount = 0;
        List<City> cities = new List<City>();
        foreach(Vertex v in BoardState.instance.vertexPosition.Values)
        {
            GamePiece gp = v.getOccupyingPiece();
            if(!Object.ReferenceEquals(gp, null))
            {
                if (gp.getPieceType() == PieceType.CITY) citiesCount++;
            }
        }

        foreach(Vertex v in BoardState.instance.vertexPosition.Values)
        {
            GamePiece gp = v.getOccupyingPiece();
            if(!Object.ReferenceEquals(gp, null))
            {
                if (gp.getPieceType() == PieceType.KNIGHT) {
                    Knight k = (Knight)gp;
                    if (k.isActive()) knightNum += k.getLevel();
                }
            }
        }
        if (knightNum >= citiesCount)
        {
            barbarianWin(0);
        }
        else
        {
            barbarianLoss(0);
        }
    }

    public void barbarianWinShortcut(int start) {
        barbarianWin(start);
    }

    private void barbarianWin(int start) {
        int[] playerContributions = new int[players.Count];
        foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
            GamePiece gp = v.getOccupyingPiece();
            if(!Object.ReferenceEquals(gp, null))
            {
                if (gp.getPieceType() == PieceType.KNIGHT) {
                    Knight k = (Knight)gp;
                    if (k.isActive()) playerContributions[(int)k.getColor()] += k.getLevel();
                }
            }
        }

        int bestPlayer = 0;
        bool tie = false;
        for (int i = 1; i < players.Count; i++) {
            if (playerContributions[i] > playerContributions[bestPlayer]) {
                bestPlayer = i;
                tie = false;
            } 
            else if (playerContributions[i] == playerContributions[bestPlayer]) tie = true;
        }

        if (!tie) {
            getPersonalPlayer().changeVictoryPoints(1, bestPlayer);
        } else {
            for (int i = start; i < players.Count; i++)
            {
                if (playerContributions[i] < playerContributions[bestPlayer]) continue;

                Player p = players[i];

                foreach (Player p2 in players) {
                    getPersonalPlayer().setMoveType(MoveType.SPECIAL, p2.getID());
                }

                getPersonalPlayer().setSpecial(Special.CHOOSE_PROGRESS_PILE, p.getID());
                foreach (Player p2 in players) {
                    if(!Object.ReferenceEquals(p, p2)) getPersonalPlayer().setSpecial(Special.NONE, p2.getID());
                }
                getPersonalPlayer().setSpecialTurn(p.getID());
                return;
            }
        }

        foreach(Player p in getPlayers()) {
            getPersonalPlayer().setSpecial(Special.NONE, p.getID());
            getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
        }
        getPersonalPlayer().revertTurn();
        getPersonalPlayer().endPhaseOne();
    }

    public void barbarianLossShortcut(int start) {
        barbarianLoss(start);
    }

    private void barbarianLoss(int start) {
        Debug.Log("bloss");
        int[] playerContributions = new int[players.Count];
        foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
            GamePiece gp = v.getOccupyingPiece();
            if(!Object.ReferenceEquals(gp, null))
            {
                if (gp.getPieceType() == PieceType.KNIGHT) {
                    Knight k = (Knight)gp;
                    if (k.isActive()) playerContributions[(int)k.getColor()] += k.getLevel();
                }
            }
        }

        bool[] possibleDestruction = new bool[players.Count];
        for (int i = 0; i < players.Count; i++) {
            possibleDestruction[i] = false;
        }
        foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
            GamePiece gp = v.getOccupyingPiece();
            if(!Object.ReferenceEquals(gp, null))
            {
                if (gp.getPieceType() == PieceType.CITY) {
                    City c = (City)gp;
                    if (!c.isMetropolis()) possibleDestruction[(int)c.getColor()] = true;
                }
            }
        }

        int worstPlayer = -1;
        for (int i = 0; i < players.Count; i++) {
            if (possibleDestruction[i]) {
                worstPlayer = i;
                break; 
            } 
        }

        Debug.Log("wp:" + worstPlayer);
        if (worstPlayer == -1) {
            Debug.Log("wp-1");
            foreach(Player p in getPlayers()) {
                getPersonalPlayer().setSpecial(Special.NONE, p.getID());
                getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
            }
            getPersonalPlayer().revertTurn();
            getPersonalPlayer().endPhaseOne();
            return;
        }

        for (int i = 0; i < players.Count; i++) {
            if (possibleDestruction[i] && (playerContributions[i] < playerContributions[worstPlayer])) {
                worstPlayer = i;
            } 
        }

        for (int i = start; i < players.Count; i++)
        {
            if (!possibleDestruction[i] || playerContributions[i] > playerContributions[worstPlayer]) continue;

            Player p = players[i];

            foreach (Player p2 in players) {
                getPersonalPlayer().setMoveType(MoveType.SPECIAL, p2.getID());
            }

            getPersonalPlayer().setSpecial(Special.CHOOSE_DESTROYED_CITY, p.getID());
            foreach (Player p2 in players) {
                if(!Object.ReferenceEquals(p, p2)) getPersonalPlayer().setSpecial(Special.NONE, p2.getID());
            }
            getPersonalPlayer().setSpecialTurn(p.getID());
            return;
        }

        foreach(Player p in getPlayers()) {
            getPersonalPlayer().setSpecial(Special.NONE, p.getID());
            getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
        }
        getPersonalPlayer().deactivateKnights();
        getPersonalPlayer().revertTurn();
        getPersonalPlayer().endPhaseOne();
    }

    // Distribute the appropriate resources to all players
    private void distribute(bool barbarianAfter)
    {
        int num = firstDie + secondDie;

        // Make sure there are enough resources and commodities in the bank
        Dictionary<Enums.ResourceType, bool> enoughRes = new Dictionary<Enums.ResourceType, bool>();
        Dictionary<Enums.CommodityType, bool> enoughComs = new Dictionary<Enums.CommodityType, bool>();
        
        bool[] playerAqueducts = new bool[players.Count];
        for (int i = 0; i < players.Count; i++) {
            if (getPlayer(i).getAqueduct()) playerAqueducts[i] = true;
            else playerAqueducts[i] = false;
        }

        for (int i = 0; i < numResources; i++)
        {
            enoughRes.Add((Enums.ResourceType)i, checkResources((Enums.ResourceType)i, num));
        }
        for (int i = 0; i < numCommodities; i++)
        {
            enoughComs.Add((Enums.CommodityType)i, checkCommodities((Enums.CommodityType)i, num));
        }

        foreach (Hex h in BoardState.instance.hexPosition.Values)
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
            if (res == Enums.ResourceType.NONE && hType != Enums.HexType.DESERT)
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
                    if(hType == Enums.HexType.DESERT)
                    {
                        Debug.Log("single fish");
                        int numFish = Bank.instance.getFishToken(p.isServer);
                        getPersonalPlayer().changeFishCount(numFish, p.getID());
                        playerAqueducts[p.getID()] = false;
                    }
                    else if (res != Enums.ResourceType.NONE && enoughRes[res])
                    {
                        Bank.instance.withdrawResource(res, 1, p.isServer);
                        getPersonalPlayer().changeResource(res, 1, p.getID());
                        playerAqueducts[p.getID()] = false;
                    }
                    else if (gold)
                    {
                        getPersonalPlayer().changeGoldCount(2, p.getID());
                        playerAqueducts[p.getID()] = false;
                    }
                }

                // Distribute resources and commodities for cities
                if (current.getPieceType() == Enums.PieceType.CITY)
                {
                    Enums.Color ownerColor = current.getColor();
                    Player p = getPlayer(ownerColor);
                    if(hType == Enums.HexType.DESERT)
                    {
                        Debug.Log("fish");
                        int numFish = Bank.instance.getFishTokens(p.isServer);
                        getPersonalPlayer().changeFishCount(numFish, p.getID());
                        playerAqueducts[p.getID()] = false;
                    }
                    if (com != Enums.CommodityType.NONE)
                    {
                        if (enoughRes[res])
                        {
                            Bank.instance.withdrawResource(res, 1, p.isServer);
                            getPersonalPlayer().changeResource(res, 1, p.getID());
                            playerAqueducts[p.getID()] = false;
                        }
                        if (enoughComs[com])
                        {
                            Bank.instance.withdrawCommodity(com, 1, p.isServer);
                            getPersonalPlayer().changeCommodity(com, 1, p.getID());
                            playerAqueducts[p.getID()] = false;
                        }
                    }
                    else if (res == Enums.ResourceType.BRICK && enoughRes[res])
                    {
                        Bank.instance.withdrawResource(res, 2, p.isServer);
                        getPersonalPlayer().changeResource(res, 2, p.getID());
                        playerAqueducts[p.getID()] = false;
                    }
                    else if (res == Enums.ResourceType.GRAIN && enoughRes[res])
                    {
                        Bank.instance.withdrawResource(res, 2, p.isServer);
                        getPersonalPlayer().changeResource(res, 2, p.getID());
                        playerAqueducts[p.getID()] = false;
                    }
                    else if (gold)
                    {
                        getPersonalPlayer().changeGoldCount(4, p.getID());
                        playerAqueducts[p.getID()] = false;
                    }
                }
            }
        }
        getPersonalPlayer().createAqueductArray(playerAqueducts);
        aqueducts = playerAqueducts;
        giveAqueduct(0, barbarianAfter);
    }

    public void aqueductShortcut(int start) {
        giveAqueduct(start, getPersonalPlayer().getB1());
    }

    private void giveAqueduct(int start, bool barbarianAfter) {

        for (int i = start; i < players.Count; i++)
        {
            if (!aqueducts[i]) continue;

            Player p = players[i];

            foreach (Player p2 in players) {
                getPersonalPlayer().setMoveType(MoveType.SPECIAL, p2.getID());
            }

            getPersonalPlayer().setSpecial(Special.AQUEDUCT, p.getID());
            foreach (Player p2 in players) {
                if(!Object.ReferenceEquals(p, p2)) getPersonalPlayer().setSpecial(Special.NONE, p2.getID());
                if (barbarianAfter) getPersonalPlayer().setB1(true, p2.getID());
                else getPersonalPlayer().setB1(false, p2.getID());
            }
            getPersonalPlayer().setSpecialTurn(p.getID());
            return;
        }

        if (!barbarianAfter) {
            foreach(Player p in GameManager.instance.getPlayers()) {
                getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
                getPersonalPlayer().setSpecial(Special.NONE, p.getID());
            }
            getPersonalPlayer().revertTurn();
            getPersonalPlayer().endPhaseOne();
        } else {
            GameManager.instance.barbarianAttack();
        }
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

        foreach (Hex h in BoardState.instance.hexPosition.Values)
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

        foreach (Hex h in BoardState.instance.hexPosition.Values)
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
