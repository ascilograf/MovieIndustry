using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;

public enum PersMovingStates
{
	none,
	moveToBuilding,
	moveInsideBuilding,	
	moveOutOfBuilding,
}

//Скрипт по управлению персонажем, рандомное перемещение, поведение рабочих, проигрывание анимаций.
public class PersonController: MonoBehaviour 
{
	
	public bool isPremium = false;						//премиальный ли персонаж
	public CharAnimationType animType;					//тип анимации, который сейчас проигрывается	
	CharAnimationType oldAnimType;
	public CharacterType type;							//тип персонажа, выбор из списка: рабочий ,актер и пр.
		
	public BoundingBox frame;							//рамка, ограничивающая движение персонажа по сцене
	//public string name;									//имя персонажа
//	public filmsGenres filmsExp;						//параметры опыта, разделен по 7 категориям
	public float speed;									//скорость передвижения
	Transform thisTr;									//ссылка на трансформ этого объекта
	Vector3 movement;									//точка, в которую будет двигаться персонаж
	float time = 0;										//время, используется для разных действий
	float tempTime = 0;									//используется для запоминания случайного времени
	
	public tk2dSpriteAnimator head;							//анимированный спрайт головы
	public tk2dSpriteAnimator body;							//анимированный спрайт тела
	public tk2dSpriteAnimator legs;							//анимированный спрайт ног
	
	int genderIndex;									//индекс пола
	public bool busy;									//занят ли работой персонаж?
	public Transform target;							//цель, в которую двигается персонаж
	Vector3 tempPos;									//временная переменная позиции, используется для разных целей
	public List<Doors> doors;
	int currDoorIndex;
	bool animCanPlay = false;
	
	public int floor;									//этаж, на котором сейчас находится персонаж, 0 означает граунд.
	float tempY;
	public PersMovingStates state;
	
	public bool selfInst = false;
	public GameObject doorsCollider;
	public BoundingBox bb = new BoundingBox();
	
	public Waypoint[] waypoints;
	
	Transform leftPoint, rightPoint;
	
	public GroundWaypoint currWaypoint;
	public GroundWaypoint endWaypoint;
	public GroundWaypoint[] way;
	
	public int currentSeekerWaypoint;
	Seeker seeker;
	public Path path;
	
	void Awake()
	{
		seeker = GetComponent<Seeker>();
		thisTr = transform;
	}

	
	void Start () 
	{
		
		if(selfInst)
		{
			InstantiatePerson(type);
		}
		
		//StartMoving();
	}
	
	
	
