using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bank {

	private static int[] resources;
	private static int[] commodities;
	private static List<Enums.ProgressCardName> tradeCards;
	private static List<Enums.ProgressCardName> politicsCards;
	private static List<Enums.ProgressCardName> scienceCards;

	public static int getResourceAmount(Enums.ResourceType res) {
		return resources [(int)res];
	}

	public static int getCommodityAmount(Enums.CommodityType com) {
		return commodities [(int)com];
	}

	public static bool withdrawResource(Enums.ResourceType res, int amount) {
		if (resources [(int)res] < amount) {
			return false;
		}
		resources [(int)res] -= amount;
		return true;
	}

	public static bool withdrawCommodity(Enums.CommodityType com, int amount) {
		if (commodities [(int)com] < amount) {
			return false;
		}
		commodities [(int)com] -= amount;
		return true;
	}

	public static void depositResource(Enums.ResourceType res, int amount) {
		resources [(int)res] += amount;
	}

	public static void depositCommodity(Enums.CommodityType com, int amount) {
		commodities [(int)com] += amount;
	}

	// Put the given progress card on the bottom of a progress card pile
	public static void depositProgressCard(Enums.DevChartType progressType, 
		Enums.ProgressCardName progressCard){

		if (progressType == Enums.DevChartType.TRADE) {
			tradeCards.Add (progressCard);
		} else if (progressType == Enums.DevChartType.POLITICS) {
			politicsCards.Add (progressCard);
		} else if (progressType == Enums.DevChartType.SCIENCE) {
			scienceCards.Add (progressCard);
		}
	}

	// Draw and return a progress card from the requested pile
	public static Enums.ProgressCardName withdrawProgressCard(Enums.DevChartType progressType) {
		Enums.ProgressCardName ret;

		if (progressType == Enums.DevChartType.TRADE) {
			ret = tradeCards [0];
			tradeCards.RemoveAt (0);
			return ret;
		} else if (progressType == Enums.DevChartType.POLITICS) {
			ret = politicsCards [0];
			politicsCards.RemoveAt (0);
			return ret;
		} else {
			ret = scienceCards [0];
			scienceCards.RemoveAt (0);
			return ret;
		}
	}

	// Is valid bank trade needs to be done

	// Trade with bank needs to be done
}
