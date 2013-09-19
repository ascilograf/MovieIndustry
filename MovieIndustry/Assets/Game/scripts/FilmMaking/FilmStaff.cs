using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CharInfoState
{
	hide,
	showForHire,
	ShowForInfo,
	ShowForInstantiating,
	ShowForLvlUp,
	ShowForInventory,
}

//скрипт в котором содержатся параметры персонала для найма на фильм
//также здесь инициализируется автар персонажа
//и здесь находится ункция по обновлению характеристик персонажа в прогрессбарах
public class FilmStaff : MonoBehaviour 
{
	
	public List<FilmSkills> skills;				//умения по жанрам
	public List<StaffSkills> mainSkill;

	//public List<FilmGenres> mainSkill;		//избранные умения
	public List<GameObject> mainSkillProgress;	//прогрессбары основных скилов (для инвентаря)
	public List<FilmGenres> availableGenres;//доступные жанры
	public int visuals;
	public UIProgressBar visualsProgressBar;
	public GameObject inventoryObject;
	public int lvl;							//уровень
	public int exp;							//кол-во опыта
	public int freeSkillPoints;
	public GameObject icon;					//иконка
	public GameObject avatar; 				//аватар персонажа
	public bool canBeUsed;					//может ли быть использованным
	public tk2dTextMesh price;				//префаб текстМеша
	public tk2dTextMesh lvlMesh;
	public tk2dTextMesh expMesh;
	public MeshRenderer mark;				//маркер
	public bool busy;						//занят ли
	public CharInfoState charInfoState;
	public GameObject lvlArrow;
	
	int lesserParam = 0;
	PersonController pc;
	AvailableGenre ag;
	
	public TrailerController trailer;
	public BonusParams bonuses;
	
	StaffInventory inventory;
	
	void Awake()
	{
		pc = GetComponent<PersonController>();
		inventory = GetComponent<StaffInventory>();
	}
	
