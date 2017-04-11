using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

/// <summary>
/// Class handling behviour with regards to displaying which MoveManager panel
/// in the top middle of the screen depending on the current player's game state
/// </summary>
public class UIMoveManager : MonoBehaviour {

	private Player _CurrentPlayer;
	private UIManager _UIManager;

	[SerializeField]
	private UIGameStateBanner _Banner;

	[SerializeField]
	private Transform _ResetButton;

	#region Panel Toggle Booleans

	private bool buildingToggle;
	private bool knightToggle;
	private bool fishToggle;
	private bool progressCardToggle;

	#endregion

	/// <summary>
	/// The progress cards panel.
	/// </summary>
	[SerializeField]
	private Transform _ProgressCardsPanel;

	#region Moves Panels
	/// <summary>
	/// The initial turn panel. Allowing for Building initial settlement, city and routes
	/// </summary>
	[SerializeField]
	private Transform _InitialPhasePanel;

	/// <summary>
	/// The knight panel. Allowing for upgrade, move, displace and activate knight
	/// </summary>
	[SerializeField]
	private Transform _KnightPanel;

	/// <summary>
	/// The building panel. Allowing for buliding settlement, city, routes, city wall, and moving ships
	/// </summary>
	[SerializeField]
	private Transform _BuildingPanel;

	[SerializeField]
	private Transform _FishPanel;

	/// <summary>
	/// Specific panels for the fish button chosen
	/// </summary>
	[SerializeField]
	private Transform _3FishPanel;
	[SerializeField]
	private Transform _4FishPanel;
	[SerializeField]
	private Transform _7FishPanel;

	/// <summary>
	/// The resource discard panel.
	/// </summary>
	[SerializeField]
	private Transform _ResourceDiscardPanel;
	private int originalPlayerHandSum;

	[SerializeField]
	private Transform _PirateRobberChoosePanel;

	[SerializeField]
	private Transform _AqueductPanel;

	[SerializeField]
	private Transform _ChooseProgressCardPanel;

	[SerializeField]
	private Transform _UpgradeDevChartPanel;

	[SerializeField]
	private Transform _AlchemistDicePanel;

	[SerializeField]
	private Transform _CraneDevPanel;
	#endregion

	// -------------------------

	void Start () {
		_UIManager = GetComponent<UIManager> ();

		_InitialPhasePanel.gameObject.SetActive (true);

		_ResourceDiscardPanel.gameObject.SetActive (false);
		originalPlayerHandSum = 0;

		_ResetButton.gameObject.SetActive (false);

		_KnightPanel.gameObject.SetActive (false);
		_BuildingPanel.gameObject.SetActive (false);
		_FishPanel.gameObject.SetActive (false);
		_ProgressCardsPanel.gameObject.SetActive (false);

		_3FishPanel.gameObject.SetActive (false);
		_4FishPanel.gameObject.SetActive (false);
		_7FishPanel.gameObject.SetActive (false);

		_PirateRobberChoosePanel.gameObject.SetActive (false);
		_AqueductPanel.gameObject.SetActive (false);
		_ChooseProgressCardPanel.gameObject.SetActive (false);
		_UpgradeDevChartPanel.gameObject.SetActive (false);
		_AlchemistDicePanel.gameObject.SetActive (false);
		_CraneDevPanel.gameObject.SetActive (false);

		buildingToggle = false;
		knightToggle = false;
	}
		

	/// <summary>
	/// Ends the player's turn
	/// </summary>
	public void uiEndTurn()
	{
		_CurrentPlayer.endTurn ();
	}

	private void revealResetButton()
	{
		bool playerVCheck = GameObject.ReferenceEquals (_CurrentPlayer.v1, null);
		bool playerECheck = GameObject.ReferenceEquals (_CurrentPlayer.e1, null);
		bool playerHCheck = GameObject.ReferenceEquals (_CurrentPlayer.h1, null);

		// If any one of the player attributes is null, reveal the reset button
		if (playerVCheck || playerECheck || playerHCheck) 
		{
			_ResetButton.gameObject.SetActive (true);
		}
	}

	/// <summary>
	/// Method called when reset Button is clicked
	/// </summary>
	public void resetButtonOnClick()
	{
		_CurrentPlayer.ResetV1(_CurrentPlayer.getID());
		_CurrentPlayer.ResetE1 (_CurrentPlayer.getID ());
		_CurrentPlayer.ResetH1 (_CurrentPlayer.getID ());

	}

