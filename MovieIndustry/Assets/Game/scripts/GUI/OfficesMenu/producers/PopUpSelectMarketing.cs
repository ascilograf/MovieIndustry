using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//сначала свайпом выбирается фильм, фиксируется, 
//потом выбирается маркетинг, также как сеттинг
//по нажатие на "ок" отправляется сигнал о запуске маркетинга в офис.
public class PopUpSelectMarketing : MonoBehaviour {
	
	public List<FilmItem> films;						//список всех фильмов
	public GameObject popUpMarketing;					//меню
	public GameObject buttonClose;						//кнопка закрыть
	public TapOnElement buttonAccept;					//кнопка принять
	public GetTextFromFile headerText;
	public tk2dTextMesh priceMesh;
	public tk2dTextMesh timeMesh;
	public GameObject acceptButtonCaption;
	public tk2dUIScrollableArea filmScroll;						//инфо
	
	public Camera filmsCamera;
	public Camera producersCamera;
	
	public MarketingButton[] marketingButtons;
	public Transform marketingPlanesParent;
	
	public float timeToMarketing;						//время на маркетинг
	public int price;									//цена
	public int time;								
	public MarketingButton firstMarketing;					
	public MarketingButton secondMarketing;
	public List<PersonController> producers;			//продюсеры
	public int maxMarketings;							//макс. кол-во маркетингов
	public FilmItem currFilm;							//текущий фильм
	public MarketingOfficeStage office;
	
	void Start()
	{
		
	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);

	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	public void SetParams(MarketingOfficeStage stage, FilmItem f = null)
	{
		currFilm = f;
		gameObject.SetActive(true);
		GlobalVars.cameraStates = CameraStates.menu;
		office = stage;
		office.isBusy = true;
		RefreshFilmsList();
		RefreshMarketingButtons();
		if(currFilm == null)
		{
			SetColorForMarketingButtons(Color.grey);
		}
		else
		{
			SetColorForMarketingButtons(Color.white);
		}
		RefreshTextMeshes();
	}
	
	void CheckTap(GameObject go)
	{
		//если нажали закрыть - закрываем
		if(go == buttonClose)
		{	
			if(currFilm != null)
				currFilm.activeIcon.enabled = false;
			SetMarketingsToNull();
			office.isBusy = false;
			//swipe.InfosToStart();
			gameObject.SetActive(false);
			FilmInfoToParent();
			GlobalVars.cameraStates = CameraStates.normal;
		}
		
		//если нажали на принять - запускаем маркетинг и скрываем меню
		else if(go == buttonAccept.gameObject)
		{
			Finish();
		}
		if(go.GetComponent<ParentInfo>() != null)
		{
			AddFilm(go.GetComponent<ParentInfo>().parent.GetComponent<FilmItem>());
		}
		else
		{
			foreach(MarketingButton b in marketingButtons)
			{
				if(go == b.gameObject)
				{
					AddMarketing(b);
				}
			}
		}
		
	}
	
	//отправить информацию о фильмах обратно фильмам
	void FilmInfoToParent()
	{
		foreach(FilmItem f in films)
		{
			f.TakeCharInfoBack();
		}
	}
	
	//обновить список фильмов
	public void RefreshFilmsList()
	{
		films.Clear();

		foreach(InventoryItem ii in GlobalVars.inventory.items)
		{
			if(ii.GetComponent<FilmItem>() != null)
			{
				if(ii.GetComponent<FilmItem>().visuals > 0 && !ii.GetComponent<FilmItem>().busy)
				{
					films.Add(ii.GetComponent<FilmItem>());
				}
			}
		}
		
		if(films.Count > 0)
		{
			for(int i = 0; i < films.Count; i++)
			{
				films[i].icon.transform.parent = filmScroll.contentContainer.transform;
				films[i].icon.transform.localPosition = new Vector3(-280, (i * -150) + 105, -10);
				films[i].icon.transform.localScale = Vector3.one;
				films[i].icon.SetActive(true);
				films[i].activeIcon.enabled = false;
				films[i].RefreshInfo();
				//swipe.items.Add(films[i].info.gameObject);
			}
		}
		if(films.Count >= 2)
		{
			filmScroll.ContentLength = (films.Count - 2) * 140 + 351;
		}
		else
		{
			filmScroll.ContentLength = 351;
		}
		
		if(currFilm != null)
		{
			currFilm.activeIcon.enabled = true;
		}
	}
	
	//добавить маркетинг
	void AddMarketing(MarketingButton m)
	{
		if(m.type == MarketingTypes.FirstType)
		{
			if(firstMarketing == null)
			{
				firstMarketing = m;
				firstMarketing.SetChoosen(true);
			}
			else if(m == firstMarketing)
			{
				firstMarketing.SetChoosen(false);
				firstMarketing = null;
			}
			else if(m != null)
			{
				firstMarketing.SetChoosen(false);
				firstMarketing = m;
				firstMarketing.SetChoosen(true);
			}
		}
		else if(m.type == MarketingTypes.SecondType)
		{
			if(secondMarketing == null)
			{
				secondMarketing = m;
				secondMarketing.SetChoosen(true);
			}
			else if(m == secondMarketing)
			{
				secondMarketing.SetChoosen(false);
				secondMarketing = null;
			}
			else if(m != null)
			{
				secondMarketing.SetChoosen(false);
				secondMarketing = m;
				secondMarketing.SetChoosen(true);
			}
		}
		RefreshTextMeshes();
	}
	