	void Start () 
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject g)
	{
		if(g == gameObject)
		{
			GlobalVars.popUpHireStaff.ShowMenu(gameObject.tag, this);
		}
	}
	
	public void FindTrailer()
	{
		GameObject[] trailers = GameObject.FindGameObjectsWithTag("Trailer");
		foreach(GameObject g in trailers)
		{
			TrailerController t = g.GetComponent<TrailerController>();
			if(t != null)
			{
				if(t.staff == null)
				{
					t.staff = this;
					trailer = t;
					return;
				}
			}
		}
	}
	
	//обновление имени и значения прогрессбара основных умений в меню инвентаря
	public void RefreshInventoryInfo()
	{
		for(int i = 0; i < mainSkillProgress.Count; i++)
		{
			tk2dTextMesh textMesh = mainSkillProgress[i].GetComponentInChildren<tk2dTextMesh>();
			UIProgressBar progressBar = mainSkillProgress[i].GetComponentInChildren<UIProgressBar>();
			if(mainSkill.Count > i)
			{	
				textMesh.text = mainSkill[i].ToString();
				textMesh.Commit();
				foreach(FilmSkills selectedSkill in skills)
				{
					if(selectedSkill.genre == mainSkill[i].genre)
					{
						progressBar.Value = GetSkillWithBonus(selectedSkill.genre) * 0.01f;
					}
				}
			}
			else 
			{
				textMesh.text = "null";
				textMesh.Commit();
				progressBar.Value = 0;
			}
		}
		ChangeBonuses();
	}
	
	//вычислить значение умения + бонус от инвентаря
	public int GetSkillWithBonus(FilmGenres genre)
	{
		int val = 0;
		foreach(FilmSkills x in skills)
		{
			if(x.genre == genre)
			{
				if(x.genre == mainSkill[0].genre)
				{
					val = x.skill + bonuses.genresBonuses[0];
					
				}
				else if(x.genre == mainSkill[1].genre)
				{
					val = x.skill + bonuses.genresBonuses[1];
				}
				else 
				{
					val = x.skill;
				}
			}
		}
		return val;
	}
	
	public void StartFadeCheckMark()
	{
		mark.enabled = true;
		StartCoroutine(fadeCheckMark ());
	}
	
	IEnumerator fadeCheckMark(float t = 0)
	{
		while(true)
		{
			if(t >= 1)
			{
				mark.sharedMaterial.color = Color.white;
				mark.enabled = false;
				yield break;
			}
			t += Time.deltaTime;
			Color c = Color.white;
			c.a = 0;
			mark.sharedMaterial.color = Color.Lerp(Color.white, c, t);
			yield return 0;
		}
	}
	
	//обновить сумарные бонусы от всех предметов в инвентаре
	public void ChangeBonuses()
	{
		BonusParams b = new BonusParams();
		foreach(StaffInventoryItemParams item in inventory.items)
		{	
			if(item.item != null)
			{
				b.ExpGainBonus += item.item.bonuses.ExpGainBonus;
				b.genresBonuses[0] += item.item.bonuses.genresBonuses[0];
				b.genresBonuses[1] += item.item.bonuses.genresBonuses[1];
				b.MoneyGainBonus += item.item.bonuses.MoneyGainBonus;
			}
		}
		bonuses = b;
	}
	
	//проверить кол-во доступных жанров
	void CheckAvailableGenres()
	{
		availableGenres.Clear();
		switch(tag)
		{
		case "scripter":
			
			foreach(AvailableGenre ag in GlobalVars.scriptersGenres)
			{
				availableGenres.Add(ag.genre);
			}
			break;
		case "actor":
			foreach(AvailableGenre ag in GlobalVars.actorsGenres)
			{
				availableGenres.Add(ag.genre);
			}
			break;
		case "director":
			foreach(AvailableGenre ag in GlobalVars.directorsGenres)
			{
				availableGenres.Add(ag.genre);
			}//
			break;
		case "cameraman":
			foreach(AvailableGenre ag in GlobalVars.cameramansGenres)
			{	
				availableGenres.Add(ag.genre);
			}
			break;
		}
	}
	
	//обновить параметры персонажа
	public void RefreshStats()
	{
		if(skills.Count > 0)
		{
			CheckAvailableGenres();
			CheckLesserParam();
			for(int i = 0; i < skills.Count; i++)
			{
				if(availableGenres.Exists(delegate(FilmGenres fg)
				{ return fg ==  skills[i].genre;	}))
				{
					if(mainSkill.Exists(delegate(StaffSkills fg)
					{ return fg.genre ==  skills[i].genre;	}))
					{
						//skills[i].progressBar.Value = GetSkillWithBonus(skills[i].genre) * 0.01f;
					}
					else if(mainSkill.Count > 1)
					{
						if(lesserParam > 0)
						{
							skills[i].skill = lesserParam/2;
						}
						else
						{
							skills[i].skill = 0;
						}
						//skills[i].progressBar.Value = skills[i].skill * 0.01f;
					}
					else 
					{
						skills[i].skill = 0;
						//skills[i].progressBar.Value = skills[i].skill * 0.01f;
					}
				}
				else
				{
					skills[i].skill = 0;
					//skills[i].progressBar.Value = skills[i].skill * 0.01f;
				}
			}
			
			
		}
		else if(visualsProgressBar != null)
		{
			visualsProgressBar.Value = visuals * 0.01f;
		}
		//price.text = (lvl * 1000).ToString();
		//price.Commit();
		CheckLvl();
	}
	
	//проверить какой из параметров самый минимальный и по нему считать не основные параметры
	void CheckLesserParam()
	{
		lesserParam = 0;
		foreach(FilmSkills f in skills)
		{
			if(mainSkill.Exists(delegate(StaffSkills fg)
			{	return fg.genre == f.genre;	}))
			{
				if(lesserParam == 0)
				{
					lesserParam = f.skill;
				}
				else if(lesserParam > f.skill)
				{
					lesserParam = f.skill;
				}
			}
		}
	}
	
	void Update ()
	{
		//CheckLvl();
		if(pc.head != null && pc.body != null & pc.legs != null)
		{
			if(canBeUsed && freeSkillPoints > 0)
			{
				if(!lvlArrow.activeSelf || collider.enabled == false)
				{
					collider.enabled = true;
					lvlArrow.SetActive(true);
				}
			}
			else
			{
				if(lvlArrow.activeSelf || collider.enabled == true)
				{
					collider.enabled = false;
					lvlArrow.SetActive(false);
				}
			}
		}	
	}
	
	
	public void CheckLvl()
	{
		if(exp >= 100 * (lvl + 1))
		{
			exp = exp - 100 * (lvl + 1);
			lvl++;
			freeSkillPoints++;
		}
	}
}