	/// <summary>
	/// Handles the initial phase panel display. Hiding when it is not SETUP gamephase
	/// Displaying when it is
	/// </summary>
	public void handleInitialPhasePanelDisplay()
	{
		bool setupOne = (GameManager.instance.getGamePhase() == Enums.GamePhase.SETUP_ONE);
		bool setupTwo = (GameManager.instance.getGamePhase() == Enums.GamePhase.SETUP_TWO);
		bool moveTypeNone = _CurrentPlayer.getMoveType() == MoveType.NONE;
		bool moveTypeInitialShip = _CurrentPlayer.getMoveType() == MoveType.PLACE_INITIAL_SHIP;
		bool moveTypeInitialRoad = _CurrentPlayer.getMoveType() == MoveType.PLACE_INITIAL_ROAD;
			
		if ((setupOne || setupTwo) && (moveTypeNone || moveTypeInitialShip || moveTypeInitialRoad)) 
		{
			_InitialPhasePanel.gameObject.SetActive (true);
		} 
		else 
		{
			_InitialPhasePanel.gameObject.SetActive (false);
		}


	}

	/// <summary>
	/// Updates the banner text based on GameManager gamePhase attribute
	/// and Player's moveType attribute
	/// </summary>
	private void handleBannerText()
	{
		_Banner.setHeaderText (convert(GameManager.instance.getGamePhase()));
		_Banner.setSubText (convert(_CurrentPlayer.getMoveType()));
	}

	#region Initial Turns Methods
		
	/// <summary>
	/// Calls necessary methods to place players road. (either initial, or second road) 
	/// </summary>
	/// <param name="p_RouteNumber">P route number.</param>
	public void uiPlaceInitialRoad()
	{
		moveTypeChange (MoveType.PLACE_INITIAL_ROAD);
	}

	/// <summary>
	/// Calls necessary methods to place player's ship. (either initial, or second road)
	/// </summary>
	/// <param name="p_RouteNumber">P route number.</param>
	public void uiPlaceInitialShip()
	{

		moveTypeChange (MoveType.PLACE_INITIAL_SHIP);
			// Update Move ENUM Here
	}

	/*
	/// <summary>
	/// Calls necessary methods to place player's ship
	/// </summary>
	public void uiPlaceInitialCity()
	{
		_Banner.setHeaderText ("SECOND TURN");
		_Banner.setSubText ("Place Initial City");
	}*/

	#endregion

	#region Infrastructure Building Methods
	/// <summary>
	/// Calls necessary methods to make it so player can build settlement
	/// </summary>
	public void uiBuildSettlement()
	{
		moveTypeChange(MoveType.BUILD_SETTLEMENT);
	}


	/// <summary>
	/// Calls necessary methods to make it so player can build City
	/// </summary>
	public void uiBuildCity()
	{
		moveTypeChange(MoveType.BUILD_CITY);
	}

	/// <summary>
	/// Calls necessary methods to make it so player can build City wall
	/// </summary>
	public void uiBuildCityWall()
	{

		moveTypeChange(MoveType.BUILD_CITY_WALL);
	}

	/// <summary>
	/// Calls necessary methods to make it so player can build road
	/// </summary>
	public void uiBuildRoad()
	{

		moveTypeChange (MoveType.BUILD_ROAD);
	}

	/// <summary>
	/// Calls necessary methods to make it so player can build ship 
	/// </summary>
	public void uiBuildShip()
	{
		moveTypeChange(MoveType.BUILD_SHIP);
	}

	/// <summary>
	/// Calls necessary methods to make it so player can move ship
	/// </summary>
	public void uiMoveShip()
	{

		moveTypeChange(MoveType.MOVE_SHIP);
	}


	#endregion

	#region Knight Methods
	/// <summary>
	/// Calls necessary methods so player can move knight
	/// </summary>
	public void uiBuildKnight()
	{

		moveTypeChange(MoveType.BUILD_KNIGHT);
	}


	/// <summary>
	/// Calls necessary methods so player can move knight
	/// </summary>
	public void uiMoveKnight()
	{

		moveTypeChange(MoveType.MOVE_KNIGHT);
	}

	/// <summary>
	/// Calls necessary methods so player can displace knight
	/// </summary>
	public void uiDisplaceKnight()
	{
		moveTypeChange (MoveType.DISPLACE_KNIGHT);
	}

	/// <summary>
	/// Calls necessary methods so player can upgrade knight
	/// </summary>
	public void uiUpgradeKnight()
	{
		moveTypeChange(MoveType.UPGRADE_KNIGHT);
	}

	/// <summary>
	/// Calls necessary methods so player can activate knight
	/// </summary>
	public void uiActivateKnight()
	{
		moveTypeChange (MoveType.ACTIVATE_KNIGHT);
	}

	/// <summary>
	/// Calls necessary methods so player can activate knight
	/// </summary>
	public void uiChaseRobber()
	{
		moveTypeChange(MoveType.CHASE_ROBBER);
	}

