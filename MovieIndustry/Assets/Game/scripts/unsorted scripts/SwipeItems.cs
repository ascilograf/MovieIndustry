using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//свайп элементов, добавленных в итемс
//если элементов более чем один, то при зажатой кнопке мыши род. объект инфос будет двигаться вместе с курсором
//если мышь отпущена, то скрипт доводит текщий элемент до положения "по центру"
public class SwipeItems : MonoBehaviour 
{
	public GameObject infos;					//род. объект
	public float maxLength;
	public float minLength;
	public Collider moveFrame;
	
	public bool isVertical;
	public Camera cam;
	
	bool canSwipe;
	//отработка свайпа
	void Update()
	{
		if(moveFrame != null)
		{
			if(Input.GetMouseButtonDown(0))
			{
				RaycastHit hit;
				Ray ray = GlobalVars.GUICamera.ScreenPointToRay(Input.mousePosition);
				if(moveFrame.Raycast(ray, out hit, 1000))
				{
					canSwipe = true;
				}
			}
			else if(Input.GetMouseButtonUp(0))
			{
				canSwipe = false;
			}
		}
		if(canSwipe || moveFrame == null)
		{
			if(!isVertical)
				MoveButtonsOnTapHorizontal();
			if(isVertical)
				MoveButtonsOnTapVertical();
		}
	}
	
	public void SetParams(float min, float max)
	{
		minLength = min;
		maxLength = max;
	}
	
	float dist = 0;
	//свайп элементов
	void MoveButtonsOnTapHorizontal()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Vector3 v3 = Input.mousePosition;
			v3.x -= transform.localPosition.x;
			dist = v3.x;
		}
		
		if(Input.GetMouseButton(0))
		{
			Vector3 v3 = Input.mousePosition;
			v3.x -= dist;
			if(v3.x < minLength)
			{
				v3.x = minLength;
			}
			if(v3.x > maxLength)
			{
				v3.x = maxLength;
			}
			v3.z = transform.localPosition.z;
			v3.y = transform.localPosition.y;
			transform.localPosition = v3;
		}
		if(Input.GetMouseButtonUp(0))
		{
			dist = 0;
		}
	}
	
	void MoveButtonsOnTapVertical()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Vector3 v3 = Input.mousePosition;
			v3.y -= transform.localPosition.y;
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
			v3.z = transform.localPosition.z;
			v3.x = transform.localPosition.x;
			transform.localPosition = v3;
		}
		if(Input.GetMouseButtonUp(0))
		{
			dist = 0;
		}
	}
}
