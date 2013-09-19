using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//класс строения, используется для выбора места строительства, строительства и улучшения здания
//на старте инициализируется заглушка "под строительством" и выбирается место - куда ставить новое здание
//далее начинается процесс строительства, зовутся свободные рабочие
//после строительства заглушка меняется на построенное здание, при апгрейде происходит то же самое, кроме выбора места постройки.
public class Construct : MonoBehaviour 
{
	enum States
	{
		underConstruction,
		replace,
		builded,
	}
	
	public BuildingType type;					//тип здания
	public int currWorkersCount;				//кол-во рабочих которое имеется сейчас
	//Building thisBuilding;					//параметры здания взятые из GlobalVars
	Transform thisTr;							//трансформ этого объекта
	GameObject tempGo = null;					//временный GameObject
	public Stages stage = Stages.first;			//этажность здания
	public float timeToBuild;					//время строительства
	float time;
	//bool underConstr = true;					//находится ли здание в режиме строительства			
	
	Vector3 tempPos;							//временная позиция
	tk2dSprite sprite1;							//первый спрайт для изменения цвета
	tk2dSprite sprite2;							//второй спрайт для изменения цвета
	public GameObject startBuilding;			//GameObject на который повешен звук начала строительства
	public GameObject finishBuilding;			//GameObject на который повешен звук окончания строительства
	public List<GameObject> workersStaff;		//список рабочих в доме
	public GameObject buttonPlace;
	public GameObject buttonCancelPlace;
	
	public tk2dTextMesh timeLeftPartMesh;
	public tk2dTextMesh timeRightPartMesh;
	public GameObject timeParent;
	
	public GameObject[] buildings;				//объекты зданий
	public GameObject[] upgrades;				//объекты апгрейдов
	
	public GameObject callSpeedUpMenu;
	
	public GameObject buildTiles;
	
	GameObject thisBuilding;
	
	GameObject placeBuildingPrefab;
	GameObject acceptButton;
	GameObject declineButton;
	Vector3 startPos;
	States state;
	
	PlaceBuilding placeBuildingButtons;
	
	/// <summary>
	/// Awake this instance.
	///инициализация трансформа
	void Awake()
	{
		thisTr = transform;
	}
	
	void Start()
	{
		Messenger<GameObject>.AddListener("Staff on construct", IncrStaffCount);
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
		Messenger.Broadcast("Check workers count");
		Messenger<Construct>.Broadcast("buildUpgradeBuilding", this);
	}
	
	void CheckTap(GameObject g)
	{
		if(placeBuildingPrefab != null)
		{
			if(acceptButton == g && canPlaceHere)
			{	
				ActivateColliders(true);
				SwitchState(States.builded);
			}
			else if(declineButton == g)
			{
				transform.position = startPos;
				ActivateColliders(true);
			}
		}
		else if(g == callSpeedUpMenu && state == States.underConstruction && currWorkersCount == workersStaff.Count)
		{
			GlobalVars.popUpSpeedUpAnyJob.SetParamsForFinish(this);
		}
	}
	
	void Update()
	{
	}
	
	void SwitchState(States s)
	{
		switch(s)
		{
		case States.builded:
			callSpeedUpMenu.SetActive(false);
			callSpeedUpMenu.collider.enabled = false;
			if(buildTiles != null)
				buildTiles.SetActive(false);
			break;
		case States.replace:
			callSpeedUpMenu.SetActive(false);
			callSpeedUpMenu.collider.enabled = false;
			if(buildTiles != null)
				buildTiles.SetActive(true);
			break;
		case States.underConstruction:
			if(type != BuildingType.buildersHut)
			{
				callSpeedUpMenu.SetActive(true);
				callSpeedUpMenu.collider.enabled = true;
			}
			else
			{
				callSpeedUpMenu.SetActive(false);
				callSpeedUpMenu.collider.enabled = false;
			}
			break;
		}
		state = s;
	}
	
	void IncrStaffCount(GameObject building)
	{
		if(building == gameObject)
		{
			currWorkersCount++;
		}
	}
	
	//старт постройки/апгрейда здания
	public void StartBuildNewOffice(int time, int workers, int price)
	{
		timeToBuild = time;
		price = 0;
		SwitchUnderConstruction(stage);
		timeParent.SetActive(true);
		StartCoroutine(BuildNewOffice(time, workers));		
		SwitchState(States.underConstruction);
		//Messenger<GameObject, float>.Broadcast("New Activity Stack", gameObject, timeToBuild);
	}
	
