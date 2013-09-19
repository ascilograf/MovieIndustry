using UnityEngine;
using System.Collections;

public class GroundWaypoint : MonoBehaviour 
{
	public GroundWaypoint[] closestWaypoints;
	
	// Use this for initialization
	void Awake () 
	{
		
	}
	
	public void LookForClosestWaypoints()
	{
		int index = 0;
		closestWaypoints = new GroundWaypoint[8];
		foreach(GroundWaypoint point in GlobalVars.allGroundWaypoints)
		{
			if(point == null)
			{
				
			}
			else if(Vector3.Distance(point.transform.position, transform.position) <= 65 && point != this)
			{
				closestWaypoints[index] = point;
				index++;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
