using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//список статик-методов для удобства работы с персонажами

public static class StaffManagment 
{
	
	//не используется
	public static void SetStaffFree(List<PersonController> persons, Doors[] door)
	{
		for(int i = 0; i < persons.Count; i++)
		{
			//PersonController pc = workers[i].GetComponent<PersonController>();
			Vector3 v3 = persons[i].transform.position;
			v3.y = GlobalVars.buildedFloorsHeight[persons[i].GetFloor()].yMax;
			persons[i].transform.position = v3;
			//Doors[] door = GetComponentsInChildren<Doors>();
			for(int j = 0; j < door.Length; j++)
			{
				if(persons[i].GetFloor() != 0)
				{
					persons[i].doors.Add(door[j]);
				}
				if(persons[i].GetFloor() == 0 && door[j].door == DoorsLocation.exitDoor)
				{
					persons[i].MoveToDoor(door[j].transform);
				}
				else if(persons[i].GetFloor() + 1 == (int)door[j].door)
				{
					if(persons[i].target == null)
					{
						persons[i].MoveToDoor(door[j].transform);
					}
				}						
			}
			//persons[i].MoveToBuilding
		}
	}
	
	//не используется
	public static void RefreshListOf(CharacterType type, out List<GameObject> list)//, out List<GameObject> infos)
	{
		list = new List<GameObject>();
		GameObject[] go = GameObject.FindGameObjectsWithTag(type.ToString());
		foreach(GameObject g in go)
		{
			//infos.Add(g.transform.FindChild("CharInfo"));
			list.Add(g);
		}
	}
	
	//не используется
	public static void PlaceInfosWithIndent(List<GameObject> list, Transform center, float indent)
	{
		for(int i = 0; i < list.Count; i++)
		{
			
			center.localPosition = new Vector3(0 + i * indent, center.localPosition.y, center.localPosition.z);
			list[i].transform.localPosition = center.localPosition;
		}
	}
	
	//отсылка плашек персонажей обратно на персонажей
	public static void CharInfoToParent(List<FilmStaff> staff)
	{
		for(int i = 0; i < staff.Count; i++)
		{
			staff[i].icon.transform.parent = staff[i].icon.GetComponent<CharInfo>().character.transform;
			staff[i].icon.SetActive(false);		
		}
	}
	
	public static IEnumerator LvlUpPlaneAnimation(Transform target, GameObject bg, CharInfo info, float startXPos, float scaleFactor = 1.5f, float animTime = 1, float time = 0)
	{
		while(true)
		{
			if(time >= animTime)
			{
				target.localPosition = new Vector3(startXPos, 0, 0);
				target.localEulerAngles = Vector3.zero;
				target.localScale = Vector3.one;
				GlobalVars.popUpStaffLevelUp.SetParams(info);
				GlobalVars.SwipeCamera.enabled = false;
				target.gameObject.SetActive(true);
				yield break;
			}
			Vector3 v3 = target.localEulerAngles;
			time += Time.deltaTime;
			v3.y = Mathf.Lerp(0, 180, time / animTime);
			target.localEulerAngles = v3;
			v3 = target.transform.localPosition;
			v3.x = 0 - target.transform.parent.localPosition.x;
			v3.y = 0 - target.transform.parent.localPosition.y;
			v3.z = -15;
			target.transform.localPosition = Vector3.Lerp(target.transform.localPosition, v3, time / animTime);
			v3 = Vector3.one * scaleFactor;
			target.transform.localScale = Vector3.Lerp(Vector3.one, v3, time / animTime);
			if(target.localEulerAngles.y >= 90)
			{
				target.gameObject.SetActive(false);
				bg.SetActive(true);
			}
			yield return 0;
		}
	}
	
	public static void SetCharInfos(List<FilmStaff> staff, Transform parent, float step, float startPoint)
	{
		for(int i = 0; i < staff.Count; i++)
		{
			CharInfo c = staff[i].GetComponent<FilmStaff>().icon.GetComponent<CharInfo>();
			c.transform.parent = parent;
			c.transform.localPosition = new Vector3(startPoint + i * step, 0, 0);
			c.transform.localScale = Vector3.one;
			c.gameObject.SetActive(true);
			c.Refresh();
			c.buttonHireStaff.SetActive(false);
			c.ShowBottomType();
		}
	}
	
	public static void HireStaff(CharacterType type)
	{	
		//сначала определяется тип персонажа поданный на вход, в зависимости от этого выбирается нужный список доступных жанров
		List<AvailableGenre> ag = new List<AvailableGenre>();
		string tag = "";
		switch(type)
		{
		case CharacterType.actor:
			ag = GlobalVars.actorsGenres;
			tag = "actor";
			break;
		case CharacterType.cameraman:
			ag = GlobalVars.cameramansGenres;
			tag = "cameraman";
			break;
		case CharacterType.director:
			ag = GlobalVars.directorsGenres;
			tag = "director";
			break;
		case CharacterType.scripter:
			ag = GlobalVars.scriptersGenres;
			tag = "scripter";
			break;
		}
		
		GameObject go = null;
		SetParamsForStaff(type, ag, tag, out go, 8, 16);
	}
	
