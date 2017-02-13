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

	// Check if there is an illegal ship-road bridge
	private static Vertex roadToShip(Vertex opposite, Edge edge, Enums.Color color) {
		GamePiece intersection = opposite.getOccupyingPiece ();
		GamePiece edgePiece = edge.getOccupyingPiece ();

		// If there is a town-piece, there is no illegal bridge
		if (!Object.ReferenceEquals (intersection, null)) {
			if (intersection.getPieceType () == Enums.PieceType.CITY ||
			    intersection.getPieceType () == Enums.PieceType.SETTLEMENT) {

				return opposite;
			}
		} else {
			Road currentRoad = (Road)edgePiece;
			bool isShip = currentRoad.getIsShip ();

			int total = 0;
			int same = 0;

			// Check the total number of edge pieces you control around opposite
			// As well as all edge pieces that share a type with the edgePiece
			foreach (Edge away in opposite.getNeighbouringEdges()) {
				GamePiece currPiece = away.getOccupyingPiece ();

				if (Object.ReferenceEquals (away, edge)) {
					continue;
				}
				if (Object.ReferenceEquals (currPiece, null)) {
					continue;
				} else if (currPiece.getColor () != color) {
					continue;
				} else if (currPiece.getPieceType () != Enums.PieceType.ROAD) {
					continue;
				}

				total++;
				Road currRoad = (Road)currPiece;
				if (currRoad.getIsShip () == isShip) {
					same++;
				}
			}

			// If no types are shared, there is no connection
			if (same == 0) {
				opposite.setVisited ();

			// If only 1 of 2 types are shared, make sure to continue checking the correct path
			} else if (total == 2 && same == 1) {
				opposite.setVisited ();
				foreach (Edge away in opposite.getNeighbouringEdges()) {
					GamePiece currPiece = away.getOccupyingPiece ();

					if (Object.ReferenceEquals (away, edge)) {
						continue;
					}
					if (Object.ReferenceEquals (edgePiece, null)) {
						continue;
					} else if (edgePiece.getColor () != color) {
						continue;
					} else if (edgePiece.getPieceType () != Enums.PieceType.ROAD) {
						continue;
					}

					Road currRoad = (Road)currPiece;
					if (currRoad.getIsShip () != isShip) {
						if (Object.ReferenceEquals (away.getLeftVertex (), opposite)) {
							return away.getRightVertex ();
						} else {
							return away.getLeftVertex ();
						}
					}
				}
			}
		}
			
		// Return the new point for path checking
		return opposite;
	}

	// Check if a vertex has a piece that breaks a route
	public static bool checkRouteBreak (Vertex v) {
		GamePiece interPiece = v.getOccupyingPiece ();

		// First check if there is a piece on vertex v
		if (Object.ReferenceEquals (interPiece, null)) {
			return false;
		}
			
		// Create a color dictionary
		Dictionary<Enums.Color, int> colors = new Dictionary<Enums.Color, int>();
		Enums.Color interColor = interPiece.getColor ();
		for (int i = 0; i < 5; i++) {
			colors.Add ((Enums.Color)i, 0);
		}

		// Get a count of colored edges around vertex v
		// If two edges don't match the piece at vertex v, there is a break
		foreach (Edge e in v.getNeighbouringEdges()) {
			GamePiece edgePiece = e.getOccupyingPiece ();

			if (Object.ReferenceEquals (edgePiece, null)) {
				continue;
			} else {
				int currentColor = colors [edgePiece.getColor ()];
				currentColor++;

				if (edgePiece.getColor () != interColor) {
					if (currentColor >= 2) {
						return true;
					}
				}
				colors.Remove (edgePiece.getColor ());
				colors.Add (edgePiece.getColor (), currentColor);
			}
		}
		return false;
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

	// Reset all the vertices
	public static void edgeReset(Edge e) {
		edgeClear (e);
		edgeUnvisit (e);
	}

	// Collect all edges connected to an edge
	// Make sure to reset all edges before using this method
	public static List<Edge> collectRoutEdges(Edge edge) {
		List<Edge> ret = new List<Edge> ();



		return ret;
	}
}