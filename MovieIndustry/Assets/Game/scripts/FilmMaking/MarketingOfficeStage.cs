using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//контроль этажа офиса макретологов, отсюда запускается радиальное меню маркетологов
//также здесь происходит работа продюссеров над фильмом
public class MarketingOfficeStage : MonoBehaviour 
{
	public float timeToMarketing;						//время на маркетинг
	//public int currProdCount;							//текущее кол-во продюсеров
	//bool isMarketingStarted;							//идет ли маркетинг
	//Vector3 tapDown;									//время клика										
	public bool isBusy;									//занят ли этаж
	public int officeLvl;								//уровень офиса
	public int officeStage;								//этаж офиса
	public int currProdCount;							//текущее кол-во продюссеров
	public int producersNeeded;
	
	public List<FilmStaff> producers;
	
	StageWork stageWork;
	
	void Awake()
	{
		stageWork = GetComponent<StageWork>();
	}
	
	void Start()
	{
		Messenger<PersonController>.AddListener("Staff on stage", IncrStaffCount);
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject go)
	{
		if(go == this.gameObject)
		{
			if(!isBusy && !stageWork.isStageBusy)
			{
				
				GlobalVars.radialMenuProducers.SetParams(this);
				GlobalVars.radialMenuProducers.ShowMenu();
				Utils.FocusOn(transform);
				GlobalVars.cameraStates = CameraStates.menu;
			}
		}
	}
	
	void IncrStaffCount(PersonController person)
	{
		if(stageWork.worker != null)
		{
			if(stageWork.worker.GetComponent<PersonController>() == person)
			{
				currProdCount++;
			}
		}	
	}
	
	void Update () 
	{
		stageWork.currScriptersCount = currProdCount;
		if(stageWork.time == 0 && !isBusy)
		{
			currProdCount = 0;
		}
			
		if(GlobalVars.cameraStates == CameraStates.menu)
		{
			return;
		}
		
		if(!isBusy && !stageWork.isStageBusy)
		{
			if(GlobalVars.defaultLayerTarget == gameObject)
			{
				GlobalVars.defaultLayerTarget = null;
				print ("it's ok");
				GlobalVars.tapSound.Play();
				Utils.FocusOn(transform);
					//GlobalVars.radialMenuProducers.gameObject.SetActiveRecursively(true);
				GlobalVars.radialMenuProducers.ShowMenu();
				GlobalVars.radialMenuProducers.SetParams(this);
				isBusy = true;
				GlobalVars.cameraStates = CameraStates.menu;
			}
			//else
			//{
			//	print ("captain, we have problems");
			//}
		}
	
		//если бало нажатие по офису и никаких работ не ведется - вызываем радиальное меню
		/*if(Input.GetMouseButtonUp(0) && !isBusy && !stageWork.isStageBusy && Vector3.Distance(tapDown, Input.mousePosition) < GlobalVars.tapTremble)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				if(hit.collider == collider)
				{
					Utils.FocusOn(transform);
					//GlobalVars.radialMenuProducers.gameObject.SetActiveRecursively(true);
					GlobalVars.radialMenuProducers.ShowMenu();
					GlobalVars.radialMenuProducers.SetParams(this);
					isBusy = true;
					GlobalVars.cameraStates = CameraStates.menu;
				}
			}
		}*/
	}
	
	//public void StartMarketingProgram()
	//{
	
	/*public void SetParams(FilmItem f)
	{
		isBusy = true;
		film = f;
		time = 0;
		StartCoroutine(StartMarketing());
	}
	
	//старт маркетинга
	IEnumerator StartMarketing()
	{
		while(true)
		{
			if(!isMarketingStarted && producersNeeded > producers.Count)
			{
				GameObject[] go = GameObject.FindGameObjectsWithTag ("producer");
				for(int i = 0; i < go.Length; i++)
				{
					if(producersNeeded > producers.Count)
					{
						go[i].GetComponent<PersonController>().MoveToBuilding(stageWaypoints.waypointsDirectSequence, stageWaypoints.leftPoint, stageWaypoints.rightPoint);
						producers.Add(go[i].GetComponent<FilmStaff>());
					}
				}
				if(maxMarketings == producers.Count)
				{
					isMarketingStarted = true;
				}
			}
			if(isMarketingStarted)
			{
				if(timeToMarketing <= 0)
				{	
					//PrepareForRental();
					yield break;
				}
				else if(currProdCount == producers.Count)
				{
					timeToMarketing -= Time.deltaTime;
					//yield return new WaitForSeconds(Time.deltaTime);
				}
			}
			//yield return 0;
		}
	}
	
	//подготовка к прокату
	public void PrepareForRental(FilmItem currFilm, int result)
	{
		
		//обсчет времени жизни фильма в секундах
		currFilm.lifeTime = (int)(currFilm.acting + currFilm.direction + currFilm.cinematography + currFilm.visuals + currFilm.story) * 60;
		//плюс процент от маркетинга
		currFilm.lifeTime += (int)((currFilm.lifeTime/100) * result);
		//высчитывание общега множителя ревенуэ
		float revenuePercent = currFilm.acting * 0.15f + currFilm.direction * 0.15f + currFilm.story * 0.2f + currFilm.cinematography * 0.1f + currFilm.visuals * 0.2f;
		//высчет общей прибыли
		currFilm.Revenue = (int)(currFilm.budget + (currFilm.budget * 0.1f) + (currFilm.budget * revenuePercent));
		currFilm.Revenue += (int)(currFilm.Revenue * (GlobalVars.cinemasRevenue / 100));
		currFilm.Revenue = (int)(currFilm.Revenue * currFilm.fit);
		currFilm.revemuePerSec = (int)(currFilm.Revenue / currFilm.lifeTime);
		//isMarketingStarted = false;
		//GlobalVars.inventory.items.Add(currFilm.GetComponent<InventoryItem>());
		GlobalVars.inventory.items.Remove(currFilm.GetComponent<InventoryItem>());
		GlobalVars.worldRental.films.Add(currFilm);
		currFilm.transform.parent = GlobalVars.inventory.transform;
		currFilm.GetComponent<InventoryItem>().inventoryTexture = currFilm.GetComponent<InventoryItem>().inventoryTexture2;
		//Doors[] doors = GetComponentsInChildren<Doors>();
		//StaffManagment.SetStaffFree(producers, doors);
		isBusy = false;
	}*/
	
	
	

	
	//инициализировать кружочки, по кол-ву персонажей
	/*void InstPageControl(List<FilmItem> list)
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
			go.transform.parent = filmScroll.transform;
			go.transform.localPosition = new Vector3(start + 25 * i, -45, -10);
			go.GetComponent<PageControl>().parent = list[i].gameObject;
			pageControl.Add(go);
		}
	}
	
	//уничтожить кружочки
	void DestroyPageControl()
	{
		if(pageControl.Count > 0)
		{
			for(int i = 0; i < pageControl.Count; i++)
			{
				Destroy(pageControl[i].gameObject);
			}
			pageControl.Clear();
		}
	}
	
	//проверка выбранного персонажа, если он равен паренту кружка - делаем круг закрашенным
	void CheckPageControl(GameObject go)
	{
		if(pageControl.Count > 0)
		{
			for(int i = 0; i < pageControl.Count; i++)
			{
				if(pageControl[i].gameObject.GetComponent<PageControl>().parent == go)
				{
					pageControl[i].gameObject.GetComponent<PageControl>().isChecked = true;
				}
				else
				{
					pageControl[i].gameObject.GetComponent<PageControl>().isChecked = false;
				}
			}
		}
	}*/
}
