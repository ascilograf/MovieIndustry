using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//управление плашки инфо персонажа, отслеживание прогресса уровня, 
//выставление звезд, свободных очков, имени персонажа, уровня, опыта, цены, аватара
public class CharInfo : MonoBehaviour 
{
	enum States
	{
		normal,
		waitForUp,
		showType,
		showPrice,
		showLvlUp
	}
	
	public GameObject character;						//персонаж
			
	public List<FilmSkills> filmSkills;					//умения
	public OtherSkills otherSkills;
	public tk2dTextMesh freeSkillPointsMesh;			//меш свободных очков
	public int freeSkillPoints;							//кол-во свободных очков
	
	//меши параметроы персонажа
	public tk2dTextMesh characterName;					
	public tk2dTextMesh characterLevel;	
	public tk2dTextMesh characterExp;
	public tk2dTextMesh characterPrice;
	public tk2dTextMesh timerMesh;
	public tk2dTextMesh finishUpNowPrice;
	
	public GameObject[] invItems;
	public GameObject timerParent;
	public GameObject bottomPrice;						//объект нижней цены
	public GameObject bottomType;
	public GameObject buttonHireStaff;					//кнопка покупки персонажа
	public GameObject buttonLvlUp;
	public GameObject buttonFinishLvlUp;
	public GameObject background;
	public GameObject smile;
	public tk2dTextMesh buttonCaption;
	
	//объекты прогрессбара опыта
	public tk2dUIProgressBar expProgressbar;
	
	public int priceToUse;
	public Transform avatar;							//парент-объект аватара
	public FilmStaff staff;								//FilmStaff персонажа
	
	bool haveGenres;
	float timeToUp;
	States state = States.normal;
	
	//по активации включаем приемник и обновляем параметры
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	//по деактивации выключаем приемник
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	//проверка нажатия по плюсику
	void CheckTap(GameObject go)
	{
		if(go == buttonLvlUp)
		{
			//gameObject.SetActive(false);
			//background.SetActive(true);
			Coroutiner.StartCoroutine(StaffManagment.LvlUpPlaneAnimation(transform, background, this, transform.localPosition.x, 1.5625f, 0.5f, 0));
		}
		else if(go == buttonFinishLvlUp)
		{
			if(GlobalVars.stars >= (int)(timeToUp - Time.time))
			{
				GlobalVars.stars -= (int)(timeToUp - Time.time);
				timeToUp = Time.time;
			}
		}
	}
	
	//первоначальная установка параметров персонажа
	public void SetParams(FilmStaff fs)
	{
		staff = fs;
		character = fs.gameObject;
		staff.CheckLvl();
		if(	character.GetComponent<PersonController>().type != CharacterType.postProdWorker &&
			character.GetComponent<PersonController>().type != CharacterType.producer)
		{
			haveGenres = true;
		}
		else
		{
			haveGenres = false;
		}
		
		SetNameLevel(fs.name, fs.lvl);
		CheckFreeSkillPoints();
		ChangeProgress(staff.exp, staff.lvl);
		InstantiateAvatar();
		if(haveGenres)
		{
			ChangeProgressBars(staff.mainSkill);
		}
		else
		{
			SetOtherSkillValue();
		}
		SetPricePerUse();
		if(buttonHireStaff != null)
		{
			buttonHireStaff.SetActive(false);
		}
		
		gameObject.SetActive(false);
	}
	
	//обновление параметров персонажа
	public void Refresh()
	{
		gameObject.SetActive(true);
		staff.CheckLvl();
		SetNameLevel(staff.name, staff.lvl);
		if(state != States.waitForUp)
		{
			CheckFreeSkillPoints();
			ChangeProgress(staff.exp, staff.lvl);
			if(haveGenres)
			{
				ChangeProgressBars(staff.mainSkill);
			}
			else
			{
				SetOtherSkillValue();
			}
			SetPricePerUse();
			if(buttonHireStaff != null)
			{
				buttonHireStaff.SetActive(false);
			}

			bottomType.SetActive(false);
			timerParent.SetActive(false);
			buttonFinishLvlUp.SetActive(false);
			buttonLvlUp.SetActive(false);
		}
		else if(state == States.waitForUp)
		{
			ShowTimer(true);
		}
	}
	
	public bool HaveGenres
	{
		get
		{	return haveGenres; }
	}
	
	public void ShowButton()
	{
		//if(staff.freeSkillPoints == 0)
		//{
			buttonHireStaff.SetActive(true);
			bottomPrice.SetActive(false);
			bottomType.SetActive(false);
			buttonLvlUp.SetActive(false);
		//}
	}
	
