using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// A subelement within the UI Canvas.
/// </summary>
public abstract class UIElement : MonoBehaviour, IDescribable<string>, IPointerEnterHandler, IPointerExitHandler {

	#region Protected Attributes
	#endregion


	#region Protected Methods
	#endregion


	#region Public Methods

	/// <summary>
	/// Updates the UI Element
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public abstract void uiUpdate (Player p_Player);


	/// <summary>
	/// Checks whether number of contents of parameter are greater than 0
	/// </summary>
	/// <returns><c>true</c>, if check was nulled, <c>false</c> otherwise.</returns>
	/// <param name="p_Array">P array.</param>
	protected virtual bool isArrayEmpty(int[] p_Array)
	{
		return (p_Array.Length <= 0);
	}

	protected virtual bool isStringNull(string p_String)
	{
		return (p_String == null);
	}

	/// <summary>
	/// Shows the description of the UI element being hovered
	/// </summary>
	/// <param name="p_Parameter">P parameter.</param>
	public virtual void showDescription(string p_Parameter)
	{
		Descriptor.textComponent.text = p_Parameter;
		Descriptor.transformComponent.position = Input.mousePosition + getPositionOffset ();
	}
		

	/// <summary>
	/// Raises the pointer enter event. Makes descriptor print the gameObject name
	/// </summary>
	/// <param name="p_EventData">P event data.</param>
	public virtual void OnPointerEnter(PointerEventData p_EventData)
	{
		showDescription(name);
	}

	/// <summary>
	/// Raises the pointer exit event. Makes the descriptor empty
	/// </summary>
	/// <param name="p_EventData">P event data.</param>
	public virtual void OnPointerExit(PointerEventData p_EventData)
	{
		showDescription ("");
	}

	#endregion


	#region Private Methods

	/// <summary>
	/// Offsets the descriptor text based on mouse position
	/// </summary>
	/// <returns>The position offset.</returns>
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

	#endregion
}
