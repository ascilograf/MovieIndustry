 using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//управление меню выбора персонажей для добавления визуальных эффектов фильму
//на старте выбирает первое свободное здание поспродакшена
//после выбора рабочих этот скрипт передает все нужные данные в скрипт PostProdOffice, прикрепленный к зданию
//и зовет выбранных рабочих к зданию
public class AddVisualEffects : MonoBehaviour 
{
	public FilmItem film;									//переменная фильма
	public GameObject closeButton;							//кнопка закрытия
	public GameObject accept;								//кнопка подтверждения
	public Transform infosStaff;
	public Transform infosFilms; 
	
	public GetTextFromFile headerText;
	public tk2dTextMesh timeMesh;
	public int time;
	public tk2dTextMesh budgetMesh;							//надпись бюджета
	public int budget;										//бюджет
	public List<FilmItem> films;
	public List<FilmStaff> staff;							//все доступные постпрод рабочие
	public List<FilmStaff> selectedStaff;					//все выбранные персонажи
	public int maxStaff;									//макс. кол-во рабочих
	public Vector3 tapUp, tapDown = Vector3.zero;			//позиции тачей
	public PostProdOfficeStage office;						//офис
	
	public Camera postProdsCamera;
	public Camera filmsCamera;
	public tk2dUIScrollableArea postProdsScrollArea;
	public tk2dUIScrollableArea filmsScrollArea;
	
	public MeshRenderer moneyIconMesh;
	
	public float minLengthX = 0; 
	public float maxLengthX = 0;
	public float minLengthY = 0; 
	public float maxLengthY = 0;
	//на старте обновляем кол-во свободных персонажей и ищем первый попавшийся постпрод офис, если не нашли, то закрываем это меню
	void Start()
	{
		//postProdsCamera.orthographicSize = Screen.height / 2;
	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
		//GlobalVars.SwipeCamera = postProdsCamera;
		//postProdsCamera.gameObject.active = true;
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
		//GlobalVars.SwipeCamera = null;
		//postProdsCamera.gameObject.active = false;
		MakeAllStaffPlanesTransparent(false);
	}
	
	void CheckTap(GameObject go)
	{
		if(go == accept)
		{
			if(film != null && selectedStaff.Count > 0)
			{
				CloseMenu();
				PrepareForOffice();	
			}
		}
		else if(go == closeButton)
		{
			GlobalVars.cameraStates = CameraStates.normal;
			gameObject.SetActive(false);
			office.busy = false;
			office = null;
			film = null;
			CloseMenu(true);
		}
		else if(film != null && go.name == "CharInfo")
		{
			AddStaff(go.GetComponent<CharInfo>().staff);
		}
		else if(go.GetComponent<ParentInfo>() != null)
		{
			AddFilm(go.GetComponent<ParentInfo>().parent.GetComponent<FilmItem>());
		}
		print (go.name);
	}
	
	public void ShowMenu(PostProdOfficeStage pp, FilmItem f = null)
	{
		film = f;
		maxStaff = pp.floor + 1;
		office = pp;
		gameObject.SetActive(true);
		RefreshPostProdWorkers();
		RefreshFilmsList();
		infosFilms.localPosition = Vector3.zero;
		infosStaff.localPosition = Vector3.zero;
		if(film == null)
			MakeAllStaffPlanesTransparent(true);
		else if (film != null)
			MakeAllStaffPlanesTransparent(false);
		headerText.SetTextWithIndex(0);
		SetBudget();
	}
	
	void AddFilm(FilmItem f)
	{
		if(film == null)
		{
			film = f;	
			film.activeIcon.enabled = true;
			foreach(FilmStaff ff in selectedStaff)
			{
				ff.mark.enabled = false;
			}
			selectedStaff.Clear();
			MakeAllStaffPlanesTransparent(false);
		}
		else if(f == film)
		{
			film.activeIcon.enabled = false;
			film = null;
			MakeAllStaffPlanesTransparent(true);
		}
		else if(film != null)
		{				
			film.activeIcon.enabled = false;	
			film = f;	
			film.activeIcon.enabled = true;
			foreach(FilmStaff ff in selectedStaff)
			{
				ff.mark.enabled = false;
			}
			selectedStaff.Clear();
			MakeAllStaffPlanesTransparent(false);
		}
		budget = 0;
		time = 0;
		Utils.SetText(timeMesh, Utils.FormatIntToUsualTimeString((int)time, 2));
		SetBudget();
	}
	
