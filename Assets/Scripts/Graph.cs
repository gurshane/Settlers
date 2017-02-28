using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public static class Graph {

	// Set all vertice to the reset state
	private static void vertexClear(Vertex v) {
		v.clearVisited ();
		foreach (Edge e in v.getNeighbouringEdges()) {
			Vertex left = e.getLeftVertex ();
			Vertex right = e.getRightVertex ();

			if (left.getVisited () != 2) {
				vertexClear (left);
			}

			if (right.getVisited () != 2) {
				vertexClear (right);
			}
		}
	}

	// Set all vertices to the unvisited state
	private static void vertexUnvisit(Vertex v) {
		v.resetVisited ();
		foreach (Edge e in v.getNeighbouringEdges()) {
			Vertex left = e.getLeftVertex ();
			Vertex right = e.getRightVertex ();

			if (left.getVisited () != 0) {
				vertexUnvisit (left);
			}

			if (right.getVisited () != 0) {
				vertexUnvisit (right);
			}
		}
	}

	// Set all edges to the reset state
	private static void edgeClear(Edge edge) {
		edge.clearVisited ();
		Vertex current = edge.getLeftVertex ();
		foreach (Edge e in current.getNeighbouringEdges()) {
			if (e.getVisited () != 2) {
				edgeClear (e);
			}
		}

		current = edge.getRightVertex ();
		foreach (Edge e in current.getNeighbouringEdges()) {
			if (e.getVisited () != 2) {
				edgeClear (e);
			}
		}
	}

	// Set all vertices to the unvisited state
	private static void edgeUnvisit(Edge edge) {
		edge.resetVisited ();
		Vertex current = edge.getLeftVertex ();
		foreach (Edge e in current.getNeighbouringEdges()) {
			if (e.getVisited () != 0) {
				edgeUnvisit (e);
			}
		}

		current = edge.getRightVertex ();
		foreach (Edge e in current.getNeighbouringEdges()) {
			if (e.getVisited () != 0) {
				edgeUnvisit (e);
			}
		}
	}

	// Check to make sure there isn't an illegal ship-road bridge at an intersection
	private static bool shipToRoad(GamePiece a, GamePiece b, GamePiece intersection) {

		// Make sure the given edge pieces are actually edge pieces
		if (a.getPieceType () != Enums.PieceType.ROAD) {
			return false;
		}
		if (b.getPieceType () != Enums.PieceType.ROAD) {
			return false;
		}

		Road roadA = (Road)a;
		Road roadB = (Road)b;

		// Check to see if there is a town-piece at the intersection
		if (!Object.ReferenceEquals (intersection, null)) {
			if (intersection.getPieceType () == Enums.PieceType.CITY ||
				intersection.getPieceType () == Enums.PieceType.SETTLEMENT) {

				return false;
			}
		}

		// Make sure the two edge-pieces are of the same type
		if (roadA.getIsShip () == roadB.getIsShip ()) {
			return false;
		} else {
			return true;
		}
	}

	// A recursive method to find the longest route from a given start edge
	private static int recLongRoute (Edge start, Vertex vertex, List<Edge> edges, int current){

		List<Edge> toPass = new List<Edge> ();

		// All possible ways the route could extend from start
		int retA = current + 1;
		int retB = current + 1;

		// Pass a list of the remaining edges
		foreach (Edge e in edges) {
			if (Object.ReferenceEquals (e, start)) {
				continue;
			} else {
				toPass.Add (e);
			}
		}

		// Recursively check the edges coming from the given vertex
		int i = 0;
		foreach (Edge e in vertex.getNeighbouringEdges()) {
			if (Object.ReferenceEquals (e, start)) {
				continue;
			}
			i++;

			if (!containsEdge (e, toPass)) {
				continue;
			}

			Vertex next;
			if (Object.ReferenceEquals (e.getLeftVertex (), vertex)) {
				next = e.getRightVertex ();
			} else {
				next = e.getLeftVertex ();
			}

			if (i == 1) {
				retA = recLongRoute (e, next, toPass, retA);
			} else if (i == 2) {
				retB = recLongRoute (e, next, toPass, retB);
			}
		}

		// Return the longest of all the routes
		return Mathf.Max (retA, retB);
	}

	// Check if a list of edges contains a given edge
	private static bool containsEdge (Edge edge, List<Edge> edges) {
		foreach (Edge e in edges) {
			if (Object.ReferenceEquals(e, edge)) {
				return true;
			}
		}
		return false;
	}

	// Reset all the vertices
	public static void vertexReset(Vertex v) {
		vertexClear (v);
		vertexUnvisit (v);
	}

	// Check if two vertices are legally connected via edges
	// Note: returns true if there is an enemy knight on v2
	// (Helpful for displacing knights)
	// Note: returns false if there are any other pieces on v2
	// Make sure to reset vertices before using this method
	public static bool areConnectedVertices(Vertex v1, Vertex v2, Enums.Color color) {

		// If the vertices are the same, they are connected
		if (Object.ReferenceEquals(v1, v2)) {
			return true;
		}

		v1.setVisited();
		foreach (Edge e in v1.getNeighbouringEdges()) {

			GamePiece edgePiece = e.getOccupyingPiece (); 

			// Make sure the edge has a correctly colored piece
			if (Object.ReferenceEquals(edgePiece, null)) {
				continue;
			} else if (edgePiece.getColor () != color) {
				continue;
			} else if (edgePiece.getPieceType () != Enums.PieceType.ROAD) {
				continue;
			}

			// Check the opposite vertex along the edge
			Vertex opposite;
			if (Object.ReferenceEquals(e.getLeftVertex(), v1)) {
				opposite = e.getRightVertex ();
			} else {
				opposite = e.getLeftVertex ();
			}
				
			// If the opposite vertex has an enemy piece, further vertices are not connected
			GamePiece intersection = opposite.getOccupyingPiece ();
			if (!Object.ReferenceEquals(intersection, null)) {
				if (intersection.getColor () != color) {
					if (Object.ReferenceEquals (opposite, v2)) {
						if (intersection.getPieceType () != Enums.PieceType.KNIGHT) {
							opposite.setVisited ();
							continue;
						}
					} else {
						opposite.setVisited ();
						continue;
					}
				}
			}

			// If the opposite vertex is the target vertex, return true
			if (Object.ReferenceEquals(opposite, v2)) {
				return true;
			} 
				
			// Check the opposite vertex if it hasn't been visited
			if (opposite.getVisited () == 0) {
				if (areConnectedVertices (opposite, v2, color)) {
					return true;
				}
			}
		}
		return false;
	}

	// Check if a vertex is free (a town-piece can be placed on it)
	public static bool freeVertex(Vertex v) {

		// If there is a piece already on it, it is not free
		if (!Object.ReferenceEquals(v.getOccupyingPiece(), null)) {
			return false;
		}

		// If any neighbouring vertices have a piece, it is also not free
		foreach (Edge e in v.getNeighbouringEdges()) {
			Vertex opposite;

			if (Object.ReferenceEquals(e.getLeftVertex(), v)) {
				opposite = e.getRightVertex ();
			} else {
				opposite = e.getLeftVertex ();
			}

			GamePiece piece = opposite.getOccupyingPiece ();
			if (!Object.ReferenceEquals(piece, null)) {
				if (piece.getPieceType () != Enums.PieceType.KNIGHT) {
					return false;
				}
			}
		}
		return true;
	}
	// Check if a vertex is next to an edge of a color
	public static bool nextToMyEdge(Vertex v, Enums.Color color) {
		foreach (Edge e in v.getNeighbouringEdges()) {
			GamePiece p = e.getOccupyingPiece ();
			if (Object.ReferenceEquals(p, null)) {
				continue;
			}

			if (p.getColor() == color) {
				return true;
			}
		}
		return false;
	}

	// Check if a vertex is next to a road of a color
	public static bool nextToMyRoad(Vertex v, Enums.Color color) {
		foreach (Edge e in v.getNeighbouringEdges()) {
			GamePiece p = e.getOccupyingPiece ();
			if (Object.ReferenceEquals(p, null)) {
				continue;
			}

			if (p.getPieceType () != Enums.PieceType.ROAD) {
				continue;
			}

			Road road = (Road)p;
			if (road.getIsShip()) {
				continue;
			}

			if (p.getColor() == color) {
				return true;
			}
		}
		return false;
	}

	// Check if a vertex is next to a ship of a color
	public static bool nextToMyShip(Vertex v, Enums.Color color) {
		foreach (Edge e in v.getNeighbouringEdges()) {
			GamePiece p = e.getOccupyingPiece ();
			if (Object.ReferenceEquals(p, null)) {
				continue;
			}

			if (p.getPieceType () != Enums.PieceType.ROAD) {
				continue;
			}

			Road road = (Road)p;
			if (!road.getIsShip()) {
				continue;
			}

			if (p.getColor() == color) {
				return true;
			}
		}
		return false;
	}

	// Check if an edge is next to a town-piece of a color
	public static bool nextToMyCityOrSettlement(Edge e, Enums.Color color) {
		GamePiece leftPiece = e.getLeftVertex ().getOccupyingPiece ();
		GamePiece rightPiece = e.getRightVertex ().getOccupyingPiece ();

		// Check the left vertex of the edge
		if (!Object.ReferenceEquals(leftPiece, null)) {
			if (leftPiece.getColor () == color) {
				if (leftPiece.getPieceType () == Enums.PieceType.CITY ||
				    leftPiece.getPieceType () == Enums.PieceType.SETTLEMENT) {
					return true;
				}
			}
		}

		// Check the right vertex of the edge
		if (!Object.ReferenceEquals (rightPiece, null)) {
			if (rightPiece.getColor () == color) {
				if (rightPiece.getPieceType () == Enums.PieceType.CITY ||
				   rightPiece.getPieceType () == Enums.PieceType.SETTLEMENT) {
					return true;
				}
			}
		}
		return false;
	}

	// Check if an edge is next to a piece of a color
	public static bool nextToMyPiece(Edge e, Enums.Color color) {
		GamePiece leftPiece = e.getLeftVertex ().getOccupyingPiece ();
		GamePiece rightPiece = e.getRightVertex ().getOccupyingPiece ();

		// Check the left vertex of the edge
		if (!Object.ReferenceEquals(leftPiece, null)) {
			if (leftPiece.getColor () == color) {
				return true;
			}
		}

		// Check the right vertex of the edge
		if (!Object.ReferenceEquals (rightPiece, null)) {
			if (rightPiece.getColor () == color) {
				return true;
			}
		}
		return false;
	}

	// Check if a ship is closed
	public static bool isClosedShip(Edge edge, Enums.Color color) {
		Vertex left = edge.getLeftVertex ();
		Vertex right = edge.getRightVertex ();

		bool leftClosed = false;
		bool rightClosed = false;

		// Check if a ship is closed on its left side
		foreach (Edge e in left.getNeighbouringEdges()) {
			if (Object.ReferenceEquals(e, edge)) {
				continue;
			}

			GamePiece piece = e.getOccupyingPiece ();
			if (Object.ReferenceEquals(piece, null)) {
				continue;
			}
			if (piece.getColor () == color) {
				if (piece.getPieceType () == Enums.PieceType.ROAD) {
					Road ship = (Road)piece;
					if (ship.getIsShip ()) {
						leftClosed = true;
					}
				}
			}
		}

		// Check if a ship is closed on its right side
		foreach (Edge e in right.getNeighbouringEdges()) {
			if (Object.ReferenceEquals(e, edge)) {
				continue;
			}

			GamePiece piece = e.getOccupyingPiece ();
			if (Object.ReferenceEquals(piece, null)) {
				continue;
			}
			if (piece.getColor () == color) {
				if (piece.getPieceType () == Enums.PieceType.ROAD) {
					Road ship = (Road)piece;
					if (ship.getIsShip ()) {
						rightClosed = true;
					}
				}
			}
		}

		// If both sides are closed, the ship is closed
		if (leftClosed && rightClosed) {
			return true;
		}
		return false;
	}

	// Reset all the vertices
	public static void edgeReset(Edge e) {
		edgeClear (e);
		edgeUnvisit (e);
	}

	// Collect all edges connected to an edge
	// Make sure to reset all edges before using this method
	public static List<Edge> collectRouteEdges(Edge edge, Enums.Color color) {
		List<Edge> ret = new List<Edge> ();

		// Make sure the current edge has a valid edge piece
		GamePiece edgePiece = edge.getOccupyingPiece ();
		if (Object.ReferenceEquals (edgePiece, null)) {
			return ret;
		}
		if (edgePiece.getColor () != color) {
			return ret;
		}
		if (edge.getVisited () != 0) {
			return ret;
		}

		// Add the current edge to the list
		ret.Add (edge);
		edge.setVisited ();

		// Make sure the left and right vertices are valid in the route
		Vertex left = edge.getLeftVertex ();
		Vertex right = edge.getRightVertex ();
		GamePiece leftPiece = left.getOccupyingPiece ();
		GamePiece rightPiece = right.getOccupyingPiece ();

		// Recursively add edges on the left side of the given edge
		foreach (Edge e in left.getNeighbouringEdges()) {
			if (Object.ReferenceEquals (edge, e)) {
				continue;
			}
			if (e.getVisited () != 0) {
				continue;
			}
			if (!Object.ReferenceEquals (leftPiece, null)) {
				if (leftPiece.getColor () != color) {
					continue;
				}
			}

			GamePiece ePiece = e.getOccupyingPiece ();
			if (Object.ReferenceEquals (ePiece, null)) {
				continue;
			}
			if (ePiece.getColor () != color) {
				continue;
			}

			// Make sure there isn't an illegal ship-road bridge
			if (shipToRoad (edgePiece, ePiece, leftPiece)) {
				continue;
			}
			ret.AddRange (collectRouteEdges (e, color));
		}

		// Recursively add edges on the right side of the given edge
		foreach (Edge e in right.getNeighbouringEdges()) {
			if (Object.ReferenceEquals (edge, e)) {
				continue;
			}
			if (e.getVisited () != 0) {
				continue;
			}
			if (!Object.ReferenceEquals (rightPiece, null)) {
				if (rightPiece.getColor () != color) {
					continue;
				}
			}

			GamePiece ePiece = e.getOccupyingPiece ();
			if (Object.ReferenceEquals (ePiece, null)) {
				continue;
			}
			if (ePiece.getColor () != color) {
				continue;
			}

			// Make sure there isn't an illegal ship-road bridge
			if (shipToRoad (edgePiece, ePiece, rightPiece)) {
				continue;
			}
			ret.AddRange (collectRouteEdges (e, color));
		}
		return ret;
	}

	// An algorithm that determines the longest route of a collection of edges in a given color
	public static int longestRoute(List<Edge> edges, Enums.Color color) {

		// Check the base cases
		if (edges.Count == 0) {
			return 0;
		}
		if (edges.Count == 1) {
			return 1;
		}

		// Recursively check every edge to see if it is the start of the longest route
		int max = 0;
		foreach (Edge endpoint in edges) {
			Vertex left = endpoint.getLeftVertex ();
			Vertex right = endpoint.getRightVertex ();

			int current = recLongRoute (endpoint, left, edges, 0);
			if (current > max) {
				max = current;
			}
			current = recLongRoute (endpoint, right, edges, 0);
			if (current > max) {
				max = current;
			}
		}
		return max;
	}
}			