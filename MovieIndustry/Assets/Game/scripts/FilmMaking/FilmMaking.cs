using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//создание фильма, здесь выполняется последовательность действий, нужная для съемок фильма
//сбор рабочих - строительство декораций - сбор персонала - съемка сцены - снос декораций,
//цикл повторяется соответственно кол-ву сцен
public class FilmMaking : MonoBehaviour 
{
	public Scenario script;						//сценариий
	public GameObject timer;
	public tk2dTextMesh timerLeft;
	public tk2dTextMesh timerRight;
	public int budget;							//бюджет фильма
	public int acting;							//мастерство актера
	public int direction;						//мастерство режиссера
	public int cinematography;					//мастерство оператора
	public float fit;							//множитель заработка
	public FilmStaff director;					//режиссер фильма
	public FilmStaff cameraman;					//оператор
	public List<FilmStaff> actors;				//актеры
	public int scenesCount;						//кол-во сцен
	
	public int maxWorkers;						//макс. кол-во рабочих
	public bool busy = false;					//занят ли ангар
	public GameObject buildDecorIcon;			//иконка постройки новых декораций
	public GameObject destroyDecorIcon;			//иконка уничтожения декораций
	public GameObject boostIcon;				//иконка буста
	
	public GameObject office;					//объект офиса
	
	public List<GameObject> workers;			//рабочие на декорациях
	public int workersCount;					//текущее кол-во рабочих
	
	public int decorBuildPrice = 1000;			//цена постройки декора
	
	
	public float timeForBuildDecor;				//время для постройки декораций
	public float timeForMakeScene;				//время для снятия сцены фильма
	public float timeForDestroyDecor;			//время для уничтожения текущих декораций
	public float time;							//время, к нему присваиваются другие переменные времени, в зависимости от задачи
	
	public List<FilmStaff> staff;				//весь персонал, занятый в фильме
	public int staffCount;						//текущее кол-во персонала в ангаре
	public Costumes[] costumes;
	public Vector3[] positions;
	public CostumePosition[] costumePositions;
	public List<Scene> scenesToCast;
	
	public Waypoint[] actorsWay;
	public Waypoint[] directorWay;
	public Waypoint[] cameramanWay;
	public tk2dSpriteAnimator tableAnimation; 
	
	
	GameObject currDecor;						//текущие декорации
	Vector3 tapDown, tapUp;
	StageWork stageWork;
	Setting setting;
	RarityLevel rarity;
	Costumes tempCostume;
	public List<GameObject> instancedCostumes = new List<GameObject>();
	
	List<Setting> settingsForFilm = new List<Setting>();
	List<RarityLevel> rarityLvlsForFilm = new List<RarityLevel>();
	FilmItem film;
	
	StageWaypoints stageWaypoints;
	
	public List<GameObject> usedDecorations;
	//на всякий случай скрываем все кнопки
	void Start () 
	{
		directorWay[directorWay.Length - 1].pointPos.gameObject.SetActive(false);
		stageWaypoints = GetComponent<StageWaypoints>();
		stageWork = GetComponent<StageWork>();
		destroyDecorIcon.SetActive(false);
		boostIcon.SetActive(false);
		buildDecorIcon.SetActive(false);
		time = 0;
		Messenger<PersonController>.AddListener("Staff on stage", IncrStaffCount);
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
		Messenger<PersonController>.AddListener("Actor is come", ActorIsCome);
	}
	
	void CheckTap(GameObject go)
	{
		if(busy && staffCount == staff.Count && time > 0)
		{
			if(go == gameObject)
			{
				GlobalVars.popUpSpeedUpAnyJob.SetParamsForFinish(this);
			}
		}
		else if(go == gameObject && !busy && !stageWork.isStageBusy)
		{
			GlobalVars.radialMenuHangar.gameObject.SetActive(true);
			RadialMenuScripters r = GlobalVars.radialMenuHangar.GetComponent<RadialMenuScripters>();
			r.SetParams(this);		
			r.ShowMenu();
			busy = true;
			Utils.FocusOn(transform);
		}
		/*else if(go == boostIcon && scenesCount > 0)
		{
			GlobalVars.popUpSelectDecoration.SetParams(this);
			boostIcon.SetActive(false);
		}*/
		else if(go == boostIcon && scenesCount == scenesToCast.Count)
		{
			GlobalVars.popUpFinishAnyJob.SetParamsForFilm(staff, scenesToCast, film, this);
		}
	}
	
