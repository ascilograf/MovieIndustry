using UnityEngine;
using System.Collections;

//покупка части для крафта декорации
//при активации определяется сеттинг, рарность и что за часть будет покупаться
//выставляются параметры цены, определяется иконка для демонстрации в превью

public class BuyDecorCraftItemPlane : MonoBehaviour {
	
	public GameObject allIcons;
	public GameObject buttonClose;
	public GameObject buttonAccept;
	public tk2dTextMesh priceMesh;
	public CraftItemsIconsWithSetting[] icons;
	int price;
	public Setting setting;
	//int ind;
	GameObject temp;
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
		GlobalVars.SwipeCamera.enabled = false;
	}
	
	void OnDisable()
	{
		GlobalVars.SwipeCamera.enabled = true;
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject g)
	{
		if(g == buttonAccept && GlobalVars.stars >= price)
		{
			
			Instantiate(temp);
			GlobalVars.stars -= price;
			StartCoroutine(DelayedClick());
		}
		else if(g == buttonClose)
		{
			gameObject.SetActive(false);
		}
	}
	
	IEnumerator DelayedClick()
	{
		yield return new WaitForSeconds(Time.deltaTime);
		GlobalVars.popUpCraftDecorations.RefreshParts();
		gameObject.SetActive(false);
		yield break;
	}
		
	public void Activate(Setting s, int index, RarityLevel r)
	{
		gameObject.SetActive(true);
		allIcons.SetActive(false);
		setting = s;
		foreach(CraftItemsIconsWithSetting c in icons)
		{
			if(c.setting == s)
			{
				switch(r)
				{
				case RarityLevel.common:
					temp = c.itemsPrefabsCommon[index];
					c.iconsCommon[index].SetActive(true);
					price = 10;
					priceMesh.text = 10 + "";
					priceMesh.Commit();
					break;
				case RarityLevel.rare:
					temp = c.itemsPrefabsRare[index];
					c.iconsRare[index].SetActive(true);
					price = 100;
					priceMesh.text = 100 + "";
					priceMesh.Commit();
					break;
				case RarityLevel.unique:
					temp = c.itemsPrefabsUnique[index];
					c.iconsUnique[index].SetActive(true);
					price = 1000;
					priceMesh.text = 1000 + "";
					priceMesh.Commit();
					break;
				}
					
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
