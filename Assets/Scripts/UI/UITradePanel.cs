using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITradePanel : MonoBehaviour {

	[SerializeField]
	private UIManager _UIM;
	private Player _CurrentPlayer;

	[SerializeField]
	private UITradeOfferingPanel _OfferingPanel;
	[SerializeField]
	private UITradeReceivePanel _ReceivingPanel;

	// Use this for initialization
	void Start () {
		_CurrentPlayer = _UIM.getCurrentPlayer ();	
	}

	public void submitButton()
	{
		//Trade, using all the slider values
	}

	// Update is called once per frame
	void Update () {
		
	}
}
