using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfoPanel : UIElement {

	#region Private Attributes

	/// <summary>
	/// The Text object showing player's name
	/// </summary>
	[SerializeField]
	private Text _PlayerName;

	/// <summary>
	/// Text object showing current player's victory points
	/// </summary>
	[SerializeField]
	private Text _VictoryPoints;

	/// <summary>
	/// Image showing Player's progress in Trade 
	/// </summary>
	[SerializeField]
	private Image _TradeProgressImage;

	/// <summary>
	/// Image showing Player's progress in Politics
	/// </summary>
	[SerializeField]
	private Image _PoliticsProgressImage;

	/// <summary>
	/// Image showing Player's progress in Science
	/// </summary>
	[SerializeField]
	private Image _ScienceProgressImage;

	/// <summary>
	/// Array containing player development chart information.
	/// Each index corresponds to a different area
	/// </summary>
	[SerializeField]
	private Image[] _DevChartImages;

	#endregion

	// Use this for initialization
	void Start () {

		setDevChart ();

	}
		
	/// <summary>
	/// Sets the PlayerName and VictoryPoints text for this instance
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public void instantiate(Player p_Player)
	{
        //_PlayerName.text = p_Player.getUserName ();
        //_PlayerName.text = "Hi";
		//_VictoryPoints.text = "" + p_Player.getVictoryCounts ();
	}

	/// <summary>
	/// Updates the UI element
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public override void uiUpdate(Player p_Player)
	{

		//Change the text displaying player name accordingly
		_PlayerName.text = enumToString (p_Player.getColor());


		// Update the victory points displayed for this player
		_VictoryPoints.text = "" + p_Player.getVictoryCounts ();

		// Get Player's devChart
		int[] devChart = p_Player.getDevFlipChart ();

		// Loop through devChart array, and update the corresponding Images on the UI.
		for (int x = 0; x < devChart.Length; x++) 
		{
			float floatVal = (float)devChart [x];
			// Divide by 4 because fillAmount ranges from 0 to 1
			// There are 4 stages of development, not including the 0th stage.
			_DevChartImages [x].fillAmount = floatVal/4f;
		}
	}

	/// <summary>
	/// Initialises DevChart attribute to have each of the progressImages as an element
	/// </summary>
	private void setDevChart()
	{
		_DevChartImages = new Image[3];
		_DevChartImages [0] = _TradeProgressImage;
		_DevChartImages [1] = _PoliticsProgressImage;
		_DevChartImages [2] = _ScienceProgressImage;
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

	// Update is called once per frame
	void Update () {
		
	}
}
