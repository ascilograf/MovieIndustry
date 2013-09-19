using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum SearchStaffPlaneState
{
	readyForSearch,
	searchInProgress,
	resumesAreReady,
}

//плашка поиска, отвечает за поиск персонажа, показ резюме
public class SearchStaffPlane : MonoBehaviour 
{		
	//объекты положений плашки поиска
	public GameObject readyForSearch;				//готово для поиска
	public GameObject searchInProgress;				//поиск в процессе
	public GameObject resumesAreReady;				//резюме готовы
	
	SearchStaffPlaneState state;					//состояние плашки
	
	public CharacterType type;						//тип персонажа которого будем искать
	
	//Меши разных состойний
	//readyForSearch state
	public tk2dTextMesh timeMesh;	
	public tk2dTextMesh priceMesh;
	
	//searchInProgress state
	public tk2dTextMesh countdownMesh;
	public tk2dTextMesh dollarsMesh;
	
	//all states
	public tk2dTextMesh captionButtonMesh;
	public tk2dTextMesh captionPlaneMesh;
	
	public int priceInCoins;						//цена за начало поиска
	public int priceInDollars;						//цена в долларах за окончание поиска
	public float searchTime;						//время поиска
				
	float tempTime;									//время для отсчета
	
	public PopUpHireStaff menu;						//меню найма персонажей
	public List<GameObject> searchedStaff;			//найденный персонал
	
	
	void Start()
	{
		SwitchState(SearchStaffPlaneState.readyForSearch);
	}
	
	//по активации меняем состояние плашки, активируем приемник
	void OnEnable () 
	{
		if(searchedStaff.Count > 0 || (searchTime) < Time.time && searchTime > 0)
		{
			SwitchState(SearchStaffPlaneState.resumesAreReady);
		}
		else
		{
			SwitchState(SearchStaffPlaneState.readyForSearch);
		}
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	//деактивируем приемник
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	//нажатия позеленой кнопке, в зависимости от текущего состояния она выполняет разные действия
	//также отслеживается нажатие по кнопке найма ,определяется персонаж которому она принадлежит и происходит найм
	void CheckTap(GameObject go)
	{
		if(go == readyForSearch)
		{
			if(GlobalVars.money >= priceInCoins)
			{
				GlobalVars.money -= priceInCoins;
				searchTime = Time.time + 60;
				SwitchState(SearchStaffPlaneState.searchInProgress);
				ClearSearchedStuff();
				menu.ShowCharInfos();
				//menu.InfosToStart();
			}
		}
		if(go == searchInProgress)
		{
			if(GlobalVars.stars >= priceInDollars)
			{
				GlobalVars.stars -= priceInDollars;
				SwitchState(SearchStaffPlaneState.resumesAreReady);
			}
		}
		if(go == resumesAreReady)
		{
			SwitchState(SearchStaffPlaneState.readyForSearch);
			if(searchedStaff.Count == 0)
			{
				menu.SetParamsAndStart(type, false);
			}
			else
			{
				PosCharInfos();	
			}
			menu.InfosToStart();
			menu.HideCharInfos();
			
		}
		if(go.name == "buttonHire")
		{
			if(searchedStaff.Count > 0)
			{
				foreach(GameObject g in searchedStaff)
				{
					CharInfo ci = g.GetComponent<CharInfo>();
					if(ci.buttonHireStaff == go)
					{
						ci.character.SetActive(true);
						ci.character.transform.localPosition = new Vector3(0, -150, GlobalVars.CHARACTER_FREE_LAYER);
						ci.bottomPrice.SetActive(true);
						searchedStaff.Remove(g);
						ci.gameObject.SetActive(false);
						ClearSearchedStuff();
						menu.ShowCharInfos();
						SwitchState(SearchStaffPlaneState.readyForSearch);
					}
					//PosCharInfos();
				}
			}
		}
	}
	
	//репозиционирование плашек, подтягивание к ним плашки поиска (используется при изменении списка плашек персонажей)
	void PosCharInfos()
	{
		for(int i = 0; i < searchedStaff.Count; i++)
		{
			searchedStaff[i].transform.localPosition = new Vector3(-220 + i * 420, 0, 0);
			searchedStaff[i].SetActive(true);
			searchedStaff[i].GetComponent<CharInfo>().Refresh();
			searchedStaff[i].GetComponent<CharInfo>().ShowButton();
		}
		transform.localPosition = new Vector3(-220 + searchedStaff.Count * 420, 0, 0);
		menu.InfosToStart();
		menu.SetMinMaxLength((searchedStaff.Count - 1) * -420, 0);
	}
	
	//делаем отсчет времени каждый кадр
	void Update () 
	{
		if(state == SearchStaffPlaneState.searchInProgress)
		{
			CheckTime();
		}
	}
	
	//изменение надписи
	void ChangeString(tk2dTextMesh mesh, string text)
	{
		mesh.text = text;
		mesh.Commit();
	}
	
	//проверка времени, каждую секунду изменяем надпись времени, проверяем, не вышло ли время отведенное на поиск резюме
	void CheckTime()
	{
		if((searchTime) < Time.time)
		{
			SwitchState(SearchStaffPlaneState.resumesAreReady);
			return;
		}
		if((Time.time - tempTime) >= 1)
		{
			tempTime = Time.time;
			ChangeString(countdownMesh, ((int)searchTime - (int)Time.time).ToString());
		}
	}
	
	//удалить всех найденных персонажей для нового поиска
	void ClearSearchedStuff()
	{
		if(searchedStaff.Count > 0)
		{
			foreach(GameObject g in searchedStaff)
			{
				Destroy(g.GetComponent<CharInfo>().character);
				Destroy(g);
			}
			searchedStaff.Clear();
			searchedStaff = new List<GameObject>();
		}
	}
	
	//сменить состояние на текущее
	public void SwitchToCuttent()
	{
		SwitchState(state);
	}
	
	//смена состояния плашки, в зависимости от состояния показываем/скрываем разные объекты, меняем текст меши
	void SwitchState(SearchStaffPlaneState s)
	{
		switch(s)
		{
		case SearchStaffPlaneState.readyForSearch:
			readyForSearch.SetActive(true);
			resumesAreReady.SetActive(false);
			searchInProgress.SetActive(false);
			searchTime = 0;
			ChangeString(timeMesh, (60).ToString());
			ChangeString(priceMesh, 10000 + "");
			//ChangeString(captionPlaneMesh, "Hire an " + type.ToString());
			state = s;
			break;
		case SearchStaffPlaneState.resumesAreReady:
			readyForSearch.SetActive(false);
			resumesAreReady.SetActive(true);
			searchInProgress.SetActive(false);
			ChangeString(dollarsMesh, 200 + "");
			//ChangeString(captionPlaneMesh, "Search is ended");
			
			state = s;
			break;
		case SearchStaffPlaneState.searchInProgress:
			readyForSearch.SetActive(false);
			resumesAreReady.SetActive(false);
			searchInProgress.SetActive(true);
			//ChangeString(captionButtonMesh, 200 + "");
			//ChangeString(captionPlaneMesh, "Search an actor");
			state = s;
			break;
		}
	}
}