	void CheckTapOnFilm()
	{
		/*if(	Input.GetMouseButtonUp(0) && 
			Input.mousePosition.x < filmsCamera.pixelRect.xMax &&
			Input.mousePosition.x > filmsCamera.pixelRect.xMin &&
			Input.mousePosition.y < filmsCamera.pixelRect.yMax &&
			Input.mousePosition.y > filmsCamera.pixelRect.yMin)
		{
			RaycastHit hit;
			Ray ray = filmsCamera.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				if(hit.collider.name == "filmInfo")
				{
					FilmItem f = hit.collider.GetComponent<ParentInfo>().parent.GetComponent<FilmItem>();
					if(film == null)
					{
						film = f;	
						film.activeIcon.enabled = true;
						foreach(FilmStaff ff in selectedStaff)
						{
							ff.mark.enabled = false;
						}
						selectedStaff.Clear();
						MakeAllStaffPlanesTransparent(false);
					}
					else if(f == film)
					{
						film.activeIcon.enabled = false;
						film = null;
						MakeAllStaffPlanesTransparent(true);
					}
					else if(film != null)
					{				
						film.activeIcon.enabled = false;	
						film = f;	
						film.activeIcon.enabled = true;
						foreach(FilmStaff ff in selectedStaff)
						{
							ff.mark.enabled = false;
						}
						selectedStaff.Clear();
						MakeAllStaffPlanesTransparent(false);
					}
					budget = 0;
					time = 0;
					Utils.SetText(timeMesh, Utils.FormatIntToUsualTimeString((int)time, 2));
					SetBudget();
				}
			}
			SetHeaderText();
		}
		*/
	}
	
	void CloseMenu(bool freeStaff = false)
	{
		foreach(FilmItem f in films)
		{
			f.TakeCharInfoBack();
			f.icon.SetActive(false);
		}
		foreach(FilmStaff fs in staff)
		{
			fs.icon.transform.parent = fs.icon.GetComponent<CharInfo>().staff.transform;
			if(freeStaff)
			{
				fs.canBeUsed = true;
			}
			fs.icon.SetActive(false);
			fs.mark.enabled = false;
		}
		budget = 0;
		time = 0;
		Utils.SetText(budgetMesh, "");
		Utils.SetText(timeMesh, "");
		MakeAllStaffPlanesTransparent(false);
		//film = null;
		films = new List<FilmItem>();
		staff = new List<FilmStaff>();
	}
	
	//обновление кол-ва незанятых персонажей
	public void RefreshPostProdWorkers()
	{
		selectedStaff.Clear();
		staff.Clear();
		selectedStaff = new List<FilmStaff>();
		staff = new List<FilmStaff>();
		
		GameObject[] go = GameObject.FindGameObjectsWithTag("postProdWorker");
		for(int i = 0; i < go.Length; i++)
		{
			FilmStaff fs = go[i].GetComponent<FilmStaff>();
			if(fs.canBeUsed)
			{
				staff.Add(fs);
			}
		}
		if(staff.Count > 0)
		{
			for(int i = 0; i < staff.Count; i++)
			{
				staff[i].icon.transform.parent = infosStaff.transform;
				staff[i].icon.transform.localPosition = new Vector3(160 + i * 350,0,-1);
				staff[i].icon.transform.localScale = new Vector3(1,1,1);
				staff[i].icon.SetActive(true);
				staff[i].icon.GetComponent<CharInfo>().Refresh();
				staff[i].icon.GetComponent<CharInfo>().SetPricePerUse();
			}
			postProdsScrollArea.ContentLength = staff.Count * 350 + 1;
		}
 	}
	