	#endregion

	#region Resource Discard Panel Methods

	public void revealResourceDiscardPanel()
	{

		if (_CurrentPlayer.getSpecial () == Special.DISCARD_RESOURCE_SEVEN && GameManager.instance.getGamePhase() == GamePhase.PHASE_TWO) 
		{
			_ResourceDiscardPanel.gameObject.SetActive (true);

			// If originalHandSum hasn't been modified at all, then set it to Player's current hand size
			if (originalPlayerHandSum == 0) {
				originalPlayerHandSum = _CurrentPlayer.getHandSize ();
			}
		} 
		else {
			_ResourceDiscardPanel.gameObject.SetActive (false);
			originalPlayerHandSum = 0;
		}
	}


	/// <summary>
	/// Decrements player's p_resource by 1
	/// </summary>
	/// <param name="p_Resource">P resource.</param>
	public void discardPlayerResource(int p_Resource)
	{
		_CurrentPlayer.changeResource ((ResourceType)p_Resource, -1, _CurrentPlayer.getID());

		// If handsize goes lower than half the original hand size when discard began,
		// close panel, revert turn back to the player who rolled 7
		if (_CurrentPlayer.getHandSize () <= (originalPlayerHandSum - originalPlayerHandSum / 2)) 
		{
			int temp = GameManager.instance.getPlayerTurn ();

			Debug.Log ("Revert Turn" + GameManager.instance.getPlayerTurn ());

			// Goes to the next player to discard
			GameManager.instance.sevenShortcut (temp+1);
		}
	}

	/// <summary>
	/// Decrements player's pCommodity by 1
	/// </summary>
	/// <param name="p_Commodity">P commodity.</param>
	public void discardPlayerCommodity(int p_Commodity)
	{
		_CurrentPlayer.changeCommodity ((CommodityType)p_Commodity, -1, _CurrentPlayer.getID());

		// If handsize goes lower than half the original hand size when discard began,
		// close panel, revert turn back to the player who rolled 7
		if (_CurrentPlayer.getHandSize () <= originalPlayerHandSum - originalPlayerHandSum / 2) 
		{
			int temp = GameManager.instance.getPlayerTurn ();

			// Goes to the next player to discard
			GameManager.instance.sevenShortcut (temp+1);
		}

	}

	#endregion



	#region Development Chart Methods
	/// <summary>
	/// Reveals the choose progress card panel at the proper player state
	/// and hides otherwise
	/// </summary>
	public void revealChooseProgressCardPanelBarbarian()
	{
		if (_CurrentPlayer.getSpecial () == Special.CHOOSE_PROGRESS_PILE && GameManager.instance.getGamePhase() == GamePhase.PHASE_TWO) 
		{
			_ChooseProgressCardPanel.gameObject.SetActive (true);
		} 

		else {
			_ChooseProgressCardPanel.gameObject.SetActive (false);
		}
	}

	/// <summary>
	/// Reveals the upgrade dev chart at the right time
	/// hides otherwise
	/// </summary>
	public void revealUpgradeDevChart()
	{
		if (_CurrentPlayer.getMoveType() == MoveType.UPGRADE_DEVELOPMENT_CHART && GameManager.instance.getGamePhase() == GamePhase.PHASE_TWO) 
		{
			_UpgradeDevChartPanel.gameObject.SetActive (true);
		} 

		else {
			_UpgradeDevChartPanel.gameObject.SetActive (false);
		}
	}

	/// <summary>
	/// Upgrades the dev chart bsaed on parameter passed
	/// </summary>
	/// <param name="p_ChartType">P chart type.</param>
	public void upgradeDevChart(int p_ChartType)
	{
		//_CurrentPlayer.upgradeDevChart((DevChartType) p_ChartType, _CurrentPlayer.getID ());
		_CurrentPlayer.CmdUpgradeDevelopmentChart((DevChartType) p_ChartType);

		// Set movetype to none afterwards
		//moveTypeChange(MoveType.NONE);
		//_CurrentPlayer.setMoveType (MoveType.NONE, _CurrentPlayer.getID ());

	}

	/// <summary>
	/// For the close button of any menu to close
	/// </summary>
	public void setNoneMoveType()
	{
		moveTypeChange(MoveType.NONE);
		//_CurrentPlayer.setMoveType (MoveType.NONE, _CurrentPlayer.getID ());
	}

	/// <summary>
	/// Picks the progress card corresponding to the int parameter provided
	/// </summary>
	/// <param name="p_ChartType">P chart type.</param>
	public void pickProgressCard(int p_ChartType)
	{
		Bank.instance.withdrawProgressCard ((DevChartType) p_ChartType, _CurrentPlayer.getID ());

		int temp = GameManager.instance.getPlayerTurn ();
		GameManager.instance.barbarianWinShortcut (temp + 1);
	}

