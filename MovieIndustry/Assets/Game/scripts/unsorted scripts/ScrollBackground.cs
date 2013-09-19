using UnityEngine;
using System.Collections;

public class ScrollBackground : MonoBehaviour 
{
	public Transform partToMove;
	
	public float addedLength = 100;
	public bool isVertical;
	
	tk2dUIScrollableArea scroll;
	float currLength = 0;
	// Use this for initialization
	void Awake () 
	{
		 scroll = GetComponent<tk2dUIScrollableArea>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(currLength != scroll.ContentLength)
		{
			ChangeBgSize();
		}
	}
	
	void ChangeBgSize()
	{
		currLength = scroll.ContentLength;
		Vector3 v3 = partToMove.localPosition;
		if(isVertical)
			v3.x = scroll.ContentLength + addedLength;
		else
			v3.y = (scroll.ContentLength + addedLength) * -1;
		partToMove.localPosition = v3;
	}
}
