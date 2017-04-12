using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// User interface element containing the name of the player this UI instance is displaying for.
/// </summary>
public class UIMyPlayerPanel : UIElement {

	#region Private Attributes

	/// The text component attached to this instance
	/// </summary>
	[SerializeField]
	private Text _PlayerName;

	/// The text component attached to this instance which displays player's current victory points
	/// </summary>
	[SerializeField]
	private Text _PlayerInTurnText;

	/// <summary>
	/// Image showing Player's color
	/// </summary>
	[SerializeField]
	private Image _PlayerIcon;

	#endregion

	// Use this for initialization
	void Start () {

		_PlayerName = transform.GetChild (0).GetComponent<Text>();
		_PlayerIcon = transform.GetChild (1).GetComponent<Image> ();
		_PlayerInTurnText = transform.GetChild (2).GetComponent<Text> ();
	}

	/// <summary>
	/// Updates the UI Element
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public override void uiUpdate(Player p_Player)
	{
        // If the player name is null, return
        //if (isStringNull(p_Player.getUserName())) return;

		p_Player.getColor ();
        // Update UI Text to display the Player's name
        //_PlayerName.text = "\"" + p_Player.getUserName() + "\"";

		_PlayerName.text = enumToString(p_Player.getColor());
		_PlayerInTurnText.text = "Now Making Move: " + enumToString(GameManager.instance.getCurrentPlayer().getColor());

		// Update UI Image to display appropriate colour
		updateIconColor(p_Player);
	}
		
	/// <summary>
	/// Updates the color of the icon using the Player's highlighter component
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public void updateIconColor(Player p_Player)
	{
		// Get the color of the player from its Highlighter Component
		Color playerColor = enumToColor( p_Player.getColor() );

		// Get the Fill image on the UI - 
		// the first child of the _PlayerIcon attribute of this instance of myPlayerPanel
		Image playerIconFillImage = _PlayerIcon.transform.GetChild (0).GetComponent<Image> ();

		// Set the Fill image color to the newly acquired color
		playerIconFillImage.color = enumToColor(GameManager.instance.getCurrentPlayer().getColor());
	}

	/// <summary>
	/// Convert the specified p_Color Enum into a Color for an image to use
	/// </summary>
	/// <param name="p_Color">P color.</param>
	private Color enumToColor(Enums.Color p_Color)
	{
		Color rColor = new Color (0, 0, 0);
		switch (p_Color) 
		{
		case Enums.Color.BLUE:
			rColor = Color.blue;
			break;
		case Enums.Color.ORANGE:
			rColor = Color.green;//new Color32(0xF6, 0xA1,0x09,0xFF);
			break;
		case Enums.Color.RED:
			rColor = Color.red;
			break;
		case Enums.Color.WHITE:
			rColor = Color.white;
			break;
		default:
			break;
		}

		return rColor;
	}

	/// <summary>
	/// Converts from Color enum to string
	/// </summary>
	/// <returns>The to string.</returns>
	/// <param name="p_Color">P color.</param>
	private string enumToString(Enums.Color p_Color)
	{
		string rString = "";
		switch (p_Color) 
		{
		case Enums.Color.BLUE:
			rString = "Blue Player";
			break;
		case Enums.Color.ORANGE:
			rString = "Green Player";
			break;
		case Enums.Color.RED:
			rString = "Red Player";
			break;
		case Enums.Color.WHITE:
			rString = "White Player";
			break;
		default:
			break;
		}

		return rString;
	}
}
