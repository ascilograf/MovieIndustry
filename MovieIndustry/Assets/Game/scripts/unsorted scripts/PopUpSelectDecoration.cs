using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum PopUpSelectDecorationState
{
	notActive,
	selectSetting,
	selectRarity,
}

//выбор декораций, поочередно предоставляет выбор сначала сеттинга, потом грейда декорации
//если есть выбранная декорация - происходит инициализация декорации

public class PopUpSelectDecoration : MonoBehaviour 
{
	//кнопки и блоки кнопок
	public GameObject blockSettings;				
	public GameObject blockRarity;
	public GameObject buttonClose;
	public GameObject buttonBack;
	public GameObject buttonNextStep;
	public GetTextFromFile headerText;
	public GetTextFromFile buttonText;
	
	
	FilmMaking film;									//текущее задание
	PopUpSelectDecorationState state;					//текущее состояние меню
	Setting setting = Setting.none;						//текущий сеттинг
	RarityLevel rarity = RarityLevel.none;				//текущий грейд
	public List<DecorationItem> decorWithCurrSetting = new List<DecorationItem>();				//список декораций по сеттингу
	public List<DecorationItem> decorWithCurrRarityLevel = new List<DecorationItem>();			//список декораций в сеттинге по грейду
	float fit;											//фит
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject go)
	{
		if(state == PopUpSelectDecorationState.notActive)
		{
			return;
		}
		else if(state == PopUpSelectDecorationState.selectSetting)
		{
			SettingButton s = go.GetComponent<SettingButton>();
			if( s != null)
			{
				if(CheckAvailableSetting(s.setting))
				{
					SelectSetting(s);
				}
			}
		}
		else if(go == buttonBack)
		{
			SwitchState(PopUpSelectDecorationState.selectSetting);
		}
		else if(state == PopUpSelectDecorationState.selectRarity)
		{
			if(go.name == "common" || go.name == "rare" || go.name == "unique")
			{
				SelectRarity(go);
			}
		}
		if(go == buttonClose)
		{			
			film.boostIcon.SetActive(true);
			SwitchState(PopUpSelectDecorationState.notActive);
			ClearParams();
		}
		else if(go == buttonNextStep)
		{
			if(setting == Setting.none)
			{
				SwitchState(PopUpSelectDecorationState.selectSetting);
			}
			else if(rarity == RarityLevel.none)
			{
				SwitchState(PopUpSelectDecorationState.selectRarity);
			}
			else
			{
				StartBuildDecor();	
			}
		}
		else if(go.name == "craftDecor")
		{
			film.boostIcon.SetActive(true); 
			ClearParams();
			SwitchState(PopUpSelectDecorationState.notActive);
			GlobalVars.popUpCraftDecorations.gameObject.SetActive(true);
			GlobalVars.popUpCraftDecorations.ActivateAdvTab();
			GlobalVars.cameraStates = CameraStates.menu;
		}
	}
	
	//есть ли доступные декорации в этом сеттинге
	bool CheckAvailableSetting(Setting s)
	{
		foreach(InventoryItem item in GlobalVars.inventory.items)
		{
			if(item.GetComponent<DecorationItem>() != null)
			{
				if(item.GetComponent<DecorationItem>().setting == s)
				{
					return true;
				}
			}
		}
		return false;
	}
	
	void FillDecorationsWothSetting(Setting s)
	{
		decorWithCurrSetting.Clear();
		foreach(InventoryItem item in GlobalVars.inventory.items)
		{
			if(item.GetComponent<DecorationItem>() != null)
			{
				if(item.GetComponent<DecorationItem>().setting == s)
				{
					decorWithCurrSetting.Add(item.GetComponent<DecorationItem>());
				}
			}
		}
	}
	
	//есть ли доступные декорации в выбранном сеттинге с этим грейдом
	bool CheckCountItemsWithRarity(RarityLevel r)
	{
		foreach(DecorationItem item in decorWithCurrSetting)
		{
			if(item.rarity == r)
			{
				return true;//decorWithCurrRarityLevel.Add(item.GetComponent<DecorationItem>());
			}
		}
		return false;
	}
	
	void FillDecorationsWithRarity(RarityLevel r)
	{
		decorWithCurrRarityLevel.Clear();
		decorWithCurrRarityLevel = new List<DecorationItem>();
		foreach(DecorationItem item in decorWithCurrSetting)
		{
			if(item.rarity == r)
			{
				decorWithCurrRarityLevel.Add(item.GetComponent<DecorationItem>());
			}
		}
	}
	
	//создание декорации
	void StartBuildDecor()
	{
		//film.ActivateBuild(setting, fit, rarity);
		GameObject icon = Instantiate(GlobalVars.decorIconPrefab) as GameObject;
		icon.transform.parent = film.transform;
		icon.GetComponent<DecorationIcon>().SetIcon(setting, rarity, fit);
		icon.SetActive(false);
		film.usedDecorations.Add(icon);
		decorWithCurrRarityLevel[0].maintenance -= 1;
		if(decorWithCurrRarityLevel[0].maintenance == 0)
		{
			GlobalVars.inventory.items.Remove(decorWithCurrRarityLevel[0].GetComponent<InventoryItem>());
			Destroy(decorWithCurrRarityLevel[0]);
		}
		ClearParams();
		Messenger.Broadcast("Check workers count");
	}
	
	//очистка параметров
	void ClearParams()
	{
		fit = 0;
		rarity = RarityLevel.none;
		setting = Setting.none;
		SwitchState(PopUpSelectDecorationState.notActive);
		film = null;
		SettingButton[] allButtons = gameObject.GetComponentsInChildren<SettingButton>(true);
		foreach(SettingButton sb in allButtons)
		{
			sb.SetChoosen(false);
		}
		decorWithCurrRarityLevel.Clear();
		decorWithCurrSetting.Clear();
	}
	
	//установка первоначальных параметров, активация меню
	public void SetParams(FilmMaking filmMaking)
	{
		SwitchState(PopUpSelectDecorationState.selectSetting);
		film = filmMaking;
	}
	
	//обработка кнопок сеттинга
	void SelectSetting(SettingButton button)
	{
		SettingButton[] allButtons = blockSettings.GetComponentsInChildren<SettingButton>();
		foreach(SettingButton sb in allButtons)
		{
			sb.SetChoosen(false);
		}
		if(setting == Setting.none)
		{
			setting = button.setting;
			button.SetChoosen(true);
			decorWithCurrSetting.Clear();
			FillDecorationsWothSetting(button.setting);
		}
		else
		{
			setting = Setting.none;
		}
	}
	
	//обработка кнопок грейда
	void SelectRarity(GameObject button)
	{
		SettingButton[] allButtons = blockRarity.GetComponentsInChildren<SettingButton>();
		foreach(SettingButton sb in allButtons)
		{
			sb.SetChoosen(false);
		}
		if(rarity == RarityLevel.none)
		{		
			if(button.name == "common" && CheckCountItemsWithRarity(RarityLevel.common))
			{
				FillDecorationsWithRarity(RarityLevel.common);
				button.GetComponent<SettingButton>().SetChoosen(true);
				rarity = RarityLevel.common;
			}
			else if(button.name == "common" && !CheckCountItemsWithRarity(RarityLevel.common))
			{
				button.GetComponent<SettingButton>().SetChoosen(false);
			}
			if(button.name == "rare" && CheckCountItemsWithRarity(RarityLevel.rare))
			{
				FillDecorationsWithRarity(RarityLevel.rare);
				button.GetComponent<SettingButton>().SetChoosen(true);
				rarity = RarityLevel.rare;
			}
			else if(button.name == "rare" && !CheckCountItemsWithRarity(RarityLevel.rare))
			{
				button.GetComponent<SettingButton>().SetChoosen(false);
			}
			if(button.name == "unique" && CheckCountItemsWithRarity(RarityLevel.unique))
			{
				FillDecorationsWithRarity(RarityLevel.unique);
				button.GetComponent<SettingButton>().SetChoosen(true);
				rarity = RarityLevel.unique;
			}
			else if(button.name == "unique" && !CheckCountItemsWithRarity(RarityLevel.unique))
			{
				button.GetComponent<SettingButton>().SetChoosen(false);
			}
			CheckFit();
		}
		else
		{
			rarity = RarityLevel.none;
		}
	}
	
	int GetMaintenanceOf(RarityLevel r)
	{
		foreach(DecorationItem d in decorWithCurrSetting)
		{
			if(d.rarity == r && d.maintenance > 0)
			{
				return d.maintenance;
			}
		}
		return 0;
	}
	
	void CheckRarity()
	{
		SettingButton[] allButtons = blockRarity.GetComponentsInChildren<SettingButton>();
		foreach(SettingButton sb in allButtons)
		{
			sb.SetChoosen(false);
			if(sb.name == "common" && CheckCountItemsWithRarity(RarityLevel.common))
			{
				print ("common");
				FillDecorationsWithRarity(RarityLevel.common);
				sb.SetDecoration(decorWithCurrRarityLevel[0]);
				sb.count.text = "Count: " + decorWithCurrRarityLevel.Count.ToString();
				sb.count.Commit();
			}
			else if(sb.name == "rare" && CheckCountItemsWithRarity(RarityLevel.rare))
			{
				print ("rare");
				FillDecorationsWithRarity(RarityLevel.rare);
				sb.SetDecoration(decorWithCurrRarityLevel[0]);
				sb.count.text = "Count: " + decorWithCurrRarityLevel.Count.ToString();
				sb.count.Commit();
			}
			else if(sb.name == "unique" && CheckCountItemsWithRarity(RarityLevel.unique))
			{
				print ("unique");
				FillDecorationsWithRarity(RarityLevel.unique);
				sb.SetDecoration(decorWithCurrRarityLevel[0]);
				sb.count.text = "Count: " + decorWithCurrRarityLevel.Count.ToString();
				sb.count.Commit();
			}
			else
			{
				print ("none");
				sb.SetDecoration();
			}
		}
	}
	
	//определение фита
	void CheckFit()
	{
		foreach(FilmGenres fg in film.script.genres)
		{
			for(int i = 0; i < GlobalVars.fits.Length; i++)
			{
				if(GlobalVars.fits[i].genre == fg)
				{
					if(GlobalVars.fits[i].perfectFit.Exists(delegate(Setting g)
					{ return g == setting;}))
					{
						fit++;	
					}
					else if(GlobalVars.fits[i].badFit.Exists(delegate(Setting g)
					{ return g == setting;}))
					{
						fit--;	
					}
				}
			}
		}
		
		if(film.script.genres.Count == 1)
		{
			if(fit == 1)
			{
				fit = (int)rarity * 0.15f + 0.45f;
			}
			else if(fit < 1)
			{
				fit = (int)rarity * 0.15f + 0.3f;
			}
		}
		else if(film.script.genres.Count >= 1)
		{
			if(fit == 2)
			{
				fit = (int)rarity * 0.15f + 0.45f;
			}
			else if(fit < 2)
			{
				fit = (int)rarity * 0.15f + 0.3f;
			}
		}
	}
	
	//смена состояний
	void SwitchState(PopUpSelectDecorationState newState)
	{
		switch(newState)
		{
		case PopUpSelectDecorationState.notActive:
			gameObject.SetActive(false);
			state = PopUpSelectDecorationState.notActive;
			GlobalVars.cameraStates = CameraStates.normal;
			break;
			
		case PopUpSelectDecorationState.selectRarity:
			blockRarity.SetActive(true);
			state = PopUpSelectDecorationState.selectRarity;
			buttonBack.SetActive(true);
			
			CheckRarity();
			blockSettings.SetActive(false);
			buttonText.SetTextWithIndex(1);
			headerText.SetTextWithIndex(1);
			break;
			
		case PopUpSelectDecorationState.selectSetting:
			state = PopUpSelectDecorationState.selectSetting;
			gameObject.SetActive(true);
			blockSettings.SetActive(true);
			blockRarity.SetActive(false);
			buttonBack.SetActive(false);
			GlobalVars.cameraStates = CameraStates.menu;
			buttonText.SetTextWithIndex(0);
			headerText.SetTextWithIndex(0);
			decorWithCurrSetting.Clear();
			setting = Setting.none;
			rarity = RarityLevel.none;
			break;
		}
	}
}
