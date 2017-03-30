using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	}

	#region Initial Turns Methods
	/// <summary>
	/// Calls necessary methods to place player's initial settlement
	/// </summary>
	public void uiPlaceInitialSettlement()
	{
		_Banner.setHeaderText ("FIRST TURN");
		_Banner.setSubText ("Place Initial Settlement");

	}
		
	/// <summary>
	/// Calls necessary methods to place players road. (either initial, or second road) 
	/// </summary>
	/// <param name="p_RouteNumber">P route number.</param>
	public void uiPlaceInitialRoad(int p_RouteNumber)
	{
		switch (p_RouteNumber) 
		{
		case 1:
			_Banner.setHeaderText ("FIRST TURN");
			_Banner.setSubText ("Place Initial Road");
			// Update Move ENUM Here
			break;
		case 2:
			_Banner.setHeaderText ("SECOND TURN");
			_Banner.setSubText ("Place Second Road");
			// Update Move ENUM here
			break;
		default:
			break;
		}

	}

	/// <summary>
	/// Calls necessary methods to place player's ship. (either initial, or second road)
	/// </summary>
	/// <param name="p_RouteNumber">P route number.</param>
	public void uiPlaceInitialShip(int p_RouteNumber)
	{
		switch (p_RouteNumber) 
		{
		case 1:
			_Banner.setHeaderText ("FIRST TURN");
			_Banner.setSubText ("Place Initial Ship");
			// Update Move ENUM Here
			break;
		case 2:
			_Banner.setHeaderText ("SECOND TURN");
			_Banner.setSubText ("Place Second Ship");
			// Update Move ENUM here
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Calls necessary methods to place player's ship
	/// </summary>
	public void uiPlaceInitialCity()
	{
		_Banner.setHeaderText ("SECOND TURN");
		_Banner.setSubText ("Place Initial City");
	}

	#endregion

	#region Infrastructure Building Methods
	/// <summary>
	/// Calls necessary methods to make it so player can build settlement
	/// </summary>
	public void uiBuildSettlement()
	{
		_Banner.setHeaderText ("BUILDING SETTLEMENT");
		_Banner.setSubText ("");
	}

	/// <summary>
	/// Calls necessary methods to make it so player can build City
	/// </summary>
	public void uiBuildCity()
	{
		_Banner.setHeaderText ("BUILDING CITY");
		_Banner.setSubText ("");
	}

	/// <summary>
	/// Calls necessary methods to make it so player can build City wall
	/// </summary>
	public void uiBuildCityWall()
	{
		_Banner.setHeaderText ("BUILDING CITY WALL");
		_Banner.setSubText ("");
	}

	/// <summary>
	/// Calls necessary methods to make it so player can build road
	/// </summary>
	public void uiBuildRoad()
	{
		_Banner.setHeaderText ("BUILDING ROAD");
		_Banner.setSubText ("");
	}

	/// <summary>
	/// Calls necessary methods to make it so player can build ship 
	/// </summary>
	public void uiBuildShip()
	{
		_Banner.setHeaderText ("BUILDING SHIP");
		_Banner.setSubText ("");
	}

	/// <summary>
	/// Calls necessary methods to make it so player can move ship
	/// </summary>
	public void uiMoveShip()
	{
		_Banner.setHeaderText ("MOVING SHIP");
		_Banner.setSubText ("");
	}


	#endregion

	#region Knight Methods
	/// <summary>
	/// Calls necessary methods so player can move knight
	/// </summary>
	public void uiMoveKnight()
	{
		_Banner.setHeaderText ("MOVING KNIGHT");
		_Banner.setSubText ("");
	}

	/// <summary>
	/// Calls necessary methods so player can displace knight
	/// </summary>
	public void uiDisplaceKnight()
	{
		_Banner.setHeaderText ("DISPLACING KNIGHT");
		_Banner.setSubText ("");
	}

	/// <summary>
	/// Calls necessary methods so player can upgrade knight
	/// </summary>
	public void uiUpgradeKnight()
	{
		_Banner.setHeaderText ("UPGRADING KNIGHT");
		_Banner.setSubText ("");
	}

	/// <summary>
	/// Calls necessary methods so player can activate knight
	/// </summary>
	public void uiActivateKnight()
	{
		_Banner.setHeaderText ("ACTIVATING KNIGHT");
		_Banner.setSubText ("");
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

	// Update is called once per frame
	void Update () {
		
	}
}
