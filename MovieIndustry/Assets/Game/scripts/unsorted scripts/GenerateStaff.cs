using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//надо доделать, пока не используется
//инициализация заданного списка персонажей на старте

public class GenerateStaff : MonoBehaviour {
	
	public int count;
	
	// Use this for initialization
	void Start () 
	{
		SetParamsAndStart(CharacterType.actor, false);
	}
	
	//установка параметров, на вход тип персонажа и примумный ли он
	public void SetParamsAndStart(CharacterType type, bool isPremium)
	{		
		//сначала определяется тип персонажа поданный на вход, в зависимости от этого выбирается нужный список доступных жанров
		List<AvailableGenre> ag = new List<AvailableGenre>();
		switch(type)
		{
		case CharacterType.actor:
			ag = GlobalVars.actorsGenres;
			break;
		case CharacterType.cameraman:
			ag = GlobalVars.cameramansGenres;
			break;
		case CharacterType.director:
			ag = GlobalVars.directorsGenres;
			break;
		case CharacterType.scripter:
			ag = GlobalVars.scriptersGenres;
			break;
		}
		
		//список персонажей заполняется тремя персонажами
		List<FilmStaff> st = new List<FilmStaff>();

		for(int i = 0; i < 3; i++)
		{
			//первый персонаж
			if(i == 0)
			{
				GameObject go = null;
				SetParamsForStaff(type, ag, out go, isPremium, 8, 16, new Vector3(-220 + i * 420, 0, 0));
				st.Add(go.GetComponent<FilmStaff>());
			}
			//второй и третий
			else
			{
				GameObject go = null;
				SetParamsForStaff(type, ag, out go, isPremium, 16, 20, new Vector3(-220 + i * 420, 0, 0));
				st.Add(go.GetComponent<FilmStaff>());
			}
			print (i.ToString());
		}
		//planeIndex = 3;
		//SetMinMaxLength(-815, 0);
		//SetSearchPlane();
	}
	
	//инициализация персонажей, обнуление их параметров, выборка доступных жанров и присвоение им новых значений
	void SetParamsForStaff(CharacterType ct, List<AvailableGenre> ag, out GameObject go, bool isPremium, int first, int second, Vector3 pos)
	{
		go = null;
		go = Instantiate(GlobalVars.commonPersonPrefab) as GameObject;
		go.tag = this.tag;
		go.GetComponent<PersonController>().InstantiatePerson(ct);
		go.SetActive(true);
		
		FilmStaff fs = go.GetComponent<FilmStaff>();
		//searchPlane.searchPlane.searchedStaff.Add(fs.icon);

		fs.exp = 0;
		fs.lvl = 0;
		fs.freeSkillPoints = 0;
		
		List<AvailableGenre> list = new List<AvailableGenre>();
		foreach(AvailableGenre a in ag)
		{
			list.Add(a);
		}
		if(list.Count > 0)
		{
			int rand = Random.Range(0, list.Count);	
			StaffSkills s = new StaffSkills();
			s.genre = list[rand].genre;
			s.skill = list[rand].max - first;
			fs.mainSkill.Add(s);
			foreach(FilmSkills skills in fs.skills)
			{
				if(skills.genre == fs.mainSkill[0].genre)
				{
					skills.skill = list[rand].max - first;
				}
			}
			list.Remove(list[rand]);
		}
		if(list.Count > 0)
		{
			int rand = Random.Range(0, list.Count);
			StaffSkills s = new StaffSkills();
			s.genre = list[rand].genre;
			s.skill = list[rand].max - first;
			fs.mainSkill.Add(s);
			foreach(FilmSkills skills in fs.skills)
			{
				if(skills.genre == fs.mainSkill[1].genre)
				{
					skills.skill = list[rand].max - second;
				}
			}
		}
		fs.icon.GetComponent<CharInfo>().SetParams(fs);
		fs.icon.SetActive(false);
		fs.icon.GetComponent<CharInfo>().Refresh();
		fs.icon.transform.localPosition = pos;
		fs.icon.transform.localScale = Vector3.one;
		
		go.SetActive(true);
		go.transform.localPosition = new Vector3(0, -150, GlobalVars.CHARACTER_FREE_LAYER);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
