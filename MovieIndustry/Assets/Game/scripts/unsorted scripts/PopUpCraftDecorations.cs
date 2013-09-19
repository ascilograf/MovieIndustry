using UnityEngine;
using System.Collections;

//открытие/закрытие меню крафта декораций
//посыл сигнала о сборке декорации
public class PopUpCraftDecorations : MonoBehaviour 
{
	public GameObject buttonClose;					//кнопка закрытия
	public GameObject infos;						//итемы
	
	public Camera cam;
				
	public tk2dUIScrollableArea scrollArea;
	public float maxLength;							//макс. высота
	public float minLength;							//мин. высота
	
	public CraftDecorTab[] tabs;
	
	CraftDecorTab currTab;
	
	void Start () 
	{
		GlobalVars.popUpCraftDecorations = this;
	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject go)
	{
		if(go == buttonClose)
		{
			gameObject.SetActive(false);
			GlobalVars.cameraStates = CameraStates.normal;
		}
		else if(go.GetComponent<CraftDecorItem>() != null)
		{
			go.GetComponent<CraftDecorItem>().CraftNewItem();
		}
		else if(go.name == "tab")
		{
			foreach(CraftDecorTab t in tabs)
			{
				if(t.tabButton == go)
				{
					ActivateTab(t);	
				}
			}
		}
	}
	
	void ActivateTab(CraftDecorTab tab)
	{
		foreach(CraftDecorTab t in tabs)
		{
			if(t == tab)
			{
				currTab = t;
				t.tab.SetActive(true);
				t.activeTab.enabled = true;
				scrollArea.contentContainer = t.infos;
				CheckAllItems(t.items);
			}
			else
			{
				t.activeTab.enabled = false;
				t.infos.SetActive(false);
			}
		}
	}
	
	public void ActivateAdvTab()
	{
		ActivateTab(tabs[0]);
	}
	
	public void RefreshParts()
	{
		//print ("wowwowwow!");
		foreach(CraftDecorTab t in tabs)
		{
			if(t == currTab)
			{
				CheckAllItems(t.items);
			}
		}
	}
	
	//проверка всех итемов при активации меню
	public void CheckAllItems(CraftDecorItem[] craftPlanes)
	{
		foreach(CraftDecorItem c in craftPlanes)
		{
			c.RefreshParts();
		}
	}
}