	//инициализация персонажа по частям, на вход тип персонажа
	public void InstantiatePerson(CharacterType t)
	{
		type = t;
		if(type == CharacterType.worker && GetComponent<FilmStaff>() != null)
		{	
			GetComponent<FilmStaff>().enabled = false;	
		}
		transform.position = new Vector3(transform.position.x, transform.position.y, -GlobalVars.CHARACTER_FREE_LAYER);
		StartCoroutine(MoveInsideBuilding());
		GameObject parent = GameObject.Find(tag + "s");
		if(parent != null)
		{
			transform.parent = parent.transform;
		}
		floor = 0;
		//на старте мы случайно выбираем из двух полов
		genderIndex = (int)Random.Range (0,2);
		int charIndex= 0;
		for(int j = 0; j < GlobalVars.characters.Length; j++)
		{
			if(GlobalVars.characters[j].personType == type)
			{
				charIndex = j;
			}
		}
		if(!isPremium)
		{
			//для не премиумных персонажей на старте выдаем имена по индексу пола
			if(genderIndex == 0)
			{
				gameObject.name = Textes.namesMale[Random.Range(0, Textes.lastNames.Count)] + " " + Textes.lastNames[Random.Range(0, Textes.namesMale.Count)];
			}
			if(genderIndex == 1)
			{
				gameObject.name = Textes.namesFemale[Random.Range(0, Textes.lastNames.Count)] + " " + Textes.lastNames[Random.Range(0, Textes.namesFemale.Count)];
			}
			
			//выбираем случайную голову
			int i = (int)Random.Range (0, GlobalVars.characters[charIndex].genders[genderIndex].heads.Length);
			//инициализируем ее, присваиваем нужные параметры позиции и масштаба
			GameObject go = Instantiate(GlobalVars.characters[charIndex].genders[genderIndex].heads[i]) as GameObject;
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3(0,0,-0.09f);
			go.transform.localScale = new Vector3(1,1,1);
			//получаем компонент анимированного спрайта головы и присваиваем его переменной
			head = go.transform.GetComponent<tk2dSpriteAnimator>();
			
			//выбираем случайное тело
			i = (int)Random.Range (0, GlobalVars.characters[charIndex].genders[genderIndex].bodys.Length);
			//инициализируем его, присваиваем нужные параметры позиции и масштаба
			go = Instantiate(GlobalVars.characters[charIndex].genders[genderIndex].bodys[i]) as GameObject;
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3(0,0,-0.05f);
			go.transform.localScale = new Vector3(1,1,1);
			//получаем компонент анимированного спрайта тела и присваиваем его переменной
			body = go.transform.GetComponent<tk2dSpriteAnimator>();
			
			//выбираем случайные ноги
			i = (int)Random.Range (0, GlobalVars.characters[charIndex].genders[genderIndex].legs.Length);
			//инициализируем их, присваиваем нужные параметры позиции и масштаба
			go = Instantiate(GlobalVars.characters[charIndex].genders[genderIndex].legs[i]) as GameObject;
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3(0,0,0);
			go.transform.localScale = new Vector3(1,1,1);
			//получаем компонент анимированного спрайта ног и присваиваем его переменной
			legs = go.transform.GetComponent<tk2dSpriteAnimator>();
			
		}
			//ChangeMoveDir(frame);
			animCanPlay = true;
		
		if(isPersonWithTrailer())
		{
			
		}
		
		head.GetClipByName("idle").fps = 13;
		legs.GetClipByName("idle").fps = 13;
		body.GetClipByName("idle").fps = 13;
		head.GetClipByName("walk").fps = 13;
		legs.GetClipByName("walk").fps = 13;
		body.GetClipByName("walk").fps = 13;
		
		if(!selfInst)
		{
			gameObject.SetActive(false);
		}
		else if(type != CharacterType.worker)
		{
			FilmStaff f = GetComponent<FilmStaff>();
			f.icon.GetComponent<CharInfo>().SetParams(f);
		}
	}
	
	void OnEnable()
	{
		//Repath();
		StartCoroutine(MoveInsideOffice());
		//StartCoroutine(MoveOnStreet());
	}
	
	public Gender gender()
	{
		return (Gender)genderIndex;
	}
	
	//предоставляем доступ к переменной busy
	public bool IsBusy()
	{
		return busy;
	}
	
	public int GetFloor()
	{
		return floor;
	}
	
	public void AddDoor(Doors door)
	{
		doors.Add(door);
	}

	public List<Doors> wayPoints;
	