	#endregion


	#region Progress Card Methods
	public void revealAqueductPanel()
	{
		if (_CurrentPlayer.getSpecial () == Special.AQUEDUCT) 
		{
			_AqueductPanel.gameObject.SetActive (true);
		} 

		else {
			_AqueductPanel.gameObject.SetActive (false);
		}
	}



	/// <summary>
	/// Increments the resource chosen due to the aqueduct progress card
	/// </summary>
	/// <param name="p_Resource">P resource.</param>
	public void aqueductPickResource(int p_Resource)
	{
		_CurrentPlayer.changeResource ((ResourceType)p_Resource, 1, _CurrentPlayer.getID());

		int temp = GameManager.instance.getPlayerTurn ();
		GameManager.instance.aqueductShortcut(temp+1);

	}

	public void revealProgressCardsPanel()
	{
		progressCardToggle = !progressCardToggle;
		_ProgressCardsPanel.gameObject.SetActive (progressCardToggle);

		updateProgressCardButtons ();
	}

	private void updateProgressCardButtons()
	{
		int index = 0;

		// Set all buttons to inactive to begin.
		foreach (Transform child in _ProgressCardsPanel) 
		{
			child.gameObject.SetActive (false);
		}

		// Loop through buttons for as many times as there are progress Cards in the player's hand
		foreach (Transform child in _ProgressCardsPanel) 
		{
			// If index exceeds number of progress cards that player may have, return immediately
			if (index > _CurrentPlayer.getProgressCards ().Count) 
			{
				return;
			}

			// Get the button component of the child object
			Button _button = child.GetComponent<Button> ();
			List<ProgressCardName> _PCards = _CurrentPlayer.getProgressCards ();

			Enums.ProgressCardName _progressCardName = _PCards [index];

			// Get the child of this button child
			Transform textChild = child.GetChild (0);
			// Get the text component of this child of the button
			Text _text = textChild.GetComponent<Text> ();


			assignButtonAndText (_progressCardName, _text, _button);

			index++;
		}
	}


	private void assignButtonAndText(ProgressCardName _pCN, Text p_Text, Button p_Button)
	{
		// Remove all listeners at the beginning
		p_Button.onClick.RemoveAllListeners ();

		switch (_pCN) 
		{
		case ProgressCardName.ALCHEMIST:
			p_Text.text = "Alchemist";
			moveTypeChange(MoveType.PROGRESS_ALCHEMIST);
			//_CurrentPlayer.setMoveType (MoveType.PROGRESS_ALCHEMIST, _CurrentPlayer.getID ());
			break;
		case ProgressCardName.BISHOP:
			p_Text.text = "Bishop";
			moveTypeChange(MoveType.PROGRESS_BISHOP);
			//_CurrentPlayer.setMoveType (MoveType.PROGRESS_BISHOP, _CurrentPlayer.getID ());
			break;
		case ProgressCardName.COMMERCIALHARBOR:
			//TODO : Yeah, no
			break;
		case ProgressCardName.CONSTITUTION:
			p_Text.text = "Constitution";
			p_Button.onClick.AddListener (ProgressCards.instance.constitution);
			break;
		case ProgressCardName.CRANE:
			p_Text.text = "Crane";
			moveTypeChange(MoveType.PROGRESS_CRANE);
			//_CurrentPlayer.setMoveType (MoveType.PROGRESS_CRANE, _CurrentPlayer.getID ());
			break;
		case ProgressCardName.DESERTER:
			//TODO: Yeah, no
			break;
		case ProgressCardName.DIPLOMAT:
			p_Text.text = "Diplomat";
			moveTypeChange(MoveType.PROGRESS_DIPLOMAT);
			//_CurrentPlayer.setMoveType (MoveType.PROGRESS_DIPLOMAT, _CurrentPlayer.getID ());
			break;
		case ProgressCardName.ENGINEER:
			p_Text.text = "Engineer";
			moveTypeChange(MoveType.PROGRESS_ENGINEER);
			//_CurrentPlayer.setMoveType (MoveType.PROGRESS_ENGINEER, _CurrentPlayer.getID ());
			break;
		case ProgressCardName.INTRIGUE:
			p_Text.text = "Intrigue";
			moveTypeChange(MoveType.PROGRESS_INTRIGUE);
			//_CurrentPlayer.setMoveType (MoveType.PROGRESS_INTRIGUE, _CurrentPlayer.getID ());
			break;
		case ProgressCardName.INVENTOR:
			p_Text.text = "Inventor";
			moveTypeChange(MoveType.PROGRESS_INVENTOR);
			//_CurrentPlayer.setMoveType (MoveType.PROGRESS_INVENTOR, _CurrentPlayer.getID ());
			break;
		case ProgressCardName.IRRIGATION:
			p_Text.text = "Irrigation";
			p_Button.onClick.AddListener (() => {ProgressCards.instance.irrigation(_CurrentPlayer.getColor());});
			break;
		case ProgressCardName.MASTERMERCHANT:
			// TODO: Yeah, no
			break;
		case ProgressCardName.MEDICINE:
			p_Text.text = "Medicine";
			moveTypeChange(MoveType.PROGRESS_MEDICINE);
			//_CurrentPlayer.setMoveType (MoveType.PROGRESS_MEDICINE, _CurrentPlayer.getID ());
			break;
		case ProgressCardName.MERCHANT:
			//TODO: Yeah, no
			break;
		case ProgressCardName.MERCHANTFLEET:
			//TODO: Yeah, no
			break;
		case ProgressCardName.MINING:
			p_Text.text = "Mining";
			p_Button.onClick.AddListener (() => {ProgressCards.instance.mining(_CurrentPlayer.getColor());});
			break;
		case ProgressCardName.PRINTER:
			p_Text.text = "Printer";
			p_Button.onClick.AddListener (ProgressCards.instance.printer);
			break;
		case ProgressCardName.RESOURCEMONOPOLY:
			//TODO: Yeah, no
			break;
		case ProgressCardName.ROADBUILDING:
			break;
		case ProgressCardName.SABOTEUR:
			//TODO: Yeah, no
			break;
		case ProgressCardName.SMITH:
			//TODO: Yeah, no
			break;
		case ProgressCardName.SPY:
			//TODO: Yeah, no
			break;
		case ProgressCardName.TRADEMONOPOLY:
			//TODO: Yeah, no
			break;
		case ProgressCardName.WARLORD:
			//TODO: Yeah, no
			break;
		case ProgressCardName.WEDDING:
			//TODO: Yeah, no
			break;
		}

		// Add this to make sure the button closes the panel afterwards
		p_Button.onClick.AddListener (closeProgressCardPanel);
	}


