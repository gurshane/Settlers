using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

public class MoveManager : NetworkBehaviour {
    
    GameManager gameManager;
    PrefabHolder prefabHolder;
    Bank bank;
    MoveAuthorizer ma;
	Graph graph;

	private NetworkIdentity objNetId;

    void Start()
    {
        ma = new MoveAuthorizer();
		graph = new Graph ();
    }

	public void Init() {
		bank = GameObject.FindWithTag ("Bank").GetComponent<Bank> ();
		gameManager = GameObject.FindWithTag ("GameManager").GetComponent<GameManager> ();
		prefabHolder = GameObject.FindWithTag ("PrefabHolder").GetComponent<PrefabHolder> ();
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

		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);

        RpcMoveKnight(source.transform.position, target.transform.position, level, color);

		objNetId.RemoveClientAuthority (connectionToClient);

		// Check if longest route needs to be updated

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

        Knight sourceKnight = (Knight)sourcePiece.getOccupyingPiece();

        // Move the knight
        sourcePiece.setOccupyingPiece(null);
        targetPiece.setOccupyingPiece(sourceKnight);

        // Deactivate the knight
        sourceKnight.deactivateKnight();
		BoardState.instance.spawnedObjects.Add(target, knight);
		BoardState.instance.spawnedObjects.Remove (source);

    }

	// Displace a knight at target with knight at source
	public bool displaceKnight(Vertex source, Vertex target, Enums.Color color)
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

		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);

		RpcDisplaceKnight(source.transform.position, target.transform.position, 
			displacedLocation.transform.position, sourceLevel, targetLevel, gone, color);

		objNetId.RemoveClientAuthority (connectionToClient);

		// Check if longest route needs to be updated

		return true;
	}

    [ClientRpc]
	void RpcDisplaceKnight(Vector3 source, Vector3 target, Vector3 displacedLocation,
		int sourceLevel, int targetLevel, bool gone, Enums.Color color)
    {
        Vertex sourcePiece = BoardState.instance.vertexPosition[source];
        Vertex targetPiece = BoardState.instance.vertexPosition[target];

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
        
        // Move the knight
        sourcePiece.setOccupyingPiece(null);

        Vertex displacedTarget = new Vertex(Enums.TerrainType.LAND); // = forcedDisplacement(target);

        targetPiece.setOccupyingPiece(sourceKnight);
        displacedTarget.setOccupyingPiece(targetKnight);

        // Deactivate the knight
        sourceKnight.deactivateKnight();

		BoardState.instance.spawnedObjects.Remove(target);
		BoardState.instance.spawnedObjects.Remove(source);
		BoardState.instance.spawnedObjects.Add(target, sourceKnightObject);
    }

	// Upgrade a knight at vertex v
	public bool upgradeKnight(int[] resources, int[] devChart, Vertex v, List<GamePiece> pieces)
    {

		// Check if the knight can be upgraded
		if (!ma.canUpgradeKnight (resources, devChart, v, pieces))
        {
			return false;
		}

		Knight k = (Knight)v.getOccupyingPiece ();
		int level = k.getLevel ();

		Enums.Color color = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn()).getColor();

		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);
        RpcUpgradeKnight(resources, devChart, v.transform.position, level, color);

		objNetId.RemoveClientAuthority (connectionToClient);
		return true;
	}

    [ClientRpc]
	void RpcUpgradeKnight(int[] resources, int[] devChart, Vector3 v, int level, Enums.Color color)
    {
		Vertex source = BoardState.instance.vertexPosition[v];

		GameObject newKnight = getKnightFromLevel (level + 1, v);
		newKnight.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));

		// Upgrade the knight
		Knight knight = (Knight)source.getOccupyingPiece();

		knight.upgrade ();

		BoardState.instance.spawnedObjects.Add(v, newKnight);
    }

	// Activate a knight at vertex v
	public bool activateKnight(int[] resources, Vertex v)
    {
		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);

        RpcActivateKnight(resources, v.transform.position);
		objNetId.RemoveClientAuthority (connectionToClient);
		return true;
	}

    [ClientRpc]
    void RpcActivateKnight(int[] resources, Vector3 v)
    {

    }

	// Upgrade a development chart in the specified field
	public bool upgradeDevChart(Enums.DevChartType dev, int[] commodities, 
		List<GamePiece> pieces, int[] devChart)
    {
        
		return true;
	}

	// Build a settlement at location
	public bool buidSettlement(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color)
    {

		if (!ma.canBuildSettlement (location, resources, pieces, color))
        {
			return false;
		}

		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);

        RpcBuildSettlement(location.transform.position, color);
		objNetId.RemoveClientAuthority (connectionToClient);

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

    [ClientRpc]
	void RpcBuildSettlement(Vector3 location, Enums.Color color)
    {
        Vertex source = BoardState.instance.vertexPosition[location];
		GameObject newSettlement = Instantiate<GameObject>(prefabHolder.settlement, location, Quaternion.identity);
		newSettlement.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));

		List<GamePiece> pieces = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn()).getGamePieces();// gameManager.getCurrentPlayer().getGamePieces();

        // Put a settlement on the board
        Settlement settlement = Settlement.getFreeSettlement(pieces);

        source.setOccupyingPiece(settlement);
        settlement.putOnBoard();
		BoardState.instance.spawnedObjects.Add(location, newSettlement);

		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());
		current.changeVictoryPoints(1);
		updatePort(source);

    }


	// Build a city at location
	public bool buildCity(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color)
    {

		if (!ma.canBuildCity (location, resources, pieces, color))
        {
			return false;
		}

		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);

        RpcBuildCity(location.transform.position, color, false);

		objNetId.RemoveClientAuthority (connectionToClient);

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

    [ClientRpc]
	void RpcBuildCity(Vector3 location, Enums.Color color, bool initial)
    {
        // Remove the current settlement
        Vertex source = BoardState.instance.vertexPosition[location];
        GamePiece settlement = source.getOccupyingPiece();
		GameObject spawnedCity = Instantiate<GameObject>(prefabHolder.city, location, Quaternion.identity);
		spawnedCity.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));

		if (!initial) {
			settlement.takeOffBoard ();
			Destroy (BoardState.instance.vertexPosition [location]);
		}

        // Build a city
		List<GamePiece> pieces = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn()).getGamePieces();// gameManager.getCurrentPlayer().getGamePieces();
        City city = City.getFreeCity(pieces);

        source.setOccupyingPiece(city);
        city.putOnBoard();
		BoardState.instance.spawnedObjects.Add(location, spawnedCity);
    }

	// Check if a city wall can be built at a vertex
	public bool buildCityWall(Vertex location, int[] resources,
		int cityWalls, Enums.Color color)
    {

		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);
        RpcBuildCityWall(location.transform.position, color);
		objNetId.RemoveClientAuthority (connectionToClient);
		return true;
	}

    [ClientRpc]
	void RpcBuildCityWall(Vector3 location, Enums.Color color)
    {
		GameObject spawnedCityWall = Instantiate<GameObject>(prefabHolder.cityWall, location, Quaternion.identity);
		spawnedCityWall.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, spawnedCityWall);
    }

	// Build a knight at location
	public bool buildKnight(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color)
    {
		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);
        RpcBuildKnight(location.transform.position, color);
		objNetId.RemoveClientAuthority (connectionToClient);
		return true;
	}

    [ClientRpc]
	void RpcBuildKnight(Vector3 location, Enums.Color color)
    {
		GameObject spawnedKnight = Instantiate<GameObject>(prefabHolder.levelOneKnight, location, Quaternion.identity);
		spawnedKnight.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));
		BoardState.instance.spawnedObjects.Add(location, spawnedKnight);
    }

	// Build a road at location
	public bool buildRoad(Edge location, int[] resources,
		List<GamePiece> pieces, Enums.Color color)
    {

		if (!ma.canBuildRoad (location, resources, pieces, color))
        {
			return false;
		}

		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);
        RpcBuildRoad(location.transform.position, color);
		objNetId.RemoveClientAuthority (connectionToClient);

		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());

        // Spend the resources
        current.changeResource(Enums.ResourceType.BRICK, -1);
        bank.depositResource(Enums.ResourceType.BRICK, 1);

        current.changeResource(Enums.ResourceType.LUMBER, -1);
        bank.depositResource(Enums.ResourceType.LUMBER, 1);
        //Update longest route

        return true;
	}

    [ClientRpc]
	void RpcBuildRoad(Vector3 location, Enums.Color color)
    {
        Edge edge = BoardState.instance.edgePosition[location];
		GameObject spawnedRoad = Instantiate<GameObject>(prefabHolder.road, location, Quaternion.identity);
		spawnedRoad.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));

        // Put a road on the given edge
		List<GamePiece> pieces = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn()).getGamePieces();// gameManager.getCurrentPlayer().getGamePieces();
        Road road = Road.getFreeRoad(pieces);
        
        edge.setOccupyingPiece(road);
        road.putOnBoard();
        road.wasBuiltThisTurn();
		BoardState.instance.spawnedObjects.Add(location, spawnedRoad);

		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());
    }

	// Build a ship at location
	public bool buildShip(Edge location, int[] resources,
		List<GamePiece> pieces, Enums.Color color)
    {

		if (!ma.canBuildShip (location, resources, pieces, color))
        {
			return false;
		}

		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);
        RpcBuildShip(location.transform.position, color);
		objNetId.RemoveClientAuthority (connectionToClient);

		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());

        // Spend the resources
        current.changeResource(Enums.ResourceType.WOOL, -1);
        bank.depositResource(Enums.ResourceType.WOOL, 1);

        current.changeResource(Enums.ResourceType.LUMBER, -1);
        bank.depositResource(Enums.ResourceType.LUMBER, 1);
        //Update longest route

        return true;
	}
		

    [ClientRpc]
	void RpcBuildShip(Vector3 location, Enums.Color color)
    {
        Edge edge = BoardState.instance.edgePosition[location];
		GameObject spawnedBoat = Instantiate<GameObject>(prefabHolder.boat, location, Quaternion.identity);
		spawnedBoat.GetComponent<MeshRenderer>().material.SetColor("_Color", translateColor(color));

        // Put a road on the given edge
		List<GamePiece> pieces = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn()).getGamePieces();// gameManager.getCurrentPlayer().getGamePieces();
        Road ship = Road.getFreeShip(pieces);

        edge.setOccupyingPiece(ship);
        ship.putOnBoard();
        ship.wasBuiltThisTurn();
		BoardState.instance.spawnedObjects.Add(location, spawnedBoat);
    }

	// Move a ship from source to target
	public bool moveShip(Edge source, Edge target)
    {
		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);
        RpcMoveShip(source.transform.position, target.transform.position);
		objNetId.RemoveClientAuthority (connectionToClient);
		return true;
	}

    [ClientRpc]
    void RpcMoveShip(Vector3 source, Vector3 target)
    {

    }

	// Chase robber from source
	public bool chaseRobber(Vertex source)
    {
		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);
        RpcChaseRobber(source.transform.position);
		objNetId.RemoveClientAuthority (connectionToClient);
		return true;
	}

    [ClientRpc]
    void RpcChaseRobber(Vector3 source)
    {

    }

	// Move robber to target
	public bool moveRobber(Hex target)
    {
		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);
        RpcMoveRobber(target.transform.position);
		objNetId.RemoveClientAuthority (connectionToClient);
		return true;
	}

    [ClientRpc]
    void RpcMoveRobber(Vector3 target)
    {

    }

	// Move Pirate to target
	public bool movePirate(Hex target)
    {
		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);
        RpcMovePirate(target.transform.position);
		objNetId.RemoveClientAuthority (connectionToClient);
		return true;
	}

    [ClientRpc]
    void RpcMovePirate(Vector3 target)
    {

    }

	// Place Merchant at target
	public bool placeMerchant(Hex target)
    {
		objNetId = this.GetComponent<NetworkIdentity> ();        
		objNetId.AssignClientAuthority (connectionToClient);
        RpcPlaceMerchant(target.transform.position);
		objNetId.RemoveClientAuthority (connectionToClient);

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

		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());

		if (!server) {
			objNetId = this.GetComponent<NetworkIdentity> ();        
			objNetId.AssignClientAuthority (connectionToClient);  
		}

		RpcBuildSettlement(v.transform.position, current.getColor());

		if (!server) {
			objNetId.RemoveClientAuthority (connectionToClient);
		}
			        

        return true;
    }

    // Place an initial city
	public bool placeInitialCity (Vertex v, List<GamePiece> pieces, bool server)
    {
		if (!ma.canPlaceInitialTownPiece (v))
        {
			return false;
		}

		// Get the resources around the city
		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());

		if (!server) {
			objNetId = this.GetComponent<NetworkIdentity> ();        
			objNetId.AssignClientAuthority (connectionToClient);  
		}
		RpcBuildCity(v.transform.position, current.getColor(), true);
		if (!server) {
			objNetId.RemoveClientAuthority (connectionToClient);
		}
        
		foreach (Hex h in BoardState.instance.hexPoisition.Values)
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
	public bool placeInitialRoad (Edge e, Enums.Color color, List<GamePiece> pieces, bool server)
    {
		if (!ma.canPlaceInitialRoad (e, color))
        {
			return false;
		}

		if (!server) {
			objNetId = this.GetComponent<NetworkIdentity> ();        
			objNetId.AssignClientAuthority (connectionToClient);  
		}

        RpcBuildRoad(e.transform.position, color);

		if (!server) {
			objNetId.RemoveClientAuthority (connectionToClient);
		}

		return true;
	}
	 
	// Place an initial ship
	public bool placeInitialShip (Edge e, Enums.Color color, List<GamePiece> pieces, bool server)
    {
		Debug.Log ("Before ma");

		if (!ma.canPlaceInitialShip (e, color))
        {
			return false;
		}

		Debug.Log ("After ma");

		if (!server) {
			objNetId = this.GetComponent<NetworkIdentity> ();        
			objNetId.AssignClientAuthority (connectionToClient);  
		}

        RpcBuildShip(e.transform.position, color);

		if (!server) {
			objNetId.RemoveClientAuthority (connectionToClient);
		}
		return true;
	}

	private GameObject getKnightFromLevel(int level, Vector3 location) {
		GameObject knight;
		switch (level) {
		case 1:
			knight = Instantiate<GameObject> (prefabHolder.levelOneKnight, location, Quaternion.identity);
			break;
		case 2:
			knight = Instantiate<GameObject> (prefabHolder.levelTwoKnight, location, Quaternion.identity);
			break;
		case 3:
			knight = Instantiate<GameObject> (prefabHolder.levelThreeKnight, location, Quaternion.identity);
			break;
		default:
			knight = null;
			break;
		}
		return knight;
	}
		

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
		Player current = GameManager.instance.getPlayer(GameManager.instance.getPlayerTurn());
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
}