	/*public void Repath () 
	{
		Vector3 v3 = Vector3.zero;
		v3.x = Random.Range(-400, 400);
		v3.y = Random.Range(-400, -100);
		path = new ABPath(transform.position, v3, null);
		seeker.StartPath(path, OnPathComplete);
	}
	
	public void OnPathComplete (Path p) 
	{
		if(p.error)
		{
			Repath();
		}
		canMove = true;
	}
	
	bool canMove = false;
	
	IEnumerator MoveOnStreet()
	{
		while(true)
		{
			if(state != PersMovingStates.none)
			{
				yield return 0;
			}
			if(!canMove)
			{
				yield return 0;
			}
			
	        if (path == null) {
	            //We have no path to move after yet
				Debug.Log ("path is null");
	            yield return 0;
	        }
	        
	        if ((currentSeekerWaypoint) >= path.vectorPath.Count) {
				//canMove = false;
				currentSeekerWaypoint = 0;
				Repath();
	            Debug.Log ("End Of Path Reached");
	            yield return 0;
	        }
	        
			transform.position = Vector3.MoveTowards(transform.position, path.vectorPath[currentSeekerWaypoint], speed * Time.fixedDeltaTime);
	        
	        //Check if we are close enough to the next waypoint
	        //If we are, proceed to follow the next waypoint
	        if (Vector3.Distance (transform.position, path.vectorPath[currentSeekerWaypoint]) < GlobalVars.SEEKER_DISTANCE) {
	            currentSeekerWaypoint++;
	            yield return 0;
	        }
			yield return 0;
		}
	}*/
	
	//в эту функцию помещается трансформ цели, куда пойдет персонаж
	public void MoveToBuilding(Transform building, int fl, bool setFree = true)
	{
		
		Doors[] go;
		if(building.parent != null)
		{
			go = building.parent.GetComponentsInChildren<Doors>();
		}
		else
		{
			go = building.GetComponentsInChildren<Doors>();
		}
		List<Doors> waypoints = new List<Doors>();
		
		wayPoints.Clear();
		
		foreach(Doors door in go)
		{
			if(door.door == DoorsLocation.ground)
			{
				waypoints.Add(door);
				//if(door.door != null)
				//{
				//	break;
				//}
			}
		}
		
		foreach(Doors door in go)
		{
			if(door.door == DoorsLocation.exitDoor)
			{
				waypoints.Add(door);
				//if(door.door != null)
				//{
				//	break;
				//}
			}
		}
		//if(type != CharacterType.worker)
		//{
		if(go.Length > 2)
		{
			if(fl != 0)
			{
				foreach(Doors door in go)
				{
					if(door.door == DoorsLocation.floor1)
					{
						waypoints.Add(door);
					}
				}
				foreach(Doors door in go)
				{
					if(door.door == (DoorsLocation)(fl + 1))
					{
						waypoints.Add(door);
					}
				}
			}
		}
		//}
		if(state == PersMovingStates.moveInsideBuilding)
		{
			waypoints.Reverse();	
		}
		else
		{
			if(isPersonWithTrailer())
			{
				movement = GetComponent<FilmStaff>().trailer.door.transform.position;
				movement.z = -80;
				transform.position = movement;
			}
		}
		
		busy = true;
		
		floor = fl;
		
		target = building;
		SetBoundingBox();
		movement = Vector3.zero;
		busy = true;
		movement.x = waypoints[0].transform.position.x;
		movement.z = transform.position.z;
		
		wayPoints = waypoints;
		
		//условия для правильного разворота персонажа
		CheckDirection(movement, thisTr.position);
		SwitchAnim("walk");
		if(state == PersMovingStates.moveInsideBuilding)
		{
			movement.y = waypoints[0].transform.position.y - 15;
			StartCoroutine(MoveToStage(building, waypoints, true, 0, setFree));
		}
		else
		{
			movement.y = waypoints[0].transform.position.y - 15;
			StartCoroutine(MoveToStage(building, waypoints, false, 0));
		}
		Messenger.Broadcast("Check workers count");
		SwitchAnim("walk");
		/*}
		else if(isPersonWithTrailer() && state != PersMovingStates.moveInsideBuilding)
		{
			ToTrailer(building, wayPoints, false, 0);
		}
		else if(isPersonWithTrailer() && state == PersMovingStates.moveInsideBuilding)
		{
			if(state == PersMovingStates.moveInsideBuilding)
			{
				movement.y = wayPoints[0].transform.position.y - 15;
				StartCoroutine(MoveToStage(building, wayPoints, true, 0));
			}
			else
			{
				movement.y = wayPoints[0].transform.position.y - 15;
				StartCoroutine(MoveToStage(building, wayPoints, false, 0));
			}
		}*/
	}
	