	public void ShowLvlUpButton()
	{
		//if(staff.freeSkillPoints == 0)
		//{
			buttonHireStaff.SetActive(false);
			bottomPrice.SetActive(false);
			bottomType.SetActive(false);
			buttonLvlUp.SetActive(true);
		//}
	}
	
	//установка жанров и умения для каждого из основных умений персонажа
	public void ChangeProgressBars(List<StaffSkills> skills)
	{
		for(int i = 0; i < 3; i++)
		{
			if(i < skills.Count)
			{
				filmSkills[i].genre = skills[i].genre;
				filmSkills[i].skill = skills[i].skill;
				SetSkillsValue(filmSkills[i]);
			}
			else
			{
				filmSkills[i].skill = 0;
				SetSkillsValue(filmSkills[i]);
			}
		}
	}
	
	void SetOtherSkillValue()
	{
		foreach(FilmSkills fs in filmSkills)
		{
			fs.skill = 0;
			for(int i = 0; i < 5; i++)
			{
				DeactivateStarExcept(fs.progressBar[i], "");
			}
			fs.skillNameMesh.text = "";
			fs.skillNameMesh.Commit();
			//fs.plusButton.gameObject.SetActive(false);
		}
		
		int skill = otherSkills.skill;
		for(int i = 0; i < 5; i++)
		{
			if((skill - 20) > 0)
			{	
				DeactivateStarExcept(otherSkills.progressBar[i], "5state");
			}
			else if(skill < 1)
			{
				DeactivateStarExcept(otherSkills.progressBar[i], "empty");
			}
			else if(skill < 5)
			{
				DeactivateStarExcept(otherSkills.progressBar[i], "1state", otherSkills.progressBar[i].transform);
			}
			else if(skill < 9)
			{
				DeactivateStarExcept(otherSkills.progressBar[i], "2state", otherSkills.progressBar[i].transform);
			}
			else if(skill < 13)
			{
				DeactivateStarExcept(otherSkills.progressBar[i], "3state", otherSkills.progressBar[i].transform);
			}
			else if(skill < 17)
			{
				DeactivateStarExcept(otherSkills.progressBar[i], "4state", otherSkills.progressBar[i].transform);
			}
			else if(skill < 21)
			{
				DeactivateStarExcept(otherSkills.progressBar[i], "5state", otherSkills.progressBar[i].transform);
			}
			skill -= 20;
		}
		Utils.SetText(otherSkills.skillNameMesh, "Visuals");
		//otherSkills.skillNameMesh.Commit();
		//otherSkills.plusButton.transform.localPosition = new Vector3(130, -3, -2);
	}
	
	//установка нужного спрайта звезды в зависимости от уровня в жанре, если уровень в жанре равен нулю, то скрываем звезды, название жанра и кнопку поднятия уровня
	void SetSkillsValue(FilmSkills fs)
	{
		for(int i = 0; i < 5; i++)
		{
			DeactivateStarExcept(otherSkills.progressBar[i], "");
		}
		otherSkills.skillNameMesh.text = "";
		otherSkills.skillNameMesh.Commit();
		
		if(fs.skill > 0)
		{
			int skill = fs.skill;
			for(int i = 0; i < 5; i++)
			{
				if((skill - 20) > 0)
				{	
					DeactivateStarExcept(fs.progressBar[i], "5state");
				}
				else if(skill < 1)
				{
					DeactivateStarExcept(fs.progressBar[i], "empty");
				}
				else if(skill < 5)
				{
					DeactivateStarExcept(fs.progressBar[i], "1state", fs.progressBar[i].transform);
				}
				else if(skill < 9)
				{
					DeactivateStarExcept(fs.progressBar[i], "2state", fs.progressBar[i].transform);
				}
				else if(skill < 13)
				{
					DeactivateStarExcept(fs.progressBar[i], "3state", fs.progressBar[i].transform);
				}
				else if(skill < 17)
				{
					DeactivateStarExcept(fs.progressBar[i], "4state", fs.progressBar[i].transform);
				}
				else if(skill < 21)
				{
					DeactivateStarExcept(fs.progressBar[i], "5state", fs.progressBar[i].transform);
				}
				skill -= 20;
			}
			fs.skillNameMesh.text = fs.genre.ToString();
			fs.skillNameMesh.Commit();
			//fs.plusButton.transform.localPosition = new Vector3(130, -3, -2);
		}
		else
		{
			for(int i = 0; i < 5; i++)
			{
				DeactivateStarExcept(fs.progressBar[i], "");
			}
			fs.skillNameMesh.text = "";
			fs.skillNameMesh.Commit();
			//fs.plusButton.gameObject.SetActive(false);
		}
	}
	
