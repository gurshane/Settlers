using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPortSymbol : MonoBehaviour {

	private Camera _Camera;

	private Sprite _Sprite;

	// Use this for initialization
	void Start () {
		_Camera = Camera.main;
		_Sprite = GetComponent<Sprite> ();
		
	}

	private void updateSpriteColor()
	{
		
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
		
	}
}