	bool isPersonWithTrailer()
	{
		if((tag == "director" || tag == "actor") && GetComponent<FilmStaff>().trailer != null)
		{
			return true;
		}
		else
		{
			return false; 
		}
	}
	
	//присвоение пустого значения target
	public void SetTargetNull()
	{
		target = null;
	}
	
	void Update()
	{
		//иначе если пришло время для смены направления движения - обнуляем время и меняем направление движения
		if(state == PersMovingStates.none)
		{
			if(time >= tempTime)
			{
				ChangeMoveDir(frame);
				tempTime = Random.Range(2,7);
				time = 0;
			}
			else
			{
				if(Vector3.Distance(new Vector3(thisTr.position.x, thisTr.position.y, 0), new Vector3(movement.x, movement.y,0)) <= 10)
				{
					if(time > 4 && type == CharacterType.worker)// || type == CharacterType.actor)
					{
						SwitchAnim("idleSpecial");
					}
					else
					SwitchAnim("idle");//oldAnimType = CharAnimationType.idle;				
				}
				else
				{
					thisTr.position = Vector3.MoveTowards(thisTr.position, movement, Time.deltaTime * speed);
				}
				time += Time.deltaTime;
			}
		}
		
		if(state == PersMovingStates.none)
		{
			thisTr.position = new Vector3(thisTr.position.x, thisTr.position.y, -80 - (frame.yMax - thisTr.position.y));
		}
	}
	
	public void ToConstruct(GameObject go)
	{
		target = go.transform;
		movement = go.transform.position;
		movement.z = transform.position.z;
		floor =	Random.Range(0, (int)go.GetComponent<Construct>().stage);
		SetBoundingBox();
		movement.y += 30;
		state = PersMovingStates.moveToBuilding;
		busy = true;
		CheckDirection(movement, thisTr.position);
		StartCoroutine(MoveToConstruct(go));
		
	}
	
	IEnumerator MoveToConstruct(GameObject go)
	{
		while(true)
		{
			thisTr.position = new Vector3(thisTr.position.x, thisTr.position.y, -80 - (frame.yMax - thisTr.position.y));
			thisTr.position = Vector3.MoveTowards(thisTr.position, movement, Time.deltaTime * speed);
			if(Vector3.Distance(new Vector3(thisTr.position.x, thisTr.position.y, 0), new Vector3(movement.x, movement.y,0)) < 6)
			{
				movement.y = GlobalVars.underConstrFloorHeght[floor].yMax;
				state = PersMovingStates.moveInsideBuilding;
				Messenger<GameObject>.Broadcast("Staff on construct", go);
				thisTr.position = movement;
				yield break;
			}
			yield return 0;
		}
	}
	
	public void ToTrailer(bool setFree = true)
	{
		TrailerController tc = GetComponent<FilmStaff>().trailer;
		movement = tc.door.transform.position;
		movement.z = -GlobalVars.CHARACTER_FREE_LAYER;
		CheckDirection(movement, thisTr.position);
		StartCoroutine(MoveToTrailer(tc, setFree));
	}
	
	void CheckDirection(Vector3 v1, Vector3 v2)
	{
		if(v1.x > v2.x)
		{
			head.transform.localEulerAngles = new Vector3(0,180,0);
		    legs.transform.localEulerAngles = new Vector3(0,180,0);
			body.transform.localEulerAngles = new Vector3(0,180,0);
		}
		else if(movement.x < thisTr.position.x)
		{
			head.transform.localEulerAngles = new Vector3(0,0,0);
		    legs.transform.localEulerAngles = new Vector3(0,0,0);
			body.transform.localEulerAngles = new Vector3(0,0,0);
		}
	}
	