	//деактивация звезд кроме выбранный
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
	
	//инициализация аватара
	void InstantiateAvatar()
	{
		PersonController pc = staff.GetComponent<PersonController>();
		if(pc.head != null && pc.body != null & pc.legs != null)
		{
			InstPartOfAvatar(pc.head.gameObject);
			InstPartOfAvatar(pc.body.gameObject);
			InstPartOfAvatar(pc.legs.gameObject);
			gameObject.SetActive(false);
		}
	}
	
	//инициализация части аватара, установка парента, позиции, масштаба
	void InstPartOfAvatar(GameObject go)
	{
		GameObject g = Instantiate(go) as GameObject;
		g.transform.parent = avatar;
		g.transform.localPosition = new Vector3(0,0, go.transform.localPosition.z-5);
		g.transform.localScale = new Vector3(0.6f,0.6f,0.6f);
		g.transform.localEulerAngles = Vector3.zero;
		g.GetComponent<tk2dSpriteAnimator>().playAutomatically = false;
		g.GetComponent<tk2dSpriteAnimator>().Play("idle");
		g.GetComponent<tk2dSpriteAnimator>().Stop();
		g.layer = 10;
	}
	
	//есть ли свободные очки, если есть - активируем плюсики
	void CheckFreeSkillPoints()
	{
		freeSkillPointsMesh.text = staff.freeSkillPoints.ToString();
		if(haveGenres)
		{
			if(staff.freeSkillPoints > 0)
			{
				buttonLvlUp.SetActive(true);
				bottomPrice.SetActive(false);
				bottomType.SetActive(false);
			}
			else
			{
				buttonLvlUp.SetActive(false);
			}
			/*foreach(FilmSkills fs in filmSkills)
			{
				if(staff.freeSkillPoints > 0)
				{
					fs.plusButton.gameObject.SetActive(true);
				}
				else
				{
					fs.plusButton.gameObject.SetActive(false);
				}
			}*/
		}
		if(!haveGenres)
		{
			if(staff.freeSkillPoints > 0)
			{
				buttonLvlUp.SetActive(true);
				bottomPrice.SetActive(false);
				bottomType.SetActive(false);
				//otherSkills.plusButton.gameObject.SetActive(true);
			}
			else
			{
				//otherSkills.plusButton.gameObject.SetActive(false);
				buttonLvlUp.SetActive(false);
			}
		}
	}
	
	public void ShowBottomType()
	{
		//if(staff.freeSkillPoints == 0)
		//{
			buttonHireStaff.SetActive(false);
			bottomPrice.SetActive(false);
			buttonLvlUp.SetActive(false);
			buttonFinishLvlUp.SetActive(false);
			GameObject icon = bottomType.transform.FindChild(staff.tag).gameObject;
			bottomType.SetActive(false);
			icon.SetActive(true);
		//}
	}
	
	//установка имени и уровня
	void SetNameLevel(string name, int lvl)
	{
		characterName.text = name;
		characterName.Commit();
		characterLevel.text = lvl.ToString();
		characterLevel.Commit();
	}
	
	//определение прогресса в процентах
	float ProgressInPercents(int e, int l)
	{
		int percents = e / ((100*(l + 1))/100);
		return percents;
	}
	
	//изменение прогрессбара опыта
	void ChangeProgress(int e, int l)
	{
		characterExp.text = e + " / " + (100*(l + 1));
		expProgressbar.Value = ProgressInPercents(e,l) / 100;
	}
	
	int lvl;
	int exp;
	
	public void GainExp(int gainedExp)
	{
		lvl = staff.lvl;
		exp = staff.exp;
		StartCoroutine(ChangeProgressAnimation(staff.exp, staff.exp + gainedExp));
	}
	
	IEnumerator ChangeProgressAnimation(int curr, int next, float t = 0)
	{
		while(true)
		{
			if(t >= 2 || Input.GetMouseButton(0))
			{
				exp = next;
				if((staff.lvl + 1) * 100 < exp)
				{
					exp = (curr + (next - curr)) - (lvl + 1) * 100;
					lvl++;
					StartCoroutine(AnimateLvl());	
				}
				ChangeProgress(exp, lvl);
				Utils.SetText(characterLevel, lvl.ToString());
				yield break;
			}
			exp = (int)Mathf.Lerp(curr, next, t / 2);
			
			if((lvl + 1) * 100 < exp)
			{
				next = (curr + (next - curr)) - (lvl + 1) * 100;
				curr = 0;
				t = 0;
				StartCoroutine(AnimateLvl());
				lvl++;
			}
			ChangeProgress(exp, lvl);
			Utils.SetText(characterLevel, lvl.ToString());
			t += Time.deltaTime;
			yield return 0;
		}
	}
	