	void MakeAllStaffPlanesTransparent(bool makeTransparent)
	{
		tk2dSprite[] sprites = infosStaff.GetComponentsInChildren<tk2dSprite>(true);
		Color32 c = new Color32();	
		foreach(tk2dSprite s in sprites)
		{
			if(makeTransparent)
			{
				c = Color.grey;
			}
			else
			{
				c = Color.white;
			}
			s.color = c;
		}
		tk2dTextMesh[] textes = infosStaff.GetComponentsInChildren<tk2dTextMesh>(true);	
		foreach(tk2dTextMesh s in textes)
		{
			if(makeTransparent)
			{
				c = s.color;
				c.a = 100;
			}
			else
			{
				c = Color.white;
			}
			s.color = c;
			s.Commit();
		}
		if(makeTransparent)
		{
			foreach(FilmStaff f in selectedStaff)
			{
				f.mark.enabled = false;
			}
			selectedStaff.Clear();
		}
	}
	
	void RefreshFilmsList()
	{
		films.Clear();
		films = new List<FilmItem>();
		foreach(InventoryItem ii in GlobalVars.inventory.items)
		{
			FilmItem f = ii.GetComponent<FilmItem>();
			if(f != null)
			{
				if(!f.busy && f.visuals == 0)
				{
					films.Add(ii.GetComponent<FilmItem>());
				}
			}
		}
		if(films.Count > 0)
		{
			for(int i = 0; i < films.Count; i++)
			{
				films[i].icon.transform.parent = infosFilms.transform;
				films[i].icon.transform.localPosition = new Vector3(-280, 100 - i * 160, -1);
				films[i].icon.transform.localScale = Vector3.one;
				films[i].icon.SetActive(true);
				films[i].activeIcon.enabled = false;
				films[i].RefreshInfo();
			}
			filmsScrollArea.ContentLength = films.Count * 148 + 1;
		}
		if(film != null)
		{
			film.activeIcon.enabled = true;
		}
		accept.GetComponent<TapOnElement>().SetActiveTo(false);
		SetBudget();
	}
	
	
	//добавить/удалить персонажа
	void AddStaff(FilmStaff fs)
	{
		if(selectedStaff.Exists(delegate(FilmStaff p)
		{ return p ==  fs;}))
		{
			fs.canBeUsed = true;
			fs.mark.enabled = false;
			budget -= fs.icon.GetComponent<CharInfo>().GetPrice();
			time -= fs.icon.GetComponent<CharInfo>().otherSkills.skill * 20;
			selectedStaff.Remove(fs);
		}
		else if(selectedStaff.Count < maxStaff)
		{
			fs.canBeUsed = false;
			fs.mark.enabled = true;
			selectedStaff.Add(fs);
			budget += fs.icon.GetComponent<CharInfo>().GetPrice();
			time += fs.icon.GetComponent<CharInfo>().otherSkills.skill * 20;
		}
		SetBudget();
		Utils.SetText(timeMesh, Utils.FormatIntToUsualTimeString((int)time, 2));
		SetHeaderText();
	}
	
	void SetHeaderText()
	{
		if(film != null)
		{
			headerText.SetTextWithIndex(1, " " + selectedStaff.Count + "/" + maxStaff);
		}
		else
		{
			headerText.SetTextWithIndex(0);
		}	
	}
	
	//приготовить офис к начале работ, передача переменных, вызов рабочих к зданию и уничтожение меню.
	void PrepareForOffice()
	{
		foreach(FilmStaff p in selectedStaff)
		{
			p.canBeUsed = false;
			p.mark.enabled = false;
			p.icon.SetActive(false);
		}
		List<FilmStaff> list = new List<FilmStaff>();
		foreach(FilmStaff fs in selectedStaff)
		{
			list.Add(fs);
		}
		GlobalVars.inventory.items.Remove(film.GetComponent<InventoryItem>());
		GlobalVars.cameraStates = CameraStates.normal;
		film.busy = true;
		FilmItem f = null;
		f = film;
		office.SetParams(f, list);
		gameObject.SetActive(false);
		film = null;
		office = null;
	}
	