	void AddFilm(FilmItem f)
	{
		if(currFilm == null)
		{
			currFilm = f;
			currFilm.activeIcon.enabled = true;
			SetColorForMarketingButtons(Color.white);
		}
		else if(currFilm == f)
		{
			currFilm.activeIcon.enabled = false;
			currFilm = null;
			SetColorForMarketingButtons(Color.grey);
		}
		else if(currFilm != null)
		{
			currFilm.activeIcon.enabled = false;
			currFilm = f;
			currFilm.activeIcon.enabled = true;
			SetColorForMarketingButtons(Color.white);
		}
		SetMarketingsToNull();
		RefreshTextMeshes();
	}
	
	void RefreshMarketingButtons()
	{
		if(office == null)
			return;
		
		foreach(MarketingButton m in marketingButtons)
		{
			if(m.marketingLvl > office.officeStage)
			{
				m.collider.enabled = false;
				m.tooHighForThisStage.enabled = true;
			}
			else
			{
				m.collider.enabled = true;
				m.tooHighForThisStage.enabled = false;
			}
		}
	}
	
	void SetMarketingsToNull()
	{
		if(firstMarketing != null)
		{
			firstMarketing.SetChoosen(false);
			firstMarketing = null;
		}
		if(secondMarketing != null)
		{
			secondMarketing.SetChoosen(false);
			firstMarketing = null;
		}
	}
	
	void SetColorForMarketingButtons(Color32 col)
	{
		tk2dSprite[] sprites = marketingPlanesParent.GetComponentsInChildren<tk2dSprite>();
		tk2dTextMesh[] textes = marketingPlanesParent.GetComponentsInChildren<tk2dTextMesh>();
		foreach(tk2dSprite s in sprites)
		{
			if(s.name != "blocked")
			{
				s.color = col;
			}
		}
		foreach(tk2dTextMesh t in textes)
		{
			t.color = col;
			t.Commit();
		}
	}
	
	void CheckTapOnFilm()
	{
		if(	Input.GetMouseButtonUp(0) && 
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
					FilmItem film = hit.collider.GetComponent<ParentInfo>().parent.GetComponent<FilmItem>();
					AddFilm(film);
				}
			}
		}
	}
	
	void RefreshTextMeshes()
	{
		//time = 10;
		if(firstMarketing != null || secondMarketing != null)
		{
			acceptButtonCaption.transform.localScale = Vector3.one;// * 1.2f;
			acceptButtonCaption.transform.localPosition = new Vector3(0, 13, -5);
			//Utils.SetText(timeMesh, Utils.FormatIntToUsualTimeString(time, 2));
			buttonAccept.SetActiveTo(true);
			price = 0;
			if(firstMarketing != null)
				price = (int)((Formulas.FirstWeekEndMoney(currFilm) / 100) * firstMarketing.firstType.bonus);
			if(secondMarketing != null)
				price += secondMarketing.secondType.price;
			Utils.SetText(priceMesh, Utils.ToNumberWithSpaces(price.ToString()));
		}
			//headerText.SetTextWithIndex(1);
		else if(firstMarketing == null && secondMarketing == null)
		{
			acceptButtonCaption.transform.localScale = Vector3.one * 1.2f;
			acceptButtonCaption.transform.localPosition = new Vector3(0, 0, -5);
			//Utils.SetText(timeMesh, Utils.FormatIntToUsualTimeString(time, 2));
			Utils.SetText(priceMesh, Utils.ToNumberWithSpaces(price.ToString()));
			buttonAccept.SetActiveTo(false);
			//headerText.SetTextWithIndex(0);
		}
	}
	
	void Finish()
	{
		if(GlobalVars.money >= price)
			Utils.ChangeMoney(-price);
		else
			return;
		if(firstMarketing != null)
			currFilm.firstTypeMarketingBonus = Formulas.GetChanceToBonus(firstMarketing.firstType);
		if(secondMarketing != null)
			currFilm.secondTypeMarketing = secondMarketing.secondType;
		else
		{
			currFilm.secondTypeMarketing.failChance = 25;
			currFilm.secondTypeMarketing.oneTownChance = 40;
			currFilm.secondTypeMarketing.twoTownsChance = 20;
			currFilm.secondTypeMarketing.threeTownsChance = 15;	
		}
		currFilm.busy = true;
		office.isBusy = false;
		currFilm.activeIcon.enabled = false;
		gameObject.SetActive(false);
		GlobalVars.worldRental.AddFilmToChart(currFilm, Formulas.FirstWeekEndMoney(currFilm));
		SetMarketingsToNull();
		FilmInfoToParent();
	}
	
	void Update()
	{
		//CheckTapOnFilm();
	}
}
