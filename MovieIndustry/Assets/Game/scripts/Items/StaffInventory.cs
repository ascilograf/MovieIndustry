using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaffInventory : MonoBehaviour {
	
	public List<StaffInventoryItemParams> items;
	FilmStaff filmStaff;
	// Use this for initialization
	void Awake()
	{
		filmStaff = GetComponent<FilmStaff>();
	}
	
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach(StaffInventoryItemParams s in items)
		{	
			if(s.item != null && s.isSlotOpen)
			{
				if(s.item.bonuses.useTime > 0)
				{
					s.item.bonuses.useTime -= Time.deltaTime;
					filmStaff.ChangeBonuses();
				}
				else if(s.item.bonuses.useTime == -1)
				{
					
				}
				else if(s.item.bonuses.useTime <= 0)
				{
					filmStaff.ChangeBonuses();
					Destroy(s.item.gameObject);
					s.item = null;
				}
				s.lockedSlotIcon.enabled = false;
				s.emptySlotIcon.enabled = false;
			}
			else if (!s.isSlotOpen)
			{
				s.lockedSlotIcon.enabled = true;
				s.emptySlotIcon.enabled = false;
			}
			else if(s.item == null)
			{
				s.lockedSlotIcon.enabled = false;
				s.emptySlotIcon.enabled = true;
			}
		}
	}
}
