using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;

/// <summary>
/// Handles update and display of all Player attributes to HUD
/// </summary>
public class UIManager : NetworkBehaviour {


	/// <summary>
	/// The game manager object
	/// </summary>
	[SerializeField]
	private GameObject _GameManager;

	#region Player instance and Attributes

	/// <summary>
	/// The player which this UI displays the info of
	/// </summary>
	[SerializeField]
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
	private UIElement _MyPlayerPanel;

	/// <summary>
	/// Panel containing Barbarian slider
	/// </summary>
	[SerializeField]
	private Transform _BarbarianPanel;

	/// <summary>
	/// The context panel which displays when a board vertex or edge is clicked
	/// </summary>
	[SerializeField]
	private Transform _ContextPanel;

	/// <summary>
	/// Contents within the ContextPanel. Parent of the VertexButtons and the EdgeButtons panels
	/// </summary>
	[SerializeField]
	private Transform _ContextContentPanel;

	[SerializeField]
	private Transform _VertexButtonsPanel;
	[SerializeField]
	private Transform _EdgeButtonsPanel;

    #endregion


	// Use this for initialization
	void Start () {
        
        _CurrentPlayer = GameObject.Find(Network.player.ipAddress).GetComponent<Player>();

        if (_CurrentPlayer.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            // Hide ContextPanel and its contents on startup
            setContextPanelChildren();
            _ContextPanel.gameObject.SetActive(false);
            _VertexButtonsPanel.gameObject.SetActive(false);
            _EdgeButtonsPanel.gameObject.SetActive(false);


            //_GameManager = GameObject.FindGameObjectWithTag("GameManager");
            //setPlayerInfoPanels();

        }
    }




    #region Update Methods
    /// <summary>
    /// Updates the resources of the currentPlayer to be displayed on the UI
    /// </summary>
    public void updateResources()
	{
		foreach (Transform resource in _ResourcesPanel)
		{
			resource.GetComponent<UIElement> ().uiUpdate (_CurrentPlayer);
		}
	}

	/// <summary>
	/// Updates the commodities of the currentPlayer to be displayed on the UI
	/// </summary>
	public void updateCommodities()
	{
		foreach (Transform commodity in _CommoditiesPanel)
		{
			commodity.GetComponent<UIElement> ().uiUpdate (_CurrentPlayer);
		}
	}

	/// <summary>
	/// Updates the current player panel to be displayed on the UI
	/// </summary>
	public void updateMyPlayerPanel()
	{
		// Update the CurrentPlayer panel using information from this instance's CurrentPlayer attribute
		_MyPlayerPanel.uiUpdate (_CurrentPlayer);
	}

	/// <summary>
	/// Updates the player info panels to be displayed on the UI
	/// </summary>
	/*
	public void updatePlayerInfoPanels ()
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
	public void setPlayerInfoPanels()
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
	}*/

	/// <summary>
	/// Handles response for when a Vertex or Edge on the board is clicked
	/// </summary>
	public void boardClicked(string p_TargetTag)
	{
		// Set Context Panel to Active
		_ContextPanel.gameObject.SetActive (true);

		// Displace Panel based on Mouse position when the click occurred
		_ContextPanel.position = Input.mousePosition;
		_ContextPanel.position += offsetContextPanel ();

		// Depending on Target's tag, display either the list of options for a Vertex or an Edge
		switch (p_TargetTag) 
		{
		case "Vertex":
			_VertexButtonsPanel.gameObject.SetActive (true);
			_EdgeButtonsPanel.gameObject.SetActive (false);
			break;
		case "Edge":
			_VertexButtonsPanel.gameObject.SetActive (false);
			_EdgeButtonsPanel.gameObject.SetActive (true);
			break;
		default:
			break;
		}
	}

	#endregion

	// DEBUGGING
	/// <summary>
	/// Checks if the _CurrentPlayer has been instantiated or not
	/// </summary>
	/// <returns><c>true</c>, if player check was nulled, <c>false</c> otherwise.</returns>
	private bool isCurrentPlayerNull()
	{
		return (_CurrentPlayer == null);
	}
		
	private Vector3 offsetContextPanel()
	{
		Vector3 rVec = new Vector3 (0, 0, 0);

		if (Input.mousePosition.y > 4 * (Screen.height/5) ) 
		{
			rVec.y = -80f;
		}

		else
		{
			rVec.y = 80f;
		}

		return rVec;
	}


	#region Setters

	/// <summary>
	/// Sets the current player attribute of this UIManager instance to the parameter
	/// </summary>
	/// <param name="p_CurrentPlayer">P current player.</param>
	public void setCurrentPlayer(Player p_CurrentPlayer)
	{
		_CurrentPlayer = p_CurrentPlayer;
	}

	/// <summary>
	/// Sets the context panel contents for selecting options of either a vertex or edge
	/// </summary>
	private void setContextPanelChildren()
	{
		_VertexButtonsPanel = _ContextContentPanel.GetChild (0);
		_EdgeButtonsPanel = _ContextContentPanel.GetChild (1);
	}

	#endregion

	// Update is called once per frame
	void Update () {

        // DEBUGGING -----
        if (_CurrentPlayer.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            // Check whether _CurrentPlayer has been initialised
            if (!isCurrentPlayerNull())
            {
                updateResources();
                updateCommodities();
                updateMyPlayerPanel();
            }
        }
	
	}
    
}
