using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITradeNotification : MonoBehaviour {

	public Transform _GoldImage;

	public Transform _OreImage;
	public Transform _LumberImage;
	public Transform _WoolImage;
	public Transform _BrickImage;
	public Transform _GrainImage;

	public Transform _ClothImage;
	public Transform _CoinImage;
	public Transform _PaperImage;

	// Use this for initialization
	void Start () {
		
	}

	public void updateGoldText(int gold)
	{
		_GoldImage.GetChild (0).GetComponent<Text> ().text = "" + gold;
	}

	public void updateResourcesText(int[] resources)
	{
		_OreImage.GetChild (0).GetComponent<Text> ().text = "" + resources [0];
		_LumberImage.GetChild (0).GetComponent<Text> ().text = "" + resources [1];
		_WoolImage.GetChild (0).GetComponent<Text> ().text = "" + resources [2];
		_BrickImage.GetChild (0).GetComponent<Text> ().text = "" + resources [3];
		_GrainImage.GetChild (0).GetComponent<Text> ().text = "" + resources [4];
	}

	public void updateCommoditiesText(int[] commodities)
	{
		_ClothImage.GetChild (0).GetComponent<Text> ().text = "" + commodities [0];
		_CoinImage.GetChild (0).GetComponent<Text> ().text = "" + commodities [1];
		_PaperImage.GetChild (0).GetComponent<Text> ().text = "" + commodities [2];
	}

	// Update is called once per frame
	void Update () {

	}
}
