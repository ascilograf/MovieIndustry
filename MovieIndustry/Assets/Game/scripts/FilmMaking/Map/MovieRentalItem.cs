using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//контроль участка сбора спедств с проката фильма,
//физическое представление участка в игре, кол-во денег на участке и плавное исчезание денег
public class MovieRentalItem : MonoBehaviour 
{
	//public MeshRenderer iconGetMoney;			//иконка получить деньги
	public GameObject iconBuyItem;			 	//иконка купить этот участок карты
	public GameObject iconBuyCinema;			//иконка купить кинотеатр
	public tk2dTextMesh text;					//текст кинотеатров .. из ..
	public int priceToBuyItem;					//цена покупки этого участка карты
	public int priceToBuyCinema;				//цена на покупку кинотеатра
	public int cinemaCount;						//кол-во кинотеатров
	public int maxCinemaCount;					//макс. кол-во кинотеатров
	public float timeToUpgrade;					//время для апгрейда
	public float timeToUpgradeModifier;			//множитель времени
	public List<Cinema> cinemas;					//кмнотеатры
	public List<Cinema> openedCinemas;
	//public int money;							//кол-во денег на участке
	//public tk2dTextMesh moneyMesh;			//надпись денег
	//float time = 0;							//время (для исчезания)
	//int tempMoney;							//временныое кол-во денег (для исчезания)
	public bool isAvailable = false;			//доступен ли этот участок
	public MovieRental popUpRental;				//контроллер карты
	public float tempTime;
	//делаем невидимым меш циферок
	void Start()
	{
		//Utils.ShowHideObject(iconBuyCinema, false);
		//CinemaCountCheck();
		
		Messenger<GameObject>.AddListener("Tap on target", CheckTapOnGUI);
		StartCoroutine(EachTimeToDo());
		CheckAvailability();
		if(isAvailable)
		{
			ShowOpenedCinemas();
		}
	}
		
	void CheckTapOnGUI(GameObject go)
	{
		if(go == gameObject && !isAvailable)
		{
			if(popUpRental.allStates.Exists(delegate (MovieRentalItem m)
			{	return this == m;	}))
			{
				if(!popUpRental.states.Exists(delegate (MovieRentalItem m)
				{	return this == m;	}) || popUpRental.states.Count == 0)
					{
						popUpRental.popUpBuyNewState.SetForBuyState(popUpRental, this, priceToBuyItem * popUpRental.states.Count, name + " region");
						return;
					}
				}
		}
		if(go.tag == "Cinema")
		{
			Cinema c = go.GetComponent<Cinema>();
			if(cinemas.Exists(delegate (Cinema cin)
			{	return c == cin;		}))
			{
				if(!openedCinemas.Exists(delegate (Cinema cin)
				{	return cin == c;		}) || openedCinemas.Count == 0)
				{
					popUpRental.popUpBuyNewState.SetForBuyCinema(this, c, priceToBuyCinema * popUpRental.GetCinemaCount() * ((int)c.grade + 1), c.name, c.transform.FindChild("theater").gameObject);
					return;
				}
			}	
		}
	}
	
	void OnEnable()
	{
		if(isAvailable)
		{
			ShowOpenedCinemas();
		}
	}
	
	public void ShowOpenedCinemas()
	{
		foreach(Cinema c in cinemas)
		{
			c.gameObject.SetActive(true);
			c.iconGetMoney.gameObject.SetActive(false);
			c.iconLocked.enabled = false;
			c.iconCoinsBubble.enabled = false;
			c.iconTruckBubble.enabled = false;
			c.iconSetFilm.enabled = false;
			c.iconCinema.enabled = false;
			//c.iconLocked.gameObject.SetActive(false);
			c.moneyMesh.gameObject.SetActive(false);
			if(openedCinemas.Exists(delegate (Cinema cin)
			{	return cin == c;	}))
			{
				c.iconLocked.enabled = false;
				c.iconCinema.enabled = true;
				c.enabled = true;
			}
			else if(isAvailable)
			{
				c.iconLocked.enabled = true;
				c.enabled = false;
			}
			else
			{
				c.iconTruckBubble.enabled = false;
				c.iconLocked.enabled = false;
				c.enabled = false;	
			}
		}
		
		
	}
	
	public void CheckAvailability()
	{
		if(!isAvailable && popUpRental.states[0].cinemas.Count == popUpRental.states[0].openedCinemas.Count)
		{
			iconBuyCinema.SetActive(false);
			iconBuyItem.SetActive(true);
			collider.enabled = true;
		}
		else if(!isAvailable)
		{
			iconBuyCinema.SetActive(false);
			iconBuyItem.SetActive(true);
			collider.enabled = false;
		}
		else if(isAvailable)
		{
			iconBuyCinema.SetActive(true);
			iconBuyItem.SetActive(false);
			collider.enabled = false;
		}
	}

	IEnumerator EachTimeToDo()
	{
		while(true)
		{	
			yield return new WaitForSeconds(0.1f);
			yield return 0;
		}
	}
	
	public void CinemaCountCheck()
	{
		if(cinemaCount > 0)
		{
			//Utils.ShowHideObject(iconBuyCinema, true);
			for(int i = 0; i < cinemas.Count; i++)
			{
				if(i < cinemaCount)
				{
					cinemas[i].gameObject.SetActive(true);
				}
				else
				{
					cinemas[i].gameObject.SetActive(false);
				}
			}
			text.text = "Theaters: " + cinemaCount + "/" + maxCinemaCount; 
			text.Commit();
		}
	}
	
	public void RefreshCinemaRentals(int money)
	{
		//money /= maxCinemaCount;
		for(int i = 0; i < openedCinemas.Count ; i++)
		{
			openedCinemas[i].money += money;
		}
	}
	
	
}