using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponent : MonoBehaviour {


	public void distributeComponents()
	{
		foreach (Transform child in transform) 
		{
			child.gameObject.AddComponent<UIElement> ();
		}
	}
	// Use this for initialization
	void Start () {
		distributeComponents ();
	}

}
