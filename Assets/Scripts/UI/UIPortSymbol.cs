using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPortSymbol : MonoBehaviour {

	private Camera _Camera;

	/// <summary>
	/// The sprite component of this instance
	/// </summary>
	[SerializeField]
	private SpriteRenderer _SpriteRenderer;

	/// <summary>
	/// The vertex who's information is used to determine the port trade type and modify the sprite color with
	/// </summary>
	[SerializeField]
	private Vertex _Vertex;

	// Use this for initialization
	void Start () {
		_Camera = Camera.main;
		_SpriteRenderer = GetComponent<SpriteRenderer> ();
		
	}

	private void updateSpriteColor()
	{
		_SpriteRenderer.color = convert (_Vertex.portType);
	}

	/// <summary>
	/// Updates the position of the port symbol
	/// </summary>
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


	/// <summary>
	/// Convert the specified p_PortType into a Color for the port sprite to use
	/// </summary>
	/// <param name="p_EventDieEnum">P event die enum.</param>
	private Color convert(Enums.PortType p_PortType)
	{
		Color rColor = new Color32();

		switch (p_PortType) 
		{
		case Enums.PortType.BRICK:
			//D28D1CFF
			rColor = new Color32(0xD2, 0x8D,0x1C,0xFF);
			break;
		case Enums.PortType.GENERIC:
			rColor = Color.white;
			break;
		case Enums.PortType.GRAIN:
			//F0DE4FFF
			rColor = new Color32(0xF0, 0xDE,0x4F,0xFF);
			break;
		case Enums.PortType.LUMBER:
			//11542CFF
			rColor = new Color32(0x11, 0x54,0x2C,0xFF);
			break;
		case Enums.PortType.ORE:
			//577786FF
			rColor = new Color32(0x57, 0x77,0x86,0xFF);
			break;
		case Enums.PortType.WOOL:
			//2FB651FF
			rColor = new Color32(0x2F, 0xB6,0x51,0xFF);
			break;
		default:
			break;
		}

		return rColor;
	}

	// Update is called once per frame
	void Update () {

		faceCamera ();
		updateSpriteColor ();
		updatePosition ();
		
	}
}