	IEnumerator MoveToTrailer(TrailerController tc, bool setFree = true)
	{
		while(true)
		{
			if(	movement.x != tc.door.transform.position.x &&
				movement.y != tc.door.transform.position.y)
			{
				yield break;
			}
			thisTr.position = new Vector3(thisTr.position.x, thisTr.position.y, -80 - (frame.yMax - thisTr.position.y));
			thisTr.position = Vector3.MoveTowards(thisTr.position, movement, Time.deltaTime * speed);
			if(Vector3.Distance(new Vector3(thisTr.position.x, thisTr.position.y, 0), new Vector3(movement.x, movement.y,0)) < 6)
			{
				tc.door.openedDoor.enabled = true;
				yield return new WaitForSeconds(0.3f);
				tc.door.openedDoor.enabled = false;
				//movement.y = tc.stagesHeight[0] + tc.transform.position.y;
				movement.z = -2000;
				thisTr.position = movement;
				busy = false;
				if(GetComponent<FilmStaff>() != null && setFree)
				{
					GetComponent<FilmStaff>().canBeUsed = true;
				}
				target = null;
				yield break;
			}
			yield return 0;
		}
	}
	
	IEnumerator MoveToStage(Transform building, List<Doors> waypoints, bool isMovingOut, int index = 0, bool setFree = true)
	{
		while(true)
		{		
			
			if(!isMovingOut)
			{	
				thisTr.position = new Vector3(thisTr.position.x, thisTr.position.y, -80 - (frame.yMax - thisTr.position.y));
				if(Vector3.Distance(new Vector3(thisTr.position.x, thisTr.position.y, 0), new Vector3(movement.x, movement.y,0)) > 5)
				{
					state = PersMovingStates.moveToBuilding;
					thisTr.position = Vector3.MoveTowards(thisTr.position, movement, Time.deltaTime * speed);
				}
				else if(Vector3.Distance(new Vector3(thisTr.position.x, thisTr.position.y, 0), new Vector3(movement.x, movement.y,0)) < 6)
				{
					waypoints[index].openedDoor.enabled = true;
					index++;
					yield return new WaitForSeconds(0.2f);
					waypoints[index - 1].openedDoor.enabled = false;
					if(index + 1 > waypoints.Count)
					{
						if(tag == "actor")
						{
							Messenger<PersonController>.Broadcast("Actor is come", this);
						}
						else
						{
							Messenger<PersonController>.Broadcast("Staff on stage", this);
						}
						state = PersMovingStates.moveInsideBuilding;
						index = 0;
						
						yield break;		
					}
					
					movement.x = waypoints[index].transform.position.x;
					movement.y = waypoints[index].transform.position.y - 15;
					movement.z = GlobalVars.CHARACTER_IN_BUILDING_LAYER;
					if(waypoints[index].door > DoorsLocation.floor1 && waypoints[index].door <= DoorsLocation.floor5)
					{
						thisTr.position = movement;
					}
					CheckDirection(movement, thisTr.position);
				}
			}
			else
			{
				if(Vector3.Distance(new Vector3(thisTr.position.x, thisTr.position.y, 0), new Vector3(movement.x, movement.y,0)) > 5)
				{
					state = PersMovingStates.moveToBuilding;
					thisTr.position = Vector3.MoveTowards(thisTr.position, movement, Time.deltaTime * speed);
				}
				else if(Vector3.Distance(new Vector3(thisTr.position.x, thisTr.position.y, 0), new Vector3(movement.x, movement.y,0)) < 6)
				{
					if(waypoints[index] == null)
					{
						index++;
					}
					waypoints[index].openedDoor.enabled = true;
					yield return new WaitForSeconds(0.2f);
					waypoints[index].openedDoor.enabled = false;
					index++;
					if(index + 1 > waypoints.Count)
					{
						
						if(isPersonWithTrailer())
						{
							ToTrailer(setFree);
						}
						else
						{
							if(this.GetComponent<FilmStaff>() != null && setFree)
							{
								this.GetComponent<FilmStaff>().canBeUsed = true;
							}
							movement.y = -97;
							movement.z = -GlobalVars.CHARACTER_FREE_LAYER;
							thisTr.position = movement;
							state = PersMovingStates.none;
							busy = false;
							target = null;
						}
						index = 0;
						Messenger.Broadcast("Check workers count");
						yield break;
					}
					
					
					
					movement.x = waypoints[index].transform.position.x;
					movement.y = waypoints[index].transform.position.y - 15;
					movement.z = GlobalVars.CHARACTER_IN_BUILDING_LAYER;
					
					if(waypoints[index].door == DoorsLocation.floor1)
					{
						thisTr.position = movement;
					}
					
					CheckDirection(movement, thisTr.position);
					
					
				}
			}
			yield return 0;
		}
	}
	
