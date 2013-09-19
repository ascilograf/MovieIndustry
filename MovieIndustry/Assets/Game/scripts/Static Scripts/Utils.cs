using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//функции утиля
public static class Utils 
{
	static Vector3 tapDown, tapUp;
	static string h, m, s;
	
	public static void FillTimeParameters()
	{
		foreach(ImportedString str in Textes.mi_ui_lines)
		{
			if(str.caption.Equals("global_time_seconds"))
			{
				s = str.text;
			}
			else if(str.caption.Equals("global_time_hours"))
			{
				h = str.text;
			}
			else if(str.caption.Equals("global_time_minuts"))
			{
				m = str.text;
			}
			if(h != null && m != null && s != null)
			{
				return;
			}
		}
	}
	
	public static void SetText(tk2dTextMesh textMesh, string s)
	{
		textMesh.text = s;
	}
	
	public static string FormatStringToText(string inputString, int maxCharsInLine)
	{
		string[] words = inputString.Split(" " [0]);
		string outputString = "";
		string s = "";
		for(int i = 0; i < words.Length; i++)
		{
			if((words[i] + s).Length <= maxCharsInLine)
			{
				s += words[i] + " ";
			}
			else
			{
				outputString += s;
				outputString += "\n";
				s = "";
				if(i != words.Length - 1)
				{
					s += words[i] + " ";
				}
				else
				{
					s += words[i];
				}
			}
			if((i == words.Length - 1) && s != "")
			{
				outputString += s;
			}
		}
		return outputString;
	}
	
	public static string ToNumberWithSpaces(string inString)
	{
		string outString = "";
		int index = 0;
		for(int i = 0; i < inString.Length; i++)
		{
			outString = inString[inString.Length - 1 - i] + outString;
			if((outString.Length - index) % 3 == 0)
			{
				outString = " " + outString;
				index++;
			}
		}
		return outString;
	}
	
	//скрыть/показать объект
	public static void ShowHideObject(GameObject go, bool b)
	{
		Transform[] tr = go.GetComponentsInChildren<Transform>(true);
		foreach(Transform t in tr)
		{
			t.gameObject.SetActive(b);
		}
	}
	
	//сфокусировать камеру на объекте
	public static void FocusOn(Transform target)
	{
		float h = Screen.height;
		//Camera.main.orthographicSize = (h/(2 + h/768));
		//Camera.main.orthographicSize = h / 768;
		Vector3 v3 = Camera.main.transform.position;
		v3.x = target.position.x;
		/*if(target.position.y > 80 * (Screen.height / 768))
		{
			v3.y = target.position.y;
		}
		else
		{
			v3.y = 80 * (Screen.height / 768);
		}*/
		Camera.main.transform.position = v3;
	}
	
	public static float GetPercentsFromCinemasCount()
	{
		float f = 0;
		
		return f;
	}
	
