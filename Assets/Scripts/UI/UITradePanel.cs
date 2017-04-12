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

	[SerializeField]
	private Button _PlayerSubmitButton;
	[SerializeField]
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
	public void showBankSubmitButton ()
	{
		_PlayerSubmitButton.gameObject.SetActive (false);
		_BankSubmitButton.gameObject.SetActive (true);
	}

	/// <summary>
	/// Shows the player submit button in the trade panel
	/// </summary>
	public void showPlayerSubmitButton ()
	{
		_PlayerSubmitButton.gameObject.SetActive (true);
		_BankSubmitButton.gameObject.SetActive (false);
	}

	public void bankTradeSubmit()
	{
		//Trade, using all the slider values
		int[] resourcesOffered = _OfferingPanel.getResources();
		int[] commoditiesOffered = _OfferingPanel.getCommodities ();
		int goldOffered = _OfferingPanel.getGold ();

		int[] resourcesReceive = _ReceivingPanel.getResources();
		int[] commoditiesReceive = _ReceivingPanel.getCommodities ();
		int goldReceive = _ReceivingPanel.getGold ();

		//_CurrentPlayer
	}
		
	public void playerTradeSubmit()
	{
		int[] resourcesOffered = _OfferingPanel.getResources();
		int[] commoditiesOffered = _OfferingPanel.getCommodities ();
		int goldOffered = _OfferingPanel.getGold ();

		int[] resourcesReceive = _ReceivingPanel.getResources();
		int[] commoditiesReceive = _ReceivingPanel.getCommodities ();
		int goldReceive = _ReceivingPanel.getGold ();

		//CurrentPlayer
	}

	// Update is called once per frame
	void Update () {
		
	}
}
