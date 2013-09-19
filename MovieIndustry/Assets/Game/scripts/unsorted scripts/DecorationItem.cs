using UnityEngine;
using System.Collections;

//скрипт, для крпеления к объекту собранной готовой декорации
//содержит инфу о рарности, кол-во хп и сеттинг

public class DecorationItem : MonoBehaviour 
{	
	public int maintenance;
	public RarityLevel rarity;
	public Setting setting;
	
	// Use this for initialization
	void Start () 
	{
		GlobalVars.inventory.items.Add(GetComponent<InventoryItem>());
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
