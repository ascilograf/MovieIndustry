using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//этаж написания сценария
//по нажатию на этаж, если на нем не выполняется ничего, то показываем радиальное меню
//после действий в радиальном меню, если выбрана работа сценаристов
//то запускаем корутину написания сценария
public class ScriptersOfficeStage : MonoBehaviour 
{
	public GameObject timer;
	public tk2dTextMesh timerLeft;
	public tk2dTextMesh timerRight;
	public GameObject checkMark;
	public int scriptersMax;								//максимальное кол-во скриптеров
	public float time;										//время написания сценария
	public float tempTime;									//временная переменная времени
	bool boosted = false;											//есть ли прорыв?
	public float story;										//параметр story, который передастся на новый сценарий
	public GameObject makingScript;							//префаб сценария, забирается из GlobalVars
	public bool isStageBusy;
	public int currScriptersCount = 0;						//текущее кол-во сценаристов
	public int stageIndex;									//индекс этажа
	public int officeLvl;									//уровень офиса
	public GameObject radialMenu;							//радиальное меню
	public List<FilmStaff> scripters;						//сценаристы
	public StageWork stageWork;	
	Scenario scen = null;
	List<FilmGenres> genres = new List<FilmGenres>();
	
	StageWaypoints stageWaypoints;
	
	void Awake()
	{
		stageWaypoints = GetComponent<StageWaypoints>();
		stageWork = GetComponent<StageWork>();
	}
	
	void Start()
	{
		if(GlobalVars.radialMenuScripters != null)
		{
			radialMenu = GlobalVars.radialMenuScripters.gameObject;
		}
		checkMark.SetActive(false);
		stageWork.checkMark.SetActive(false);
		CheckAvailableGenres();
	}
	
	void CheckAvailableGenres()
	{
		List<FilmGenres> freeGenres = new List<FilmGenres>();
		freeGenres.Add(FilmGenres.Action);
		freeGenres.Add(FilmGenres.Comedy);
		freeGenres.Add(FilmGenres.Drama);
		freeGenres.Add(FilmGenres.Horror);
		freeGenres.Add(FilmGenres.Romance);
		freeGenres.Add(FilmGenres.Scifi);
		
		foreach(AvailableGenre ag in GlobalVars.scriptersGenres)
		{
			freeGenres.Remove(ag.genre);
		}

		while((GlobalVars.scriptersGenres.Count - 2) < officeLvl || freeGenres.Count == 0)
		{
			int rand = Random.Range(0, freeGenres.Count);
			AvailableGenre g = new AvailableGenre();
			g.genre = freeGenres[rand];
			g.max = 28;
			GlobalVars.scriptersGenres.Add(g);
			freeGenres.Remove(freeGenres[rand]);
		}	
	}
	
	void OnEnable()
	{
		if(GlobalVars.radialMenuScripters != null)
		{
			radialMenu = GlobalVars.radialMenuScripters.gameObject;
		}
		Messenger<PersonController>.AddListener("Staff on stage", IncrStaffCount);
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
		Messenger<GameObject, int>.AddListener("breakthrough", Boost);
		checkMark.SetActive(false);
		stageWork.checkMark.SetActive(false);
	}
	
	void OnDisable()
	{
		Messenger<PersonController>.RemoveListener("Staff on stage", IncrStaffCount);
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
		Messenger<GameObject, int>.RemoveListener("breakthrough", Boost);
	}
	
	public List<FilmGenres> Genres
	{
		get
		{
			return genres;
		}
	}
	
	void Boost(GameObject go, int percent)
	{
		if(go == gameObject)
		{
			int rand = Random.Range(0, 100);
			int chance = percent;
			if(rand <= chance)
			{
				boosted = true;
				story += 10;
			}
		}
	}
	
	public StageWaypoints WaypointsToOut()
	{
		return stageWaypoints;
	}
	
	void CheckTap(GameObject go)
	{
		if(!isStageBusy && !stageWork.isStageBusy)
		{
			if(go == gameObject) 
			{
				
				radialMenu.SetActive(true);
				RadialMenuScripters r = radialMenu.GetComponent<RadialMenuScripters>();
				r.SetParams(this);		
				r.ShowMenu();
				Utils.FocusOn(transform);
			}
		}
		else if(isStageBusy && currScriptersCount == scripters.Count && time > 0)
		{
			if(go == gameObject)
			{
				GlobalVars.popUpSpeedUpAnyJob.SetParamsForFinish(this);
			}
		}
		else if(go == checkMark)
		{
			GlobalVars.popUpFinishAnyJob.SetParams(scripters, scen, this);
		}
	}
		
	void Finish()
	{
		
		scen.transform.parent = GlobalVars.inventory.transform;
		scen.numberOfScriptWritters = scripters.Count;
		scen.genres = genres;

		
		scen.story = (int)story;
		scen.numberOfScenes = (int)(scen.story / 20);
		GlobalVars.expGain.gainForScriptWriting(scen.story);
		if(scen.numberOfScenes == 0)
		{
			scen.numberOfScenes = 1;
		}
	}
	
	void IncrStaffCount(PersonController person)
	{
		foreach(FilmStaff fs in scripters)
		{
			if(fs.GetComponent<PersonController>() == person)
			{
				currScriptersCount++;
				return;
			}
		}
		if(stageWork.worker != null)
		{
			if(stageWork.worker.GetComponent<PersonController>() == person)
			{
				currScriptersCount++;
			}
		}	
	}
	