	void SetBoundingBox()
	{
		BuildingType b = BuildingType.buildersHut;
		switch(type)
		{
		case CharacterType.actor:
			b = BuildingType.hangar;
			break;
		case CharacterType.cameraman:
			b = BuildingType.hangar;
			break;
		case CharacterType.postProdWorker:
			b = BuildingType.postproduction;
			break;
		case CharacterType.director:
			b = BuildingType.hangar;
			break;
		case CharacterType.producer:
			b = BuildingType.office;
			break;
		case CharacterType.scripter:
			b = BuildingType.scriptWrittersOffice;
			break;
		}
		foreach(StageBoundingBox s in GlobalVars.stageBoundingBox)
		{
			if(s.buildingType == b && (int)s.stage == floor)
			{
				bb = s.box[floor];
			}
		}
		if(type == CharacterType.worker)
		{
			bb = GlobalVars.underConstrFloorHeght[floor];
		}
	}
	
	IEnumerator MoveInsideOffice()
	{
		while(true)
		{
			/*if(state == PersMovingStates.moveInsideBuilding)
			{
				//если время больше нуля, то обнуляем время и меняем направление движения персонажа
				if(time >= tempTime)
				{
					BoundingBox b = new BoundingBox();
					//b.xMax = //target.transform.position.x + bb.xMax;;
					//b.xMin = //target.transform.position.x + bb.xMin;;
					b.yMax = bb.yMax;
					b.yMin = bb.yMin;
					ChangeMoveDir(b);
					tempTime = Random.Range(2,7);
					time = 0;
				}
				//иначе двигаем персонажа если он еще не достиг пункта назначения, иначе меняем ему анимацию
				else
				{
					if(Vector3.Distance(movement,thisTr.position) < 2)
					{
						SwitchAnim("idle");
					}
					else
					{
						thisTr.position = Vector3.MoveTowards(thisTr.position, movement, Time.deltaTime * speed);
					}
					time += Time.deltaTime;
				}
			}*/
			yield return 0;
		}
	}
	
	//движение персонажа к двери
	public void MoveToDoor(Transform door)
	{
		if(target != null)
		{
			target.GetComponent<Doors>().openedDoor.enabled = false;
		}
		target = door;
		movement = Vector3.zero;
		busy = true;
		movement.x = door.position.x;
		movement.z = thisTr.position.z;
		movement.y = door.position.y- 10;
		SwitchAnim("walk");
		CheckDirection(movement, thisTr.position);
	}
	
	//присвоение идл анимации
	void ToIdle()
	{
		movement = Vector3.zero;
		SwitchAnim("idle");
	}
	
	//изменение направления движения
	void ChangeMoveDir(BoundingBox b)
	{
		movement = Vector3.zero;
		if(!busy)
		{
			movement.y = Random.Range(b.yMin , b.yMax);
		}
		else
		{
			movement.y = b.yMax;
		}
		movement.x = Random.Range(b.xMin , b.xMax);
		if(state == PersMovingStates.moveInsideBuilding)
		{
			movement.z = GlobalVars.CHARACTER_IN_BUILDING_LAYER;
		}
		else
		{		
			movement.z = -GlobalVars.CHARACTER_FREE_LAYER;
		}
		//выбор правильного разворота персонажа
		CheckDirection(movement, thisTr.position);
		SwitchAnim("walk");
	}
	
