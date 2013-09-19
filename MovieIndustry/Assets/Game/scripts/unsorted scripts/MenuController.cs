using UnityEngine;
using System.Collections;


//Этот скрипт контролирует ГУИ-элементы, нажатия по ним
//Отправляет сообщения в GUIListener
public class MenuController : MonoBehaviour 
{
	
	BuildingType buildingType;
	int buildingIndex;
	public GameObject buttons;
	
	public GameObject buttonClose;
	public GameObject buttonEmployStaff;
	public GameObject buttonEmployPremiumStaff;
	public GameObject menuEmployStaff;
	public GameObject buttonBuild;
	public GameObject menuBuild;
	public GameObject buttonInventory;
	public GameObject menuInventory;
	public GameObject buttonUSAMap;
	public MovieRental menuUSAMap;
	public GameObject buttonStaffInventory;
	public GameObject menuStaffInventory;
	public GameObject buttonAchievments;
	public GameObject menuAchievments;
	public GameObject buttonCraftDecorations;
	public GameObject menuCraftDecorations;
	
	public GameObject dollarsPlus;
	public GameObject coinsPlus;
	
	public tk2dTextMesh meshCoins;
	public tk2dTextMesh meshDollars;
	public GameObject shadow;
	
	int tempCoins;
	int tempDollars;
	
	void Start()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject go)
	{
		//если какое-нибудь меню уже не открыто
		if(GlobalVars.cameraStates == CameraStates.normal)//GlobalVars.currMenu == null && !GlobalVars.inventory.showInventory)
		{
			//Отправить сообщение сделать активным меню наёма персонажей
			if(go == buttonEmployStaff)
			{
				//print ("hire");
				GlobalVars.popUpHireStaff.ShowMenu("actor");
				//Messenger<bool>.Broadcast("Activate Hire Menu", true);
				
				//GlobalVars.currMenu = menuEmployStaff;
			}
			
			else if(go == buttonEmployPremiumStaff)
			{
				GlobalVars.popUpHireStaff.gameObject.SetActive(true);
				GlobalVars.popUpHireStaff.SetParamsAndStart(CharacterType.actor, true);
				GlobalVars.cameraStates = CameraStates.menu;
			}
			
			//Отправить сообщение сделать активным меню строительства
			else if(go == buttonBuild)
			{
				menuBuild.GetComponent<PopUpShop>().SwitchStateToActive();
				GlobalVars.cameraStates = CameraStates.menu;
				GlobalVars.currMenu = menuBuild;
			}
			
			else if(go == buttonCraftDecorations)
			{
				menuCraftDecorations.SetActive(true);
				menuCraftDecorations.GetComponent<PopUpCraftDecorations>().ActivateAdvTab();
				GlobalVars.cameraStates = CameraStates.menu;
				GlobalVars.currMenu = menuBuild;
			}
		
			//открыть инвентарь
			else if(go == buttonInventory)
			{
				GlobalVars.inventory.showInventory = true;
				GlobalVars.cameraStates = CameraStates.menu;
				//GlobalVars.currMenu = inv;
			}
			
			else if(go == buttonUSAMap)
			{
				menuUSAMap.ShowMenu();
				GlobalVars.cameraStates = CameraStates.menu;
				//GlobalVars.currMenu = menuUSAMap;
			}
			
			else if(go == buttonStaffInventory)
			{
				menuStaffInventory.SetActive(true);
				//menuStaffInventory.GetComponent<PopUpStaffInventory>().SwitchState(PopUpStaffInventoryState.showInventory);
				GlobalVars.cameraStates = CameraStates.menu;
				GlobalVars.currMenu = menuStaffInventory;
			}
			else if(go == buttonAchievments)
			{
				menuAchievments.SetActive(true);
				GlobalVars.cameraStates = CameraStates.menu;
				GlobalVars.currMenu = menuAchievments;
			}
			else if(go == coinsPlus)
			{
				GlobalVars.money += 100000;
			}
			else if(go == dollarsPlus)
			{
				GlobalVars.stars += 1000;
			}
		}
	}
	
	//Кастуем луч из камеры по коллайдерам в меню, если есть попадание
	//то отправляем то или иное сообщениt 
	void Update () 
	{
		CheckMoneyChange();
		
		if(GlobalVars.cameraStates == CameraStates.normal)
		{
			collider.enabled = false;
		}
		else
		{
			collider.enabled = true;
		}
		
		if(GlobalVars.cameraStates == CameraStates.menu)
		{
			shadow.SetActive(true);
		}
		else if(GlobalVars.cameraStates == CameraStates.normal)
		{
			shadow.SetActive(false);
		}
	}
	
	void CheckMoneyChange()
	{
		if(GlobalVars.money != tempCoins)
		{
			tempCoins = (int)GlobalVars.money;
			meshCoins.text = Utils.ToNumberWithSpaces(tempCoins.ToString());
		}
		if(GlobalVars.stars != tempDollars)
		{
			tempDollars = GlobalVars.stars;
			meshDollars.text = Utils.ToNumberWithSpaces(tempDollars.ToString());
		} 
	}
}
