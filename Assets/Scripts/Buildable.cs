using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class Buildable {

	private MoveAuthorizer ma;
	private ProgressAuthroizor pa;
	private Graph graph;

	public Buildable() {
		ma = new MoveAuthorizer();
		pa = new ProgressAuthroizor();
		graph = new Graph();
	}

	public List<Vertex> buildableMoveKnight (Enums.Color color) {

		Vertex v1 = GameManager.instance.getPersonalPlayer().v1;
		if (!Object.ReferenceEquals(v1, null)) {
			List<Vertex> vertices = new List<Vertex>();

			foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
				if (ma.canKnightMove(v1, v, color)) {
					vertices.Add(v);
				}
			}
			return vertices;
		} else {
			return findActiveKnights(color);
		}
	}

	public List<Vertex> buildableDisplaceKnight (Enums.Color color) {

		Vertex v1 = GameManager.instance.getPersonalPlayer().v1;
		if (!Object.ReferenceEquals(v1, null)) {
			List<Vertex> vertices = new List<Vertex>();

			foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
				if (ma.canKnightDisplace(v1, v, color)) {
					vertices.Add(v);
				}
			}
			return vertices;
		} else {
			return findActiveKnights(color);
		}
	}

	public List<Vertex> buildableUpgradgeKnight (int[] resources, int[] devChart, List<GamePiece> pieces, Enums.Color color) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canUpgradeKnight(resources, devChart, v, pieces, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Vertex> buildableActivateKnight (int[] resources, Enums.Color color) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canActivateKnight(resources, v, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Enums.DevChartType> buildableUpgradeDevChart(int[] commodities,
        List<GamePiece> pieces, int[] devChart) {

		List<Enums.DevChartType> devs = new List<Enums.DevChartType>();

		for (int i = 0; i < devChart.Length; i++) {
			if (ma.canUpgradeDevChart((Enums.DevChartType)i, commodities, pieces, devChart)) {
				devs.Add((Enums.DevChartType)i);
			}
		}
		return devs;
	}

	public List<Vertex> buildableBuildSettlement (int[] resources,
        List<GamePiece> pieces, Enums.Color color) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canBuildSettlement(v, resources, pieces, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Vertex> buildableBuildCity (int[] resources,
        List<GamePiece> pieces, Enums.Color color) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canBuildCity(v, resources, pieces, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Vertex> buildableBuildCityWall (int[] resources,
        int cityWalls, Enums.Color color) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canBuildCityWall(v, resources, cityWalls, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Vertex> buildableBuildKnight (int[] resources,
        List<GamePiece> pieces, Enums.Color color) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canBuildKnight(v, resources, pieces, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Edge> buildableBuildRoad (int[] resources,
        List<GamePiece> pieces, Enums.Color color) {

		List<Edge> edges = new List<Edge>();

		foreach (Edge e in BoardState.instance.edgePosition.Values) {
			if (ma.canBuildRoad(e, resources, pieces, color)) {
				edges.Add(e);
			}
		}
		return edges;
	}

	public List<Edge> buildableMoveShip (Enums.Color color) {

		List<Edge> edges = new List<Edge>();

		Edge e1 = GameManager.instance.getPersonalPlayer().e1;
		if (!Object.ReferenceEquals(e1, null)) {
			foreach (Edge e in BoardState.instance.edgePosition.Values) {
				if (ma.canShipMove(e1, e, color)) {
					edges.Add(e);
				}
			}
			return edges;
		}

		foreach (Edge source in BoardState.instance.edgePosition.Values) {
			GamePiece sourcePiece = source.getOccupyingPiece();

			// Make sure there is a ship on the source edge
			if (Object.ReferenceEquals(sourcePiece, null))
			{
				continue;
			}
			if (sourcePiece.getPieceType() != Enums.PieceType.ROAD)
			{
				continue;
			}
			if (sourcePiece.getColor() != color)
			{
				continue;
			}

			Road ship = (Road)sourcePiece;
			if (!ship.getIsShip())
			{
				continue;
			}

			// Make sure the source ship is valid for moving
			if (graph.nextToMyPiece(source, color))
			{
				continue;
			}
			if (graph.isClosedShip(source, color))
			{
				continue;
			}
			if (ship.getBuiltThisTurn())
			{
				continue;
			}

			// Make sure the source edge is not next to the pirate
			Hex checkHex = source.getLeftHex();
			GamePiece hexPiece;
			if (!Object.ReferenceEquals(checkHex, null))
			{
				hexPiece = checkHex.getOccupyingPiece();
				if (!Object.ReferenceEquals(hexPiece, null))
				{
					if (hexPiece.getPieceType() == Enums.PieceType.PIRATE)
					{
						continue;
					}
				}
			}
			checkHex = source.getRightHex();
			if (!Object.ReferenceEquals(checkHex, null))
			{
				hexPiece = checkHex.getOccupyingPiece();
				if (!Object.ReferenceEquals(hexPiece, null))
				{
					if (hexPiece.getPieceType() == Enums.PieceType.PIRATE)
					{
						continue;
					}
				}
			}

			edges.Add(source);
		}

		return edges;
	}

	public List<Edge> buildableBuildShip (int[] resources,
        List<GamePiece> pieces, Enums.Color color) {

		List<Edge> edges = new List<Edge>();

		foreach (Edge e in BoardState.instance.edgePosition.Values) {
			if (ma.canBuildShip(e, resources, pieces, color)) {
				edges.Add(e);
			}
		}
		return edges;
	}

	private List<Vertex> findActiveKnights(Enums.Color color) {
		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			GamePiece sourcePiece = v.getOccupyingPiece();

			// Make sure the vertex has an available knight
			if (Object.ReferenceEquals(sourcePiece, null))
			{
				continue;
			}
			if (sourcePiece.getPieceType() != Enums.PieceType.KNIGHT)
			{
				continue;
			}
			if (sourcePiece.getColor() != color)
			{
				continue;
			}

			Knight sourceKnight = (Knight)sourcePiece;
			if (!sourceKnight.isActive())
			{
				continue;
			}
			if (sourceKnight.wasActivatedThisTurn())
			{
				continue;
        	}

			vertices.Add(v);
		}
		return vertices;
	}

	public List<Vertex> buildableChaseRobber (Enums.Color color) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canChaseRobber(v, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Vertex> buildablePlaceInitialTownPiece () {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canPlaceInitialTownPiece(v)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Edge> buildablePlaceInitialRoad (Enums.Color color) {

		List<Edge> edges = new List<Edge>();

		foreach (Edge e in BoardState.instance.edgePosition.Values) {
			if (ma.canPlaceInitialRoad(e, color)) {
				edges.Add(e);
			}
		}
		return edges;
	}

	public List<Edge> buildablePlaceInitialShip (Enums.Color color) {

		List<Edge> edges = new List<Edge>();

		foreach (Edge e in BoardState.instance.edgePosition.Values) {
			if (ma.canPlaceInitialShip(e, color)) {
				edges.Add(e);
			}
		}
		return edges;
	}

	public List<Vertex> buildableCanStealRobber(Enums.Color color) {
		
		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canStealRobber(v, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Edge> buildableCanStealPirate(Enums.Color color) {
		
		List<Edge> edges = new List<Edge>();

		foreach (Edge e in BoardState.instance.edgePosition.Values) {
			if (ma.canStealPirate(e, color)) {
				edges.Add(e);
			}
		}
		return edges;
	}

	public List<Hex> buildableCanMoveRobber() {
		List<Hex> hexes = new List<Hex>();

		foreach (Hex h in BoardState.instance.hexPosition.Values) {
			if (ma.canMoveRobber(h)) {
				hexes.Add(h);
			}
		}
		return hexes;
	}

	public List<Hex> buildableCanMovePirate() {
		List<Hex> hexes = new List<Hex>();

		foreach (Hex h in BoardState.instance.hexPosition.Values) {
			if (ma.canMovePirate(h)) {
				hexes.Add(h);
			}
		}
		return hexes;
	}

	public List<Vertex> buildableCanDestroyCity(Enums.Color color) {
		List<Vertex> vertices = new List<Vertex>();
		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canDestroyCity(v, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Vertex> buildableCanChooseMetropolis(Enums.Color color) {
		List<Vertex> vertices = new List<Vertex>();
		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canChooseMetropolis(v, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Edge> buildableFishRoad (int numFish,
        List<GamePiece> pieces, Enums.Color color) {

		List<Edge> edges = new List<Edge>();

		foreach (Edge e in BoardState.instance.edgePosition.Values) {
			if (ma.canFishRoad(e, numFish, pieces, color)) {
				edges.Add(e);
			}
		}
		return edges;
	}

	public List<Vertex> buildableForcedKnightMove(Enums.Color color) {

		Vertex v1 = GameManager.instance.getPersonalPlayer().v1;
		List<Vertex> vertices = new List<Vertex>();

		if (!Object.ReferenceEquals(v1, null)) {
			
			foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
				if (ma.canForcedKnightMove(v1, v, color)) {
					vertices.Add(v);
				}
			}
			return vertices;
		} 

		return vertices;
	}

	public List<Edge> buildableMoveRoad (Enums.Color color) {

		List<Edge> edges = new List<Edge>();

		Edge e1 = GameManager.instance.getPersonalPlayer().e1;
		if (!Object.ReferenceEquals(e1, null)) {
			foreach (Edge e in BoardState.instance.edgePosition.Values) {
				if (pa.canRoadMove(e1, e, color)) {
					edges.Add(e);
				}
			}
			return edges;
		}

		foreach (Edge source in BoardState.instance.edgePosition.Values) {
			GamePiece sourcePiece = source.getOccupyingPiece();

			// Make sure there is a ship on the source edge
			if (Object.ReferenceEquals(sourcePiece, null))
			{
				continue;
			}
			if (sourcePiece.getPieceType() != Enums.PieceType.ROAD)
			{
				continue;
			}
			if (sourcePiece.getColor() != color)
			{
				continue;
			}

			Road road = (Road)sourcePiece;
			if (road.getIsShip())
			{
				continue;
			}

			// Make sure the source ship is valid for moving
			if (graph.nextToMyPiece(source, color))
			{
				continue;
			}
			if (graph.isClosedRoad(source, color))
			{
				continue;
			}

			edges.Add(source);
		}

		return edges;
	}

	public List<Vertex> buildableIntrigueKnight (Enums.Color color) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (pa.canIntrigueKnight(v, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Vertex> buildableEngineer (int cityWallsLeft, Enums.Color color) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (pa.canEngineer(v, cityWallsLeft, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Hex> buildableInventor() {
		List<Hex> hexes = new List<Hex>();

		Hex h1 = GameManager.instance.getPersonalPlayer().h1;

		if (!Object.ReferenceEquals(h1, null)) {
			foreach (Hex h in BoardState.instance.hexPosition.Values) {
				if (pa.canInventor(h1, h)) {
					hexes.Add(h);
				}
			}
		} else {
			foreach (Hex h in BoardState.instance.hexPosition.Values) {

				if (h.getTerrainType() == TerrainType.WATER) continue;

				if (h.getHexType() == HexType.DESERT) continue;

				int i = h.getHexNumber();

				if (i == 2 || i == 12 || i == 6 || i == 8) continue;

				hexes.Add(h);
			}
		}

		
		return hexes;
	}

	public List<Vertex> buildableMedicine (int[] resources,
        List<GamePiece> pieces, Enums.Color color) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (pa.canMedicine(v, resources, pieces, color)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Edge> buildableRoadBuilding (List<GamePiece> pieces, Enums.Color color) {

		List<Edge> edges = new List<Edge>();

		foreach (Edge e in BoardState.instance.edgePosition.Values) {
			if (pa.canRoadBuilding(e, pieces, color)) {
				edges.Add(e);
			}
		}
		return edges;
	}

	public List<Hex> buildablePlaceMerchant () {

		List<Hex> hexes = new List<Hex>();

		foreach (Hex h in BoardState.instance.hexPosition.Values) {
			if (ma.canPlaceMerchant(h)) {
				hexes.Add(h);
			}
		}
		return hexes;
	}

}
