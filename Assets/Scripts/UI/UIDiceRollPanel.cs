using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// User interface for the dice roll panel, shown at the beginning of each player's turn
/// </summary>
public class UIDiceRollPanel : MonoBehaviour {

	/// <summary>
	/// Text displaying FirstDie's value
	/// </summary>
	[SerializeField]
	private Text _FirstDieText;

	/// <summary>
	/// Text displaying SecondDie's value
	/// </summary>
	[SerializeField]
	private Text _SecondDieText;

	/// <summary>
	/// Text displaying EventDie's value
	/// </summary>
	[SerializeField]
	private Text _EventDieText;

	/// <summary>
	/// Button which allows the player who's turn it is to roll dice
	/// </summary>
	[SerializeField]
	private Transform _DiceRollButton;

	// Use this for initialization
	void Start () {
		
	}


	/// <summary>
	/// Updates the Dice Roll panel to display the value of the dice after a roll
	/// Renders the roll button inactive afterwards, so another roll cannot occur until the next player's turn
	/// </summary>
	/// <param name="p_GameManager">P game manager.</param>
	public void updatePanel(GameManager p_GameManager)
	{
		_FirstDieText.text = "" + p_GameManager.getFirstDie();
		_SecondDieText.text = "" + p_GameManager.getSecondDie();
		_EventDieText.text = convert (p_GameManager.getEventDie() );

		// After updating values, deactivate Roll button (Uncomment Later)
		//_DiceRollButton.gameObject.SetActive (false);
	}

	/// <summary>
	/// Displays the Roll button if this particular player is the one who's turn it is
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public void showRollButton(Player p_Player)
	{
		// If this player is the one who's turn it is:
		_DiceRollButton.gameObject.SetActive(true);
	}

	/// <summary>
	/// Convert the specified p_EventDieEnum into a string
	/// </summary>
	/// <param name="p_EventDieEnum">P event die enum.</param>
	private string convert(Enums.EventDie p_EventDieEnum)
	{
		string rString = "";

		switch (p_EventDieEnum) 
		{
		case Enums.EventDie.BARBARIAN:
			rString = "Barbarian";
			break;
		case Enums.EventDie.POLITICS:
			rString = "Politics";
			break;
		case Enums.EventDie.SCIENCE:
			rString = "Science";
				break;
		case Enums.EventDie.TRADE:
			rString = "Trade";
			break;
		default:
			break;
		}

		return rString;
	}

}
