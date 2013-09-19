using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//отображение иконок города и над городом
//управление надписью с монетками, список улучшений и отслеживание улучшений города

public class Cinema : MonoBehaviour 
{
	public enum States
	{
		normal,
		notActive,
		underUpgrade,
		readyForMerch,
		readyForTakeMoney,
		readyForFilm,
	}
	
	[System.Serializable]
	public class FilmInRental
	{
		public FilmItem film;
		public int revenuePerMin;
		public int lifeTime;
		public int lifeTimeGross;
		public int focusChance;
		public float timeRentalStart;
	}
	
	public tk2dAnimatedSprite iconGetMoney;		//иконка получить деньги
	public int money;							//кол-во денег на участке
	public MeshRenderer iconLocked;
	public MeshRenderer iconCoinsBubble;
	public MeshRenderer iconTruckBubble;
	public MeshRenderer iconSetFilm;
	public MeshRenderer iconCinema;
	public tk2dTextMesh moneyMesh;				//надпись денег
	public tk2dTextMesh nameMesh;
	int tempMoney;								//временныое кол-во денег (для исчезания)
	public string cinemaName;
	public TownGrade grade;
	public List<FilmItem> films;
	public FilmItem film;
	public int revenuePerMin;
	public int periodInMins;
	public List<CityUpgradeType> upgrades = new List<CityUpgradeType>();
	
	float tempTime = 0;
	float upgradeTime;
	States state;
	public FilmInRental filmInRental;
	
	void Start () 
	{
		state = States.readyForMerch;
		if(nameMesh != null)
		{
			nameMesh.text = cinemaName;
			nameMesh.Commit();
		}
		Color c = moneyMesh.color;
		c.a = 1;
		moneyMesh.color = c;
		GlobalVars.achievments.IncrMoviesReleased();
		
		moneyMesh.text = "";
		moneyMesh.Commit();
		
		SetParams();
		//Messenger<GameObject>.AddListener("Tap on target", CheckTapOnGUI);
	}
	
	void OnEnable()
	{
		if(iconGetMoney != null)
		{
			iconGetMoney.gameObject.SetActive(false);
		}
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
		//Coroutiner.StartCoroutine(Utils.MapMoneyGettingUp(moneyMesh, 2));
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
		//Coroutiner.StartCoroutine(Utils.MapMoneyGettingUp(moneyMesh, 2));
	}
	
	void CheckTap(GameObject go)
	{
		if(go == gameObject)
		{
			switch(state)
			{
			case States.normal:
				GlobalVars.worldRental.cityMenu.SetParams(this, true);
				break;
			case States.notActive:
				break;
			case States.readyForFilm:
				GlobalVars.worldRental.cityMenu.SetParams(this, false);
				break;
			case States.readyForMerch:
				GlobalVars.soundController.MerchClick();
				state = States.normal;
				SetTime(Time.time);
				SwitchState(States.normal);
				break;
			case States.readyForTakeMoney:
				GlobalVars.soundController.CollectMoney();
				if(filmInRental.film != null)
				{
					money += filmInRental.revenuePerMin * periodInMins;
					filmInRental.film.budgetTaken += filmInRental.revenuePerMin * periodInMins;
				}
				Utils.ChangeMoney(money);
				moneyMesh.gameObject.SetActive(true);
				Coroutiner.StartCoroutine(Utils.MapMoneyGettingUp(moneyMesh, moneyMesh.transform.GetChild(0).GetComponent<tk2dSprite>(), "+" + money.ToString(), 2));
				SetTime(0);
				iconGetMoney.gameObject.SetActive(true);
				iconGetMoney.Play();
				tempTime = Time.time;
				StartCoroutine(ShowCoins());
				SwitchState(States.normal);
				GlobalVars.worldRental.chart.RefreshFilmList();
				break;
			}
		}
	}
	
	void SetParams()
	{
		revenuePerMin = GlobalVars.townParams[(int)grade].revenuePerMin;
		periodInMins = GlobalVars.townParams[(int)grade].time;
	}
	
	public void StartUpgrade(float t, CityUpgradeType item)
	{
		upgradeTime = t;
		upgrades.Add(item);
		SwitchState(States.underUpgrade);
	}
	
	public void SetTime(float t)
	{
		tempTime = t;
	}
	
	void Update () 
	{
		if(!moneyMesh.gameObject.activeSelf)
		{
			if(state == States.underUpgrade)
			{
				if(upgradeTime <= Time.time)
				{
					SwitchState(States.normal);
					upgradeTime = Time.time;
				}
			}
			else if(state == States.readyForMerch)
			{
				SwitchState(States.readyForMerch);
			}
			else if(film != null)
			{
				SwitchState(States.readyForFilm);
			}
			else if((tempTime + periodInMins * 60) <= Time.time)  //past here fothis formula    
			{
				SwitchState(States.readyForTakeMoney);
				money = revenuePerMin * periodInMins;
				
			}
			else if(upgrades.Count == 0)
			{
				SwitchState(States.normal);
			}
			else
			{
				SwitchState(States.normal);
			}
		}
		if(filmInRental.film != null)
		{
			if((filmInRental.timeRentalStart + filmInRental.lifeTime) <= Time.time)
			{
				filmInRental.film = null;
				return;
			}
			if(filmInRental.film.lifeTime > 0)
			{	
				filmInRental.film.lifeTime = (int)((filmInRental.lifeTime) - (Time.time - filmInRental.timeRentalStart));
			}
			else
			{
				filmInRental.film.lifeTime = 0;
			}
		}
	}
	
	void SwitchState(States s)
	{
		switch(s)
		{
		case States.normal:
			iconCoinsBubble.enabled = false;
			iconTruckBubble.enabled = false;
			iconSetFilm.enabled = false;
			break;
		case States.underUpgrade:
			iconCoinsBubble.enabled = false;
			iconTruckBubble.enabled = false;
			iconSetFilm.enabled = false;
			break;
		case States.readyForMerch:
			iconTruckBubble.enabled = true;
			iconCoinsBubble.enabled = false;
			iconSetFilm.enabled = false;
			break;
		case States.readyForTakeMoney:
			iconCoinsBubble.enabled = true;
			iconTruckBubble.enabled = false;
			iconSetFilm.enabled = false;
			break;
		case States.readyForFilm:
			iconSetFilm.enabled = true;
			iconTruckBubble.enabled = false;
			iconCoinsBubble.enabled = false;
			break;
		}
		state = s;
	}
	
	IEnumerator ShowCoins()
	{
		while(true)
		{
			if(!iconGetMoney.Playing)
			{
				iconGetMoney.Stop();
				iconGetMoney.gameObject.SetActive(false);
				yield break;
			}
			yield return new WaitForSeconds(Time.deltaTime);
			yield return 0;
		}
	}
	
	public void SetFilm(FilmItem f)
	{
		film = null;
		filmInRental.film = f;
		filmInRental.lifeTime = f.visuals * 60;
		filmInRental.revenuePerMin = Formulas.SetRevenuePerMinForRental(f);
		filmInRental.timeRentalStart = Time.time;
		SwitchState(States.normal);
	}
}
