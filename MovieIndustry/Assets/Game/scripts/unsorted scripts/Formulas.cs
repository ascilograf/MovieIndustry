using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//формулы механик

public static class Formulas {
	
	//находим значение по имени переменной
	static float FindFormulaVariable(string variable)
	{
		foreach(ImportedString impStr in Textes.formulasVariables)
		{
			if(impStr.caption == variable)
			{
				return float.Parse(impStr.text);
			}
		}
		return 0;
	}
	
	public static int FirstWeekEndMoney(FilmItem film)
	{
		float divisor = FindFormulaVariable("weekendBoxOfficeDivisor");
		int money = 0;
		money = Mathf.RoundToInt(film.budget + (film.budget/ divisor * GlobalVars.worldRental.GetPercentsFromCinemas()));
		money += (int)((money / 100) * film.firstTypeMarketingBonus);
		film.budgetTaken = money;
		return money;
	}
	
	public static int StaffPrice(CharInfo staff, List<FilmGenres> genres)
	{
		float factor = FindFormulaVariable("staffPriceFactor");
		int money = 0;
		money = 0;
		foreach(FilmGenres fg in genres)
		{
			for(int j = 0; j < staff.filmSkills.Count; j++)
			{
				if(staff.filmSkills[j].genre == fg)
				{
					money += staff.filmSkills[j].skill;
				}
			}
		}
		money = (int)(money * factor);
		return money;
	}
	
	//определение скилла в фильм, по каждому из жанров определяем совпадение в скиллах персонажей и подсчитываем сумму по всем персонажам
	public static int FilmActingDirectiogCinematography(List<FilmStaff> staff, List<FilmGenres> genres, string variableName)
	{
		float factor = FindFormulaVariable(variableName);
		int skill = 0;
		foreach(FilmGenres fg in genres)
		{
			for(int i = 0; i < staff.Count; i++)
			{
				CharInfo info = staff[i].icon.GetComponent<CharInfo>();
				for(int j = 0; j < info.filmSkills.Count; j++)
				{
					if(info.filmSkills[j].genre == fg)
					{ 
						skill += info.filmSkills[j].skill;
					}
				}
			}
		}
		skill = (int)(skill / factor);
		return skill;
	}
	
	//то же что и выше, но для одного персонажа
	public static int FilmActingDirectiogCinematography(FilmStaff staff, List<FilmGenres> genres, string variableName) 
	{
		float divisor = FindFormulaVariable(variableName);
		int skill = 0;
		foreach(FilmGenres fg in genres)
		{
			CharInfo info = staff.icon.GetComponent<CharInfo>();
			for(int j = 0; j < info.filmSkills.Count; j++)
			{
				if(info.filmSkills[j].genre == fg)
				{
					skill += info.filmSkills[j].skill;
				}
			}
		}
		skill = (int)(skill / divisor);
		return skill;
	}
	
	public static int LvlUpCost(int skill)
	{
		return skill * 10000;
	}
	
	public static int LvlUpTime(int skill)
	{
		return skill * 60;
	}
	
	public static int SetRevenuePerMinForRental(FilmItem film)
	{
		return (film.acting + film.direction + film.cinematography + film.story) * 2;
	}
	
	public static float GetChanceToBonus(FirstTypeMarketing m)
	{
		if(m.failChance == 0)
		{
			return 0;
		}
		int rand = Random.Range(0, 101);
		if(rand <= m.failChance)
		{
			return 0;
		}
		else
		{
			rand -= m.failChance;
		}
		if(rand <= m.zeroChance)
		{
			return m.price;
		}
		else
		{
			rand -= m.zeroChance;
		}
		if(rand <= m.bonusChance)
		{
			return m.bonus;
		}
		float f = 0;
		return f;
	}
}
