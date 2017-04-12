using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITradeReceivePanel : MonoBehaviour {

	#region Sliders
	[SerializeField]
	private Slider _OreSlider;
	[SerializeField]
	private Slider _LumberSlider;
	[SerializeField]
	private Slider _WoolSlider;
	[SerializeField]
	private Slider _BrickSlider;
	[SerializeField]
	private Slider _GrainSlider;
	[SerializeField]
	private Slider _GoldSlider;

	[SerializeField]
	private Slider _ClothSlider;
	[SerializeField]
	private Slider _CoinSlider;
	[SerializeField]
	private Slider _PaperSlider;
	#endregion


	// Use this for initialization
	void Start () {

	}


	public int[] getResources()
	{
		int[] resourcesOffered = new int[5];

		resourcesOffered [0] = (int) _OreSlider.value;
		resourcesOffered [1] = (int) _LumberSlider.value;
		resourcesOffered [2] = (int) _WoolSlider.value;
		resourcesOffered [3] = (int) _BrickSlider.value;
		resourcesOffered [4] = (int) _GrainSlider.value;

		return resourcesOffered;

	}

	public int[] getCommodities()
	{
		int[] commoditiesOffered = new int[3];

		commoditiesOffered [0] = (int)_ClothSlider.value;
		commoditiesOffered [1] = (int)_CoinSlider.value;
		commoditiesOffered [2] = (int)_PaperSlider.value;

		return commoditiesOffered;
	}

	public int getGold()
	{
		return (int) _GoldSlider.value;
	}



	public void updateTextResource(int p_SliderType)
	{
		Text numText;

		switch (p_SliderType) 
		{
		case 0:
			numText = _OreSlider.transform.GetChild (3).GetComponent<Text> ();
			numText.text = "" + _OreSlider.value;
			break;
		case 1:
			numText = _LumberSlider.transform.GetChild (3).GetComponent<Text> ();
			numText.text = "" + _LumberSlider.value;
			break;
		case 2:
			numText = _WoolSlider.transform.GetChild (3).GetComponent<Text> ();
			numText.text = "" + _WoolSlider.value;
			break;
		case 3:
			numText = _BrickSlider.transform.GetChild (3).GetComponent<Text> ();
			numText.text = "" + _BrickSlider.value;
			break;
		case 4:
			numText = _GrainSlider.transform.GetChild (3).GetComponent<Text> ();
			numText.text = "" + _GrainSlider.value;
			break;
		case 5:
			numText = _GoldSlider.transform.GetChild (3).GetComponent<Text> ();
			numText.text = "" + _GoldSlider.value;
			break;
		default:
			break;
		}
	}

	public void updateTextCommodity(int p_SliderType)
	{
		Text numText;

		switch (p_SliderType) 
		{
		case 0:
			numText = _ClothSlider.transform.GetChild (3).GetComponent<Text> ();
			numText.text = "" + _ClothSlider.value;
			break;
		case 1:
			numText = _CoinSlider.transform.GetChild (3).GetComponent<Text> ();
			numText.text = "" + _CoinSlider.value;
			break;
		case 2:
			numText = _PaperSlider.transform.GetChild (3).GetComponent<Text> ();
			numText.text = "" + _PaperSlider.value;
			break;
		default:
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
