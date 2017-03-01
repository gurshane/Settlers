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

	// Make sure a given trade is valid
	public static bool isValidBankTrade(int[] resRatios, int[] comRatios, Trades trade) {
		int totalAvailable = 0;
		int totalWanted = 0;

		// Extract the information from the trade
		int[] resOffered = trade.getResourcesOffered ();
		int[] resWanted = trade.getResourcesWanted ();
		int[] comOffered = trade.getCommoditiesOffered ();
		int[] comWanted = trade.getCommoditiesWanted ();

		// Find the total offered amount
		for (int i = 0; i < resOffered.Length; i++) {
			totalAvailable += resOffered [i] / resRatios [i];
		}
		for (int i = 0; i < comOffered.Length; i++) {
			totalAvailable += comOffered [i] / comRatios [i];
		}
		totalAvailable += trade.getGoldOffered () / 2;

		// Find the total requested amount
		for (int i = 0; i < resWanted.Length; i++) {
			if (resWanted [i] > resources [i]) {
				return false;
			}
			totalWanted += resWanted [i];
		}
		for (int i = 0; i < comWanted.Length; i++) {
			if (comWanted [i] > commodities [i]) {
				return false;
			}
			totalWanted += comWanted [i];
		}

		// Return true if the requested amount is valid
		if (totalWanted != 0 && totalWanted <= totalAvailable) {
			return true;
		} else {
			return false;
		}
	}

	// Make a trade with the bank
	public static bool tradeWithBank(int[] resRatios, int[] comRatios, Trades trade) {
		if (!isValidBankTrade (resRatios, comRatios, trade)) {
			return false;
		}

		// Extract the information from the trade
		int[] resOffered = trade.getResourcesOffered ();
		int[] resWanted = trade.getResourcesWanted ();
		int[] comOffered = trade.getCommoditiesOffered ();
		int[] comWanted = trade.getCommoditiesWanted ();

		Player trader = trade.getPlayerOffering ();

		// Update all relevent fields
		for (int i = 0; i < resOffered.Length; i++) {
			trader.discardResource ((Enums.ResourceType)i, resOffered [i]);
			depositResource ((Enums.ResourceType)i, resOffered [i]);
		}
		for (int i = 0; i < comOffered.Length; i++) {
			trader.discardCommodity ((Enums.CommodityType)i, comOffered [i]);
			depositCommodity ((Enums.CommodityType)i, comOffered [i]);
		}
		for (int i = 0; i < resWanted.Length; i++) {
			trader.addResource ((Enums.ResourceType)i, resWanted [i]);
			withdrawResource ((Enums.ResourceType)i, resWanted [i]);
		}
		for (int i = 0; i < comWanted.Length; i++) {
			trader.addCommodity ((Enums.CommodityType)i, comWanted [i]);
			withdrawCommodity ((Enums.CommodityType)i, comWanted [i]);
		}
		trader.decrementGoldCount (trade.getGoldOffered());

		return true;
	}
}
