using UnityEngine;
using System.Collections;

//кнопка жанра
public class GenreButton : MonoBehaviour 
{
	public FilmGenres genre;				//жанр, который привязан к этой кнопке
	public GameObject button;
	public MeshRenderer checkMark;			//мешрендер галочки
	bool isChoosen = false;					//выбран/нет жанр
	
	GetTextFromFile textControl;
	public tk2dSprite icon;
	
	void Start()
	{
		textControl = GetComponentInChildren<GetTextFromFile>();
	}
	
	public void CheckAvailability()
	{
		if(textControl == null)
		{
			textControl = GetComponentInChildren<GetTextFromFile>();
		}
		
		
		if(textControl != null && GlobalVars.scriptersGenres.Exists(delegate (AvailableGenre ag)
		{	return genre == ag.genre;	}))
		{
			textControl.enabled = true;
			icon.color = Color.white;
		}
		else
		{
			icon.color = Color.black;
		}
	}
	
	void OnEnable()
	{
		CheckAvailability();
	}
	
	public bool GetChoosen()
	{
		return isChoosen;
	}
	
	public void SetChoosen(bool b)
	{
		isChoosen = b;
	}
	
	void Update () 
	{
		if(isChoosen && checkMark != null)
		{
			checkMark.enabled = true;
		}
		else if(!isChoosen && checkMark != null)
		{
			checkMark.enabled = false;
		}
	}
}
