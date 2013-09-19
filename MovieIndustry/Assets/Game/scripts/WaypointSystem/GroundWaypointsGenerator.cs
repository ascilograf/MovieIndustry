using UnityEngine;
using System.Collections;

public class GroundWaypointsGenerator : MonoBehaviour {
	
	public GameObject waypointPrefab;
	// Use this for initialization
	void Awake () 
	{
		int index = 0;
		GlobalVars.allGroundWaypoints = new GroundWaypoint[640];
		for(int i = 0; i < 4; i++)
		{
			for(int j = 0; j < 160; j++)
			{
				GameObject go = Instantiate(waypointPrefab) as GameObject;
				go.transform.parent = transform;
				go.transform.localPosition = new Vector3(j * 40, i * -40, (i * -30) - 100);
				GlobalVars.allGroundWaypoints[index] = go.GetComponent<GroundWaypoint>();
				index++;
			}
		}
		foreach(GroundWaypoint point in GlobalVars.allGroundWaypoints)
		{
			if(point != null)
			{
				point.LookForClosestWaypoints();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