	//изменение анимации, на вход идет название анимации
	void SwitchAnim(string str)
	{
		//если эта анимация уже не проигрывается, то останавливаем все анимации и запускаем нужную нам
		if(animCanPlay)
		{
			if(animType.ToString() != str)
			{
				head.Stop();
				if(!isPremium)
				{
					body.Stop();
					legs.Stop();
					//legs.CurrentClip.fps = GlobalVars.STAFF_ANIMATIONS_SPEED;
					//body.CurrentClip.fps = GlobalVars.STAFF_ANIMATIONS_SPEED;
					body.Play(str);
					legs.Play(str);
				}
				
				//head.CurrentClip.fps = GlobalVars.STAFF_ANIMATIONS_SPEED;
				head.Play(str);
			
				if(str == "walk")
				{
					animType = CharAnimationType.walk;
				}
				else if(str == "idle")
				{
					animType = CharAnimationType.idle;
				}
				if(str == "idleSpecial")
				{
					animType = CharAnimationType.idleSpecial;
				}
			}
		}
	}
	
	public void MoveToBuilding(Waypoint[] points, Transform leftTr, Transform rightTr)
	{	
		leftPoint = leftTr;
		rightPoint = rightTr;
		//LookForClosestWaypoint();
		//waypoints = StaffManagment.CalculateWayToBuilding(currWaypoint, points[0], points);
		waypoints = points;
		movement = Vector3.zero;
		movement.x = waypoints[0].pointPos.position.x;
		movement.y = waypoints[0].pointPos.position.y;
		movement.z = transform.position.z;// + Random.Range(-5f, 5f);
		CheckDirection(movement, thisTr.position);
		state = PersMovingStates.moveToBuilding;
		SwitchAnim("walk");
		StartCoroutine(Move(state));
		if(this.GetComponent<FilmStaff>() != null)
		{
			this.GetComponent<FilmStaff>().canBeUsed = false;
		}
	}
	
	public void MoveToBuilding(Waypoint[] points)
	{	
		state = PersMovingStates.moveToBuilding;
		waypoints = points;
		movement = Vector3.zero;
		movement.x = waypoints[0].pointPos.position.x;
		movement.y = waypoints[0].pointPos.position.y;
		movement.z = transform.position.z; //+ Random.Range(-5f, 5f);
		CheckDirection(movement, thisTr.position);
		SwitchAnim("walk");
		StartCoroutine(Move(state));
		if(this.GetComponent<FilmStaff>() != null)
		{
			this.GetComponent<FilmStaff>().canBeUsed = false;
		}
	}
	
	public void MoveOutOfBuilding(Waypoint[] points)
	{
		waypoints = points;
		movement = Vector3.zero;
		movement.x = waypoints[0].pointPos.position.x;
		movement.y = waypoints[0].pointPos.position.y;
		movement.z = transform.position.z; //+ Random.Range(-5f, 5f);
		CheckDirection(movement, thisTr.position);
		state = PersMovingStates.moveOutOfBuilding;
		SwitchAnim("walk");
		StartCoroutine(Move(state));
	}
	
	//waypoint moving
	/*public void StartMoving()
	{
		LookForClosestWaypoint();
		waypoints = StaffManagment.CalculateWayOnGround(currWaypoint, out endWaypoint);
		movement = Vector3.zero;
		movement.x = waypoints[0].pointPos.position.x;
		movement.y = waypoints[0].pointPos.position.y;
		movement.z = waypoints[0].pointPos.position.z + Random.Range(-5f, 5f);
		CheckDirection(movement, thisTr.position);
		SwitchAnim("walk");
		state = PersMovingStates.none;
		StartCoroutine (Move (state));
	}*/
	
