using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//контроллер меню завершения таких задач как: снятие сцены, написание сценария, постпродакшн, постройка/апгрейд здания
//на вход в функцию SetParamsForFinish() поступает класс этажа от здания, заполняются параметры, обновляются текстМеши
//если это постройка/апгрейд - определяется тип здания и из массива иконок этого здания выдергивается нужны этаж
//если это не постройка/апгрейд - дергаются плашки персонажей и выставляются на показ, определяются границы свайпа
//при нажатии на кнопку завершения проверяется наличие прем. валюты.

public class PopUpSpeedUpAnyJob : MonoBehaviour 
{
	enum States
	{
		finishScript,
		finishPostProd,
		finishScene,
		finishConstruct,
		notActive,
	}
	
	
	public GameObject buttonClose;
	public GameObject buttonFinishItNow;
	public GameObject parentMenu;
	public Transform infosCharInfo;
	public Transform infosSceneInfo;
	public tk2dUIScrollableArea staffScroll;
	
	//params for finish script
	public GameObject parentStory;
	public GameObject parentScenes;
	public tk2dTextMesh meshScenes;
	
	//params for finish post-Production
	public GameObject parentRating;
	
	//params for build and upgrade
	public tk2dTextMesh descriptionMesh;
	public GameObject constructParent;
	public tk2dTextMesh moneyMesh;
	public tk2dTextMesh timeForWorkMesh;
	public tk2dTextMesh workNameMesh;
	public GameObject[] iconsMainOffices;
	public GameObject[] iconsScriptersOffices;
	public GameObject[] iconsPostProdsOffices;
	public GameObject[] iconsHangars;
	public GameObject[] iconsTrailers;
	
	//params fo all
	public tk2dTextMesh meshHeader;
	public tk2dTextMesh meshRemainingTime;
	public tk2dTextMesh meshGenres;
	public tk2dTextMesh meshPriceInDollars;
	public GameObject[] progresbarStars;
	
	States state;
	ScriptersOfficeStage scriptersStage;
	PostProdOfficeStage postProdStage;
	FilmMaking filmMaking;
	public Construct construct;
	GameObject iconDecoration;
	
