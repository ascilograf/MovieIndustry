using UnityEngine;
using System.Collections;

public enum PopUpIncrStaffLvlStates
{
	notActive,
	increaseLvl,
	showCharInfo,
}


//меню, в котором можно повышать уровень персонажей 
//унифицировано для всех персонажей
public class PopUpIncrStaffLvl : MonoBehaviour 
{		
	public FilmStaff staff;							//персонаж
	public GameObject buttonClose;					//кнпка закрыть
	public GameObject buttonsIncrSkills; 			//кнопки плюсики
	public GameObject menu;							//парент-объект меню
	public PopUpIncrStaffLvlStates state;			//состояние меню
	
	void Awake()
	{
		GlobalVars.popUpIncrStafLvl = this;
	}
	
	void Start () 
	{
		SwitchState(PopUpIncrStaffLvlStates.notActive);
		Messenger<GameObject>.AddListener("Tap on GUI Layer", CheckTapOnGUI);
		Messenger<GameObject>.AddListener("Tap on Default Layer", CheckTap);
	}
	
	void CheckTapOnGUI(GameObject go)
	{
		if(state != PopUpIncrStaffLvlStates.notActive)
		{
			if(go.GetComponent<GenreButton>())
			{
				IncreaseSkill(go.GetComponent<GenreButton>().genre);
			}
			if(go == buttonClose)
			{
				SwitchState(PopUpIncrStaffLvlStates.notActive);
			}
		}
	}
	
	void CheckTap(GameObject go)
	{
		if(state == PopUpIncrStaffLvlStates.notActive)
		{
			FilmStaff fs = go.GetComponent<FilmStaff>();
			if(fs != null)
			{
				SetStaff(fs);
			}
		}
	}
	
	//установка параметров, на вход - персонаж
	public void SetStaff(FilmStaff st)
	{
		staff = st;
		GlobalVars.cameraStates = CameraStates.menu;
		staff.charInfoState = CharInfoState.ShowForLvlUp;
		staff.icon.transform.parent = transform;
		staff.icon.transform.localPosition = Vector3.zero;
		staff.icon.transform.localScale = Vector3.one;
		if(staff.freeSkillPoints > 0)
		{
			SwitchState(PopUpIncrStaffLvlStates.increaseLvl);
		}
		else if(staff.freeSkillPoints == 0)
		{
			SwitchState(PopUpIncrStaffLvlStates.showCharInfo);
		}
		
	}
	
	
	
	void Update () 
	{
	}
	
