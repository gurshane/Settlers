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
	private GameManager _GameManager;

	private UIMoveManager _UIMoveManager;

	#region Player instance and Attributes

	/// <summary>
	/// The player which this UI displays the info of
	/// </summary>
	[SerializeField]
	private Player _CurrentPlayer;

	/// <summary>
	/// The player's highlighter component
	/// </summary>
	HighLighter _PlayerHighlighter;

	#endregion

	#region Trade Attributes
	/// <summary>
	/// Resource that Player proposes in a trade
	/// </summary>
	[SerializeField]
	private Enums.ResourceType _FromResource;

	/// <summary>
	/// Resource that Player hopes to receive from a trade
	/// </summary>
	[SerializeField]
	private Enums.ResourceType _ToResource;
	#endregion

	#region UI Panels

	/// <summary>
	/// The smart panel, located on screen right. Houses several buttons needed for gameplay
	/// </summary>
	[SerializeField]
	private Transform _SmartPanel;

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
	/// The panel displaying what the current game state is, and what the player can do
	/// with whichever game state enum they are in at the moment
	/// </summary>
	[SerializeField]
	private UIElement _GameStateBannerPanel;

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

	/// <summary>
	/// The dice roll panel displayed on the UI after a dice roll has occurred
	/// </summary>
	[SerializeField]
	private Transform _DiceRollPanel;

	/// <summary>
	/// The maritime trade panel displayed once it is SecondTurn
	/// </summary>
	[SerializeField]
	private Transform _MaritimeTradePanel;


    #endregion


	// Use this for initialization
	void Awake () {
        
		_SmartPanel.gameObject.SetActive (true);
		_UIMoveManager = GetComponent<UIMoveManager> ();


        _CurrentPlayer = GameObject.Find(Network.player.ipAddress).GetComponent<Player>();

		// Set UIMoveManager's Current Player to this instance's current player attribute
		_UIMoveManager.setCurrentPlayer (_CurrentPlayer);

		_PlayerHighlighter = _CurrentPlayer.GetComponent<HighLighter> ();

        if (_CurrentPlayer.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            // Hide ContextPanel and its contents on startup
            setContextPanelChildren();
            _ContextPanel.gameObject.SetActive(false);
            _VertexButtonsPanel.gameObject.SetActive(false);
            _EdgeButtonsPanel.gameObject.SetActive(false);


			// Get the GameManager Component off of the CurrentPlayer object
			_GameManager = _CurrentPlayer.GetComponent<GameManager>();

			// Set the Trade Attributes to NONE. And the Maritime panel active to false
			_FromResource = Enums.ResourceType.NONE;
			_ToResource = Enums.ResourceType.NONE;
			_MaritimeTradePanel.gameObject.SetActive (false);


			// Set the Dice Roll panel to false at start. Only shows during second turn
			_DiceRollPanel.gameObject.SetActive(true);
        }
    }

	#region Dice Roll Panel Methods

	/// <summary>
	/// Calls necessary methods to roll dice in game
	/// </summary>
	public void rollDice()
	{
		_CurrentPlayer.CmdDiceRoll ();
	}

	public void showDiceRollPanel()
	{
		bool check1 = GameManager.instance.getPlayerTurn () == _CurrentPlayer.getID ();
		bool check2 = GameManager.instance.getGamePhase () == GamePhase.PHASE_ONE;
		UIDiceRollPanel diceButton = _DiceRollPanel.GetComponent<UIDiceRollPanel> ();

		if (check1 && check2) {
			diceButton.showRollButton(true);
		} 

		else 
		{
			diceButton.showRollButton(false);
		}
	}

	#endregion


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
		// Update the CurrentPlayer panel using information from this instance's CurrentPlayer attributes
		_MyPlayerPanel.uiUpdate (_CurrentPlayer);
	}


	/// <summary>
	/// Updates the dice roll panel using _GameManager's dice information
	/// </summary>
	public void updateDiceRollPanel()
	{
		
		_DiceRollPanel.GetComponent<UIElement> ().uiUpdate(_CurrentPlayer);

	}

	/// <summary>
	/// Updates the panel stating whether it is the First Turn, and if so, what pieces to place
	/// </summary>
	public void updateTurnsPanel()
	{
		_GameStateBannerPanel.uiUpdate (_CurrentPlayer);

		// If it is second turn, set MaritimeTradePanel active to true
		//if (_PlayerHighlighter.secondTurn == true) _MaritimeTradePanel.gameObject.SetActive (true);
	}
		

	/// <summary>
	/// Updates the player info panels to be displayed on the UI
	/// </summary>

	public void updatePlayerInfoPanels ()
	{

		Transform panel = _PlayerInfosPanel.GetChild (0);
		UIPlayerInfoPanel infoPanel = panel.GetComponent<UIPlayerInfoPanel> ();

		infoPanel.uiUpdate (_CurrentPlayer);

		/*
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
		}*/
	}
	/*
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

	#endregion

	#region OnClick method
	/// <summary>
	/// Handles response for when a Vertex or Edge on the board is clicked
	/// </summary>
	public void showContextPanel(string p_TargetTag)
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
		

	#region Trade Methods

	public void maritimeTrade(string p_ToResource)
	{
		// Set ToResource from parameter info
		setToResource (p_ToResource);

		// Make the Maritime Trade using the FromResource and ToResource Attributes
		//_CurrentPlayer.GetComponent<HighLighter> ().makeMaritimeTrade (_FromResource, _ToResource);

		// Revert the trade attributes to NONE so no residual trades carry over to subsequent ones
		//_ToResource = Enums.ResourceType.NONE;
		//_FromResource = Enums.ResourceType.NONE;
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
	/// Gets the game manager attribute of this instance
	/// </summary>
	/// <returns>The game manager.</returns>
	public GameManager getGameManager()
	{
		return _GameManager;
	}

	/// <summary>
	/// Sets the current player attribute of this UIManager instance to the parameter
	/// </summary>
	/// <param name="p_CurrentPlayer">P current player.</param>
	public void setCurrentPlayer(Player p_CurrentPlayer)
	{
		_CurrentPlayer = p_CurrentPlayer;
	}

	/// <summary>
	/// Sets the FromResource attribute of this instance of UI manager that the Player wants to give in a trade
	/// </summary>
	public void setFromResource(string p_ResourceType)
	{
		
		switch (p_ResourceType) 
		{
		case "Brick":
			_FromResource = Enums.ResourceType.BRICK;
			break;
		case "Wool":
			_FromResource = Enums.ResourceType.WOOL;
			break;
		case "Grain":
			_FromResource = Enums.ResourceType.GRAIN;
			break;
		case "Lumber":
			_FromResource = Enums.ResourceType.LUMBER;
			break;
		case "Ore":
			_FromResource = Enums.ResourceType.ORE;
			break;
		}

	}

	/// <summary>
	/// Sets the ToResource attribute of this instance of UI manager that the Player wants to get from a trade
	/// This is Private, because it is only called when the actual trade with bank is called as well
	/// </summary>
	/// <param name="p_ResourceType">P resource type.</param>
	private void setToResource(string p_ResourceType)
	{
		switch (p_ResourceType) 
		{
		case "Brick":
			_ToResource = Enums.ResourceType.BRICK;
			break;
		case "Wool":
			_ToResource = Enums.ResourceType.WOOL;
			break;
		case "Grain":
			_ToResource = Enums.ResourceType.GRAIN;
			break;
		case "Lumber":
			_ToResource = Enums.ResourceType.LUMBER;
			break;
		case "Ore":
			_ToResource = Enums.ResourceType.ORE;
			break;
		}

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
            // Check whether _CurrentPlayer has been initialised before running these checks
			// These are all here for DEBUGGING only
			// Each of these methods must be placed in the appropriate class in final version
            if (!isCurrentPlayerNull())
            {
                updateResources();
                updateCommodities();
                updateMyPlayerPanel();
				updateDiceRollPanel ();
				updateTurnsPanel();

				updatePlayerInfoPanels ();

				showDiceRollPanel ();
            }
        }
	
	}
    
}