	void Update () 
	{
		stageWork.currScriptersCount = currScriptersCount;
		if(stageWork.time == 0 && !isStageBusy)
		{
			currScriptersCount = 0;
		}
		
		if(GlobalVars.cameraStates == CameraStates.menu)
		{
			return;
		}
	}
	
	//старт корутины по созданию сценарися, на вход сценаристы, жанры, сеттинг и время
	public void StartScript(List<FilmStaff> sc, List<FilmGenres> g, int t)
	{
		isStageBusy = true;
		scripters = sc;
		this.genres = new List<FilmGenres>();
		this.genres = g;
		for(int i = 0; i < scripters.Count; i++)
		{
			scripters[i].canBeUsed = false;
			scripters[i].GetComponent<PersonController>().MoveToBuilding(stageWaypoints.waypointsDirectSequence, stageWaypoints.leftPoint, stageWaypoints.rightPoint);
		}
		time = t;
		tempTime = t;						
		StartCoroutine(StartNewScript(sc));
		isStageBusy = true;
		SetStory();
		makingScript = Instantiate(GlobalVars.scenarioPref) as GameObject;
		scen = makingScript.GetComponent<Scenario>();
	}
	
	void SetStory()
	{
		story = 0;
		for(int i = 0; i < genres.Count; i++)
		{
			for(int j = 0; j < scripters.Count; j++)
			{
				int s = 0;
				for(int k = 0; k < scripters[j].skills.Count; k++)
				{
					if(genres[i] == scripters[j].skills[k].genre)
					{			
						s += scripters[j].skills[k].skill;
					}
				}
				if(s == 0)
				{
					for(int k = 0; k < scripters[j].skills.Count; k++)
					{
						if(s == 0)
						{			
							s = scripters[j].skills[k].skill/2;
						}
						else if(scripters[j].skills[k].skill/2 < s)
						{
							s = scripters[j].skills[k].skill/2;
						}
					}
				}
				story += s;
			}
		}
	}
	
	//инициализировать кружочки, по кол-ву персонажей
	/*void InstPageControl(List<Scripter> list)
	{
		float start = 0;
		if(list.Count % 2 == 0)
		{
			start = -(list.Count*25)/2 + 12.5f;
		}
		else if(list.Count % 2 != 0)
		{
			start = -((list.Count - 1)*25)/2;
		}
		for(int i = 0; i < list.Count; i++)
		{
			GameObject go = Instantiate(GlobalVars.pageControlItem) as GameObject;
			go.transform.parent = accept.transform;
			go.transform.localPosition = new Vector3(start + 25 * i, 20, -10);
			go.GetComponent<PageControl>().parent = list[i].gameObject;
			pageContol.Add(go);
		}
	}
	
	//уничтожить кружочки
	void DestroyPageControl()
	{
		if(pageContol.Count > 0)
		{
			for(int i = 0; i < pageContol.Count; i++)
			{
				Destroy(pageContol[i].gameObject);
			}
			pageContol.Clear();
		}
	}
	
	//проверка выбранного персонажа, если он равен паренту кружка - делаем круг закрашенным
	void CheckPageControl(GameObject scripter)
	{
		if(pageContol.Count > 0)
		{
			for(int i = 0; i < pageContol.Count; i++)
			{
				if(pageContol[i].gameObject.GetComponent<PageControl>().parent == scripter)
				{
					pageContol[i].gameObject.GetComponent<PageControl>().isChecked = true;
				}
				else
				{
					pageContol[i].gameObject.GetComponent<PageControl>().isChecked = false;
				}
			}
		}
	}*/
	
	
	//корутина, которая создает новый сценарий
	IEnumerator StartNewScript(List<FilmStaff> sc)
	{
		while(true)
		{
			//если сценарий запущен и кол-во сценаристов равно кол-ву дошедших до здания сценаристов,
			//то отсчитываем время, когда время станет равным нулю - распускаем сценаристов, создаем новый предмет инвентаря -
			//сценарий, присваиваем ему параметры в соответствии с навыком сценаристов, помещаем его в массив предметов инвентаря
			if(!boosted)
			{
				if(currScriptersCount == sc.Count && time <= 0)
				{	
					timerLeft.text = "";
					timerRight.text = "";
					timer.SetActive(false);
					//yield return new WaitForSeconds(time);
					checkMark.SetActive(true);
					Finish();
					yield break;
				}
				else if(currScriptersCount == sc.Count)
				{
					if(!timer.activeSelf)
					{
						timer.SetActive(true);
					}
					isStageBusy = true;
					time -= Time.deltaTime;
					Utils.FormatIntTo2PartsTimeString(timerLeft, timerRight, (int)time);
					/*if(!boosted)
					{
						for(int i = 0; i < sc.Count; i++)
						{
							if(tempTime - time >= 60)
							{
								tempTime = time;
								int rand = Random.Range (0, 100);
								int chanceToBoost = (int)(10 * (sc[i].lvl * 1.5f));
								if(rand <= chanceToBoost)
								{
									GlobalVars.popUpBreakthrough.SetParams(sc[i], gameObject);
								}
							}
						}
					}*/
					//yield return new WaitForSeconds(Time.deltaTime);
				}
			}
			yield return 0;
		}
	}
	
	
	
	//Система "прорыва" (увеличение сюжета с процентным шансом)
	
}