	void SetBudget()
	{
		string s = "";
		if(budget > 0)
		{
			s = budget.ToString();
			moneyIconMesh.enabled = true;
			if(budget <= GlobalVars.money)
			{
				accept.GetComponent<TapOnElement>().SetActiveTo(true);
				accept.transform.FindChild("caption").transform.localPosition = new Vector3(0, 13, -5);
				accept.transform.FindChild("caption").transform.localScale = Vector3.one;
			}
			else
			{
				accept.GetComponent<TapOnElement>().SetActiveTo(false);
				accept.transform.FindChild("caption").transform.localPosition = new Vector3(0, 0, -5);
				accept.transform.FindChild("caption").transform.localScale = Vector3.one * 1.3f;
			}
		}
		else
		{
			accept.GetComponent<TapOnElement>().SetActiveTo(false);
			accept.transform.FindChild("caption").transform.localPosition = new Vector3(0, 0, -5);
			accept.transform.FindChild("caption").transform.localScale = Vector3.one * 1.3f;
			moneyIconMesh.enabled = false;
		}
		Utils.SetText(budgetMesh, Utils.ToNumberWithSpaces(s));
	}
	
	//уничтожить это меню, на взод булева, если да, то помещаем фильм обратно в инвентарь.
	void DestroyThisMenu(bool b)
	{
		if(b)
		{
			GlobalVars.inventory.items.Add(film.GetComponent<InventoryItem>());
			film.transform.parent = GlobalVars.inventory.transform;
		}
		StaffManagment.CharInfoToParent(staff);
		GlobalVars.currMenu = null;
		GlobalVars.cameraStates = CameraStates.normal;
		gameObject.SetActive(false);
	}
	
	void Update()
	{
		/*if(film != null)
		{
			MoveStaffInfosOnTap();
		}
		else
		{
			MoveFilmInfosOnTap();
		}
		CheckTapOnFilm();*/
	}
	
	float dist = 0;
	void MoveStaffInfosOnTap()
	{
		//if(	Input.mousePosition.x < postProdsCamera.pixelRect.xMax &&
		//	Input.mousePosition.x > postProdsCamera.pixelRect.xMin &&
		//	Input.mousePosition.y < postProdsCamera.pixelRect.xMax &&
		//	Input.mousePosition.y > postProdsCamera.pixelRect.xMin)
		//{
			if(Input.GetMouseButtonDown(0))
			{
				Vector3 v3 = Input.mousePosition;
				v3.x -= infosStaff.localPosition.x;
				dist = v3.x;
			}
			
			if(Input.GetMouseButton(0))
			{
				
				Vector3 v3 = Input.mousePosition;
				v3.x -= dist;
				//print (v3.x);
				if(v3.x < minLengthX)
				{
					v3.x = minLengthX;
				}
				if(v3.x > maxLengthX)
				{
					v3.x = maxLengthX;
				}
				v3.z = infosStaff.localPosition.z;
				v3.y = infosStaff.localPosition.y;
				infosStaff.localPosition = v3;
			}
		//}
		if(Input.GetMouseButtonUp(0))
		{
			dist = 0;
		}
	}
	
	void MoveFilmInfosOnTap()
	{
		//if( Input.mousePosition.x < filmsCamera.pixelRect.xMax &&
		//	Input.mousePosition.x > filmsCamera.pixelRect.xMin &&
		//	Input.mousePosition.y < filmsCamera.pixelRect.xMax &&
		//	Input.mousePosition.y > filmsCamera.pixelRect.xMin)
		//{
		if(Input.GetMouseButtonDown(0))
		{
			Vector3 v3 = Input.mousePosition;
			v3.y -= infosFilms.localPosition.y;
			dist = v3.y;
		}
		
		if(Input.GetMouseButton(0))
		{
			Vector3 v3 = Input.mousePosition;
			v3.y -= dist;
			if(v3.y < minLengthY)
			{
				v3.y = minLengthY;
			}
			if(v3.y > maxLengthY)
			{
				v3.y = maxLengthY;
			}
			v3.z = infosFilms.localPosition.z;
			v3.x = infosFilms.localPosition.x;
			infosFilms.localPosition = v3;
		}
		if(Input.GetMouseButtonUp(0))
		{
			dist = 0;
		}
	}
}//
