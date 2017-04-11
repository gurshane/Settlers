using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class ProgressAuthroizor {

	private Graph graph = new Graph();

	// Check if a ship can be moved to an edge
    public bool canRoadMove(Edge source, Edge target, Enums.Color color)
    {
        GamePiece sourcePiece = source.getOccupyingPiece();

        // Make sure there is a ship on the source edge
        if (Object.ReferenceEquals(sourcePiece, null))
        {
            return false;
        }
        if (sourcePiece.getPieceType() != Enums.PieceType.ROAD)
        {
            return false;
        }
        if (sourcePiece.getColor() != color)
        {
            return false;
        }

        Road road = (Road)sourcePiece;
        if (road.getIsShip())
        {
            return false;
        }

        // Make sure the target edge is empty
        if (!Object.ReferenceEquals(target.getOccupyingPiece(), null))
        {
            return false;
        }

        // Make sure the source ship is valid for moving
        if (graph.nextToMyPiece(source, color))
        {
            return false;
        }
        if (graph.isClosedRoad(source, color))
        {
            return false;
        }

        if (target.getTerrainType() != Enums.TerrainType.LAND) {
            return false;
        }

        // Make sure the target edge is next to a town-piece or ship (not the one being moved)
        bool nextToTown = graph.nextToMyCityOrSettlement(target, color);
        bool nextToRoad = false;
        Vertex current = target.getLeftVertex();
        foreach (Edge e in current.getNeighbouringEdges())
        {
            if (Object.ReferenceEquals(e, target))
            {
                continue;
            }

            GamePiece touchingRoad = e.getOccupyingPiece();
            if (Object.ReferenceEquals(touchingRoad, null))
            {
                continue;
            }
            if (Object.ReferenceEquals(touchingRoad, road))
            {
                continue;
            }
            if (touchingRoad.getPieceType() != Enums.PieceType.ROAD)
            {
                continue;
            }
            if (touchingRoad.getColor() != color)
            {
                continue;
            }

            Road secondRoad = (Road)touchingRoad;
            if (secondRoad.getIsShip())
            {
                continue;
            }
            nextToRoad = true;
        }
        current = target.getRightVertex();
        foreach (Edge e in current.getNeighbouringEdges())
        {
            if (Object.ReferenceEquals(e, target))
            {
                continue;
            }

            GamePiece touchingRoad = e.getOccupyingPiece();
            if (Object.ReferenceEquals(touchingRoad, null))
            {
                continue;
            }
            if (Object.ReferenceEquals(touchingRoad, road))
            {
                continue;
            }
            if (touchingRoad.getPieceType() != Enums.PieceType.ROAD)
            {
                continue;
            }
            if (touchingRoad.getColor() != color)
            {
                continue;
            }

            Road secondRoad = (Road)touchingRoad;
            if (secondRoad.getIsShip())
            {
                continue;
            }
            nextToRoad = true;
        }
        if (!nextToTown && !nextToRoad)
        {
            return false;
        }
        return true;
    }

	 // Check if a knight can displace another knight
    public bool canIntrigueKnight (Vertex target, Enums.Color color)
    {

        // Make sure there is a lower-level knight at the target vertex
        GamePiece targetPiece = target.getOccupyingPiece();
        if (!Object.ReferenceEquals(targetPiece, null))
        {
            if (targetPiece.getColor() == color)
            {
                return false;
            }
            if (targetPiece.getPieceType() != Enums.PieceType.KNIGHT)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        return graph.nextToMyEdge(target, color);
    }
}
