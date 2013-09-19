using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//возможные состояния этого меню
public enum PopUpNewScriptState
{
	notActive,
	selectGenres,
	selectScripters,
	selectSettings,
}

//скрипт полностью контролит меню выбора персонажей, жанров и сеттинга для написания сценария
//на вход получается переменная этажаОфиса, поочередно происходит выбор жанра, сценаристов
//сеттинга для нового сценария, за показ одних частей меню и скрытие других отвечает функция 
//SwitchState, на нее также опираются некоторые другие функции.
public class popUpNewScript : MonoBehaviour 
{
	public GameObject partBackground;				//бэкграунд
	public GameObject partGenres;					//жанры
	public GameObject partSettings;					//сеттинги
	public GameObject partInfos;					//чарИнфо
	public GameObject hireScripterPlane;
	public GameObject hireScripterButton;
	public tk2dTextMesh hireScripterPriceMesh;
	public GetTextFromFile headerText; 
	public tk2dUIScrollableArea staffScrollArea;
	
	public GameObject[] genreButtons;
	
	public tk2dTextMesh scriptPriceMesh;			//текстМеш цены	
	int scriptPrice;								//цифровое значение цены
	
	public tk2dTextMesh scriptTimeMesh;				//текстМеш времени
	int scriptTime;									//цифровое значение времени
	
	public GameObject buttonAccept;					//кнопка "ок"
	public GameObject buttonClose;					//кнопка закрыть меню
	public GameObject backButton;					//кнопка назад

	public ScriptersOfficeStage office = null;				//офис
	PopUpNewScriptState state = PopUpNewScriptState.notActive;						//состояние меню

	List<FilmStaff> availableScripters = new List<FilmStaff>();		//все доступные сценаристы
	List<FilmStaff> addedSripters = new List<FilmStaff>();			//выбранные сценаристы
	int scriptersMax = 0;						//макс. кол-во сценаристов
	
	List<FilmGenres> genres = new List<FilmGenres>();			//выбранные жанры
	GameObject setting;						//сеттинг
	
	void Start () 
	{
		SwitchState(PopUpNewScriptState.selectGenres);
	}
	
