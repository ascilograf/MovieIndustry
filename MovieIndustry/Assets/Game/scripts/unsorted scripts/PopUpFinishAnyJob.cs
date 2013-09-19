using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//скрипт для меню завершения какой-либо из работ

public class PopUpFinishAnyJob : MonoBehaviour 
{
	enum States
	{
		film,
		script,
		vfx,
	}
	
	//params for all
	public GameObject acceptButton;
	public GameObject closeButton;
	public GameObject editName;
	public tk2dTextMesh headerText;
	public tk2dTextMesh genresText;
	public GetTextFromFile buttonText;
	public GameObject uniPlanePrefab;
	public SwipeItems infosCharInfo;
	
	//params for vfx/film making
	public ProgressBar storyProgressbar;
	public ProgressBar actingProgressbar;
	public ProgressBar directionProgressbar;
	public ProgressBar cameraWorkProgressbar;
	public ProgressBar visualsProgressbar;
	
	//params for script
	public ProgressBar storyScriptProgressbar;
	public tk2dTextMesh genresScriptText;
	
	public tk2dUIScrollableArea staffScrollArea;
	public List<FilmStaff> staff;
	public List<Scene> scenes;
	public FilmItem film;
	public Scenario scenario;
	
	//params for film making
	public GameObject filmMakingStep;
	public GameObject[] genreIcons;
	public ProgressBar overallRating;
	public Transform staffParent;
	public tk2dUIScrollableArea scenesScroll;
	public GameObject[] finalScreenScenesTitles;
	
	List<UniversalMiniPlane> staffPlanes = new List<UniversalMiniPlane>();
	List<UniversalMiniPlane> scenePlanes = new List<UniversalMiniPlane>();
	
	States state;
	
	ScriptersOfficeStage scriptStage;
	public FilmMaking filmMaking;
	PostProdOfficeStage vfxStage;

	void Start () 
	{
		//SetParamsForFilm (staff, scenes, film, filmMaking);
	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
		Messenger.AddListener("Menu appear", StartFillBars);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
		Messenger.RemoveListener("Menu appear", StartFillBars);
	}
	
	void CheckTap(GameObject g)
	{
		if(g.name == "closeButton")
		{
			Finish ();
			//gameObject.SetActive(false);
			//GlobalVars.cameraStates = CameraStates.normal;
			//StaffManagment.CharInfoToParent(staff);
		}
		else if(g.name == "acceptButton")
		{
			switch(state)
			{
			case States.film:
				if(GlobalVars.popUpShortCut.HaveFreeOffice(BuildingType.postproduction))
				{
					Finish ();
					GlobalVars.popUpShortCut.SetParams(BuildingType.postproduction, film, null);
				}
				else
				{
					GlobalVars.popUpShortCut.SetParams(BuildingType.postproduction, film, null);
				}
				break;
			}
		}
		else if(g == editName)
		{
			GlobalVars.filmsWithoutNames.Add(scenario);
		}
	}
		
	public void SetParams(List<FilmStaff> fs, FilmItem inFilm, FilmMaking hangar = null, PostProdOfficeStage postProdsOffice = null)
	{
		gameObject.SetActive(true);
		GlobalVars.cameraStates = CameraStates.menu;
		staff = fs;
		film = inFilm;
		editName.SetActive(false);
		Utils.SetProgressBarValue(storyProgressbar.stars, 0);	
		Utils.SetProgressBarValue(actingProgressbar.stars, 0);	
		Utils.SetProgressBarValue(directionProgressbar.stars, 0);
		Utils.SetProgressBarValue(cameraWorkProgressbar.stars, 0);
		
		if(film.visuals > 0)
		{
			vfxStage = postProdsOffice;
			visualsProgressbar.parentObj.SetActive(true);
			Utils.SetProgressBarValue(visualsProgressbar.stars, 0);
			state = States.vfx;
		}
		else
		{
			filmMaking = hangar;
			visualsProgressbar.parentObj.SetActive(false);
			state = States.film;
		}
		storyScriptProgressbar.parentObj.SetActive(false);
		genresScriptText.gameObject.SetActive(false);
		Utils.SetText(headerText, film.name);
		Utils.SetText(genresText, Utils.GenresInText(film.genres));
		StaffManagment.SetCharInfos(staff, staffScrollArea.contentContainer.transform, 350, 90);
		foreach(FilmStaff f in staff)
		{
			f.icon.GetComponent<CharInfo>().GainExp(50);
		}
		SetSwipe();
	}
	
