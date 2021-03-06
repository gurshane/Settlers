﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfoPanel : UIElement {



	#region Private Attributes
	[SerializeField]
	private UIManager _UIM;
	private Player _CurrentPlayer;

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
	/// Image showing the colour of the player
	/// </summary>
	[SerializeField]
	private Image _FillCircle;

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

		// Update color of the fill circle
		updateFillCircleColor (p_Player);

		// Get Player's devChart
		int[] devChart = p_Player.getDevFlipChart ();

		// Loop through devChart array, and update the corresponding Images on the UI.
		for (int x = 0; x < devChart.Length; x++) 
		{
			float floatVal = (float)devChart [x];
			// Divide by 4 because fillAmount ranges from 0 to 1
			// There are 4 stages of development, not including the 0th stage.
			_DevChartImages [x].fillAmount = floatVal/4f;
			if (devChart[x] == 5)
				changeColor (_DevChartImages[x]);
		}
	}

	public void changeColor(Image p_Image)
	{

		if (p_Image == _TradeProgressImage) 
		{
			p_Image.color = Color.yellow;
		}

		if (p_Image == _PoliticsProgressImage) 
		{
			p_Image.color = Color.blue;
		}

		if (p_Image == _ScienceProgressImage) 
		{
			p_Image.color = Color.green;
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


	public void updateFillCircleColor(Player p_Player)
	{
		// Get the color of the player from its Highlighter Component
		Color playerColor = enumToColor( p_Player.getColor() );

		_FillCircle.color = playerColor;
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

	// Update is called once per frame
	void Update () {
		
	}
}
