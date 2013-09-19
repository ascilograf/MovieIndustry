using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum HireForFilmState
{
	mainWindow,
	selectStaff,
	selectScript,
	notActive,
}

//скрипт управляет меню найма персонажей на фильм и отправляет параметры в ангар, если всё прошло успешно
//вызывается из инвентаря, по кнопке Use на сценарии
public class HireForFilm : MonoBehaviour 
{
	public Scenario script;										//сценарий
	public GameObject buttonAccept;								//кнопка подтверждения
	public GameObject buttonClose;								//кнопка закрытия меню
	public GameObject buttonBack;
	public GameObject buttonPickScript;
	public GameObject topBlock;									//верхний блок
	public GameObject bgPart;
	public Camera cam;
	
	public GameObject staffPlanes;								//нижний блок
	public GameObject infos;
	public GameObject scenarioInfos;
	
	public tk2dUIScrollableArea staffScrollArea;
	public tk2dUIScrollableArea scriptsScrollArea;
	
	public HireForFilmPlane planeDirector;
	public HireForFilmPlane planeActors;
	public HireForFilmPlane planeCameramans;
	
	public GetTextFromFile textHeader;
	public GetTextFromFile textButton;
	public tk2dTextMesh textScenarioTitle;
	public tk2dTextMesh textScenarioGenres;
	public tk2dTextMesh priceMesh;								//надпись общей цены
	public float price;											//переменная цены найма персонажей + актеров
	public tk2dTextMesh helpMessage;							//надпись сообщения-подсказки

	public List<FilmStaff> directors;							//выбранные режиссеры
	public List<FilmStaff> actors;								//выбранные актеры
	public List<FilmStaff> operators;							//операторы
	GameObject tempGo;											//временный объект
	FilmStaff currFilmStaff;									//текущий выбранные персонаж
	List<FilmStaff> currStaff = new List<FilmStaff>();	//список персонала для фильма
	