	public void SetParamsForFilm(List<FilmStaff> fs, List<Scene> sc, FilmItem inFilm, FilmMaking hangar)
	{
		gameObject.SetActive(true);
		filmMakingStep.SetActive(true);
		GlobalVars.cameraStates = CameraStates.menu;
		staff = fs;
		film = inFilm;
		editName.SetActive(false);
		int rating = (int)((film.acting + film.direction + film.visuals + film.story)/4);
		Utils.SetProgressBarValue(overallRating.stars, rating);
		filmMaking = hangar;
		headerText.text = film.name;
		
		for(int i = 0; i < genreIcons.Length; i++)
		{
			if(i == (int)film.genres[0])
			{
				genreIcons[i].SetActive(true);
			}
			else
			{
				genreIcons[i].SetActive(false);
			}
		}
		
		FillFinishCastingFilm();
	}
	
	void FillFinishCastingFilm()
	{
		int ind = 0;
		state = States.film;
		for(int i = 0; i < staff.Count; i++)
		{
			GameObject g = Instantiate(uniPlanePrefab) as GameObject;
			g.transform.parent = staffParent;
			g.transform.localScale = Vector3.one;
			UniversalMiniPlane up = g.GetComponent<UniversalMiniPlane>();
			up.SetParamsForfinishFilm(staff[i], 50);
			up.staff = staff[i];
			staffPlanes.Add(up);
			PersonController pc = staff[i].GetComponent<PersonController>();
			if(pc.type == CharacterType.director)
				g.transform.localPosition = new Vector3(0, 15, 0);
			else if(pc.type == CharacterType.cameraman)
				g.transform.localPosition = new Vector3(0, -170, 0);	
			else if(pc.type == CharacterType.actor)
			{
				g.transform.localPosition = new Vector3(0, ind * -150 - 370, 0);
				ind++;
			}
		}
		
		for(int i = 0; i < filmMaking.usedDecorations.Count; i++)
		{
			GameObject g = filmMaking.usedDecorations[i];
			g.SetActive(true);
			g.transform.localScale = Vector3.one;
			g.transform.parent = scenesScroll.contentContainer.transform;
			g.transform.localPosition = new Vector3(0, i * -185 + 15, 0);
		}
		if(filmMaking.usedDecorations.Count > 2)
		{
			scenesScroll.ContentLength = (filmMaking.usedDecorations.Count - 2) * 185 + 145;
		}
		else
		{
			scenesScroll.ContentLength = 186;
		}
		for(int i = 0; i < finalScreenScenesTitles.Length; i++)
		{
			if(i < filmMaking.usedDecorations.Count)
				finalScreenScenesTitles[i].SetActive(true);
			else
				finalScreenScenesTitles[i].SetActive(false);
		}
	}
	
	
	public void SetParams(List<FilmStaff> fs, Scenario inScript, ScriptersOfficeStage scriptersStage)
	{
		gameObject.SetActive(true);
		GlobalVars.cameraStates = CameraStates.menu;
		staff = fs;
		scenario = inScript;
		state = States.script;
		scriptStage = scriptersStage;
		
		editName.SetActive(true);
		
		
		genresScriptText.gameObject.SetActive(true);
		Utils.SetText(genresScriptText, Utils.GenresInText(scenario.genres));
		Utils.SetProgressBarValueInTime(storyScriptProgressbar.stars, 0);
		storyProgressbar.parentObj.gameObject.SetActive(false);
		actingProgressbar.parentObj.gameObject.SetActive(false);
		directionProgressbar.parentObj.gameObject.SetActive(false);
		cameraWorkProgressbar.parentObj.gameObject.SetActive(false);
		visualsProgressbar.parentObj.SetActive(false);
		genresText.gameObject.SetActive(false);
		
		Utils.SetText(headerText, "Set scenario name");
		StaffManagment.SetCharInfos(staff, staffScrollArea.contentContainer.transform, 350, 90);
		foreach(FilmStaff f in staff)
		{
			f.icon.GetComponent<CharInfo>().GainExp(50);
		}
		SetSwipe();
	}
	
