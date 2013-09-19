using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//кнопка выбора сеттинга, аналогична кнопке выбора жанра
public class SettingButton : MonoBehaviour {
	
	public Setting setting;					//сеттинг
	public RarityLevel rarity;
	public MeshRenderer checkMark;			//маркер
	bool isChoosen = false;
	
	public tk2dTextMesh count;
	public tk2dTextMesh timeMesh;
	public GetTextFromFile maintenance;
	public GetTextFromFile headerText;

	public tk2dUIProgressBar progressbar;
	
	public GameObject icons;
	public GameObject closed;
	public GameObject adventure;
	public GameObject war;
	public GameObject space;
	public GameObject historical;
	public GameObject fantasy;
	public GameObject modern;
	public GameObject button;
	public GameObject bottomPart;
	
	public List<DecorationItem> decorationItems;
	
	public DecorationItem decor;
	
	void Start()
	{
		RefreshPlaneInfo(Setting.Adventure, 5);
	}
	
	public bool GetChoosen()
	{
		return isChoosen;
	}
	
	public void SetChoosen(bool b)
	{
		isChoosen = b;
	}
	
	public void SetDecoration(DecorationItem item = null)
	{
		if(item != null)
		{
			decor = item;
			SetIcon(item.setting);
			ChangeProgress(item.maintenance);
			maintenance.SetTextWithIndex(0, " " + (Mathf.RoundToInt(item.maintenance / 0.05f)).ToString() + "%");
			ShowButton(false);
		}
		else
		{
			SetIcon();
			ShowButton(true);
		}
	}
	
	public void SetIcon(Setting s = Setting.none)
	{	
		setting = s;
		icons.SetActive(false);
		switch(setting)
		{
		case Setting.Adventure:
			adventure.SetActive(true);
			break;
		case Setting.Fantasy:
			fantasy.SetActive(true);
			break;
		case Setting.Historical:
			historical.SetActive(true);
			break;
		case Setting.Modern:
			modern.SetActive(true);
			break;
		case Setting.Space:
			space.SetActive(true);
			break;
		case Setting.War:
			war.SetActive(true);
			break;
		default:
			closed.SetActive(true);
			break;
		}
	}
	
	public void RefreshPlaneInfo(Setting s, int time)
	{
		timeMesh.text = Utils.FormatIntToUsualTimeString(time * 60, 2);
		setting = s;
		decorationItems.Clear();
		foreach(InventoryItem item in GlobalVars.inventory.items)
		{
			if(item.GetComponent<DecorationItem>() != null)
			{
				if(item.GetComponent<DecorationItem>().setting == setting && item.GetComponent<DecorationItem>().rarity == rarity)
				{
					decorationItems.Add(item.GetComponent<DecorationItem>());
				}
			}
		}
		count.text = decorationItems.Count.ToString();
		if(decorationItems.Count > 0)
			closed.SetActive(false);
		else
			closed.SetActive(true);
		adventure.SetActive(false);
		war.SetActive(false);
		fantasy.SetActive(false);
		historical.SetActive(false);
		modern.SetActive(false);
		space.SetActive(false);
		switch(setting)
		{
		case Setting.Adventure:
			adventure.SetActive(true);
			headerText.SetTextWithIndex(0);
			break;
		case Setting.Fantasy:
			fantasy.SetActive(true);
			headerText.SetTextWithIndex(1);
			break;
		case Setting.Historical:
			historical.SetActive(true);
			headerText.SetTextWithIndex(2);
			break;
		case Setting.Modern:
			modern.SetActive(true);
			headerText.SetTextWithIndex(3);
			break;
		case Setting.Space:
			space.SetActive(true);
			headerText.SetTextWithIndex(4);
			break;
		case Setting.War:
			war.SetActive(true);
			headerText.SetTextWithIndex(5);
			break;
		}
	}
	
	public void ShowButton(bool b)
	{
		if(b)
		{
			button.SetActive(true);
			bottomPart.SetActive(false);
		}
		else
		{
			button.SetActive(false);
			bottomPart.SetActive(true);
		}
	}
	
	public void ChangeProgress(int curr)
	{
		float percents = (curr / 0.05f);
		progressbar.Value = percents / 100;
	}
	
	void Update () 
	{
		if(isChoosen)
		{
			checkMark.enabled = true;
		}
		else if(!isChoosen)
		{
			checkMark.enabled = false;
		}
	}
}
