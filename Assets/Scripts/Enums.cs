﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enums {

	public enum Color { ORANGE, WHITE, RED, BLUE, NONE };

	public enum ResourceType { ORE, LUMBER, WOOL, BRICK, GRAIN, NONE };

	public enum CommodityType { CLOTH, COIN, PAPER, NONE };

	public enum PieceType { ROAD, SETTLEMENT, CITY, KNIGHT, 
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

	public enum MoveType { MOVE_KNIGHT, DISPLACE_KNIGHT, UPGRADE_KNIGHT, 
		ACTIVATE_KNIGHT, UPGRADE_DEVELOPMENT_CHART, BUILD_SETTLEMENT, BUILD_CITY, BUILD_CITY_WALL,
		BUILD_KNIGHT, BUILD_ROAD, BUILD_SHIP, MOVE_SHIP, CHASE_ROBBER, PLACE_INITIAL_SETTLEMENT,
		PLACE_INITIAL_CITY, PLACE_INITIAL_ROAD, PLACE_INITIAL_SHIP, FISH_2, FISH_3, FISH_4,
		FISH_5, FISH_7, PROGRESS_BISHOP, PROGRESS_ALCHEMIST, PROGRESS_DIPLOMAT, PROGRESS_INTRIGUE, 
		PROGRESS_CRANE, PROGRESS_ENGINEER, PROGRESS_INVENTOR, PROGRESS_MEDICINE, PROGRESS_ROAD_BUILDING_1, 
		PROGRESS_ROAD_BUILDING_2, PROGRESS_TRADE_MONOPOLY, PROGRESS_RESOURCE_MONOPOLY,
		PROGRESS_SPY, PROGRESS_MERCHANT, SPECIAL, NONE}

	public enum Special {KNIGHT_DISPLACED, DISCARD_RESOURCE_SEVEN, DISCARD_PROGRESS, CHOOSE_PIRATE_OR_ROBBER,
		MOVE_ROBBER, MOVE_PIRATE, STEAL_RESOURCES_ROBBER, STEAL_RESOURCES_PIRATE, CHOOSE_OPPONENT_RESOURCES,
		CHOOSE_PROGRESS_PILE, CHOOSE_DESTROYED_CITY, CHOOSE_METROPOLIS, AQUEDUCT, DISCARD_RESOURCE_SABOTEUR,
		TAKE_OPPONENT_PROGRESS, YOU_WIN, YOU_LOSE,  NONE}
}
