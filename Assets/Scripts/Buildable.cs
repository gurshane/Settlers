using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable {

	private MoveAuthorizer ma;

	public Buildable() {
		ma = new MoveAuthorizer();
	}

	public List<Vertex> buildableMoveKnight (Enums.Color color) {

		return findActiveKnights(color);
	}

	public List<Vertex> buildableDisplaceKnight (Enums.Color color) {

		return findActiveKnights(color);
	}

	public List<Vertex> buildableUpgradgeKnight (int[] resources, int[] devChart, List<GamePiece> pieces) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canUpgradeKnight(resources, devChart, v, pieces)) {
				vertices.Add(v);
			}
		}
		return vertices;
	}

	public List<Vertex> buildableActivateKnight (int[] resources) {

		List<Vertex> vertices = new List<Vertex>();

		foreach (Vertex v in BoardState.instance.vertexPosition.Values) {
			if (ma.canActivateKnight(resources, v)) {
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
}
