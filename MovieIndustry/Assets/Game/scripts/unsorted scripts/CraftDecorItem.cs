using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//плейн крафта, отображение кол-ва итемов 
//проверки на кол-во частей для крафта, проверка на деньги

public class CraftDecorItem : MonoBehaviour 
{
	public Setting setting;					//сеттинг
	public RarityLevel rarity;				//грейд
	public tk2dTextMesh firstItemMesh;		//первые части
	public GameObject firstItemButton;
	public tk2dTextMesh secondItemMesh;		//вторые части
	public GameObject secondItemButton;
	public tk2dTextMesh thirdItemMesh;		//третьи части
	public GameObject thirdItemButton;
	public tk2dTextMesh priceMesh;			//цена
	
	public MeshRenderer activeButton;		//активна ли кнопка
	
	public Color colorAvailable;			//цвет доступных частей
	public Color colorNotAvailable;			//цвет недоступных частей
	
	public BuyDecorCraftItemPlane buyPlane;
	
	int price;								//цена
	
	
	
	public List<CraftDecorationPart> firstParts = new List<CraftDecorationPart>();			//первые части
	public List<CraftDecorationPart> secondParts = new List<CraftDecorationPart>();			//вторые части
	public List<CraftDecorationPart> thirdParts = new List<CraftDecorationPart>();			//общие части
	
	void Start () 
	{
		
		//RefreshParts();
	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
		
	}
	
	void CheckTap(GameObject go)
	{
		if(go == firstItemButton)
		{
			buyPlane.Activate(setting, 0, rarity);
		}
		else if(go == secondItemButton)
		{
			buyPlane.Activate(setting, 1, rarity);
		}
		else if(go == thirdItemButton)
		{
			buyPlane.Activate(Setting.none, 0, rarity);
		}
	}
	
	//определение цены
	void SetPrice()
	{
		switch(rarity)
		{
		case RarityLevel.common:
			price = 100 * GlobalVars.studioLvl;
			break;
		case RarityLevel.rare:
			price = 1000 * GlobalVars.studioLvl;
			break;
		case RarityLevel.unique:
			price = 10000 * GlobalVars.studioLvl;
			break;
		}
	}
	
	//обновление списков частей
	public void RefreshParts()
	{
		firstParts.Clear();
		secondParts.Clear();
		thirdParts.Clear();
		price = 0;
		List<CraftDecorationPart> tempParts = new List<CraftDecorationPart>();
		foreach(InventoryItem ii in GlobalVars.inventory.items)
		{
			if(ii.GetComponent<CraftDecorationPart>() != null)
			{
				if(ii.GetComponent<CraftDecorationPart>().setting == setting 
					&& ii.GetComponent<CraftDecorationPart>().rarity == rarity)
				{
					tempParts.Add(ii.GetComponent<CraftDecorationPart>());
				}
				if(	ii.GetComponent<CraftDecorationPart>().setting == Setting.none 
					&& ii.GetComponent<CraftDecorationPart>().rarity == rarity)
				{
					tempParts.Add(ii.GetComponent<CraftDecorationPart>());
				}
			}
		}
		if(tempParts.Count > 0)
		{
			foreach(CraftDecorationPart part in tempParts)
			{
				if(part.typePart == PartType.firstPart)
				{
					firstParts.Add(part);
				}
				if(part.typePart == PartType.secondPart)
				{
					secondParts.Add(part);
				}
				if(part.typePart == PartType.general)
				{
					thirdParts.Add(part);
				}
			}
		}
		SetPrice();
		RefreshCountOfParts();
	}
	
	//обновление надписей частей крафта и денег
	void RefreshCountOfParts()
	{
		FormatString(ref firstItemMesh, firstParts.Count, false);
		FormatString(ref secondItemMesh, secondParts.Count, false);
		FormatString(ref thirdItemMesh, thirdParts.Count, false);
		FormatString(ref priceMesh, price, true, "");
		if(isPartsEnough())
		{
			activeButton.enabled = true;
			firstItemButton.SetActive(false);
			secondItemButton.SetActive(false);
			thirdItemButton.SetActive(false);
		}
		else
		{
			activeButton.enabled = false;
			if(firstParts.Count == 0)
			{
				firstItemButton.SetActive(true);
			}
			else
			{
				firstItemButton.SetActive(false);
			}
			if(secondParts.Count == 0)
			{
				secondItemButton.SetActive(true);
			}
			else
			{
				secondItemButton.SetActive(false);
			}
			if(thirdParts.Count == 0)
			{
				thirdItemButton.SetActive(true);
			}
			else
			{
				thirdItemButton.SetActive(false);
			}
		}
	}
	
	//форматирование текстМеша, ему дается значение count, опрделение цвета в зависимости от возможности создать объект
	//этого плейна
	void FormatString(ref tk2dTextMesh mesh, int count, bool isMoney, string addedVal = "/1")
	{
		
		if(count > 0 && !isMoney)
		{
			mesh.color = colorAvailable;
		}
		else if(!isMoney)
		{
			mesh.color = colorNotAvailable;
		}
		else
		{
			if(isPartsEnough())
			{
				mesh.color = Color.white;
			}
			else
			{
				mesh.color = Color.red;
			}
		}
		mesh.text = count + addedVal;
		mesh.Commit();
	}
	
	//проверка на доступность крафта декорации
	bool isPartsEnough()
	{
		if(firstParts.Count > 0 && secondParts.Count > 0 && thirdParts.Count > 0 && price <= GlobalVars.money)
		{
			firstItemButton.SetActive(false);
			secondItemButton.SetActive(false);
			thirdItemButton.SetActive(false);
			return true;
		}
		
		return false;
	}
	
	//создание новой декорации, удаление по одной части у каждой из частей, происходит определение грейда и инстанс новой декорации
	//закрытие меню
	public void CraftNewItem()
	{
		if(isPartsEnough())
		{
			RemoveItem(firstParts, firstParts[0]);
			RemoveItem(secondParts, secondParts[0]);
			RemoveItem(thirdParts, thirdParts[0]);
			GlobalVars.money -= price;
			foreach(Decorations decor in GlobalVars.decorations)
			{
				if(decor.setting == setting)
				{
					int rand;
					if(rarity == RarityLevel.common)
					{
						rand = Random.Range(0, decor.commonDecorationPrefab.Length);
						GameObject go = Instantiate(decor.commonDecorationItemPrefab[rand]) as GameObject;
						go.transform.parent = GameObject.Find("Items").transform;
					}
					else if(rarity == RarityLevel.rare)
					{
						rand = Random.Range(0, decor.rareDecorationPrefab.Length);
						GameObject go = Instantiate(decor.rareDecorationItemPrefab[rand]) as GameObject;
						go.transform.parent = GameObject.Find("Items").transform;
					}
					else if(rarity == RarityLevel.unique)
					{
						rand = Random.Range(0, decor.uniqueDecorationPrefab.Length);
						GameObject go = Instantiate(decor.uniqueDecorationItemPrefab[rand]) as GameObject;
						go.transform.parent = GameObject.Find("Items").transform;
					}
				}
			}
			
			GlobalVars.popUpCraftDecorations.gameObject.SetActive(false);
			GlobalVars.cameraStates = CameraStates.normal;
		}
		
		
	}
	
	//удаление итема из массива 
	void RemoveItem(List<CraftDecorationPart> parts, CraftDecorationPart part)
	{
		GlobalVars.inventory.items.Remove(part.GetComponent<InventoryItem>());
		parts.Remove(part);
		Destroy(part.gameObject);
	}
	
	void Update () 
	{
	
	}
}