	//инициализация персонажей, обнуление их параметров, выборка доступных жанров и присвоение им новых значений
	static void SetParamsForStaff(CharacterType ct, List<AvailableGenre> ag, string tag, out GameObject go, int first, int second)
	{
		go = null;
		
		go = GameObject.Instantiate(GlobalVars.commonPersonPrefab) as GameObject;
		go.tag = tag;
		go.GetComponent<PersonController>().InstantiatePerson(ct);
		
		FilmStaff fs = go.GetComponent<FilmStaff>();

		fs.exp = 0;
		fs.lvl = 1;
		fs.freeSkillPoints = 0;
		
		if(ct != CharacterType.postProdWorker && ct != CharacterType.producer)
		{
			List<AvailableGenre> list = new List<AvailableGenre>();
			foreach(AvailableGenre a in ag)
			{
				list.Add(a);
			}
			if(list.Count > 0)
			{
				int rand = Random.Range(0, list.Count);	
				StaffSkills s = new StaffSkills();
				s.genre = list[rand].genre;
				s.skill = list[rand].max - first;
				fs.mainSkill.Add(s);
				foreach(FilmSkills skills in fs.skills)
				{
					if(skills.genre == fs.mainSkill[0].genre)
					{
						skills.skill = list[rand].max - first;
					}
				}
				list.Remove(list[rand]);
			}
			if(list.Count > 0)
			{
				int rand = Random.Range(0, list.Count);
				StaffSkills s = new StaffSkills();
				s.genre = list[rand].genre;
				s.skill = list[rand].max - first;
				fs.mainSkill.Add(s);
				foreach(FilmSkills skills in fs.skills)
				{
					if(skills.genre == fs.mainSkill[1].genre)
					{
						skills.skill = list[rand].max - second;
					}
				}
			}
		}
		else
		{
			fs.icon.GetComponent<CharInfo>().otherSkills.skill = GlobalVars.studioLvl * 8;
			if(ct == CharacterType.postProdWorker)
			{
				fs.icon.GetComponent<CharInfo>().otherSkills.activity = OtherActivities.Postproduction;
			}
			else if(ct == CharacterType.producer)
			{
				fs.icon.GetComponent<CharInfo>().otherSkills.activity = OtherActivities.Marketing;
			}
		}
		go.SetActive(true);
		fs.icon.GetComponent<CharInfo>().SetParams(fs);
		fs.icon.SetActive(false);
	}
	
	public static Waypoint[] CalculateWayOnGround(GroundWaypoint currPosition, out GroundWaypoint endWaypoint)
	{
		endWaypoint = GlobalVars.allGroundWaypoints[Random.Range(0, GlobalVars.allGroundWaypoints.Length)];;
		GroundWaypoint currWaypoint = currPosition;
		List<GroundWaypoint> way = new List<GroundWaypoint>();
		way.Add(currWaypoint);
		while(way[way.Count - 1] != endWaypoint)
		{
			GroundWaypoint temp = null;
			foreach(GroundWaypoint g in way[way.Count - 1].closestWaypoints)
			{
				if(g == null)
				{
					
				}
				else if(temp == null)
				{
					temp = g;
				}
				else if(!temp.enabled)
				{
				}
				else if(Vector3.Distance(g.transform.position,endWaypoint.transform.position) < Vector3.Distance(temp.transform.position,endWaypoint.transform.position))
				{
					temp = g;
				}
			}
			if(temp == null)
			{
			}
			else
			{
				way.Add(temp);
			}
		}
		Waypoint[] w = new Waypoint[way.Count];
		for(int i = 0; i < w.Length; i++)
		{
			Waypoint point = new Waypoint();
			point.pointPos = way[i].transform;
			point.isTeleportPoint = false;
			w[i] = point;
		}
		return w;
	}
	
	public static Waypoint[] CalculateWayToBuilding(GroundWaypoint currPosition, Waypoint firstBuildingWaypoint, Waypoint[] addedWaypoints)
	{
		GroundWaypoint endGroundWaypoint = null;
		foreach(GroundWaypoint g in GlobalVars.allGroundWaypoints)
		{
			if(endGroundWaypoint == null)
			{
				endGroundWaypoint = g;
			}
			else if(Vector3.Distance(g.transform.position, firstBuildingWaypoint.pointPos.position) 
					< Vector3.Distance(endGroundWaypoint.transform.position, firstBuildingWaypoint.pointPos.position))
			{
				endGroundWaypoint = g;
			}
		}
		List<GroundWaypoint> way = new List<GroundWaypoint>();
		way.Add(currPosition);
		while(way[way.Count - 1] != endGroundWaypoint)
		{
			GroundWaypoint temp = null;
			foreach(GroundWaypoint g in way[way.Count - 1].closestWaypoints)
			{
				if(g == null)
				{
				}
				else if(temp == null)
				{
					temp = g;
				}
				else if(Vector3.Distance(g.transform.position, endGroundWaypoint.transform.position) < Vector3.Distance(temp.transform.position, endGroundWaypoint.transform.position))
				{
					temp = g;
				}
			}
			if(temp != null)
			{
				way.Add(temp);
			}
		}
		Waypoint[] w = new Waypoint[way.Count + addedWaypoints.Length];
		for(int i = 0; i < w.Length; i++)
		{
			Waypoint point = new Waypoint();
			if(i < way.Count)
			{
				point.pointPos = way[i].transform;
				point.isTeleportPoint = false;
				w[i] = point;
			}
			else
			{
				w[i] = addedWaypoints[i - way.Count];
			}
		}
		return w;
	}
}
