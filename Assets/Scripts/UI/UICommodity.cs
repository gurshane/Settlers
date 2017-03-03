using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class UICommodity : UIElement  {

	#region Private Attributes
	/// <summary>
	/// The resource type of this instance
	/// </summary>
	[SerializeField]
	private CommodityType _Commodity;

	private Text _CommodityCount;

	#endregion

	// Use this for initialization
	void Start () {

		setCommodityType ();
	}


	/// <summary>
	/// Updates the UI Element to display the commodities of the given Player
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public override void uiUpdate(Player p_Player)
	{
		// Get Player's commodity list, find the index in the list
		// which corresponds to this instance's CommodityType
		int[] commodityList = p_Player.getCommodities();
		int commodityIndex = (int)_Commodity;

		// Update UI Text to display the new value
		_CommodityCount.text = "x " + commodityList [commodityIndex];

	}


	/// <summary>
	/// Set the resource type of this instance based on its name
	/// </summary>
	private void setCommodityType()
	{
		switch (name) 
		{
		case "Cloth_Image":
			_Commodity = CommodityType.CLOTH;
			break;
		case "Coin_Image":
			_Commodity = CommodityType.COIN;
			break;
		case "Paper_Image":
			_Commodity = CommodityType.PAPER;
			break;
		default:
			break;
		}
	}


	// Update is called once per frame
	void Update () {

	}
}
