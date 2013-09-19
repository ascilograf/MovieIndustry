using UnityEngine;
using System.Collections;

//контроллер меню покупки части карты/кинотеатра
//есть 2 метода, для части карты и для кинотеатров, в нем выставляются параметры и подготавливается покупка
//если покупаем кинотеатр, то еще дублируется иконка города и выставляется в заготовленное место

public class BuyNewState : MonoBehaviour 
{
	public GameObject buttonAccept;
	public GameObject buttonClose;
	public Transform iconPlaceholder;
	public tk2dTextMesh cityNameMesh;
	public tk2dTextMesh text2;
	public tk2dTextMesh text;
	public MovieRental rental = null;
	public Cinema cin = null;
	public MovieRentalItem item = null;
	int price;
	GameObject t;
	
	void Start()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject go)
	{
		if(go == buttonAccept)
		{
			GlobalVars.soundController.BuyNewCity();
			if(rental == null)
			{
				BuyCinema();
			}
			else if(cin == null)
			{
				BuyState();
			}
		}
		if(go == buttonClose)
		{
			CloseMenu();
		}
	}
	
	public void SetForBuyState(MovieRental rental,MovieRentalItem item, int price, string regionName)
	{
		cityNameMesh.text = regionName;
		cityNameMesh.Commit();
		this.rental = rental;
		this.item = item;
		this.price = price;
		text2.text = "Upgrade distribution network for";
		text2.Commit();
		Utils.ShowHideObject(this.gameObject, true);
		text.text = price.ToString();
		if(GlobalVars.money < price)
		{
			text.color = Color.red;
		}
		text.Commit();
		GlobalVars.worldRental.chart.cam.enabled = false;
	}
	
	void BuyState()
	{
		if(GlobalVars.money >= price)
		{
			GlobalVars.money -= price;
			item.isAvailable = true;
			rental.states.Add(item);
			rental.ShowMenu();
			GlobalVars.expGain.gainForNewState();
			CloseMenu();
		}
	}
	
	public void SetForBuyCinema(MovieRentalItem item, Cinema cinema, int price, string n, GameObject theater)
	{
		t = Instantiate(theater) as GameObject;
		t.GetComponent<MeshRenderer>().enabled = true;
		t.transform.parent = iconPlaceholder;
		t.transform.localPosition = Vector2.zero;
		cityNameMesh.text = n;
		cityNameMesh.Commit();
		this.price = price;
		text2.text = "Purchase new cinema for";
		this.price = price;
		cin = cinema;
		this.item = item;
		text2.Commit();
		Utils.ShowHideObject(this.gameObject, true);
		text.text = price.ToString();
		if(GlobalVars.money < price)
		{
			text.color = Color.red;
		}
		text.Commit();
		GlobalVars.worldRental.chart.cam.enabled = false;
		//StartCoroutine(BuyCinemaCoroutine(item, price));
	}
	
	void BuyCinema()
	{
		if(GlobalVars.money >= price)
		{
			Utils.ChangeMoneyBalance(-price);
			cin.SetTime(Time.time);
			item.openedCinemas.Add(cin);
			item.ShowOpenedCinemas();
			GlobalVars.expGain.gainForBuildNewCinema(price);
			cin.iconGetMoney.gameObject.SetActive (false);
			CloseMenu();
			GlobalVars.worldRental.CheckAvailability();
		}
	}
	
	void CloseMenu()
	{
		Destroy (t);
		gameObject.SetActive(false);
		text.text = "";
		text.color = Color.white;
		text.Commit();
		rental = null;
		cin = null;
		price = 0;
		item = null;
		GlobalVars.worldRental.chart.cam.enabled = true;
	}
}
