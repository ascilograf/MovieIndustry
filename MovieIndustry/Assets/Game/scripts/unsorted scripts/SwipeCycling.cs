using UnityEngine;
using System.Collections;

public class SwipeCycling : MonoBehaviour 
{
	public GameObject[] elements;
	public float distanceToMove;
	public float newPosDist;
	
	tk2dUIScrollableArea scrollArea;
	
	// Use this for initialization
	void Start () 
	{
		scrollArea = GetComponent <tk2dUIScrollableArea>();
		
		foreach(GameObject g in elements)
		{
			if(Vector3.Distance(g.transform.position, transform.position) < distanceToMove)
			{
				
			}
			else if(g.transform.position.x > transform.position.x)
			{
				MoveElements (true, g);
			}
			else if(g.transform.position.x < transform.position.x)
			{
				MoveElements (false, g);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButton(0))
		{
			foreach(GameObject g in elements)
			{
				if(Vector3.Distance(g.transform.position, transform.position) < distanceToMove)
				{
					
				}
				else if(g.transform.position.x > transform.position.x)
				{
					MoveElements (true, g);
				}
				else if(g.transform.position.x < transform.position.x)
				{
					MoveElements (false, g);
				}
			}
		}
	}
	
	void MoveElements(bool left, GameObject obj)
	{
		GameObject[] temp = elements;
		Vector3 v3 = obj.transform.position;
		if(left)
		{
			temp[0] = obj;
			v3.x = elements[0].transform.position.x - newPosDist;
		}
		else
		{
			temp[5] = obj;
			v3.x = elements[elements.Length - 1].transform.position.x + newPosDist;
		}
		for(int i = 0; i < elements.Length; i++)
		{
			if(elements[i] != obj)
				temp[i] = elements[i];
		}
		elements = temp;
		obj.transform.position = v3;
	}
}