	void IncrStaffCount(PersonController person)
	{
		foreach(FilmStaff fs in staff)
		{
			if(fs.GetComponent<PersonController>() == person)
			{
				if(person.type == CharacterType.director)
				{
					Vector3 v3 = person.transform.position;
					v3.y += 1.5f;
					person.legs.gameObject.SetActive(false);
					person.body.enabled = false;
					person.head.enabled = false;
					person.body.Stop();
					person.head.Stop();
					//tableAnimation.Play();
					StartCoroutine(SitDownMrDirector(person.transform, person.transform.position, v3));
				}
				else
				{
					staffCount++;
				}
				return;
			}
		}
		foreach(GameObject worker in workers)
		{
			if(worker.GetComponent<PersonController>() == person)
			{
				workersCount++;
				return;
			}
		}
		if(stageWork.worker != null)
		{
			if(stageWork.worker.GetComponent<PersonController>() == person)
			{
				staffCount++;
			}
			
		}
	}
	
	IEnumerator SitDownMrDirector(Transform d, Vector3 fromPos, Vector3 toPos, float t = 0)
	{
		while(true)
		{
			if(t >= 1)
			{
				staffCount++;
				yield break;
			}
			t += Time.deltaTime;
			d.transform.position = Vector3.Lerp(fromPos, toPos, t);
			yield return 0;
		}
	}
	
	void ActorIsCome(PersonController pc)
	{
		for(int i = 0; i < actors.Count; i++)
		{
			if(actors[i].GetComponent<PersonController>() == pc)
			{
				staffCount++;
				costumePositions[i].mirror.Play();
				costumePositions[i].mirror.gameObject.SetActive(true);
				StartCoroutine(PlayWearCostumeAnim(i, actors[i].GetComponent<PersonController>().gender()));
				actors[i].gameObject.SetActive(false);
				return;
			}
		}
	}
	
	IEnumerator PlayWearCostumeAnim(int ind, Gender g, float t = 0, bool b = false)
	{
		while(true)
		{
			yield return new WaitForSeconds((costumePositions[ind].mirror.CurrentClip.frames.Length / costumePositions[ind].mirror.CurrentClip.fps)/2);
			InstantiateCostume(setting, g, rarity, ind);
			yield return new WaitForSeconds((costumePositions[ind].mirror.CurrentClip.frames.Length / costumePositions[ind].mirror.CurrentClip.fps)/2);
			costumePositions[ind].mirror.Stop();
			costumePositions[ind].mirror.gameObject.SetActive(false);
			yield break;
		}
	}

	void Update () 
	{
		stageWork.currScriptersCount = staffCount;
		if(stageWork.time == 0 && !busy)
		{
			workersCount = 0;
			staffCount = 0;
		}
	}
	
	//установить кол-во сцен
	public void SetScenesCount(int i)
	{
		//scenesCount = i;
	}
	
	//установка строящегося здания вместо текущего ангара
	/*void InstUnderConstr()
	{
		GameObject go = Instantiate(GlobalVars.underConstr[0].buildPrefab) as GameObject;
		go.transform.parent = transform;
		go.transform.localPosition = new Vector3(0,15, -1);
		currDecor = go;
	}*/
	
	
	
	//позвать свободных рабочих
	/*public void CallWorkers()
	{
		workers.Clear();
		workersCount = 0;
		StartCoroutine(SearchForWorkers());
	}
	
	//найти свободных рабочих
	IEnumerator SearchForWorkers()
	{
		while(true)
		{
			if(maxWorkers > workers.Count)
			{
				GameObject[] go = GameObject.FindGameObjectsWithTag ("worker");
					
				for(int i = 0; i < go.Length; i++)
				{
					//Если кол-во элементов в списке рабочих меньше чем нужное кол-во рабочих - продолжаем звать новых рабочих
					if(maxWorkers > workers.Count)
					{
						if(!go[i].GetComponent<PersonController>().busy)
						{
							go[i].GetComponent<PersonController>().MoveToBuilding(stageWaypoints.waypointsDirectSequence, stageWaypoints.leftPoint, stageWaypoints.rightPoint);
							go[i].GetComponent<PersonController>().busy = true;
							workers.Add(go[i]);
						}
					}
				}
			}
			if(workersCount == maxWorkers)
			{
				
				ActivateBuild(scenesToCast[scenesToCast.Count - scenesCount].setting, 1, scenesToCast[scenesToCast.Count - scenesCount].rarity);
				yield break;
			}
			yield return 0;
		}
	}*/
	
