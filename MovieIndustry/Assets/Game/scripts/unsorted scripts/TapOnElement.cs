using UnityEngine;
using System.Collections;

//определение тапа по элементу
//если было попадание - показываем elementTapped
//есть возможность показывать неактивную кнопку вызывая SetActiveTo()

public class TapOnElement : MonoBehaviour 
{
	public MeshRenderer elementTapped;
	public MeshRenderer elementNotActive;
	public GameObject tooltip;
	//public MeshRenderer elementNotActive;
	// Use this for initialization
	void OnEnable () 
	{
		Messenger<GameObject>.AddListener("Tap on GUI Layer begin", CheckTapOnGUIBegin);
		Messenger.AddListener("Finger was lifted", FingerWasLifted);
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject g)
	{
		if(tooltip != null)
		{	
			if(gameObject == g)
			{
				tooltip.SetActive(true);
				GlobalVars.BlockInput = true;
			}		
		}
	}
	
	void CheckTapOnGUIBegin(GameObject go)
	{
		
		if(gameObject == go)
		{
			elementTapped.enabled = true;
		}
		
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on GUI Layer begin", CheckTapOnGUIBegin);
		Messenger.RemoveListener("Finger was lifted", FingerWasLifted);
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void FingerWasLifted()
	{
		if(tooltip == null)
			elementTapped.enabled = false;
		else if(GlobalVars.BlockInput)
		{
			tooltip.SetActive(false);
			elementTapped.enabled = false;
		}
	}
	
	public void SetActiveTo(bool b)
	{
		elementNotActive.enabled = !b;
		collider.enabled = b;
	}

}
