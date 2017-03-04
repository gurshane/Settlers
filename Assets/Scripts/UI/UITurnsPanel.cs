using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// User interface displaying panel showing what turn it is, and what pieces to place
/// </summary>
public class UITurnsPanel : UIElement {

	/// The text component attached to this instance saying whether it is the first turn
	/// </summary>
	[SerializeField]
	private Text _FirstTurnText;

	/// <summary>
	/// The text component attached to this instance, saying what piece to place
	/// </summary>
	[SerializeField]
	private Text _PlacementText;


	// Use this for initialization
	void Start () {
		
		// Get the text components to be modified during updates
		_FirstTurnText = transform.GetChild (0).GetComponent<Text>();
		_PlacementText = transform.GetChild (1).GetComponent<Text>();
	}

	/// <summary>
	/// Updates the UI Element
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public override void uiUpdate (Player p_Player)
	{
		// Get the Highlighter component from the player parameter
		HighLighter _PlayerHighlighter = p_Player.GetComponent<HighLighter> ();

		// Check for if it is the first turn or second turn. Respond accordingly
		updateFirstTurnText(_PlayerHighlighter);

	}


	/// <summary>
	/// Updates the first turn text based on whether it is the First Turn or not
	/// and prompts player to place a piece
	/// If it is Second Turn, clear the PlacementText.text as well
	/// </summary>
	/// <param name="p_Highlighter">P highlighter.</param>
	private void updateFirstTurnText(HighLighter p_Highlighter)
	{
		if (p_Highlighter.firstTurn == true) {
			_FirstTurnText.text = "  FIRST TURN";
			updatePlacementText (p_Highlighter);
		}
		else 
		{
			_FirstTurnText.text = "  SECOND TURN";
			_PlacementText.text = "";
		}
	}

	/// <summary>
	/// Updates the placement text to prompt placing a settlement or route depending on turn
	/// </summary>
	/// <param name="p_Highlighter">P highlighter.</param>
	private void updatePlacementText(HighLighter p_Highlighter)
	{
		if (p_Highlighter.placedFirstSettlement == false) {

			_PlacementText.text = "  - Place First Settlement";
		} 
		else 
		{
			_PlacementText.text = "  - Place First Road/Boat";
		}
	}
		
}
