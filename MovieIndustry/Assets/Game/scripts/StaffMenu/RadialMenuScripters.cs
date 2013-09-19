
using UnityEngine;
using System.Collections;

//контроллер радиального меню, общий
//в нем указываются углы элементов радиального меню
//в зависимости от типа здания на старте заполняются переменные радиального меню
//далее на вход принимается экземпляр скрипта здания
//после выбора сабменю - этот экземпляр отправляется в сабменю
public class RadialMenuScripters: MonoBehaviour 
{
	public RadialMenu[] menus;								//массив элементов этого меню
	public BuildingType type;								//тип здания
	public ScriptersOfficeStage scriptersOfficeStage;		
	public MarketingOfficeStage marketingOfficeStage;
	public FilmMaking hangarOfficeStage;
	public PostProdOfficeStage postProdOfficeStage;
	
	public StageWork stageWork;								//этажная работа
	public int officeLvl;									//уровень здания
	public int stage;										//этаж
	
	float scale;

	
	void Start()
	{
		//scale = transform.localScale.y;
		//gameObject.SetActive(false);
		
	}
	
	void OnEnable()
	{
		GlobalVars.soundController.ShowRadialMenu();
		//Messenger<GameObject>.AddListener("Tap on GUI Layer begin", CheckTapBegin);
		Messenger.AddListener("Finger was lifted", FingerLifted);
		for(int i = 0; i < menus.Length; i++)
		{
			ActivateSprites(menus[i].button, false);
		}
	}
	
	void OnDisable()
	{
		//Messenger<GameObject>.RemoveListener("Tap on GUI Layer begin", CheckTapBegin);
		Messenger.RemoveListener("Finger was lifted", FingerLifted);
	}
	
	void FingerLifted()
	{
		print ("SD");
		foreach(RadialMenu r in menus)
		{
			ActivateSprites(r.button, false);
		};
	}
	