	private void revealAlchemistDiceRollPanel()
	{
		if (_CurrentPlayer.getMoveType () == MoveType.PROGRESS_ALCHEMIST  && GameManager.instance.getGamePhase() == GamePhase.PHASE_ONE ) 
		{
			_AlchemistDicePanel.gameObject.SetActive (true);
		} 

		else {
			_AlchemistDicePanel.gameObject.SetActive (false);
		}
	}

	private void revealCraneUpgradeDevPanel()
	{
		if (_CurrentPlayer.getMoveType () == MoveType.PROGRESS_CRANE  && GameManager.instance.getGamePhase() == GamePhase.PHASE_TWO ) 
		{
			_CraneDevPanel.gameObject.SetActive (true);
		} 

		else {
			_CraneDevPanel.gameObject.SetActive (false);
		}
	}

	public void craneUpgradeDevChart(int p_ChartType)
	{
		//_CurrentPlayer.upgradeDevChart((DevChartType) p_ChartType, _CurrentPlayer.getID ());
		_CurrentPlayer.CmdUpgradeDevelopmentChart((DevChartType) p_ChartType);

		// Set movetype to none afterwards
		//moveTypeChange(MoveType.NONE);
		//_CurrentPlayer.setMoveType (MoveType.NONE, _CurrentPlayer.getID ());

	}



	public void closeProgressCardPanel()
	{
		_ProgressCardsPanel.gameObject.SetActive (false);	
	}

	#endregion

	#region Fish Methods
	public void fish3StealResource(int p_ColorInt)
	{
		// If Player doesn't exist
		List<Player> _Players = GameManager.instance.players;

		// If the requested player color is out of bounds, assume they do not exist. 
		// return early
		if (p_ColorInt >= _Players.Count)
			return;
		
		// If the index of currentPlayer's colour and the parameter do match, then return
		if ((int)_CurrentPlayer.getColor () == p_ColorInt) 
		{
			return;
		}
			
		Player oppo  = GameManager.instance.getPlayer(p_ColorInt);
		bool taken = false;
		for (int i = 0; i < 5; i++) {
			if(oppo.getResources()[i] > 0) {
				_CurrentPlayer.changeResource((ResourceType)i, -1, oppo.getID());
				taken = true;
				_CurrentPlayer.changeResource((ResourceType)i, 1, _CurrentPlayer.getID());
				break;
			}
		}
		if (!taken) {
			for (int i = 0; i < 3; i++) {
				if (oppo.getCommodities () [i] > 0) {
					_CurrentPlayer.changeCommodity ((CommodityType)i, -1, oppo.getID ());
					_CurrentPlayer.changeCommodity ((CommodityType)i, 1, _CurrentPlayer.getID ());
					break;
				}
			}
		}

		// Set moveType back to NONE
		moveTypeChange(MoveType.NONE);
		//_CurrentPlayer.setMoveType (MoveType.NONE, _CurrentPlayer.getID ());
	}


