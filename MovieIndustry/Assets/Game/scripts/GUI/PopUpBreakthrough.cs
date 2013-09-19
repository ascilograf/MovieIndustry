using UnityEngine;
using System.Collections;

public class
PopUpBreakthrough : MonoBehaviour {

	public tk2dTextMesh title;
	public tk2dTextMesh price;
	public GameObject buttonClose;
	public GameObject buttonIncrPercent;
	public GameObject buttonDecrPercent;
	public GameObject buttonOk;
	
	int percent = 0;
	//int money = 0;
	FilmStaff staff;
	GameObject stage;
	// Use this for initialization
	void Start () 
	{
		Messenger<GameObject>.AddListener("Tap on GUI Layer", CheckTapOnGUI);
		GlobalVars.popUpBreakthrough = this;
		gameObject.SetActive(false);
	}
	
	void CheckTapOnGUI(GameObject go)
	{
		if(go == buttonOk)
		{
			if(((staff.lvl * 1000) * percent) >= GlobalVars.money)
			{
				GlobalVars.money -= (staff.lvl * 1000) * percent;
				Final();
				GlobalVars.cameraStates = CameraStates.normal;
			}
		}
		else if(go == buttonClose)
		{
			GlobalVars.cameraStates = CameraStates.normal;
		}
		else if(go == buttonIncrPercent)
		{
			percent++;
			price.text = "for " + ((staff.lvl * 1000) * percent).ToString();
			price.Commit();
		}
		else if(go == buttonDecrPercent)
		{
			if(percent-- >= 0)
			{
				percent--;
			}
			price.text = "for " + ((staff.lvl * 1000) * percent).ToString();
			price.Commit();
			
		}
	}
	
	public void SetParams(FilmStaff fs, GameObject go)
	{
		price.text = "for " + ((staff.lvl * 1000) * percent).ToString();
		price.Commit();
		GlobalVars.cameraStates = CameraStates.menu;
		title.text = "Attempt breakthrough in " + fs.name;
		title.Commit();
		gameObject.SetActive(true);
		staff = fs;
		stage = go;
	}
	
	void Final()
	{
		gameObject.SetActive(false);
		Messenger<GameObject, int>.Broadcast("breakthrow", stage, percent);
	}
}
