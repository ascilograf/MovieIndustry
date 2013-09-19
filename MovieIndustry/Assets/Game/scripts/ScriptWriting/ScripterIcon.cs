using UnityEngine;
using System.Collections;


public class ScripterIcon : MonoBehaviour 
{
	Scripter _scripter;
	// Use this for initialization
	void Start () {
	
	}
	
	public Scripter scripter
	{
		get {	return _scripter;	}
		set {	_scripter = value; 	}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
