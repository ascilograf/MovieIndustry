using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PopUpStaffInventoryState
{
	nonActive,
	showInventory,
	buyNewItem,
}

//контроль попАп меню инвентаря персонажей
//выбор персонажа, покупка новых слотов инвентаря, покупка предметов
public class PopUpStaffInventory : MonoBehaviour 
{
	//кнопки выбора персонажей
	public GameObject buttonClose;
	public GameObject buttonActors;
	public GameObject buttonDirectors;
	public GameObject buttonCameramans;
	public GameObject buttonScripters;
	public GameObject buttonPostProds;
	public GameObject buttonBuyNewItem;
	
	//группы объектов меню
	public GameObject buttons;
	public GameObject menu;
	public GameObject buyNewItem;
	
	public Camera swipeCamera;					//камера свайпа
	
	PopUpStaffInventoryState state;				//состояние меню
	
	public List<FilmStaff> staff = new List<FilmStaff>();		//список выбранных персонажей
	SwipeItems swipe;
	Vector3 tapDown;
	
	void Awake()
	{
		//swipe = GetComponent<SwipeItems>();
	}
	
	void Start () 
	{
		//SwitchState(PopUpStaffInventoryState.nonActive);
	}
	
	/*void Update () 
	{
		/*if(Input.GetMouseButtonDown(0))
		{
			tapDown = Input.mousePosition;
		}
		if(Input.GetMouseButtonUp(0) && Vector3.Distance(tapDown, Input.mousePosition) < 20 && state == PopUpStaffInventoryState.showInventory)
		{
			RaycastHit hit;
			int layer = 1 << 9;
			Ray ray = GlobalVars.GUICamera.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
			{
				//обработка основных кнопок меню
				if(hit.collider == buttonClose.collider)
				{
					gameObject.SetActiveRecursively(false);
					StaffManagment.CharInfoToParent(staff);
					GlobalVars.cameraStates = CameraStates.normal;
					return;
				}
				else if(hit.collider == buttonActors.collider)
				{
					RefreshListOf("actor");
				}
				else if(hit.collider == buttonCameramans.collider)
				{
					RefreshListOf("cameraman");
				} 
				else if(hit.collider == buttonDirectors.collider)
				{
					RefreshListOf("director");
				}
				else if(hit.collider == buttonPostProds.collider)
				{
					RefreshListOf("postProdWorker");
				} 
				else if(hit.collider == buttonScripters.collider)
				{
					RefreshListOf("scripter");
				} 
				
			}
			
			ray = swipeCamera.ScreenPointToRay(Input.mousePosition);
			layer = 1 << 10;
			//обработка нажатий по инвентарю персонажей.
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
			{
				StaffInventory inventory = staff[swipe.itemIndex].GetComponent<StaffInventory>();
				for(int i = 0; i < inventory.items.Count; i++)
				{
					if(hit.collider == inventory.items[i].button && inventory.items[i].isSlotOpen
						&& inventory.items[i].item == null)
					{
						StartCoroutine(BuyNewItem(i));
						SwitchState(PopUpStaffInventoryState.buyNewItem);
					}
					else if(hit.collider == inventory.items[i].button && !inventory.items[i].isSlotOpen && GlobalVars.stars >= 25)
					{
						GlobalVars.stars -= 25;
						inventory.items[i].isSlotOpen = true;
					}
				}
				print (hit.collider.name);
			}
			
			
		}
		if(Input.GetMouseButtonUp(0))
		{
			swipe.CheckItem();
		}
		GlobalVars.cameraStates = CameraStates.menu;
	}
	
	//корутина покупки нового предмета для персонажа, на вход индекс слота в инвентаре
	IEnumerator BuyNewItem(int index)
	{
		while(true)
		{
			if(Input.GetMouseButtonDown(0) && state == PopUpStaffInventoryState.buyNewItem)
			{
				
				RaycastHit hit;
				int layer = 1 << 10;
				Ray ray = swipeCamera.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
				{
					StaffItemButton button = hit.collider.GetComponent<StaffItemButton>();
					//если мы нажали в кнопку
					if(button != null)
					{
						StaffInventory inventory = staff[swipe.itemIndex].GetComponent<StaffInventory>();
						BonusParams prices = new BonusParams();
						
						//проверка бонусных параметров
						foreach(BonusParams par in GlobalVars.staffInventoryParams)
						{
							if(par.type == button.item.GetComponent<StaffInventoryItem>().bonuses.type)
							{
								prices = par;
							}
						}
						
						//покупка премиумного предмета
						if(prices.priceStars != 0 && prices.priceStars <= GlobalVars.stars)
						{
							GlobalVars.stars -= prices.priceStars;
							GameObject go = Instantiate(button.item) as GameObject;
							go.transform.parent = inventory.items[index].button.transform;
							go.transform.localPosition = Vector3.zero;
							inventory.items[index].item = go.GetComponent<StaffInventoryItem>();
						}
						
						//покупка обычного предмета
						else if(prices.priceMoney <= GlobalVars.money)
						{
							GlobalVars.money -= prices.priceMoney;
							print (button.item.GetComponent<StaffInventoryItem>().bonuses.priceMoney);
							GameObject go = Instantiate(button.item) as GameObject;
							go.transform.parent = inventory.items[index].button.transform;
							go.transform.localPosition = Vector3.zero;
							inventory.items[index].item = go.GetComponent<StaffInventoryItem>();
						}
						staff[swipe.itemIndex].RefreshStats();
					}
				}
				yield return new WaitForSeconds(Time.deltaTime * 3);
				SwitchState(PopUpStaffInventoryState.showInventory);
				yield break;
			}
			yield return 0;
		}
	}
	
	//обновление списка персонажей
	//обнуляем список ,потом по тэгу заполняем его по-новой.
	void RefreshListOf(string tag)
	{
		StaffManagment.CharInfoToParent(staff);
		staff.Clear();
		swipe.items.Clear();
		swipe.InfosToStart();
		swipe.itemIndex = 0;
		GameObject[] go = GameObject.FindGameObjectsWithTag(tag);
		foreach(GameObject g in go)
		{
			staff.Add(g.GetComponent<FilmStaff>());
		}
		for(int i = 0; i < staff.Count; i++)
		{
			swipe.items.Add(staff[i].gameObject);
			staff[i].charInfoState = CharInfoState.ShowForInventory;
			staff[i].icon.transform.parent = swipe.infos.transform;
			staff[i].icon.transform.localScale = Vector3.one;
			staff[i].icon.transform.localPosition = new Vector3(0 + i * swipe.indent, -27, -10);
		}
	}
	
	//смена стейтов меню, скрытие/показ элементов меню в зависимости от стейта
	public void SwitchState(PopUpStaffInventoryState s)
	{
		switch(s)
		{
		case PopUpStaffInventoryState.buyNewItem:
			buyNewItem.SetActiveRecursively(true);
			state = PopUpStaffInventoryState.buyNewItem;
			break;
		case PopUpStaffInventoryState.nonActive:
			GlobalVars.cameraStates = CameraStates.normal;
			gameObject.SetActiveRecursively(false);
			state = PopUpStaffInventoryState.nonActive;
			break;
		case PopUpStaffInventoryState.showInventory:
			GlobalVars.cameraStates = CameraStates.menu;
			buttons.SetActiveRecursively(true);
			buyNewItem.SetActiveRecursively(false);
			state = PopUpStaffInventoryState.showInventory;
			break;
		}
	}*/
}
