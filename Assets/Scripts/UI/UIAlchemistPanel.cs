using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class UIAlchemistPanel : MonoBehaviour {

	[SerializeField]
	private UIManager _UIM;
	private Player _CurrentPlayer;

	[SerializeField]
	private Slider _YellowDieSlider;
	[SerializeField]
	private Text _YellowDieText;

	[SerializeField]
	private Slider _RedDieSlider;
	[SerializeField]
	private Text _RedDieText;

	private Button _SubmitButton;


	// Use this for initialization
	void Start () {
		_CurrentPlayer = _UIM.getCurrentPlayer ();
	}


	public void updateText(int p_DieType)
	{
		switch (p_DieType) 
		{
		case 0:
			_YellowDieText.text = "" + _YellowDieSlider.value;
			break;
		case 1:
			_RedDieText.text = "" + _RedDieSlider.value;
			break;
		default:
			break;
		}
	}

	//Rolls the Dice, but in an Alchemist kind of way
	public void rollAlchemistDice()
	{
		_CurrentPlayer.CmdAlchemistRoll ((int)_YellowDieSlider.value, (int)_RedDieSlider.value);
		_CurrentPlayer.setMoveType (MoveType.NONE, _CurrentPlayer.getID());

		_CurrentPlayer.removeProgressCard (ProgressCardName.ALCHEMIST, _CurrentPlayer.getID ());
	}


}