	int maxStaff;
	CharacterType staffType;
	List<FilmStaff> list = new List<FilmStaff>();
	List<Scenario> scen = new List<Scenario>();
	HireForFilmState state;
	FilmMaking hangar;
	
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
		GetPlanesSprites();
		staffPlanes.transform.localPosition = new Vector3(0, -80, -1.5f);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject go)
	{
		if(go == planeActors.plane && script != null && staffPlanes.transform.localPosition == Vector3.zero)
		{ 
			RefreshListOf(CharacterType.actor);
		}
		else if(go == planeDirector.plane && script != null && staffPlanes.transform.localPosition == Vector3.zero)
		{
			RefreshListOf(CharacterType.director);
		}
		else if(go == planeCameramans.plane && script != null && staffPlanes.transform.localPosition == Vector3.zero)
		{
			RefreshListOf(CharacterType.cameraman);
		}
		else if(go.name == "CharInfo")
		{
			AddNewStaff(go.transform.GetComponent<CharInfo>().staff);
		}
		else if(go == buttonAccept && staffPlanes.transform.localPosition == Vector3.zero)
		{
			if(state == HireForFilmState.mainWindow)
			{
				if(script != null && directors.Count > 0 && operators.Count > 0 && actors.Count > 0)
				{
					PrepareForNewFilm();
				}
			}
			else if(state == HireForFilmState.selectStaff)
			{
				SwitchState(HireForFilmState.mainWindow);
			}
		}
		else if(go == buttonPickScript)
		{
			SwitchState(HireForFilmState.selectScript);
		}
		else if(go.name == "ScenarioInfo")
		{
			AddScript(go.GetComponent<ParentInfo>().parent.GetComponent<Scenario>());
			StartCoroutine(MoveStaffPlanesUp(false));
		}
		else if(go == buttonBack)
		{
			
			currStaff.Clear();
			CharInfoToParent();
			ScenInfosToParent();
			SwitchState(HireForFilmState.mainWindow);
		}
		else if(go == buttonClose)
		{

			hangar.busy = false;
			hangar = null;
			CharInfoToParent();
			SwitchState(HireForFilmState.notActive);
		}
	}
	
	//подвтердить запись на меше
	public void CommitText(tk2dTextMesh mesh, string s)
	{
		mesh.text = s;
		mesh.Commit();
	}
	
	//показать/спрятать объект
	public void ShowHideObject(GameObject go, bool b)
	{
		Transform[] tr = go.GetComponentsInChildren<Transform>(true);
		foreach(Transform t in tr)
		{
			t.gameObject.SetActive(b);
		}
	}
	
	void RefreshStaffPlanes()
	{
		SetStaff(planeActors, actors);
		SetStaff(planeDirector, directors);
		SetStaff(planeCameramans, operators);
	}
	
	void HideMainWindow()
	{
		staffPlanes.SetActive(false);
		topBlock.SetActive(false);
	}
	
	public void ShowMenu(FilmMaking fm, Scenario s = null)
	{
		script = s;
		hangar = fm;
		if(script != null)
		{
			script.activeIcon.enabled = true;
		}
		GlobalVars.cameraStates = CameraStates.menu;
		SwitchState(HireForFilmState.selectScript);		
		GetPlanesSprites();
		//CheckAcceptButton();
	}
	
	void SetStaff(HireForFilmPlane plane, List<FilmStaff> fsList)
	{
		plane.plane.SetActive(true);
		
		foreach(GameObject g in plane.states)
		{
			g.SetActive(false);
		}
		
		plane.states[fsList.Count].SetActive(true);
		
		for(int i = 0; i < plane.names.Length; i++)
		{
			if(fsList.Count > i)
			{
				plane.names[i].text = fsList[i].name;
				plane.prices[i].text = fsList[i].icon.GetComponent<CharInfo>().GetPrice(script.genres).ToString();
			}
			else
			{
				plane.names[i].text = "";
				plane.prices[i].text = "";
			}
			plane.prices[i].Commit();
			plane.names[i].Commit();
		}
	}
	
	//обновить список (на вход принимается тип персонажа), строим список не занятых персонажей, 
	//выбираем первого в списке, фокусируемся на нем, остальные будут правее
	int index;
	void RefreshListOf(CharacterType type)
	{
		staffType = type;
		currStaff.Clear();
		SwitchState(HireForFilmState.selectStaff);
		switch(type)
		{
		case CharacterType.director:
			directors.Clear();
			index = 3;
			textHeader.SetTextWithIndex(index);
			maxStaff = 1;
			SetPrice();
			break;
		case CharacterType.actor:
			actors.Clear();
			index = 2;
			maxStaff = 3;
			textHeader.SetTextWithIndex(index);
			SetPrice();
			break;
		case CharacterType.cameraman:
			operators.Clear();
			index = 4;
			maxStaff = 1;
			textHeader.SetTextWithIndex(index);
			SetPrice();
			break;
		}
		
		bool flag = false;
		
		
		string tag = type.ToString();
		GameObject[] staff = GameObject.FindGameObjectsWithTag(tag);
		
		foreach(GameObject st in staff)
		{
			FilmStaff fs = st.GetComponent<FilmStaff>();
			for(int j = 0; j < script.genres.Count; j++)
			{
				for(int k = 0; k < fs.mainSkill.Count; k++)
				{
					if(fs.mainSkill[k].genre == script.genres[j] && fs.canBeUsed)
					{
						if(fs.mainSkill[k].skill > 0)
						{
							if(!list.Exists(delegate(FilmStaff sc)
							{ return sc ==  fs;}))
							{
								list.Add(fs);
							}
						}
						else if(fs.skills[k].skill <= 0)
						{
							if(list.Exists(delegate(FilmStaff sc)
							{ return sc ==  fs;}))
							{
								list.Remove(fs);
							}
							flag = true;
						}
					}
					if(flag)
					{
						break;
					}
				}
			}
		}
		
		for(int i = 0; i < list.Count; i++)
		{
			list[i].icon.transform.parent = infos.transform;
			list[i].icon.transform.localPosition = new Vector3(-250 + 350 * i, 0, -1);
			list[i].icon.transform.localScale = Vector3.one * 0.9f;
			list[i].icon.SetActive(true);
			list[i].icon.GetComponent<CharInfo>().Refresh();
			list[i].icon.GetComponent<CharInfo>().SetPricePerUse(script.genres);
			staffScrollArea.ContentLength = ((list.Count - 1) * 350) + 1;
		}
		
	}
	
	void AddNewStaff(FilmStaff staff)
	{
		if(currStaff.Exists(delegate(FilmStaff sc)
		{ return sc == staff;}))
		{
			staff.mark.enabled = false;
			price -= staff.icon.GetComponent<CharInfo>().GetPrice(script.genres);
			print ("remove");
		 	currStaff.Remove(staff);
		}
		else if (currStaff.Count < maxStaff)
		{
			staff.mark.enabled = true;	
			price += staff.icon.GetComponent<CharInfo>().GetPrice(script.genres);
			print ("add");
			currStaff.Add(staff);
		}
		priceMesh.text = price.ToString();
		priceMesh.Commit();
		CheckAcceptButton();
		SetPrice();
		textHeader.SetTextWithIndex(index, " " + currStaff.Count + "/" + maxStaff);
	}
	
	//возвращаем объект характеристик обратно родителям
	void CharInfoToParent()
	{
		foreach(FilmStaff fm in list)
		{
			fm.icon.transform.parent = fm.icon.GetComponent<CharInfo>().character.transform;
			fm.mark.enabled = false;
			fm.icon.SetActive(false);
		}
		list.Clear();
		list = new List<FilmStaff>();
	}
	
	void CharInfoToParent(List<FilmStaff> st)
	{
		foreach(FilmStaff f in st)
		{
			f.icon.transform.parent = f.transform;
			f.mark.enabled = false;
			f.icon.SetActive(false);
		}
		st.Clear();
		st = new List<FilmStaff>();
	}
	
	void FillStaffList(List<FilmStaff> list, CharacterType t)
	{
		switch(t)
		{
		case CharacterType.actor:
			actors.Clear();
			foreach(FilmStaff f in list)
			{
				actors.Add(f);
			}
			break;
		case CharacterType.cameraman:
			operators.Clear();
			foreach(FilmStaff f in list)
			{
				operators.Add(f);
			}
			break;
		case CharacterType.director:
			directors.Clear();
			foreach(FilmStaff f in list)
			{
				directors.Add(f);
			}
			break;
		}
	}
	
	void AddScript(Scenario s)
	{
		if(script == null)
		{
			script = s;
			foreach(Scenario sc in scen)
			{
				if(sc.activeIcon.enabled)
				{
					sc.activeIcon.enabled = false;
				}
			}
			ScenInfosToParent();
			SwitchState(HireForFilmState.mainWindow);
		}
		
	}
	
	void SetScriptInfo(string title, string genres)
	{
		textScenarioTitle.text = title;
		textScenarioGenres.text = genres;
		textScenarioGenres.Commit();
		textScenarioTitle.Commit();
	}
	
	void ScenInfosToParent()
	{
		foreach(Scenario s in scen)
		{
			s.TakeCharInfoBack();
		}
		scen.Clear();
	}
	
	void RefreshScenariosList()
	{
		foreach(InventoryItem ii in GlobalVars.inventory.items)
		{
			if(ii.GetComponent<Scenario>() != null)
			{
				scen.Add(ii.GetComponent<Scenario>());
			}
		}
		
		for(int i = 0; i < scen.Count; i++)
		{
			scen[i].icon.transform.parent = scenarioInfos.transform;
			scen[i].icon.transform.localPosition = new Vector3(-315 + i * 245, 115, -2);
			scen[i].icon.transform.localScale = Vector3.one;
			scen[i].icon.SetActive(true);
			scen[i].RefreshInfo();
			
			
		}
		scriptsScrollArea.ContentLength = scen.Count * 245 + 1;
	}
	
	void SetPrice()
	{
		price = 0;
		string s = "";
		foreach(FilmStaff f in actors)
		{
			price += f.icon.GetComponent<CharInfo>().GetPrice(script.genres);
		}
		foreach(FilmStaff f in directors)
		{
			price += f.icon.GetComponent<CharInfo>().GetPrice(script.genres);
		}
		foreach(FilmStaff f in operators)
		{
			price += f.icon.GetComponent<CharInfo>().GetPrice(script.genres);
		}
		foreach(FilmStaff f in currStaff)
		{
			price += f.icon.GetComponent<CharInfo>().GetPrice(script.genres);
		}
		if(price <= 0)
		{
			buttonAccept.transform.FindChild("textHire2").GetComponent<MeshRenderer>().enabled = true;
			buttonAccept.transform.FindChild("coins").GetComponent<MeshRenderer>().enabled = false;
			buttonAccept.transform.FindChild("textHire1").GetComponent<MeshRenderer>().enabled = false;
		}
		else if(price > 0)
		{
			s = price.ToString();
			buttonAccept.transform.FindChild("textHire1").GetComponent<MeshRenderer>().enabled = true;
			buttonAccept.transform.FindChild("coins").GetComponent<MeshRenderer>().enabled = true;
			buttonAccept.transform.FindChild("textHire2").GetComponent<MeshRenderer>().enabled = false;
		}
		priceMesh.text = s;
		priceMesh.Commit();
	}
	
	void CheckAcceptButton()
	{
		if(	state == HireForFilmState.mainWindow && 
			actors.Count > 0 && 
			directors.Count > 0 &&
			operators.Count > 0 &&
			script != null && 
			GlobalVars.money >= price)
		{
			buttonAccept.GetComponent<TapOnElement>().SetActiveTo(true);
		}
		else if(state == HireForFilmState.selectStaff && currStaff.Count > 0 && GlobalVars.money >= price)
		{
			buttonAccept.GetComponent<TapOnElement>().SetActiveTo(true);
		}
		else
		{
			buttonAccept.GetComponent<TapOnElement>().SetActiveTo(false);
		}
	}
	
	void SwitchState(HireForFilmState s)
	{
		state = s;
		switch(state)
		{
		case HireForFilmState.mainWindow:
			
			//gameObject.SetActiveRecursively(true);
			CharInfoToParent();
			FillStaffList(currStaff, staffType);
			RefreshStaffPlanes();
			if(script != null)
			{
				SetScriptInfo(script.name, script.GetGenres());
			}else
			{
				SetScriptInfo("","");
			}
			buttonPickScript.SetActive(true);
			staffPlanes.SetActive(true);
			buttonBack.SetActive(false);
			textButton.SetTextWithIndex(0);
			textHeader.SetTextWithIndex(0);
			topBlock.SetActive(true);
			CheckAcceptButton();
			SetStaffPlanesToWhite();
			break;
		case HireForFilmState.selectScript:
			//state = s;
			
			//gameObject.active = true;
			gameObject.SetActive(true);
			bgPart.SetActive(true);
			staffPlanes.SetActive(true);
			buttonPickScript.SetActive(false);
			buttonBack.SetActive(false);
			CharInfoToParent(actors);
			CharInfoToParent(directors);
			CharInfoToParent(operators);
			CharInfoToParent(currStaff);
			CharInfoToParent();
			RefreshStaffPlanes();
			//topBlock.SetActive();
			
			
			topBlock.SetActive(true);
			scenarioInfos.SetActive(true);
			if(staffPlanes.transform.localPosition.y != -80)
			{	
				//buttonAccept.SetActiveRecursively(false);
				StartCoroutine(MoveStaffPlanesUp(true));
			}
			else
			{
				
				RefreshScenariosList();
			}
			script = null;
			buttonAccept.SetActive(true);
			//HideMainWindow();
			
			//GlobalVars.SwipeCamera.enabled = true;
			//GlobalVars.SwipeCamera.gameObject.active = true;
			buttonBack.SetActive(false);
			//CheckAcceptButton();
			textHeader.SetTextWithIndex(1);
			SetPrice();
			break;
		case HireForFilmState.selectStaff:
			//state = s;
			HideMainWindow();
			//GlobalVars.SwipeCamera.enabled = true;
			//GlobalVars.SwipeCamera.gameObject.active = true;
			buttonAccept.SetActive(true);
			buttonBack.SetActive(true);
			CheckAcceptButton();
			textButton.SetTextWithIndex(1);
			break;
		case HireForFilmState.notActive:
			//state = s;
			scen = new List<Scenario>();
			list = new List<FilmStaff>();
			currStaff = new List<FilmStaff>();
			actors = new List<FilmStaff>();
			directors = new List<FilmStaff>();
			operators = new List<FilmStaff>();
			script = null;
			price = 0;
			//GlobalVars.SwipeCamera.gameObject.active = false;
			
			gameObject.SetActive(false);
			GlobalVars.cameraStates = CameraStates.normal;
			break;
		}
	}
	
	public tk2dSprite[] planesSprites;
	public tk2dTextMesh[] planesTextes;
	Color32 transparent = new Color32();
	
	void GetPlanesSprites()
	{
		transparent = Color.white;
		transparent.a = 100;
		planesSprites = staffPlanes.GetComponentsInChildren<tk2dSprite>();
		foreach(tk2dSprite mr in planesSprites)
		{
			mr.color = transparent;
		}
		planesTextes = staffPlanes.GetComponentsInChildren<tk2dTextMesh>(true);
		foreach(tk2dTextMesh mr in planesTextes)
		{
			mr.color = transparent;
			mr.Commit();
		}
	}
	
	void SetStaffPlanesToWhite()
	{
		planesSprites = staffPlanes.GetComponentsInChildren<tk2dSprite>();
		foreach(tk2dSprite mr in planesSprites)
		{
			mr.color = Color.white;
		}
		planesTextes = staffPlanes.GetComponentsInChildren<tk2dTextMesh>(true);
		foreach(tk2dTextMesh mr in planesTextes)
		{
			mr.color = Color.white;
		}
	}
	
	IEnumerator MoveStaffPlanesUp(bool isUp, float t = 0)
	{
		while(true)
		{
			if(t >= 1 || Input.GetMouseButtonDown(0))
			{
				if(!isUp)
				{
					staffPlanes.transform.localPosition = Vector3.zero;
					//buttonAccept.SetActiveRecursively(true);
					CheckAcceptButton();
					SetPrice();
					state = HireForFilmState.mainWindow;
				}
				else
				{
					staffPlanes.transform.localPosition = new Vector3(0, -80, -10);
					RefreshScenariosList();
				}
				yield break;
			}
			if(!isUp)
			{
				foreach(tk2dSprite mr in planesSprites)
				{
					mr.color = Color.Lerp(transparent, Color.white, t);
				}
				foreach(tk2dTextMesh mr in planesTextes)
				{
					mr.color = Color.Lerp(transparent, Color.white, t);
					mr.Commit();
				}
				staffPlanes.transform.localPosition = Vector3.Lerp(new Vector3(0,-80,-10), Vector3.zero, t);
			}
			else
			{
				foreach(tk2dSprite mr in planesSprites)
				{
					mr.color = Color.Lerp(Color.white, transparent, t);
				}
				foreach(tk2dTextMesh mr in planesTextes)
				{
					mr.color = Color.Lerp(Color.white, transparent, t);
					mr.Commit();
				}
				staffPlanes.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0, -80, -10), t);
			}
			t += Time.deltaTime;
			yield return 0;
		}
	}
	
	
	
	//уничтожить текущее меню, на вход принимается отправлять ли обратно сценарий в инвентарь(если мы закрываем меню), или не отправл\ть (если мы запускаем съемку фильма и отправляем сценарий в FilmMaking)
	void DestroyMenu(bool b)
	{
		if(b)
		{
			GlobalVars.inventory.items.Add(script.GetComponent<InventoryItem>());
			script.transform.parent = GlobalVars.inventory.transform;
		}
		CharInfoToParent();
		GlobalVars.cameraStates = CameraStates.normal;
		GlobalVars.currMenu = null;
		Destroy(this.gameObject);
	}
	
	//подготовка параметров в скрипт FilmMaking на ангаре
	void PrepareForNewFilm()
	{
		foreach(FilmStaff fs in actors)
		{
			fs.icon.transform.parent = fs.transform;
			fs.busy = true;
			fs.canBeUsed = false;
		}
		foreach(FilmStaff fs in directors)
		{
			fs.icon.transform.parent = fs.transform;
			fs.busy = true;
			fs.canBeUsed = false;
		}
		foreach(FilmStaff fs in operators)
		{
			fs.icon.transform.parent = fs.transform;
			fs.busy = true;
			fs.canBeUsed = false;
		}
		GlobalVars.inventory.items.Remove(script.GetComponent<InventoryItem>());
		hangar.busy = true;
		hangar.director = directors[0];
		hangar.cameraman = operators[0];
		hangar.actors = actors;
		hangar.script = script;
		//hangar.transform.parent = hangar.transform;
		//hangar.CallWorkers();
		hangar.SetScenesCount(script.numberOfScenes);
		hangar.budget = (int)price;
		GlobalVars.money -= (int)price;	
		SwitchState(HireForFilmState.notActive);
	}
}
