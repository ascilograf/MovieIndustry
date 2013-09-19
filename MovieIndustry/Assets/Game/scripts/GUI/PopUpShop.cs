using UnityEngine;
using System.Collections;

enum PopUpShopStates
{
	notActive,
	build,
	decorations,
	items,
	money,
}

//контроллер магазина
//при запуске активируется таб строительства, остальные деактивируются
//при нажатии на таб происходит деактивация всех табов и повторная активация выбранного
//отслеживание нажатий по элементам таба, при попадании на кнопку построить от PopUpShopElement - происходит инстанс превью здания с кнопками
//и началом выбора места

public class PopUpShop : MonoBehaviour {
	
	public GameObject menu;
	
	public GameObject buttonClose;
	
	public GameObject subMenuBuild;
	public GameObject tabBuild;
	public GameObject subMenuDecorations;
	public GameObject tabDecorations;
	public GameObject subMenuItems;
	public GameObject tabItems;
	public GameObject subMenuMoney;
	public GameObject tabMoney;
	
	public GameObject background;
	
	public GameObject placeBuilding;
	
	GameObject tempIcon;
	//GameObject tempButtons;
	//PopUpShopStates menuState;	
	
	void Start () 
	{ 
	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTapOnGUI);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTapOnGUI);
	}
	
	void CheckTapOnGUI(GameObject go)
	{
		if( go == tabBuild)
		{
			SwitchState(PopUpShopStates.build);
		}
		else if(go == tabDecorations)
		{
			SwitchState(PopUpShopStates.decorations);
		}
		else if(go == tabItems)
		{
			SwitchState(PopUpShopStates.items);
		}
		else if(go == tabMoney)
		{
			SwitchState(PopUpShopStates.money);
		}
		else if(go == buttonClose)
		{
			GlobalVars.cameraStates = CameraStates.normal;
			SwitchState(PopUpShopStates.notActive);
		}
		if(go.GetComponent<PopUpShopElement>())
		{
			ShopItemParams par = go.GetComponent<PopUpShopElement>().shopParams;
			if(GlobalVars.money >= par.price)
			{
				GlobalVars.cameraStates = CameraStates.normal;
				GlobalVars.money -= par.price;
				StartNewConstruct(go.GetComponent<PopUpShopElement>().buildingType, par.time, par.workersCount, par.price);
			}
		}
		print (go.name);
	}
	
	void StartNewConstruct(BuildingType type, int time, int workers, int price)
	{
		GameObject go = Instantiate(placeBuilding, Vector3.zero, Quaternion.identity) as GameObject;
		go.GetComponent<PlaceBuilding>().SetParams(type, time, workers, price);
		SwitchState(PopUpShopStates.notActive);
	}
	
	void DeactivetaAllMenuExcept(GameObject menu)
	{
		subMenuBuild.SetActive(true);
		tabBuild.SetActive(true);
		
		subMenuDecorations.SetActive(true);
		tabDecorations.SetActive(true);
		
		subMenuItems.SetActive(true);
		tabItems.SetActive(true);
		
		subMenuMoney.SetActive(true);
		tabMoney.SetActive(true);
		
		if(menu.transform.FindChild("buttons") != null)
		{
			subMenuBuild.transform.FindChild("buttons").gameObject.SetActive(true);
			//tempButtons = menu.transform.FindChild("buttons").gameObject;	
		}
		menu.SetActive(true);
		background.SetActive(true);
	}
	
	public void SwitchStateToActive()
	{
		SwitchState(PopUpShopStates.build);
	}
	
	void SwitchButton(GameObject go)
	{
		if(tempIcon != null)
		{
			tempIcon.transform.FindChild("tabActive").GetComponent<MeshRenderer>().enabled = false;
			tempIcon.transform.FindChild("icon").localScale = Vector3.one;
		}
		tempIcon = go;
		tempIcon.transform.FindChild("icon").localScale = Vector3.one * 1.2f;
		tempIcon.transform.FindChild("tabActive").GetComponent<MeshRenderer>().enabled = true;
	}
	
	void SwitchState(PopUpShopStates state)
	{
		switch(state)
		{
		case PopUpShopStates.build:
			DeactivetaAllMenuExcept(subMenuBuild);
			gameObject.SetActive(true);
			SwitchButton(tabBuild);
			
			break;
		case PopUpShopStates.decorations:
			DeactivetaAllMenuExcept(subMenuDecorations);
			SwitchButton(tabDecorations);
			break;
		case PopUpShopStates.items:
			DeactivetaAllMenuExcept(subMenuItems);
			SwitchButton(tabItems);
			break;
		case PopUpShopStates.money:
			DeactivetaAllMenuExcept(subMenuMoney);
			SwitchButton(tabMoney);
			break;
		case PopUpShopStates.notActive:
			gameObject.SetActive(false);
			break;
		}
		//menuState = state;
	}
}