	/// <summary>
	/// Increments the resource chosen due to the aqueduct progress card
	/// </summary>
	/// <param name="p_Resource">P resource.</param>
	public void fish4PickResource(int p_Resource)
	{
		_CurrentPlayer.changeResource ((ResourceType)p_Resource, 1, _CurrentPlayer.getID());

		moveTypeChange(MoveType.NONE);
		//_CurrentPlayer.setMoveType (MoveType.NONE, _CurrentPlayer.getID ());

	}


	/// <summary>
	/// Picks the progress card corresponding to the int parameter provided
	/// </summary>
	/// <param name="p_ChartType">P chart type.</param>
	public void fish7PickProgressCard(int p_ChartType)
	{
		Bank.instance.withdrawProgressCard ((DevChartType) p_ChartType, _CurrentPlayer.getID ());

		moveTypeChange(MoveType.NONE);
		//_CurrentPlayer.setMoveType (MoveType.NONE, _CurrentPlayer.getID ());
	}

	/// <summary>
	/// Reveals the fish3 panel
	/// </summary>
	public void revealFish3()
	{
		if (_CurrentPlayer.getMoveType () == MoveType.FISH_3 && GameManager.instance.getGamePhase() == GamePhase.PHASE_TWO) 
		{
			_3FishPanel.gameObject.SetActive (true);
		} 

		else {
			_3FishPanel.gameObject.SetActive (false);
		}
	}



	/// <summary>
	/// Reveals the fish4 panel
	/// </summary>
	public void revealFish4()
	{
		if (_CurrentPlayer.getMoveType () == MoveType.FISH_4 && GameManager.instance.getGamePhase() == GamePhase.PHASE_TWO) 
		{
			_4FishPanel.gameObject.SetActive (true);
		} 

		else 
		{
			_4FishPanel.gameObject.SetActive (false);
		}
	}

	/// <summary>
	/// Reveals the fish7 panel
	/// </summary>
	public void revealFish7()
	{
		if (_CurrentPlayer.getMoveType () == MoveType.FISH_7 && GameManager.instance.getGamePhase() == GamePhase.PHASE_TWO) 
		{
			_7FishPanel.gameObject.SetActive (true);
		} 

		else 
		{
			_7FishPanel.gameObject.SetActive (false);
		}
	}

	public void uiFish(int p_FishNumber)
	{
		switch (p_FishNumber) 
		{
		case 2:
			moveTypeChange(MoveType.FISH_2);
			//_CurrentPlayer.setMoveType (MoveType.FISH_2, _CurrentPlayer.getID());
			break;
		case 3:
			moveTypeChange(MoveType.FISH_3);
			//_CurrentPlayer.setMoveType (MoveType.FISH_3, _CurrentPlayer.getID());
			break;
		case 4:
			moveTypeChange(MoveType.FISH_4);
			//_CurrentPlayer.setMoveType (MoveType.FISH_4, _CurrentPlayer.getID());
			break;
		case 5:
			moveTypeChange(MoveType.FISH_5);
			//_CurrentPlayer.setMoveType (MoveType.FISH_5, _CurrentPlayer.getID());
			break;
		case 7:
			moveTypeChange(MoveType.FISH_7);
			//_CurrentPlayer.setMoveType (MoveType.FISH_7, _CurrentPlayer.getID());
			break;
		default:
			break;
		}
	}

	#endregion


	#region Choose Pirate or Robber Panel Methods
	public void revealChoosePirateRobberPanel()
	{
		if (_CurrentPlayer.getSpecial () == Special.CHOOSE_PIRATE_OR_ROBBER && GameManager.instance.getGamePhase() == GamePhase.PHASE_TWO) 
		{
			_PirateRobberChoosePanel.gameObject.SetActive (true);

		} 
		else {
			_PirateRobberChoosePanel.gameObject.SetActive (false);

		}
	}

	/// <summary>
	/// Chooses the pirate or robber depending on parameter value
	/// </summary>
	/// <param name="p_EnumIndex">P enum index.</param>
	public void choosePirateOrRobber(int p_EnumIndex)
	{
		switch (p_EnumIndex) 
		{
		case 1:
			_CurrentPlayer.setSpecial (Enums.Special.MOVE_ROBBER, _CurrentPlayer.getID());
			break;
		case 2:
			_CurrentPlayer.setSpecial (Enums.Special.MOVE_PIRATE, _CurrentPlayer.getID());
			break;
		default:
			break;
		}
	}
	#endregion


