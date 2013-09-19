using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//работа на этаже
//при запуске из меню поэтаженых работ - запускается корутина, если во время корутины было нажатие по этажу - показываем окно информации
public class StageWork : MonoBehaviour {
	
	public float time;						//время
	public GameObject checkMark;			//галка
	public GameObject timerParent;
	public tk2dTextMesh timerLeft;			//текстмеш времени
	public tk2dTextMesh timerRight;
	public bool isStageBusy;				//занят ли этаж
	Vector3 tapUp, tapDown;					
	public int currScriptersCount;			
	public int lvl;
	public int stage;
	
	public int commonChance;
	public int rareChance;
	public int uniqueChance;
	
	int expForStudio;
	int expForWorker; 
	int reward;
	public FilmStaff worker;

	StageWaypoints stageWaypoints;
	
	void Start()
	{
		stageWaypoints = GetComponent<StageWaypoints>();
	}
	
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
		checkMark.SetActive(false);
		timerParent.SetActive(false);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject go)
	{
		if(isStageBusy)
		{
			if(go == gameObject)
			{
				if(time <= 0)
				{
					StartCoroutine(DelayedClick(Time.deltaTime));
				}
				else if(currScriptersCount != 0)
				{
					GlobalVars.popUpStageWork.ShowInfo(worker, (int)time, this);		
				}
			}
		}
	}
	
	IEnumerator DelayedClick(float time)
	{
		yield return new WaitForSeconds(time);
		GlobalVars.expGain.gainForWork(expForStudio);
		GlobalVars.money += reward + (int)(reward * worker.bonuses.MoneyGainBonus * 0.01f);
		worker.exp += expForWorker + (int)(expForWorker * worker.bonuses.ExpGainBonus * 0.01f);
		List<PersonController> list = new List<PersonController>();
		list.Add(worker.GetComponent<PersonController>());
		worker.GetComponent<PersonController>().MoveOutOfBuilding(stageWaypoints.waypointsReverseSequence);
		//worker.canBeUsed = false;
		timerLeft.text = "";
		timerLeft.Commit();
		timerRight.text = "";
		timerRight.Commit();
		worker = null;
		checkMark.SetActive(false);
		isStageBusy = false;
		currScriptersCount = 0;
		CalculateChance(commonChance, rareChance, uniqueChance);
		yield break;
	}
	
	//запуск работы, на вход время, опыт студии, опыт работнику, награда в деньгах, персонал фильма.
	public void NewStaffWork(int timeInSec, int expStudio, int expWorker, int rew, FilmStaff staff)
	{
		staff.canBeUsed = false;
		time = timeInSec;
		expForStudio = expStudio;
		expForWorker = expWorker;
		reward = rew;
		worker = staff;
		//worker.GetComponent<PersonController>().MoveToBuilding(transform, stage);
		worker.GetComponent<PersonController>().MoveToBuilding(stageWaypoints.waypointsDirectSequence, stageWaypoints.leftPoint, stageWaypoints.rightPoint);
		StartCoroutine(StartStaffNewWork());
		isStageBusy = true;
	}
	
	IEnumerator StartStaffNewWork()
	{
		while(true)
		{
			if(time <= 0)
			{
				checkMark.SetActive(true);
				timerParent.SetActive(false);
				yield break;
			}

			if(currScriptersCount == 1 && time > 0)
			{
				if(!timerParent.activeSelf)
				{
					timerParent.SetActive(true);
				}
				isStageBusy = true;
				Utils.FormatIntTo2PartsTimeString(timerLeft, timerRight, (int)time);
				time -= Time.deltaTime;
			}
			yield return 0;
		}
	}
	
	
	//вычисление шанса выпадения предмета, расчет ведется от низкого грейда к высокому
	void CalculateChance(int commonPercent, int rarePercent, int uniquePrecent) 
	{
		int rand = Random.Range(0, 100);
		if(rand < commonPercent)
		{
			InstantCraftPart(RarityLevel.common);
			return;
		}
		rand = Random.Range(0, 100);
		if(rand < rarePercent)
		{
			InstantCraftPart(RarityLevel.rare);
			return;
		}
		rand = Random.Range(0, 100);
		if(rand < uniquePrecent)
		{
			InstantCraftPart(RarityLevel.unique);
			return;
		}
	}
	
	//инициализация части крафта по грейду
	void InstantCraftPart(RarityLevel rarity)
	{	
		//GameObject go = null;
		foreach(CraftPartPrefabs c in GlobalVars.craftPartsRefabs)
		{
			if(c.rarity == rarity)
			{
				int rand = Random.Range(0, c.prefabs.Length);
				Instantiate(c.prefabs[rand]);// as GameObject;
			}
		}
	}
}
