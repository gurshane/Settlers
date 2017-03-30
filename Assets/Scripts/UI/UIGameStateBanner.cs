using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// User interface displaying panel showing what the current game state is and what can
/// be done in that state
/// </summary>
public class UIGameStateBanner : UIElement {

	/// The text component attached to this instance saying what the current state
	/// allows for
	/// </summary>
	[SerializeField]
	private Text _HeaderText;

	/// <summary>
	/// The text component attached to this instance, saying what piece to place
	/// </summary>
	[SerializeField]
	private Text _SubText;


	// Use this for initialization
	void Start () {
		
		// Get the text components to be modified during updates
		_HeaderText = transform.GetChild (0).GetComponent<Text>();
		_SubText = transform.GetChild (1).GetComponent<Text>();
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
	/// Sets the header text to parameter value
	/// </summary>
	/// <param name="p_String">P string.</param>
	public void setHeaderText(string p_String)
	{
		_HeaderText.text = p_String;
	}

	/// <summary>
	/// Sets the sub text value to parameter value
	/// </summary>
	/// <param name="p_String">P string.</param>
	public void setSubText(string p_String)
	{
		_SubText.text = p_String;
	}

	/// <summary>
	/// Updates the first turn text based on whether it is the First Turn or not
	/// and prompts player to place a piece
	/// If it is Second Turn, clear the PlacementText.text as well
	/// </summary>
	/// <param name="p_Highlighter">P highlighter.</param>
	private void updateFirstTurnText(HighLighter p_Highlighter)
	{
		//if (p_Highlighter.firstTurn == true) {
		//	_TurnText.text = "  FIRST TURN";
		//	updatePlacementText (p_Highlighter);
		//}
		//else if (p_Highlighter.firstTurn == false) {
		//	_TurnText.text = "";
		//	_PlacementText.text = "  WAITING FOR OTHER PLAYERS";
		//} 

		//if (p_Highlighter.secondTurn == true) 
		//{
		//	_TurnText.text = "  SECOND TURN";
		//	_PlacementText.text = "";
		//}
	}

	/// <summary>
	/// Updates the placement text to prompt placing a settlement or route depending on turn. 
	/// Only displays one of the prompts at at time
	/// </summary>
	/// <param name="p_Highlighter">P highlighter.</param>
	private void updatePlacementText(HighLighter p_Highlighter)
	{
		//// If first settlement not placed, print a prompt saying so
		//if (p_Highlighter.placedFirstSettlement == false) 
		//{
		//	_PlacementText.text = "  - Place First Settlement";
		//	return;
		//} 

		//// If first road/boat not placed, print a prompt saying so
		//if (p_Highlighter.placedFirstEdge == false) 
		//{
		//	_PlacementText.text = "  - Place First Road/Boat";
		//	return;
		//}

		//// If first city not placed, print a prompt saying so
		//if (p_Highlighter.placedFirstCity == false) 
		//{
		//	_PlacementText.text = "  - Place First City";
		//	return;
		//}

		//// If second road/boat not placed, print a prompt saying so
		//if (p_Highlighter.placedSecondEdge == false) 
		//{
		//	_PlacementText.text = "  - Place Second Road/Boat";
		//	return;
		//}
	}
		
}
