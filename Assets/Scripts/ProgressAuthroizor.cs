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

    public bool canDiplomatRemove(Edge target, Enums.Color color)
    {
        GamePiece targetPiece = target.getOccupyingPiece();

        // Make sure there is a ship on the source edge
        if (Object.ReferenceEquals(targetPiece, null))
        {
            return false;
        }
        if (targetPiece.getPieceType() != Enums.PieceType.ROAD)
        {
            return false;
        }
        if (targetPiece.getColor() == color)
        {
            return false;
        }

        Road road = (Road)targetPiece;
        if (road.getIsShip())
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

	public bool canCrane(Enums.DevChartType dev, int[] commodities,
        List<GamePiece> pieces, int[] devChart)
    {

        // Make sure there is a city on the board
        bool cityOnBoard = false;
        foreach (GamePiece p in pieces)
        {
            if (p.getPieceType() != Enums.PieceType.CITY)
            {
                continue;
            }
            else
            {
                if (p.isOnBoard())
                {
                    cityOnBoard = true;
                    break;
                }
            }
        }
        if (!cityOnBoard)
        {
            return false;
        }

        // Make sure there are enough resources for the upgrade
        // Or that the chart is not at maximum capacity
        int level, coms;
        if (dev == Enums.DevChartType.TRADE)
        {
            level = devChart[(int)Enums.DevChartType.TRADE];
            coms = commodities[(int)Enums.CommodityType.CLOTH];
            if (coms < level)
            {
                return false;
            }
            else if (level >= 5)
            {
                return false;
            }
        }
        else if (dev == Enums.DevChartType.POLITICS)
        {
            level = devChart[(int)Enums.DevChartType.POLITICS];
            coms = commodities[(int)Enums.CommodityType.COIN];
            if (coms < level)
            {
                return false;
            }
            else if (level >= 5)
            {
                return false;
            }
        }
        else if (dev == Enums.DevChartType.SCIENCE)
        {
            level = devChart[(int)Enums.DevChartType.SCIENCE];
            coms = commodities[(int)Enums.CommodityType.PAPER];
            if (coms < level)
            {
                return false;
            }
            else if (level >= 5)
            {
                return false;
            }
        }
        return true;
    }

	public bool canEngineer(Vertex location, int cityWalls, Enums.Color color)
    {

        GamePiece city = location.getOccupyingPiece();

        // Make sure the location is valid
        if (Object.ReferenceEquals(city, null))
        {
            return false;
        }
        if (city.getPieceType() != Enums.PieceType.CITY)
        {
            return false;
        }
        if (location.getHasWall())
        {
            return false;
        }
        if (city.getColor() != color)
        {
            return false;
        }
        if (cityWalls < 1)
        {
            return false;
        }

        return true;
    }

	public bool canInventor(Hex source, Hex target) {

		if (source.getTerrainType() == TerrainType.WATER) return false;
		if (target.getTerrainType() == TerrainType.WATER) return false;

		if (source.getHexType() == HexType.DESERT) return false;
		if (target.getHexType() == HexType.DESERT) return false;

		int i = source.getHexNumber();
		int j = target.getHexNumber();

		if (i == 2 || i == 12 || i == 6 || i == 8) return false;

		if (j == 2 || j == 12 || j == 6 || j == 8) return false;

		if (Object.ReferenceEquals(source, target)) return false;

        return true;
	}

	public bool canMedicine(Vertex location, int[] resources,
        List<GamePiece> pieces, Enums.Color color)
    {

        GamePiece settlement = location.getOccupyingPiece();

        // Make sure the location is valid
        if (Object.ReferenceEquals(settlement, null))
        {
            return false;
        }
        if (settlement.getPieceType() != Enums.PieceType.SETTLEMENT)
        {
            return false;
        }
        if (settlement.getColor() != color)
        {
            return false;
        }

        // Make sure there is an available city
        bool availablePiece = false;
        foreach (GamePiece p in pieces)
        {
            if (p.getPieceType() != Enums.PieceType.CITY)
            {
                continue;
            }
            if (!p.isOnBoard())
            {
                availablePiece = true;
            }
        }
        if (!availablePiece)
        {
            return false;
        }

        // Make sure there are enough resources
        if (resources[(int)Enums.ResourceType.GRAIN] < 1)
        {
            return false;
        }
        if (resources[(int)Enums.ResourceType.ORE] < 2)
        {
            return false;
        }
        return true;
    }

	public bool canRoadBuilding(Edge location, List<GamePiece> pieces, Enums.Color color)
    {

        // Make sure the location is valid
        if (!Object.ReferenceEquals(location.getOccupyingPiece(), null))
        {
            return false;
        }
        if (location.getTerrainType() == Enums.TerrainType.WATER)
        {
            return false;
        }

        bool nextToTown = graph.nextToMyCityOrSettlement(location, color);
        bool nextToRoad = false;
        if (graph.nextToMyRoad(location.getLeftVertex(), color))
        {
            nextToRoad = true;
        }
        else if (graph.nextToMyRoad(location.getRightVertex(), color))
        {
            nextToRoad = true;
        }
        if (!nextToTown && !nextToRoad)
        {
            return false;
        }

        // Make sure there is an available road
        bool availablePiece = false;
        foreach (GamePiece p in pieces)
        {
            if (p.getPieceType() != Enums.PieceType.ROAD)
            {
                continue;
            }

            Road currentRoad = (Road)p;
            if (currentRoad.getIsShip())
            {
                continue;
            }
            if (!p.isOnBoard())
            {
                availablePiece = true;
            }
        }
        if (!availablePiece)
        {
            return false;
        }

        return true;
    }

    public bool canSmith(int[] devChart, Vertex v, List<GamePiece> pieces, Enums.Color color)
    {

        // Make sure there is a knight that can be upgraded at the vertex
        GamePiece sourcePiece = v.getOccupyingPiece();
        if (Object.ReferenceEquals(sourcePiece, null))
        {
            return false;
        }
        if (sourcePiece.getColor() != color) {
            return false;
        }
        if (sourcePiece.getPieceType() != Enums.PieceType.KNIGHT)
        {
            return false;
        }

        Knight sourceKnight = (Knight)sourcePiece;
        int upgradeLevel = sourceKnight.getLevel() + 1;
        if (sourceKnight.wasUpgraded())
        {
            return false;
        }

        // Make sure there are enough knights of that level available
        int total = 0;
        foreach (GamePiece p in pieces)
        {
            if (p.getPieceType() != Enums.PieceType.KNIGHT)
            {
                continue;
            }
            else if (!p.isOnBoard())
            {
                continue;
            }

            Knight k = (Knight)p;
            if (k.getLevel() == upgradeLevel)
            {
                total++;
                if (total == 2)
                {
                    return false;
                }
            }
        }


        // Check the politics level for high-level knights
        int level = devChart[(int)Enums.DevChartType.POLITICS];
        if (level < 3)
        {
            if (sourceKnight.getLevel() >= 2)
            {
                return false;
            }
        }
        else if (sourceKnight.getLevel() >= 3)
        {
            return false;
        }
        return true;
    }
}
