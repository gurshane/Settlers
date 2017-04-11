using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

/// <summary>
/// User interface resource element.
/// </summary>
public class UIResource : UIElement {

	[SerializeField]
	private bool isGold;
	[SerializeField]
	private bool isFish;

	#region Private Attributes
	/// <summary>
	/// The resource type of this instance
	/// </summary>
	[SerializeField]
	private ResourceType _Resource;
	/// <summary>

	/// The text component attached to this instance
	/// </summary>
	private Text _ResourceCount;

	#endregion

	// Use this for initialization
	void Start () {

		if (name == "Gold_Image")
			isGold = true;

		if (name == "Fish_Image")
			isFish = true;


		setResourceType ();
		_ResourceCount = transform.GetChild (0).GetComponent<Text>();
	}
		

	/// <summary>
	/// Updates the UI Element
	/// </summary>
	/// <param name="p_Player">P player.</param>
	public override void uiUpdate(Player p_Player)
	{
		if (isGold) 
		{
			_ResourceCount.text = "x" + p_Player.getGoldCount ();
			return;
		}

		if (isFish) 
		{
			_ResourceCount.text = "x" + p_Player.numFish;
		}

		// Get Player's resource list, find the index in the list
		// which corresponds to this instance's ResourceType
		int[] resourceList = p_Player.getResources ();
		int resourceIndex = (int)_Resource;

		// Check whether the player resource list has been initialised or not
		if ( isArrayEmpty (resourceList) ) return;

		// Update UI Text to display the new value
		_ResourceCount.text = "x" + resourceList [resourceIndex];

	}


	/// <summary>
	/// Set the resource type of this instance based on its name
	/// </summary>
	private void setResourceType()
	{
		switch (name) 
		{
		case "Wool_Image":
			_Resource = ResourceType.WOOL;
			break;
		case "Grain_Image":
			_Resource = ResourceType.GRAIN;
			break;
		case "Brick_Image":
			_Resource = ResourceType.BRICK;
			break;
		case "Lumber_Image":
			_Resource = ResourceType.LUMBER;
			break;
		case "Ore_Image":
			_Resource = ResourceType.ORE;
			break;
		default:
			break;
		}
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
