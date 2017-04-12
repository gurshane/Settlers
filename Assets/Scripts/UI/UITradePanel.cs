using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITradePanel : MonoBehaviour {

	[SerializeField]
	private Transform _OfferingPanel;
	[SerializeField]
	private Transform _ReceivingPanel;

	// Use this for initialization
	void Start () {
		
	}

	public void updateTextResource(int p_SliderType)
	{
		switch (p_SliderType) 
		{
		case 0:
			break;
		case 1:
			break;
		default:
			break;
		}
	}

	public void updateTextCommodity(int p_SliderType)
	{
		
	}

	// Update is called once per frame
	void Update () {
		
	}
}
