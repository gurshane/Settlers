using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITradePanel : MonoBehaviour {

	public UITradeNotification _OfferedNotification;
	public UITradeNotification _ReceiveNotification;

	[SerializeField]
	private UIManager _UIM;
	private Player _CurrentPlayer;

	[SerializeField]
	private UITradeOfferingPanel _OfferingPanel;
	[SerializeField]
	private UITradeReceivePanel _ReceivingPanel;

	[SerializeField]
	private Transform _TradeAcceptancePanel;

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

		_CurrentPlayer.tradeWithBank (resourcesOffered, resourcesReceive, commoditiesOffered, commoditiesReceive, goldOffered, goldReceive);
	}
		
	public void playerTradeSubmit()
	{
		int[] resourcesOffered = _OfferingPanel.getResources();
		int[] commoditiesOffered = _OfferingPanel.getCommodities ();
		int goldOffered = _OfferingPanel.getGold ();

		int[] resourcesReceive = _ReceivingPanel.getResources();
		int[] commoditiesReceive = _ReceivingPanel.getCommodities ();
		int goldReceive = _ReceivingPanel.getGold ();

		// if this is true, pop up the accept panel
		//bool _CurrentPlayer.validateTrade;
		//_CurrentPlayer.CmdDeclineTrade();
		//_CurrentPlayer.validateTrade();

		//_CurrentPlayer.canTrade (Enums.ResourceType);

		_CurrentPlayer.CmdSpawnTrade (resourcesOffered, resourcesReceive, commoditiesOffered, commoditiesReceive, goldOffered, goldReceive);
	}

	public void revealTradeAcceptance()
	{
		//Trade, using all the slider values
		int[] resourcesOffered = _OfferingPanel.getResources();
		int[] commoditiesOffered = _OfferingPanel.getCommodities ();
		int goldOffered = _OfferingPanel.getGold ();

		int[] resourcesReceive = _ReceivingPanel.getResources();
		int[] commoditiesReceive = _ReceivingPanel.getCommodities ();
		int goldReceive = _ReceivingPanel.getGold ();


		if (_CurrentPlayer.validateTrade()  && GameManager.instance.getGamePhase() == Enums.GamePhase.PHASE_TWO ) 
		{
			_TradeAcceptancePanel.gameObject.SetActive (true);

			_OfferedNotification.updateResourcesText (resourcesOffered);
			_OfferedNotification.updateCommoditiesText (commoditiesOffered);
			_OfferedNotification.updateGoldText (goldOffered);

			_ReceiveNotification.updateResourcesText (resourcesReceive);
			_ReceiveNotification.updateCommoditiesText (commoditiesReceive);
			_ReceiveNotification.updateGoldText (goldReceive);


		} 

		else {
			_TradeAcceptancePanel.gameObject.SetActive (false);
		}
	}

	public void acceptTrade()
	{
		_CurrentPlayer.validateTrade();
	}

	public void declineTrade()
	{
		_CurrentPlayer.CmdDeclineTrade();
	}

	// Update is called once per frame
	void Update () {

		revealTradeAcceptance ();
	}
}