	//контроль нажатий по экрану, 
	//во всех случаях обработки луча выполняется следующий механизм:
	//вначале заапускается луч по GUI слою, и если было попадание, то происходит переход к следующему событию, либо рассылка
	//если же попадания в GUI не было, то происходит каст по дефолт слою,
	//если не было попаданий вовсе, то рассылается сообщение что палец просто был поднят.
	public static void GameInput() 
	{
		GameObject caughtObject = null;
		if(GlobalVars.isMobilePlatform)
		{
			if(Input.touchCount > 0)
			{
				//обработка нажатия, начало
				if(Input.GetTouch(0).phase == TouchPhase.Began)
				{
					tapDown.x = Input.GetTouch(0).position.x;
					tapDown.y = Input.GetTouch(0).position.y;
					tapDown.z = 0;
					
					if(CheckTapOnLayers() != null)
					{
						Messenger<GameObject>.Broadcast("Tap on GUI Layer begin", CheckTapOnLayers());
					}
				}
				
				//обработка нажатия, окончание
				if(Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
				{
					tapUp.x = Input.GetTouch(0).position.x;
					tapUp.y = Input.GetTouch(0).position.y;
					tapUp.z = 0;
					
					if(Vector3.Distance(tapUp, tapDown) > GlobalVars.tapTremble)
					{
						Messenger.Broadcast("Finger was lifted");
						return;
					}
					
					caughtObject = CheckTapOnLayers();
					
					tapUp = Vector3.zero;
					tapDown = Vector3.zero;
					
					Messenger.Broadcast("Finger was lifted");
					if(caughtObject == null)
					{
						Debug.Log("null go");
						return;
					}
				}
			}
		}
		else
		{
			//обработка нажатия, начало
			if(Input.GetMouseButtonDown(0))
			{
				tapDown = Input.mousePosition;
				tapDown.z = 0;
				if(CheckTapOnLayers() != null)
				{
					Messenger<GameObject>.Broadcast("Tap on GUI Layer begin", CheckTapOnLayers());
				}
			}
			//обработка нажатия, окончание
			if(Input.GetMouseButtonUp(0))
			{
				tapUp = Input.mousePosition;
				tapUp.z = 0;
			
				if(Vector3.Distance(tapUp, tapDown) > GlobalVars.tapTremble)
				{
					Messenger.Broadcast("Finger was lifted");
					return;
				}
				
				caughtObject = CheckTapOnLayers();
				
				tapUp = Vector3.zero;
				tapDown = Vector3.zero;
				
				Messenger.Broadcast("Finger was lifted");
				if(caughtObject == null)
				{
					//Debug.Log("null go");
					return;
				}
			}
		}
		
		if(caughtObject != null)
		{
			Messenger<GameObject>.Broadcast("Tap on target", caughtObject);
			//GlobalVars.tapSound.Play();
			Debug.Log(caughtObject.name);
		}
	}
	
	static GameObject CheckTapOnLayers()
	{
		GameObject caughtObject = null;
		int layer = 1 << 10;
		RaycastHit hit;
		//Camera c = GlobalVars.SwipeCamera;
		//if(GlobalVars.SwipeCamera != null && GlobalVars.SwipeCamera.enabled == true)
		//{
		Ray ray = GlobalVars.SwipeCamera.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
		{
			if(GlobalVars.buttonForLearning == null)
			{
				caughtObject = hit.collider.gameObject;
			}
			else if(GlobalVars.buttonForLearning == hit.collider.gameObject)
			{
				caughtObject = hit.collider.gameObject;
			}
		}
		else 
		{
			layer = 1 << 9;
			ray = GlobalVars.GUICamera.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
			{
				if(GlobalVars.buttonForLearning == null)
				{
					caughtObject = hit.collider.gameObject;
				}
				else if(GlobalVars.buttonForLearning == hit.collider.gameObject)
				{
					caughtObject = hit.collider.gameObject;
				}
			}
			else
			{
				layer = 1 << 0;
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
				{
					if(GlobalVars.buttonForLearning == null)
					{
						caughtObject = hit.collider.gameObject;
					}
					else if(GlobalVars.buttonForLearning == hit.collider.gameObject)
					{
						caughtObject = hit.collider.gameObject;
					}
				}
			}
		}
		return caughtObject;
	}

	public static void ColorizeSprites(tk2dSprite[] sprites, Color32 color)
	{
		foreach(tk2dSprite s in sprites)
		{
			s.color = color;
		}
	}
	
	//форматирование строки, *UNFINISHED*
	public static string FormatString(string inputString)
	{
		inputString = inputString.Replace("#" , "\n");
		string tempString = inputString;
		return tempString;
	}
	
	public static string FormatFilmName(string filmName)
	{
		string outString = "";
		string[] text = filmName.Split(" " [0]);
		for(int i = 0; i < text.Length; i++)
		{
			text[i] += " ";
			if(i == text.Length - 1)
			{
				text[i] = "\n" + text[i];
			}
			outString += text[i];
		}
		return outString;
	}
	
	public static string FormatIntToUsualTimeString(int inputTimeInSec, int timeItemsCount = 3)
	{
		string tempString = "";
		int secs = 0;
		int mins = 0;
		int hours = 0;
		if(inputTimeInSec >= 60)
		{
			mins = inputTimeInSec / 60;
			secs = inputTimeInSec - mins*60;
			if(mins >= 60)
			{
				hours = (int)mins / 60;
				mins = mins - hours*60;
				//if(hours >= 24)
				//{
				//	days = hours / 24;
				//	hours = hours - days * 24;
				//}
			}
		}
		else
		{
			secs = inputTimeInSec;
		}
		if(timeItemsCount == 3)
		{
			if(hours > 0)
			{
				tempString += hours + h + " ";
			}
			if(mins > 0)
			{
				tempString += mins + m + " ";
			}
			if(secs > 0)
			{
				tempString += secs + s;
			}
		}
		else if(timeItemsCount == 2)
		{
			if(hours > 0 && mins > 0)
			{
				tempString += hours + h + " ";
				tempString += mins + m + " ";
			}
			else if(hours > 0 && secs > 0)
			{
				tempString += hours + h + " ";
				tempString += secs + s;
			}
			else if(mins > 0 && secs > 0)
			{
				tempString += mins + m + " ";
				tempString += secs + s;
			}
			else if(secs > 0)
			{
				tempString += secs + s;
			}
			else if(hours > 0 && mins == 0 && secs == 0)
			{
				tempString += hours + h + "";
			}
			else if(mins > 0 && secs == 0)
			{
				tempString += mins + m + "";
			}
		}
		else if(timeItemsCount == 1)
		{
			if(hours > 0)
			{
				tempString += hours + h + " ";
			}
			else if(mins > 0)
			{
				tempString += mins + m + " ";
			}
			else if(mins > 0)
			{
				tempString += secs + s;
			}
		}
		return tempString;
	}
	
	public static void FormatIntTo2PartsTimeString(tk2dTextMesh leftPart, tk2dTextMesh rightPart, int inputTimeInSec)
	{
		string leftString = "";
		string rightString = "";
		int secs = 0;
		int mins = 0;
		int hours = 0;
		int days = 0;
		if(inputTimeInSec >= 60)
		{
			mins = inputTimeInSec / 60;
			secs = inputTimeInSec - mins*60;
			if(mins >= 60)
			{
				hours = (int)mins / 60;
				mins = mins - hours*60;
				if(hours >= 24)
				{
					days = hours / 24;
					hours = hours - days * 24;
				}
			}
		}
		else
		{
			secs = inputTimeInSec;
		}
		if(hours > 0)
		{
			leftString += hours + h;
			if(mins > 0)
			{
				rightString += mins + m;
			}
			else if(mins == 0 && secs > 0)
			{
				rightString += secs + s;
			}
		}
		else if(mins > 0)
		{
			leftString += mins + m;
			if(secs > 0)
			{
				rightString = secs + s;
			}
		}
		else if(secs > 0)
		{
			rightString += secs + s;
		}
		leftPart.text = leftString;
		rightPart.text = rightString;
		leftPart.Commit();
		rightPart.Commit();
	}
	
	//форматирование секундного значения времени в строку с маркерами дней, часов, минут, секунд.
	public static string FormatIntToTimeString(int inputTimeInSec)
	{
		string tempString = "";
		int secs = 0;
		int mins = 0;
		int hours = 0;
		int days = 0;
		if(inputTimeInSec >= 60)
		{
			mins = inputTimeInSec / 60;
			secs = inputTimeInSec - mins*60;
			if(mins >= 60)
			{
				hours = (int)mins / 60;
				mins = mins - hours*60;
				if(hours >= 24)
				{
					days = hours / 24;
					hours = hours - days * 24;
				}
			}
		}
		else
		{
			secs = inputTimeInSec;
		}
		
		if(days > 0)
		{
			if(hours == 0 && mins == 0 && secs == 0)
			{
				if(days > 1)
				{
					tempString += days.ToString() + Textes.timeLoc.manyDays;	
				}
				else if(days == 1)
				{
					tempString += days.ToString() + Textes.timeLoc.oneDay;
				}
			}
			else
			{
				tempString += days.ToString() + Textes.timeLoc.d;
			}
		}
		if(hours > 0)
		{
			if(days == 0 && mins == 0 && secs == 0)
			{
				if(hours > 1)
				{
					tempString += hours.ToString() + Textes.timeLoc.manyHours;
				}
				else if(hours == 1)
				{
					tempString += hours.ToString() + Textes.timeLoc.oneHour;
				}
			}
			else
			{
				if(hours > 0)
				{
					tempString += ", " + hours.ToString() + Textes.timeLoc.h;
				}
				else
				{
					tempString += hours.ToString() + Textes.timeLoc.h;
				}
			}	
		}
		if(mins > 0)
		{
			if(hours == 0 && days == 0 && secs == 0)
			{
				if(mins > 1)
				{
					tempString += mins.ToString() + Textes.timeLoc.manyMinuts;
				}
				else if(mins == 1)
				{
					tempString += mins.ToString() + Textes.timeLoc.oneMinute;
				}
			}
			else
			{
				if(mins > 0)
				{
					tempString += ", " + mins.ToString() + Textes.timeLoc.m;
				}
				else
				{
					tempString += mins.ToString() + Textes.timeLoc.m;
				}
			}	
		}
		if(secs > 0)
		{
			if(hours == 0 && days == 0 && mins == 0)
			{
				if(secs > 1)
				{
					tempString += secs.ToString() + Textes.timeLoc.manySeconds;
				}
				else if(secs == 1)
				{
					tempString += secs.ToString() + Textes.timeLoc.oneSecond;
				}
			}
			else
			{
				if(secs > 0)
				{
					tempString += ", " + secs.ToString() + Textes.timeLoc.s;
				}
				else
				{
					tempString += secs.ToString() + Textes.timeLoc.s;
				}
			}	
		}
		
		return tempString;
	}
	
	public static IEnumerator MapMoneyGettingUp(tk2dTextMesh mesh, tk2dSprite sprite, string text, float t)
	{
		while(true)
		{
			if(t <= 0)
			{
				yield return new WaitForSeconds(2);
				mesh.gameObject.SetActive(false);
				yield break;
			}
			Color32 col = new Color32(182, 211, 63, 0);
			Color32 col2 = new Color32(182, 211, 63, 255);
			Color32 col3 = Color.white;
			col3.a = 0;
			sprite.color = Color.Lerp(Color.white, col3, t / 2);
			mesh.text = text;
			mesh.transform.localPosition = Vector3.Lerp(new Vector3(0,40,-5), new Vector3(0,0,-5), t / 2);
			mesh.color = Color.Lerp(col2, col, t / 2);	
			t -= Time.deltaTime;
			mesh.Commit();
			yield return 0;
		}
	}
	
	public static IEnumerator MapChartMove(Transform target, Vector3 fromPos, Vector3 toPos, float t)
	{
		while(true)
		{
			if(t <= 0)
			{
				yield break;
			}
			target.localPosition = Vector3.Lerp(fromPos, toPos, t);
			t -= Time.deltaTime;
			yield return 0;
		}
	}
	
	static int currMoney;
	static int changeMoney;
	
	public static void ChangeMoney(int count)
	{
		currMoney = GlobalVars.money;
		Debug.Log (currMoney);
		changeMoney = GlobalVars.money + count;
		Debug.Log(changeMoney);
		Coroutiner.StartCoroutine(ChangeMoneyBalance());
	}
	
	public static IEnumerator ChangeMoneyBalance(float time = 1)
	{
		while(true)
		{
			if(time <= 0)
			{
				yield break;
			}
			time -= Time.deltaTime;
			float k = Mathf.Lerp(changeMoney, currMoney, time);
			GlobalVars.money = (int)k;
			yield return 0;
		}
	}
	
	public static void SetProgressBarValue(GameObject[] progresbarStars,int skill)
	{
		for(int i = 0; i < 5; i++)
		{
			if((skill - 20) > 0)
			{	
				DeactivateStarExcept(progresbarStars[i], "5state");
			}
			else if(skill < 1)
			{
				DeactivateStarExcept(progresbarStars[i], "empty");
			}
			else if(skill < 5)
			{
				DeactivateStarExcept(progresbarStars[i], "1state", progresbarStars[i].transform);
			}
			else if(skill < 9)
			{
				DeactivateStarExcept(progresbarStars[i], "2state", progresbarStars[i].transform);
			}
			else if(skill < 13)
			{
				DeactivateStarExcept(progresbarStars[i], "3state", progresbarStars[i].transform);
			}
			else if(skill < 17)
			{
				DeactivateStarExcept(progresbarStars[i], "4state", progresbarStars[i].transform);
			}
			else if(skill < 21)
			{
				DeactivateStarExcept(progresbarStars[i], "5state", progresbarStars[i].transform);
			}
			skill -= 20;
		}
	}
	
	public static IEnumerator SetProgressBarValueInTime(GameObject[] progresbarStars,int skill)
	{
		for(int i = 0; i < 5; i++)
		{
			if((skill - 20) > 0)
			{	
				DeactivateStarExcept(progresbarStars[i], "5state");
			}
			else if(skill < 1)
			{
				DeactivateStarExcept(progresbarStars[i], "empty");
			}
			else if(skill < 5)
			{
				DeactivateStarExcept(progresbarStars[i], "1state", progresbarStars[i].transform);
			}
			else if(skill < 9)
			{
				DeactivateStarExcept(progresbarStars[i], "2state", progresbarStars[i].transform);
			}
			else if(skill < 13)
			{
				DeactivateStarExcept(progresbarStars[i], "3state", progresbarStars[i].transform);
			}
			else if(skill < 17)
			{
				DeactivateStarExcept(progresbarStars[i], "4state", progresbarStars[i].transform);
			}
			else if(skill < 21)
			{
				DeactivateStarExcept(progresbarStars[i], "5state", progresbarStars[i].transform);
			}
			if(skill > 0)
			{
				for(float t = 0; t < 1; t += Time.deltaTime)
				{
					yield return new WaitForSeconds(Time.deltaTime);
					progresbarStars[i].transform.localScale = Vector3.Lerp(Vector3.one * 1.4f, Vector3.one, t);
				}
			}
			skill -= 20;
		}
		yield break;
	}
	
	static void DeactivateStarExcept(GameObject go,string state, Transform button = null)
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
	
	public static string GenresInText(List<FilmGenres> genres)
	{
		string s = "";
		for(int i = 0; i < genres.Count; i++)
		{
			s += genres[i].ToString();
			if(i < genres.Count - 1)
			{
				s += ", ";
			}
		}
		return s;
	}
	
	public static IEnumerator MoveCameraTo(Vector3 posFrom, Vector3 posTo, PopUpShortCut shortCut, float t = 0)
	{
		while(true)
		{
			if(t >= 1)
			{
				GlobalVars.cameraStates = CameraStates.normal;
				shortCut.CallMenu();
				yield break;
			}
			GlobalVars.cameraStates = CameraStates.learning;
			t += Time.deltaTime;
			Camera.main.transform.position = Vector3.Lerp(posFrom, posTo, t);
			yield return 0;
		}
	}
	
	public static IEnumerator MoveCameraTo(Vector3 posFrom, Vector3 posTo, PopUpQuests popUpQuests, float t = 0)
	{
		while(true)
		{
			if(t >= 1)
			{
				GlobalVars.cameraStates = CameraStates.normal;
				popUpQuests.CallMenu();
				yield break;
			}
			GlobalVars.cameraStates = CameraStates.learning;
			t += Time.deltaTime;
			Camera.main.transform.position = Vector3.Lerp(posFrom, posTo, t);
			yield return 0;
		}
	}

	public static IEnumerator SwitchFilmPlanes(MapChartItem item, int index, Vector3 firstPos, Vector3 secondPos, float t = 0)
	{
		while(true)
		{
			if(t >= 1)
			{
				item.isMoving = false;
				GlobalVars.worldRental.chart.filmItemsInfo[index] = item;
				item.transform.localPosition = secondPos;
				yield break;
			}
			t += Time.deltaTime;
			item.transform.localPosition = Vector3.Lerp(firstPos, secondPos, t);
			yield return 0;
		}
	}
	
	public static IEnumerator ChangeExpBalance(int currExp, int nextExp, float time = 1)
	{
		while(true)
		{
			if(time <= 0)
			{
				yield break;
			}
			time -= Time.deltaTime;
			float k = Mathf.Lerp(nextExp, currExp, time);
			GlobalVars.exp = (int)k;
			yield return 0;
		}
	}
	
	public static IEnumerator ShowWindow(Transform objToMove, Vector3 startPos, Vector3 finPos, bool setActive,float time = 0f)
	{
		while(true)
		{
			if(time >= 0.4f || Input.GetMouseButtonDown(0))
			{
				GlobalVars.BlockInput = false;
				objToMove.localPosition = finPos;
				objToMove.localScale = Vector3.one;
				objToMove.gameObject.SetActive(setActive);
				yield break;
			}
			if(setActive)
				objToMove.localScale = Vector3.Lerp(Vector3.one * 0.4f, Vector3.one, time * 2.5f);
			else
				objToMove.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.4f, time * 2.5f);
			objToMove.localPosition = Vector3.Lerp(startPos, finPos, time * 2.5f);
			
			time += Time.deltaTime;
			
			yield return 0;
		}
	}
}