	IEnumerator BuildNewOffice(float time, int workers)
	{
		while(true)
		{
			if(workers > workersStaff.Count)
			{
				GameObject[] go = GameObject.FindGameObjectsWithTag ("worker");
				
				for(int i = 0; i < go.Length; i++)
				{
					//Если кол-во элементов в списке рабочих меньше чем нужное кол-во рабочих - продолжаем звать новых рабочих
					if(workers > workersStaff.Count)
					{
						if(go[i].GetComponent<PersonController>().IsBusy() != true)
						{
							go[i].GetComponent<PersonController>().ToConstruct(gameObject);
							workersStaff.Add(go[i]);
						}
					}
				}
				yield return new WaitForSeconds(Time.deltaTime);
			}
			//Если кол-во рабочих равно нужному нам, то проигрываем звук, а через 3 секунды строим новое здание, уничтожаем леса и увеличиваем этаж
			if(currWorkersCount == workers && timeToBuild <= 0)
			{
				
				Destroy(tempGo);
				SwitchStage(stage);
				
				//для каждого рабочего в списке: заполняем массив дверей - которые он должен пройти чтобы выйти из здания и запускаем движение к первой двери
				for(int i = 0; i < workersStaff.Count; i++)
				{
					PersonController pc = workersStaff[i].GetComponent<PersonController>();
					pc.MoveToBuilding(gameObject.transform, pc.floor);
				}
				
				//чистим список рабочих, выставляем положение камеры в обычное, проигрываем анимацию и обнуляем текущее кол-во рабочих
				workersStaff.Clear();
				finishBuilding.audio.Play();
				currWorkersCount = 0;
				timeParent.SetActive(false);
				SwitchState(States.builded);
				yield break;
			}
			else if(currWorkersCount == workers)
			{
				timeToBuild -= Time.deltaTime;
				//yield return new WaitForSeconds(Time.deltaTime);
			}
			Utils.FormatIntTo2PartsTimeString(timeLeftPartMesh,timeRightPartMesh, (int)timeToBuild);
			//yield return new WaitForSeconds(Time.deltaTime);
			yield return 0;
		}
	}
		
	//float t = 2;
	bool canPlaceHere = true;
	bool placed = true;
	tk2dSprite[] sprites;
	Vector3 v3;
	
	//сбор всех спрайтов здания для перекраски 
	public void ChangePlace()
	{
		sprites = gameObject.GetComponentsInChildren<tk2dSprite>();
		GameObject g = Instantiate(GlobalVars.placeBuildingPrefab) as GameObject;
		g.transform.parent = transform;
		g.transform.localPosition = Vector3.zero;
		placeBuildingPrefab = g;
		g.GetComponent<PlaceBuilding>().enabled = false;
		acceptButton = placeBuildingPrefab.transform.FindChild("buttonAccept").gameObject;
		declineButton = placeBuildingPrefab.transform.FindChild("buttonCancel").gameObject;
		
		
		ActivateColliders(false);
		
		acceptButton.collider.enabled = true;
		declineButton.collider.enabled = true;
		collider.enabled = true;
		
		startPos = transform.position;
		placed = false;
		//ColorizePrefab(Color.green);
		StartCoroutine(MoveBuilding());
		SwitchState(States.replace);
	}
	
	void ActivateColliders(bool setTo)
	{
		BoxCollider[] col = gameObject.GetComponentsInChildren<BoxCollider>();
		foreach(BoxCollider b in col)
		{
			b.enabled = setTo;
		}
		if(setTo)
		{
			Destroy(placeBuildingPrefab);
			placeBuildingPrefab = null;
			canPlaceHere = true;
			placed = true;
			ColorizePrefab(Color.white);
		}
	}
	
	//двигать здание по центру камеры, при нажатии в течение 2-х секунда здание строится на новом месте
	IEnumerator MoveBuilding()
	{
		while(true)
		{
			if(placeBuildingPrefab == null)
			{
				yield break;
			}
			Vector3 pos = transform.position;
			if((int)(Camera.main.transform.position.x/55) > (int)pos.x/57)
			{
				pos.x = (int)Camera.main.transform.position.x;
			}
			else if((int)(Camera.main.transform.position.x/55) < (int)pos.x/57)
			{
				pos.x = (int)Camera.main.transform.position.x;
			}
			//pos.x = Camera.main.transform.position.x;
			pos.y = -150;
			pos.z = GlobalVars.BUILDING_LAYER;
			transform.position = pos;
			yield return 0;
		}
	}
	
	void ColorizePrefab(Color color)
	{
		foreach(tk2dSprite s in sprites)
		{
			if(	s.transform.parent != acceptButton.transform ||
				s.transform.parent != declineButton.transform)
			{
				s.color = color;
			}
			else
			{
				s.color = Color.white;
			}
		}
	}
	
