using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class ProgressCards : NetworkBehaviour {

	MoveAuthorizer ma;
	ProgressAuthroizor pa;
	Graph graph;
	private NetworkIdentity objNetId;
	static public ProgressCards instance = null;

	void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ma = new MoveAuthorizer();
		pa = new ProgressAuthroizor();
		graph = new Graph ();
    }

	// Move a knight from source to target
	public bool bishop(Hex target, Enums.Color color, bool server)
    {

		// Check if the knight can be moved
		if (!ma.canMoveRobber (target))
        {
			return false;
		}

		Player current = GameManager.instance.getCurrentPlayer();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {

			bool nextToRobber = false;
			if(target.adjacentToVertex(v)) {
				if (!Object.ReferenceEquals(v.getOccupyingPiece(), null)) {
					if (v.getOccupyingPiece().getColor() != color) {
						if(v.getOccupyingPiece().getPieceType() == PieceType.CITY ||
							v.getOccupyingPiece().getPieceType() == PieceType.SETTLEMENT) {

							nextToRobber = true;
						}
					}
				}
			}

			if (nextToRobber) {
				int opp = (int)v.getOccupyingPiece().getColor();
				Player oppo  = GameManager.instance.getPlayer(opp);
				bool taken = false;
				for (int i = 0; i < 5; i++) {
					if(oppo.getResources()[i] > 0) {
						GameManager.instance.getPersonalPlayer().changeResource((ResourceType)i, -1, oppo.getID());
						taken = true;
						GameManager.instance.getPersonalPlayer().changeResource((ResourceType)i, 1, current.getID());
						break;
					}
				}
				if (!taken) {
					for (int i = 0; i < 3; i++) {
						if(oppo.getCommodities()[i] > 0) {
							GameManager.instance.getPersonalPlayer().changeCommodity((CommodityType)i, -1, oppo.getID());
							GameManager.instance.getPersonalPlayer().changeCommodity((CommodityType)i, 1, current.getID());
							break;
						}
					}
				}
			}
		}

		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.BISHOP, current.getID());

		assignAuthority(server);
        RpcBishop(target.transform.position);
		removeAuthority(server);

		return true;
	}

    [ClientRpc]
	void RpcBishop(Vector3 target)
    {

		Hex source = null;
		GamePiece piece = null;
		foreach (Hex h in BoardState.instance.hexPosition.Values) {
			piece = h.getOccupyingPiece();
			if (!Object.ReferenceEquals(piece, null)) {
				if (piece.getPieceType() == Enums.PieceType.ROBBER) {
					source = h;
					break;
				} else {
					piece = null;
				}
			}
		}

		Hex targetLocation = BoardState.instance.hexPosition[target];
		if (!Object.ReferenceEquals(piece, null)) {
			targetLocation.setOccupyingPiece(piece);
		} else {
			targetLocation.setOccupyingPiece(new Robber());
		}

		GameObject newRobber = Instantiate<GameObject>(PrefabHolder.instance.robber, target, Quaternion.identity);
        newRobber.transform.position += new Vector3(0f, 10f, 0f);
		BoardState.instance.spawnedObjects.Add(target, newRobber);

		if (!Object.ReferenceEquals(piece, null)) {
			source.setOccupyingPiece(null);
			Destroy (BoardState.instance.spawnedObjects [source.transform.position]);
			BoardState.instance.spawnedObjects.Remove(source.transform.position);
		}
    }

	public void constitution()
    {

		Player current = GameManager.instance.getCurrentPlayer();

		GameManager.instance.getPersonalPlayer().changeVictoryPoints(1, current.getID());

		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.CONSTITUTION, current.getID());

	}

	public void printer()
    {

		Player current = GameManager.instance.getCurrentPlayer();

		GameManager.instance.getPersonalPlayer().changeVictoryPoints(1, current.getID());

		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.PRINTER, current.getID());

	}

	// Move a knight from source to target
	public bool diplomat(Edge source, Edge target, Enums.Color color, bool server)
    {

		if (!pa.canRoadMove (source, target, color))
        {
			return false;
		}
		assignAuthority(server);
        RpcDiplomat(source.transform.position, target.transform.position, color);

		Player current = GameManager.instance.getCurrentPlayer();
		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.DIPLOMAT, current.getID());

		removeAuthority(server);
		return true;
	}

    [ClientRpc]
	void RpcDiplomat(Vector3 source, Vector3 target, Enums.Color color)
    {

		Edge sourcePiece = BoardState.instance.edgePosition [source];
        Edge targetPiece = BoardState.instance.edgePosition [target];

		GameObject spawnedRoad = Instantiate<GameObject>(PrefabHolder.instance.road, target, Quaternion.identity);
        fixPieceRotationAndPosition(spawnedRoad);
        if (targetPiece.isRightPointing)
        {
            spawnedRoad.transform.Rotate(0f, 0f, -29f);
        }
        else if (targetPiece.isLeftPointing)
        {
            spawnedRoad.transform.Rotate(0f, 0f, 31f);
        }
        else if (targetPiece.isForwardPointing)
        {
            spawnedRoad.transform.Rotate(0f, 0f, 90f);
        }
        spawnedRoad.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		Destroy (BoardState.instance.spawnedObjects [source]);

		BoardState.instance.spawnedObjects.Add(target, spawnedRoad);
		BoardState.instance.spawnedObjects.Remove (source);

        Road movedRoad = (Road)sourcePiece.getOccupyingPiece();

        // Move the knight
        sourcePiece.setOccupyingPiece(null);
        targetPiece.setOccupyingPiece(movedRoad);
    }

	// Displace a knight at target with knight at source
	public bool intrigue(Vertex target, Enums.Color color, bool server)
    {
		// Check if the knight can displace
		if (!pa.canIntrigueKnight (target, color))
        {
			return false;
		}

		Knight kTarget = (Knight)target.getOccupyingPiece ();
		int targetLevel = kTarget.getLevel ();

		bool gone = true;
		foreach (Vertex v in BoardState.instance.vertexPosition.Values)
        {
			if (graph.areConnectedVertices (v, target, kTarget.getColor()))
            {
				if (Object.ReferenceEquals (v.getOccupyingPiece (), null))
                {
					gone = false;
					break;
				}
			}
		}

		if (!gone) {

			int currTurn = GameManager.instance.getPlayerTurn();
			GameManager.instance.getPersonalPlayer().setOldTurn(currTurn);

			foreach (Player p in GameManager.instance.players) {
				GameManager.instance.getPersonalPlayer().setMoveType(MoveType.SPECIAL, p.getID());
			}

			Player opponent = GameManager.instance.getPlayer((int)kTarget.getColor());
			GameManager.instance.getPersonalPlayer().setSpecial(Special.KNIGHT_DISPLACED, opponent.getID());
			GameManager.instance.getPersonalPlayer().setI1(targetLevel, opponent.getID());
			GameManager.instance.getPersonalPlayer().setB1(kTarget.isActive(), opponent.getID());
			GameManager.instance.getPersonalPlayer().SetV1(target, opponent.getID());
			GameManager.instance.getPersonalPlayer().setSpecialTurn((int)kTarget.getColor());
		}
		Player current = GameManager.instance.getCurrentPlayer();
		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.INTRIGUE, current.getID());

		assignAuthority(server);
		RpcIntrigue(target.transform.position, targetLevel, color);
		removeAuthority(server);
		return true;
	}

    [ClientRpc]
	void RpcIntrigue(Vector3 target, int targetLevel, Enums.Color color)
    {
        Vertex targetPiece = BoardState.instance.vertexPosition[target];
        Knight targetKnight = (Knight)targetPiece.getOccupyingPiece();

		Destroy (BoardState.instance.spawnedObjects [target]);
		BoardState.instance.spawnedObjects.Remove(target);
        
        targetPiece.setOccupyingPiece(null);
		targetKnight.takeOffBoard();

    }

	public bool crane(Enums.DevChartType dev, int[] commodities, 
		List<GamePiece> pieces, int[] devChart, bool server)
    {
		// Check if the knight can be upgraded
		if (!pa.canCrane (dev, commodities, pieces, devChart))
        {
			return false;
		}

		Player current = GameManager.instance.getCurrentPlayer();

		Enums.CommodityType com = (CommodityType)((int)dev);
		int level = devChart[(int)dev];
		int newLevel = level + 1;

		// Spend the correct resources
        GameManager.instance.getPersonalPlayer().changeCommodity(com, -level, current.getID());
        Bank.instance.depositCommodity(com, level, current.isServer);

		GameManager.instance.getPersonalPlayer().upgradeDevChart(dev, current.getID());

		if (newLevel == 3) {
			if (dev == Enums.DevChartType.TRADE) {
				for (int i = 0; i < 3; i++) {
					GameManager.instance.getPersonalPlayer().updateCommodityRatio((CommodityType)i, 2, current.getID());
				}
			} else if (dev == Enums.DevChartType.SCIENCE) {
				GameManager.instance.getPersonalPlayer().makeAqueduct(current.getID());
			}
		} 
		
		bool check0 = false;
		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			GamePiece city = v.getOccupyingPiece();
			if (!Object.ReferenceEquals(city, null)) {
				if (city.getPieceType() == PieceType.CITY) {
					City c = (City)city;
					if (!c.isMetropolis()) {
						if (c.getColor() == GameManager.instance.getCurrentPlayer().getColor()) check0 = true;
					}
				}
			}
		}
		
		if (newLevel == 4 && Object.ReferenceEquals(GameManager.instance.getMetropolisAt(dev), null) && check0) {

			Debug.Log("numPlayers");
			foreach (Player p2 in GameManager.instance.getPlayers()) {
				Debug.Log("setmovetype");
				GameManager.instance.getPersonalPlayer().setMoveType(MoveType.SPECIAL, p2.getID());
			}
			int ident = GameManager.instance.getCurrentPlayer().getID();

			GameManager.instance.getPersonalPlayer().setSpecial(Special.CHOOSE_METROPOLIS, ident);
			GameManager.instance.getPersonalPlayer().setMetropolis(dev, ident);
			
			foreach (Player p2 in GameManager.instance.getPlayers()) {
				if(!Object.ReferenceEquals(GameManager.instance.getPlayer(ident), p2)) GameManager.instance.getPersonalPlayer().setSpecial(Special.NONE, p2.getID());
			}
		}

		bool check1 = (newLevel == 5);
		bool check2 = Object.ReferenceEquals(GameManager.instance.getMetropolisAt(dev), null);
		bool check3 = false;
		bool check4 = false;
		Vertex currentMet = GameManager.instance.getMetropolisAt(dev);
		if (!Object.ReferenceEquals(currentMet, null)) {
			GamePiece c = currentMet.getOccupyingPiece();
			int i = (int)c.getColor(); 
			int[] oppDevChart = GameManager.instance.getPlayer(i).getDevFlipChart();
			if (oppDevChart[(int)dev] < 5) check3 = true;
			if (i == GameManager.instance.getCurrentPlayer().getID()) check4 = true;
		}

		if (check1 && (check2 || check3) && !check4 && check0) {
			foreach (Player p2 in GameManager.instance.getPlayers()) {
				GameManager.instance.getPersonalPlayer().setMoveType(MoveType.SPECIAL, p2.getID());
			}
			int ident = GameManager.instance.getCurrentPlayer().getID();

			GameManager.instance.getPersonalPlayer().setSpecial(Special.CHOOSE_METROPOLIS, ident);
			GameManager.instance.getPersonalPlayer().setMetropolis(dev, ident);
			
			foreach (Player p2 in GameManager.instance.getPlayers()) {
				if(!Object.ReferenceEquals(GameManager.instance.getPlayer(ident), p2)) GameManager.instance.getPersonalPlayer().setSpecial(Special.NONE, p2.getID());
			}
		}

		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.CRANE, current.getID());

		return true;
	}

	public bool engineer(Vertex location, int cityWalls, Enums.Color color, bool server)
    {
		if (!pa.canEngineer (location, cityWalls, color))
        {
			return false;
		}

		assignAuthority(server);
        RpcEngineer(location.transform.position, color);

		Player current = GameManager.instance.getCurrentPlayer();

		GameManager.instance.getPersonalPlayer().changeCityWallCount(-1, current.getID());

		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.ENGINEER, current.getID());

		removeAuthority(server);

        return true;
	}

	[ClientRpc]
	void RpcEngineer(Vector3 location, Enums.Color color)
    {
		// Remove the current settlement
        Vertex source = BoardState.instance.vertexPosition[location];
        City city = (City)source.getOccupyingPiece();

		Destroy (BoardState.instance.spawnedObjects [location]);
		BoardState.instance.spawnedObjects.Remove(location);

		GameObject spawnedCityWall;
		if (!city.isMetropolis()) { 
			spawnedCityWall = Instantiate<GameObject>(PrefabHolder.instance.cityWithCityWall, location, Quaternion.identity);
        	fixPieceRotationAndPosition(spawnedCityWall);

			spawnedCityWall.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
			foreach(MeshRenderer meshRend in spawnedCityWall.GetComponentsInChildren<MeshRenderer>())
			{
				meshRend.material.SetColor("_Color", translateColor(color));
			}
		} else {
			spawnedCityWall = Instantiate<GameObject>(PrefabHolder.instance.metropolisWithCityWall, location, Quaternion.identity);
        	fixPieceRotationAndPosition(spawnedCityWall);

			if (Object.ReferenceEquals(GameManager.instance.getMetropolisAt(DevChartType.TRADE), source)) {
				spawnedCityWall.GetComponent<MeshRenderer>().material.SetColor("_Color", UnityEngine.Color.yellow);
				foreach(MeshRenderer meshRend in spawnedCityWall.GetComponentsInChildren<MeshRenderer>())
				{
					meshRend.material.SetColor("_Color", UnityEngine.Color.yellow);
				}
			} else if (Object.ReferenceEquals(GameManager.instance.getMetropolisAt(DevChartType.POLITICS), source)) {
				spawnedCityWall.GetComponent<MeshRenderer>().material.SetColor("_Color", UnityEngine.Color.blue);
				foreach(MeshRenderer meshRend in spawnedCityWall.GetComponentsInChildren<MeshRenderer>())
				{
					meshRend.material.SetColor("_Color", UnityEngine.Color.blue);
				}
			} else {
				spawnedCityWall.GetComponent<MeshRenderer>().material.SetColor("_Color", UnityEngine.Color.green);
				foreach(MeshRenderer meshRend in spawnedCityWall.GetComponentsInChildren<MeshRenderer>())
				{
					meshRend.material.SetColor("_Color", UnityEngine.Color.green);
				}
			}
		}

        BoardState.instance.spawnedObjects.Add(location, spawnedCityWall);

		source.addWall();

    }

	public bool inventor(Hex source, Hex target, bool server)
    {
		if (!pa.canInventor (source, target))
        {
			return false;
		}

		assignAuthority(server);
        RpcInventor(source.transform.position, target.transform.position);

		Player current = GameManager.instance.getCurrentPlayer();
		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.INVENTOR, current.getID());
		removeAuthority(server);

        return true;
	}

	[ClientRpc]
	void RpcInventor(Vector3 source, Vector3 target)
    {
		Hex s = BoardState.instance.hexPosition[source];
		Hex t = BoardState.instance.hexPosition[target];

		int temp = s.getHexNumber();
		s.setHexNumber(t.getHexNumber());
		t.setHexNumber(temp);
    }

	public void irrigation(Enums.Color color) {

		foreach(Hex h in BoardState.instance.hexPosition.Values) {
			if (h.getHexType() != HexType.FIELD) continue;
			foreach (Vertex v in h.getVertices()) {
				GamePiece piece = v.getOccupyingPiece();
				if(!Object.ReferenceEquals(piece, null)) {
					if (piece.getColor() == color) {
						Player current = GameManager.instance.getCurrentPlayer();
						GameManager.instance.getPersonalPlayer().changeResource(ResourceType.GRAIN, 2, current.getID());
						Bank.instance.withdrawResource(ResourceType.GRAIN, 2, current.isServer);
						break;
					}
				}
			}
		}
		Player curr = GameManager.instance.getCurrentPlayer();
		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.IRRIGATION, curr.getID());
	}

	public void mining(Enums.Color color) {
		foreach(Hex h in BoardState.instance.hexPosition.Values) {
			if (h.getHexType() != HexType.MOUNTAIN) continue;
			foreach (Vertex v in h.getVertices()) {
				GamePiece piece = v.getOccupyingPiece();
				if(!Object.ReferenceEquals(piece, null)) {
					if (piece.getColor() == color) {
						Player current = GameManager.instance.getCurrentPlayer();
						GameManager.instance.getPersonalPlayer().changeResource(ResourceType.ORE, 2, current.getID());
						Bank.instance.withdrawResource(ResourceType.ORE, 2, current.isServer);
						break;
					}
				}
			}
		}

		Player curr = GameManager.instance.getCurrentPlayer();
		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.MINING, curr.getID());
	}

	// Get a unity color from an enums color
	private UnityEngine.Color translateColor(Enums.Color color) {
		switch (color)
		{
		case Enums.Color.RED:
			return UnityEngine.Color.red;
		case Enums.Color.BLUE:
			return UnityEngine.Color.blue; 
		case Enums.Color.ORANGE:
			return UnityEngine.Color.green;
		case Enums.Color.WHITE:
			return UnityEngine.Color.white;
		default:
			return UnityEngine.Color.gray;
		}
	}

	// Assign client authority
	private void assignAuthority(bool server) {
        if (!server) {
			objNetId = this.GetComponent<NetworkIdentity> ();     
			objNetId.AssignClientAuthority (connectionToClient);   
		}
    }

	// Remove client authority
    private void removeAuthority(bool server) {
        if (!server) {
			objNetId.RemoveClientAuthority (connectionToClient); 
		}
    }

	void fixPieceRotationAndPosition(GameObject go)
    {
        go.transform.position = go.transform.position + (new Vector3(0f, 10f, 0f));
        go.transform.Rotate(-90f, 0f, 0f);
    }

	public bool medicine(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color, bool server)
    {

		if (!pa.canMedicine (location, resources, pieces, color))
        {
			return false;
		}

		assignAuthority(server);
        RpcMedicine(location.transform.position, color);

		Player current = GameManager.instance.getCurrentPlayer();

		// Add an appropriate amount of victory points
        GameManager.instance.getPersonalPlayer().changeVictoryPoints(1, current.getID());

        // Spend the resources
        GameManager.instance.getPersonalPlayer().changeResource(Enums.ResourceType.GRAIN, -1, current.getID());
        Bank.instance.depositResource(Enums.ResourceType.GRAIN, 1, current.isServer);

        GameManager.instance.getPersonalPlayer().changeResource(Enums.ResourceType.ORE, -2, current.getID());
        Bank.instance.depositResource(Enums.ResourceType.ORE, 2, current.isServer);

		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.MEDICINE, current.getID());

		removeAuthority(server);

        return true;
	}

    [ClientRpc]
	void RpcMedicine(Vector3 location, Enums.Color color)
    {
        // Remove the current settlement
        Vertex source = BoardState.instance.vertexPosition[location];
        GamePiece settlement = source.getOccupyingPiece();
		Destroy (BoardState.instance.spawnedObjects [location]);
		BoardState.instance.spawnedObjects.Remove(location);

		GameObject spawnedCity = Instantiate<GameObject>(PrefabHolder.instance.city, location, Quaternion.identity);
        fixPieceRotationAndPosition(spawnedCity);
        spawnedCity.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));

		BoardState.instance.spawnedObjects.Add(location, spawnedCity);

		// Remove settlement at location
		settlement.takeOffBoard ();

		Player current = GameManager.instance.getCurrentPlayer();
		List<GamePiece> pieces = current.getGamePieces();
        City city = City.getFreeCity(pieces);

		if (!Object.ReferenceEquals(city, null)) {
			source.setOccupyingPiece(city);
			city.putOnBoard();
		}
    }

	public bool roadBuilding(Edge location, List<GamePiece> pieces, Enums.Color color, bool server)
    {

		if (!pa.canRoadBuilding (location, pieces, color))
        {
			return false;
		}

		assignAuthority(server);

		RpcRoadBuilding(location.transform.position, color);

		Player current = GameManager.instance.getCurrentPlayer();
		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.ROADBUILDING, current.getID());

		removeAuthority(server);
		
        return true;
	}

    [ClientRpc]
	void RpcRoadBuilding(Vector3 location, Enums.Color color)
    {
        Edge edge = BoardState.instance.edgePosition[location];

		GameObject spawnedRoad = Instantiate<GameObject>(PrefabHolder.instance.road, location, Quaternion.identity);

        fixPieceRotationAndPosition(spawnedRoad);
        if (edge.isRightPointing)
        {
            spawnedRoad.transform.Rotate(0f, 0f, -29f);
        }
        else if (edge.isLeftPointing)
        {
            spawnedRoad.transform.Rotate(0f, 0f, 31f);
        }
        else if (edge.isForwardPointing)
        {
            spawnedRoad.transform.Rotate(0f, 0f, 90f);
        }
        spawnedRoad.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, spawnedRoad);

        // Put a road on the given edge
		Player current = GameManager.instance.getCurrentPlayer();
		List<GamePiece> pieces = current.getGamePieces();
        Road road = Road.getFreeRoad(pieces);
        
		if (!Object.ReferenceEquals(road, null)) {
			edge.setOccupyingPiece(road);
			road.putOnBoard();
			road.wasBuiltThisTurn();
		}

        //Update longest route
    }

    public void WarLord()
    {
        Player current = GameManager.instance.getCurrentPlayer();
        current.activateKnights(current.getID());

        GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.WARLORD, current.getID());
    }

	public void saboteur() {

		Player current = GameManager.instance.getCurrentPlayer();
		GameManager.instance.getPersonalPlayer().removeProgressCard(ProgressCardName.SABOTEUR, current.getID());

		GameManager.instance.getPersonalPlayer().setOldTurn(GameManager.instance.getPlayerTurn());

		saboteurDiscard(0);	
	}

	public void saboteurShortcut(int start) {
		saboteurDiscard(start);
	}

	private void saboteurDiscard(int start) {

		List<Player> players = GameManager.instance.getPlayers();

        for (int i = start; i < players.Count; i++)
        {
            Player p = players[i];
            if (p.getVictoryCounts() > GameManager.instance.getPlayer(GameManager.instance.getPersonalPlayer().getOldTurn()).getVictoryCounts() ){

                foreach (Player p2 in players) {
                    GameManager.instance.getPersonalPlayer().setMoveType(MoveType.SPECIAL, p2.getID());
                }

                GameManager.instance.getPersonalPlayer().setSpecial(Special.DISCARD_RESOURCE_SABOTEUR, p.getID());
                foreach (Player p2 in players) {
                    if(!Object.ReferenceEquals(p, p2)) GameManager.instance.getPersonalPlayer().setSpecial(Special.NONE, p2.getID());
                }
                GameManager.instance.getPersonalPlayer().setSpecialTurn(p.getID());
                return;
            }
		}
		
		foreach(Player p in GameManager.instance.getPlayers()) {
			GameManager.instance.getPersonalPlayer().setMoveType(MoveType.NONE, p.getID());
			GameManager.instance.getPersonalPlayer().setSpecial(Special.NONE, p.getID());
		}
		GameManager.instance.getPersonalPlayer().revertTurn();
	}

}
