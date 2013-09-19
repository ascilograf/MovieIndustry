using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PopUpStageWorkState
{
	notActive,
	selectWork,
	showInfo,
}

//это скрипт общего меню для всех типов работ
//на вход принимается информация о типе здания, этаж и уровень здания
//после этого выбирает персонаж для работы
//по нажатию начать - на здании запускается корутина работы, в нее помещаются данные о времени и наградах за выполнение работы
public class popUpStageWork : MonoBehaviour {
	
	//текстмеши различных параметров, которые выводятся для информации игроку
	public tk2dTextMesh workName;
	public tk2dTextMesh workTime;
	public tk2dTextMesh workTimeButtonText;
	public tk2dTextMesh profit;						
	public tk2dTextMesh expForStudio;
	public tk2dTextMesh priceInStars;
	public tk2dTextMesh firstChance;
	public tk2dTextMesh secondChance;
	public tk2dTextMesh thirdChance;
	public tk2dTextMesh workInfo;
	
	public GameObject buttonAccept; 				//кнопка "принять"
	public GameObject buttonClose;					//кнопка закрыть меню
	public GameObject buyForStars;					//кнопка купить за звезды
	public GameObject infos;						//парент-объект для помещения в него информаций
	public GameObject menu;							//объект меню
	
	public tk2dUIScrollableArea staffScroll;
	
	public List<Scripter> scripters;				//сценаристы
	public List<FilmStaff> staff;					//персонал
	public FilmStaff currFilmStaff;					//текущий работник
	public ScriptersOfficeStage office;				//офис сценаристов
	public MarketingOfficeStage marketingOffice;	//офис маркетологов
	StageWork stageWork;							//работта на этаже
	public PopUpStageWorkState state;				//текщее состояние этого меню
								
	Works work;
	int stars;
	BuildingType type;
		
	float scale;
	
	void Awake()
	{
		GlobalVars.popUpStageWork = this;
	}
	
	void Start()
	{	
		//scale = transform.localScale.y;
		//swipe = GetComponent<SwipeItems>();
		//SwitchState(PopUpStageWorkState.notActive);
	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject go)
	{
		if(go == buttonClose)
		{
			SwitchState(PopUpStageWorkState.notActive);
		}
		else if(go == buttonAccept)
		{
			if(currFilmStaff != null && state == PopUpStageWorkState.selectWork)
			{
				stageWork.NewStaffWork(work.timeSec, work.expForStudio, work.expForWorker, work.profit, currFilmStaff);
				SwitchState(PopUpStageWorkState.notActive);
			}
		}
		else if(go == buyForStars)
		{
			if(state == PopUpStageWorkState.showInfo && GlobalVars.stars >= stars)
			{
				GlobalVars.stars -= stars;
				stageWork.time = 0;
				SwitchState(PopUpStageWorkState.notActive);
			}
		}
		else if(go.collider.name == "CharInfo" && state == PopUpStageWorkState.selectWork)
		{
			AddStaff(go.transform.GetComponent<CharInfo>().staff);
		}
	}
	
	void AddStaff(FilmStaff fs)
	{
		if(currFilmStaff != null)
		{
			currFilmStaff.mark.enabled = false;
			if(fs != currFilmStaff)
			{
				currFilmStaff = fs;
				currFilmStaff.mark.enabled = true;	
			}
			else
			{
				currFilmStaff = null;
			}
		}
		else
		{
			currFilmStaff = fs;
			currFilmStaff.mark.enabled = true;
		}
	}
	
	//обновить список сценаристов
	void PrepareStaff()
	{
		string t = "";
		infos.transform.localPosition = Vector3.zero;
		switch (type)
		{
		case BuildingType.scriptWrittersOffice:
			t = (CharacterType.scripter).ToString();
			break;
		case BuildingType.office:
			t = (CharacterType.producer).ToString();
			break;
		case BuildingType.hangar:	
			t = (CharacterType.cameraman).ToString();
			break;
		case BuildingType.postproduction:
			t = (CharacterType.postProdWorker).ToString();
			break;
		}
		
		staff.Clear();
		GameObject[] sc = GameObject.FindGameObjectsWithTag(t);
		for(int i = 0; i < sc.Length; i++)
		{
			if(sc[i].GetComponent<FilmStaff>().canBeUsed)
			{
				staff.Add(sc[i].GetComponent<FilmStaff>());
			}
		}
		for(int i = 0; i < staff.Count; i++)
		{
			CharInfo c = staff[i].GetComponent<FilmStaff>().icon.GetComponent<CharInfo>();
			c.transform.parent = infos.transform;
			c.transform.localPosition = new Vector3(90 + i * 350, 0, 0);
			c.transform.localScale = Vector3.one;
			c.gameObject.SetActive(true);
			c.Refresh();
			c.buttonHireStaff.SetActive(false);
			c.bottomPrice.SetActive(true);
		}
		if(staff.Count > 1)
		{
			staffScroll.ContentLength = (staff.Count) * 350 - 200;
		}
		else
		{
			staffScroll.ContentLength = 351;
		}
	}
	
