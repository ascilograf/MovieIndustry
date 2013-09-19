using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//отслеживание изменения достижений, выдача наград по достижениям
public class Achievments : MonoBehaviour {

	public List<AchievmentsTypes> achievments;			//массив достижений с параметрами
	
	int moviesReleased;			
	int regionsUnlocked;
	
	float time;
	
	void Awake()
	{
		GlobalVars.achievments = this;
	}

	//каждые 0.3 секунды происходит проверка на выполнение условий достижений
	void Update () 
	{
		if(time > 0)
		{
			time -= Time.deltaTime;
		}
		else
		{
			CheckActorComedy();
			CheckBuild();
			CheckMainOfficeUpgr();
			CheckMoviesReleased();
			CheckRegionsUnlocked();
			time = 0.3f;
		}
	}
	
	public void IncrMoviesReleased()
	{
		moviesReleased++;
	}
	
	public void IncrRegionsUnlocked()
	{
		regionsUnlocked++;
	}
	
	//находим на сцене мэйн офис, если он есть, то проверяем его этаж
	//в зависимости от этажа проверяем есть ли повышение уровня, если есть - выдаем награду.
	void CheckMainOfficeUpgr()
	{
		GameObject go = GameObject.FindGameObjectWithTag("Offices");
		if(go != null)
		{
			Construct office = go.transform.parent.GetComponent<Construct>();
			if(office != null)
			{
				int stage = (int)office.stage + 1;
				foreach(AchievmentsTypes type in achievments)
				{
					if(type.name == "a_mainOfficeUpgrade")
					{
						if(stage >= 5 && type.lvl < 3)
						{
							type.lvl = 3;
							GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
						}
						if(stage >= 3 && type.lvl < 2)
						{
							type.lvl = 2;
							GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
						}
						if(stage == 2 && type.lvl < 1)
						{
							type.lvl = 1;
							GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
						}
					}
				}
			}
		}
	}
	
	//находим на сцене ангар, трэйлер, постпродОфис, если они есть, то выставляем булевые переменные
	//в зависимости от их комбинации, если был достигнут новый уровень достижения - выдаем награду.
	void CheckBuild()
	{
		bool hangar, trailer, postProd;
		hangar = false;
		trailer = false;
		postProd = false;
		GameObject go = null;
		go = GameObject.FindGameObjectWithTag("Pavillion");
		if(go != null)
		{
			if(go.GetComponent<Construct>() != null)	
				hangar = true;
		}
		go = GameObject.FindGameObjectWithTag("Trailer");
		if(go != null)
		{
			if(go.GetComponent<Construct>() != null)
				trailer = true;
		}
		go = GameObject.FindGameObjectWithTag("postProdOffice");
		if(go != null)
		{
			if(go.GetComponent<Construct>() != null)
				postProd = true;
		}
		foreach(AchievmentsTypes type in achievments)
		{
			if(type.name == "a_build")
			{
				if(hangar && trailer && postProd && type.lvl < 3)
				{
					type.lvl = 3;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
				else if(hangar && trailer && type.lvl < 2)
				{
					type.lvl = 2;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
				else if(hangar && type.lvl < 1)
				{
					type.lvl = 1;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
			}
		}
	}
	
	//проверяем кол-во выпущенных фильмов.
	void CheckMoviesReleased()
	{
		foreach(AchievmentsTypes type in achievments)
		{
			if(type.name == "a_moviesReleased")
			{
				if(moviesReleased >= 100 && type.lvl < 3)
				{
					type.lvl = 3;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
				else if(moviesReleased >= 10 && type.lvl < 2)
				{
					type.lvl = 2;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
				else if(moviesReleased >= 5 && type.lvl < 1)
				{
					type.lvl = 1;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
			}
		}
	}
	
	//проверяем кол-во навыка комедии для актера.
	void CheckActorComedy()
	{
		AvailableGenre skill = GlobalVars.actorsGenres.Find(delegate(AvailableGenre ag)
		{return ag.genre == FilmGenres.Comedy ;});
		foreach(AchievmentsTypes type in achievments)
		{
			if(type.name == "a_actorComedy")
			{
				if(skill.max >= 100 && type.lvl < 3)
				{
					type.lvl = 3;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
				else if(skill.max >= 60 && type.lvl < 2)
				{
					type.lvl = 2;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
				else if(skill.max >= 20 && type.lvl < 1)
				{
					type.lvl = 1;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
				
				
			}
		}
	}
	
	//проверка кол-ва открытых регионов
	public void CheckRegionsUnlocked()
	{
		foreach(AchievmentsTypes type in achievments)
		{
			if(type.name == "a_regionsUnlocked")
			{
				if(regionsUnlocked >= 5 && type.lvl < 3)
				{
					type.lvl = 3;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
				if(regionsUnlocked >= 3 && type.lvl < 2)
				{
					type.lvl = 2;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
				if(regionsUnlocked == 2 && type.lvl < 1)
				{
					type.lvl = 1;
					GetProfit(type.achievmentsProfit[type.lvl - 1].stars, type.achievmentsProfit[type.lvl - 1].expToStudio);
				}
			}
		}
	}
	
	//получение награды
	void GetProfit(int stars, int exp)
	{
		GlobalVars.stars += stars;
		GlobalVars.exp += exp;
	}
}
