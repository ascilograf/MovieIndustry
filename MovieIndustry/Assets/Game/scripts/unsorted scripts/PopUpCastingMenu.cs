using UnityEngine;
using System.Collections.Generic;
using System.Collections;



public class PopUpCastingMenu : MonoBehaviour 
{
	public enum Phases			//to private
	{
		phase1,
		phase2,
		phase3,
		phase4,
	}
	
	public GameObject buttonBack;
	public GameObject buttonClose;
	public GameObject buttonFinish;
	public GameObject buttonPrevButton;
	public GameObject buttonNextSetting;
	public AudioSource switchPhasesSound;
	
	public GameObject phase1;
	public GameObject phase2;
	public GameObject phase3;
	public GameObject phase4;
	public GameObject infoPart;
	public Transform bottomPart;
	public GameObject helperAvatar;
	public GetTextFromFile helpText;
	public MeshRenderer clock;
	public tk2dTextMesh moneyMesh;
	public tk2dTextMesh timeMesh;
	public tk2dTextMesh nameMesh;
	public tk2dTextMesh finalScreenPrice;
	public tk2dTextMesh finalScreenTime;
	public GetTextFromFile rubricatorGenreMesh;
	public GameObject[] finalScreenGenres;
	public GameObject[] headerGenres;
	
	public GameObject directorPlane;
	public GameObject cameramanPlane;
	public GameObject[] actorsPlanes;
	public GameObject[] scenePlanes;
	public SettingButton[] decorations;
	
	public GameObject universalPlanePrefab;

	public int price;
	public int timeInSec;
	//public GetTextFromFile helpText;
	public Scenario pickedScript;
	public FilmStaff director;
	public FilmStaff cameraman;
	public List<FilmStaff> actors;
	
	public tk2dUIScrollableArea scriptsScroll;		
	public tk2dUIScrollableArea staffScroll;
	public tk2dUIScrollableArea staffPlacesScroll;
	public tk2dUIScrollableArea scenesPlacesScroll;
	public tk2dUIScrollableArea scenesScroll;
	public tk2dUIScrollableArea finalScreenStaffScroll;
	public tk2dUIScrollableArea finalScreenScenesScroll;
	public GameObject[] finalScreenScenesTitles;
	public List<Scenario> scripts;			//to private
	
	public Phases phase;					//to private
	public List<FilmStaff> currStaff;		//to private
	public CharacterType staffType;         //to private
	public Setting currSceneSetting;				//to private
	public RarityLevel currSceneRarity;				//to private
	public List<Scene> scenes;						
	public List<UniversalMiniPlane> universalPlanesStaff;	//to private
	public List<UniversalMiniPlane> universalPlaneScene;	//to private
	public SettingButton currSettButt; 
	public FilmMaking hangar;
	List<GameObject> bigPlanes;
	List<GameObject> smallPlanes;
	
	void Start () 
	{	
		
		//SwitchPhase(phase);
		//GlobalVars.cameraStates = CameraStates.menu;
	}
	
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
		if(g == directorPlane)
		{	
			RefreshListOf(CharacterType.director);
			RefreshInformations();
			return;
		}
		else if(buttonFinish == g)
		{
			Finish();
		}
		else if(g.name == "closeButton")
		{
			hangar.busy = false;
			ClearParams(true);
			gameObject.SetActive(false);
		}
		else if(g == cameramanPlane)
		{
			RefreshListOf(CharacterType.cameraman);
			RefreshInformations();
			return;
		}
		else if(g == buttonNextSetting)
		{
			RefreshSetting(true);
			RefreshInformations();
			return;
		}
		else if(g == buttonPrevButton)
		{
			RefreshSetting(false);
			RefreshInformations();
			return;
		}
		else if(g.name == "backButton")
		{
			TapOnBackBtn();
			return;
		}
		else if(g.GetComponent<UniversalMiniPlane>() != null)
		{
			TapOnUniPlane(g.GetComponent<UniversalMiniPlane>());
		}
		if(g.GetComponent<SettingButton>() != null)
		{
			AddScene(g.GetComponent<SettingButton>());
		}
		foreach(GameObject go in actorsPlanes)
		{
			if(go == g)
			{
				RefreshInformations();
				RefreshListOf(CharacterType.actor);
				return;
			}
		}
		
		
		foreach(GameObject go in scenePlanes)
		{
			if(go == g)
			{
				RefreshDecorationsList();
				RefreshInformations();
				return;
			}
		}
		
