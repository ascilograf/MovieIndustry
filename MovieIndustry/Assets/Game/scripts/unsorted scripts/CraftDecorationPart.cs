using UnityEngine;
using System.Collections;

public class CraftDecorationPart : MonoBehaviour 
{
	public Setting setting;
	public RarityLevel rarity;
	public PartType typePart;
	
	// Use this for initialization
	void Start () 
	{
		GlobalVars.inventory.items.Add(GetComponent<InventoryItem>());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