	//обновить персонал
	void RefreshStaff()
	{
		staff.Clear();
		staff.Add(director.GetComponent<FilmStaff>());
		staff.Add(cameraman.GetComponent<FilmStaff>());
		foreach(FilmStaff fs in actors)
		{
			staff.Add (fs.GetComponent<FilmStaff>());
		}
	}
	
	//позвать персонал
	public void CallStaff()
	{
		//RefreshStaff();
		staffCount = 0;
		for(int i = 0; i < actors.Count; i++)
		{
			actorsWay[2] = null;
			actorsWay[2] = costumePositions[i].waypoint;
			
			actors[i].GetComponent<PersonController>().MoveToBuilding(new Waypoint[]{actorsWay[0], actorsWay[1], costumePositions[i].waypoint});
			actors[i].GetComponent<PersonController>().busy = true;
			print("actor way 2: " + actorsWay[2].pointPos.gameObject);
		}
		print("actor way 2: " + actors[0].GetComponent<PersonController>().waypoints[2].pointPos.name);
		director.GetComponent<PersonController>().MoveToBuilding(directorWay);
		director.GetComponent<PersonController>().busy = true;
		cameraman.GetComponent<PersonController>().MoveToBuilding(cameramanWay);
		cameraman.GetComponent<PersonController>().busy = true;
		time = GlobalVars.scenesTimers[scenesCount]  * 60;
		
	}
	
	
	
	//постройка декораций
	IEnumerator BuildDecorations()
	{
		while(true)
		{	
			if(time <= 0)
			{
				Destroy(currDecor);
				buildDecorIcon.SetActive(false);
				destroyDecorIcon.SetActive(false);
				boostIcon.SetActive(false);
				currDecor = null;
				
				currDecor = Instantiate(DecorationsWithRarity(rarity, setting)) as GameObject;
				currDecor.transform.parent = transform;
				currDecor.transform.localPosition = new Vector3(0,0,GlobalVars.DECOR1_LAYER);
				
				print ("Workers count: " + workers.Count);
				for(int i = 0; i < workers.Count; i++)
				{
					PersonController pc = workers[i].GetComponent<PersonController>();
					Vector3 v3 = pc.transform.position;
					v3.y = GlobalVars.buildedFloorsHeight[pc.GetFloor()].yMax;
					pc.transform.position = v3;
					print ("Worker is making his way out: " + pc.name);
					pc.MoveOutOfBuilding(stageWaypoints.waypointsReverseSequence);
				}
				time = 0;
				
				
				
				timer.SetActive(false);
				directorWay[directorWay.Length - 1].pointPos.gameObject.SetActive(true);
				break;
			}
			else
			{
				time -= Time.deltaTime;
				if(!timer.activeSelf)
				{
					timer.SetActive(true);
				}
				Utils.FormatIntTo2PartsTimeString(timerLeft,timerRight, (int)time);
				//yield return new WaitForSeconds(Time.deltaTime);
			}
			yield return 0;
		}
	}
	
	
	GameObject DecorationsWithRarity(RarityLevel r, Setting s)
	{
		foreach(Decorations decor in GlobalVars.decorations)
		{
			if(setting == decor.setting)
			{
				int rand;
				switch(r)
				{
				case RarityLevel.common:
					rand = Random.Range(0, decor.commonDecorationPrefab.Length);
					return decor.commonDecorationPrefab[rand];
					//break;
				case RarityLevel.rare:
					rand = Random.Range(0, decor.rareDecorationPrefab.Length);
					return decor.rareDecorationPrefab[rand];
					//break;
				case RarityLevel.unique:
					rand = Random.Range(0, decor.uniqueDecorationPrefab.Length);
					return decor.uniqueDecorationPrefab[rand];
					//break;
				}
			}
		}
		return null;
	}
	
	
	//инициализыировать фильм в инвентарь, передать ему все нужные параметры
	void InstFilm()
	{
		GameObject go = Instantiate(GlobalVars.filmItemPref) as GameObject;
		go.transform.parent = GlobalVars.inventory.transform;
		FilmItem film = go.GetComponent<FilmItem>();
		film.transform.parent = GlobalVars.inventory.transform;
		film.name = script.name;
		film.genres = script.genres;
		film.settings = settingsForFilm;
		film.rarityOfScenes = rarityLvlsForFilm;
		settingsForFilm = new List<Setting>();
		rarityLvlsForFilm = new List<RarityLevel>();
		film.story = script.story;
		acting = Formulas.FilmActingDirectiogCinematography(actors, script.genres, "actingDivisor");
		direction = Formulas.FilmActingDirectiogCinematography(director, script.genres, "directionDivisor");
		cinematography = Formulas.FilmActingDirectiogCinematography(cameraman, script.genres, "cinematographyDivisor");
		foreach(FilmStaff fs in staff)
		{
			fs.busy = false;
		}
		
		#region получение опыта персонажами
		GlobalVars.charExpGain.DirectorMakeFilmExpGain(director);
		GlobalVars.charExpGain.CameramanMakeFilmExpGain(cameraman);
		foreach(FilmStaff fs in actors)
		{
			GlobalVars.charExpGain.ActorMakeFilmExpGain(fs);
		}
		#endregion
		
		
		
		film.budget = budget;
		film.fit = fit;
		film.acting = acting;
		film.direction = direction;
		film.cinematography = cinematography;
		
		this.film = film;
		GlobalVars.inventory.items.Remove(script.GetComponent<InventoryItem>());
		Destroy(script.gameObject);
		GlobalVars.expGain.gainForFinishingFilm(film);
		//PrepareForRental(film, 1);
		
		timer.SetActive(false);
	}
	