	void ActivateSprites(MeshRenderer s, bool b)
	{
		s.enabled = b;
		MeshRenderer[] sprites = s.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer spr in sprites)
		{
			spr.enabled = b;
		}
	}
	
	void Update () 
	{
		if(Input.GetMouseButton(0))
		{
			Vector3 v1 = transform.position;
			v1.z = 0;
			Vector3 v2 = GlobalVars.GUICamera.ScreenToWorldPoint(Input.mousePosition);
			v2.z = 0;//new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
			float f = Screen.height;
			float dist = (f/768) * 150;
			if(Vector3.Distance(v1, v2) < dist)
			{
				Vector3 dir = v2 - v1;
				float dir1 = Vector3.Angle(dir, transform.up);
				//print (dir);
				
				if(dir.x < 0 && dir.y < 0)
				{
					dir1 += (180 - Vector3.Angle(dir, transform.up)) * 2;
				}
				if(dir.x < 0 && dir.y > 0)
				{
					dir1 = 360 - Vector3.Angle(dir, transform.up);
				}
				print ("dir1: " + transform.position);
				print ("dist1: "+ GlobalVars.GUICamera.ScreenPointToRay(Input.mousePosition));
				for(int i = 0; i < menus.Length; i++)
				{
					
					if(menus[i].angleFrom < dir1 && menus[i].angleTo > dir1)
					{
						
						ActivateSprites(menus[i].button, true);
					}
					else
					{
						print ("angle: " + dir1);
						ActivateSprites(menus[i].button, false);
					}
					
				}
			}
		}
		if(Input.GetMouseButtonUp(0))
		{
			Vector3 v1 = transform.position;
			v1.z = 0;
			Vector3 v2 = GlobalVars.GUICamera.ScreenToWorldPoint(Input.mousePosition);
			v2.z = 0;//new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
			float f = Screen.height;
			float dist = (f/768) * 150;
			if(Vector3.Distance(v1, v2) < dist)
			{
				GlobalVars.soundController.RadialMenuTap();
				Vector3 dir = v2 - v1;
				float dir1 = Vector3.Angle(dir, transform.up);
				//print (dir);
				
				if(dir.x < 0 && dir.y < 0)
				{
					dir1 += (180 - Vector3.Angle(dir, transform.up)) * 2;
				}
				if(dir.x < 0 && dir.y > 0)
				{
					dir1 = 360 - Vector3.Angle(dir, transform.up);
				}
				print (dir1);
				for(int i = 0; i < menus.Length; i++)
				{
					if(menus[i].angleFrom < dir1 && menus[i].angleTo > dir1 && menus[i].menu != null)
					{
						if(menus[i].menu.GetComponent<popUpStageWork>() != null)
						{
							menus[i].menu.GetComponent<popUpStageWork>().SetParams(type, officeLvl, stage, stageWork);					
							gameObject.SetActive(false);
							SetNotBusy();
							GlobalVars.cameraStates = CameraStates.menu;
						}
						if(menus[i].menu.GetComponent<popUpNewScript>() != null)
						{
							GlobalVars.cameraStates = CameraStates.menu;
							//menus[i].menu.GetComponent<popUpNewScript>().ShowMenu();
							menus[i].menu.GetComponent<popUpNewScript>().SetParams(scriptersOfficeStage);
							menus[i].menu.GetComponent<popUpNewScript>().SwitchState(PopUpNewScriptState.selectGenres);
							gameObject.SetActive(false);
							//menus[i].menu.GetComponent<popUpNewScript>().PrepareScripters();
						}
						if(menus[i].menu.GetComponent<HireForFilm>() != null)
						{
							GlobalVars.cameraStates = CameraStates.menu;
							GlobalVars.popUpCastingMenu.SetParams(hangarOfficeStage);
							gameObject.SetActive(false);
								//menus[i].menu.GetComponent<popUpNewScript>().PrepareScripters();
						}
						if(menus[i].menu.GetComponent<AddVisualEffects>() != null)
						{
							GlobalVars.cameraStates = CameraStates.menu;
							menus[i].menu.GetComponent<AddVisualEffects>().ShowMenu(postProdOfficeStage);
							gameObject.SetActive(false);
							//menus[i].menu.GetComponent<popUpNewScript>().PrepareScripters();
						}
						if(menus[i].menu.GetComponent<PopUpSelectMarketing>() != null)
						{
							GlobalVars.cameraStates = CameraStates.menu;
							//menus[i].menu.GetComponent<PopUpSelectMarketing>().ShowMenu();
							menus[i].menu.GetComponent<PopUpSelectMarketing>().SetParams(marketingOfficeStage);
							gameObject.SetActive(false);
							//menus[i].menu.GetComponent<popUpNewScript>().PrepareScripters();
						}
						
						//перемещение здания, поиск констракта, передача ему команды о перемещении
						if(menus[i].menu.name == "PlaceBuilding")
						{
							Construct constr = null;
							if(stageWork.transform.parent != null)
							{
								constr = stageWork.transform.parent.GetComponent<Construct>();
								if(constr == null && stageWork.transform.parent.transform.parent != null)
								{
									constr = stageWork.transform.parent.transform.parent.GetComponent<Construct>();
								}
								if(!constr.CheckUpgradeAvailability())
								{
									return;
								}
							}
							if(constr != null)
							{
								constr.ChangePlace();
								GlobalVars.cameraStates = CameraStates.normal; 	
								gameObject.SetActive(false);
								SetNotBusy();
							}
						}
						
						//апгрейд здания, поиск констракта, передача ему команды о апгрейде
						if(menus[i].menu.name == "Upgrade")
						{
								Construct constr = null;
								if(stageWork.transform.parent != null)
								{
									constr = stageWork.transform.parent.GetComponent<Construct>();
									if(constr == null && stageWork.transform.parent.transform.parent != null)
									{
										constr = stageWork.transform.parent.transform.parent.GetComponent<Construct>();
									}
									if(!constr.CheckUpgradeAvailability())
									{
										return;
									}
								}
								if(constr != null)
								{
									if(constr.buildings.Length <= (int)constr.stage + 1)
									{
										GlobalVars.cameraStates = CameraStates.normal; 	
										gameObject.SetActive(false);
										SetNotBusy();
										return;
									}
									constr.stage++;
									constr.StartBuildNewOffice(10, (int)constr.stage, (int)constr.stage * 1000);
									GlobalVars.cameraStates = CameraStates.normal; 	
									gameObject.SetActive(false);
								}
								Messenger.Broadcast("Check workers count");
							
						}
					}
				}
			}
			else
			{
				gameObject.SetActive(false);
				if(marketingOfficeStage != null)
				{
					marketingOfficeStage.isBusy = false;
				}
				if(scriptersOfficeStage != null)
				{
					scriptersOfficeStage.isStageBusy = false;
				}
				if(hangarOfficeStage != null)
				{
					hangarOfficeStage.busy = false;
				}
				if(postProdOfficeStage != null)
				{
					postProdOfficeStage.busy = false;
				}
				GlobalVars.cameraStates = CameraStates.normal;
			}
		}
	}
	
	//сделать этаж активнымд для задач
	void SetNotBusy()
	{
		if(marketingOfficeStage != null)
		{
			marketingOfficeStage.isBusy = false;
		}
		if(scriptersOfficeStage != null)
		{
			scriptersOfficeStage.isStageBusy = false;
		}
		if(hangarOfficeStage != null)
		{
			hangarOfficeStage.busy = false;
		}
		if(postProdOfficeStage != null)
		{
			postProdOfficeStage.busy = false;
		}
	}
	
	//показать меню
	public void ShowMenu()
	{
		gameObject.SetActive(true);
		GlobalVars.cameraStates = CameraStates.menu;		
	}
	
	void Pos()
	{
		/*Vector3 v = gameObject.transform.position;
		if(stage == 0)
		{
			v.y = -120 * (Screen.height / 768);
			v.z = 200;
		}
		else
		{
			v = Vector3.zero;
		}
			
		gameObject.transform.localPosition = v;*/
	}
	
	#region передача экземпляра этажа офиса в это меню, выставление параметров этажа и уровня
	public void SetParams(ScriptersOfficeStage office)
	{
		officeLvl = office.officeLvl;
		stage = office.stageIndex;
		scriptersOfficeStage = office;
		stageWork = office.GetComponent<StageWork>();
		Pos();
	}
	
	
	
	public void SetParams(MarketingOfficeStage office)
	{
		officeLvl = office.officeLvl;
		stage = office.officeStage;
		marketingOfficeStage = office;
		stageWork = office.GetComponent<StageWork>();
		Pos();
	}
	
	public void SetParams(FilmMaking office)
	{
		officeLvl = office.GetComponent<StageWork>().lvl;
		stage = office.GetComponent<StageWork>().stage;
		hangarOfficeStage = office;
		stageWork = office.GetComponent<StageWork>();
		Pos();
	}
	
	public void SetParams(PostProdOfficeStage office)
	{
		officeLvl = office.GetComponent<StageWork>().lvl;
		stage = office.GetComponent<StageWork>().stage;
		postProdOfficeStage = office;
		stageWork = office.GetComponent<StageWork>();
		Pos();
	}
	#endregion
}
