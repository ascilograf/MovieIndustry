using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//управление участками и картой проката фильмов
//скрытие/показ карты, увеличение денег по участкам каждый период.
public class MovieRental: MonoBehaviour 
{
	public List<MovieRentalItem> states;					//участки
	public List<MovieRentalItem> allStates;
	public List<FilmItem> films;							//фильмы в прокате
	public int period;										//период в секундах
	public GameObject menu;									//меню
	public GameObject openMenuButton;						//кнопка открыть
	public GameObject closeMenuButton;						//кнопка закрыть
	public BuyNewState popUpBuyNewState;
	public MapFilmsChart chart;
	public PopUpCityMenu cityMenu;
	
	public float lifeTime;									
	public float tempTime;	
	
	public List<FilmItem> tempFilms; 
	
	void Start()
	{
		foreach(FilmItem f in films)
		{
			f.revemuePerSec += Mathf.RoundToInt((f.revemuePerSec/100f) * GetPercentsFromCinemas());
		}
	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTapOnGUI);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTapOnGUI);
		chart.gameObject.SetActive(false);
	}
	
	void CheckTapOnGUI(GameObject go)
	{
		if(go == closeMenuButton)
		{
			GlobalVars.soundController.PlayMainTheme();
			menu.SetActive(false);	
			GlobalVars.cameraStates = CameraStates.normal;
			chart.gameObject.SetActive(false);
		}
	}
	
	public void CheckAvailability()
	{
		foreach(MovieRentalItem m in allStates)
		{
			m.CheckAvailability();
		}
	}
	
	IEnumerator ShowCoins(Cinema c)
	{
		while(true)
		{
			if(!c.iconGetMoney.Playing)
			{
				c.iconGetMoney.Stop();
				c.iconGetMoney.gameObject.SetActive(false);
				yield break;
			}
			yield return new WaitForSeconds(Time.deltaTime);
			yield return 0;
		}
	}
	
	public float GetPercentsFromCinemas()
	{
		float f = 0;
		foreach(MovieRentalItem m in states)
		{
			foreach(Cinema c in m.openedCinemas)
			{
				switch(c.grade)
				{
				case TownGrade.huge:
					f += GlobalVars.townGradePercent[2];
					break;
				case TownGrade.small:
					f += GlobalVars.townGradePercent[0];
					break;
				case TownGrade.usual:
					f += GlobalVars.townGradePercent[1];
					break;
				}
			}
		}
		return f;
	}
	
	public int GetCinemaCount()
	{
		int i = 0;
		foreach(MovieRentalItem m in states)
		{
			i += m.openedCinemas.Count; 
		}
		return i;
	}
	
	public void ShowMenu()
	{
		GlobalVars.soundController.PlayMapTheme();
		menu.SetActive(true);
		
		foreach(MovieRentalItem m in allStates)
		{
			m.gameObject.SetActive(true);
			m.GetComponent<MovieRentalItem>().ShowOpenedCinemas();
		}
		CheckAvailability();
		chart.gameObject.SetActive(true);
	}
	
	public void AddFilmAtRental(FilmItem f)
	{
		//GlobalVars.worldRental.films.Add(f);
		int rand = 0;
		rand = Random.Range(1, 101);
		if(rand <= f.secondTypeMarketing.failChance)
		{
			return;
		}
		else
		{
			rand -= f.secondTypeMarketing.failChance;
		}
		if(rand <= f.secondTypeMarketing.oneTownChance)
		{
			SetFilmToRentalInCinemas(1, f);
			print ("rand = " + rand);
			return;
		}
		else
		{
			rand -= f.secondTypeMarketing.oneTownChance;
		}
		if(rand <= f.secondTypeMarketing.twoTownsChance)
		{
			SetFilmToRentalInCinemas(2, f);
			print ("rand = " + rand);
			return;
		}
		else
		{
			rand -= f.secondTypeMarketing.twoTownsChance;
		}
		if(rand <= f.secondTypeMarketing.threeTownsChance)
		{
			SetFilmToRentalInCinemas(3, f);
			print ("rand = " + rand);
			return;
		}
	}
	
	void SetFilmToRentalInCinemas(int count, FilmItem film)
	{
		List<Cinema> allCinemas = new List<Cinema>();
		foreach(MovieRentalItem i in states)
		{
			foreach(Cinema c in i.openedCinemas)
			{
				allCinemas.Add(c);
			}
		}
		for(int i = 0; i < count; i++)
		{
			if(allCinemas.Count > 0)
			{
				int rand = Random.Range(0, allCinemas.Count);
				if(allCinemas[rand].film == null)
				{
					allCinemas[rand].film = film;
					allCinemas.Remove(allCinemas[rand]);
				}
			}
		}
	}
	
	//получение ревенью каждый период
	IEnumerator GetRevenue()
	{
		while(true)
		{
			chart.RefreshFilmList();
			//если фильмов в прокате больше нуля, то каждый период добавляем каждому участку с каждого фильма деньги
			if(films.Count > 0)
			{
				if(tempTime - lifeTime >= period)
				{
					
					tempTime = lifeTime;
					int price = 0;
					for(int i = 0; i < films.Count; i++)
					{
						if(films[i].lifeTime > 0)
						{
							price += (int)films[i].revemuePerSec * period;
							films[i].budgetToTake += ((int)films[i].revemuePerSec * period);
							films[i].lifeTime -= period;
						}
					}
					foreach(MovieRentalItem state in states)
					{
						if(state.openedCinemas.Count > 0)
						{
							state.RefreshCinemaRentals(price);
						}//state.money += price;
					}
				}
				else
				{
					lifeTime -= Time.deltaTime;
				}
			}
			if(tempFilms != films)
			{
				tempFilms = films;
				foreach(MovieRentalItem part in allStates)
				{
					foreach(Cinema c in part.openedCinemas)
					{
						c.films = films;
					}
				}
			}
			
			//yield return new WaitForSeconds(Time.deltaTime);
			yield return 0;
		}
	}
	
	public void AddFilmToChart(FilmItem film, float money)
	{
		chart.MaximizeTab();
		
		ShowMenu();
		films.Add(film);	
		StartCoroutine(AddMoneyToBudget(film, money));
		chart.RefreshFilmList();
		chart.ToBottom();
	}
	
	IEnumerator AddMoneyToBudget(FilmItem item, float money, float t = 0)
	{
		while(true)
		{
			if(Input.GetMouseButtonDown(0))
			{
				item.budgetTaken = money;
				yield return new WaitForSeconds(1);
				GlobalVars.cameraStates = CameraStates.normal;
				chart.gameObject.SetActive(false);
				GlobalVars.popUpFinishMakingFilm.SetParams(item);			
				break;
			}
			if(t >= 10)
			{
				yield return new WaitForSeconds(1);
				menu.SetActive(false);	
				GlobalVars.cameraStates = CameraStates.normal;
				chart.gameObject.SetActive(false);
				GlobalVars.popUpFinishMakingFilm.SetParams(item);
				item.budgetTaken = money;
				yield break;
			}
			t += Time.deltaTime;
			item.budgetTaken = (int)Mathf.Lerp(0, money, t/10);
			//yield return new WaitForSeconds(Time.deltaTime);
			yield return 0;			
		}
	}
	
	//OLD Version
	void Update () 
	{
		chart.RefreshFilmList();
	}
	
	
}
