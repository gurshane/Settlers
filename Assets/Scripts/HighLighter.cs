using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLighter : MonoBehaviour {

    private GameObject currentlyHighlighted;

	
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit impact;
            if(Physics.Raycast(ray, out impact))
            {
                GameObject temp = impact.collider.gameObject;
                if(temp.tag == "Edge" || temp.tag == "Vertex")
                {
                    if (currentlyHighlighted != null)
                    {
                        currentlyHighlighted.GetComponent<MeshRenderer>().enabled = false;
                    }
                    temp.GetComponent<MeshRenderer>().enabled = true;
                    currentlyHighlighted = temp;
                }
            }
        }
	}
}
