using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enums {

	public enum Color { WHITE, ORANGE, RED, BLUE, NONE };

	public enum ResourceType { ORE, LUMBER, WOOL, BRICK, GRAIN };

	public enum CommodityType { CLOTH, COIN, PAPER };

	public enum PieceType { ROAD, SETTLEMENT, CITY, KNIGHT, SHIP, 
		PIRATE, ROBBER, MERCHANT };

	public enum Status { ACTIVE, INACTIVE };

	public enum DevChartType { TRADE, POLITICS, SCIENCE };

	public enum GamePhase { SETUP_ONE, SETUP_TWO, PHASE_ONE, PHASE_TWO };

	public enum EventDie { BARBARIAN, TRADE, POLITICS, SCIENCE };

	public enum BarbarianStatus { HERE, THERE };

	public enum HexType { GOLD, WATER, DESERT, FOREST, PASTURE,
		FIELD, HILL, MOUNTAIN };

	public enum TurnOrder { FIRST, SECOND, THIRD, FOURTH };

	public enum TerrainType { LAND, WATER };

	public enum ProgressCardName { ALCHEMIST, CRANE, ENGINEER, INVENTOR,
		IRRIGATION, MEDICINE, MINING, PRINTER, ROADBUILDING, 
		SMITH, BISHOP, CONSTITUTION, DESERTER, DIPLOMAT, INTRIGUE, 
		SABOTEUR, SPY, WARLORD, WEDDING, COMMERCIALHARBOR, 
		MASTERMERCHANT, MERCHANT, MERCHANTFLEET,
		RESOURCEMONOPOLY, TRADEMONOPOLY };

	public enum PortType { NONE, GENERIC, ORE, LUMBER, WOOL, BRICK, GRAIN };
}
