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

        Road ship = (Road)sourcePiece;
        if (!ship.getIsShip())
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
        if (graph.isClosedShip(source, color))
        {
            return false;
        }
        if (ship.getBuiltThisTurn())
        {
            return false;
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
                    return false;
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
                    return false;
                }
            }
        }

        // Make sure the target edge is by water and not next to the pirate
        bool nextToWater = false;
        checkHex = target.getLeftHex();
        if (!Object.ReferenceEquals(checkHex, null))
        {
            hexPiece = checkHex.getOccupyingPiece();
            if (!Object.ReferenceEquals(hexPiece, null))
            {
                if (hexPiece.getPieceType() == Enums.PieceType.PIRATE)
                {
                    return false;
                }
            }
            if (checkHex.getTerrainType() == Enums.TerrainType.WATER)
            {
                nextToWater = true;
            }
        }
        checkHex = target.getRightHex();
        if (!Object.ReferenceEquals(checkHex, null))
        {
            hexPiece = checkHex.getOccupyingPiece();
            if (!Object.ReferenceEquals(hexPiece, null))
            {
                if (hexPiece.getPieceType() == Enums.PieceType.PIRATE)
                {
                    return false;
                }
            }
            if (checkHex.getTerrainType() == Enums.TerrainType.WATER)
            {
                nextToWater = true;
            }
        }
        if (target.getTerrainType() == Enums.TerrainType.WATER) {
            nextToWater = true;
        }
        if (!nextToWater)
        {
            return false;
        }

        // Make sure the target edge is next to a town-piece or ship (not the one being moved)
        bool nextToTown = graph.nextToMyCityOrSettlement(target, color);
        bool nextToShip = false;
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
            if (Object.ReferenceEquals(touchingRoad, ship))
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

            Road touchingShip = (Road)touchingRoad;
            if (!touchingShip.getIsShip())
            {
                continue;
            }
            nextToShip = true;
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
            if (Object.ReferenceEquals(touchingRoad, ship))
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

            Road touchingShip = (Road)touchingRoad;
            if (!touchingShip.getIsShip())
            {
                continue;
            }
            nextToShip = true;
        }
        if (!nextToTown && !nextToShip)
        {
            return false;
        }
        return true;
    }

}
