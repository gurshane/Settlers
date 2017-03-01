using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public static class MoveManager {

	// Move a knight from source to target
	public static bool moveKnight(Vertex source, Vertex target, Enums.Color color, int currentLongest) {

		// Check if the knight can be moved
		if (!MoveAuthorizer.canKnightMove (source, target, color)) {
			return false;
		}

		Knight sourceKnight = (Knight)source.getOccupyingPiece ();

		// Move the knight
		source.setOccupyingPiece (null);
		target.setOccupyingPiece (sourceKnight);

		// Deactivate the knight
		sourceKnight.deactivateKnight ();

		// Check if longest route needs to be updated

		return true;
	}

	// Displace a knight at target with knight at source
	public static bool displaceKnight(Vertex source, Vertex target, Enums.Color color) {
		
		// Check if the knight can displace
		if (!MoveAuthorizer.canKnightDisplace (source, target, color)) {
			return false;
		}
			
		Knight sourceKnight = (Knight)source.getOccupyingPiece ();
		Knight targetKnight = (Knight)target.getOccupyingPiece ();

		// Move the knight
		source.setOccupyingPiece (null);

		Vertex displacedTarget = new Vertex(Enums.TerrainType.LAND); // = forcedDisplacement(target);
		target.setOccupyingPiece (sourceKnight);
		displacedTarget.setOccupyingPiece (targetKnight);

		// Deactivate the knight
		sourceKnight.deactivateKnight ();

		// Check if longest route needs to be updated

		return true;
	}

	// Upgrade a knight at vertex v
	public static bool upgradeKnight(int[] resources, int[] devChart, Vertex v) {

		return true;
	}

	// Activate a knight at vertex v
	public static bool activateKnight(int[] resources, Vertex v) {
		return true;
	}

	// Upgrade a development chart in the specified field
	public static bool upgradeDevChart(Enums.DevChartType dev, int[] commodities, 
		List<GamePiece> pieces, int[] devChart) {

		return true;
	}

	// Build a settlement at location
	public static bool buidSettlement(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color) {

		if (!MoveAuthorizer.canBuildSettlement (location, resources, pieces, color)) {
			return false;
		}

		// Put a settlement on the board
		Settlement settlement = Settlement.getFreeSettlement (pieces);
		location.setOccupyingPiece (settlement);
		settlement.putOnBoard ();

		// Add an appropriate amount of victory points
		Player current = GameManager.getCurrentPlayer ();
		current.incrementVictoryPoints (1);
		current.incrementVictoryPoints (location.getChits ());

		// Spend the correct resources
		current.discardResource (Enums.ResourceType.BRICK, 1);
		Bank.depositResource (Enums.ResourceType.BRICK, 1);

		current.discardResource (Enums.ResourceType.GRAIN, 1);
		Bank.depositResource (Enums.ResourceType.GRAIN, 1);

		current.discardResource (Enums.ResourceType.WOOL, 1);
		Bank.depositResource (Enums.ResourceType.WOOL, 1);

		current.discardResource (Enums.ResourceType.LUMBER, 1);
		Bank.depositResource (Enums.ResourceType.LUMBER, 1);

		// Check if there is a port
		updatePort (location);

		// Update longest road

		return true;
	}


	// Build a city at location
	public static bool buildCity(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color) {

		if (!MoveAuthorizer.canBuildCity (location, resources, pieces, color)) {
			return false;
		}

		// Remove the current settlement
		GamePiece settlement = location.getOccupyingPiece ();
		settlement.takeOffBoard ();

		// Build a city
		City city = City.getFreeCity (pieces);
		location.setOccupyingPiece (city);
		city.putOnBoard ();

		// Add an appropriate amount of victory points
		Player current = GameManager.getCurrentPlayer ();
		current.incrementVictoryPoints (1);

		// Spend the resources
		current.discardResource (Enums.ResourceType.GRAIN, 2);
		Bank.depositResource (Enums.ResourceType.GRAIN, 2);

		current.discardResource (Enums.ResourceType.ORE, 3);
		Bank.depositResource (Enums.ResourceType.ORE, 3);

		return true;
	}

	// Check if a city wall can be built at a vertex
	public static bool buildCityWall(Vertex location, int[] resources,
		int cityWalls, Enums.Color color) {

		return true;
	}

	// Build a knight at location
	public static bool buildKnight(Vertex location, int[] resources,
		List<GamePiece> pieces, Enums.Color color) {

		return true;
	}

	// Build a road at location
	public static bool buildRoad(Edge location, int[] resources,
		List<GamePiece> pieces, Enums.Color color) {

		if (!MoveAuthorizer.canBuildRoad (location, resources, pieces, color)) {
			return false;
		}

		// Put a road on the given edge
		Road road = Road.getFreeRoad (pieces);
		location.setOccupyingPiece (road);
		road.putOnBoard ();
		road.wasBuiltThisTurn ();

		Player current = GameManager.getCurrentPlayer ();

		// Spend the resources
		current.discardResource (Enums.ResourceType.BRICK, 1);
		Bank.depositResource (Enums.ResourceType.BRICK, 1);

		current.discardResource (Enums.ResourceType.LUMBER, 1);
		Bank.depositResource (Enums.ResourceType.LUMBER, 1);

		//Update longest route

		return true;
	}

	// Build a ship at location
	public static bool buildShip(Edge location, int[] resources,
		List<GamePiece> pieces, Enums.Color color) {

		if (!MoveAuthorizer.canBuildShip (location, resources, pieces, color)) {
			return false;
		}

		// Put a road on the given edge
		Road ship = Road.getFreeShip (pieces);
		location.setOccupyingPiece (ship);
		ship.putOnBoard ();
		ship.wasBuiltThisTurn ();

		Player current = GameManager.getCurrentPlayer ();

		// Spend the resources
		current.discardResource (Enums.ResourceType.WOOL, 1);
		Bank.depositResource (Enums.ResourceType.WOOL, 1);

		current.discardResource (Enums.ResourceType.LUMBER, 1);
		Bank.depositResource (Enums.ResourceType.LUMBER, 1);

		//Update longest route

		return true;
	}

	// Move a ship from source to target
	public static bool moveShip(Edge source, Edge target, Enums.Color color) {
		return true;
	}

	// Chase robber from source
	public static bool chaseRobber(Vertex source) {
		return true;
	}

	// Move robber to target
	public static bool moveRobber(Hex target) {
		return true;
	}

	// Move Pirate to target
	public static bool movePirate(Hex target) {
		return true;
	}

	// Place Merchant at target
	public static bool placeMerchant(Hex target) {



		return true;
	}

	// Place an initial settlement
	public static bool placeInitialSettlement (Vertex v, List<GamePiece> pieces, List<Hex> validHexes) {
		if (!MoveAuthorizer.canPlaceInitialTownPiece (v, validHexes)) {
			return false;
		}
	
		// Place a settlement
		Settlement settlement = Settlement.getFreeSettlement (pieces);
		v.setOccupyingPiece (settlement);
		settlement.putOnBoard ();

		// Get the resources around the settlement
		Player current = GameManager.getCurrentPlayer ();
		foreach (Hex h in validHexes) {
			if (h.adjacentToVertex (v)) {
				Enums.ResourceType res = GameManager.getResourceFromHex (h.getHexType());
				if (res != Enums.ResourceType.NONE) {
					current.addResource (res, 1);
					Bank.withdrawResource (res, 1);
				}
			}
		}

		// Update the victory points and add a port
		current.incrementVictoryPoints (1);
		updatePort (v);

		return true;
	}

	// Place an initial city
	public static bool placeInitialCity (Vertex v, List<GamePiece> pieces, List<Hex> validHexes) {
		if (!MoveAuthorizer.canPlaceInitialTownPiece (v, validHexes)) {
			return false;
		}

		// Place a city
		City city = City.getFreeCity (pieces);
		v.setOccupyingPiece (city);
		city.putOnBoard ();

		// Get the resources around the city
		Player current = GameManager.getCurrentPlayer ();
		foreach (Hex h in validHexes) {
			if (h.adjacentToVertex (v)) {
				Enums.ResourceType res = GameManager.getResourceFromHex (h.getHexType());
				if (res != Enums.ResourceType.NONE) {
					current.addResource (res, 1);
					Bank.withdrawResource (res, 1);
				}
			}
		}

		// Update the victory points and add a port 
		current.incrementVictoryPoints (2);
		updatePort (v);

		return true;
	}

	// Place an initial road
	public static bool placeInitialRoad (Edge e, Enums.Color color, List<GamePiece> pieces) {
		if (!MoveAuthorizer.canPlaceInitialRoad (e, color)) {
			return false;
		}

		// Put a road on the given edge
		Road road = Road.getFreeRoad (pieces);
		e.setOccupyingPiece (road);
		road.putOnBoard ();

		return true;
	}
	 
	// Place an initial ship
	public static bool placeInitialShip (Edge e, Enums.Color color, List<GamePiece> pieces) {
		if (!MoveAuthorizer.canPlaceInitialShip (e, color)) {
			return false;
		}
			
		// Put a ship on the given edge
		Road ship = Road.getFreeShip (pieces);
		e.setOccupyingPiece (ship);
		ship.putOnBoard ();

		return true;
	}

	// Get a resource type from a port type
	private static Enums.ResourceType getResourceFromPort(Enums.PortType port) {

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
	private static void updatePort(Vertex v) {
		Player current = GameManager.getCurrentPlayer ();
		int[] ratios = current.getResourceRatios ();
		Enums.PortType port = v.getPortType ();

		// If the port is generic, update the ratios accordingly
		if (port == Enums.PortType.GENERIC) {
			for (int i = 0; i < ratios.Length; i++) {
				if (ratios [i] > 3) {
					ratios [i] = 3;
				}
			}
			current.updateResoureRatios (ratios);

		// If there is a specific port, update the correct ratio
		} else if (port != Enums.PortType.NONE) {
			current.updateResourceRatio (getResourceFromPort (port), 2);
		}
	}
}