	public void ClearParams()
	{
		script = null;						//сценариий
		budget = 0;							//бюджет фильма
		acting = 0;							//мастерство актера
		direction = 0;						//мастерство режиссера
		cinematography = 0;					//мастерство оператора
		fit = 0;							//множитель заработка
		director = null;					//режиссер фильма
		cameraman = null;					//оператор
		actors.Clear();						//актеры
		scenesCount = 0;					//кол-во сцен
		
		busy = false;						//занят ли ангар
		print ("workers have been cleared");
		workers.Clear();					//рабочие на декорациях
		workersCount = 0;					//текущее кол-во рабочих
		
		decorBuildPrice = 1000;				//цена постройки декора
		
		staff.Clear();						//весь персонал, занятый в фильме
		staffCount = 0;						//текущее кол-во персонала в ангаре
		
		currDecor = null;
	}
	
	//определение текущего сеттинга, пола актера и рарности сцены, потом показ его
	void InstantiateCostume(Setting s, Gender g, RarityLevel r, int index = 0)
	{
		tk2dSpriteAnimator animSprite = null;
		GameObject costumeToAdd = null;
		int rand = Random.Range(0, 2);
		foreach(Costumes c in costumes)
		{
			if(c.setting == s)
			{
				tempCostume = c;
				if(g == Gender.female)
				{
					if(r == RarityLevel.common && c.commonFemale.Length > 0)
					{
						costumeToAdd = Instantiate(c.commonFemale[rand]) as GameObject;
					}
					if(r == RarityLevel.rare && c.rareFemale.Length > 0)
					{
						costumeToAdd = Instantiate(c.rareFemale[rand]) as GameObject;
					}
					if(r == RarityLevel.unique && c.uniqueFemale.Length > 0)
					{
						costumeToAdd = Instantiate(c.uniqueFemale[rand]) as GameObject;
					}
				}
				else if(g == Gender.male )
				{
					if(r == RarityLevel.common && c.commonMale.Length > 0)
					{
						costumeToAdd = Instantiate(c.commonMale[rand]) as GameObject;
					}
					if(r == RarityLevel.rare && c.rareMale.Length > 0)
					{
						costumeToAdd = Instantiate(c.rareMale[rand]) as GameObject;
					}
					if(r == RarityLevel.unique && c.uniqueMale.Length > 0)
					{
						costumeToAdd = Instantiate(c.uniqueMale[rand]) as GameObject;
					}
				}
			}
		}
		
		instancedCostumes.Add(costumeToAdd);
		animSprite = costumeToAdd.GetComponent<tk2dSpriteAnimator>();
		costumeToAdd.SetActive(true);
		costumeToAdd.transform.parent = this.transform;
		costumeToAdd.transform.localPosition = costumePositions[index].position;
		
		if(index == 1)
		{
			animSprite.transform.localScale = new Vector3(-1, 1, 1);
		}
		else
		{
			animSprite.transform.localScale = Vector3.one;
		}
		animSprite.playAutomatically = false;
		animSprite.Stop ();
	}
	
	void PlayAnimationsOfCostumes()
	{
		foreach(GameObject c in instancedCostumes)
		{
			c.GetComponent<tk2dSpriteAnimator>().Play("play");
		}
	}
	
	public void SetParamsForStartCast(FilmStaff inDirector, FilmStaff inCameraman, List<FilmStaff> inActors, List<Scene> inScenes, Scenario inScript)
	{
		scenesCount = 0;//inScript.numberOfScenes;
		director = inDirector;
		cameraman = inCameraman;
		actors = inActors;
		scenesToCast = inScenes;
		script = inScript;
		
		staff.Add(cameraman);
		staff.Add(director);
		staff.Add(actors[0]);
		staff.Add(actors[1]);
		staff.Add(actors[2]);
		CallStaff();
		SetupNewScene();
	}
	
