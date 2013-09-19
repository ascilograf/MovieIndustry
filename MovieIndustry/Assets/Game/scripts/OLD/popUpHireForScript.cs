using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HireForScriptStates
{
	selectGenre,
	selectStaff,
	selectSetting,
}

public class popUpHireForScript : MonoBehaviour 
{
	public tk2dTextMesh time;
	public List<Scripter> availableScripters;
	public List<Scripter> scripters;
	public List<FilmGenres> genres;
	SwipeItems swipe;
	
	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}
	
	/*void RefreshAvailableScripters(FilmGenres genre)
	{
		Messenger.Broadcast("Refresh Scripters List");
		Scripter temp = null;
		//DestroyPageControl();
		bool flag = false;
		availableScripters.Clear();//scriptersUIList.ClearList(false);
		swipe.items.Clear();
		swipe.itemIndex = 0;
		//scripterIndex = 0;
		float f = 0;
		for(int i = 0; i < GlobalVars.allScripters.Count; i++)
		{
			for(int j = 0; j < genres.Count; j++)
			{
				for(int k = 0; k < GlobalVars.allScripters[i].skills.Length; k++)
				{
					if(genres[j] == GlobalVars.allScripters[i].skills[k].genre)
					{
						if(GlobalVars.allScripters[i].skills[k].skill > 0)
						{
							temp = GlobalVars.allScripters[i];
						}
						else
						{
							if(availableScripters.Exists(delegate(Scripter sc)
							{ return sc ==  GlobalVars.allScripters[i];}))
							{
								availableScripters.Remove(GlobalVars.allScripters[i]);
								if(availableScripters.Count <= 0)
								{
								//	ShowHideTransform(GlobalVars.allScripters[i].icon, false);
									//ShowHideTransform(currentScripter.gameObject, false);
									//currentScripter = null;
									//currScripterPrice.text = "";
									//currScripterPrice.Commit();
									//ShowHideTransform(accept, false);//ClearParams();
								}
							}
							temp = null;
							flag = true;
						}
					}
					if(flag) 
					{	
						//flag = false;
						break;	
					}
				}
				if(temp != null)
				{
					if(!availableScripters.Exists(delegate(Scripter sc)
					{ return sc == temp;}))
					{
						availableScripters.Add(temp);
						//ShowHideTransform(availableScripters[0].icon, true);
						//currentScripter = availableScripters[0];
						//currScripterPrice.text = (currentScripter.lvl * 1000).ToString();
						//CheckPageControl(currentScripter.gameObject);
						//currScripterPrice.Commit();
						//ShowHideTransform(accept, true);
					}
				}
			}
		}
		for(int i = 0; i < availableScripters.Count; i++)
		{
			availableScripters[i].icon.transform.localPosition = new Vector3(-27 + 607 * i, -27, -1);
			availableScripters[i].icon.SetActiveRecursively(true);
			if(!swipe.items.Exists(delegate(GameObject g)
			{ return g == availableScripters[i].gameObject;}))
			{
				swipe.items.Add(availableScripters[i].gameObject);
			}
		}
		//if(availableScripters.Count > 0 && currentScripter !=null)
		//{
		//	InstPageControl(availableScripters);
		//	CheckPageControl(currentScripter.gameObject);
		//}
		//else if(availableScripters.Count <= 0)
		//{
		//	HideAllAvailableScripters();
		//}
	}*/
}
