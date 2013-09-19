using UnityEngine;
using System.Collections;

public class PopUpShortCut : MonoBehaviour 
{	
	public GameObject closeButton;
	public GameObject acceptButton;
	public tk2dTextMesh buttonText;
	public tk2dTextMesh text;
	
	public BuildingType type;
	
	public GameObject[] stages;
	FilmMaking filmMaking;
	PostProdOfficeStage vfxStage;
	ScriptersOfficeStage scriptersStage;
	MarketingOfficeStage marketingStage;
	FilmItem film;
	Scenario scenario;
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject g)
	{
		if(g == acceptButton)
		{
			CallMenu();//Finish();
		}
		else
		{
			gameObject.SetActive(false);
			//GlobalVars.cameraStates = CameraStates.normal;
		}
	}
	
	public void SetParams(BuildingType t, FilmItem f = null, Scenario s = null)
	{		
		//GlobalVars.BlockInput = true;
		type = t;
		switch(type)
		{
		case BuildingType.hangar:
			scenario = s;
			stages = GameObject.FindGameObjectsWithTag("Pavillion");
			foreach(GameObject stage in stages)
			{
				if(!stage.GetComponent<StageWork>().isStageBusy && !stage.GetComponent<FilmMaking>().busy)
				{
					filmMaking = stage.GetComponent<FilmMaking>();
					gameObject.SetActive(true);
					GlobalVars.cameraStates = CameraStates.menu;
					Utils.SetText(text, Utils.FormatStringToText("You can cast film now", 12));
					break;
				}
				else if(stage.GetComponent<StageWork>().isStageBusy)
				{
					filmMaking = stage.GetComponent<FilmMaking>();
					gameObject.SetActive(true);
					GlobalVars.cameraStates = CameraStates.menu;
					Utils.SetText(text, Utils.FormatStringToText("Right now there are no free hangars", 12));
				}
				else if(stage.GetComponent<FilmMaking>().busy)
				{
					filmMaking = stage.GetComponent<FilmMaking>();
					gameObject.SetActive(true);
					GlobalVars.cameraStates = CameraStates.menu;
					Utils.SetText(text, Utils.FormatStringToText("Right now there are no free hangars", 12));	
				}
			}
			Utils.SetText(buttonText, "Cast!");
			break;
		case BuildingType.postproduction:
			film = f;
			
			stages = GameObject.FindGameObjectsWithTag("postProdOffice");
			foreach(GameObject stage in stages)
			{
				if(!stage.GetComponent<PostProdOfficeStage>().busy && !stage.GetComponent<StageWork>().isStageBusy)
				{
					if(GlobalVars.popUpFinishAnyJob.gameObject.activeSelf)
						GlobalVars.popUpFinishAnyJob.Finish();
					vfxStage = stage.GetComponent<PostProdOfficeStage>();
					GlobalVars.popUpHireEffects.GetComponent<AddVisualEffects>().ShowMenu(vfxStage, film);
					//gameObject.SetActive(true);
					GlobalVars.cameraStates = CameraStates.menu;
					//Utils.SetText(text, Utils.FormatStringToText("You can vfx film now", 12));
					break;
				}
				else if(stage.GetComponent<PostProdOfficeStage>().busy || stage.GetComponent<StageWork>().isStageBusy)
				{
					vfxStage = stage.GetComponent<PostProdOfficeStage>();
					gameObject.SetActive(true);
					GlobalVars.cameraStates = CameraStates.menu;
					//Utils.SetText(text, Utils.FormatStringToText("Right now there are no free vfx offices", 12));
				}
				
			}
			Utils.SetText(buttonText, "Do vfx!");
			break;
		case BuildingType.office:
			film = f;
			stages = GameObject.FindGameObjectsWithTag("Offices");
			foreach(GameObject stage in stages)
			{
				if(stage.GetComponent<StageWork>() == null)
				{
					
				}
				else if(!stage.GetComponent<StageWork>().isStageBusy)
				{
					marketingStage = stage.GetComponent<MarketingOfficeStage>();
					gameObject.SetActive(true);
					GlobalVars.cameraStates = CameraStates.menu;
					Utils.SetText(text, Utils.FormatStringToText("You can marketing film now", 12));
					break;
				}
				else if(stage.GetComponent<StageWork>().isStageBusy)
				{
					marketingStage = stage.GetComponent<MarketingOfficeStage>();
					gameObject.SetActive(true);
					GlobalVars.cameraStates = CameraStates.menu;
					Utils.SetText(text, Utils.FormatStringToText("Right now there are no free marketing offices", 12));
				}
			}
			Utils.SetText(buttonText, "Promote!");
			break;
		}
		
	}
	
	public bool HaveFreeOffice(BuildingType t)
	{
		switch(t)
		{
		case BuildingType.hangar:
			//scenario = s;
			stages = GameObject.FindGameObjectsWithTag("Pavillion");
			foreach(GameObject stage in stages)
			{
				if(stage.GetComponent<StageWork>().isStageBusy)
				{
					return false;
				}
				else if(stage.GetComponent<FilmMaking>().busy)
				{
					return false;	
				}
			}
			break;
		case BuildingType.postproduction:
			//film = f;
			stages = GameObject.FindGameObjectsWithTag("postProdOffice");
			foreach(GameObject stage in stages)
			{
				if(stage.GetComponent<PostProdOfficeStage>().busy || stage.GetComponent<StageWork>().isStageBusy)
				{
					return false;
				}
				
			}
			break;
		case BuildingType.office:
			//film = f;
			stages = GameObject.FindGameObjectsWithTag("Offices");
			foreach(GameObject stage in stages)
			{
				if(stage.GetComponent<StageWork>() == null)
				{
					
				}
				else if(stage.GetComponent<StageWork>().isStageBusy)
				{
					return false;
				}
			}
			break;
		}
		return true;
	}
	
	public void CallMenu()
	{
		switch(type)
		{
		case BuildingType.hangar:
			if(filmMaking.boostIcon.activeSelf)
			{
				GlobalVars.cameraStates = CameraStates.normal;
			}
			else if(filmMaking.GetComponent<StageWork>().worker != null)
			{
				StageWork s = filmMaking.GetComponent<StageWork>();
				GlobalVars.popUpStageWork.ShowInfo(s.worker, (int)s.time, s);	
			}
			else if(filmMaking.busy && filmMaking.staffCount == filmMaking.staff.Count)
			{
				GlobalVars.popUpSpeedUpAnyJob.SetParamsForFinish(filmMaking);	
			}
			else if(!filmMaking.busy)
			{
				GlobalVars.cameraStates = CameraStates.menu;
				//GlobalVars.popUpHireForFilm.gameObject.SetActive(true);
				//GlobalVars.popUpHireForFilm.script = scenario;
				//scenario.icon
				GlobalVars.popUpCastingMenu.SetParams(filmMaking, scenario);	
			}
			break;
		case BuildingType.postproduction:
			if(GlobalVars.popUpFinishAnyJob.gameObject.activeSelf)
					GlobalVars.popUpFinishAnyJob.Finish();
			if(vfxStage.GetComponent<StageWork>().worker != null)
			{
				StageWork s = vfxStage.GetComponent<StageWork>();
				GlobalVars.popUpStageWork.ShowInfo(s.worker, (int)s.time, s);
			}
			else if(vfxStage.busy)
			{
				GlobalVars.popUpSpeedUpAnyJob.SetParamsForFinish(vfxStage);		
			}
			else if(!vfxStage.busy)
			{
				GlobalVars.cameraStates = CameraStates.menu;
				GlobalVars.popUpHireEffects.GetComponent<AddVisualEffects>().ShowMenu(vfxStage, film);
			}
			break;
		case BuildingType.office:
			if(marketingStage.GetComponent<StageWork>().worker != null)
			{
				StageWork s = marketingStage.GetComponent<StageWork>();
				GlobalVars.popUpStageWork.ShowInfo(s.worker, (int)s.time, s);	
			}
			else 
			{
				GlobalVars.cameraStates = CameraStates.menu;
				GlobalVars.popUpSelectMarketing.SetParams(marketingStage, film);
			}
			break;
		}
		gameObject.SetActive(false);
	}
	
	void Finish () 
	{
		Vector3 v3 = Camera.main.transform.position;
		v3.z = -650;
		switch(type)
		{
		case BuildingType.hangar:
			
			v3.x = filmMaking.transform.position.x;
			break;
		case BuildingType.postproduction:
			
			v3.x = vfxStage.transform.position.x;
			break;
		case BuildingType.office:
			
			v3.x = marketingStage.transform.position.x;
			break;
		}
		
		gameObject.SetActive(false);
		Coroutiner.StartCoroutine(Utils.MoveCameraTo(Camera.main.transform.position, v3, this));
	}
}