	public void CommitLvl()
	{
		staff.lvl += 50;
		staff.exp += 50;
		staff.CheckLvl();
	}
	
	IEnumerator AnimateLvl(float t = 0)
	{
		while(true)
		{
			if(t >= 2 || Input.GetMouseButton(0))
			{
				characterLevel.transform.localScale = Vector3.one;
				yield break;
			}
			characterLevel.transform.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.one, t/2);
			t += Time.deltaTime;
			yield return 0;
		}
	}
	
	public int GetPrice(List<FilmGenres> genres)
	{
		priceToUse = 0;
		if(haveGenres)
		{
			priceToUse = Formulas.StaffPrice(this, genres);
		}
		return priceToUse;
	}
	
	public int GetPrice()
	{
		priceToUse = 0;
		priceToUse = otherSkills.skill * 10000;
		return priceToUse;
	}
	
	//установка цены за использование
	public void SetPricePerUse(List<FilmGenres> genres = null)
	{
		if(haveGenres && genres != null)
		{
			GetPrice (genres);
		}
		else if(haveGenres)
		{
			GetPrice();
		}
		else
		{
			GetPrice();
		}
		buttonHireStaff.SetActive(false);
		buttonLvlUp.SetActive(false);
		bottomType.SetActive(false);
		bottomPrice.gameObject.SetActive(true);
		characterPrice.text = Utils.ToNumberWithSpaces(priceToUse.ToString()); 
		buttonCaption.text = priceToUse.ToString();
	}
	
	public void SetParamsForUpSkill(int timeToLearn, bool b = true)
	{
		if(b)
		{
			timeToUp = Time.time + timeToLearn;
			state = States.waitForUp;
			Refresh();
			
		}
		else
		{
			Refresh();
			ShowLvlUpButton();
		}
	}
	
	void ShowTimer(bool b)
	{
		timerParent.SetActive(b);
		/*foreach(GameObject g in invItems)
		{
			g.SetActive(false);
		}*/
		
		foreach(FilmSkills fs in filmSkills)
		{
			fs.skill = 0;
			for(int i = 0; i < 5; i++)
			{
				DeactivateStarExcept(fs.progressBar[i], "");
			}
			fs.skillNameMesh.text = "";
			fs.skillNameMesh.Commit();
			//fs.plusButton.gameObject.SetActive(false);
		}
		
		for(int i = 0; i < 5; i++)
		{
			DeactivateStarExcept(otherSkills.progressBar[i], "");
		}
		otherSkills.skillNameMesh.text = "";
		otherSkills.skillNameMesh.Commit();
		//otherSkills.plusButton.gameObject.SetActive(false);
		freeSkillPointsMesh.transform.parent.gameObject.SetActive(false);
		
		bottomPrice.SetActive(false);
		bottomType.SetActive(false);
		buttonHireStaff.SetActive(false);
		buttonLvlUp.SetActive(false);
		buttonFinishLvlUp.SetActive(true);
		staff.canBeUsed = false;
	}
	
	void Update()
	{
		if(state == States.waitForUp)
		{
			Utils.SetText(timerMesh, Utils.FormatIntToUsualTimeString((int)(timeToUp - Time.time)));
			Utils.SetText(finishUpNowPrice, Utils.ToNumberWithSpaces(((int)(timeToUp - Time.time)).ToString()));
			staff.canBeUsed = false;
			if(Time.time >= timeToUp)
			{
				if(haveGenres)
				{
					foreach(FilmSkills fs in filmSkills)
					{
						foreach(StaffSkills s in staff.mainSkill)
						{
							if(s.genre == fs.genre)
							{
								s.skill += fs.skillToUp;
								fs.skillToUp = 0;
							}
						}
					}
				}
				else
				{
					otherSkills.skill += otherSkills.skillToUp;
					otherSkills.skillToUp = 0;
				}
				staff.canBeUsed = true;
				bottomPrice.SetActive(true);
				state = States.normal;
				Refresh();
				if(staff.freeSkillPoints > 0)
				{
					ShowLvlUpButton();
				}
			}
		}
	}
}
