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

	#region Panel Toggle Booleans

	private bool buildingToggle;
	private bool knightToggle;

	#endregion


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

	#endregion

	// -------------------------

	void Start () {
		_UIManager = GetComponent<UIManager> ();

		buildingToggle = false;
		knightToggle = false;
	}
		

	/// <summary>
	/// Ends the player's turn
	/// </summary>
	public void uiEndTurn()
	{
		_CurrentPlayer.CmdEndTurn ();
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
		buildingToggle = !buildingToggle;
		_BuildingPanel.gameObject.SetActive (buildingToggle);
		_KnightPanel.gameObject.SetActive (false);
	}

	public void toggleKnightsPanel()
	{
		knightToggle = !knightToggle;
		_KnightPanel.gameObject.SetActive (knightToggle);
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
		case Enums.MoveType.MOVE_KNIGHT:
			rString = "Move Knight";
			break;
		case Enums.MoveType.MOVE_SHIP:
			rString = "Move Ship";
			break;
		case Enums.MoveType.NONE:
			rString = "NONE";
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
		case Enums.MoveType.SPECIAL:
			rString = "Special";
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

	private void moveTypeChange(Enums.MoveType mt) {
		if (_CurrentPlayer.getMoveType () != MoveType.SPECIAL) {
			_CurrentPlayer.CmdSetMoveType (mt);
		}
	}
			

	// Update is called once per frame
	void Update () {

		handleInitialPhasePanelDisplay ();
		handleBannerText ();
	}
}