	float minLength, maxLength;
	int price;
	// Use this for initialization
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject g)
	{
		if(g == buttonFinishItNow)
		{
			if(GlobalVars.stars < price)
			{
				return;
			}
			else if(state == States.finishScript)
			{
				GlobalVars.stars -= price;
				scriptersStage.time = 0;
				SwitchState(States.notActive);
			}
			else if(state == States.finishPostProd)
			{
				GlobalVars.stars -= price;
				postProdStage.time = 0;
				SwitchState(States.notActive);
			}
			else if(state == States.finishScene)
			{
				GlobalVars.stars -= price;
				filmMaking.time = 0;
				SwitchState(States.notActive);
			}
			else if(state == States.finishConstruct)
			{
				GlobalVars.stars -= price;
				construct.timeToBuild = 0;
				SwitchState(States.notActive);
			}
		}
		else if(g == buttonClose)
		{
			SwitchState(States.notActive);
		}
	}
	
	
	
	public void SetParamsForFinish(ScriptersOfficeStage s)
	{
		scriptersStage = s;
		Utils.SetText(meshGenres, Utils.GenresInText(s.Genres));
		SetText(meshScenes, "3");
		SwitchState(States.finishScript);
		SetProgressBarValue((int)scriptersStage.story);
		SetCharInfos(s.scripters);
		SetText(meshHeader, "Writing a script");
	}
	
	public void SetParamsForFinish(PostProdOfficeStage p)
	{
		postProdStage = p;
		Utils.SetText(meshGenres, Utils.GenresInText(p.film.genres));
		SwitchState(States.finishPostProd);
		SetProgressBarValue((int)p.visuals);
		SetCharInfos(p.postProdWorkers);
		SetText(meshHeader, p.film.name);
	}
	
	public void SetParamsForFinish(FilmMaking f)
	{
		filmMaking = f;
		Utils.SetText(meshGenres, Utils.GenresInText(filmMaking.script.genres));
		SwitchState(States.finishScene);
		SetProgressBarValue(filmMaking.script.story);
		SetCharInfos(filmMaking.staff);
		//SetSceneInfos();
		SetText(meshHeader, filmMaking.script.name);
		SetText(meshScenes, filmMaking.usedDecorations.Count + "/" + filmMaking.script.numberOfScenes);
	}
	
	
	public void SetParamsForFinish(Construct c)
	{
		construct = c;	
		SwitchState(States.finishConstruct);
		SetInformationForFinishConstruct(construct);
		Utils.SetText(descriptionMesh, Utils.FormatStringToText("I need some long text for test this new feature in our project. Do You have it? Right? Nice, thank You, this is what I'm looking for.", 17));
	}
	
	
	
	public void SetInformationForFinishConstruct(Construct c)
	{
		Works work = new Works();
		int index = (int)c.stage;
		switch (c.type)
		{
		case BuildingType.scriptWrittersOffice:
			work = GlobalVars.scriptersWorklist[index].works[index];
			meshHeader.GetComponent<GetTextFromFile>().SetTextWithIndex(1, ", Stage " + index + 1);
			iconsScriptersOffices[index].SetActive(true);
			break;
		case BuildingType.office:
			work = GlobalVars.producersWorklist[index].works[index];
			meshHeader.GetComponent<GetTextFromFile>().SetTextWithIndex(0, ", Stage " + index + 1);
			iconsMainOffices[index].SetActive(true);
			break;
		case BuildingType.hangar:	
			work = GlobalVars.hangarWorklist[index].works[0];
			meshHeader.GetComponent<GetTextFromFile>().SetTextWithIndex(3, ", Stage " + index + 1);
			iconsHangars[index].SetActive(true);
			break;
		case BuildingType.postproduction:
			work = GlobalVars.postProdWorklist[index].works[index];
			meshHeader.GetComponent<GetTextFromFile>().SetTextWithIndex(2, ", Stage " + index + 1);
			iconsPostProdsOffices[index].SetActive(true);
			break;
		case BuildingType.trailer:
			workNameMesh.gameObject.SetActive(false);
			timeForWorkMesh.gameObject.SetActive(false);
			moneyMesh.gameObject.SetActive(false);
			iconsTrailers[index].SetActive(true);
			break;
		}
		SetText(workNameMesh, work.name);
		SetText(timeForWorkMesh, Utils.FormatIntToUsualTimeString(work.timeSec, 2));
		SetText(moneyMesh, work.profit.ToString());
		SetText (meshPriceInDollars, construct.timeToBuild.ToString());
	}
	
	void ChangeTime()
	{
		switch(state)
		{
		case States.finishPostProd:
			SetText(meshRemainingTime, Utils.FormatIntToUsualTimeString((int)postProdStage.time, 2));
			SetPrice((int)postProdStage.time);
			if(postProdStage.time <= 0)
			{
				SwitchState(States.notActive);
			}
			break;
		case States.finishScript:
			SetText(meshRemainingTime, Utils.FormatIntToUsualTimeString((int)scriptersStage.time, 2));
			SetPrice((int)scriptersStage.time);
			if(scriptersStage.time <= 0)
			{
				SwitchState(States.notActive);
			}
			break;
		case States.finishScene:
			SetText(meshRemainingTime, Utils.FormatIntToUsualTimeString((int)filmMaking.time, 2));
			SetPrice((int)filmMaking.time);
			if(filmMaking.time <= 0)
			{
				SwitchState(States.notActive);
			}
			break;
		case States.finishConstruct:
			SetText(meshRemainingTime, Utils.FormatIntToUsualTimeString((int)construct.timeToBuild, 2));
			SetPrice((int)construct.timeToBuild);
			if(construct.timeToBuild <= 0)
			{
				SwitchState(States.notActive);
			}
			break;	
		}
	}
	
	void SetText(tk2dTextMesh mesh, string s)
	{
		mesh.text = s;
		mesh.Commit();
	}
	
	void CharInfoToParent(List<FilmStaff> staff)
	{
		if(staff.Count > 0)
		{
			for(int i = 0; i < staff.Count; i++)
			{
				staff[i].icon.transform.parent = staff[i].icon.GetComponent<CharInfo>().character.transform;
				staff[i].mark.enabled = false;
				staff[i].icon.SetActive(false);	
			}
		}
	}
	
	void SetSceneInfos()
	{
		GameObject icon = Instantiate(filmMaking.usedDecorations[filmMaking.usedDecorations.Count - 1]) as GameObject;
		icon.transform.parent = infosSceneInfo;
		icon.transform.localPosition = new Vector3(-275, 0,-5);
		icon.transform.localScale = Vector3.one;
		icon.SetActive(true);
		iconDecoration = icon; 
	}
	
	void SetCharInfos(List<FilmStaff> staff)
	{
		for(int i = 0; i < staff.Count; i++)
		{
			CharInfo c = staff[i].GetComponent<FilmStaff>().icon.GetComponent<CharInfo>();
			c.transform.parent = infosCharInfo.transform;
			c.transform.localPosition = new Vector3(90 + i * 350, 0, 0);
			c.transform.localScale = Vector3.one;
			c.gameObject.SetActive(true);
			c.Refresh();
			c.ShowBottomType();
			c.buttonHireStaff.SetActive(false);
		}
		if(staff.Count > 1)
		{
			staffScroll.ContentLength = (staff.Count - 1) * 350 + 220;
		}
		else
		{
			staffScroll.ContentLength = 351;	
		}
	}
	
	void SetPrice(int time)
	{
		price = Mathf.RoundToInt(time * 0.1f);
		if(price == 0)
		{
			price = 1;
		}
		meshPriceInDollars.text = price.ToString();
		meshPriceInDollars.Commit();
	}
	
	void SetProgressBarValue(int skill)
	{
		for(int i = 0; i < 5; i++)
		{
			if((skill - 20) > 0)
			{	
				DeactivateStarExcept(progresbarStars[i], "5state");
			}
			else if(skill < 1)
			{
				DeactivateStarExcept(progresbarStars[i], "empty");
			}
			else if(skill < 5)
			{
				DeactivateStarExcept(progresbarStars[i], "1state", progresbarStars[i].transform);
			}
			else if(skill < 9)
			{
				DeactivateStarExcept(progresbarStars[i], "2state", progresbarStars[i].transform);
			}
			else if(skill < 13)
			{
				DeactivateStarExcept(progresbarStars[i], "3state", progresbarStars[i].transform);
			}
			else if(skill < 17)
			{
				DeactivateStarExcept(progresbarStars[i], "4state", progresbarStars[i].transform);
			}
			else if(skill < 21)
			{
				DeactivateStarExcept(progresbarStars[i], "5state", progresbarStars[i].transform);
			}
			skill -= 20;
		}
	}
	
	void DeactivateStarExcept(GameObject go,string state, Transform button = null)
	{
		go.SetActive(true);
		Transform[] t = go.GetComponentsInChildren<Transform>(true);
		foreach(Transform tr in t)
		{
			if(tr.name == state || tr.name == go.name)
			{
				tr.gameObject.SetActive(true);
			}
			else
			{
				tr.gameObject.SetActive(false);
			}
		}
	}
	
	void SwitchState(States s)
	{
		
		gameObject.SetActive(true);
		meshGenres.transform.parent.gameObject.SetActive(false);
		workNameMesh.gameObject.SetActive(false);
		timeForWorkMesh.gameObject.SetActive(false);
		moneyMesh.gameObject.SetActive(false);
		descriptionMesh.gameObject.SetActive(false);
		constructParent.gameObject.SetActive(false);
		parentRating.SetActive(false);
		GlobalVars.cameraStates = CameraStates.menu;
		switch(s)
		{
		case States.finishPostProd:
			meshGenres.transform.gameObject.SetActive(true);
			parentRating.SetActive(true);
			break;
		case States.finishScript:
			meshGenres.transform.gameObject.SetActive(true);
			parentStory.SetActive(true);
			parentScenes.SetActive(true);
			break;
		case States.finishScene:
			meshGenres.transform.gameObject.SetActive(true);
			parentScenes.SetActive(true);
			parentRating.SetActive(true);
			break;
		case States.finishConstruct:
			workNameMesh.gameObject.SetActive(true);
			timeForWorkMesh.gameObject.SetActive(true);
			moneyMesh.gameObject.SetActive(true);
			descriptionMesh.gameObject.SetActive(true);
			break;
		case States.notActive:
			GlobalVars.cameraStates = CameraStates.normal;
			gameObject.SetActive(false);
			if(state == States.finishScript)
			{
				CharInfoToParent(scriptersStage.scripters);
			}
			else if(state == States.finishPostProd)
			{
				CharInfoToParent(postProdStage.postProdWorkers);
			}
			else if(state == States.finishScene)
			{
				CharInfoToParent(filmMaking.staff);
				Destroy(iconDecoration);
			}
			else if(state == States.finishConstruct)
			{
				
			}
			break;
		}
		state = s;
	}
	
	//отработка свайпа
	void Update()
	{
		ChangeTime();
	}
}
