using UnityEngine;
using System.Collections;

//отслеживание кол-ва свободных/занятых рабочих
//здесь располагается приемник сообщений

public class WorkersCount : MonoBehaviour 
{
	public tk2dTextMesh count;
	// Use this for initialization
	void Awake () 
	{
		Messenger.AddListener("Check workers count", CheckWorkersCount);
	}
	
	void CheckWorkersCount()
	{
		int c = 0;
		GameObject[] workers = GameObject.FindGameObjectsWithTag("worker");
		foreach(GameObject g in workers)
		{
			if(!g.GetComponent<PersonController>().busy)
			{
				c++;
			}
		}
		count.text = c.ToString() + "/" + workers.Length;
		count.Commit();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
