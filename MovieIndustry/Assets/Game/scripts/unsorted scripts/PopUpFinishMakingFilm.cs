using UnityEngine;
using System.Collections;

//контроллер финального окна фильма, на вход фильм.
//при вызове начальной функции заполняются текстовые поля, по кнопке коллект фильм отправляется в чарт

public class PopUpFinishMakingFilm : MonoBehaviour 
{
	public GameObject collectButton;
	public Camera swipeCam;
	public Transform progresbars;
	
	//изменяемые надписи
	public tk2dTextMesh meshGenres;
	public tk2dTextMesh meshRating;
	public tk2dTextMesh meshFilmName;
	public tk2dTextMesh meshFirstWeek;
	public tk2dTextMesh meshRunTime;

	
	//progressbars
	public GameObject[] rating;
	public GameObject[] acting;
	public GameObject[] cameraWork;
	public GameObject[] story;
	public GameObject[] visuals;
	public GameObject[] direction;
	
	public float minLength;
	public float maxLength;
	
	FilmItem film;
	
	void OnEnable()
	{
		GlobalVars.soundController.FinishMakingFilmWindow();
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject go)
	{
		if(go == collectButton)
		{
			GlobalVars.worldRental.AddFilmAtRental(film);
			gameObject.SetActive(false);
			Utils.ChangeMoney(Formulas.FirstWeekEndMoney(film));
			GlobalVars.worldRental.ShowMenu();
			Messenger<FilmItem>.Broadcast("releaseFilm", film);
			GlobalVars.soundController.CollectMoney();
			//GlobalVars.cameraStates = CameraStates.normal;
		}
	}
	
	public void SetParams(FilmItem f)
	{
		float h = Screen.height;	
		gameObject.SetActive(true);
		GlobalVars.cameraStates = CameraStates.menu;
		film = f;
		SetGenres();
		Utils.SetText(meshRunTime, Utils.FormatIntToUsualTimeString(film.lifeTime, 2));
		SetProgressBarValue(rating, film.story);
		SetProgressBarValue(acting, film.acting);
		SetProgressBarValue(cameraWork, film.cinematography);
		SetProgressBarValue(story, film.story);
		SetProgressBarValue(visuals, film.visuals);
		SetProgressBarValue(direction, film.direction);
		Utils.SetText(meshFilmName, film.name);
		Utils.SetText(meshFirstWeek, Formulas.FirstWeekEndMoney(film).ToString());
		swipeCam.orthographicSize = h / 2;
	}
	
	void SetGenres()
	{
		string s = "";
		for(int i = 0; i < film.genres.Count; i++)
		{
			s += film.genres[i].ToString();
			if(i != film.genres.Count - 1)
			{
				s += ", ";
			}
		}
		meshGenres.text = s;
		meshGenres.Commit();
	}
	
	void SetProgressBarValue(GameObject[] progressbar, int skill)
	{
		for(int i = 0; i < 5; i++)
		{
			if((skill - 20) > 0)
			{	
				DeactivateStarExcept(progressbar[i], "5state");
			}
			else if(skill < 1)
			{
				DeactivateStarExcept(progressbar[i], "empty");
			}
			else if(skill < 5)
			{
				DeactivateStarExcept(progressbar[i], "1state", progressbar[i].transform);
			}
			else if(skill < 9)
			{
				DeactivateStarExcept(progressbar[i], "2state", progressbar[i].transform);
			}
			else if(skill < 13)
			{
				DeactivateStarExcept(progressbar[i], "3state", progressbar[i].transform);
			}
			else if(skill < 17)
			{
				DeactivateStarExcept(progressbar[i], "4state", progressbar[i].transform);
			}
			else if(skill < 21)
			{
				DeactivateStarExcept(progressbar[i], "5state", progressbar[i].transform);
			}
			skill -= 20;
		}
	}
	
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
	
	//отработка свайпа
	void Update()
	{
		//MoveButtonsOnTap();
	}
	
	float dist = 0;
	
	//свайп элементов
	void MoveButtonsOnTap()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Vector3 v3 = Input.mousePosition;
			v3.y -= progresbars.localPosition.y;
			dist = v3.y;
		}
		
		if(Input.GetMouseButton(0))
		{
			Vector3 v3 = Input.mousePosition;
			v3.y -= dist;
			if(v3.y < minLength)
			{
				v3.y = minLength;
			}
			if(v3.y > maxLength)
			{
				v3.y = maxLength;
			}
			v3.z = progresbars.localPosition.z;
			v3.x = progresbars.localPosition.x;
			progresbars.localPosition = v3;
		}
		if(Input.GetMouseButtonUp(0))
		{
			dist = 0;
		}
	}
}