	#region Development Chart Methods
	/// <summary>
	/// Calls necessary methods so player can upgrade development chart
	/// </summary>
	public void uiUpgradeDevelopmentChart()
	{
		moveTypeChange(MoveType.UPGRADE_DEVELOPMENT_CHART);
	}

	#endregion


	#region Panel Toggles

	/// <summary>
	/// Toggles the build panel.
	/// </summary>
	public void toggleBuildPanel()
	{
		if (GameManager.instance.getGamePhase () == GamePhase.SETUP_ONE || GameManager.instance.getGamePhase () == GamePhase.SETUP_TWO)
			return;
		
		buildingToggle = !buildingToggle;
		_BuildingPanel.gameObject.SetActive (buildingToggle);
		_KnightPanel.gameObject.SetActive (false);
		_FishPanel.gameObject.SetActive (false);
	}

	public void toggleKnightsPanel()
	{
		if (GameManager.instance.getGamePhase () == GamePhase.SETUP_ONE || GameManager.instance.getGamePhase () == GamePhase.SETUP_TWO)
			return;
		
		knightToggle = !knightToggle;
		_KnightPanel.gameObject.SetActive (knightToggle);
		_BuildingPanel.gameObject.SetActive (false);
		_FishPanel.gameObject.SetActive (false);
	}

	public void toggleFishPanel()
	{
		if (GameManager.instance.getGamePhase () == GamePhase.SETUP_ONE || GameManager.instance.getGamePhase () == GamePhase.SETUP_TWO)
			return;

		fishToggle = !fishToggle;
		_FishPanel.gameObject.SetActive (fishToggle);
		_KnightPanel.gameObject.SetActive (false);
		_BuildingPanel.gameObject.SetActive (false);
	}

	#endregion
	/// <summary>
	/// Sets the current player attribute of this instance using parameter value
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public void setCurrentPlayer(Player p_Player)
	{
		_CurrentPlayer = p_Player;
	}


	/// <summary>
	/// Converts GamePHase enum
	/// </summary>
	/// <param name="p_GamePhase">P game phase.</param>
	private string convert(Enums.GamePhase p_GamePhase)
	{
		string rString = "";

		switch (p_GamePhase) 
		{
		case Enums.GamePhase.SETUP_ONE:
			rString = "Setup One";
			break;
		case Enums.GamePhase.SETUP_TWO:
			rString = "Setup Two";
			break;
		case Enums.GamePhase.PHASE_ONE:
			rString = "Phase One";
			break;
		case Enums.GamePhase.PHASE_TWO:
			rString = "Phase Two";
			break;
		default:
			break;
		}

		return rString;
	}

