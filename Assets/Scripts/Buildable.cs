using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable {

	private MoveAuthorizer ma;
	private Graph graph;

	public Buildable() {
		ma = new MoveAuthorizer();
		graph = new Graph();
	}

	public List<Vertex> buildableMoveKnight (Enums.Color color) {

		return findActiveKnights(color);
	}

	public List<Vertex> buildableDisplaceKnight (Enums.Color color) {

		return findActiveKnights(color);
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
}
