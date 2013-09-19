using UnityEngine;
using System.Collections;

//static script с таблицами нужными для квестов, а также здесь находятся энумы и сер. классы для квестов

public enum QuestTypes
{
	writeScriptGenres,
	shootFilmScenes,
	shootFilmGenres,
	releaseFilmBudget,
	releaseFilmGenres,
	vfxFilmGenres,
	none,
}

public enum NeedsTypes
{
	building,
	mapTownOpen,
	filmStaff,
	studioLvl,
	quest,
	none,
}

public static class QuestsTables 
{
	public static MainQuest[] mainTable;
	public static TypesQuests[] typesTable;
	public static ProfitQuests[] profitTable;
	public static NeedsTypeQuests[] needsTypesTable;
}

#region Tables
[System.Serializable]
public class MainQuest
{
	public int id = 0;
	public string header = "null";
	public string description = "null";
	public string quest1ID = "null";
	public QuestTypes quest1Type = QuestTypes.none;
	public string quest2ID = "null";
	public QuestTypes quest2Type = QuestTypes.none;
	public string quest3ID = "null";
	public QuestTypes quest3Type = QuestTypes.none;
	public int profitID = 0;
	public string nextQuestID = "null";
	public string prevQuestID = "null";
	public string needsID = "null";
	public NeedsTypes needsType = NeedsTypes.none;
} 

[System.Serializable]
public class TypesQuests
{
	public string id = "null";
	public string param1 = "null";
	public string param2 = "null";
	public string param3 = "null";
}

[System.Serializable]
public class ProfitQuests
{
	public string id = "null";
	public int coins = 0;
	public int premium = 0;
	public int expToStudio = 0;
}

[System.Serializable]
public class NeedsTypeQuests
{
	public string id = "null";
	public string param1 = "null";
	public string param2 = "null";
	public string param3 = "null";
	public string param4 = "null";
}
#endregion

#region Quest Needs
[System.Serializable]
public class BuildingNeeds
{
	public BuildingType buildingType;
	public int count;
	public int upgradeLvl;
}

[System.Serializable]
public class FilmStaffNeeds
{
	public CharacterType characterType;
	public int count;
	public FilmGenres genres;
	public int genreLvl;
}

[System.Serializable]
public class MapTownOpenNeeds
{
	public string townName;
}

[System.Serializable]
public class StudioLvlNeeds
{
	public int studioLvl;
}
#endregion

#region Quest types ser. classes
[System.Serializable]
public class ReleaseFilmBudgetQuest
{
	public int id;
	public int budgetTaken;
}

[System.Serializable]
public class ReleaseFilmGenresQuest
{
	public int id;
	public FilmGenres genres;
	public Setting setting;
	public int releasedFilmsCount;
}

[System.Serializable]
public class ShootFilmScenesQuest
{
	public int id;
	public FilmGenres genres;
	public Setting setting;
	public int filmToShootCount;
}

[System.Serializable]
public class WriteScriptGenreQuest
{
	public int id;
	public FilmGenres genres;
	public int storyLvl;
	public int scriptsCount;
}

[System.Serializable]
public class VfxFilmGenresQuest
{
	public int id;
	public FilmGenres genres;
	public Setting setting;
	public int moviesToVfxCount;
}

[System.Serializable]
public class QuestStatus
{
	public QuestTypes type = QuestTypes.none;
	public bool isComplete = false;
	public int counter = 0;
	public string text = "";
}
#endregion