		foreach(Scenario s in scripts)
		{
			if(s.icon == g)
			{
				AddScript(s);
				return;
			}
		}
		foreach(FilmStaff f in currStaff)
		{
			if(f.icon == g)
			{
				AddNewStaff(f);
				return;
			}
		}
	}
	
	void TapOnBackBtn()
	{
		if(phase == Phases.phase2)
		{
			SwitchPhase(Phases.phase1);
			director = null;
			cameraman = null;
			actors.Clear();
			pickedScript = null;
			StaffManagment.CharInfoToParent(currStaff);
			currStaff.Clear();
			foreach(UniversalMiniPlane u in universalPlaneScene)
			{
				if(u != null)
				{
					Destroy(u.gameObject);
				}
			}
			foreach(UniversalMiniPlane u in universalPlanesStaff)
			{
				Destroy(u.gameObject);
			}
			universalPlaneScene.Clear();
			universalPlanesStaff.Clear();
			scenes.Clear();
			cameramanPlane.SetActive(true);
			actorsPlanes[0].SetActive(true);
			actorsPlanes[1].SetActive(true);
			actorsPlanes[2].SetActive(true);	
		}
		else if(phase == Phases.phase3)
		{
			actors.Remove(actors[2]);
			SwitchPhase(Phases.phase2);
			RefreshListOf(CharacterType.actor);
		}
		else if(phase == Phases.phase4)
		{
			GameObject g = universalPlaneScene[scenes.Count - 1].gameObject;
			universalPlaneScene.Remove(universalPlaneScene[scenes.Count - 1]);
			scenesPlacesScroll.Value = 1;
			Destroy(g);
			scenes.Remove(scenes[scenes.Count - 1]);
			RefreshScenesPlanes();
			SwitchPhase(Phases.phase3);
			RefreshDecorationsList();
		}
		RefreshInformations();
	}
	
	void TapOnUniPlane(UniversalMiniPlane u)
	{
		if(phase == Phases.phase2)
		{
			if(u.staff == director && cameraman != null && scenes.Count != pickedScript.numberOfScenes)
			{
				director = null;
				RefreshListOf(CharacterType.director);
				universalPlanesStaff.Remove(u);
				Destroy(u.gameObject);
			}
			else if(u.staff == director && cameraman == null && scenes.Count != pickedScript.numberOfScenes && actors.Count == 0)
			{
				director = null;
				RefreshListOf(CharacterType.director);
				universalPlanesStaff.Remove(u);
				Destroy(u.gameObject);
			}
			else if(u.staff == cameraman && director != null && scenes.Count != pickedScript.numberOfScenes)
			{
				cameraman = null;
				RefreshListOf(CharacterType.cameraman);
				universalPlanesStaff.Remove(u);
				Destroy(u.gameObject);
			}
			else if(actors.Count == 0)
			{
				
			}
			else if(u.staff == actors[0] && cameraman != null && director != null && scenes.Count != pickedScript.numberOfScenes)
			{
				actors.Remove(actors[0]);
				RefreshListOf(CharacterType.actor);
				universalPlanesStaff.Remove(u);
				Destroy(u.gameObject);
			}
			else if(actors.Count == 1)
			{
				
			}
			else if(u.staff == actors[1] && cameraman != null && director != null && scenes.Count != pickedScript.numberOfScenes)
			{
				actors.Remove(actors[1]);
				RefreshListOf(CharacterType.actor);
				universalPlanesStaff.Remove(u);
				Destroy(u.gameObject);
			}
			RefreshPlanes();
			RefreshInformations();
		}
		else if(phase == Phases.phase3)
		{
			for(int j = 0; j < scenes.Count; j++)
			{
				if(scenes[j].plane == null)
				{
					return;
				}
			}
			if(scenes[scenes.Count - 1].plane == u)
			{
				scenes.Remove(scenes[scenes.Count - 1]);
			}
			else
			{
				for(int i = 0; i < scenes.Count; i++)
				{
					if(scenes[i].plane == u)
					{
						scenes[i] = new Scene();
					}
					if(universalPlaneScene[i] == u)
					{
						universalPlaneScene[i] = null;
					}
				}
			}
			Destroy(u.gameObject);
			RefreshScenesPlanes();
			RefreshInformations();
			RefreshDecorationsList();
			/*for(int i = 0; i < universalPlaneScene.Count; i++)
			{
				if(universalPlaneScene[i] == u)
				{
					scenes.Remove(scenes[i]);
					universalPlaneScene.Remove(u);
					Destroy(u.gameObject);
				}
			}
			RefreshScenesPlanes();
			RefreshInformations();*/
		}
		else if(phase == Phases.phase4)
		{
			if(u.staff != null)
			{
				if(u.staff == director)
				{
					director = null;
					RefreshListOf(CharacterType.director);
					staffPlacesScroll.Value = 0;
				}
				else if(u.staff == cameraman)
				{
					cameraman = null;
					RefreshListOf(CharacterType.cameraman);
					staffPlacesScroll.Value = 0.25f;
				}
				else if(u.staff == actors[0])
				{
					actors.Remove(actors[0]);
					RefreshListOf(CharacterType.actor);
					staffPlacesScroll.Value = 1f;
				}
				else if(u.staff == actors[1])
				{
					actors.Remove(actors[1]);
					RefreshListOf(CharacterType.actor);
					staffPlacesScroll.Value = 1f;
				}
				else if(u.staff == actors[2])
				{
					actors.Remove(actors[2]);
					RefreshListOf(CharacterType.actor);
					staffPlacesScroll.Value = 1f;
				}
				universalPlanesStaff.Remove(u);
				Destroy(u.gameObject);
				SwitchPhase(Phases.phase2);
			}
			else if(u.scene != null)
			{
				for(int i = 0; i < scenes.Count; i++)
				{
					if(scenes[i].plane == u)
					{
						scenes[i] = new Scene();
					}
					if(universalPlaneScene[i] == u)
					{
						universalPlaneScene[i] = null;
					}
				}
				Destroy(u.gameObject);
				
				RefreshScenesPlanes();
				SwitchPhase(Phases.phase3);
			}
		}
	}
	
	public void SetParams(FilmMaking fm, Scenario sc = null)
	{
		if(sc != null)
			sc.activeIcon.enabled = true;
		hangar = fm;
		gameObject.SetActive(true);
		phase = Phases.phase1;
		SwitchPhase(Phases.phase1);
		GlobalVars.cameraStates = CameraStates.menu;
	}
	
	void AddScript(Scenario s)
	{
		if(s == null)
		{
			return;
		}
		pickedScript = s;
		foreach(Scenario sc in scripts)
		{
			if(sc.activeIcon.enabled)
			{
				sc.activeIcon.enabled = false;
			}
			sc.TakeCharInfoBack();
		}
		
		RefreshListOf(CharacterType.director);

		scripts.Clear();
		SwitchPhase(Phases.phase2);
	}
	
	void AddNewStaff(FilmStaff staff)
	{
		if(staffType == CharacterType.director)
		{
			if(director == null)
			{
				director = staff;
			}
			else if(director != staff)
			{
				director = null;
			}
			else if(director != staff)
			{
				director = staff;
			}
			
		}
		else if(staffType == CharacterType.cameraman)
		{
			if(cameraman == null)
			{
				cameraman = staff;
			}
			else if(cameraman == staff)
			{
				cameraman = null;
			}
			else if(cameraman != staff)
			{
				cameraman = staff;
				cameraman.mark.enabled = true;
			}
			RefreshListOf(CharacterType.actor);
		}
		else if(staffType == CharacterType.actor)
		{
			actors.Add(staff);
		}
		
		StaffManagment.CharInfoToParent(currStaff);
		
		if(director != null && cameraman == null)
		{
			RefreshListOf(CharacterType.cameraman);
		}
		else if(director != null && cameraman != null && actors.Count != 3)
		{
			RefreshListOf(CharacterType.actor);
		}
		
		RefreshPlanes();
		RefreshInformations();
		if(actors.Count == 3)
		{
			SwitchPhase(Phases.phase3);	
			RefreshDecorationsList();
		}
		
	}
	
	void AddScene(SettingButton sb)
	{
		if(sb.decorationItems.Count > 0)
		{
			currSettButt = sb;
			for(int i = 0; i < scenes.Count; i++)
			{
				if(scenes[i].plane == null)
				{
					scenes[i].rarity = sb.rarity;
					scenes[i].setting = sb.setting;
					InstantiateUniPlaneFor(	scenesPlacesScroll.contentContainer.transform, 
											scenePlanes[i].transform.transform.localPosition, null, currSettButt);
					RefreshInformations();
					if(scenes.Count == pickedScript.numberOfScenes)
					{
						SwitchPhase(Phases.phase4);
						return;
					}
					RefreshScenesPlanes();
					RefreshDecorationsList();
					RefreshInformations();
					return;
				}
			}
			
			
			Scene s = new Scene();
			s.rarity = sb.rarity;
			s.setting = sb.setting;
			scenes.Add(s);
			InstantiateUniPlaneFor(	scenesPlacesScroll.contentContainer.transform, 
									scenePlanes[scenes.Count - 1].transform.transform.localPosition, null, currSettButt);
			if(	scenes.Count == pickedScript.numberOfScenes	&& !scenes.Exists(delegate (Scene obj)	{	return obj.plane == null;}))
			{
				SwitchPhase(Phases.phase4);
				return;
			}
			
			RefreshScenesPlanes();
			RefreshDecorationsList();
			RefreshInformations();
			
			//scenesScroll.contentContainer.gameObject.SetActive(false);
		}
	}
	
	void ActivatePhase(GameObject currPhase, GameObject prevPhase, bool forward)
	{
		phase1.SetActive(false);
		phase2.SetActive(false);
		phase3.SetActive(false);
		phase4.SetActive(false);
		print (currPhase.name);
		currPhase.SetActive(true);
		GlobalVars.BlockInput = true;
		if(forward)
		{
			Coroutiner.StartCoroutine(Utils.ShowWindow(currPhase.transform, new Vector3(600, -22.5f, 0), new Vector3(-215, -22.5f, 0), true));
			if(prevPhase != null)
			{
				prevPhase.SetActive(true);
				Coroutiner.StartCoroutine(Utils.ShowWindow(prevPhase.transform, new Vector3(-215, -22.5f, 0), new Vector3(-900, -22.5f, 0), false));
			}
		}
		else
		{
			prevPhase.SetActive(true);
			Coroutiner.StartCoroutine(Utils.ShowWindow(currPhase.transform, new Vector3(-900, -22.5f, 0), new Vector3(-215, -22.5f, 0), true));
			Coroutiner.StartCoroutine(Utils.ShowWindow(prevPhase.transform, new Vector3(-215, -22.5f, 0), new Vector3(600, -22.5f, 0), false));
		}
	}
	
	void RefreshScenariosList()
	{
		foreach(InventoryItem ii in GlobalVars.inventory.items)
		{
			if(ii.GetComponent<Scenario>() != null)
			{
				scripts.Add(ii.GetComponent<Scenario>());
			}
		}
		
		for(int i = 0; i < scripts.Count; i++)
		{
			scripts[i].icon.transform.parent =  scriptsScroll.contentContainer.transform;
			scripts[i].icon.transform.localPosition = new Vector3(-301, (i * -150) + 75, -2);
			scripts[i].icon.transform.localScale = Vector3.one;
			scripts[i].icon.SetActive(true);
			scripts[i].RefreshInfo();
		}
		if(scripts.Count > 2)
		{
			scriptsScroll.ContentLength = (scripts.Count-2) * 150 + 75;
		}
		else
		{
			scriptsScroll.ContentLength = 151;
		}
	}
	
	void RefreshListOf(CharacterType type)
	{
		staffType = type;
		StaffManagment.CharInfoToParent(currStaff);
		currStaff.Clear();
		string tag = type.ToString();
		GameObject[] staff = GameObject.FindGameObjectsWithTag(tag);
		
		foreach(GameObject st in staff)
		{
			FilmStaff fs = st.GetComponent<FilmStaff>();
			if(fs.canBeUsed && staffType != CharacterType.actor)
			{
				currStaff.Add(fs);
			}
			else if(!actors.Exists(delegate(FilmStaff sc)
			{ return sc == fs;}))
			{
				currStaff.Add(fs);
			}
		}
		
		for(int i = 0; i < currStaff.Count; i++)
		{
			if(!currStaff[i].icon.GetComponent<CharInfo>().filmSkills.Exists(delegate (FilmSkills fs)
			{	return fs.genre == pickedScript.genres[0]; }))
			{
				currStaff[i].icon.transform.FindChild("smile").gameObject.SetActive(true);
			}
			else
			{
				currStaff[i].icon.transform.FindChild("smile").gameObject.SetActive(false);
			}
			currStaff[i].icon.transform.parent = staffScroll.contentContainer.transform;
			currStaff[i].icon.transform.localPosition = new Vector3(400 * i, 0, -5);
			currStaff[i].icon.transform.localScale = Vector3.one;
			currStaff[i].icon.SetActive(true);
			currStaff[i].icon.GetComponent<CharInfo>().Refresh();
			//currStaff[i].icon.GetComponent<CharInfo>().SetPricePerUse(script.genres);
		}
		
		if(currStaff.Count > 1)
		{
			staffScroll.ContentLength = currStaff.Count * 400 - 170;
		}
		else
		{
			staffScroll.ContentLength = 400;
		}
	}
	
	void RefreshDecorationsList()
	{
		scenesScroll.contentContainer.gameObject.SetActive(true);
		int i;
		for(i = 0; i < scenes.Count;i++)
		{
			if(scenes[i].plane == null)
			{
				break;
			}
		}
		
		foreach(SettingButton sb in decorations)
		{
			sb.RefreshPlaneInfo(currSceneSetting, GlobalVars.scenesTimers[i]);
			sb.gameObject.SetActive(true);
		}
	}
	
	void FillFinalScreen()
	{
		for(int i = 0; i < universalPlanesStaff.Count; i++)
		{
			universalPlanesStaff[i].transform.parent = finalScreenStaffScroll.contentContainer.transform;
			if(universalPlanesStaff[i].staff == director)
				universalPlanesStaff[i].transform.localPosition = new Vector3(0, 15, 0);
			else if(universalPlanesStaff[i].staff == cameraman)
				universalPlanesStaff[i].transform.localPosition = new Vector3(0, -170, 0);	
			universalPlanesStaff[i].transform.localScale = Vector3.one;
			universalPlanesStaff[i].gameObject.SetActive(true);
			universalPlanesStaff[i].ShowForFinalScreen();
		}
		for(int i = 0; i < actors.Count; i++)
		{
			foreach(UniversalMiniPlane u in universalPlanesStaff)
			{
				if(u.staff == actors[i])
				{
					u.transform.localPosition = new Vector3(0, i * -150 - 370, 0);	
				}
			}
		}
		for(int i = 0; i < universalPlaneScene.Count; i++)
		{
			universalPlaneScene[i].transform.parent = finalScreenScenesScroll.contentContainer.transform;
			universalPlaneScene[i].transform.localPosition = new Vector3(0, i * -185 + 15, 0);
			universalPlaneScene[i].transform.localScale = Vector3.one;
			universalPlaneScene[i].gameObject.SetActive(true);
		}
		if(universalPlaneScene.Count > 2)
		{
			finalScreenScenesScroll.ContentLength = (universalPlaneScene.Count - 2) * 185 + 145;
		}
		else
		{
			finalScreenScenesScroll.ContentLength = 186;
		}
		for(int i = 0; i < finalScreenScenesTitles.Length; i++)
		{
			if(i < universalPlaneScene.Count)
				finalScreenScenesTitles[i].SetActive(true);
			else
				finalScreenScenesTitles[i].SetActive(false);
		}
	}
	
	void SwitchPhase(Phases p)
	{
		switchPhasesSound.Play();
		if(director != null && cameraman != null && actors.Count == 3 && scenes.Count == pickedScript.numberOfScenes
			&& !scenes.Exists(delegate (Scene s)	{	return s.plane == null;}))
		{
			p = Phases.phase4;
		}
		switch(p)
		{
		case Phases.phase1:
			if(phase == Phases.phase1)
			{
				ActivatePhase(phase1, null, true);
				scriptsScroll.Value = 1;
			}
			else if(phase == Phases.phase2)
			{
				ActivatePhase(phase1, phase2, false);
			}
			RefreshScenariosList();
			nameMesh.GetComponent<GetTextFromFile>().SetTextWithIndex(0);
			RefreshInformations();
			break;
		case Phases.phase2:
			RefreshPlanes();
			if(phase == Phases.phase1)
			{
				ActivatePhase(phase2, phase1, true);
				staffPlacesScroll.Value = 0;
			}
			else if(phase == Phases.phase3)
			{
				ActivatePhase(phase2, phase3, false);
			}
			else if(phase == Phases.phase4)
			{
				ActivatePhase(phase2, phase4, false);
			}
			
			break;
		case Phases.phase3:
			RefreshScenesPlanes();
			if(phase == Phases.phase2)
			{
				ActivatePhase(phase3, phase2, true);
				scenesPlacesScroll.Value = 0;
			}
			else if(phase == Phases.phase4)
			{
				ActivatePhase(phase3, phase4, false);
			}
			foreach(UniversalMiniPlane u in universalPlanesStaff)
			{
				u.gameObject.SetActive(false);
			}
			break;
		case Phases.phase4:
			FillFinalScreen();
			if(phase == Phases.phase3)
			{
				ActivatePhase(phase4, phase3, true);
				finalScreenScenesScroll.Value = 0;
				finalScreenStaffScroll.Value = 0;
			}
			else if(phase == Phases.phase2)
			{
				ActivatePhase(phase4, phase2, true);
			}
			break;
		}
		phase = p;
		RefreshInformations();
	}
	
	void RefreshPlanes()
	{
		if(director == null)
		{
			directorPlane.SetActive(true);
			ActivateStaffPlane(directorPlane, false);
		}
		else
		{
			directorPlane.SetActive(false);
		}
		if(cameraman == null)
		{
			cameramanPlane.SetActive(true);
			ActivateStaffPlane(cameramanPlane, false);
		}
		else
		{
			cameramanPlane.SetActive(false);
		}
		for(int i = 0; i < actorsPlanes.Length; i++)
		{
			if(actors.Count > i)
			{
				actorsPlanes[i].SetActive(false);
			}
			else
			{
				actorsPlanes[i].SetActive(true);
				ActivateStaffPlane(actorsPlanes[i], false);
			}
		}
		
		if(director == null)
		{
			ActivateStaffPlane(directorPlane, true);
			staffPlacesScroll.Value = 0;
		}
		else if(cameraman == null)
		{
			ActivateStaffPlane(cameramanPlane, true);
			staffPlacesScroll.Value = 0.25f;
		}
		else if(actors.Count == 0)
		{
			ActivateStaffPlane(actorsPlanes[0], true);
			staffPlacesScroll.Value = 0.5f;
		}
		else if(actors.Count == 1)
		{
			ActivateStaffPlane(actorsPlanes[1], true);
			staffPlacesScroll.Value = 0.75f;
		}
		else if(actors.Count == 2)
		{
			ActivateStaffPlane(actorsPlanes[2], true);
			staffPlacesScroll.Value = 1f;
		}
		if(director != null)
		{
			if(!universalPlanesStaff.Exists(delegate(UniversalMiniPlane u)
			{ return u.staff == director;}))
			{
				InstantiateUniPlaneFor(	staffPlacesScroll.contentContainer.transform, 
										directorPlane.transform.localPosition, director, null);
			}
			foreach(UniversalMiniPlane u in universalPlanesStaff)
			{
				if(u.staff == director)
				{
					u.transform.parent = staffPlacesScroll.contentContainer.transform;
					u.transform.localPosition = directorPlane.transform.localPosition;
					u.transform.localScale = Vector3.one;
					u.gameObject.SetActive(true);
					u.ShowRegular();
				}
			}
		}
		if(cameraman != null)
		{
			if(!universalPlanesStaff.Exists(delegate(UniversalMiniPlane u)
			{ return u.staff == cameraman;}))
			{
				InstantiateUniPlaneFor(	staffPlacesScroll.contentContainer.transform, 
										cameramanPlane.transform.transform.localPosition, cameraman, null);
			}
			foreach(UniversalMiniPlane u in universalPlanesStaff)
			{
				if(u.staff == cameraman)
				{
					u.transform.parent = staffPlacesScroll.contentContainer.transform;
					u.transform.localPosition = cameramanPlane.transform.localPosition;
					u.transform.localScale = Vector3.one;
					u.gameObject.SetActive(true);
					u.ShowRegular();
				}
			}
		}
		for(int i = 0; i < actors.Count; i++)
		{
			if(!universalPlanesStaff.Exists(delegate(UniversalMiniPlane u)
			{ return u.staff == actors[i];}))
			{
				InstantiateUniPlaneFor(	staffPlacesScroll.contentContainer.transform, 
										actorsPlanes[i].transform.transform.localPosition, actors[i], null);
			}
			foreach(UniversalMiniPlane u in universalPlanesStaff)
			{
				if(u.staff == actors[i])
				{
					u.transform.parent = staffPlacesScroll.contentContainer.transform;
					u.transform.localPosition = actorsPlanes[i].transform.localPosition;
					u.transform.localScale = Vector3.one;
					u.gameObject.SetActive(true);
					u.ShowRegular();
				}
			}
		}
	}
	
	void InstantiateUniPlaneFor(Transform parent, Vector3 pos, FilmStaff fs, SettingButton sb)
	{
		GameObject go = Instantiate(universalPlanePrefab) as GameObject;
		go.transform.parent = parent;
		go.transform.localPosition = pos;
		go.transform.localScale = Vector3.one;
		if(fs != null)
		{
			if(fs.icon.GetComponent<CharInfo>().filmSkills.Exists(delegate (FilmSkills f)
			{	return f.genre == pickedScript.genres[0]; }))
			{
				go.GetComponent<UniversalMiniPlane>().SetParamsForCasting(fs, true);
			}
			else
			{
				go.GetComponent<UniversalMiniPlane>().SetParamsForCasting(fs, false);
			}
			universalPlanesStaff.Add(go.GetComponent<UniversalMiniPlane>());
		}
		if(sb != null)
		{
			UniversalMiniPlane plane = go.GetComponent<UniversalMiniPlane>();
			plane.SetParamsForCasting(currSettButt, true);
			
			foreach(Scene sc in scenes)
			{
				if(sc.plane == null)
				{
					sc.plane = plane;
				}
			}
			for(int i = 0; i < universalPlaneScene.Count; i++)
			{
				if(universalPlaneScene[i] == null)
				{
					universalPlaneScene[i] = plane;
					return;
				}
			}
			universalPlaneScene.Add(plane);
		}
	}
	
	void RefreshSetting(bool isNext)
	{
		Setting s = Setting.none;
		if(isNext)
		{
			switch(currSceneSetting)
			{
			case Setting.Fantasy:
				s = Setting.Space;
				break;
			case Setting.Space:
				s = Setting.Modern;
				break;
			case Setting.Modern:
				s = Setting.Adventure;
				break;
			case Setting.Adventure:
				s = Setting.War;
				break;
			case Setting.War:
				s = Setting.Historical;
				break;
			case Setting.Historical:
				s = Setting.Fantasy;
				break;
			}
		}
		else
		{
			switch(currSceneSetting)
			{
			case Setting.Fantasy:
				s = Setting.Historical;
				break;
			case Setting.Space:
				s = Setting.Fantasy;
				break;
			case Setting.Modern:
				s = Setting.Space;
				break;
			case Setting.Adventure:
				s = Setting.Modern;
				break;
			case Setting.War:
				s = Setting.Adventure;
				break;
			case Setting.Historical:
				s = Setting.War;
				break;
			}
		}
		currSceneSetting = s;
		rubricatorGenreMesh.SetTextWithIndex((int)currSceneSetting);
		int i;
		for(i = 0; i < scenes.Count;i++)
		{
			if(scenes[i].plane == null)
			{
				break;
			}
		}
		foreach(SettingButton sb in decorations)
		{
			sb.RefreshPlaneInfo(currSceneSetting, GlobalVars.scenesTimers[i]);
		}
	}
	
	void RefreshScenesPlanes()
	{
		int scenesCount = (int) (pickedScript.numberOfScenes);
		if(scenesCount == 0)
		{
			scenesCount = 1;
		}
		
		if(scenes.Count == 0)
		{
			ActivateStaffPlane(scenePlanes[0], true);
		}
		bool flag = false;
		for(int i = 0; i < scenePlanes.Length; i++)
		{
			if(i >= scenes.Count && i < scenesCount)
			{
				scenePlanes[i].SetActive(true);
				ActivateStaffPlane(scenePlanes[i], false);
			}
			else if(i >= scenes.Count && i >= scenesCount)
			{
				scenePlanes[i].SetActive(false);
			}
			else if(scenes[i].plane == null)
			{
				scenePlanes[i].SetActive(true);
				ActivateStaffPlane(scenePlanes[i],true);
				scenesPlacesScroll.Value = i * 0.25f;
				flag = true;
			}
			else
			{
				scenePlanes[i].SetActive(false);
			}
		}
		if(scenesCount > 2)
		{
			scenesPlacesScroll.ContentLength = (scenesCount - 2) * 150 + 50;
		}
		else
		{
			scenesPlacesScroll.ContentLength = 151;
		}
		
		if(scenePlanes.Length > scenes.Count && !flag)
		{
			ActivateStaffPlane(scenePlanes[scenes.Count], true);
			scenesPlacesScroll.Value = (scenes.Count) * 0.25f;
		}
			
		for(int j = 0; j < scenes.Count; j++)
		{
			for(int i = 0; i < universalPlaneScene.Count; i++)
			{
				if(universalPlaneScene[i] != null)
				{
					if(	universalPlaneScene[i] == scenes[j].plane)
					{
						universalPlaneScene[i].transform.parent = scenesPlacesScroll.contentContainer.transform;
						universalPlaneScene[i].transform.localPosition = scenePlanes[j].transform.transform.localPosition;
						universalPlaneScene[i].transform.localScale = Vector3.one;
						universalPlaneScene[i].gameObject.SetActive(true);
					}
				}
			}
		}
		if(scenes.Count > 0)
		{
			RefreshDecorationsList();
		}
	}
	
	void SetPartsToNewParent(Transform parent)
	{
		infoPart.transform.parent = parent;
		infoPart.transform.localScale = Vector3.one;
		infoPart.transform.localPosition = new Vector3(215, -282.5f, -200);
		nameMesh.transform.parent = parent;
		nameMesh.transform.localScale = Vector3.one;
		nameMesh.transform.localPosition = new Vector3(360, 227.5f, -100);
		bottomPart.transform.parent = parent;
		bottomPart.transform.localScale = Vector3.one;
		bottomPart.transform.localPosition = new Vector3(216, -289, -160);
	}
	
	
	void RefreshInformations()
	{
		switch(phase)
		{
		case Phases.phase1:
			price = 0;
			timeInSec = 0;
			SetPartsToNewParent(phase1.transform);
			infoPart.SetActive(false);
			nameMesh.GetComponent<GetTextFromFile>().SetTextWithIndex(0);
			if(pickedScript == null)
			{
				helperAvatar.SetActive(true);
				helpText.gameObject.SetActive(true);
				helpText.SetTextWithIndex(0);
			}
			else
			{
				helperAvatar.SetActive(false);
			}
			break;
		case Phases.phase2:
			SetPartsToNewParent(phase2.transform);
			//buttonBack.SetActive(true);
			nameMesh.text = pickedScript.name;
			infoPart.SetActive(true);
			if(director == null && cameraman == null && actors.Count == 0)
			{
				infoPart.SetActive(false);
				helpText.gameObject.SetActive(true);
				helpText.SetTextWithIndex(1);
			}
			else
			{
				helpText.gameObject.SetActive(false);
				infoPart.SetActive(true);
				clock.enabled = false;
				timeMesh.text = "";
				price = 0;
				if(director != null)
					price = director.icon.GetComponent<CharInfo>().GetPrice();
				if(cameraman != null)
					price += cameraman.icon.GetComponent<CharInfo>().GetPrice();
				foreach(FilmStaff fs in actors)
				{
					price += fs.icon.GetComponent<CharInfo>().GetPrice();
				}
			}
			if(price == 0)
			{
				moneyMesh.text = "";
			}
			else
			{
				moneyMesh.text = Utils.ToNumberWithSpaces(price.ToString());
			}
			
			
			
			helperAvatar.SetActive(false);
			break;
		case Phases.phase3:
			SetPartsToNewParent(phase3.transform);
			//buttonBack.SetActive(true);
			if(scenes.Count == 0)
			{
				infoPart.SetActive(false);
				clock.enabled = false;
				timeMesh.text = "";
				moneyMesh.text = "";
				helpText.gameObject.SetActive(true);
				helpText.SetTextWithIndex(2);
			}
			else
			{	
				infoPart.SetActive(true);
				helpText.gameObject.SetActive(false);
				clock.enabled = true;
				timeInSec = 0;
				for(int i = 0; i < scenes.Count; i++)
				{
					if(scenes[i].plane != null)
						timeInSec += GlobalVars.scenesTimers[i] * 60;
					else if(i <= 1)
						scenesPlacesScroll.Value = 0;
					else
						scenesPlacesScroll.Value = (i + 1) * 0.2f;
				}
				timeMesh.text = Utils.FormatIntToUsualTimeString(timeInSec, 2);
				moneyMesh.text = Utils.ToNumberWithSpaces(price.ToString());
			}	
			
			rubricatorGenreMesh.transform.parent.gameObject.SetActive(true);
			helperAvatar.SetActive(false);
			break;
		case Phases.phase4:
			SetPartsToNewParent(phase4.transform);
			helperAvatar.SetActive(false);
			//buttonBack.SetActive(true);
			infoPart.SetActive(false);
			helpText.gameObject.SetActive(false);
			finalScreenPrice.text = Utils.ToNumberWithSpaces(price.ToString());
			timeInSec = 0;	
			for(int i = 0; i < scenes.Count; i++)
			{
				if(scenes[i].plane != null)
					timeInSec += GlobalVars.scenesTimers[i] * 60;
			}
			finalScreenTime.text = Utils.FormatIntToUsualTimeString(timeInSec, 2);
			for(int i = 0; i < finalScreenGenres.Length; i++)
			{
				if(i == (int)pickedScript.genres[0])
				{
					finalScreenGenres[i].SetActive(true);
				}
				else
				{
					finalScreenGenres[i].SetActive(false);
				}
			}
			break;			
		}
		nameMesh.maxChars = 40;
		for(int i = 0; i < headerGenres.Length; i++)
		{
			if(pickedScript == null)
			{
				headerGenres[i].SetActive(false);
			}
			else if(i == (int)pickedScript.genres[0])
			{
				headerGenres[i].SetActive(true);
			}
			else
			{
				headerGenres[i].SetActive(false);
			}
		}
	}
	
	void ActivateStaffPlane(GameObject plane, bool isActive)
	{
		plane.transform.FindChild("active").gameObject.SetActive(isActive);
		plane.collider.enabled = isActive;
	}
	
	void Finish()
	{
		if(GlobalVars.money >= price)
		{
			Utils.ChangeMoneyBalance(-price);
			hangar.SetParamsForStartCast(director, cameraman, actors, scenes, pickedScript);
			foreach(UniversalMiniPlane u in universalPlaneScene)
			{
				hangar.usedDecorations.Add(u.gameObject);
				u.gameObject.SetActive(false);
			}
			gameObject.SetActive(false);
			ClearParams(false);
		}
	}
	
	void ClearParams(bool b)
	{
		StaffManagment.CharInfoToParent(currStaff);
		director = null;
		cameraman = null;
		actors = new List<FilmStaff>();
		scenes = new List<Scene>();
		pickedScript = null;
		foreach(Scenario s in scripts)
		{
			s.activeIcon.enabled = false;
			s.TakeCharInfoBack();
		}
		scripts.Clear();
		currStaff.Clear();
		GlobalVars.cameraStates = CameraStates.normal;
		foreach(UniversalMiniPlane u in universalPlanesStaff)
		{
			Destroy(u.gameObject);
		}
		universalPlanesStaff.Clear();
		if(b)
		{	
			foreach(UniversalMiniPlane u in universalPlaneScene)
			{
				if(u != null)
					Destroy(u.gameObject);
			}
			universalPlaneScene.Clear();
		}
		else
			universalPlaneScene = new List<UniversalMiniPlane>();
	}
}
