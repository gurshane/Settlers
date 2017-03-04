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
	}

	/// <summary>
	/// Updates the UI Element
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public override void uiUpdate(Player p_Player)
	{
		// If the player name is null, return
		if (isStringNull(p_Player.getUserName())) return;

		// Update UI Text to display the Player's name
		_PlayerName.text = "\"" + p_Player.getUserName() + "\"";

		// Update UI Image to display appropriate colour
		updateIconColor(p_Player);
	}
		
	/// <summary>
	/// Updates the color of the icon using the Player's highlighter component
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public void updateIconColor(Player p_Player)
	{
		//Color playerHighlighter = p_Player.GetComponent<HighLighter> ().myColor;
		Image playerIconFillImage = _PlayerIcon.transform.GetChild (0).GetComponent<Image> ();
		//playerIconFillImage.color =  
	}
}
