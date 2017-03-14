using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class MoveManager : NetworkBehaviour {
    
    GameManager gameManager;
    PrefabHolder prefabHolder;
    Bank bank;
    BoardState boardState;
    MoveAuthorizer ma;
	Graph graph;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
        //bank = GetComponent<Bank>();
        //boardState = GetComponent<BoardState>();
        //prefabHolder = GetComponent<PrefabHolder>();
        ma = new MoveAuthorizer();
		graph = new Graph ();
    }

    // Move a knight from source to target
    public bool moveKnight(Vertex source, Vertex target, Enums.Color color)
    {

		// Check if the knight can be moved
		if (!ma.canKnightMove (source, target, color))
        {
			return false;
		}

		Knight k = (Knight)source.getOccupyingPiece ();
		int level = k.getLevel ();

        CmdMoveKnight(source.transform.position, target.transform.position, level);

		// Check if longest route needs to be updated

		return true;
	}

    [Command]
	void CmdMoveKnight(Vector3 source, Vector3 target, int level)
    {
		GameObject knight;
		switch (level) {
		case 1:
			knight = Instantiate<GameObject> (prefabHolder.levelOneKnight, target, Quaternion.identity);
			break;
		case 2:
			knight = Instantiate<GameObject> (prefabHolder.levelTwoKnight, target, Quaternion.identity);
			break;
		default:
			knight = Instantiate<GameObject> (prefabHolder.levelThreeKnight, target, Quaternion.identity);
			break;
		}
		NetworkServer.Spawn(knight);
		NetworkServer.UnSpawn(boardState.spawnedObjects[source]);
        RpcMoveKnight(source, target, knight);
    }

    [ClientRpc]
	void RpcMoveKnight(Vector3 source, Vector3 target, GameObject knight)
    {
        Vertex sourcePiece = boardState.vertexPosition[source];
        Vertex targetPiece = boardState.vertexPosition[target];

        Knight sourceKnight = (Knight)sourcePiece.getOccupyingPiece();

        // Move the knight
        sourcePiece.setOccupyingPiece(null);
        targetPiece.setOccupyingPiece(sourceKnight);

        // Deactivate the knight
        sourceKnight.deactivateKnight();
		boardState.spawnedObjects.Add(target, knight);
		boardState.spawnedObjects.Remove (source);

    }

	// Displace a knight at target with knight at source
	public bool displaceKnight(Vertex source, Vertex target, Enums.Color color) {
		
		// Check if the knight can displace
		if (!ma.canKnightDisplace (source, target, color)) {
			return false;
		}

		Knight kSource = (Knight)source.getOccupyingPiece ();
		int sourceLevel = kSource.getLevel ();

		Knight kTarget = (Knight)target.getOccupyingPiece ();
		int targetLevel = kTarget.getLevel ();

		Vertex displacedLocation = null;
		foreach (Vertex v in boardState.vertexPosition.Values) {
			displacedLocation = v;
			break;
		}

		bool gone = true;
		foreach (Vertex v in boardState.vertexPosition.Values) {
			if (graph.areConnectedVertices (v, target, color)) {
				if (!Object.ReferenceEquals (v.getOccupyingPiece (), null)) {
					if (Object.ReferenceEquals (v, source)) {
						displacedLocation = v;
						gone = false;
						break;
					}
				} else {
					displacedLocation = v;
					gone = false;
					break;
				}
			}
		}

		CmdDisplaceKnight(source.transform.position, target.transform.position, 
			displacedLocation.transform.position, sourceLevel, targetLevel, gone);

		// Check if longest route needs to be updated

		return true;
	}

    [Command]
	void CmdDisplaceKnight(Vector3 source, Vector3 target,
		Vector3 displacedLocation, int sourceLevel, int targetLevel, bool gone)
	{
		GameObject sourceKnight;
		switch (sourceLevel) {
		case 1:
			sourceKnight = Instantiate<GameObject> (prefabHolder.levelOneKnight, target, Quaternion.identity);
			break;
		case 2:
			sourceKnight = Instantiate<GameObject> (prefabHolder.levelTwoKnight, target, Quaternion.identity);
			break;
		default:
			sourceKnight = Instantiate<GameObject> (prefabHolder.levelThreeKnight, target, Quaternion.identity);
			break;
		}

		GameObject targetKnight;
		switch (targetLevel) {
		case 1:
			targetKnight = Instantiate<GameObject> (prefabHolder.levelOneKnight, displacedLocation, Quaternion.identity);
			break;
		case 2:
			targetKnight = Instantiate<GameObject> (prefabHolder.levelTwoKnight, displacedLocation, Quaternion.identity);
			break;
		default:
			targetKnight = Instantiate<GameObject> (prefabHolder.levelThreeKnight, displacedLocation, Quaternion.identity);
			break;
		}

		if (!gone) {
			NetworkServer.Spawn (targetKnight);
		}

		NetworkServer.Spawn(sourceKnight);
		NetworkServer.UnSpawn (boardState.spawnedObjects [source]);
		NetworkServer.UnSpawn (boardState.spawnedObjects [target]);
		RpcDisplaceKnight(source, target, displacedLocation, sourceKnight, targetKnight, gone);
    }

    [ClientRpc]
	void RpcDisplaceKnight(Vector3 source, Vector3 target, Vector3 displacedLocation, 
		GameObject sourceKnightObject, GameObject targetKnightObject, bool gone)
    {
        Vertex sourcePiece = boardState.vertexPosition[source];
        Vertex targetPiece = boardState.vertexPosition[target];

        Knight sourceKnight = (Knight)sourcePiece.getOccupyingPiece();
        Knight targetKnight = (Knight)targetPiece.getOccupyingPiece();
        
        // Move the knight
        sourcePiece.setOccupyingPiece(null);

        Vertex displacedTarget = new Vertex(Enums.TerrainType.LAND); // = forcedDisplacement(target);

        targetPiece.setOccupyingPiece(sourceKnight);
        displacedTarget.setOccupyingPiece(targetKnight);

        // Deactivate the knight
        sourceKnight.deactivateKnight();

		if (!gone) {
			boardState.spawnedObjects.Add (displacedLocation, targetKnightObject);
		}

		boardState.spawnedObjects.Remove(target);
		boardState.spawnedObjects.Remove(source);
		boardState.spawnedObjects.Add(target, sourceKnightObject);
    }

	// Upgrade a knight at vertex v
	public bool upgradeKnight(int[] resources, int[] devChart, Vertex v, List<GamePiece> pieces) {

		// Check if the knight can be upgraded
		if (!ma.canUpgradeKnight (resources, devChart, v, pieces)) {
			return false;
		}

		Knight k = (Knight)v.getOccupyingPiece ();
		int level = k.getLevel ();

        CmdUpgradeKnight(resources, devChart, v.transform.position, level);
		return true;
	}

    [Command]
	void CmdUpgradeKnight(int[] resources, int[] devChart, Vector3 v, int level)
	{
		GameObject newKnight;
		switch (level) {
		case 1:
			newKnight = Instantiate<GameObject> (prefabHolder.levelTwoKnight, v, Quaternion.identity);
			break;
		default:
			newKnight = Instantiate<GameObject> (prefabHolder.levelThreeKnight, v, Quaternion.identity);
			break;
		}
		NetworkServer.Spawn(newKnight);
		RpcUpgradeKnight(resources, devChart, v, newKnight);
    }

    [ClientRpc]
	void RpcUpgradeKnight(int[] resources, int[] devChart, Vector3 v, GameObject newKnight)
    {
		Vertex source = boardState.vertexPosition[v];

		// Upgrade the knight
		Knight knight = (Knight)source.getOccupyingPiece();

		knight.upgrade ();

		boardState.spawnedObjects.Add(v, newKnight);
    }

	// Activate a knight at vertex v
	public bool activateKnight(int[] resources, Vertex v) {
        CmdActivateKnight(resources, v.transform.position);
		return true;
	}

    [Command]
    void CmdActivateKnight(int[] resources, Vector3 v)
    {
        RpcActivateKnight(resources, v);
    }

    [ClientRpc]
    void RpcActivateKnight(int[] resources, Vector3 v)
    {

    }

	// Upgrade a development chart in the specified field
	public bool upgradeDevChart(Enums.DevChartType dev, int[] commodities, 
		List<GamePiece> pieces, int[] devChart) {
        
		return true;
	}

	// Build a settlement at location
	public bool buidSettlement(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color) {

		if (!ma.canBuildSettlement (location, resources, pieces, color)) {
			return false;
		}

        CmdBuildSettlement(location.transform.position);

		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());

        // Add an appropriate amount of victory points
        current.changeVictoryPoints(1);
        current.changeVictoryPoints(location.getChits());
        // Spend the correct resources
        // Spend the correct resources
        current.changeResource(Enums.ResourceType.BRICK, -1);
        bank.depositResource(Enums.ResourceType.BRICK, 1);

        current.changeResource(Enums.ResourceType.GRAIN, -1);
        bank.depositResource(Enums.ResourceType.GRAIN, 1);

        current.changeResource(Enums.ResourceType.WOOL, -1);
        bank.depositResource(Enums.ResourceType.WOOL, 1);

        current.changeResource(Enums.ResourceType.LUMBER, -1);
        bank.depositResource(Enums.ResourceType.LUMBER, 1);
        // Check if there is a port
        updatePort (location);

		// Update longest road

		return true;
	}

    [Command]
    void CmdBuildSettlement(Vector3 location)
    {
        GameObject newSettlement = Instantiate<GameObject>(prefabHolder.settlement, location, Quaternion.identity);
        NetworkServer.Spawn(newSettlement);
		RpcBuildSettlement(location, newSettlement);
    }

    [ClientRpc]
	void RpcBuildSettlement(Vector3 location, GameObject newSettlement)
    {
        Vertex source = boardState.vertexPosition[location];

		List<GamePiece> pieces = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn()).getGamePieces();// gameManager.getCurrentPlayer().getGamePieces();

        // Put a settlement on the board
        Settlement settlement = Settlement.getFreeSettlement(pieces);

        source.setOccupyingPiece(settlement);
        settlement.putOnBoard();
		boardState.spawnedObjects.Add(location, newSettlement);
    }


	// Build a city at location
	public bool buildCity(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color) {

		if (!ma.canBuildCity (location, resources, pieces, color)) {
			return false;
		}

        CmdBuildCity(location.transform.position);

        // Add an appropriate amount of victory points
		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());
        current.changeVictoryPoints(1);

        // Spend the resources
        current.changeResource(Enums.ResourceType.GRAIN, -2);
        bank.depositResource(Enums.ResourceType.GRAIN, 2);

        current.changeResource(Enums.ResourceType.ORE, -3);
        bank.depositResource(Enums.ResourceType.ORE, 3);
        return true;
	}

    [Command]
    void CmdBuildCity(Vector3 location)
    {
        GameObject spawnedCity = Instantiate<GameObject>(prefabHolder.city, location, Quaternion.identity);
        NetworkServer.Spawn(spawnedCity);
		RpcBuildCity(location, spawnedCity);
    }

    [ClientRpc]
	void RpcBuildCity(Vector3 location, GameObject spawnedCity)
    {
        // Remove the current settlement
        Vertex source = boardState.vertexPosition[location];
        GamePiece settlement = source.getOccupyingPiece();
        
        settlement.takeOffBoard();

        // Build a city
		List<GamePiece> pieces = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn()).getGamePieces();// gameManager.getCurrentPlayer().getGamePieces();
        City city = City.getFreeCity(pieces);

        source.setOccupyingPiece(city);
        city.putOnBoard();
		boardState.spawnedObjects.Add(location, spawnedCity);
    }

	// Check if a city wall can be built at a vertex
	public bool buildCityWall(Vertex location, int[] resources,
		int cityWalls, Enums.Color color) {
        CmdBuildCityWall(location.transform.position);
		return true;
	}

    [Command]
    void CmdBuildCityWall(Vector3 location)
    {
        GameObject spawnedCityWall = Instantiate<GameObject>(prefabHolder.cityWall, location, Quaternion.identity);
        NetworkServer.Spawn(spawnedCityWall);
		RpcBuildCityWall(location, spawnedCityWall);

    }

    [ClientRpc]
	void RpcBuildCityWall(Vector3 location, GameObject spawnedCityWall)
    {
		boardState.spawnedObjects.Add(location, spawnedCityWall);
    }

	// Build a knight at location
	public bool buildKnight(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color) {
        CmdBuildKnight(location.transform.position);
		return true;
	}

    [Command]
    void CmdBuildKnight(Vector3 location)
    {
        GameObject spawnedKnight = Instantiate<GameObject>(prefabHolder.levelOneKnight, location, Quaternion.identity);
        NetworkServer.Spawn(spawnedKnight);
		RpcBuildKnight(location, spawnedKnight);
    }

    [ClientRpc]
    void RpcBuildKnight(Vector3 location, GameObject spawnedKnight)
    {
		boardState.spawnedObjects.Add(location, spawnedKnight);
    }

	// Build a road at location
	public bool buildRoad(Edge location, int[] resources,
		List<GamePiece> pieces, Enums.Color color) {

		if (!ma.canBuildRoad (location, resources, pieces, color)) {
			return false;
		}

        CmdBuildRoad(location.transform.position);

		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());

        // Spend the resources
        current.changeResource(Enums.ResourceType.BRICK, -1);
        bank.depositResource(Enums.ResourceType.BRICK, 1);

        current.changeResource(Enums.ResourceType.LUMBER, -1);
        bank.depositResource(Enums.ResourceType.LUMBER, 1);
        //Update longest route

        return true;
	}

    [Command]
    void CmdBuildRoad(Vector3 location)
    {
        GameObject spawnedRoad = Instantiate<GameObject>(prefabHolder.road, location, Quaternion.identity);
        NetworkServer.Spawn(spawnedRoad);
		RpcBuildRoad(location, spawnedRoad);
    }

    [ClientRpc]
    void RpcBuildRoad(Vector3 location, GameObject spawnedRoad)
    {
        Edge edge = boardState.edgePosition[location];

        // Put a road on the given edge
		List<GamePiece> pieces = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn()).getGamePieces();// gameManager.getCurrentPlayer().getGamePieces();
        Road road = Road.getFreeRoad(pieces);
        
        edge.setOccupyingPiece(road);
        road.putOnBoard();
        road.wasBuiltThisTurn();
		boardState.spawnedObjects.Add(location, spawnedRoad);
    }

	// Build a ship at location
	public bool buildShip(Edge location, int[] resources,
		List<GamePiece> pieces, Enums.Color color) {

		if (!ma.canBuildShip (location, resources, pieces, color)) {
			return false;
		}

        CmdBuildShip(location.transform.position);

		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());

        // Spend the resources
        current.changeResource(Enums.ResourceType.WOOL, -1);
        bank.depositResource(Enums.ResourceType.WOOL, 1);

        current.changeResource(Enums.ResourceType.LUMBER, -1);
        bank.depositResource(Enums.ResourceType.LUMBER, 1);
        //Update longest route

        return true;
	}

    [Command]
    void CmdBuildShip(Vector3 location)
    {
        GameObject spawnedBoat = Instantiate<GameObject>(prefabHolder.boat, location, Quaternion.identity);
        NetworkServer.Spawn(spawnedBoat);
		RpcBuildShip(location, spawnedBoat);
    }

    [ClientRpc]
    void RpcBuildShip(Vector3 location, GameObject spawnedBoat)
    {
        Edge edge = boardState.edgePosition[location];

        // Put a road on the given edge
		List<GamePiece> pieces = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn()).getGamePieces();// gameManager.getCurrentPlayer().getGamePieces();
        Road ship = Road.getFreeShip(pieces);

        edge.setOccupyingPiece(ship);
        ship.putOnBoard();
        ship.wasBuiltThisTurn();
		boardState.spawnedObjects.Add(location, spawnedBoat);
    }

	// Move a ship from source to target
	public bool moveShip(Edge source, Edge target, Enums.Color color) {
        CmdMoveShip(source.transform.position, target.transform.position);
		return true;
	}

    [Command]
    void CmdMoveShip(Vector3 source, Vector3 target)
    {
        RpcMoveShip(source, target);
    }

    [ClientRpc]
    void RpcMoveShip(Vector3 source, Vector3 target)
    {

    }

	// Chase robber from source
	public bool chaseRobber(Vertex source) {
        CmdChaseRobber(source.transform.position);
		return true;
	}

    [Command]
    void CmdChaseRobber(Vector3 source)
    {
        RpcChaseRobber(source);
    }

    [ClientRpc]
    void RpcChaseRobber(Vector3 source)
    {

    }

	// Move robber to target
	public bool moveRobber(Hex target) {
        CmdMoveRobber(target.transform.position);
		return true;
	}

    [Command]
    void CmdMoveRobber(Vector3 target)
    {
        RpcMoveRobber(target);
    }

    [ClientRpc]
    void RpcMoveRobber(Vector3 target)
    {

    }

	// Move Pirate to target
	public bool movePirate(Hex target) {
        CmdMovePirate(target.transform.position);
		return true;
	}

    [Command]
    void CmdMovePirate(Vector3 target)
    {
        RpcMovePirate(target);
    }

    [ClientRpc]
    void RpcMovePirate(Vector3 target)
    {

    }

	// Place Merchant at target
	public bool placeMerchant(Hex target) {

        CmdPlaceMerchant(target.transform.position);

		return true;
	}

    [Command]
    void CmdPlaceMerchant(Vector3 target)
    {
        RpcPlaceMerchant(target);
    }

    [ClientRpc]
    void RpcPlaceMerchant(Vector3 target)
    {

    }

	// Place an initial settlement
	public bool placeInitialSettlement (Vertex v, List<GamePiece> pieces, List<Hex> validHexes) {
		if (!ma.canPlaceInitialTownPiece (v, validHexes)) {
			return false;
		}

        CmdBuildSettlement(v.transform.position);

        // Get the resources around the settlement
		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());

        // Update the victory points and add a port
        current.changeVictoryPoints(1);
        updatePort(v);

        return true;
    }

    // Place an initial city
    public bool placeInitialCity (Vertex v, List<GamePiece> pieces, List<Hex> validHexes) {
		if (!ma.canPlaceInitialTownPiece (v, validHexes)) {
			return false;
		}

        CmdBuildCity(v.transform.position);

        // Get the resources around the city
		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());
        foreach (Hex h in validHexes)
        {
            if (h.adjacentToVertex(v))
            {
                Enums.ResourceType res = GameManager.instance.getResourceFromHex(h.getHexType());
                if (res != Enums.ResourceType.NONE)
                {
                    current.changeResource(res, 1);
                    bank.withdrawResource(res, 1);
                }
            }
        }

        // Update the victory points and add a port 
        current.changeVictoryPoints(2);
        updatePort(v);

        return true;
    }

	// Place an initial road
	public bool placeInitialRoad (Edge e, Enums.Color color, List<GamePiece> pieces) {
		if (!ma.canPlaceInitialRoad (e, color)) {
			return false;
		}

        CmdBuildRoad(e.transform.position);

		return true;
	}
	 
	// Place an initial ship
	public bool placeInitialShip (Edge e, Enums.Color color, List<GamePiece> pieces) {
		if (!ma.canPlaceInitialShip (e, color)) {
			return false;
		}

        CmdBuildShip(e.transform.position);

		return true;
	}

	// Get a resource type from a port type
	private Enums.ResourceType getResourceFromPort(Enums.PortType port) {

		switch (port) {
		case Enums.PortType.BRICK:
			return Enums.ResourceType.BRICK;
		case Enums.PortType.WOOL:
			return Enums.ResourceType.WOOL; 
		case Enums.PortType.ORE:
			return Enums.ResourceType.ORE;
		case Enums.PortType.LUMBER:
			return Enums.ResourceType.LUMBER;
		case Enums.PortType.GRAIN:
			return Enums.ResourceType.GRAIN;
		default:
			return Enums.ResourceType.NONE;
		}
	}

	// Update the current players resource ratios according to the given vertex
	private void updatePort(Vertex v) {
		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());
		int[] ratios = current.getResourceRatios ();
		Enums.PortType port = v.getPortType ();

		// If the port is generic, update the ratios accordingly
		if (port == Enums.PortType.GENERIC) {
			for (int i = 0; i < ratios.Length; i++) {
				if (ratios [i] > 3) {
					ratios [i] = 3;
				}
			}
			current.updateResourceRatios (ratios);

		// If there is a specific port, update the correct ratio
		} else if (port != Enums.PortType.NONE) {
			current.updateResourceRatio (getResourceFromPort (port), 2);
		}
	}
}
