using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Networking;
using UnityEngine.UI;

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


	#region Buildable Nodes Attributes
	/// <summary>
	/// List of vertices to highlight
	/// </summary>
	private List<Vertex> _vToHighlight;

	/// <summary>
	/// List of edges to highlight
	/// </summary>
	private List<Edge> _eToHighlight;

	/// <summary>
	/// List of hexes to highlight
	/// </summary>
	[SerializeField]
	private List<Hex> _hToHighlight;

	private bool highlightBoardPieces;

	/// <summary>
	/// The highlighted material applied to vertices/edges.
	/// </summary>
	[SerializeField]
	private Material highlightMaterial;

	private MoveType recentMove;

	#endregion

	// Use this for initialization
	void Awake () {
        
		_SmartPanel.gameObject.SetActive (true);
		_vToHighlight = new List<Vertex> ();
		_eToHighlight = new List<Edge> ();
		_hToHighlight = new List<Hex> ();
		recentMove = MoveType.NONE;

		_UIMoveManager = GetComponent<UIMoveManager> ();


        _CurrentPlayer = GameObject.Find(Network.player.ipAddress).GetComponent<Player>();

		// Set UIMoveManager's Current Player to this instance's current player attribute
		_UIMoveManager.setCurrentPlayer (_CurrentPlayer);


        if (_CurrentPlayer.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            // Hide ContextPanel and its contents on startup
            setContextPanelChildren();
            _ContextPanel.gameObject.SetActive(false);
            _VertexButtonsPanel.gameObject.SetActive(false);
            _EdgeButtonsPanel.gameObject.SetActive(false);

			// Hide PlayerPanels information
			foreach (Transform panel in _PlayerInfosPanel) 
			{
				panel.gameObject.SetActive (false);
			}

			// Get the GameManager Component off of the CurrentPlayer object
			_GameManager = _CurrentPlayer.GetComponent<GameManager>();

			// Set the Trade Attributes to NONE. And the Maritime panel active to false
			_FromResource = Enums.ResourceType.NONE;
			_ToResource = Enums.ResourceType.NONE;
			_MaritimeTradePanel.gameObject.SetActive (false);


			// Set the Dice Roll panel to true at start. Only shows the rollDice button at the right turns though
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
		bool check3 = _CurrentPlayer.getMoveType () == MoveType.SPECIAL;
		UIDiceRollPanel diceButton = _DiceRollPanel.GetComponent<UIDiceRollPanel> ();


		if (check1 && check2 && !check3) {
			diceButton.showRollButton(true);
		} 

		else 
		{
			diceButton.showRollButton(false);
		}
	}

	#endregion

	#region Smart Panel methods

	/// <summary>
	/// Toggles the highlightBoardPieces attribute. 
	/// Method for the BuildableNodes button alone
	/// </summary>
	public void toggleHighlight()
	{
		highlightBoardPieces = !highlightBoardPieces;

		updateHighlight ();

	}

	/// <summary>
	/// Updates the pieces to be highlighted if the toggle is true
	/// else, it hides all highlighted pieces again
	/// </summary>
	private void updateHighlight()
	{
		if (highlightBoardPieces == false)
			highlight(false);
		
		// If the recentMoveType has yet to change, there is no need to change the highlighted pieces
		//if (recentMove == _CurrentPlayer.getMoveType ()) return;

		// If  the highlightBoardPieces attribute is true, then highlight new set of elements
		if (highlightBoardPieces) 
		{
			highlight (false);
			getHighlightList ();
			//recentMove = _CurrentPlayer.getMoveType ();
			highlight (true);
		} 
	
	}


	/// <summary>
	/// Highlights the populated vertex list or edge list (whichever is actively populated)
	/// </summary>
	private void highlight(bool p_State)
	{
		if (_vToHighlight.Count > 0) 
		{
			foreach (Vertex v in _vToHighlight) 
			{
				v.GetComponent<Renderer> ().enabled = p_State;
				v.GetComponent<Renderer> ().material = highlightMaterial;
			}
		}

		else if (_eToHighlight.Count > 0) 
		{
			foreach (Edge e in _eToHighlight) 
			{
				e.GetComponent<Renderer> ().enabled = p_State;
				e.GetComponent<Renderer> ().material = highlightMaterial;
			}
		}


		// Special case of highlighting. Rather than changing material, modifies the text value
		// above the hex based on the parameter of this method
		else if (_hToHighlight.Count > 0) 
		{
			foreach (Hex h in _hToHighlight) 
			{
				h.hexVal.isHighlighted = p_State;
			}
		}
	}
		


	/// <summary>
	/// Fills the vToHighlight, or eToHilghlight list with the vertices/edges corresponding to the Player's current
	/// move type
	/// </summary>
	private void getHighlightList()
	{

		Buildable _build = new Buildable ();

		// Clear both lists initially. In order for highlight() to check against if either one is empty
		_vToHighlight.Clear();
		_eToHighlight.Clear ();
		_hToHighlight.Clear ();

		switch (_CurrentPlayer.getMoveType()) 
		{
		case Enums.MoveType.ACTIVATE_KNIGHT:
			_vToHighlight = _build.buildableActivateKnight (_CurrentPlayer.resources, _CurrentPlayer.getColor());
			break;
		case Enums.MoveType.BUILD_CITY:
			_vToHighlight = _build.buildableBuildCity(_CurrentPlayer.getResources(), _CurrentPlayer.getGamePieces(), _CurrentPlayer.getColor());
			break;
		case Enums.MoveType.BUILD_CITY_WALL:
			_vToHighlight = _build.buildableBuildCityWall (_CurrentPlayer.getResources(), _CurrentPlayer.getCityWallCount (), _CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.BUILD_KNIGHT:
			_vToHighlight = _build.buildableBuildKnight (_CurrentPlayer.getResources (), _CurrentPlayer.getGamePieces (), _CurrentPlayer.getColor ());
			break;
		// Edges to be Highlighted
		case Enums.MoveType.BUILD_ROAD:
			_eToHighlight = _build.buildableBuildRoad (_CurrentPlayer.getResources (), _CurrentPlayer.getGamePieces (), _CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.BUILD_SETTLEMENT:
			_vToHighlight = _build.buildableBuildSettlement (_CurrentPlayer.getResources (), _CurrentPlayer.getGamePieces (), _CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.BUILD_SHIP:
			_eToHighlight = _build.buildableBuildShip (_CurrentPlayer.getResources (), _CurrentPlayer.getGamePieces (), _CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.CHASE_ROBBER:
			_vToHighlight = _build.buildableChaseRobber (_CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.DISPLACE_KNIGHT:
			_vToHighlight = _build.buildableDisplaceKnight (_CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.FISH_5:
			_eToHighlight = _build.buildableFishRoad (_CurrentPlayer.numFish, _CurrentPlayer.getGamePieces (), _CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.MOVE_KNIGHT:
			_vToHighlight = _build.buildableMoveKnight (_CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.MOVE_SHIP:
			_eToHighlight = _build.buildableMoveShip (_CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.PLACE_INITIAL_CITY:
			_vToHighlight = _build.buildablePlaceInitialTownPiece ();
			break;
		case Enums.MoveType.PLACE_INITIAL_ROAD:
			_eToHighlight = _build.buildablePlaceInitialRoad (_CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.PLACE_INITIAL_SETTLEMENT:
			_vToHighlight = _build.buildablePlaceInitialTownPiece ();
			break;
		case Enums.MoveType.PLACE_INITIAL_SHIP:
			_eToHighlight = _build.buildablePlaceInitialShip (_CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.UPGRADE_KNIGHT:
			_vToHighlight = _build.buildableUpgradgeKnight (_CurrentPlayer.getResources (), _CurrentPlayer.getDevFlipChart (), _CurrentPlayer.getGamePieces (), _CurrentPlayer.getColor ());
			break;
		case Enums.MoveType.SPECIAL:
			if (Enums.Special.STEAL_RESOURCES_ROBBER == _CurrentPlayer.getSpecial ()) 
			{
				_vToHighlight = _build.buildableCanStealRobber (_CurrentPlayer.getColor ());
			}
			if (Enums.Special.STEAL_RESOURCES_PIRATE == _CurrentPlayer.getSpecial ()) 
			{
				_eToHighlight = _build.buildableCanStealPirate (_CurrentPlayer.getColor ());
			}
			if (Enums.Special.MOVE_ROBBER == _CurrentPlayer.getSpecial ()) 
			{
				_hToHighlight = _build.buildableCanMoveRobber ();
			}
			if (Enums.Special.MOVE_PIRATE == _CurrentPlayer.getSpecial ()) 
			{
				_hToHighlight = _build.buildableCanMovePirate ();
			}
			if (Enums.Special.CHOOSE_DESTROYED_CITY == _CurrentPlayer.getSpecial ()) 
			{
				_vToHighlight = _build.buildableCanDestroyCity (_CurrentPlayer.getColor ());
			}
			if (Enums.Special.CHOOSE_METROPOLIS == _CurrentPlayer.getSpecial ()) 
			{
				_vToHighlight = _build.buildableCanChooseMetropolis (_CurrentPlayer.getColor ());
			}
			break;
		case Enums.MoveType.NONE:
			highlight (false);
			break;
		default:
			break;
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
		/*
		Transform panel = _PlayerInfosPanel.GetChild (0);
		UIPlayerInfoPanel infoPanel = panel.GetComponent<UIPlayerInfoPanel> ();

		infoPanel.uiUpdate (_CurrentPlayer);
		*/


		// Keep track of index to assign a player to a PlayerInfoPanel
		int pIndex = 0;

		// Get the list of Player names
		List<Player> allPlayers = GameManager.instance.players;

		// Instantiate a Player object to loop through the list of GameManager.players
		Player _P;

		//Debug.Log ("Players Count: " + allPlayers.Count);
		// Loop through each GameManager.Players
		// Assign the Player to the Panel

		foreach (Transform panel in _PlayerInfosPanel) 
		{
			// If we increment further than there are players in game, break immediately
			if (pIndex+1 > allPlayers.Count)
				break;


			// set _P to be the player at the particular index in iteration
			_P = GameManager.instance.players[pIndex];

			// Make the panel active
			panel.gameObject.SetActive (true);

			// Update the PlayerInfoPanel with the player _P info
			panel.GetComponent<UIPlayerInfoPanel> ().uiUpdate (_P);

			// Increment pIndex
			pIndex++;
		}
	}


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


	#region Setters and Getters

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

	public Player getCurrentPlayer()
	{
		return _CurrentPlayer;
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


				updateHighlight ();

            }
        }
	
	}
    
}
