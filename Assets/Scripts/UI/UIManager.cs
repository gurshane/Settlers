using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

/// <summary>
/// Handles update and display of all Player attributes to HUD
/// </summary>
public class UIManager : MonoBehaviour {


	#region Player instance and Attributes

	/// <summary>
	/// The player which this UI displays the info of
	/// </summary>
	private Player _CurrentPlayer;

	#endregion


	#region UI Panels

	/// <summary>
	/// Player Specific : Panel containing player commodities UI elements
	/// </summary>
	[SerializeField]
	private Transform _CommoditiesPanel;

	/// <summary>
	/// Player Specific : Panel containing player resources UI elements
	/// </summary>
	[SerializeField]
	private Transform _ResourcesPanel;

	/// <summary>
	/// Panel containing UI elements with info on each player
	/// </summary>
	[SerializeField]
	private Transform _PlayerInfosPanel;

	/// <summary>
	/// Panel containing info of which player's turn it is and the name of the current player
	/// </summary>
	[SerializeField]
	private Transform _CurrentPlayerPanel;

	/// <summary>
	/// Panel containing Barbarian slider
	/// </summary>
	[SerializeField]
	private Transform _BarbarianPanel;

	#endregion



	// Use this for initialization
	void Start () {

		setPlayerInfoPanels();
	}




	#region Private Methods
	/// <summary>
	/// Updates the resources of the currentPlayer to be displayed on the UI
	/// </summary>
	private void updateResources()
	{
		foreach (Transform resource in _ResourcesPanel)
		{
			resource.GetComponent<UIElement> ().uiUpdate (_CurrentPlayer);
		}
	}

	/// <summary>
	/// Updates the commodities of the currentPlayer to be displayed on the UI
	/// </summary>
	private void updateCommodities()
	{
		foreach (Transform commodity in _CommoditiesPanel)
		{
			commodity.GetComponent<UIElement> ().uiUpdate (_CurrentPlayer);
		}
	}

	private void updatePlayerInfoPanels ()
	{
		// Keep track of index to assign a player to a PlayerInfoPanel
		int pIndex = 0;

		// Get the list of Player names
		List<string> pNames = GameManager.getPlayerNames ();

		// Instantiate a Player object to loop through the list of GameManager.players
		Player _P;

		// Loop through each GameManager.Players
		// Assign the Player to the Panel
		foreach (Transform panel in _PlayerInfosPanel) 
		{
			// GUsing pIndex to get the corresponding Player username
			// Use that string to getPlayer 
			_P = GameManager.getPlayer ( pNames [pIndex] );

			// Update the PlayerInfoPanel with the player _P info
			panel.GetComponent<UIPlayerInfoPanel> ().uiUpdate (_P);

			pIndex++;
		}
	}

	/// <summary>
	/// Assigns each available Player to each available PlayerInfoPanel
	/// </summary>
	private void setPlayerInfoPanels()
	{
		// Keep track of index to assign a player to a PlayerInfoPanel
		int pIndex = 0;

		// Get the list of Player names
		List<string> pNames = GameManager.getPlayerNames ();

		// Instantiate a Player object to loop through the list of GameManager.players
		Player _P;

		// Loop through each GameManager.Players
		// Assign the Player to the Panel
		foreach (Transform panel in _PlayerInfosPanel) 
		{
			// GUsing pIndex to get the corresponding Player username
			// Use that string to getPlayer 
			_P = GameManager.getPlayer ( pNames [pIndex] );

			// Instantiate a PlayerInfoPanel with the player _P
			panel.GetComponent<UIPlayerInfoPanel> ().instantiate (_P);

			pIndex++;
		}
	}

	#endregion
		

	// Update is called once per frame
	void Update () {
		
	}
}
