using UnityEngine;
using System.Collections;

public class StartWindow : MonoBehaviour {
	
	public GameObject[] adas;
	// Use this for initialization
	void Start () 
	{
		adas = GameObject.FindGameObjectsWithTag("Offices");
	}
	
	// Update is called once per frame
	void Update () 
	{
		GlobalVars.cameraStates = CameraStates.menu;
		if(Input.GetMouseButtonUp(0))
		{
			GlobalVars.cameraStates = CameraStates.normal;
			gameObject.SetActive(false);
		}
	}
}
