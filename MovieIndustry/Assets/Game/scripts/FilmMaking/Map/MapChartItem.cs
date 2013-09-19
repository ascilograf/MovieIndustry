using UnityEngine;
using System.Collections;

public class MapChartItem : MonoBehaviour 
{
	public FilmItem film;
	public tk2dTextMesh filmName;
	public tk2dTextMesh filmLifeTime;
	public tk2dTextMesh filmBudgetTaken;
	public bool isMoving;
	
	public void SetParams(FilmItem f)
	{
		film = f;
		Utils.SetText(filmName, film.name);
		Utils.SetText(filmBudgetTaken, film.budgetTaken.ToString());
		if(film.lifeTime > 0)
			Utils.SetText(filmLifeTime, film.lifeTime.ToString());
		else
			Utils.SetText(filmLifeTime, "");
	}
	
	public void RefreshParams()
	{
		if(film != null)
		{
			Utils.SetText(filmName, Utils.FormatStringToText(film.name, 13));
			Utils.SetText(filmBudgetTaken, Utils.ToNumberWithSpaces(film.budgetTaken.ToString()));
			if(film.lifeTime > 0)
				Utils.SetText(filmLifeTime, film.lifeTime.ToString());
			else
				Utils.SetText(filmLifeTime, "");
		}
		else
		{
			filmName.text = "";
			filmBudgetTaken.text = "";
			filmLifeTime.text = "";
		}
	}
	
	public void ClearItem()
	{
		Utils.SetText(filmName, "");
		Utils.SetText(filmBudgetTaken, "");
		Utils.SetText(filmLifeTime, "");
	}
}
