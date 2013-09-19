using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Items
{
	public string title;
	public string description;
	public int time;
	public int price;
	public int val;
	public CityUpgradeType type;
	
	public GameObject button;
	public Transform moneyIcon;
	public tk2dTextMesh priceMesh;
}

//контроль меню улучшения города
//на вход идет город, 2 варианта, либо пропуск фильма в кинотеатр, либо улучшение города
//определение улучшения которое выбрано, запуск улучшения на городе.

public class PopUpCityMenu : MonoBehaviour 
{
	public GameObject buttonClose;
	public GameObject buttonAccept;
	
	public GameObject showForMerchPart;
	public GameObject showForRentalPart;
	
	public MeshRenderer coinsIcon;
	public MeshRenderer premIcons;
	
	public tk2dTextMesh lifeTimeMesh;
	public tk2dTextMesh revenueMesh;
	public tk2dTextMesh timeMesh;
	public tk2dTextMesh priceMesh;
	public tk2dTextMesh headerMesh;
	
	public Items[] items;
	public Cinema cinema;
	
	int priceCoins;
	int pricePremium;
	
	List<Items> addedItems;
	Items itemToUpgrade;
	
	public Transform iconPos;
	public GameObject filmIcon;
	public tk2dTextMesh textGenres;
	public tk2dTextMesh textTitle;
	
	public GameObject[] stars;
	
	int time;
	public GameObject theater;
	
	// Use this for initialization
	void Start () 
	{
		for(int i = 0; i < items.Length; i++)
		{
			items[i].price = GlobalVars.merchItems[i].price;
			items[i].time = GlobalVars.merchItems[i].time;
			items[i].val = GlobalVars.merchItems[i].val;
		}
	}
	
	// Update is called once per frame
	void Update () 
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
	
	void CheckTap(GameObject g)
	{
		if(g == buttonClose)
		{
			gameObject.SetActive(false);
			GlobalVars.worldRental.chart.cam.enabled = true;
			Destroy (theater);
		}
		else if(g == buttonAccept)
		{
			if(showForMerchPart.activeSelf && GlobalVars.money >= priceCoins && GlobalVars.stars >= pricePremium)
			{
				StartUpgrade();
				GlobalVars.worldRental.chart.cam.enabled = true;
			}
			else if(showForRentalPart.activeSelf)
			{
				StartRental();
				GlobalVars.worldRental.chart.cam.enabled = true;
			}
		}
		else
		{
			foreach(Items item in items)
			{
				if(g == item.button)
				{
					AddUpgrade(item);
				}
			}
		}
	}
	
	public void SetParams(Cinema inCinema, bool isShowForMerch)
	{
		cinema = inCinema;
		Clear();
		theater = Instantiate(cinema.iconCinema.gameObject) as GameObject;
		//theater.GetComponent<MeshRenderer>().enabled = true;
		theater.transform.parent = iconPos;
		theater.transform.localPosition = Vector2.zero;
		gameObject.SetActive(true);
		if(isShowForMerch)
		{
			showForMerchPart.SetActive(true);
			showForRentalPart.SetActive(false);
		}
		else
		{
			showForRentalPart.SetActive(true);
			showForMerchPart.SetActive(false);
		}
		Utils.SetText(lifeTimeMesh, Utils.FormatIntToUsualTimeString(cinema.periodInMins));
		Utils.SetText(revenueMesh, Utils.ToNumberWithSpaces(((int)(cinema.revenuePerMin)).ToString()));
		Utils.SetText(headerMesh, cinema.cinemaName);
		
		foreach(Items i in items)
		{
			SetItemToActive(i.button, true);
		}
		foreach(Items i in items)
		{
			foreach(CityUpgradeType t in cinema.upgrades)
			{
				if(t == i.type)
				{
					print ("not able: " + t.ToString());
					SetItemToActive(i.button, false);
				}
			}
		}
		if(cinema.filmInRental.film != null)
		{
			RefreshInfo(cinema.filmInRental.film);
		}
		else if(cinema.film != null)
		{
			RefreshInfo(cinema.film);
		}
		else
		{
			filmIcon.SetActive(false);
		}
		if(isShowForMerch)
		{
			showForMerchPart.SetActive(true);
			showForRentalPart.SetActive(false);
		}
		else
		{
			showForRentalPart.SetActive(true);
			showForMerchPart.SetActive(false);
			coinsIcon.enabled = false;
			premIcons.enabled = false;
			Utils.SetText(timeMesh, "");
			Utils.SetText(priceMesh, "");
		}
		GlobalVars.worldRental.chart.cam.enabled = false;
	}
	