	void LookForClosestWaypoint()
	{
		foreach(GroundWaypoint g in GlobalVars.allGroundWaypoints)
		{
			if(currWaypoint == null)
			{
				currWaypoint = g;
			}
			else if(Vector3.Distance(g.transform.position, transform.position) < Vector3.Distance(currWaypoint.transform.position, transform.position))
			{
				currWaypoint = g;
			}
		}
	}
	
	IEnumerator Move(PersMovingStates s, int index = 0)
	{
		while(true)
		{	
			if(s != state)
			{
				yield break;
			}
			Vector3 v3 = thisTr.position;
			if(Vector3.Distance(v3, movement) > 5)
			{
				thisTr.position = Vector3.MoveTowards(thisTr.position, movement, Time.deltaTime * speed);
			}
			else if(Vector3.Distance(v3, movement) <= 6)
			{
				index++;
				if(index + 1 > waypoints.Length)
				{
					CharacterFinished();
					yield break;		
				}
				movement = thisTr.position;
				movement.z = waypoints[index].pointPos.position.z;
				thisTr.position = movement;	
				movement = waypoints[index].pointPos.position;
				movement.z += Random.Range(-5f, 0);
				if(waypoints[index].isTeleportPoint)
				{
					thisTr.position = movement;
				}
				
				CheckDirection(movement, thisTr.position);
			}
			yield return 0;
		}
	}
	
	void CharacterFinished()
	{
		switch(state)
		{
		case PersMovingStates.moveOutOfBuilding:
			if(isPersonWithTrailer())
			{
				ToTrailer();
			}
			else
			{
				if(this.GetComponent<FilmStaff>() != null)
				{
					if(!this.GetComponent<FilmStaff>().busy)
					{
						this.GetComponent<FilmStaff>().canBeUsed = true;
					}
				}
				movement.y = -97;
				movement.z = GlobalVars.CHARACTER_FREE_LAYER;
				thisTr.position = movement;
				state = PersMovingStates.none;
				//StartMoving();
				busy = false;
				target = null;
			}
			Messenger.Broadcast("Check workers count");
			break;
		case PersMovingStates.moveInsideBuilding:
			
			break;
		case PersMovingStates.moveToBuilding:
			if(tag == "actor")
			{
				Messenger<PersonController>.Broadcast("Actor is come", this);
			}
			else
			{
				Messenger<PersonController>.Broadcast("Staff on stage", this);
			}
			if(leftPoint != null && rightPoint != null)
			{
				state = PersMovingStates.moveInsideBuilding;
			}
			SwitchAnim("idle");
			break;
		case PersMovingStates.none:
			//StartMoving();
			break;
		}
	}
	
	IEnumerator MoveInsideBuilding(float moveTime = 0)
	{
		while(true)
		{
			if(state == PersMovingStates.moveInsideBuilding)
			{
				//если время больше нуля, то обнуляем время и меняем направление движения персонажа
				if(time >= moveTime)
				{
					if(target == null)
					{
						movement.x = Random.Range(leftPoint.position.x, rightPoint.position.x);
					}
					else if(target != null)
					{
						movement.x = Random.Range(target.transform.position.x - 300,target.transform.position.x + 300);
					}
					CheckDirection(movement,thisTr.position);
					SwitchAnim("walk");
					moveTime = Random.Range(2,7);
					time = 0;
				}
				//иначе двигаем персонажа если он еще не достиг пункта назначения, иначе меняем ему анимацию
				else
				{
					if(Vector3.Distance(movement,thisTr.position) < 2)
					{
						SwitchAnim("idle");
					}
					else
					{
						thisTr.position = Vector3.MoveTowards(thisTr.position, movement, Time.deltaTime * speed);
					}
					time += Time.deltaTime;
				}
			}
			yield return 0;
		}
	}
}
