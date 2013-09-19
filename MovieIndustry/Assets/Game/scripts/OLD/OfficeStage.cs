using UnityEngine;
using System.Collections;

public class OfficeStage : MonoBehaviour 
{
	public int stage;
	public BuildingType type;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	/*public void NewWork(int timeInSec, int expForStudio, int expForWorker, int reward, Scripter scripter)
	{
		StartCoroutine(StartNewWork(timeInSec,  expForStudio,  expForWorker,  reward, scripter));
	}
	
	IEnumerator StartNewWork(float timeInSec, int expForStudio, int expForWorker, int reward, Scripter scripter)
	{
		while(true)
		{
			if(timeInSec <= 0)
			{
				GlobalVars.exp += expForStudio;
				GlobalVars.money += reward;
				scripter.canBeUsed = true;
				scripter.exp += expForWorker;
				Doors[] doors = GetComponentsInChildren<Doors>(true);
				List<PersonController> list = new List<PersonController>();
				list.Add(scripter.GetComponent<PersonController>());
				StaffManagment.SetStaffFree(list, doors);
				isScriptWritten = false;
				yield break;
			}
			print (timeInSec);
			isScriptWritten = true;
			timeInSec -= Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
			yield return 0;
		}
	}*/
}