	//Если коллайдер входит в другой коллайдер с тэгом здание, то построить нельзя
	void OnTriggerEnter(Collider coll)
	{
		if(coll.CompareTag("building") && !placed)
		{
			//ColorizePrefab(Color.red);
			canPlaceHere = false;
		}
	}
	
	//Если коллайдер уже находится в другом коллайдере с тэгом здание, то построить нельзя	
	void OnTriggerStay(Collider coll)
	{
		if(coll.CompareTag("building") && !placed)
		{
			canPlaceHere = false;
			//ColorizePrefab(Color.red);
		}
	}
	
	//Если коллайдер выходит из другого коллайдера с тэгом здание, то построить можно
	void OnTriggerExit(Collider coll)
	{
		if(coll.CompareTag("building") && !placed)
		{
			canPlaceHere = true;
			//ColorizePrefab(Color.green);
		}
	}
	
	//Инициализируем нужный префаб из UnderConstr, присваиваем спрайты для перекраски в красный/зеленый цвета
	void UnderConstr()
	{
		//underConstr = true;
		
		Destroy(tempGo);
		tempGo = Instantiate(GlobalVars.underConstr[(int)stage].buildPrefab, thisTr.position, Quaternion.identity) as GameObject;
		tempGo.transform.parent = thisTr;
	}
	
	//смена этажа
	void SwitchStage(Stages stage)
	{
		switch(stage)
		{
		case Stages.first:
			HideBuildingsExcept(buildings[0]);
			break;
		case Stages.second:
			HideBuildingsExcept(buildings[1]);
			break;
		case Stages.third:
			HideBuildingsExcept(buildings[2]);
			break;
		case Stages.four:
			HideBuildingsExcept(buildings[3]);
			break;
		case Stages.five:
			HideBuildingsExcept(buildings[4]);
			break;
		}
		Messenger<Construct>.Broadcast("buildUpgradeBuilding", this);
	}
	
	//смена андерконстракшн
	void SwitchUnderConstruction(Stages stage)
	{
		switch(stage)
		{
		case Stages.first:
			timeParent.transform.parent = upgrades[0].transform;
			HideBuildingsExcept(upgrades[0]);
			break;
		case Stages.second:
			HideBuildingsExcept(upgrades[1]);
			timeParent.transform.parent = upgrades[1].transform;
			break;
		case Stages.third:
			HideBuildingsExcept(upgrades[2]);
			timeParent.transform.parent = upgrades[2].transform;
			break;
		case Stages.four:
			HideBuildingsExcept(upgrades[3]);
			timeParent.transform.parent = upgrades[3].transform;
			break;
		case Stages.five:
			HideBuildingsExcept(upgrades[4]);
			timeParent.transform.parent = upgrades[4].transform;
			break;	
		}
		
	}
	
	//прячем все объекты зданий и андерконстр. кроме выбранного объекта
	void HideBuildingsExcept(GameObject go)
	{
		gameObject.SetActive(false);
		gameObject.SetActive(true);
		startBuilding.SetActive(true);
		finishBuilding.SetActive(true);
		foreach(GameObject g in buildings)
		{
			if(g == go)
			{
				g.SetActive(true);
			}
			else 
			{
				g.SetActive(false);
			}
		}
		foreach(GameObject g in upgrades)
		{
			if(g == go)
			{
				g.SetActive(true);
			}
			else 
			{
				g.SetActive(false);
			}
		}

	}
	
	public bool CheckUpgradeAvailability()
	{
		switch(type)
		{
		case BuildingType.hangar:
			FilmMaking fm = transform.GetComponentInChildren<FilmMaking>();
			if(fm.busy)
			{
				return false;
			}
			break;
		case BuildingType.office:
			MarketingOfficeStage[] mo = transform.GetComponentsInChildren<MarketingOfficeStage>();
			foreach(MarketingOfficeStage m in mo)
			{
				if(m.isBusy)
				{
					return false;
				}
			}
			break;
		case BuildingType.postproduction:
			PostProdOfficeStage[] ppo = transform.GetComponentsInChildren<PostProdOfficeStage>();
			foreach(PostProdOfficeStage pp in ppo)
			{
				if(pp.busy)
				{
					return false;
				}
			}
			break;
		case BuildingType.scriptWrittersOffice:
			ScriptersOfficeStage[] so = transform.GetComponentsInChildren<ScriptersOfficeStage>();
			foreach(ScriptersOfficeStage s in so)
			{
				if(s.isStageBusy)
				{
					print ("busy");
					return false;
				}
			}
			break;
		}
		StageWork[] sw = transform.GetComponentsInChildren<StageWork>();
		foreach(StageWork s in sw)
		{
			if(s.isStageBusy)
			{
				print ("busy");
				return false;
			}
		}
		return true;
	}
}