	void SetItemToActive(GameObject g, bool active)
	{
		g.collider.enabled = active;
		tk2dSprite[] sprites = g.GetComponentsInChildren<tk2dSprite>(true);
		foreach(tk2dSprite s in sprites)
		{
			if(!active)
			{
				Color c = new Color32(160, 160, 160, 255);
				s.color = c;
			}
			else
			{
				s.color = Color.white;
			}
		}
	}
	
	void AddUpgrade(Items item)
	{
		if(itemToUpgrade == null)
		{
			itemToUpgrade = item;
			Refresh();
		}
		else if(item == itemToUpgrade)
		{
			Clear();
		}
		else
		{
			itemToUpgrade = item;
			Refresh();
		}
	}
	
	void Clear()
	{
		itemToUpgrade = null;
		Utils.SetText(priceMesh, Utils.ToNumberWithSpaces((0).ToString()));
		Utils.SetText(timeMesh, Utils.FormatIntToUsualTimeString(0, 2));
	}
	
	void Refresh()
	{
		time = 0;
		pricePremium = 0;
		priceCoins = 0;
		if(itemToUpgrade.type != CityUpgradeType.doubleLifeTime)
		{
			coinsIcon.enabled = true;
			premIcons.enabled = false;
			
			priceCoins += itemToUpgrade.price;
			Utils.SetText(priceMesh, Utils.ToNumberWithSpaces(((int)(priceCoins)).ToString()));
			
			//time = (int)((itemToUpgrade.time * 60 * 60) / 100);
			time = 10;
		}
		else
		{
			coinsIcon.enabled = false;
			premIcons.enabled = true;
			
			pricePremium += itemToUpgrade.price;
			Utils.SetText(priceMesh, Utils.ToNumberWithSpaces(((int)(pricePremium)).ToString()));
			time = (int)(0);
		}
		Utils.SetText(timeMesh, Utils.FormatIntToUsualTimeString(time, 2));
	}
	
	void StartUpgrade()
	{
		cinema.StartUpgrade(time, itemToUpgrade.type);
		if(itemToUpgrade.type != CityUpgradeType.doubleLifeTime)
		{
			cinema.revenuePerMin += (int)((cinema.revenuePerMin / 100) * itemToUpgrade.val);
			
		}
		else
		{
			cinema.periodInMins *= itemToUpgrade.val;
		}
		Utils.ChangeMoney(-priceCoins);
		GlobalVars.stars -= pricePremium;
		gameObject.SetActive(false);
		Destroy (theater);
	}
	
	void StartRental()
	{
		cinema.SetFilm(cinema.film);
		gameObject.SetActive(false);
		Destroy (theater);
	}
	
	public void RefreshInfo(FilmItem film)
	{
		textTitle.text = film.name;
		textTitle.Commit();
		
		textGenres.text = "";
		for(int i = 0; i < film.genres.Count; i++)
		{
			textGenres.text += film.genres[i].ToString();
			if(film.genres.Count == 2 && i == 0)
			{
				textGenres.text += ", ";
			}
		}
		textGenres.Commit();
		
		int skill = film.story;
		for(int i = 0; i < stars.Length; i++)
		{
			if((skill - 20) > 0)
			{	
				DeactivateStarExcept(stars[i], "5state");
			}
			else if(skill < 1)
			{
				DeactivateStarExcept(stars[i], "empty");
			}
			else if(skill < 5)
			{
				DeactivateStarExcept(stars[i], "1state", stars[i].transform);
			}
			else if(skill < 9)
			{
				DeactivateStarExcept(stars[i], "2state", stars[i].transform);
			}
			else if(skill < 13)
			{
				DeactivateStarExcept(stars[i], "3state", stars[i].transform);
			}
			else if(skill < 17)
			{
				DeactivateStarExcept(stars[i], "4state", stars[i].transform);
			}
			else if(skill < 21)
			{
				DeactivateStarExcept(stars[i], "5state", stars[i].transform);
			}
			skill -= 20;
		}	
	}
	
	//деактивация звезд кроме выбранный
	void DeactivateStarExcept(GameObject go,string state, Transform button = null)
	{
		Transform[] t = go.GetComponentsInChildren<Transform>(true);
		foreach(Transform tr in t)
		{
			if(tr.name == state)
			{
				tr.gameObject.SetActive(true);
			}
			else
			{
				tr.gameObject.SetActive(false);
			}
		}
	}
}
