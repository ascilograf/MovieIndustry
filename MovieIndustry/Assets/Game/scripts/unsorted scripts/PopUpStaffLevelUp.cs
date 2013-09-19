using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopUpStaffLevelUp : MonoBehaviour 
{
	public GameObject resetButton;
	public GameObject acceptButton;
	
	public tk2dTextMesh lvlMesh;
	public tk2dTextMesh nameMesh;
	public tk2dTextMesh freePointsMesh;
	public tk2dTextMesh priceMesh;
	public tk2dTextMesh timeMesh;
	
	public Progressbars[] progressbars;
	
	public Transform avatarPlacement;
	
	GameObject avatar;
	int startFreeSkillPoints;
	int money;
	float time;
	
	public CharInfo charInfo;
	
	[System.Serializable]
	public class Progressbars
	{
		public Collider plusButton;
		public GameObject icon;
		public GameObject[] stars; 
		public GameObject[] icons;
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);

	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject g)
	{
		if(g == resetButton)
		{
			Reset();
		}
		else if(g == acceptButton && GlobalVars.money >= money)
		{
			if(time > 0)
			{
				Accept();
			}
			else
			{
				Accept(false);
			}
		}
		
		else if(g.name == "shadow")
		{
			Reset();
			Accept(false);
		}
		else if(charInfo.HaveGenres)
		{
			foreach(FilmSkills f in charInfo.filmSkills)
			{
				if(f.plusButton == g.collider && charInfo.staff.freeSkillPoints > 0)
				{
					f.skillToUp += 4;
					charInfo.staff.freeSkillPoints--;
					Refresh();
				}
			}
		}
		else if(!charInfo.HaveGenres)
		{
			if(charInfo.otherSkills.plusButton == g.collider && charInfo.staff.freeSkillPoints > 0)
			{
				charInfo.otherSkills.skillToUp += 4;
				charInfo.staff.freeSkillPoints--;
				Refresh();
			}
		}
		
	}
	
	public void SetParams(CharInfo c)
	{
		gameObject.SetActive(true);
		charInfo = c;
		if(charInfo.HaveGenres)
		{
			for(int i = 0; i < charInfo.filmSkills.Count; i++)
			{
				if(charInfo.filmSkills[i].genre != FilmGenres.none && charInfo.filmSkills[i].skill > 0)
				{
					charInfo.filmSkills[i].plusButton = progressbars[i].plusButton;
					progressbars[i].icon.SetActive(true);
					foreach(GameObject g in progressbars[i].icons)
					{
						if(charInfo.filmSkills[i].genre.ToString() == g.name)
						{
							g.SetActive(true);
						}
						else
						{
							g.SetActive(false);
						}
					}
					//progressbars[i].icons.transform.FindChild(charInfo.filmSkills[i].genre.ToString()).gameObject.SetActive(true);		
				}
				else if(charInfo.filmSkills[i].skill == 0)
				{
					progressbars[i].plusButton.gameObject.SetActive(false);
				}
			}
		}
		else
		{
			for(int i = 0; i < 3; i++)
			{
				if(charInfo.otherSkills.skill > 0 && i == 0)
				{
					charInfo.otherSkills.plusButton = progressbars[0].plusButton;
					progressbars[0].icon.SetActive(true);
					foreach(GameObject g in progressbars[i].icons)
					{
						if(charInfo.otherSkills.activity.ToString() == g.name)
						{
							g.SetActive(true);
						}
						else
						{
							g.SetActive(false);
						}
					}
					//progressbars[0].icons.transform.FindChild(charInfo.otherSkills.activity.ToString()).gameObject.SetActive(true);		
				}
				else
				{
					progressbars[i].plusButton.gameObject.SetActive(false);
				}
			}
		}
		startFreeSkillPoints = charInfo.staff.freeSkillPoints;
		Refresh();
		DuplicateAvatar();
		Utils.SetText(nameMesh, charInfo.staff.name);
		Utils.SetText(lvlMesh, charInfo.staff.lvl.ToString());
	}
	
	void Refresh()
	{
		Utils.SetText(freePointsMesh, charInfo.staff.freeSkillPoints.ToString());
		if(charInfo.HaveGenres)
		{
			ChangeProgressBars(charInfo.filmSkills);
		}
		else
		{
			ChangeProgressBars(charInfo.otherSkills);
		}
		RefreshTimeMoneyCost();
	}
	
	void RefreshTimeMoneyCost()
	{
		money = 0;
		time = 0;
		if(charInfo.HaveGenres)
		{
			foreach(FilmSkills f in charInfo.filmSkills)
			{
				money += Formulas.LvlUpCost(f.skillToUp);
				time += Formulas.LvlUpTime(f.skillToUp);
			}
		}
		else
		{
			money += Formulas.LvlUpCost(charInfo.otherSkills.skillToUp);
			time += Formulas.LvlUpTime(charInfo.otherSkills.skillToUp);
		}
		Utils.SetText(priceMesh, Utils.ToNumberWithSpaces(money.ToString()));
		Utils.SetText(timeMesh, Utils.FormatIntToUsualTimeString((int)time, 2));
	}
	
	//установка жанров и умения для каждого из основных умений персонажа
	public void ChangeProgressBars(List<FilmSkills> skills)
	{
		for(int i = 0; i < 3; i++)
		{
			if(i < skills.Count)
			{
				SetSkillValue(charInfo.filmSkills[i].skill + charInfo.filmSkills[i].skillToUp, progressbars[i].stars);
			}
			else
			{
				SetSkillValue(0, progressbars[i].stars);
			}
		}
	}
	
	public void ChangeProgressBars(OtherSkills skills)
	{
		for(int i = 0; i < 3; i++)
		{
			if(i == 0)
			{
				SetSkillValue(skills.skill + skills.skillToUp, progressbars[i].stars);
			}
			else
			{
				SetSkillValue(0, progressbars[i].stars);
			}
		}
	}
	
	void SetSkillValue(int skill, GameObject[] progress)
	{	
		if(skill > 0)
		{
			for(int i = 0; i < 5; i++)
			{
				if((skill - 20) > 0)
				{	
					DeactivateStarExcept(progress[i], "5state");
				}
				else if(skill < 1)
				{
					DeactivateStarExcept(progress[i], "empty");
				}
				else if(skill < 5)
				{
					DeactivateStarExcept(progress[i], "1state", progress[i].transform);
				}
				else if(skill < 9)
				{
					DeactivateStarExcept(progress[i], "2state", progress[i].transform);
				}
				else if(skill < 13)
				{
					DeactivateStarExcept(progress[i], "3state", progress[i].transform);
				}
				else if(skill < 17)
				{
					DeactivateStarExcept(progress[i], "4state", progress[i].transform);
				}
				else if(skill < 21)
				{
					DeactivateStarExcept(progress[i], "5state", progress[i].transform);
				}
				skill -= 20;
			}
		}
		else
		{
			for(int i = 0; i < 5; i++)
			{
				progress[i].SetActive(false);
			}
		}
	}
	
	//деактивация звезд кроме выбранный
	void DeactivateStarExcept(GameObject go,string state, Transform button = null)
	{
		go.SetActive(true);
		Transform[] t = go.GetComponentsInChildren<Transform>(true);
		foreach(Transform tr in t)
		{
			if(tr.name == state || go.name == tr.name)
			{
				tr.gameObject.SetActive(true);
			}
			else
			{
				tr.gameObject.SetActive(false);
			}
		}
	}
	
	void DuplicateAvatar()
	{
		avatar = Instantiate(charInfo.avatar.gameObject) as GameObject;
		Transform[] tr = avatar.GetComponentsInChildren<Transform>();
		avatar.transform.parent = avatarPlacement;
		avatar.transform.localPosition = new Vector3(0, -2, 0);
		avatar.transform.localScale = Vector3.one;
	}
	
	void Reset()
	{
		foreach(FilmSkills f in charInfo.filmSkills)
		{
			f.skillToUp = 0;
		}
		charInfo.otherSkills.skillToUp = 0;
		charInfo.staff.freeSkillPoints = startFreeSkillPoints;
		Refresh();
	}
	
	void Accept(bool b = true)
	{
		Destroy(avatar);
		if(b)
			charInfo.SetParamsForUpSkill((int)time);
		else
			charInfo.SetParamsForUpSkill((int)time, false);
		gameObject.SetActive(false);
		GlobalVars.SwipeCamera.enabled = true;
	}
}
