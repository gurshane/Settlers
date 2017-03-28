using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class MoveManager : NetworkBehaviour {
    
    MoveAuthorizer ma;
	Graph graph;
	private NetworkIdentity objNetId;
	static public MoveManager instance = null;

	void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ma = new MoveAuthorizer();
		graph = new Graph ();
    }

    // Move a knight from source to target
	public bool moveKnight(Vertex source, Vertex target, Enums.Color color, bool server)
    {

		// Check if the knight can be moved
		if (!ma.canKnightMove (source, target, color))
        {
			return false;
		}

		Knight k = (Knight)source.getOccupyingPiece ();
		int level = k.getLevel ();

		assignAuthority(server);
        RpcMoveKnight(source.transform.position, target.transform.position, level, color);
		removeAuthority(server);

		return true;
	}

    [ClientRpc]
	void RpcMoveKnight(Vector3 source, Vector3 target, int level, Enums.Color color)
    {
        Vertex sourcePiece = BoardState.instance.vertexPosition[source];
        Vertex targetPiece = BoardState.instance.vertexPosition[target];

		GameObject knight = getKnightFromLevel (level, target);
		knight.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		Destroy (BoardState.instance.spawnedObjects [source]);

		BoardState.instance.spawnedObjects.Add(target, knight);
		BoardState.instance.spawnedObjects.Remove (source);

        Knight sourceKnight = (Knight)sourcePiece.getOccupyingPiece();

        // Move the knight
        sourcePiece.setOccupyingPiece(null);
        targetPiece.setOccupyingPiece(sourceKnight);

        // Deactivate the knight
        sourceKnight.deactivateKnight();

		// Check if longest road needs to be updated
    }

	// Displace a knight at target with knight at source
	public bool displaceKnight(Vertex source, Vertex target, Enums.Color color, bool server)
    {
		
		// Check if the knight can displace
		if (!ma.canKnightDisplace (source, target, color))
        {
			return false;
		}

		Knight kSource = (Knight)source.getOccupyingPiece ();
		int sourceLevel = kSource.getLevel ();

		Knight kTarget = (Knight)target.getOccupyingPiece ();
		int targetLevel = kTarget.getLevel ();

		Vertex displacedLocation = null;
		foreach (Vertex v in BoardState.instance.vertexPosition.Values)
        {
			displacedLocation = v;
			break;
		}

		bool gone = true;
		foreach (Vertex v in BoardState.instance.vertexPosition.Values)
        {
			if (graph.areConnectedVertices (v, target, color))
            {
				if (!Object.ReferenceEquals (v.getOccupyingPiece (), null))
                {
					if (Object.ReferenceEquals (v, source))
                    {
						displacedLocation = v;
						gone = false;
						break;
					}
				}
                else
                {
					displacedLocation = v;
					gone = false;
					break;
				}
			}
		}

		assignAuthority(server);
		RpcDisplaceKnight(source.transform.position, target.transform.position, 
			displacedLocation.transform.position, sourceLevel, targetLevel, gone, color);

		removeAuthority(server);

		return true;
	}

    [ClientRpc]
	void RpcDisplaceKnight(Vector3 source, Vector3 target, Vector3 displacedLocation,
		int sourceLevel, int targetLevel, bool gone, Enums.Color color)
    {
        Vertex sourcePiece = BoardState.instance.vertexPosition[source];
        Vertex targetPiece = BoardState.instance.vertexPosition[target];
		Vertex displacedPiece = BoardState.instance.vertexPosition[displacedLocation];

        Knight sourceKnight = (Knight)sourcePiece.getOccupyingPiece();
        Knight targetKnight = (Knight)targetPiece.getOccupyingPiece();

		GameObject sourceKnightObject = getKnightFromLevel (sourceLevel, target);
		sourceKnightObject.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));

		if (!gone) {
			GameObject targetKnightObject = getKnightFromLevel (targetLevel, displacedLocation);
			targetKnightObject.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
			BoardState.instance.spawnedObjects.Add (displacedLocation, targetKnightObject);
		}

		Destroy (BoardState.instance.spawnedObjects [source]);
		Destroy (BoardState.instance.spawnedObjects [target]);

		BoardState.instance.spawnedObjects.Remove(target);
		BoardState.instance.spawnedObjects.Remove(source);
		BoardState.instance.spawnedObjects.Add(target, sourceKnightObject);
        
        // Move the knight
        sourcePiece.setOccupyingPiece(null);

        targetPiece.setOccupyingPiece(sourceKnight);
		if (!gone) {
        	displacedPiece.setOccupyingPiece(targetKnight);
		}

        // Deactivate the knight
        sourceKnight.deactivateKnight();

		// Check if longest road needs to be updated
    }

	// Upgrade a knight at vertex v
	public bool upgradeKnight(int[] resources, int[] devChart, Vertex v, List<GamePiece> pieces, bool server)
    {

		// Check if the knight can be upgraded
		if (!ma.canUpgradeKnight (resources, devChart, v, pieces))
        {
			return false;
		}

		Knight k = (Knight)v.getOccupyingPiece ();
		int level = k.getLevel ();

		Enums.Color color = GameManager.instance.getCurrentPlayer().getColor();

		assignAuthority(server);
        RpcUpgradeKnight(resources, devChart, v.transform.position, level, color);
		removeAuthority(server);
		return true;
	}

    [ClientRpc]
	void RpcUpgradeKnight(int[] resources, int[] devChart, Vector3 v, int level, Enums.Color color)
    {
		Vertex source = BoardState.instance.vertexPosition[v];

		GameObject newKnight = getKnightFromLevel (level + 1, v);
		newKnight.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(v, newKnight);

		// Upgrade the knight
		Knight knight = (Knight)source.getOccupyingPiece();

		knight.upgrade ();
    }

	// Activate a knight at vertex v
	public bool activateKnight(int[] resources, Vertex v, bool server)
    {
		assignAuthority(server);
        RpcActivateKnight(resources, v.transform.position);
		removeAuthority(server);
		return true;
	}

    [ClientRpc]
    void RpcActivateKnight(int[] resources, Vector3 v)
    {

    }

	// Upgrade a development chart in the specified field
	public bool upgradeDevChart(Enums.DevChartType dev, int[] commodities, 
		List<GamePiece> pieces, int[] devChart, bool server)
    {
		return true;
	}

	// Build a settlement at location
	public bool buidSettlement(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color, bool server)
    {

		if (!ma.canBuildSettlement (location, resources, pieces, color))
        {
			return false;
		}

		assignAuthority(server);
        RpcBuildSettlement(location.transform.position, color);
		removeAuthority(server);

		Player current = GameManager.instance.getCurrentPlayer();

		// Add an appropriate amount of victory points
        current.changeVictoryPoints(1);
        current.changeVictoryPoints(location.getChits());

        // Spend the correct resources
        current.changeResource(Enums.ResourceType.BRICK, -1);
        Bank.instance.depositResource(Enums.ResourceType.BRICK, 1, current.isServer);

        current.changeResource(Enums.ResourceType.GRAIN, -1);
        Bank.instance.depositResource(Enums.ResourceType.GRAIN, 1, current.isServer);

        current.changeResource(Enums.ResourceType.WOOL, -1);
        Bank.instance.depositResource(Enums.ResourceType.WOOL, 1, current.isServer);

        current.changeResource(Enums.ResourceType.LUMBER, -1);
        Bank.instance.depositResource(Enums.ResourceType.LUMBER, 1, current.isServer);

		return true;
	}

    [ClientRpc]
	void RpcBuildSettlement(Vector3 location, Enums.Color color)
    {
       	Vertex source = BoardState.instance.vertexPosition[location];
		GameObject newSettlement = Instantiate<GameObject>(PrefabHolder.instance.settlement, location, Quaternion.identity);
		newSettlement.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, newSettlement);

		Player current = GameManager.instance.getCurrentPlayer();
		List<GamePiece> pieces = current.getGamePieces();

        // Put a settlement on the board
        Settlement settlement = Settlement.getFreeSettlement(pieces);

        source.setOccupyingPiece(settlement);
        settlement.putOnBoard();

        // Check if there is a port
        updatePort (source);

		// Update longest road
    }


	// Build a city at location
	public bool buildCity(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color, bool server)
    {

		if (!ma.canBuildCity (location, resources, pieces, color))
        {
			return false;
		}

		assignAuthority(server);
        RpcBuildCity(location.transform.position, color);
		removeAuthority(server);

		Player current = GameManager.instance.getCurrentPlayer();

		// Add an appropriate amount of victory points
        current.changeVictoryPoints(1);

        // Spend the resources
        current.changeResource(Enums.ResourceType.GRAIN, -2);
        Bank.instance.depositResource(Enums.ResourceType.GRAIN, 2, current.isServer);

        current.changeResource(Enums.ResourceType.ORE, -3);
        Bank.instance.depositResource(Enums.ResourceType.ORE, 3, current.isServer);

        return true;
	}

    [ClientRpc]
	void RpcBuildCity(Vector3 location, Enums.Color color)
    {
        // Remove the current settlement
        Vertex source = BoardState.instance.vertexPosition[location];
        GamePiece settlement = source.getOccupyingPiece();
		GameObject spawnedCity = Instantiate<GameObject>(PrefabHolder.instance.city, location, Quaternion.identity);
		spawnedCity.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));

		Destroy (BoardState.instance.vertexPosition [location]);
		BoardState.instance.spawnedObjects.Remove(location);

		BoardState.instance.spawnedObjects.Add(location, spawnedCity);

		// Remove settlement at location
		settlement.takeOffBoard ();

		Player current = GameManager.instance.getCurrentPlayer();
		List<GamePiece> pieces = current.getGamePieces();
        City city = City.getFreeCity(pieces);

        source.setOccupyingPiece(city);
        city.putOnBoard();
    }

	// Check if a city wall can be built at a vertex
	public bool buildCityWall(Vertex location, int[] resources,
		int cityWalls, Enums.Color color, bool server)
    {

		assignAuthority(server);
        RpcBuildCityWall(location.transform.position, color);
		removeAuthority(server);
		return true;
	}

    [ClientRpc]
	void RpcBuildCityWall(Vector3 location, Enums.Color color)
    {
		GameObject spawnedCityWall = Instantiate<GameObject>(PrefabHolder.instance.cityWall, location, Quaternion.identity);
		spawnedCityWall.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, spawnedCityWall);
    }

	// Build a knight at location
	public bool buildKnight(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color, bool server)
    {
		assignAuthority(server);
        RpcBuildKnight(location.transform.position, color);
		removeAuthority(server);
		return true;
	}

    [ClientRpc]
	void RpcBuildKnight(Vector3 location, Enums.Color color)
    {
		GameObject spawnedKnight = Instantiate<GameObject>(PrefabHolder.instance.levelOneKnight, location, Quaternion.identity);
		spawnedKnight.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, spawnedKnight);
    }

	// Build a road at location
	public bool buildRoad(Edge location, int[] resources,
		List<GamePiece> pieces, Enums.Color color, bool server)
    {

		if (!ma.canBuildRoad (location, resources, pieces, color))
        {
			return false;
		}

		assignAuthority(server);
        RpcBuildRoad(location.transform.position, color);
		removeAuthority(server);

		Player current = GameManager.instance.getCurrentPlayer();

		// Spend the resources
        current.changeResource(Enums.ResourceType.BRICK, -1);
        Bank.instance.depositResource(Enums.ResourceType.BRICK, 1, current.isServer);

        current.changeResource(Enums.ResourceType.LUMBER, -1);
        Bank.instance.depositResource(Enums.ResourceType.LUMBER, 1, current.isServer);

        return true;
	}

    [ClientRpc]
	void RpcBuildRoad(Vector3 location, Enums.Color color)
    {
        Edge edge = BoardState.instance.edgePosition[location];
		GameObject spawnedRoad = Instantiate<GameObject>(PrefabHolder.instance.road, location, Quaternion.identity);
		spawnedRoad.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, spawnedRoad);

        // Put a road on the given edge
		Player current = GameManager.instance.getCurrentPlayer();
		List<GamePiece> pieces = current.getGamePieces();
        Road road = Road.getFreeRoad(pieces);
        
        edge.setOccupyingPiece(road);
        road.putOnBoard();
        road.wasBuiltThisTurn();

        //Update longest route
    }

	// Build a ship at location
	public bool buildShip(Edge location, int[] resources,
		List<GamePiece> pieces, Enums.Color color, bool server)
    {

		if (!ma.canBuildShip (location, resources, pieces, color))
        {
			return false;
		}

		assignAuthority(server);
        RpcBuildShip(location.transform.position, color);
		removeAuthority(server);

		Player current = GameManager.instance.getCurrentPlayer();

		// Spend the resources
        current.changeResource(Enums.ResourceType.WOOL, -1);
        Bank.instance.depositResource(Enums.ResourceType.WOOL, 1, current.isServer);

        current.changeResource(Enums.ResourceType.LUMBER, -1);
        Bank.instance.depositResource(Enums.ResourceType.LUMBER, 1, current.isServer);

        return true;
	}
		

    [ClientRpc]
	void RpcBuildShip(Vector3 location, Enums.Color color)
    {
        Edge edge = BoardState.instance.edgePosition[location];
		GameObject spawnedBoat = Instantiate<GameObject>(PrefabHolder.instance.boat, location, Quaternion.identity);
		spawnedBoat.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, spawnedBoat);

        // Put a road on the given edge
		Player current = GameManager.instance.getCurrentPlayer();
		List<GamePiece> pieces = current.getGamePieces();
        Road ship = Road.getFreeShip(pieces);

        edge.setOccupyingPiece(ship);
        ship.putOnBoard();
        ship.wasBuiltThisTurn();

        //Update longest route
    }

	// Move a ship from source to target
	public bool moveShip(Edge source, Edge target, bool server)
    {
		assignAuthority(server);
        RpcMoveShip(source.transform.position, target.transform.position);
		removeAuthority(server);
		return true;
	}

    [ClientRpc]
    void RpcMoveShip(Vector3 source, Vector3 target)
    {

    }

	// Chase robber from source
	public bool chaseRobber(Vertex source, bool server)
    {
		assignAuthority(server);
        RpcChaseRobber(source.transform.position);
		removeAuthority(server);
		return true;
	}

    [ClientRpc]
    void RpcChaseRobber(Vector3 source)
    {

    }

	// Move robber to target
	public bool moveRobber(Hex target, bool server)
    {
		assignAuthority(server);
        RpcMoveRobber(target.transform.position);
		removeAuthority(server);
		return true;
	}

    [ClientRpc]
    void RpcMoveRobber(Vector3 target)
    {

    }

	// Move Pirate to target
	public bool movePirate(Hex target, bool server)
    {
		assignAuthority(server);
        RpcMovePirate(target.transform.position);
		removeAuthority(server);
		return true;
	}

    [ClientRpc]
    void RpcMovePirate(Vector3 target)
    {

    }

	// Place Merchant at target
	public bool placeMerchant(Hex target, bool server)
    {
		assignAuthority(server);
        RpcPlaceMerchant(target.transform.position);
		removeAuthority(server);
		return true;
	}

    [ClientRpc]
    void RpcPlaceMerchant(Vector3 target)
    {

    }

	// Place an initial settlement
	public bool placeInitialSettlement (Vertex v, List<GamePiece> pieces, bool server)
    {
		if (!ma.canPlaceInitialTownPiece (v))
        {
			return false;
		}

		Player current = GameManager.instance.getCurrentPlayer();

		assignAuthority(server);
		RpcPlaceInitialSettlement(v.transform.position, current.getColor());
		removeAuthority(server);

		current.changeVictoryPoints(1);
        return true;
    }

	[ClientRpc]
	void RpcPlaceInitialSettlement(Vector3 location, Enums.Color color)
    {
        Vertex source = BoardState.instance.vertexPosition[location];
		GameObject newSettlement = Instantiate<GameObject>(PrefabHolder.instance.settlement, location, Quaternion.identity);
		newSettlement.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, newSettlement);

		Player current = GameManager.instance.getCurrentPlayer();
		List<GamePiece> pieces = current.getGamePieces();

        // Put a settlement on the board
        Settlement settlement = Settlement.getFreeSettlement(pieces);

        source.setOccupyingPiece(settlement);
        settlement.putOnBoard();

		updatePort(source);
    }

    // Place an initial city
	public bool placeInitialCity (Vertex v, List<GamePiece> pieces, bool server)
    {
		if (!ma.canPlaceInitialTownPiece (v))
        {
			return false;
		}

		assignAuthority(server);
		RpcPlaceInitialCity(v.transform.position, GameManager.instance.getCurrentPlayer().getColor());
		removeAuthority(server);

		Player current = GameManager.instance.getCurrentPlayer();

		// Give starting resources to current player
		foreach (Hex h in BoardState.instance.hexPoisition.Values)
        {
            if (h.adjacentToVertex(v))
            {
                Enums.ResourceType res = GameManager.instance.getResourceFromHex(h.getHexType());
                if (res != Enums.ResourceType.NONE)
                {
                    current.changeResource(res, 1);
                    Bank.instance.withdrawResource(res, 1, current.isServer);
                }
            }
        }

		// Update the victory points and add a port 
        current.changeVictoryPoints(2);
        
        return true;
    }

	[ClientRpc]
	void RpcPlaceInitialCity(Vector3 location, Enums.Color color)
    {
        Vertex source = BoardState.instance.vertexPosition[location];
		GameObject spawnedCity = Instantiate<GameObject>(PrefabHolder.instance.city, location, Quaternion.identity);
		spawnedCity.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, spawnedCity);

        // Build a city
		Player current = GameManager.instance.getCurrentPlayer();
		List<GamePiece> pieces = current.getGamePieces();
        City city = City.getFreeCity(pieces);

        source.setOccupyingPiece(city);
        city.putOnBoard();

        updatePort(source);
    }

	// Place an initial road
	public bool placeInitialRoad (Edge e, Enums.Color color, List<GamePiece> pieces, bool server)
    {
		if (!ma.canPlaceInitialRoad (e, color))
        {
			return false;
		}

		assignAuthority(server);
        RpcPlaceInitialRoad(e.transform.position, color);
		removeAuthority(server);
		return true;
	}

	[ClientRpc]
	void RpcPlaceInitialRoad(Vector3 location, Enums.Color color)
    {
        Edge edge = BoardState.instance.edgePosition[location];
		GameObject spawnedRoad = Instantiate<GameObject>(PrefabHolder.instance.road, location, Quaternion.identity);
		spawnedRoad.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, spawnedRoad);

        // Put a road on the given edge
		Player current = GameManager.instance.getCurrentPlayer();
		List<GamePiece> pieces = current.getGamePieces();
        Road road = Road.getFreeRoad(pieces);
        
        edge.setOccupyingPiece(road);
        road.putOnBoard();
        road.wasBuiltThisTurn();
    }
	 
	// Place an initial ship
	public bool placeInitialShip (Edge e, Enums.Color color, List<GamePiece> pieces, bool server)
    {
		if (!ma.canPlaceInitialShip (e, color))
        {
			return false;
		}

		assignAuthority(server);
        RpcPlaceInitialShip(e.transform.position, color);
		removeAuthority(server);
		return true;
	}

	[ClientRpc]
	void RpcPlaceInitialShip(Vector3 location, Enums.Color color)
    {
        Edge edge = BoardState.instance.edgePosition[location];
		GameObject spawnedBoat = Instantiate<GameObject>(PrefabHolder.instance.boat, location, Quaternion.identity);
		spawnedBoat.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, spawnedBoat);

        // Put a road on the given edge
		Player current = GameManager.instance.getCurrentPlayer();
		List<GamePiece> pieces = current.getGamePieces();// gameManager.getCurrentPlayer().getGamePieces();
        Road ship = Road.getFreeShip(pieces);

        edge.setOccupyingPiece(ship);
        ship.putOnBoard();
        ship.wasBuiltThisTurn();
    }
	

	// Instantiate a knight of the given level at the given location 
	private GameObject getKnightFromLevel(int level, Vector3 location) {
		GameObject knight;
		switch (level) {
		case 1:
			knight = Instantiate<GameObject> (PrefabHolder.instance.levelOneKnight, location, Quaternion.identity);
			break;
		case 2:
			knight = Instantiate<GameObject> (PrefabHolder.instance.levelTwoKnight, location, Quaternion.identity);
			break;
		case 3:
			knight = Instantiate<GameObject> (PrefabHolder.instance.levelThreeKnight, location, Quaternion.identity);
			break;
		default:
			knight = null;
			break;
		}
		return knight;
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

	// Get a resource type from a port type
	private Enums.ResourceType getResourceFromPort(Enums.PortType port)
    {

		switch (port)
        {
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
	private void updatePort(Vertex v)
    {
		Player current = GameManager.instance.getCurrentPlayer();
		int[] ratios = current.getResourceRatios ();
		Enums.PortType port = v.getPortType ();

		// If the port is generic, update the ratios accordingly
		if (port == Enums.PortType.GENERIC)
        {
			for (int i = 0; i < ratios.Length; i++)
            {
				if (ratios [i] > 3)
                {
					ratios [i] = 3;
				}
			}
			current.updateResourceRatios (ratios);

		// If there is a specific port, update the correct ratio
		}
        else if (port != Enums.PortType.NONE)
        {
			current.updateResourceRatio (getResourceFromPort (port), 2);
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
}
