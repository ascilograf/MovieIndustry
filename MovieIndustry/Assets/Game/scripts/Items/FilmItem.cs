using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//итем фильма, в него помещаются и из него берутся параметры во время создания и продажи фильма
public class FilmItem : MonoBehaviour 
{
	public List<FilmGenres> genres;		
	public List<Setting> settings;	
	public List<RarityLevel> rarityOfScenes;
	public int story;
	public int budget;
	public float budgetTaken;
	public float budgetToTake;
	public int acting;
	public int direction;
	public int cinematography;
	public float fit;
	public int visuals;
	public int lifeTime;
	public int Revenue;
	public int revemuePerSec;
	
	public float firstTypeMarketingBonus;
	public SecondTypeMarketing secondTypeMarketing;
	
	public bool busy;
	public GameObject icon;
	public MeshRenderer activeIcon;
	public tk2dTextMesh textGenres;
	public tk2dTextMesh textTitle;
	
	public GameObject[] stars;
	
	void Start () 
	{
		RefreshInfo();
	}
	
	public void TakeCharInfoBack()
	{
		icon.transform.parent = transform;
		icon.SetActive(false);
	}
	
	void Onenable()
	{
		RefreshInfo();
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
		textTitle.text = gameObject.name;
		
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
	
	
	/*public void RefreshParams()
	{
		actingMesh.text = "Acting: " + acting;
		budgetMesh.text = "Budget: " + budget;
		storyMesh.text = "Story: " + story;
		directionMesh.text = "Direction: " + direction;
		visualsMesh.text = "Visuals: " + visuals;
		cinematographyMesh.text = "Cinematography: " + cinematography;
		actingMesh.Commit();
		budgetMesh.Commit();
		storyMesh.Commit ();
		directionMesh.Commit();
		visualsMesh.Commit();
		cinematographyMesh.Commit();
	}*/
}