	void StartFillBars()
	{
		switch(state)
		{
		case States.film:
			StartCoroutine(Utils.SetProgressBarValueInTime(actingProgressbar.stars, film.acting));
			StartCoroutine(Utils.SetProgressBarValueInTime(storyProgressbar.stars, film.story));
			StartCoroutine(Utils.SetProgressBarValueInTime(cameraWorkProgressbar.stars, film.cinematography));
			StartCoroutine(Utils.SetProgressBarValueInTime(directionProgressbar.stars, film.direction));
			break;
		case States.script:
			
			StartCoroutine(Utils.SetProgressBarValueInTime(storyScriptProgressbar.stars, scenario.story));
			break;
		case States.vfx:
			StartCoroutine(Utils.SetProgressBarValueInTime(actingProgressbar.stars, film.acting));
			StartCoroutine(Utils.SetProgressBarValueInTime(storyProgressbar.stars, film.story));
			StartCoroutine(Utils.SetProgressBarValueInTime(cameraWorkProgressbar.stars, film.cinematography));
			StartCoroutine(Utils.SetProgressBarValueInTime(directionProgressbar.stars, film.direction));
			StartCoroutine(Utils.SetProgressBarValueInTime(visualsProgressbar.stars, film.visuals));
			break;
		}
	}
	
	public void Finish()
	{
		switch(state)
		{
		case States.film:
			GlobalVars.inventory.items.Add(film.GetComponent<InventoryItem>());
			film.busy = false;
			filmMaking.busy = false;
			
			filmMaking.boostIcon.SetActive(false);
			for(int i = 0; i < staff.Count; i++)
			{
				staff[i].busy = false;
				staff[i].GetComponent<PersonController>().MoveOutOfBuilding(filmMaking.GetComponent<StageWaypoints>().waypointsReverseSequence);
				staff[i].GetComponent<PersonController>().busy = false;		
				//staff[i].icon.GetComponent<CharInfo>().CommitLvl();
			}
			
			StaffManagment.CharInfoToParent(staff);
			
			filmMaking.ClearParams();
			
			foreach(UniversalMiniPlane u in staffPlanes)
			{
				Destroy(u.gameObject);
			}
			foreach(UniversalMiniPlane u in scenePlanes)
			{
				Destroy(u.gameObject);
			};
			filmMaking.usedDecorations.Clear();
			filmMaking.scenesToCast.Clear();
			staffPlanes.Clear();
			scenePlanes.Clear();
			Messenger<FilmItem>.Broadcast("shootFilm", film);
			//GlobalVars.popUpShortCut.SetParams(BuildingType.postproduction, film, null);
			gameObject.SetActive(false);
			break;
		case States.script:
			//добавляем созданный сценарий в инвентарь
			GlobalVars.inventory.items.Add(scenario.GetComponent<InventoryItem>());
			foreach(FilmStaff s in staff)
			{
				//staff.canBeUsed = true;
				s.busy = false;
				s.GetComponent<FilmStaff>().mark.enabled = false;
				s.GetComponent<PersonController>().MoveOutOfBuilding(scriptStage.WaypointsToOut().waypointsReverseSequence);
				s.icon.GetComponent<CharInfo>().CommitLvl();
			}
			
			StaffManagment.CharInfoToParent(staff);
			
			scriptStage.isStageBusy = false;
			scriptStage.currScriptersCount = 0;
			//GlobalVars.filmsWithoutNames.Add(scen);
			scriptStage.checkMark.SetActive(false);
			GlobalVars.popUpShortCut.SetParams(BuildingType.hangar, null,scenario);
			Messenger<Scenario>.Broadcast("writeScript", scenario);
			break;
		case States.vfx:
			GlobalVars.cameraStates = CameraStates.normal;
			film.busy = false;
			//GlobalVars.popUpFinishMakingFilm.SetParams(film);
			foreach(FilmStaff fs in staff)
			{
				//fs.canBeUsed = true;
				fs.busy = false;
				fs.GetComponent<FilmStaff>().mark.enabled = false;
				fs.GetComponent<PersonController>().MoveOutOfBuilding(vfxStage.WaypointsToOut().waypointsReverseSequence);	
				fs.icon.GetComponent<CharInfo>().CommitLvl();
			}
			
			StaffManagment.CharInfoToParent(staff);
			
			vfxStage.iconFinishPostProd.SetActive(false);
			vfxStage.ClearParams();
			GlobalVars.popUpShortCut.SetParams(BuildingType.office, film);
			Messenger<FilmItem>.Broadcast("vfxFilm", film);
			break;
		}
		gameObject.SetActive(false);
		GlobalVars.cameraStates = CameraStates.normal;
	}
	
	public void SetSwipe()
	{
		if(staff.Count > 1)
		{
			staffScrollArea.ContentLength = (staff.Count - 1) * 350 + 220;
		}
		else
		{
			staffScrollArea.ContentLength = 351;
		}
	}
	
	public void SetName(string s)
	{
		if(scenario != null)
		{
			scenario.name = s;
			Utils.SetText(headerText, s);
		}
	}
	
	void Update () 
	{
	
	}
}
