using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFishSymbol : MonoBehaviour {

	/// <summary>
	/// The camera which this UI element updates to constantly face
	/// </summary>
	private Camera _Camera;


	/// <summary>
	/// The vertex who's information is used to determine where the fish symbol goes
	/// </summary>
	[SerializeField]
	private Vertex _Vertex;

	// Use this for initialization
	void Start () {

		_Camera = Camera.main;
	}

	/// <summary>
	/// Update the hex value to be displayed by this UI element
	/// Move this UI element to the Hex it is displaying for
	/// </summary>
	public void updateValue()
	{

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

		faceCamera ();
		updatePosition ();
	}
}
