﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public static class MoveAuthorizer {

	// Check if a knight can be moved from one vertex to another
	public static bool canKnightMove(Vertex source, Vertex target, Enums.Color color) {

		// Make sure there is a knight that can be moved
		GamePiece sourcePiece = source.getOccupyingPiece ();
		if (Object.ReferenceEquals(sourcePiece, null)) {
			return false;
		}
		if (sourcePiece.getPieceType () != Enums.PieceType.KNIGHT) {
			return false;
		}
		if (sourcePiece.getColor () != color) {
			return false;
		}

		Knight sourceKnight = (Knight)sourcePiece;
		if (!sourceKnight.isActive ()) {
			return false;
		}
		if (sourceKnight.wasActivatedThisTurn ()) {
			return false;
		}

		// Make sure there is nothing at the target vertex
		GamePiece targetPiece = target.getOccupyingPiece ();
		if (!Object.ReferenceEquals(targetPiece, null)) {
			return false;
		}

		// Check if the vertices are connected
		Graph.vertexReset (source);
		return Graph.areConnectedVertices (source, target, color);
	}

	// Check if a knight can displace another knight
	public static bool canKnightDisplace(Vertex source, Vertex target, Enums.Color color) {

		// Make sure there is a knight that can be moved
		GamePiece sourcePiece = source.getOccupyingPiece ();
		if (Object.ReferenceEquals(sourcePiece, null)) {
			return false;
		}
		if (sourcePiece.getPieceType () != Enums.PieceType.KNIGHT) {
			return false;
		}
		if (sourcePiece.getColor () != color) {
			return false;
		}

		Knight sourceKnight = (Knight)sourcePiece;
		if (!sourceKnight.isActive ()) {
			return false;
		}
		if (sourceKnight.wasActivatedThisTurn ()) {
			return false;
		}

		// Make sure there is a lower-level knight at the target vertex
		GamePiece targetPiece = target.getOccupyingPiece ();
		if (!Object.ReferenceEquals (targetPiece, null)) {
			if (targetPiece.getColor () == color) {
				return false;
			}
			if (targetPiece.getPieceType () != Enums.PieceType.KNIGHT) {
				return false;
			}

			Knight targetKnight = (Knight)targetPiece;
			if (targetKnight.getLevel () >= sourceKnight.getLevel ()) {
				return false;
			}
		} else { 
			return false;
		}

		// Check to see if the vertices are connected
		Graph.vertexReset (source);
		return Graph.areConnectedVertices (source, target, color);
	}

	// Check if a knight can be upgraded
	public static bool canUpgradeKnight(Dictionary<Enums.ResourceType, int> resources, 
		Dictionary<Enums.DevChartType, int> devChart, Vertex v) {

		// Make sure there are enough resources
		int wool = resources [Enums.ResourceType.WOOL];
		int sheep = resources [Enums.ResourceType.ORE];

		if (wool < 1) {
			return false;
		}
		if (sheep < 1) {
			return false;
		}

		// Make sure there is a knight that can be upgraded at the vertex
		GamePiece sourcePiece = v.getOccupyingPiece ();
		if (Object.ReferenceEquals(sourcePiece, null)) {
			return false;
		}
		if (sourcePiece.getPieceType () != Enums.PieceType.KNIGHT) {
			return false;
		}

		Knight sourceKnight = (Knight)sourcePiece;
		if (sourceKnight.wasUpgraded ()) {
			return false;
		}

		// Check the politics level for high-level knights
		int level = devChart [Enums.DevChartType.POLITICS];
		if (level < 3) {
			if (sourceKnight.getLevel () >= 2) {
				return false;
			}
		} else if (sourceKnight.getLevel () >= 3) {
			return false;
		}
		return true;
	}

	// Check if a knight can be activated
	public static bool canActivateKnight(Dictionary<Enums.ResourceType, int> resources, Vertex v) {

		// Make sure there is a grain available
		if (resources [Enums.ResourceType.GRAIN] < 1) {
			return false;
		}

		// Make sure there is a knight that can be activated
		GamePiece sourcePiece = v.getOccupyingPiece ();
		if (Object.ReferenceEquals(sourcePiece, null)) {
			return false;
		}
		if (sourcePiece.getPieceType () != Enums.PieceType.KNIGHT) {
			return false;
		}

		Knight sourceKnight = (Knight)sourcePiece;
		if (sourceKnight.isActive ()) {
			return false;
		}
		return true;
	}

	// Check if the development chart can be upgraded for a specific development type
	public static bool canUpgradeDevChart(Enums.DevChartType dev, Dictionary<Enums.CommodityType, int> commodities, 
		List<GamePiece> pieces, Dictionary<Enums.DevChartType, int> devChart) {

		// Make sure there is a city on the board
		bool cityOnBoard = false;
		foreach (GamePiece p in pieces) {
			if (p.getPieceType () != Enums.PieceType.CITY) {
				continue;
			} else {
				if (p.isOnBoard ()) {
					cityOnBoard = true;
					break;
				}
			}
		}
		if (!cityOnBoard) { 
			return false;
		}
	    
		// Make sure there are enough resources for the upgrade
		// Or that the chart is not at maximum capacity
		int level, coms;
		if (dev == Enums.DevChartType.TRADE) {
			level = devChart [Enums.DevChartType.TRADE];
			coms = commodities [Enums.CommodityType.CLOTH];
			if (coms < level) {
				return false;
			} else if (level >= 5) {
				return false;
			}
		} else if (dev == Enums.DevChartType.POLITICS) {
			level = devChart [Enums.DevChartType.POLITICS];
			coms = commodities [Enums.CommodityType.COIN];
			if (coms < level) {
				return false;
			} else if (level >= 5) {
				return false;
			}
		} else if (dev == Enums.DevChartType.SCIENCE) {
			level = devChart [Enums.DevChartType.SCIENCE];
			coms = commodities [Enums.CommodityType.PAPER];
			if (coms < level) {
				return false;
			} else if (level >= 5) {
				return false;
			}
		}
		return true;
	}

	// Check if a settlement can be built at given vertex
	public static bool canBuildSettlement(Vertex location, Dictionary<Enums.ResourceType, int> resources,
		List<GamePiece> pieces, Enums.Color color) {

		// Make sure the location is valid
		if (location.getTerrainType () == Enums.TerrainType.WATER) {
			return false;
		}
		if (!Graph.freeVertex (location)) {
			return false;
		}
		if (!Graph.nextToMyEdge (location, color)) {
			return false;
		}

		// Make sure there is an available piece
		bool availablePiece = false;
		foreach (GamePiece p in pieces) {
			if (p.getPieceType () != Enums.PieceType.SETTLEMENT) {
				continue;
			}
			if (!p.isOnBoard ()) {
				availablePiece = true;
			}
		}
		if (!availablePiece) {
			return false;
		}

		// Make sure there are enough resources
		if (resources [Enums.ResourceType.GRAIN] < 1) {
			return false;
		}
		if (resources [Enums.ResourceType.WOOL] < 1) {
			return false;
		}
		if (resources [Enums.ResourceType.BRICK] < 1) {
			return false;
		}
		if (resources [Enums.ResourceType.LUMBER] < 1) {
			return false;
		}
		return true;
	}


	// Check if a city can be built at a vertex
	public static bool canBuildCity(Vertex location, Dictionary<Enums.ResourceType, int> resources,
		List<GamePiece> pieces, Enums.Color color) {

		GamePiece settlement = location.getOccupyingPiece ();

		// Make sure the location is valid
		if (Object.ReferenceEquals(settlement, null)) {
			return false;
		}
		if (settlement.getPieceType () != Enums.PieceType.SETTLEMENT) {
			return false;
		}
		if (settlement.getColor () != color) {
			return false;
		}

		// Make sure there is an available city
		bool availablePiece = false;
		foreach (GamePiece p in pieces) {
			if (p.getPieceType () != Enums.PieceType.CITY) {
				continue;
			}
			if (!p.isOnBoard ()) {
				availablePiece = true;
			}
		}
		if (!availablePiece) {
			return false;
		}

		// Make sure there are enough resources
		if (resources [Enums.ResourceType.GRAIN] < 2) {
			return false;
		}
		if (resources [Enums.ResourceType.ORE] < 3) {
			return false;
		}
		return true;
	}

	// Check if a city wall can be built at a vertex
	public static bool canBuildCityWall(Vertex location, Dictionary<Enums.ResourceType, int> resources,
		int cityWalls, Enums.Color color) {

		GamePiece city = location.getOccupyingPiece ();

		// Make sure the location is valid
		if (Object.ReferenceEquals(city, null)) {
			return false;
		}
		if (city.getPieceType () != Enums.PieceType.CITY) {
			return false;
		}
		if (location.getHasWall ()) {
			return false;
		}
		if (city.getColor () != color) {
			return false;
		}
		if (cityWalls < 1) {
			return false;
		}

		// Make sure there are enough resources
		if (resources [Enums.ResourceType.BRICK] < 2) {
			return false;
		}
		return true;
	}

	// Check if knight can be built at vertex
	public static bool canBuildKnight(Vertex location, Dictionary<Enums.ResourceType, int> resources,
		List<GamePiece> pieces, Enums.Color color) {

		// Make sure location is valid
		if (!Object.ReferenceEquals(location.getOccupyingPiece(), null)) {
			return false;
		}
		if (!Graph.nextToMyEdge (location, color)) {
			return false;
		}

		// Make sure there is an available knight
		bool availablePiece = false;
		foreach (GamePiece p in pieces) {
			if (p.getPieceType () != Enums.PieceType.KNIGHT) {
				continue;
			}
			if (!p.isOnBoard ()) {
				availablePiece = true;
			}
		}
		if (!availablePiece) {
			return false;
		}

		// Make sure there are enough resources
		if (resources [Enums.ResourceType.WOOL] < 1) {
			return false;
		}
		if (resources [Enums.ResourceType.ORE] < 1) {
			return false;
		}
		return true;
	}

	// Check if a road can be built on an edge
	public static bool canBuildRoad(Edge location, Dictionary<Enums.ResourceType, int> resources,
		List<GamePiece> pieces, Enums.Color color) {

		// Make sure the location is valid
		if (!Object.ReferenceEquals(location.getOccupyingPiece(), null)) {
			return false;
		}
		if (location.getTerrainType () == Enums.TerrainType.WATER) {
			return false;
		}

		bool nextToTown = Graph.nextToMyCityOrSettlement (location, color);
		bool nextToRoad = false;
		if (Graph.nextToMyRoad (location.getLeftVertex (), color)) {
			nextToRoad = true;
		} else if (Graph.nextToMyRoad (location.getRightVertex (), color)) {
			nextToRoad = true;
		}
		if (!nextToTown && !nextToRoad) {
			return false;
		}

		// Make sure there is an available road
		bool availablePiece = false;
		foreach (GamePiece p in pieces) {
			if (p.getPieceType () != Enums.PieceType.ROAD) {
				continue;
			}

			Road currentRoad = (Road)p;
			if (currentRoad.getIsShip ()) {
				continue;
			}
			if (!p.isOnBoard ()) {
				availablePiece = true;
			}
		}
		if (!availablePiece) {
			return false;
		}

		// Make sure there are enough resources
		if (resources [Enums.ResourceType.BRICK] < 1) {
			return false;
		}
		if (resources [Enums.ResourceType.LUMBER] < 1) {
			return false;
		}
		return true;
	}

	// Check if a ship can be built on an edge
	public static bool canBuildShip(Edge location, Dictionary<Enums.ResourceType, int> resources,
		List<GamePiece> pieces, Enums.Color color) {

		// Make sure the location is valid
		if (!Object.ReferenceEquals(location.getOccupyingPiece(), null)) {
			return false;
		}

		bool nextToWater = false;
		if (!Object.ReferenceEquals(location.getLeftHex(), null)) {
			if (location.getLeftHex().getTerrainType() == Enums.TerrainType.WATER) {
				nextToWater = true;
			}
		}
		if (!Object.ReferenceEquals(location.getRightHex(), null)) {
			if (location.getRightHex ().getTerrainType () == Enums.TerrainType.WATER) {
				nextToWater = true;
			}
		}
		if (!nextToWater) {
			return false;
		}

		bool nextToTown = Graph.nextToMyCityOrSettlement (location, color);
		bool nextToShip = false;
		if (Graph.nextToMyShip (location.getLeftVertex (), color)) {
			nextToShip = true;
		} else if (Graph.nextToMyShip (location.getRightVertex (), color)) {
			nextToShip = true;
		}
		if (!nextToTown && !nextToShip) {
			return false;
		}

		// Make sure there is an available piece
		bool availablePiece = false;
		foreach (GamePiece p in pieces) {
			if (p.getPieceType () != Enums.PieceType.ROAD) {
				continue;
			}

			Road currentRoad = (Road)p;
			if (!currentRoad.getIsShip ()) {
				continue;
			}
			if (!p.isOnBoard ()) {
				availablePiece = true;
			}
		}
		if (!availablePiece) {
			return false;
		}

		// Make sure there are enough resources
		if (resources [Enums.ResourceType.WOOL] < 1) {
			return false;
		}
		if (resources [Enums.ResourceType.LUMBER] < 1) {
			return false;
		}
		return true;
	}

	// Check if a ship can be moved to an edge
	public static bool canShipMove(Edge source, Edge target, Enums.Color color) {
		GamePiece sourcePiece = source.getOccupyingPiece ();

		// Make sure there is a ship on the source edge
		if (Object.ReferenceEquals(sourcePiece, null)) {
			return false;
		}
		if (sourcePiece.getPieceType () != Enums.PieceType.ROAD) {
			return false;
		}
		if (sourcePiece.getColor () != color) {
			return false;
		}

		Road ship = (Road)sourcePiece;
		if (!ship.getIsShip()) {
			return false;
		}

		// Make sure the target edge is empty
		if (!Object.ReferenceEquals(target.getOccupyingPiece(), null)) {
			return false;
		}
	
		// Make sure the source ship is valid for moving
		if (Graph.nextToMyPiece (source, color)) {
			return false;
		}
		if (Graph.isClosedShip (source, color)) {
			return false;
		}
		if (ship.getBuiltThisTurn ()) {
			return false;
		}

		// Make sure the source edge is not next to the pirate
		Hex checkHex = source.getLeftHex();
		GamePiece hexPiece;
		if (!Object.ReferenceEquals(checkHex, null)) {
			hexPiece = checkHex.getOccupyingPiece ();
			if (!Object.ReferenceEquals(hexPiece, null)) {
				if (hexPiece.getPieceType () == Enums.PieceType.PIRATE) {
					return false;
				}
			}
		}
		checkHex = source.getRightHex ();
		if (!Object.ReferenceEquals(checkHex, null)) {
			hexPiece = checkHex.getOccupyingPiece ();
			if (!Object.ReferenceEquals(hexPiece, null)) {
				if (hexPiece.getPieceType () == Enums.PieceType.PIRATE) {
					return false;
				}
			}
		}

		// Make sure the target edge is by water and not next to the pirate
		bool nextToWater = false;
		checkHex = target.getLeftHex ();
		if (!Object.ReferenceEquals(checkHex, null)) {
			hexPiece = checkHex.getOccupyingPiece ();
			if (!Object.ReferenceEquals(hexPiece, null)) {
				if (hexPiece.getPieceType () == Enums.PieceType.PIRATE) {
					return false;
				}
			}
			if (checkHex.getTerrainType() == Enums.TerrainType.WATER) {
				nextToWater = true;
			}
		}
		checkHex = target.getRightHex ();
		if (!Object.ReferenceEquals(checkHex, null)) {
			hexPiece = checkHex.getOccupyingPiece ();
			if (!Object.ReferenceEquals(hexPiece, null)) {
				if (hexPiece.getPieceType () == Enums.PieceType.PIRATE) {
					return false;
				}
			}
			if (checkHex.getTerrainType () == Enums.TerrainType.WATER) {
				nextToWater = true;
			}
		}
		if (!nextToWater) {
			return false;
		}

		// Make sure the target edge is next to a town-piece or ship (not the one being moved)
		bool nextToTown = Graph.nextToMyCityOrSettlement (target, color);
		bool nextToShip = false;
		Vertex current = target.getLeftVertex ();
		foreach (Edge e in current.getNeighbouringEdges()) {
			if (Object.ReferenceEquals (e, target)) {
				continue;
			}

			GamePiece touchingRoad = e.getOccupyingPiece ();
			if (Object.ReferenceEquals (touchingRoad, null)) {
				continue;
			}
			if (Object.ReferenceEquals (touchingRoad, ship)) {
				continue;
			}	
			if (touchingRoad.getPieceType () != Enums.PieceType.ROAD) {
				continue;
			}
			if (touchingRoad.getColor () != color) {
				continue;
			}

			Road touchingShip = (Road)touchingRoad;
			if (!touchingShip.getIsShip ()) {
				continue;
			}
			nextToShip = true;
		}
		current = target.getRightVertex ();
		foreach (Edge e in current.getNeighbouringEdges()) {
			if (Object.ReferenceEquals (e, target)) {
				continue;
			}

			GamePiece touchingRoad = e.getOccupyingPiece ();
			if (Object.ReferenceEquals (touchingRoad, null)) {
				continue;
			}
			if (Object.ReferenceEquals (touchingRoad, ship)) {
				continue;
			}	
			if (touchingRoad.getPieceType () != Enums.PieceType.ROAD) {
				continue;
			}
			if (touchingRoad.getColor () != color) {
				continue;
			}

			Road touchingShip = (Road)touchingRoad;
			if (!touchingShip.getIsShip ()) {
				continue;
			}
			nextToShip = true;
		}
		if (!nextToTown && !nextToShip) {
			return false;
		}
		return true;
	}

	// Check if a knight can chase the robber
	public static bool canChaseRobber(Vertex source) {
		GamePiece sourcePiece = source.getOccupyingPiece ();

		// Make sure the vertex has an available knight
		if (Object.ReferenceEquals(sourcePiece, null)) {
			return false;
		}
		if (sourcePiece.getPieceType () != Enums.PieceType.KNIGHT) {
			return false;
		}

		Knight sourceKnight = (Knight)sourcePiece;
		if (!sourceKnight.isActive ()) {
			return false;
		}
		if (sourceKnight.wasActivatedThisTurn ()) {
			return false;
		}

		// Make sure the vertex is adjacent to the robber
		if (!source.isAdjacentToRobber ()) {
			return false;
		}
		return true;
	}

	// Check if the robber can be move to a nex hex
	public static bool canMoveRobber(Hex target) {
		GamePiece piece = target.getOccupyingPiece ();

		// Make sure the new hex is on land and different
		if (!Object.ReferenceEquals(piece, null)) {
			if (piece.getPieceType () == Enums.PieceType.ROBBER) {
				return false;
			}
		}
		if (target.getTerrainType () == Enums.TerrainType.WATER) {
			return false;
		}
		return true;
	}

	// Check if the pirate can be moved to a new hex
	public static bool canMovePirate(Hex target) {
		GamePiece piece = target.getOccupyingPiece ();

		// Make sure the new hex is on water and different
		if (!Object.ReferenceEquals(piece, null)) {
			if (piece.getPieceType () == Enums.PieceType.PIRATE) {
				return false;
			}
		}
		if (target.getTerrainType () == Enums.TerrainType.LAND) {
			return false;
		}
		return true;
	}

	// Check if the merchant can be placed on a hex
	public static bool canPlaceMerchant(Hex target) {

		// Make sure the new hex is a valid land hex
		if (target.getTerrainType () != Enums.TerrainType.LAND) {
			return false;
		}
		if (target.getHexType () == Enums.HexType.DESERT || 
			target.getHexType() == Enums.HexType.GOLD ||
			target.getHexType() == Enums.HexType.WATER) {

			return false;
		}
		return true;
	}
}
