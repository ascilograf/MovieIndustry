using UnityEngine;
using System;
using System.Collections;

//квест который имеет в себе слушателя, слушатель вызывает инстанс квест-тикета по id

public class WaitForStartQuest : MonoBehaviour 
{
	public NeedsTypes type;
	public MainQuest mainQuest;
	
	public BuildingNeeds buildingNeeds;
	public FilmStaffNeeds filmStaffNeeds;
	public MapTownOpenNeeds mapTownOpenNeeds;
	public StudioLvlNeeds studioLvlNeeds;
	
	public void Activate(int id)
	{
		foreach(MainQuest m in QuestsTables.mainTable)
		{
			if(m.needsID == id.ToString())
			{
				mainQuest = m;
				switch(type)
				{
				case NeedsTypes.building:
					BuildNeedsStart(m.id);
					break;
				case NeedsTypes.filmStaff:
					break;
				case NeedsTypes.mapTownOpen:
					break;
				case NeedsTypes.studioLvl:
					break;
				}
			}
		}
	}
	
	#region
	void BuildNeedsStart(int id)
	{
		type = NeedsTypes.building;
		foreach(NeedsTypeQuests n in QuestsTables.needsTypesTable)
		{
			if(n.id == id.ToString())
			{
				buildingNeeds.buildingType = (BuildingType)Enum.Parse(typeof(BuildingType), n.param1);
				buildingNeeds.count = int.Parse(n.param2);
				buildingNeeds.upgradeLvl = int.Parse(n.param3);
				Messenger<Construct>.AddListener("buildUpgradeBuilding", BuildNeedsFinish);
			}
		}
	}
	
	void BuildNeedsFinish(Construct building)
	{
		if(building.type == buildingNeeds.buildingType && ((int)building.stage + 1) >= buildingNeeds.upgradeLvl)
		{
			Messenger<Construct>.RemoveListener("buildUpgradeBuilding", BuildNeedsFinish);
			GlobalVars.questMainController.InstantiateQuestWithId(mainQuest.id);
			Destroy(gameObject);
		}
	}
	
	#endregion
	
	
}
