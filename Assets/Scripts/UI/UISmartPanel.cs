using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISmartPanel : MonoBehaviour {

	[SerializeField]
	private UIManager _UIM;
	private Player _CurrentPlayer;

	[SerializeField]
	private Button _MoveBuildButton;
	[SerializeField]
	private Button _MoveKnightButton;
	[SerializeField]
	private Button _CardsButton;
	[SerializeField]
	private Button _BankTradeButton;
	[SerializeField]
	private Button _PlayerTradeButton;
	[SerializeField]
	private Button _FishButton;

	// Use this for initialization
	void Start () {

		_CurrentPlayer = _UIM.getCurrentPlayer ();
		
	}

	public void updateClickability()
	{
		bool setClickable = (_CurrentPlayer.getID () == GameManager.instance.getPlayerTurn ());

		_MoveBuildButton.interactable = setClickable;
		_MoveKnightButton.interactable = setClickable;
		_CardsButton.interactable = setClickable;
		_BankTradeButton.interactable = setClickable;
		_PlayerTradeButton.interactable = setClickable;
		_FishButton.interactable = setClickable;

	}

	// Update is called once per frame
	void Update () {

		updateClickability ();
		
	}
}