	//запуск корутины старта постройки декораций
	public void SetupNewScene()
	{
		if(currDecor != null)
		{
			Destroy(currDecor);
			
			
			GlobalVars.expGain.gainForDecorChanging();
			
			//CallWorkers();
			time = 0;
			timer.SetActive(false);
			if(scenesCount >= scenesToCast.Count)
			{
				staffCount = 0;
				InstFilm();//print ("inst film");
				return;
			}
		}
			
		setting = scenesToCast[scenesCount].setting;
		fit = 1;
		rarity = scenesToCast[scenesCount].rarity;
		//time = timeForBuildDecor;
		//GlobalVars.money -= script.numberOfScenes * decorBuildPrice;
		//budget += script.numberOfScenes * decorBuildPrice;
		settingsForFilm.Add(scenesToCast[scenesCount].setting);
		rarityLvlsForFilm.Add(scenesToCast[scenesCount].rarity);
		
		currDecor = Instantiate(DecorationsWithRarity(rarity, setting)) as GameObject;
		currDecor.transform.parent = transform;
		currDecor.transform.localPosition = new Vector3(0,0,GlobalVars.DECOR1_LAYER);
		
		StartMakingFilm();
		directorWay[directorWay.Length - 1].pointPos.gameObject.SetActive(true);
		//time = 10;
		//StartCoroutine(BuildDecorations());
	}
	
	//запуск новой сцены
	void StartMakingFilm()
	{
		//
		
		time = GlobalVars.scenesTimers[scenesCount] * 60;
		StartCoroutine(MakingFilm());
	}
	
	//запуск съъемок фильма
	IEnumerator MakingFilm()
	{
		while(true)
		{
			if(time <= 0)
			{
				tableAnimation.Play("playReverse");
				yield return new WaitForSeconds(tableAnimation.CurrentClip.frames.Length/tableAnimation.CurrentClip.fps);
				
				//RefreshStaff();
				scenesCount++;
				
				
				Utils.FormatIntTo2PartsTimeString(timerLeft, timerRight, 0);
				timer.SetActive(false);
				time = 0;
				if(scenesCount >= scenesToCast.Count)
				{
					boostIcon.SetActive(true);
					directorWay[directorWay.Length - 1].pointPos.gameObject.SetActive(false);
					foreach(GameObject go in instancedCostumes)
					{
						Destroy(go);
					}
					instancedCostumes.Clear();
					for(int i = 0; i < staff.Count; i++)
					{
						PersonController pc = staff[i].GetComponent<PersonController>();
						pc.gameObject.SetActive(true);
						staff[i].icon.SetActive(false);
						Vector3 v3 = pc.transform.position;
						v3.y = GlobalVars.buildedFloorsHeight[pc.GetFloor()].yMax;
						pc.transform.position = v3;
						if(pc.type == CharacterType.director)
						{
							pc.body.enabled = true;
							pc.head.enabled = true;
							pc.transform.position = directorWay[directorWay.Length - 1].pointPos.position;
							pc.legs.gameObject.SetActive(true);
						}
					}
					SetupNewScene();
				}
				else
				{
					tableAnimation.Stop();
				
					for(int i = 0; i < actors.Count; i++)
					{
						costumePositions[i].mirror.Play();
						costumePositions[i].mirror.gameObject.SetActive(true);
						StartCoroutine(PlayWearCostumeAnim(i, actors[i].GetComponent<PersonController>().gender()));
					}
					yield return new WaitForSeconds((costumePositions[0].mirror.CurrentClip.frames.Length / costumePositions[0].mirror.CurrentClip.fps)/3);
					foreach(GameObject go in instancedCostumes)
					{
						Destroy(go);
					}
					instancedCostumes.Clear();
					SetupNewScene();
				}
				yield break;
			}
			else if(staffCount == staff.Count)
			{
				Utils.FormatIntTo2PartsTimeString(timerLeft,timerRight, (int)time);
				time -= Time.deltaTime;	
				if(!timer.activeSelf && !costumePositions[0].mirror.gameObject.activeSelf)
				{
					timer.SetActive(true);
					tableAnimation.Play("play");
					yield return new WaitForSeconds(tableAnimation.CurrentClip.frames.Length/tableAnimation.CurrentClip.fps);
					PlayAnimationsOfCostumes();
				}
			}
			yield return 0;
		}
	}
}
