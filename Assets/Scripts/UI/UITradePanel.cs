using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITradePanel : MonoBehaviour {

	[SerializeField]
	private UIManager _UIM;
	private Player _CurrentPlayer;

	[SerializeField]
	private UITradeOfferingPanel _OfferingPanel;
	[SerializeField]
	private UITradeReceivePanel _ReceivingPanel;

	private Button _PlayerSubmitButton;
	private Button _BankSubmitButton;

	// Use this for initialization
	void Start () {
		_CurrentPlayer = _UIM.getCurrentPlayer ();

		_PlayerSubmitButton.gameObject.SetActive (false);
		_BankSubmitButton.gameObject.SetActive (false);
	}

	/// <summary>
	/// Shows the bank submit button in the trade panel
	/// </summary>
	public void showBankSubmit ()
	{
		_PlayerSubmitButton.gameObject.SetActive (false);
		_BankSubmitButton.gameObject.SetActive (true);
	}

	/// <summary>
	/// Shows the player submit button in the trade panel
	/// </summary>
	public void showPlayerSubmit ()
	{
		_PlayerSubmitButton.gameObject.SetActive (true);
		_BankSubmitButton.gameObject.SetActive (false);
	}

	public void submitButton()
	{
		//Trade, using all the slider values
	}
		

	// Update is called once per frame
	void Update () {
		
	}
}
