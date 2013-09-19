using UnityEngine;
using System.Collections;

//на старте подгружает параметры из локкита в соответствии с выставленным на компоненте типом здания
//потом используется магазином для определения цены, времени и прочих параметров строительства
//также идет проверка на доступность кнопки


public class PopUpShopElement : MonoBehaviour 
{
	public ShopItemParams shopParams;
	
	public tk2dTextMesh textName;
	public tk2dTextMesh textBuilt;
	public tk2dTextMesh textDescription;
	public tk2dTextMesh textPrice;
	public tk2dTextMesh textTime;
	public tk2dTextMesh textWorkers;
	
	public BuildingType buildingType;
	public GameObject button;

	public MeshRenderer iconAvailable;
	public bool isAvailable;
	public bool isTapped;
	
	
	void Start () 
	{
		foreach(BuildParams bp in GlobalVars.buildParams)
		{
			if(bp.type == buildingType)
			{
				shopParams = bp.build;
			}
		}
		
		textPrice.text = shopParams.price.ToString();
		textTime.text = shopParams.time.ToString();
		textWorkers.text = shopParams.workersCount.ToString();
	}
	
	void Update()
	{
		CheckAvailityOfButton();
		CheckParams();
	}
	
	void CheckParams()
	{
		string s = "";
		switch(buildingType)
		{
		case BuildingType.office:
			s = "Offices";
			break;
		case BuildingType.postproduction:
			s = "postProdOffice";
			break;
		case BuildingType.pavillion:
			s = "Pavillion";
			break;
		case BuildingType.scriptWrittersOffice:
			s = "ScriptWritters";
			break;
		case BuildingType.trailer:
			s = "Trailer";
			break;
		case BuildingType.buildersHut:
			s = "buildersHut";
			break;
			
		}
		GameObject[] go = GameObject.FindGameObjectsWithTag(s);
		int i = 0;
		foreach(GameObject g in go)
		{
			if(g.GetComponent<Construct>())
			{
				i++;
			}
		}
		textBuilt.text = i + "/" + shopParams.maxCount;
		textBuilt.Commit();
	}
	
	
	void CheckAvailityOfButton()
	{
		if(GlobalVars.money < shopParams.price)
		{
			iconAvailable.enabled = false;
		}
		else
		{
			iconAvailable.enabled = true;
		}
	}
	
	
}
