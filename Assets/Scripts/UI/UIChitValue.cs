using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Text User Interface Element used to display a Hex's value
/// </summary>
public class UIChitValue : MonoBehaviour {
    

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
	private Vertex _Vertex;

	private string _text;

	// Use this for initialization
	void Start () {

        

		_Camera = Camera.main;
		_HexNumber = GetComponent<TextMesh>();
        

		// On start, set display text to the hex's hexNumber
		_text = "" + _Vertex.chits;

       
	}

	/// <summary>
	/// Update the hex value to be displayed by this UI element
	/// Move this UI element to the Hex it is displaying for
	/// </summary>
	public void updateValue()
	{
		_text = "" + _Vertex.chits;
		_HexNumber.color = Color.white;
		_HexNumber.text = _text;
	}



	public void updatePosition()
	{
		Vector3 newPos = new Vector3 ();

		newPos.x = _Vertex.transform.position.x;
		newPos.y = _Vertex.transform.position.y + 20f;
		newPos.z = _Vertex.transform.position.z;

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

        if(_Vertex.chits == 0)
        {
            Destroy(gameObject);
        }

		faceCamera ();
		updatePosition ();
		updateValue ();
	}
}
