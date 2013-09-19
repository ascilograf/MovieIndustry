using UnityEngine;
using System.Collections;

//контроллер превью строительства
//на вход идет тип, время постройки, кол-во рабочих, цена
//каждый кадр двигает превью и определяет столкновения с др. зданиями, если их нет - делает превью зеленым, если есть - красным
//если все ок - инстансит новое настоящее здание на сцену в то же место

public class PlaceBuilding : MonoBehaviour 
{
	BuildingType constrType;
	int timeToConstruct;
	int workersCount;
	int priceMoney;
	GameObject instPrefab;
	bool canPlaceHere = true;
	
	public GameObject buttonPlace;
	public GameObject buttonCancelPlace;
	
	void Start () 
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject go)
	{
		if(go == buttonPlace)
		{
			print ("!!");
			if(canPlaceHere)
			{
				ColorizePrefab(Color.white);
				BuildOffice(constrType, timeToConstruct, workersCount, priceMoney);
			}
		}
		else if(go == buttonCancelPlace)
		{
			print ("");
			GlobalVars.money += priceMoney;
			Destroy (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		ChangePlace();
	}
	
	void ChangePlace()
	{
		Vector3 v3 = transform.position;
		v3.x = Camera.main.transform.position.x;
		v3.y = -150;
		v3.z = GlobalVars.BUILDING_LAYER;
		transform.position = v3;
		if(GlobalVars.cameraStates == CameraStates.menu)
		{
			Destroy(gameObject);
		}
	}
	
	public void SetParams(BuildingType type, int time, int workers, int price, bool isReplace = false)
	{
		constrType = type;
		timeToConstruct = time;
		workersCount = workers;
		priceMoney = price;
		if(!isReplace)
		{
			foreach(BuildingPrefabs bp in GlobalVars.buildingsPrefabs)
			{
				if(bp.type == constrType)
				{
					GameObject go = Instantiate(bp.previewPrefab) as GameObject;
					go.transform.parent = transform;
					go.transform.localPosition = Vector3.zero;
					instPrefab = go;
					ColorizePrefab(Color.green);
				}
			}
		}
	}
	
	void BuildOffice(BuildingType type, int time, int workers, int price)
	{
		for(int i = 0; i < GlobalVars.buildingsPrefabs.Count; i++)
		{
			if(type == GlobalVars.buildingsPrefabs[i].type)
			{
				GameObject go = Instantiate(GlobalVars.buildingsPrefabs[i].parentObjectPrefab,
											new Vector3(Camera.main.transform.position.x, -150 , GlobalVars.BUILDING_LAYER), 
											Quaternion.identity) as GameObject;
				go.GetComponent<Construct>().stage = Stages.first;
				go.GetComponent<Construct>().StartBuildNewOffice(time, workers, price);
				//строку ниже перенести в сам конструктор, выполнять при условии постройки здания
				GlobalVars.expGain.gainForBuild(timeToConstruct);
				Destroy(this.gameObject);
			}
		}
	}
					
	void ColorizePrefab(Color color)
	{
		instPrefab.GetComponentInChildren<tk2dSprite>().color = color;
	}
	
	//Если коллайдер входит в другой коллайдер с тэгом здание, то построить нельзя
	void OnTriggerEnter(Collider coll)
	{
		if(coll.CompareTag("building"))
		{
			ColorizePrefab(Color.red);
			canPlaceHere = false;
		}
	}
	
	//Если коллайдер уже находится в другом коллайдере с тэгом здание, то построить нельзя	
	void OnTriggerStay(Collider coll)
	{
		if(coll.CompareTag("building"))
		{
			canPlaceHere = false;
			ColorizePrefab(Color.red);
		}
	}
	
	//Если коллайдер выходит из другого коллайдера с тэгом здание, то построить можно
	void OnTriggerExit(Collider coll)
	{
		if(coll.CompareTag("building"))
		{
			canPlaceHere = true;
			ColorizePrefab(Color.green);
		}
	}
}
