using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	public void rollAlchemistDice()
	{
		//_CurrentPlayer.cmdAlchemistRoll(_YellowDieSlider.value, _RedDieSlider.value);
	}


	
	// Update is called once per frame
	void Update () {
		
	}
}
