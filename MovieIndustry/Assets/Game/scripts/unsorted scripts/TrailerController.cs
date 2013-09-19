using UnityEngine;
using System.Collections;
using System.Collections.Specialized;

//на старте, если переменная персонаж пуста - ищем сначала актера без трейлера, если таковых нет - тогда так же ищем режиссера

public class TrailerController : MonoBehaviour {
	
	public int lvl;
	public FilmStaff staff;
	public Doors door;
	public int[] stagesHeight;
	
	
	
	// Use this for initialization
	void Start () 
	{
		if(staff == null)
		{
			GameObject[] actors = GameObject.FindGameObjectsWithTag("actor");
			foreach(GameObject g in actors)
			{
				FilmStaff fs = g.GetComponent<FilmStaff>();
				if(fs.trailer == null)
				{
					fs.trailer = this;
					staff = fs;
					return;
				}
			}
			
			GameObject[] directors = GameObject.FindGameObjectsWithTag("director");
			foreach(GameObject g in directors)
			{
				FilmStaff fs = g.GetComponent<FilmStaff>();
				if(fs.trailer == null)
				{
					fs.trailer = this;
					staff = fs;
					return;
				}
			}
		}	
	}

}
