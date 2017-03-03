using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Descriptor : MonoBehaviour {


	public static Text textComponent;
	public static Transform transformComponent;

	// Use this for initialization
	void Start () {

		textComponent = GetComponent<Text>();
		transformComponent = GetComponent<Transform> ();

	}

}
