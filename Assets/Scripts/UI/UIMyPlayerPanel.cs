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
	private Text _VictoryPoints;

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
		_VictoryPoints = transform.GetChild (2).GetComponent<Text> ();
	}

	/// <summary>
	/// Updates the UI Element
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public override void uiUpdate(Player p_Player)
	{
        // If the player name is null, return
        //if (isStringNull(p_Player.getUserName())) return;

        // Update UI Text to display the Player's name
        //_PlayerName.text = "\"" + p_Player.getUserName() + "\"";
        _PlayerName.text = "Hi";
		_VictoryPoints.text = "  Victory Points: " + p_Player.victoryPoints;

		// Update UI Image to display appropriate colour
		//updateIconColor(p_Player);
	}
		
	/// <summary>
	/// Updates the color of the icon using the Player's highlighter component
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public void updateIconColor(Player p_Player)
	{
		// Get the color of the player from its Highlighter Component
		Color playerColor = convert( p_Player.GetComponent<HighLighter> ().myColor );

		// Get the Fill image on the UI - 
		// the first child of the _PlayerIcon attribute of this instance of myPlayerPanel
		Image playerIconFillImage = _PlayerIcon.transform.GetChild (0).GetComponent<Image> ();

		// Set the Fill image color to the newly acquired color
		playerIconFillImage.color = playerColor;
	}

	/// <summary>
	/// Convert the specified p_Color Enum into a Color for an image to use
	/// </summary>
	/// <param name="p_Color">P color.</param>
	private Color convert(Enums.Color p_Color)
	{
		Color rColor = new Color (0, 0, 0);
		switch (p_Color) 
		{
		case Enums.Color.BLUE:
			rColor = Color.blue;
			break;
		case Enums.Color.ORANGE:
			rColor = new Color32(0xF6, 0xA1,0x09,0xFF);
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
}
