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

	#endregion

	// Use this for initialization
	void Start () {

		_PlayerName = transform.GetChild (0).GetComponent<Text>();
	}

	/// <summary>
	/// Updates the UI Element
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public override void uiUpdate(Player p_Player)
	{
		if (isStringNull(p_Player.getUserName())) return;

		// Update UI Text to display the Player's name
		_PlayerName.text = "\"" + p_Player.getUserName() + "\"";

	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
