using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//скрипт-пустышка для переменных сценария
public class Scenario : MonoBehaviour 
{
	public List<FilmGenres> genres;					//жанры
	public int numberOfScriptWritters;				//кол-во сценаристов
	public int story;								//качество сценария
	public int price;								//цена
	public float time;								//время написания
	public int numberOfScenes;						//кол-во сцен
	public Setting setting;							//сеттинг
	public MeshRenderer activeIcon;
	
	public GameObject icon;
	public tk2dTextMesh textGenres;
	public tk2dTextMesh textTitle;
	public tk2dTextMesh text;
	public GameObject[] stars;
	
	void Start () 
	{
		RefreshInfo();
		icon.SetActive(false);
	}
	
	public void TakeCharInfoBack()
	{
		icon.transform.parent = transform;
		icon.SetActive(false);
	}
	
	public string GetGenres()
	{
		string s = "";
		for(int i = 0; i < genres.Count; i++)
		{
			s += genres[i].ToString();
			if(genres.Count == 2 && i == 0)
			{
				s += ", ";
			}
		}
		return s;
	}
	
	public void RefreshInfo()
	{
		textTitle.text = Utils.FormatStringToText(gameObject.name, 15);
		
		textGenres.text = "";
		for(int i = 0; i < genres.Count; i++)
		{
			textGenres.text += genres[i].ToString();
			if(genres.Count == 2 && i == 0)
			{
				textGenres.text += ", ";
			}
		}
		
		int skill = story;
		for(int i = 0; i < stars.Length; i++)
		{
			if((skill - 20) > 0)
			{	
				DeactivateStarExcept(stars[i], "5state");
			}
			else if(skill < 1)
			{
				DeactivateStarExcept(stars[i], "empty");
			}
			else if(skill < 5)
			{
				DeactivateStarExcept(stars[i], "1state", stars[i].transform);
			}
			else if(skill < 9)
			{
				DeactivateStarExcept(stars[i], "2state", stars[i].transform);
			}
			else if(skill < 13)
			{
				DeactivateStarExcept(stars[i], "3state", stars[i].transform);
			}
			else if(skill < 17)
			{
				DeactivateStarExcept(stars[i], "4state", stars[i].transform);
			}
			else if(skill < 21)
			{
				DeactivateStarExcept(stars[i], "5state", stars[i].transform);
			}
			skill -= 20;
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
}
