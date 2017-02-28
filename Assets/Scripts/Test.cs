using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class Test : MonoBehaviour {

	// The board pieces
	List<Vertex> vertices;
	List<Edge> edges;
	List<Hex> hexes;

	// The board piece counts
	int vertexCount;
	int edgeCount;
	int hexCount;

	// Sample hands
	Dictionary<Enums.ResourceType, int> fullRes;
	Dictionary<Enums.ResourceType, int> emptyRes;
	Dictionary<Enums.CommodityType, int> fullCom;
	Dictionary<Enums.CommodityType, int> emptyCom;

	// The game pieces
	List<GamePiece> pieces;

	// Sample development charts
	Dictionary<Enums.DevChartType, int> fullDev;
	Dictionary<Enums.DevChartType, int> medDev;
	Dictionary<Enums.DevChartType, int> emptyDev;

	// The test function
	private void testExample() {

		// Initialize the state
		vertices = new List<Vertex> ();
		edges = new List<Edge> ();
		hexes = new List<Hex> ();

		vertexCount = 40;
		edgeCount = 52;
		hexCount = 13;

		fullRes = new Dictionary<Enums.ResourceType, int> ();
		emptyRes = new Dictionary<Enums.ResourceType, int> ();
		fullCom = new Dictionary<Enums.CommodityType, int> ();
		emptyCom = new Dictionary<Enums.CommodityType, int> ();
		for (int i = 0; i < 5; i++) {
			fullRes.Add ((Enums.ResourceType)i, 10);
		}
		for (int i = 0; i < 5; i++) {
			emptyRes.Add ((Enums.ResourceType)i, 0);
		}
		for (int i = 0; i < 3; i++) {
			fullCom.Add ((Enums.CommodityType)i, 10);
		}
		for (int i = 0; i < 3; i++) {
			emptyCom.Add ((Enums.CommodityType)i, 0);
		}

		pieces = new List<GamePiece> ();
		for (int i = 0; i < 20; i++) {
			pieces.Add (new Road (Enums.Color.WHITE, false));
		}
		for (int i = 0; i < 20; i++) {
			pieces.Add (new Road (Enums.Color.WHITE, true));
		}
		for (int i = 0; i < 20; i++) {
			pieces.Add (new Settlement (Enums.Color.WHITE));
		}
		for (int i = 0; i < 20; i++) {
			pieces.Add (new City (Enums.Color.WHITE, false));
		}
		for (int i = 0; i < 20; i++) {
			pieces.Add (new Knight (Enums.Color.WHITE));
		}

		fullDev = new Dictionary<Enums.DevChartType, int> ();
		medDev = new Dictionary<Enums.DevChartType, int> ();
		emptyDev = new Dictionary<Enums.DevChartType, int> ();
		for (int i = 0; i < 3; i++) {
			fullDev.Add ((Enums.DevChartType)i, 5);
		}
		for (int i = 0; i < 3; i++) {
			medDev.Add ((Enums.DevChartType)i, 3);
		}
		for (int i = 0; i < 3; i++) {
			emptyDev.Add ((Enums.DevChartType)i, 1);
		}

		// Construct the board with the board pieces
		for (int i = 0; i < 16; i++) {
			vertices.Add (new Vertex (Enums.TerrainType.LAND));
		}
		for (int i = 0; i < 10; i++) {
			vertices.Add (new Vertex (Enums.TerrainType.WATER));
		}
		for (int i = 0; i < 14; i++) {
			vertices.Add (new Vertex (Enums.TerrainType.LAND));
		}
		for (int i = 0; i < 16; i++) {
			edges.Add (new Edge (Enums.TerrainType.LAND));
		}
		for (int i = 0; i < 10; i++) {
			edges.Add (new Edge (Enums.TerrainType.WATER));
		}
		for (int i = 0; i < 16; i++) {
			edges.Add (new Edge (Enums.TerrainType.LAND));
		}
		for (int i = 0; i < 7; i++) {
			edges.Add (new Edge (Enums.TerrainType.WATER));
		}
		for (int i = 0; i < 3; i++) {
			edges.Add (new Edge (Enums.TerrainType.LAND));
		}
		for (int i = 0; i < 4; i++) {
			hexes.Add (new Hex (Enums.TerrainType.LAND, Enums.HexType.FIELD));
		}
		for (int i = 0; i < 6; i++) {
			hexes.Add (new Hex (Enums.TerrainType.WATER, Enums.HexType.WATER));
		}
		for (int i = 0; i < 3; i++) {
			hexes.Add (new Hex (Enums.TerrainType.LAND, Enums.HexType.GOLD));
		}

		// Connect the board pieces
		stringEdges (0, 15);
		stringEdges (16, 23);
		stringEdges (25, 39);

		combineEdgeVertices (15, 15, 10);
		combineEdgeVertices (23, 23, 0);
		combineEdgeVertices (24, 24, 35);
		combineEdgeVertices (39, 39, 26);
		combineEdgeVertices (40, 38, 29);
		combineEdgeVertices (41, 37, 32);
		combineEdgeVertices (42, 36, 19);
		combineEdgeVertices (43, 24, 17);
		combineEdgeVertices (44, 20, 39);
		combineEdgeVertices (45, 22, 25);
		combineEdgeVertices (46, 15, 18);
		combineEdgeVertices (47, 14, 21);
		combineEdgeVertices (48, 9, 16);
		combineEdgeVertices (49, 0, 13);
		combineEdgeVertices (50, 6, 11);
		combineEdgeVertices (51, 3, 12);

		hexJoin(0, new int[] {0, 1, 2, 3, 12, 13}, new int[] {0, 1, 2, 51, 12, 49});
		hexJoin(1, new int[] {3, 4, 5, 6, 11, 12}, new int[] {3, 4, 5, 50, 11, 51});
		hexJoin(2, new int[] {6, 7, 8, 9, 10, 11}, new int[] {6, 7, 8, 9, 10, 50});
		hexJoin(3, new int[] {10, 11, 12, 13, 14, 15}, new int[] {10, 11, 12, 13, 14, 15});
		hexJoin(4, new int[] {9, 10, 15, 18, 17, 16}, new int[] {9, 15, 46, 17, 16, 48});
		hexJoin(5, new int[] {15, 14, 21, 20, 19, 18}, new int[] {46, 14, 47, 20, 19, 18});
		hexJoin(6, new int[] {0, 23, 22, 21, 14, 13}, new int[] {49, 23, 22, 21, 47, 13});
		hexJoin(7, new int[] {17, 18, 19, 36, 35, 24}, new int[] {17, 18, 42, 35, 24, 43});
		hexJoin(8, new int[] {20, 21, 22, 25, 26, 39}, new int[] {20, 21, 45, 25, 39, 44});
		hexJoin(9, new int[] {19, 20, 39, 38, 37, 36}, new int[] {19, 44, 38, 37, 36, 42});
		hexJoin(10, new int[] {26, 27, 28, 29, 38, 39}, new int[] {26, 27, 28, 40, 38, 39});
		hexJoin(11, new int[] {29, 30, 31, 32, 37, 38}, new int[] {29, 30, 31, 41, 37, 40});
		hexJoin(12, new int[] {32, 33, 34, 35, 36, 37}, new int[] {32, 33, 34, 35, 36, 41});

		// Add the roads and ships to the sample board
		edges[1].setOccupyingPiece(pieces[1]);
		edges[2].setOccupyingPiece(pieces[2]);
		edges[3].setOccupyingPiece(pieces[3]);
		edges[5].setOccupyingPiece(pieces[4]);
		edges[50].setOccupyingPiece(pieces[5]);
		edges[10].setOccupyingPiece(pieces[6]);
		edges[51].setOccupyingPiece(pieces[7]);
		edges[9].setOccupyingPiece(pieces[8]);
		edges[15].setOccupyingPiece(pieces[9]);
		edges[14].setOccupyingPiece(pieces[10]);
		edges[47].setOccupyingPiece(pieces[21]);
		edges[21].setOccupyingPiece(pieces[22]);
		edges[20].setOccupyingPiece(pieces[23]);
		edges[44].setOccupyingPiece(pieces[24]);
		edges[38].setOccupyingPiece(pieces[25]);
		edges[37].setOccupyingPiece(pieces[11]);
		edges[36].setOccupyingPiece(pieces[26]);
		edges[31].setOccupyingPiece(pieces[12]);
		edges[32].setOccupyingPiece(pieces[13]);
		edges[33].setOccupyingPiece(pieces[14]);
		edges[34].setOccupyingPiece(pieces[15]);
		edges[35].setOccupyingPiece(pieces[27]);
		edges [46].setOccupyingPiece (pieces [28]);
		edges [18].setOccupyingPiece (pieces [29]);
		edges [42].setOccupyingPiece (pieces [30]);

		for (int i = 1; i <= 14; i++) {
			pieces [i].putOnBoard ();
		}
		for (int i = 21; i <= 30; i++) {
			pieces [i].putOnBoard ();
			((Road)pieces [i]).notBuiltThisTurn ();
		}

		// Add the settlements to the board
		vertices [1].setOccupyingPiece (pieces[41]);
		vertices [14].setOccupyingPiece (pieces[42]);
		vertices [31].setOccupyingPiece (pieces[43]);

		for (int i = 41; i <= 43; i++) {
			pieces [i].putOnBoard ();
		}

		// Add the cities to the board
		vertices [37].setOccupyingPiece (pieces[62]);
		vertices [37].addWall ();
		pieces [62].putOnBoard ();

		// Add the knights to the board
		vertices[11].setOccupyingPiece(pieces[81]);
		vertices[33].setOccupyingPiece(pieces[82]);
		vertices[38].setOccupyingPiece(pieces[83]);
		((Knight)vertices [33].getOccupyingPiece ()).activateKnight ();
		((Knight)vertices [33].getOccupyingPiece ()).notActivatedThisTurn() ;
		((Knight)vertices [33].getOccupyingPiece ()).upgrade() ;
		((Knight)vertices [11].getOccupyingPiece ()).activateKnight ();
		((Knight)vertices [11].getOccupyingPiece ()).notActivatedThisTurn() ;
		((Knight)vertices [11].getOccupyingPiece ()).upgrade() ;

		for (int i = 81; i <= 83; i++) {
			pieces [i].putOnBoard ();
		}

		// Add the enemy pieces to the board
		edges[6].setOccupyingPiece(new Road(Enums.Color.RED, false));
		edges[7].setOccupyingPiece(new Road(Enums.Color.RED, false));
		edges[8].setOccupyingPiece(new Road(Enums.Color.RED, true));
		edges[48].setOccupyingPiece(new Road(Enums.Color.RED, true));
		edges[16].setOccupyingPiece(new Road(Enums.Color.RED, true));
		edges[39].setOccupyingPiece(new Road(Enums.Color.RED, false));
		edges[26].setOccupyingPiece(new Road(Enums.Color.RED, false));

		vertices [27].setOccupyingPiece (new Settlement (Enums.Color.RED));
		vertices [8].setOccupyingPiece (new Settlement (Enums.Color.RED));

		vertices [6].setOccupyingPiece (new City(Enums.Color.RED, false));

		vertices[39].setOccupyingPiece(new Knight(Enums.Color.RED));

		// Add the robber and pirate
		hexes [0].setOccupyingPiece (new Robber (Enums.Color.NONE));
		hexes [6].setOccupyingPiece (new Pirate (Enums.Color.NONE));

		// Print the available actions
		printMoveAuthorizer ();
	}

	// Print the longest route for every edge
	private void printLongestRoute() {
		List<Edge> edgeList = new List<Edge> ();

		Debug.Log ("Longest Route:\n");
		for (int i = 0; i < edgeCount; i++) {
			Graph.edgeReset (edges [0]);
			edgeList = Graph.collectRouteEdges (edges [i], Enums.Color.WHITE);
			int route = Graph.longestRoute (edgeList, Enums.Color.WHITE);
			Debug.Log ("Edge " + i + " has longest route " + route +"\n");
		}
		Debug.Log("\n");
	}

	// Print edges connected to every edge
	private void printConnectedEdges() {

		List<Edge> edgeList = new List<Edge> ();

		// Print all edges that are connected to every given edge
		Debug.Log ("Connected Edges:\n");
		for (int i = 0; i < edgeCount; i++) {
			Debug.Log ("Edges connected to edge " + i + ":\n");
			Graph.edgeReset (edges [0]);
			edgeList = Graph.collectRouteEdges (edges [i], Enums.Color.WHITE);
			for (int j = 0; j < edgeCount; j++) {
				foreach (Edge e in edgeList) {
					if (Object.ReferenceEquals (e, edges [j])) {
						Debug.Log ("Edge " + i + " is connected to edge " + j + "\n");
					}
				}
			}
			Debug.Log("\n");
		}
		Debug.Log("\n");
	}

	// Prints all available actions in move authorizer
	private void printMoveAuthorizer() {

		// Check all vertices that a knight can move to
		Debug.Log ("canKnightMove:\n");
		for (int i = 0; i < vertexCount; i++) {
			for (int j = 0; j < vertexCount; j++) {
				if (MoveAuthorizer.canKnightMove(vertices[i], vertices[j], Enums.Color.WHITE)) {
					Debug.Log("Knight can move from vertex " + i + " to vertex " + j + "\n");
				}
			}
		}
		Debug.Log("\n");

		// Check all vertices that a knight can displace another knight
		Debug.Log ("canKnightDisplace:\n");
		for (int i = 0; i < vertexCount; i++) {
			for (int j = 0; j < vertexCount; j++) {
				if (MoveAuthorizer.canKnightDisplace(vertices[i], vertices[j], Enums.Color.WHITE)) {
					Debug.Log("Knight can displace knight at vertex " + j + " from vertex " + i + "\n");
				}
			}
		}
		Debug.Log("\n");

		// Check all the knights that can be upgrade
		Debug.Log ("canUpgradeKnight:\n");
		for (int i = 0; i < vertexCount; i++) {
			if (MoveAuthorizer.canUpgradeKnight(fullRes, fullDev, vertices[i])) {
				Debug.Log("Knight at vertex " + i + " can be upgraded\n");
			}
		}
		Debug.Log("\n");

		// Check all the knights can be activated
		Debug.Log ("canActivateKnight:\n");
		for (int i = 0; i < vertexCount; i++) {
			if (MoveAuthorizer.canActivateKnight(fullRes, vertices[i])) {
				Debug.Log("Knight at vertex " + i + " can be activated\n");
			}
		}
		Debug.Log("\n");

		// Check if the development chart can be upgraded
		Debug.Log ("canUpgradeDevChart:\n");
		if (MoveAuthorizer.canUpgradeDevChart(Enums.DevChartType.POLITICS, fullCom, pieces, medDev)) {
			Debug.Log("Development chart can be upgraded in politics\n");
		}
		if (MoveAuthorizer.canUpgradeDevChart(Enums.DevChartType.TRADE, fullCom, pieces, medDev)) {
			Debug.Log("Development chart can be upgraded in trade\n");
		}
		if (MoveAuthorizer.canUpgradeDevChart(Enums.DevChartType.SCIENCE, fullCom, pieces, medDev)) {
			Debug.Log("Development chart can be upgraded in science\n");
		}
		Debug.Log("\n");

		// Check where settlements can be built
		Debug.Log ("canBuildSettlement:\n");
		for (int i = 0; i < vertexCount; i++) {
			if (MoveAuthorizer.canBuildSettlement(vertices[i], fullRes, pieces, Enums.Color.WHITE)) {
				Debug.Log("Can build a settlement at vertex " + i + "\n");
			}
		}
		Debug.Log("\n");

		// Check where cities can be built
		Debug.Log ("canBuildCity:\n");
		for (int i = 0; i < vertexCount; i++) {
			if (MoveAuthorizer.canBuildCity(vertices[i], fullRes, pieces, Enums.Color.WHITE)) {
				Debug.Log("Can build a city at vertex " + i + "\n");
			}
		}
		Debug.Log("\n");

		// Check where city walls can be built
		Debug.Log ("canBuildCityWall:\n");
		for (int i = 0; i < vertexCount; i++) {
			if (MoveAuthorizer.canBuildCityWall(vertices[i], fullRes, 2, Enums.Color.WHITE)) {
				Debug.Log("Can build a city wall at vertex " + i + "\n");
			}
		}
		Debug.Log("\n");

		// Check where knights can be built
		Debug.Log ("canBuildKnight:\n");
		for (int i = 0; i < vertexCount; i++) {
			if (MoveAuthorizer.canBuildKnight(vertices[i], fullRes, pieces, Enums.Color.WHITE)) {
				Debug.Log("Can build a knight at vertex " + i + "\n");
			}
		}
		Debug.Log("\n");

		// Check where roads can be built
		Debug.Log ("canBuildRoad:\n");
		for (int i = 0; i < edgeCount; i++) {
			if (MoveAuthorizer.canBuildRoad(edges[i], fullRes, pieces, Enums.Color.WHITE)) {
				Debug.Log("Can build a road at vertex " + i + "\n");
			}
		}
		Debug.Log("\n");

		// Check where ships can be built
		Debug.Log ("canBuildShip:\n");
		for (int i = 0; i < edgeCount; i++) {
			if (MoveAuthorizer.canBuildShip(edges[i], fullRes, pieces, Enums.Color.WHITE)) {
				Debug.Log("Can build a ship at vertex " + i + "\n");
			}
		}
		Debug.Log("\n");

		// Check where ships can be moved to
		Debug.Log ("canShipMove:\n");
		for (int i = 0; i < edgeCount; i++) {
			for (int j = 0; j < edgeCount; j++) {
				if (MoveAuthorizer.canShipMove(edges[i], edges[j], Enums.Color.WHITE)) {
					Debug.Log("Ship can move from edge " + i + " to edge " + j + "\n");
				}
			}
		}
		Debug.Log("\n");

		// Check if a knight can chase the robber way
		Debug.Log ("canChaseRobber:\n");
		for (int i = 0; i < vertexCount; i++) {
			if (MoveAuthorizer.canChaseRobber(vertices[i])) {
				Debug.Log("Can chase robber from vertex " + i + "\n");
			}
		}
		Debug.Log("\n");

		// Check where the robber can be moved to
		Debug.Log ("canMoveRobber:\n");
		for (int i = 0; i < hexCount; i++) {
			if (MoveAuthorizer.canMoveRobber(hexes[i])) {
				Debug.Log("Can move robber to hex " + i + "\n");
			}
		}
		Debug.Log("\n");

		// Check where the pirate can be moved to
		Debug.Log ("canMovePirate:\n");
		for (int i = 0; i < hexCount; i++) {
			if (MoveAuthorizer.canMovePirate(hexes[i])) {
				Debug.Log("Can move pirate to hex " + i + "\n");
			}
		}
		Debug.Log("\n");

		// Check where the merchant can be placed
		Debug.Log ("canPlaceMerchant:\n");
		for (int i = 0; i < hexCount; i++) {
			if (MoveAuthorizer.canPlaceMerchant(hexes[i])) {
				Debug.Log("Merchant can be placed on hex " + i + "\n");
			}
		}
		Debug.Log("\n");

		/*
		// Check where the initial town-piece can be placed
		Debug.Log ("canPlaceInitialTownPiece:\n");
		for (int i = 0; i < vertexCount; i++) {
			if (MoveAuthorizer.canPlaceInitialTownPiece(vertices[i])) {
				Debug.Log("Initial town piece can be placed on vertex " + i + "\n");
			}
		}
		Debug.Log("\n");
		*/

		// Check where the initial road can be placed
		Debug.Log ("canPlaceInitialRoad:\n");
		for (int i = 0; i < edgeCount; i++) {
			if (MoveAuthorizer.canPlaceInitialRoad(edges[i], Enums.Color.WHITE)) {
				Debug.Log("Initial road can be placed on edge " + i + "\n");
			}
		}
		Debug.Log("\n");

		// Check where the initial ship can be placed
		Debug.Log ("canPlaceInitialShip:\n");
		for (int i = 0; i < edgeCount; i++) {
			if (MoveAuthorizer.canPlaceInitialShip(edges[i], Enums.Color.WHITE)) {
				Debug.Log("Initial ship can be placed on edge " + i + "\n");
			}
		}
		Debug.Log("\n");
	}

	// Join vertices and edges to a hex
	private void hexJoin (int h, int[] v, int[] e) {
		Hex hex = hexes [h];

		// Add the vertices to the hex
		for (int i = 0; i < 6; i++) {
			hex.addVertex (vertices [v [i]]);
		}

		// Add the edges to the hex
		Edge current;
		for (int i = 0; i < 6; i++) {
			current = edges[e [i]];
			if (Object.ReferenceEquals (current.getLeftHex (), null)) {
				current.setLeftHex (hex);
			} else if (Object.ReferenceEquals (current.getRightHex (), null)) {
				current.setRightHex (hex);
			}
		}
	}

	// String edges to corresponding vertices from int start to int end
	private void stringEdges(int start, int end) {
		for (int i = start; i < end; i++) {
			Edge current = edges [i];
			Vertex left = vertices [i];
			Vertex right = vertices [i + 1];

			current.setLeftVertex (left);
			current.setRightVertex (right);

			left.addEdge (current);
			right.addEdge (current);
		}
	}

	// Add vertices to an edge
	private void combineEdgeVertices(int edge, int v1, int v2) {
		Edge current = edges [edge];
		Vertex left = vertices [v1];
		Vertex right = vertices [v2];

		current.setLeftVertex (left);
		current.setRightVertex (right);

		left.addEdge (current);
		right.addEdge (current);
	}

	// Print the board out
	public void prettyPrintMap() {

		// Print out the vertices and their neighbouring edges
		Debug.Log ("VERTICES:\n");
		for (int i = 0; i < vertexCount; i++) {
			Vertex current = vertices [i];
			Debug.Log ("Vertex " + i + " Neighbouring Edges: ");
			foreach (Edge e in current.getNeighbouringEdges()) {
				for (int j = 0; j < edgeCount; j++) {
					if (Object.ReferenceEquals(e, edges[j])) {
						Debug.Log ("" + j + " ");
					}
				}
			}
			Debug.Log ("\n");
		}

		// Print out edges and their connected vertices/hexes
		Debug.Log ("EDGES:\n");
		for (int i = 0; i < edgeCount; i++) {
			Edge current = edges [i];
			Debug.Log ("Edge " + i + " Connected Vertices: ");

			Vertex v = current.getLeftVertex ();
			for (int j = 0; j < vertexCount; j++) {
				if (Object.ReferenceEquals(v, vertices[j])) {
					Debug.Log ("" + j + " ");
				}
			}
			v = current.getRightVertex ();
			for (int j = 0; j < vertexCount; j++) {
				if (Object.ReferenceEquals(v, vertices[j])) {
					Debug.Log ("" + j + " ");
				}
			}
			Debug.Log ("\n");
	
			Debug.Log ("Edge " + i + " Connected Hexes: ");
			Hex h = current.getLeftHex ();
			for (int j = 0; j < hexCount; j++) {
				if (Object.ReferenceEquals(h, hexes[j])) {
					Debug.Log ("" + j + " ");
				}
			}
			h = current.getRightHex ();
			for (int j = 0; j < hexCount; j++) {
				if (Object.ReferenceEquals(h, hexes[j])) {
					Debug.Log ("" + j + " ");
				}
			}
			Debug.Log ("\n");
		}

		// Print out hexes and thier neighbouring vertices/edges
		Debug.Log ("HEXES:\n");
		for (int i = 0; i < hexCount; i++) {
			Hex current = hexes [i];
			Debug.Log ("Hex " + i + " Neighbouring Vertices: ");
			foreach (Vertex v in current.getVertices()) {
				for (int j = 0; j < vertexCount; j++) {
					if (Object.ReferenceEquals(v, vertices[j])) {
						Debug.Log ("" + j + " ");
					}
				}
			}
			Debug.Log ("\n");

			Debug.Log ("Hex " + i + " Neighbouring Edges: ");
			for (int j = 0; j < edgeCount; j++) {
				if (Object.ReferenceEquals(current, edges[j].getLeftHex())) {
					Debug.Log ("" + j + " ");
				} else if (Object.ReferenceEquals(current, edges[j].getRightHex())) {
					Debug.Log ("" + j + " ");
				}
			}
			Debug.Log ("\n");
		}
	}
		
	// Call test on start
	void Start () {
		testExample ();
	}
	
	// Call test when t is pressed
	void Update () {
		if(Input.GetKeyDown(KeyCode.T)){ 
			testExample ();
		}
	}
}