	//установить параметры, в хависимости от типа здания выбираем работу по индексу уровня и этажа
	public void SetParams(BuildingType type1, int officeLvl, int officeStage, StageWork stage)
	{
		gameObject.SetActive(true);
		GlobalVars.cameraStates = CameraStates.menu;
		state = PopUpStageWorkState.selectWork;
		switch (type1)
		{
		case BuildingType.scriptWrittersOffice:
			work = GlobalVars.scriptersWorklist[officeLvl].works[officeStage];
			type = type1;
			break;
		case BuildingType.office:
			work = GlobalVars.producersWorklist[officeLvl].works[officeStage];
			type = type1;
			break;
		case BuildingType.hangar:	
			work = GlobalVars.hangarWorklist[officeLvl].works[officeStage];
			type = type1;
			break;
		case BuildingType.postproduction:
			work = GlobalVars.postProdWorklist[officeLvl].works[officeStage];
			type = type1;
			break;
		}
		
		stageWork = stage;
		//swipe.InfosToStart();
		
		workName.text = work.name;
		workTime.text = Utils.FormatIntToUsualTimeString(work.timeSec);
		workTimeButtonText.text = Utils.FormatIntToUsualTimeString(work.timeSec);
		profit.text = work.profit.ToString();
		expForStudio.text = work.expForStudio.ToString();
		
		firstChance.text = stageWork.commonChance + "%";
		secondChance.text = stageWork.rareChance + "%";
		thirdChance.text = stageWork.uniqueChance + "%";
		
		SwitchState(PopUpStageWorkState.selectWork);
		//workInfo.text = ;
	}
	
	//сменить остальной персонал на тот, который указан в индексе
	void SwitchStaff(int index)
	{
		if(staff.Count > 0)
		{
			currFilmStaff = staff[index];
		}
	}
	
	//показать меню как инфо о текущей работе, на вход другой персонал
	public void ShowInfo(FilmStaff sc, int time, StageWork stage)
	{
		state = PopUpStageWorkState.showInfo;
		gameObject.SetActive(true);
		
		stageWork = stage;
		GlobalVars.cameraStates = CameraStates.menu;
		currFilmStaff = sc;
		staff.Add(sc);
		stars = (int)(time * 0.1f);
		if(stars < 1)
		{
			stars = 1;
		}
		priceInStars.text = stars.ToString();	
		SwitchState(PopUpStageWorkState.showInfo);
	}
	
	void ShowCharacter()
	{
		CharInfo c = currFilmStaff.GetComponent<FilmStaff>().icon.GetComponent<CharInfo>();
		c.transform.parent = infos.transform;
		c.transform.localPosition = new Vector3(0, 0, -1);
		c.transform.localScale = Vector3.one;
		c.gameObject.SetActive(true);
		c.Refresh();
		c.buttonHireStaff.SetActive(false);
		c.bottomPrice.SetActive(true);
	}
	
	//изменить состояние меню, на вход состояние
	public void SwitchState(PopUpStageWorkState s)
	{
		//если меняем на неактивное
		//то делаем все элементы меню неактивными
		//все инфо сценаристов и персонала возвращаем родителям
		//возвращаем свайп в начальное положение и обнуляем индекс в свайпе
		if(s == PopUpStageWorkState.notActive)
		{
			
			gameObject.SetActive(false);
			if(staff.Count > 0)
			{
				for(int i = 0; i < staff.Count; i++)
				{
					staff[i].icon.transform.parent = staff[i].icon.GetComponent<CharInfo>().character.transform;
					staff[i].mark.enabled = false;
					staff[i].icon.SetActive(false);	
				}
			}
			staff = new List<FilmStaff>();
			staff.Clear();
			currFilmStaff = null;
			GlobalVars.cameraStates = CameraStates.normal;
			state = PopUpStageWorkState.notActive;
		}
		//если меняем на выбрать работу, то показываем толкьо нужные для этого элементы
		if(s == PopUpStageWorkState.selectWork)
		{
			state = PopUpStageWorkState.selectWork;
			buyForStars.SetActive(false);
			buttonAccept.SetActive(true);
			PrepareStaff();
		}
		//если меняем на показать инфо, то показываем толкьо нужные для этого элементы
		if(s == PopUpStageWorkState.showInfo)
		{
			state = PopUpStageWorkState.showInfo;
			buttonAccept.SetActive(false);
			buyForStars.SetActive(true);
			ShowCharacter();
		}
	}
}