	void CheckTap(GameObject go)
	{
		foreach(GameObject g in genreButtons)
		{
			if(g == go)
			{
				AddNewGenre(go.GetComponent<GenreButton>());
			}
		}
		if(go == hireScripterButton)
		{
			if(GlobalVars.stars >= 100)
			{
				GlobalVars.stars -= 100;
				StaffManagment.HireStaff(CharacterType.scripter);
				hireScripterPlane.SetActive(false);
				SwitchState(PopUpNewScriptState.selectScripters);
			}
		}
		if(go == buttonAccept)
		{
			if(state == PopUpNewScriptState.selectScripters && addedSripters.Count > 0)
			{
				if(GlobalVars.money >= scriptPrice)
				{
					GlobalVars.money -= scriptPrice;
					Finishing();
				}
			}
			
		}
		if(go == buttonClose)
		{
			if(office != null)
			{
				office.isStageBusy = false;
			}
			SwitchState(PopUpNewScriptState.notActive);
			
		}
		if(go == backButton)
		{
			SwitchState(PopUpNewScriptState.selectGenres);
		}
		if(go.name == "CharInfo")
		{
			AddNewScripter(go.transform.GetComponent<CharInfo>().staff);
		}
	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckButtonsAvailability()
	{
		foreach(GameObject g in genreButtons)
		{
			g.SetActive(true);
			g.GetComponent<GenreButton>().CheckAvailability();
		}
	}

	//добавить нового сценариста, если его нет в списке - добавляем, если есть - удаляем его из списка.
	//также обновление общей цены и времени написания сценария.
	void AddNewScripter(FilmStaff scripter)
	{
		if(addedSripters.Exists(delegate(FilmStaff sc)
		{ return sc == scripter;}))
		{
			scripter.mark.enabled = false;
			scriptPrice -= scripter.icon.GetComponent<CharInfo>().GetPrice(genres);
			scriptTime -= scripter.lvl * 30;
			addedSripters.Remove(scripter);
		}
		else if (addedSripters.Count < scriptersMax)
		{
			scripter.mark.enabled = true;	
			scriptPrice += scripter.icon.GetComponent<CharInfo>().GetPrice(genres);
			scriptTime += scripter.lvl * 30;
			addedSripters.Add(scripter);
		}
		headerText.SetTextWithIndex(1, " " + addedSripters.Count + "/" + scriptersMax);
		SetPrice();
		CheckAcceptButton();
	}
	
	void SetPrice()
	{
		string s = "";
		if(scriptPrice > 0)
		{
			buttonAccept.transform.FindChild("captionMesh1").GetComponent<MeshRenderer>().enabled = true;
			buttonAccept.transform.FindChild("captionMesh2").GetComponent<MeshRenderer>().enabled = false;
			buttonAccept.transform.FindChild("coins").GetComponent<MeshRenderer>().enabled = true;
			s = scriptPrice.ToString();
		}
		else
		{
			buttonAccept.transform.FindChild("captionMesh1").GetComponent<MeshRenderer>().enabled = false;
			buttonAccept.transform.FindChild("captionMesh2").GetComponent<MeshRenderer>().enabled = true;
			buttonAccept.transform.FindChild("coins").GetComponent<MeshRenderer>().enabled = false;
		}
		scriptPriceMesh.text = s;
		scriptPriceMesh.Commit();
	}
	
	//обновить доступных сценаристов
	void RefreshAvailableScripters()
	{
		availableScripters.Clear();
		addedSripters.Clear();
		
		GameObject[] allScripters = GameObject.FindGameObjectsWithTag("scripter");
		
		foreach(GameObject scripter in allScripters)
		{
			FilmStaff fs = scripter.GetComponent<FilmStaff>();
			if(fs.canBeUsed)
			{
				availableScripters.Add(fs);
			}
		}
		
		for(int i = 0; i < availableScripters.Count; i++)
		{
			availableScripters[i].icon.transform.parent = partInfos.transform;
			availableScripters[i].icon.transform.localPosition = new Vector3(-250 + 350 * i, 0, -1);
			availableScripters[i].icon.transform.localScale = Vector3.one * 0.9f;
			availableScripters[i].icon.SetActive(true);
			availableScripters[i].icon.GetComponent<CharInfo>().Refresh();
			availableScripters[i].icon.GetComponent<CharInfo>().SetPricePerUse(genres);
		}
		if(availableScripters.Count == 0)
		{
			hireScripterPlane.SetActive(true);
			Utils.SetText(hireScripterPriceMesh, Utils.ToNumberWithSpaces(100 + ""));
		}
		else
		{
			hireScripterPlane.SetActive(false);
		}
		if(availableScripters.Count <= 2)
		{
			staffScrollArea.ContentLength = 351;
		}
		else
		{
			staffScrollArea.ContentLength = (availableScripters.Count - 2) * 350 + 200;
		}
	}
	
	
	//добавить/удалить новый жанр
	void AddNewGenre(GenreButton gb)
	{
		/*for(int i = 0; i < genres.Count; i++)
		{
			if(genres[i] == gb.genre)
			{
				GenreButton[] g = partGenres.GetComponentsInChildren<GenreButton>();
				for(int j = 0; j < g.Length; j++)
				{
					g[j].SetChoosen(false);
				}
				genres.Clear();
				//ClearParams();
				headerText.SetTextWithIndex(0, " " + genres.Count + "/" + genresCount);
				CheckAcceptButton();
				return;
			}
		}
		if(genres.Count < genresCount)
		{
			if(GlobalVars.scriptersGenres.Exists(delegate (AvailableGenre ag)
			{	return gb.genre == ag.genre;			}))
			{
				gb.SetChoosen(true);
				genres.Add(gb.genre);
			}
		}
		headerText.SetTextWithIndex(0, " " + genres.Count + "/" + genresCount);*/
		if(!GlobalVars.scriptersGenres.Exists(delegate(AvailableGenre obj) {
			return obj.genre == gb.genre;
		}))
		{
			return;
		}
		SwitchState(PopUpNewScriptState.selectScripters);
		genres.Add(gb.genre);
		CheckAcceptButton();
	}
	
	//изменить текущий сеттинг на тот, который подеается на вход
	void SelectSetting(GameObject sett)
	{
		if(setting != null)
		{
			setting.GetComponent<SettingButton>().SetChoosen(false);
		}
		setting = sett;
		setting.GetComponent<SettingButton>().SetChoosen(true);
	}
	
	//завершение, отправка переменных офису и начало производства сценария
	//смена состояния на не активное
	void Finishing()
	{
		office.StartScript(addedSripters, genres, scriptTime);
		SwitchState(PopUpNewScriptState.notActive);
	}
	
	//изменить состояние меню, в хависимости от выбранного состояния будут активны те или иные части меню, 
	//либо оно будет не активно полностью
	public void SwitchState(PopUpNewScriptState s)
	{
		state = s;
		if(s == PopUpNewScriptState.notActive)
		{
			
			for(int i = 0; i < availableScripters.Count; i++)
			{
				availableScripters[i].icon.transform.parent = availableScripters[i].icon.GetComponent<CharInfo>().character.transform;
				availableScripters[i].mark.enabled = false;
				print (i);
				availableScripters[i].icon.SetActive(false);
			}
			
			GenreButton[] gb = transform.GetComponentsInChildren<GenreButton>(true);
			foreach(GenreButton g in gb)
			{
				g.SetChoosen(false);
			}
			
			SettingButton[] sett = transform.GetComponentsInChildren<SettingButton>(true);
			foreach(SettingButton se in sett)
			{
				se.SetChoosen(false);
			}
			state = PopUpNewScriptState.notActive;
			gameObject.SetActive(false);
			//partBackground.SetActive(false);
			//partGenres.SetActive(false);
			//buttonAccept.SetActive(false);
			//buttonClose.SetActive(false);
			//backButton.SetActive(false);
			//partInfos.SetActive(false);
			
			ClearParams();
		}
		if(s == PopUpNewScriptState.selectScripters)
		{
			
			partGenres.SetActive(false);
			partInfos.SetActive(true);
			backButton.SetActive(true);
			RefreshAvailableScripters();
			state = PopUpNewScriptState.selectScripters;
			headerText.SetTextWithIndex(1);
			buttonAccept.GetComponentInChildren<GetTextFromFile>().SetTextWithIndex(1);
			CheckAcceptButton();
			SetPrice();
		}
		
		if(s == PopUpNewScriptState.selectGenres)
		{
			for(int i = 0; i < availableScripters.Count; i++)
			{
				availableScripters[i].icon.transform.parent = availableScripters[i].icon.GetComponent<CharInfo>().character.transform;
				availableScripters[i].mark.enabled = false;
				print (i);
				availableScripters[i].icon.SetActive(false);
			}
			GenreButton[] gb = transform.GetComponentsInChildren<GenreButton>(true);
			foreach(GenreButton g in gb)
			{
				g.SetChoosen(false);
			}
			addedSripters = new List<FilmStaff>();
			scriptPriceMesh.text = "";
			scriptPriceMesh.Commit();
			
			availableScripters.Clear();
			scriptPrice = 0;
			scriptTime = 0;
			RefreshAvailableScripters();
			state = PopUpNewScriptState.selectGenres;
			genres.Clear();
			partBackground.SetActive(true);
			partGenres.SetActive(true);
			buttonAccept.SetActive(true);
			buttonClose.SetActive(true);
			partInfos.SetActive(false);
			backButton.SetActive(false);
			//buttonAccept.GetComponentInChildren<GetTextFromFile>().SetTextWithIndex(0);
			headerText.SetTextWithIndex(0);
			gameObject.SetActive(true);
			CheckAcceptButton();
			SetPrice();
		}
		if(s == PopUpNewScriptState.selectSettings)
		{	
			state = PopUpNewScriptState.selectSettings;
			partBackground.SetActive(true);
			partGenres.SetActive(false);
			buttonAccept.SetActive(true);
			buttonClose.SetActive(true);
			SetPrice();
		}
	}
	
	void ClearParams()
	{
		scriptPrice = 0;
		scriptTime = 0;
		office = null;
		scriptersMax = 0;
		setting = null;
		GlobalVars.cameraStates = CameraStates.normal;
	}
	
	//получение параметров от здания
	public void SetParams(ScriptersOfficeStage officeStage)
	{
		office = officeStage;
		scriptersMax = 3;
		SwitchState(PopUpNewScriptState.selectGenres);
	}
	
	void CheckAcceptButton()
	{
		if(state == PopUpNewScriptState.selectScripters && GlobalVars.money >= scriptPrice && addedSripters.Count > 0)
		{
			buttonAccept.GetComponent<TapOnElement>().SetActiveTo(true);
		}
		else if(state == PopUpNewScriptState.selectGenres && genres.Count > 0)
		{
			buttonAccept.GetComponent<TapOnElement>().SetActiveTo(true);
		}
		else
		{
			buttonAccept.GetComponent<TapOnElement>().SetActiveTo(false);
		}
	}
}
