using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
//контроль офиса постпрода, сюда помещается фильм из инвентаря, после окончания работ увеличивает 
//параметр визуальщины у фильма и помещает обратно в инвентарь
public class PostProdOfficeStage : MonoBehaviour 
{
	public FilmItem film;							//предмет фильма
	public List<FilmStaff> postProdWorkers;			//работники постропрода
	public int currWorkers;							//текущее кол-во построд работников
	public GameObject iconFinishPostProd;			//иконка начала постпрода
	public GameObject iconBoost;					//иконка буста
	public int visuals;								//параметр visual, который добавится к фильму
	public float timeForEffects;					//время продолжительности работы над эффектами
	public bool busy;								//занято ли здание
	public int floor;								//максимальное кол-во рабочих
	bool boosted;									//сработал ли буст
	public float time;								//время
	public List<FilmStaff> staff;
	float tempTime;									//доп. время
	public StageWork stageWork;
	Vector3 tapDown;
	
	StageWaypoints stageWaypoints;
	
	void Start()
	{
		stageWaypoints = GetComponent<StageWaypoints>();
		stageWork = GetComponent<StageWork>();
	}

	void OnEnable ()
	{
		ShowHideObject(iconFinishPostProd, false);
		ShowHideObject(iconBoost, false);
		Messenger<PersonController>.AddListener("Staff on stage", IncrStaffCount);
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void OnDisable()
	{
		Messenger<PersonController>.RemoveListener("Staff on stage", IncrStaffCount);
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject go)
	{
		if(go == iconFinishPostProd)
		{
			GlobalVars.popUpFinishAnyJob.SetParams(postProdWorkers, film, null, this);
		}
		if(go == gameObject && !busy && !stageWork.isStageBusy)
		{
			GlobalVars.radialMenuPostProd.ShowMenu();
			GlobalVars.radialMenuPostProd.SetParams(this);
			GlobalVars.cameraStates = CameraStates.menu;
			Utils.FocusOn(transform);
		}
		else if(busy && currWorkers == postProdWorkers.Count && time > 0)
		{
			if(go == gameObject)
			{
				GlobalVars.popUpSpeedUpAnyJob.SetParamsForFinish(this);
			}
		}
	}
	
	void IncrStaffCount(PersonController person)
	{
		foreach(FilmStaff f in postProdWorkers)
		{
			if(f.GetComponent<PersonController>() == person)
			{
				currWorkers++;
				if(postProdWorkers.Count == currWorkers && busy)
				{
					StartCoroutine(StartPostProd());
				}
				return;
			}
		}
		if(stageWork.worker != null)
		{
			if(stageWork.worker.GetComponent<PersonController>() == person)
			{
				currWorkers++;
			}
		}	
	}
	
	public StageWaypoints WaypointsToOut()
	{
		return stageWaypoints;
	}
	
	public void SetParams(FilmItem f, List<FilmStaff> list)
	{
		busy = true;
		visuals = 0;
		film = f;
		postProdWorkers = list;
		time = 0;
		foreach(FilmStaff fs in list)
		{
			visuals += fs.icon.GetComponent<CharInfo>().otherSkills.skill ;
			time += fs.icon.GetComponent<CharInfo>().otherSkills.skill * 20;
		}
		CallStaff();
	}
	
	void Update () 
	{
		stageWork.currScriptersCount = currWorkers;
		if(stageWork.time == 0 && !busy)
		{
			currWorkers = 0;
		}
	}
	
	void PrepareForRental(FilmItem currFilm, int result)
	{
		
		//обсчет времени жизни фильма в секундах
		//currFilm.lifeTime = (int)(currFilm.acting + currFilm.direction + currFilm.cinematography + currFilm.visuals + currFilm.story) * 60;
		//плюс процент от маркетинга
		//currFilm.lifeTime += (int)((currFilm.lifeTime/100) * result);
		//высчитывание общега множителя ревенуэ
		float revenuePercent = currFilm.acting * 0.15f + currFilm.direction * 0.15f + currFilm.story * 0.2f + currFilm.cinematography * 0.1f + currFilm.visuals * 0.2f;
		//высчет общей прибыли
		currFilm.busy = true;
		currFilm.Revenue = Mathf.RoundToInt(currFilm.budget + (currFilm.budget * 0.1f) + (currFilm.budget * revenuePercent));
		currFilm.Revenue += Mathf.RoundToInt(currFilm.Revenue * (GlobalVars.cinemasRevenue / 100));
		//currFilm.Revenue = Mathf.RoundToInt(currFilm.Revenue * currFilm.fit);
		currFilm.revemuePerSec = Mathf.RoundToInt(currFilm.Revenue / currFilm.lifeTime);
		currFilm.transform.parent = GlobalVars.inventory.transform;
		//currFilm.GetComponent<InventoryItem>().inventoryTexture = currFilm.GetComponent<InventoryItem>().inventoryTexture2;
	}
	
	//позвать персонал в здание
	public void CallStaff()
	{
		foreach(FilmStaff fs in postProdWorkers)
		{
			fs.GetComponent<PersonController>().MoveToBuilding(stageWaypoints.waypointsDirectSequence, stageWaypoints.leftPoint, stageWaypoints.rightPoint);
		}
	}
	
	//добавление эффектов к фильму
	IEnumerator StartPostProd()
	{
		while(true)
		{
			if(time <= 0 && postProdWorkers.Count == currWorkers)
			{
				stageWork.timerParent.SetActive(false);
				//распускаем рабочих и проставляем им путь до дверей
				
				//добавляем переменную эффектов к фильму и возвращение его в инвентарь, обнуление переменных
				film.visuals = visuals;
				film.busy = false;
				film.transform.parent = GlobalVars.inventory.transform;
				film.lifeTime = visuals * 10;
				PrepareForRental(film, 1);
				GlobalVars.inventory.items.Add(film.GetComponent<InventoryItem>());
				GlobalVars.expGain.gainForPostProd(film.visuals);
				iconFinishPostProd.SetActive(true);
				
				yield break;
			}
			else if(postProdWorkers.Count == currWorkers)
			{
				if(!stageWork.timerParent.activeSelf)
				{
					stageWork.timerParent.SetActive(true);
				}
				
				time -= Time.deltaTime;
				Utils.FormatIntTo2PartsTimeString(stageWork.timerLeft, stageWork.timerRight, (int)time);
			}
			yield return 0;
		}
	}
	
	
	//показать/скрыть объект
	public void ShowHideObject(GameObject g, bool b)
	{
		Transform[] tr = g.GetComponentsInChildren<Transform>(true);
		foreach(Transform t in tr)
		{
			t.gameObject.SetActive(b);
		}
	}
	
	//обнулить все параметры
	public void ClearParams()
	{
		//film = null;
		postProdWorkers = new List<FilmStaff>();
		currWorkers = 0;
		visuals = 0;
		busy = false;
		time = 0;
	}
}
