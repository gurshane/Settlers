using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Some unused scripts that may come in handy
public class Extra : MonoBehaviour {

	/* Check if there is an illegal ship-road bridge
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

	// Get all edges that are interesting in a list of edges
	private static List<Edge> getInterestingEdges(List<Edge> edges, Enums.Color color) {
		List<Edge> ret = new List<Edge> ();

		foreach (Edge e in edges) {
			Vertex left = e.getLeftVertex ();
			Vertex right = e.getRightVertex ();

			GamePiece leftPiece = left.getOccupyingPiece ();
			GamePiece rightPiece = right.getOccupyingPiece ();

			if (!Object.ReferenceEquals(leftPiece, null)) {
				if (leftPiece.getColor() != color) {
					ret.Add(e);
					continue;
				}
			}
			if (!Object.ReferenceEquals(rightPiece, null)) {
				if (rightPiece.getColor() != color) {
					ret.Add(e);
					continue;
				}
			}

			int num = 0;
			foreach (Edge neighbour in left.getNeighbouringEdges()) {
				foreach (Edge test in edges) {
					if (Object.ReferenceEquals (test, neighbour)) {
						num++;
					}
				}
			}
			if (num == 2) {
				continue;
			}

			num = 0;
			foreach (Edge neighbour in right.getNeighbouringEdges()) {
				foreach (Edge test in edges) {
					if (Object.ReferenceEquals (test, neighbour)) {
						num++;
					}
				}
			}
			if (num == 2) {
				continue;
			}
			ret.Add(e);
		}
		return ret;
	}

	// Determine if a collection of edges is a cycle
	private static bool isCyclicRoute(List<Edge> edges, Enums.Color color) {
		foreach (Edge e in edges) {
			Vertex left = e.getLeftVertex ();
			Vertex right = e.getRightVertex ();

			GamePiece leftPiece = left.getOccupyingPiece ();
			GamePiece rightPiece = right.getOccupyingPiece ();

			// If any enemy pieces are on a vertex, there is no cycle
			if (!Object.ReferenceEquals(leftPiece, null)) {
				if (leftPiece.getColor() != color) {
					return false;
				}
			}
			if (!Object.ReferenceEquals(rightPiece, null)) {
				if (rightPiece.getColor() != color) {
					return false;
				}
			}

			// Check to make sure there are exactly 2 edges at every intersection
			int num = 0;
			foreach (Edge neighbour in left.getNeighbouringEdges()) {
				foreach (Edge test in edges) {
					if (Object.ReferenceEquals (test, neighbour)) {
						num++;
					}
				}
			}
			if (num == 2) {
				continue;
			}

			num = 0;
			foreach (Edge neighbour in right.getNeighbouringEdges()) {
				foreach (Edge test in edges) {
					if (Object.ReferenceEquals (test, neighbour)) {
						num++;
					}
				}
			}
			if (num == 2) {
				continue;
			}
			return false;
		}
		return true;
	}*/

	// Get all edges that have been broken by the placement of a vertex piece
	/*public static List<Edge> getBrokenEdges(Vertex v) {
		List<Edge> ret = new List<Edge> ();
		GamePiece interPiece = v.getOccupyingPiece ();

		// Check if there is a piece on the given vertex
		if (Object.ReferenceEquals (interPiece, null)) {
			return ret;
		}

		// Get all the edge pieces that have a different color than the vertex piece
		Enums.Color interColor = interPiece.getColor ();
		foreach (Edge e in v.getNeighbouringEdges()) {
			GamePiece edgePiece = e.getOccupyingPiece ();

			if (Object.ReferenceEquals (edgePiece, null)) {
				continue;
			} else if (edgePiece.getColor () != interColor) {
				ret.Add (e);
			}
		}
		return ret;
	}

	// Get the edges at an intersection that share the same color
	public static List<Edge> getFixedEdges(Vertex v) {
		List<Edge> ret = new List<Edge> ();
		Enums.Color color1 = Enums.Color.NONE;
		Enums.Color color2 = Enums.Color.NONE;
		Enums.Color chosen = Enums.Color.NONE;

		// Get the color of the fixed edges
		foreach (Edge e in v.getNeighbouringEdges()) {
			GamePiece edgePiece = e.getOccupyingPiece ();

			if (Object.ReferenceEquals (edgePiece, null)) {
				continue;
			} else {
				if (color1 == Enums.Color.NONE) {
					color1 = edgePiece.getColor ();
				} else if (edgePiece.getColor () == color1) {
					chosen = color1;
				} else if (color2 == Enums.Color.NONE) {
					color2 = edgePiece.getColor ();
				} else if (edgePiece.getColor () == color2) {
					chosen = color2;
				}
			}
		}

		// Add the edges of the chosen color to the return list
		foreach (Edge e in v.getNeighbouringEdges()) {
			GamePiece edgePiece = e.getOccupyingPiece ();

			if (Object.ReferenceEquals (edgePiece, null)) {
				continue;
			} else if (edgePiece.getColor () == chosen) {
				ret.Add (e);
			}
		}
		return ret;
	}*/

	// Check if a vertex has a piece that breaks a route
	/*public static bool checkRouteBreak (Vertex v) {
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
	}*/

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
