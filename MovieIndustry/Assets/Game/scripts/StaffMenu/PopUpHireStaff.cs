using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//меню с выбором персонажей для найма, в нем работает свайп
//также тут происходит рандомизация жанров и выставление начальных параметров.
public class PopUpHireStaff : MonoBehaviour {
	
	public GameObject buttonClose;					//кнопка закрыть меню
	public tk2dTextMesh priceMesh;					//текстмеш цены
			
	public List<GameObject> charInfos;				//плашки персонажей
	public SubMenu[] subMenus;						//подменю для каждого из персонажей
	public tk2dUIScrollableArea staffScroll;
	
	GameObject tempIcon = null;						
	public List<GameObject> premiumPersons;			//премиальные персонажи
		
	public List<FilmStaff> staff;					//это для мониторинга								
	int price;										//цена
	float maxLength = 0;							//макс. дальность свайпа
	float minLength = 0;							//мин. дальность свайпа
	
	
	SubMenu searchPlane;							//текущая плашка поиска
	int planeIndex;									//индекс плашки
	string _tag;										//тэг
	
	//bool b = false;
	void Start()
	{
		searchPlane = subMenus[0];
	}

	//обработка нажатий по табам и кнопке закрытия
	void CheckTap(GameObject go)
	{
		foreach(SubMenu sb in subMenus)
		{
			if(sb.tab == go)
			{
				if(charInfos.Count > 0)
				{
					freeCharInfos();
				}
				searchPlane = sb;
				DeactivateAllMenusExcept(sb.subMenu);
				SearchStaffWithTag(sb.type);
				SwitchButton(sb.tab);
				InfosToStart();
			}
		}
		if(go == buttonClose)
		{
			freeCharInfos();
			foreach(SubMenu sb in subMenus)
			{
				if(sb.type == "actor")
				{
					SwitchButton(sb.tab);
				}
			}
			
			gameObject.SetActive(false);
			GlobalVars.cameraStates = CameraStates.normal;
			
		}
	}
	
	//скрываем все меню до нажатия на таб
	void OnEnable()
	{
		foreach(SubMenu s in subMenus)
		{
			s.subMenu.SetActive(false);
		}
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
		
		Messenger.AddListener("Menu appear", ActivateTab);
	}
	
	string tg = "";
	FilmStaff fs = null;
	public void ShowMenu(string t, FilmStaff f = null)
	{
		tg = t;
		fs = f;
		GlobalVars.cameraStates = CameraStates.menu;
		gameObject.SetActive(true);
	}
	
	void ActivateTab()
	{
		foreach(SubMenu sb in subMenus)
		{
			if(sb.type == tg)
			{
				if(charInfos.Count > 0)
				{
					freeCharInfos();
				}
				searchPlane = sb;
				DeactivateAllMenusExcept(sb.subMenu);
				SearchStaffWithTag(sb.type);
				SwitchButton(sb.tab);
				if(fs != null)
				{
					print ("ASDAD!" + fs.name);
					for(int i = 0; i < charInfos.Count; i++)
					{
						CharInfo c = charInfos[i].GetComponent<CharInfo>();
						if(c.staff == fs)
						{
							c.staff.StartFadeCheckMark();
							print ("ASDAD!" + fs.name);
							Vector3 v3 = sb.subMenu.transform.localPosition;
							v3.x -= charInfos[i].transform.localPosition.x;
							if(v3.x < minLength)
							{
								v3.x = minLength;
							}
							if(v3.x > maxLength)
							{
								v3.x = maxLength;
							}
							sb.subMenu.transform.localPosition = v3;
						}
					}
				}
			}
		}	
	}
	
	void FocusOnStaff()
	{
		
	}
	
	void ActivateTabOnStaff(string t)
	{		
		foreach(SubMenu sb in subMenus)
		{
			if(sb.type == t)
			{
				if(charInfos.Count > 0)
				{
					freeCharInfos();
				}
				searchPlane = sb;
				DeactivateAllMenusExcept(sb.subMenu);
				SearchStaffWithTag(sb.type);
				SwitchButton(sb.tab);
			}
		}
	}
	
	void ActivateActorsTab()
	{
		foreach(SubMenu sb in subMenus)
		{
			if(sb.type == "actor")
			{
				if(charInfos.Count > 0)
				{
					freeCharInfos();
				}
				searchPlane = sb;
				DeactivateAllMenusExcept(sb.subMenu);
				SearchStaffWithTag(sb.type);
				SwitchButton(sb.tab);
			}
		}
	}
	