	//увеличение умения персонажа
	void IncreaseSkill(FilmGenres genre)
	{
		/*if(staff.freeSkillPoints > 0 && staff.availableGenres.Exists(delegate (FilmGenres fg)
		{	return fg == genre;	}))
		{
			for(int i = 0; i < staff.skills.Count; i++)
			{
				if(staff.skills[i].genre == genre)
				{
					//если основных скилов меньше 2-х и выбранный скилл не в их списке,
					//то добавляем новый осн. скилл
					if(staff.mainSkill.Count < 2 && !staff.mainSkill.Exists(delegate (FilmGenres fg)
					{	return fg == staff.skills[i].genre;	}))
					{
						staff.mainSkill.Add(genre);
					}
					//если выбранный скилл в списке основных - плюсуем его
					if(staff.mainSkill.Exists(delegate (FilmGenres fg)
					{	return fg == staff.skills[i].genre;	}) )
					{
						if(staff.tag == "director" || staff.tag == "actor")
						{
							if(staff.trailer != null)
							{
								if(staff.skills[i].skill + 1 <= staff.trailer.lvl + 1 * 20)
								{
									staff.skills[i].skill++;
									staff.freeSkillPoints--;
								}
							}
							else
							{
								if(staff.skills[i].skill + 1 < 20)
								{
									staff.skills[i].skill++;
									staff.freeSkillPoints--;
								}
							}
						}
						else
						{
							staff.skills[i].skill++;
							staff.freeSkillPoints--;
						}
						
						//если скилл  более 28, то добавляем млед. скилл.
						if(staff.skills[i].skill >= 28)// && !staff.availableGenres.Exists(delegate (FilmGenres fg)
						//{	return fg == staff.skills[i].genre + 1;	}) )
						{	
							switch(staff.tag)
							{
							case "scripter":
								//bool flag = false;
								foreach(AvailableGenre ag in GlobalVars.scriptersGenres)
								{
									if(ag.genre == genre)
									{
										if(staff.skills[i].skill > ag.max)
										{
											ag.max = staff.skills[i].skill;
										}
										//flag = true;
									}
								}
								if(staff.skills[i].skill >= 28 && !GlobalVars.scriptersGenres.Exists(delegate (AvailableGenre av)
								{	return av.genre == staff.skills[i].genre + 1;	}) )
								{
									{
										AvailableGenre a = new AvailableGenre();
										a.genre = genre + 1;
										GlobalVars.scriptersGenres.Add(a);
									}
								}
								break;
							case "actor":
								//flag = false;
								foreach(AvailableGenre ag in GlobalVars.actorsGenres)
								{
									if(ag.genre == genre)
									{
										if(staff.skills[i].skill > ag.max)
										{
											ag.max = staff.skills[i].skill;
										}
										//flag = true;
									}
								}
								if(staff.skills[i].skill >= 28 && !GlobalVars.actorsGenres.Exists(delegate (AvailableGenre av)
								{	return av.genre == staff.skills[i].genre + 1;	}) )
								{
									{
										AvailableGenre a = new AvailableGenre();
										a.genre = genre + 1;
										GlobalVars.actorsGenres.Add(a);
									}
								}
								break;
							case "director":
								//flag = false;
								foreach(AvailableGenre ag in GlobalVars.directorsGenres)
								{
									if(ag.genre == genre)
									{
										if(staff.skills[i].skill > ag.max)
										{
											ag.max = staff.skills[i].skill;
										}
										//flag = true;
									}
								}
								if(staff.skills[i].skill >= 28 && !GlobalVars.directorsGenres.Exists(delegate (AvailableGenre av)
								{	return av.genre == staff.skills[i].genre + 1;	}) )
								{
									{
										AvailableGenre a = new AvailableGenre();
										a.genre = genre + 1;
										GlobalVars.directorsGenres.Add(a);
									}
								}
								break;
							case "cameraman":
								//flag = false;
								foreach(AvailableGenre ag in GlobalVars.cameramansGenres)
								{
									if(ag.genre == genre)
									{
										if(staff.skills[i].skill > ag.max)
										{
											ag.max = staff.skills[i].skill;
										}
										//flag = true;
									}
								}
								if(staff.skills[i].skill >= 28 && !GlobalVars.cameramansGenres.Exists(delegate (AvailableGenre av)
								{	return av.genre == staff.skills[i].genre + 1;	}) )
								{
									{
										AvailableGenre a = new AvailableGenre();
										a.genre = genre + 1;
										GlobalVars.cameramansGenres.Add(a);
									}
								}
								break;
							}
							
						}
						if(staff.freeSkillPoints == 0)
						{
							SwitchState(PopUpIncrStaffLvlStates.showCharInfo);
						}	
						staff.RefreshStats();
					}
				}
			}
		}*/
	}
	
	public void SwitchState(PopUpIncrStaffLvlStates st)
	{
		switch(st)
		{
		case PopUpIncrStaffLvlStates.notActive:
			state = PopUpIncrStaffLvlStates.notActive;
			if(staff != null)
			{
				staff.charInfoState = CharInfoState.hide;
				staff.icon.transform.parent = staff.transform;
			}
			GlobalVars.cameraStates = CameraStates.normal;
			menu.SetActive(false);
			break;
		case PopUpIncrStaffLvlStates.increaseLvl:
			state = PopUpIncrStaffLvlStates.increaseLvl;
			menu.SetActive(true);
			break;
		case PopUpIncrStaffLvlStates.showCharInfo:
			state = PopUpIncrStaffLvlStates.showCharInfo;
			menu.SetActive(true);
			buttonsIncrSkills.SetActive(false);
			break;
		}
	}
}
