using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Text User Interface Element used to display a Hex's value
/// </summary>
public class UIHexValue : MonoBehaviour {

	/// <summary>
	/// The camera which this UI element updates to constantly face
	/// </summary>
	private Camera _Camera;

	/// <summary>
	/// Text component needed to display the Hex's value
	/// </summary>
	[SerializeField]
	private TextMesh _HexNumber;

	/// <summary>
	/// The hex which this UI element displays the value for
	/// </summary>
	[SerializeField]
	private Hex _Hex;


	// Use this for initialization
	void Start () {

		_Camera = Camera.main;
		_HexNumber = GetComponent<TextMesh>();
	}

	/// <summary>
	/// Update the hex value to be displayed by this UI element
	/// Move this UI element to the Hex it is displaying for
	/// </summary>
	public void updateValue()
	{
		_HexNumber.text = "" + _Hex.hexNumber;

	}

	public void updatePosition()
	{
		Vector3 newPos = new Vector3 ();

		newPos.x = _Hex.transform.position.x;
		newPos.y = _Hex.transform.position.y + 40f;
		newPos.z = _Hex.transform.position.z;

		transform.position = newPos;
	}

	/// <summary>
	/// Billboards the UI element to always face the camera no matter where it is
	/// </summary>
	private void faceCamera()
	{
		Vector3 worldPosition = transform.position + _Camera.transform.rotation * Vector3.forward;
		Vector3 worldUp = _Camera.transform.rotation * Vector3.up;

		transform.LookAt (worldPosition, worldUp);
	}

	// Update is called once per frame
	void Update () {

		faceCamera ();
		updatePosition ();
		updateValue ();
        if(_Hex)
        {
            if(_Hex.hexType == Enums.HexType.DESERT)
            {
                Destroy(gameObject);
            }
        }
	}
}