	//выключение камеры
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
		Messenger.RemoveListener("Menu appear", ActivateTab);
	}
	
	//выключение всех сабменю кроме выбранного
	void DeactivateAllMenusExcept(GameObject menu)
	{
		foreach(SubMenu sb in subMenus)
		{
			sb.subMenu.SetActive(false);
		}
		if(menu != null)
		{
			menu.SetActive(true);
			//searchPlane.searchPlane.SwitchToCuttent();
		}
	}
	
	//смена таба
	void SwitchButton(GameObject go)
	{
		if(tempIcon != null)
		{
			tempIcon.transform.FindChild("tabActive").GetComponent<MeshRenderer>().enabled = false;
			tempIcon.transform.FindChild("icon").localScale = Vector3.one;
		}
		tempIcon = go;
		tempIcon.transform.FindChild("icon").localScale = Vector3.one * 1.1f;
		tempIcon.transform.FindChild("tabActive").GetComponent<MeshRenderer>().enabled = true;
	}
	
	//поиск персонажа по тэгу, выстраивание списка, репозиция плашек персонажей, обновление параметров плашек
	void SearchStaffWithTag(string tag, bool b = false)
	{
		_tag = tag;
		staffScroll.contentContainer = searchPlane.subMenu;
		GameObject[] searchedStaff = GameObject.FindGameObjectsWithTag (_tag);
		for(int i = 0; i < searchedStaff.Length; i++)
		{
			print (searchedStaff[i].name);
			if(searchedStaff[i].GetComponent<FilmStaff>() != null)
			{
				Transform t = searchedStaff[i].GetComponent<FilmStaff>().icon.transform;
				t.parent = searchPlane.subMenu.transform;
				t.localPosition = new Vector3(-250 + i * 350, 0, 0);
				t.localScale = Vector3.one;
				t.gameObject.SetActive(true);
				t.GetComponent<CharInfo>().Refresh();
				if(t.GetComponent<CharInfo>().staff.freeSkillPoints > 0)
				{
					t.GetComponent<CharInfo>().ShowLvlUpButton();
				}
				charInfos.Add(t.gameObject);
			}
		}
		foreach(GameObject g in searchPlane.searchPlane.searchedStaff)
		{
			g.SetActive(false);
		}
		planeIndex = searchedStaff.Length;
		if(charInfos.Count > 1)
		{
			staffScroll.ContentLength = ((charInfos.Count - 1) * 350) + 220;
		}
		else
		{
			staffScroll.ContentLength = 351;
		}
		SetSearchPlane();
	}
	
	//показать плашки
	public void ShowCharInfos()
	{
		SearchStaffWithTag(_tag);
	}
	
	//скрыть плашки
	public void HideCharInfos()
	{
		foreach(GameObject g in charInfos)
		{
			g.SetActive(false);
		}
	}
	
	//привести парент-объекты плашек к стартовому состоянию
	public void InfosToStart()
	{
		foreach(SubMenu s in subMenus)
		{
			s.subMenu.transform.localPosition = new Vector3(0, 0,0);
		}
	}
	
	//установка дальностей свайпа
	public void SetMinMaxLength(float min, float max)
	{
		minLength = min;
		maxLength = max;
	}
	
	//передать плашки персонажей обратно персонажам
	void freeCharInfos()
	{
		foreach(GameObject g in charInfos)
		{
			g.transform.parent = g.GetComponent<CharInfo>().character.transform;
			g.GetComponent<CharInfo>().Refresh();
			g.SetActive(false);
			g.transform.localPosition = Vector3.one;
		}
		charInfos.Clear();
		charInfos = new List<GameObject>();
	}
	
	//отработка свайпа
	void Update()
	{
		//MoveButtonsOnTap();
	}
	
	float dist = 0;
	//свайп элементов
	void MoveButtonsOnTap()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Vector3 v3 = Input.mousePosition;
			v3.x -= searchPlane.subMenu.transform.localPosition.x;
			dist = v3.x;
		}
		
		if(Input.GetMouseButton(0))
		{
			Vector3 v3 = Input.mousePosition;
			v3.x -= dist;
			if(v3.x < minLength)
			{
				v3.x = minLength;
			}
			if(v3.x > maxLength)
			{
				v3.x = maxLength;
			}
			v3.z = searchPlane.subMenu.transform.localPosition.z;
			v3.y = searchPlane.subMenu.transform.localPosition.y;
			searchPlane.subMenu.transform.localPosition = v3;
		}
		if(Input.GetMouseButtonUp(0))
		{
			dist = 0;
		}
	}
	
	//позиционирование плашки поиска
	void SetSearchPlane()
	{
		searchPlane.searchPlane.transform.localPosition = new Vector3(-250 + 350 * planeIndex, 0, 0);
	}
	
	//установка параметров, на вход тип персонажа и примумный ли он
	public void SetParamsAndStart(CharacterType type, bool isPremium)
	{		
		//сначала определяется тип персонажа поданный на вход, в зависимости от этого выбирается нужный список доступных жанров
		List<AvailableGenre> ag = new List<AvailableGenre>();
		switch(type)
		{
		case CharacterType.actor:
			ag = GlobalVars.actorsGenres;
			break;
		case CharacterType.cameraman:
			ag = GlobalVars.cameramansGenres;
			break;
		case CharacterType.director:
			ag = GlobalVars.directorsGenres;
			break;
		case CharacterType.scripter:
			ag = GlobalVars.scriptersGenres;
			break;
		}
		
		//список персонажей заполняется тремя персонажами
		List<FilmStaff> st = new List<FilmStaff>();
		for(int i = 0; i < 3; i++)
		{
			//первый персонаж
			if(i == 0)
			{
				GameObject go = null;
				SetParamsForStaff(type, ag, out go, isPremium, 8, 16, new Vector3(-250 + i * 350, 0, 0));
				st.Add(go.GetComponent<FilmStaff>());
			}
			//второй и третий
			else
			{
				GameObject go = null;
				SetParamsForStaff(type, ag, out go, isPremium, 16, 20, new Vector3(-250 + i * 350, 0, 0));
				st.Add(go.GetComponent<FilmStaff>());
			}
			print (i.ToString());
		}
		planeIndex = 3;
		SetMinMaxLength((3) * -350 + 500, 0);
		SetSearchPlane();
	}
	
	//инициализация персонажей, обнуление их параметров, выборка доступных жанров и присвоение им новых значений
	void SetParamsForStaff(CharacterType ct, List<AvailableGenre> ag, out GameObject go, bool isPremium, int first, int second, Vector3 pos)
	{
		go = null;
		go = Instantiate(GlobalVars.commonPersonPrefab) as GameObject;
		go.tag = _tag;
		go.GetComponent<PersonController>().InstantiatePerson(ct);
		
		FilmStaff fs = go.GetComponent<FilmStaff>();
		searchPlane.searchPlane.searchedStaff.Add(fs.icon);

		fs.exp = 0;
		fs.lvl = 1;
		fs.freeSkillPoints = 0;
		
		if(ct != CharacterType.postProdWorker && ct != CharacterType.producer)
		{
			List<AvailableGenre> list = new List<AvailableGenre>();
			foreach(AvailableGenre a in ag)
			{
				list.Add(a);
			}
			if(list.Count > 0)
			{
				int rand = Random.Range(0, list.Count);	
				StaffSkills s = new StaffSkills();
				s.genre = list[rand].genre;
				s.skill = list[rand].max - first;
				fs.mainSkill.Add(s);
				foreach(FilmSkills skills in fs.skills)
				{
					if(skills.genre == fs.mainSkill[0].genre)
					{
						skills.skill = list[rand].max - first;
					}
				}
				list.Remove(list[rand]);
			}
			if(list.Count > 0)
			{
				int rand = Random.Range(0, list.Count);
				StaffSkills s = new StaffSkills();
				s.genre = list[rand].genre;
				s.skill = list[rand].max - first;
				fs.mainSkill.Add(s);
				foreach(FilmSkills skills in fs.skills)
				{
					if(skills.genre == fs.mainSkill[1].genre)
					{
						skills.skill = list[rand].max - second;
					}
				}
			}
		}
		else
		{
			fs.icon.GetComponent<CharInfo>().otherSkills.skill = GlobalVars.studioLvl * 8;
			if(ct == CharacterType.postProdWorker)
			{
				fs.icon.GetComponent<CharInfo>().otherSkills.activity = OtherActivities.Postproduction;
			}
			else if(ct == CharacterType.producer)
			{
				fs.icon.GetComponent<CharInfo>().otherSkills.activity = OtherActivities.Marketing;
			}
		}
		
		fs.icon.GetComponent<CharInfo>().SetParams(fs);
		fs.icon.SetActive(true);
		fs.icon.GetComponent<CharInfo>().Refresh();
		fs.icon.GetComponent<CharInfo>().ShowButton();
		fs.icon.transform.parent = searchPlane.subMenu.transform;
		fs.icon.transform.localPosition = pos;
		fs.icon.transform.localScale = Vector3.one;
	}
}
