using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//В этот скрипт помещаются МешРендереры деверей и тип двери, используется для выставления пути выхода из здания для персонажей
public class Doors : MonoBehaviour 
{
	public MeshRenderer openedDoor;				//объект открытой двери
	public MeshRenderer closedDoor;				//объект закрытой двери
	public DoorsLocation door;					//тип двери
	
	public List<PersonController> persons = new List<PersonController>();
	
	void OnTriggerEnter(Collider coll)
	{
		if(coll.tag == "Person")
		{
			if(	coll.transform.parent.GetComponent<PersonController>().state == PersMovingStates.moveToBuilding
				|| coll.transform.parent.GetComponent<PersonController>().state == PersMovingStates.moveOutOfBuilding)
			{
				openedDoor.enabled = true;
				persons.Add(coll.transform.parent.GetComponent<PersonController>());
			}
		}
	}
	
	void OnTriggerExit(Collider coll)
	{
		if(coll.tag == "Person")
		{
			if(persons.Count > 0)
			{
				persons.Remove(coll.transform.parent.GetComponent<PersonController>());
			}
		}
		if(persons.Count == 0)
		{
			openedDoor.enabled = false;
		}
	}
}
