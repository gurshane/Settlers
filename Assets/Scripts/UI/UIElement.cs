using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// A subelement within the UI Canvas.
/// </summary>
public class UIElement : MonoBehaviour, IDescribable<string>, IPointerEnterHandler, IPointerExitHandler {


	public void showDescription(string p_Parameter)
	{
		Descriptor.textComponent.text = p_Parameter;
		Descriptor.transformComponent.position = Input.mousePosition + getPositionOffset ();
	}

	public void OnPointerEnter(PointerEventData p_EventData)
	{
		showDescription(name);
	}

	public void OnPointerExit(PointerEventData p_EventData)
	{
		showDescription ("");
	}
		
	private Vector3 getPositionOffset()
	{
		Vector3 returnVec = new Vector3 (0, 0, 0);

		if (Input.mousePosition.x > Screen.width/2) 
		{
			returnVec.x = -60f;
		}

		if (Input.mousePosition.x <= Screen.width / 2) 
		{
			returnVec.x = 60f;
		}

		return returnVec;
	}
}
