using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarbarianPanel : MonoBehaviour {

	/// <summary>
	/// Slider showing progress in Barabarian approach
	/// </summary>
	[SerializeField]
	private Slider _BarbarianSlider;

	// Use this for initialization
	void Start () {
		// Get slider component of this instance's first child
		_BarbarianSlider = transform.GetChild (0).GetComponent<Slider>();

	}

	private void updateValue()
	{
		_BarbarianSlider.value = GameManager.instance.getBarbarianPosition ();
	}

	// Update is called once per frame
	void Update () {
		updateValue ();
	}
}
