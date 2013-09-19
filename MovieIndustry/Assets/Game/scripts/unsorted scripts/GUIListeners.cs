using UnityEngine;
using System.Collections;


//Скрипт для "прослушки" сообщений-команд от GUI-элементов нажатия которых отслеживается
//в скрипте MenuController, открытие/закрытие меню, наём рабочих, инициализация зданий.
public class GUIListeners : MonoBehaviour 
{
	
	//добавляем слушателей
	void Start () 
	{
		Messenger<CharacterType>.AddListener("Employ person", InstPerson);	
		Messenger<bool>.AddListener("Activate Hire Menu", ShowHireMenu);
		Messenger<bool>.AddListener("Activate Build Menu", ShowBuildMenu);
		Messenger<BuildingType>.AddListener("Activate Build Submenu", ShowBuildSubmsenu);
		//Messenger<BuildingType>.AddListener("Build New", BuildNewOffice);
		//Messenger<BuildingType, int>.AddListener("Build New", BuildOffice);
		Messenger<bool>.AddListener("Close Menu", ShowCurrMenu);
		//Messenger<GameObject>.AddListener("Construct", Construct);
	}
	
	//Инициализация нового нанятого персонажа
	void InstPerson(CharacterType type)
	{
		for(int i = 0; i < GlobalVars.characters.Length; i++)
		{
			if(type == GlobalVars.characters[i].personType)
			{
				if(GlobalVars.characters[i].priceMoney < GlobalVars.money)
				{
					GlobalVars.money -= GlobalVars.characters[i].priceMoney;
					Instantiate(GlobalVars.characters[i].parentObj);
					ShowHireMenu(false);
					GlobalVars.expGain.gainForHireStaff(GlobalVars.characters[i].priceMoney);
					print (GlobalVars.characters[i].personType);
				}
			}
		}
		GlobalVars.currMenu = null;
	}
	
	//Показать/скрыть меню наёма персонажей
	void ShowHireMenu(bool b)
	{
		Transform[] tr = GlobalVars.hireMenu.GetComponentsInChildren<Transform>(true);
		for(int i = 0; i < tr.Length; i++)
		{
			tr[i].gameObject.SetActive(b);
			
		}
		if(b)
		{
			GlobalVars.currMenu = GlobalVars.hireMenu;
		}
		else
		{
			GlobalVars.cameraStates = CameraStates.normal;
		}
	}
	
	//Показать/скрыть меню строительства
	void ShowBuildMenu(bool b)
	{
		Transform[] tr = GlobalVars.buildMenu.GetComponentsInChildren<Transform>(true);
		for(int i = 0; i < tr.Length; i++)
		{
			tr[i].gameObject.SetActive(b);
		}
		if(b)
		{
			GlobalVars.currMenu = GlobalVars.buildMenu;
		}
		else
		{
			GlobalVars.cameraStates = CameraStates.normal;
		}
	}
	
	//Показать/скрыть выбранное подменю строительства
	void ShowBuildSubmsenu(BuildingType bt)
	{
		ShowCurrMenu(false);
	}
	
	//Показать/скрыть текущее меню
	void ShowCurrMenu(bool b)
	{	
		
			Transform[] tr = GlobalVars.currMenu.GetComponentsInChildren<Transform>(true);
			for(int i = 0; i < tr.Length; i++)
			{
				tr[i].gameObject.SetActive(b);
			}
			if(!b)
			{
				GlobalVars.cameraStates = CameraStates.normal;
				GlobalVars.currMenu = null;
			}
	} 
	
	//Построить новый офис по типу строения
	/*void BuildNewOffice(BuildingType type)
	{
		for(int i = 0; i < GlobalVars.buildings.Length; i++)
		{
			if(type == GlobalVars.buildings[i].type)
			{
				if(GlobalVars.buildings[i].build.buildPriceMoney < GlobalVars.money)
				{
					GlobalVars.money -= GlobalVars.buildings[i].build.buildPriceMoney;
					GameObject go = Instantiate(GlobalVars.buildings[i].build.parentPrefab) as GameObject;
					ShowHireMenu(false);
					ShowCurrMenu(false);
					return;
				}
			}
		}
	}*/
	
	//Построить новый офис по типу строения и уровню офиса
	
	
	//Построить здание
	/*void Construct(GameObject go2)
	{
		GameObject go = Instantiate(go2) as GameObject;
		ShowHireMenu(false);
		ShowCurrMenu(false);
	}*/
}