	/// <summary>
	/// Converts GamePHase enum
	/// </summary>
	/// <param name="p_GamePhase">P game phase.</param>
	private string convert(Enums.MoveType p_MoveType)
	{
		string rString = "";

		switch (p_MoveType) 
		{
		case Enums.MoveType.ACTIVATE_KNIGHT:
			rString = "Activate Knight";
			break;
		case Enums.MoveType.BUILD_CITY:
			rString = "Build City";
			break;
		case Enums.MoveType.BUILD_CITY_WALL:
			rString = "Build City Wall";
			break;
		case Enums.MoveType.BUILD_KNIGHT:
			rString = "Build Knight";
			break;
		case Enums.MoveType.BUILD_ROAD:
			rString = "Build Road";
			break;
		case Enums.MoveType.BUILD_SETTLEMENT:
			rString = "Build Settlement";
			break;
		case Enums.MoveType.BUILD_SHIP:
			rString = "Build Ship";
			break;
		case Enums.MoveType.CHASE_ROBBER:
			rString = "Chase Robber";
			break;
		case Enums.MoveType.DISPLACE_KNIGHT:
			rString = "Displace Knight";
			break;
		case Enums.MoveType.FISH_2:
			rString = "2 Fish";
			break;
		case Enums.MoveType.FISH_3:
			rString = "3 Fish";
			break;
		case Enums.MoveType.FISH_4:
			rString = "4 Fish";
			break;
		case Enums.MoveType.FISH_5:
			rString = "5 Fish";
			break;
		case Enums.MoveType.FISH_7:
			rString = "7 Fish";
			break;
		case Enums.MoveType.MOVE_KNIGHT:
			rString = "Move Knight";
			break;
		case Enums.MoveType.MOVE_SHIP:
			rString = "Move Ship";
			break;
		case Enums.MoveType.NONE:
			rString = "Use Your Imagination (NONE)";
			break;
		case Enums.MoveType.PLACE_INITIAL_CITY:
			rString = "Place Initial City";
			break;
		case Enums.MoveType.PLACE_INITIAL_ROAD:
			rString = "Place Initial Road";
			break;
		case Enums.MoveType.PLACE_INITIAL_SETTLEMENT:
			rString = "Place Initial Settlement";
			break;
		case Enums.MoveType.PLACE_INITIAL_SHIP:
			rString = "Place Initial Ship";
			break;
		case Enums.MoveType.PROGRESS_ALCHEMIST:
			rString = "TRAP CARD (Alchemist)";
			break;
		case Enums.MoveType.PROGRESS_BISHOP:
			rString = "Progress Card : Bishop";
			break;
		case Enums.MoveType.PROGRESS_CRANE:
			rString = "Progress Card : Crane";
			break;
		case Enums.MoveType.PROGRESS_DIPLOMAT:
			rString = "Progress Card : Diplomat";
			break;
		case Enums.MoveType.PROGRESS_ENGINEER:
			rString = "Progress Card : Engineer";
			break;
		case Enums.MoveType.PROGRESS_INTRIGUE:
			rString = "Progress Card : Intrigue";
			break;
		case Enums.MoveType.PROGRESS_INVENTOR:
			rString = "Progress Card : Inventor";
			break;
		case Enums.MoveType.PROGRESS_MEDICINE:
			rString = "Progress Card : Medicine";
			break;
		case Enums.MoveType.PROGRESS_ROAD_BUILDING_1:
			rString = "RoadBuilding : Place First Road";
			break;
		case Enums.MoveType.PROGRESS_ROAD_BUILDING_2:
			rString = "RoadBuilding : Place Second Road";
			break;
		case Enums.MoveType.SPECIAL:
			rString = "Special: " + convert(_CurrentPlayer.getSpecial());
			break;
		case Enums.MoveType.UPGRADE_KNIGHT:
			rString = "Upgrade Knight";
			break;
		case Enums.MoveType.UPGRADE_DEVELOPMENT_CHART:
			rString = "Upgrade Development Chart";
			break;
		default:
			break;
		}

		return rString;
	}

	private string convert(Enums.Special  p_Special)
	{
		string rString = "";

		switch (p_Special) 
		{
		case Special.AQUEDUCT:
			rString = "Aqueduct";
			break;
		case Special.CHOOSE_DESTROYED_CITY:
			rString = "Choose City to Destroy";
			break;
		case Special.CHOOSE_METROPOLIS:
			rString = "Choose Metropolis";
			break;
		case Special.CHOOSE_OPPONENT_RESOURCES:
			rString = "Choose Opponent Resources";
			break;
		case Special.CHOOSE_PIRATE_OR_ROBBER:
			rString = "Choose Pirate or Robber";
			break;
		case Special.DISCARD_PROGRESS:
			rString = "Discard Progress Card";
			break;
		case Special.CHOOSE_PROGRESS_PILE:
			rString = "Choose Progress Pile";
			break;
		case Special.DISCARD_RESOURCE_SEVEN:
			rString = "Discard Cards";
			break;
		case Special.KNIGHT_DISPLACED:
			rString = "Knight Displaced";
			break;
		case Special.MOVE_PIRATE:
			rString = "Move Pirate";
			break;
		case Special.MOVE_ROBBER:
			rString = "Move Robber";
			break;
		case Special.STEAL_RESOURCES_ROBBER:
			rString = "Steal Resources Robber";
			break;
		case Special.STEAL_RESOURCES_PIRATE:
			rString = "Steal Resources Pirate";
			break;
		case Special.NONE:
			rString = "None";
			break;
		default:
			break;
		}

		return rString;
	}


	private void moveTypeChange(Enums.MoveType mt) {
		if (_CurrentPlayer.getMoveType () != MoveType.SPECIAL) {
			_CurrentPlayer.setMoveType (mt, _CurrentPlayer.getID());
		}
	}
			

	// Update is called once per frame
	void Update () {

		handleInitialPhasePanelDisplay ();
		handleBannerText ();

		revealResetButton ();

		/* Show or hide Discard Resource Panel */
		revealResourceDiscardPanel ();
		revealChoosePirateRobberPanel ();
		revealAqueductPanel ();
		revealUpgradeDevChart ();
		revealChooseProgressCardPanelBarbarian ();

		revealFish3 ();
		revealFish4 ();
		revealFish7 ();

		revealAlchemistDiceRollPanel ();
		revealCraneUpgradeDevPanel ();
	}
}
