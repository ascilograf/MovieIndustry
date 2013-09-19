using UnityEngine;
using System.Collections;

public class BuildSlotController : MonoBehaviour 
{
	public MeshRenderer red;
	public MeshRenderer green;
	
	Collider parentColl;
	
	void Start () 
	{
		parentColl = transform.parent.parent.collider;
		green = transform.FindChild("green").GetComponent<MeshRenderer>();
		red = transform.FindChild("red").GetComponent<MeshRenderer>();
		red.enabled = false;
	}
	
	void OnTriggerEnter(Collider coll)
	{
		if(coll.tag == "building" && coll != parentColl)
		{
			green.enabled = false;
			red.enabled = true;
		}
	}
	
	void OnTriggerStay(Collider coll)
	{
		if(coll.tag == "building" && coll != parentColl)
		{
			green.enabled = false;
			red.enabled = true;
		}
	}
	
	void OnTriggerExit(Collider coll)
	{
		if(coll.tag == "building" && coll != parentColl)
		{
			green.enabled = true;
			red.enabled = false;
		}
	}
}
