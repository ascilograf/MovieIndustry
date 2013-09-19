using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Скрипт в котором собраны все сериализованные паблик классы
public class PublicSerClasses : MonoBehaviour 
{

}

//Общие паараметры персонажей
[System.Serializable]
public class Character
{
	public CharacterType personType;		//тип персонажа
	public GameObject parentObj;			//главный объект
	public int priceMoney;					//цена в деньгам
	public int priceStars;					//цена в звездах
	public Genders[] genders;				//разделение частей тела по полам
}

//Половая спецификация персонажа
[System.Serializable]
public class Genders
{
	public Gender genders;					//пол

	public GameObject[] heads;				//массив голов
	public GameObject[] bodys;				//массив тел	
	public GameObject[] legs;				//массив ног
}

//Параметры зданий 
/*[System.Serializable]
public class Build
{
	public GameObject buildingPrefab;		//префаб здания
	public int workersCount;				//кол-во рабочих необходимых для начала работы
	public GameObject parentPrefab;			//парент-префаб здания
	public float buildTime;					//время строительства
	public int buildPriceMoney;				//стоимость в деньгах
	public int buildPriceStars;				//стоимость в звездах
	//public GameObject[] doors;			//двери в этом здании
}*/

//Параметры для апгрейдов зданий
[System.Serializable]
public class BuildParams
{
	public BuildingType type;
	public ShopItemParams build;
	public List<ShopItemParams> upgrade = new List<ShopItemParams>();
}

[System.Serializable]
public class BuildingPrefabs
{
	public BuildingType type;
	public GameObject parentObjectPrefab;
	public GameObject mainBuildingPrefab;
	public GameObject previewPrefab;
	public GameObject[] upgradePrefabs;
}

/*[System.Serializable]
public class Building
{
	public BuildingType type;
	//public GameObject parentPrefab;
	public GameObject menuObject;
	public List<Build> build;
	//public List<Upgrade> upgrades;
}*/

//Список параметров зданий "под строительством"
[System.Serializable]
public class UnderConstruction
{
	public Stages stage;					//этаж
	public GameObject buildPrefab;			//префаб здания
}

//Ограничивающая рамка
[System.Serializable]
public class BoundingBox
{
	public float xMax;						//макс. значеие по Х
	public float xMin;						//мин. значение по Х
	public float yMax;						//макс. значение по У
	public float yMin;						//мин. значение по У
}

//опыт по жанрам фильмов
[System.Serializable]
public class FilmSkills
{	
	public FilmGenres genre;				//жанр
	public  GameObject[] progressBar;		//прогрессБар
	public int skill;						//уровень умения
	public int skillToUp;					//сколько прибавить
	public Collider plusButton;
	public tk2dTextMesh skillNameMesh;
}

//опыт по жанрам фильмов
[System.Serializable]
public class OtherSkills
{	
	public OtherActivities activity;				//жанр
	public  GameObject[] progressBar;		//прогрессБар
	public int skill;						//уровень умения
	public int skillToUp;
	public Collider plusButton;
	public tk2dTextMesh skillNameMesh;
}

//подменю строительства
[System.Serializable]
public class BuildSubMenu
{
	public BuildingType type;				//тип зданий
	public GameObject[] buttons;			//кнопки зданий
}

//фиты
[System.Serializable]
public class Fit
{
	public FilmGenres genre;				//жанр для которого выставляются фиты
	public List<Setting> perfectFit;		//перфект фиты для жанра
	public List<Setting> badFit;			//бэд фиты для жанра
}

//все сеттинги с подходящими префабами декораций
[System.Serializable]
public class Decorations
{
	public Setting setting;
	public GameObject[] commonDecorationPrefab;
	public GameObject[] commonDecorationItemPrefab;
	public GameObject[] rareDecorationPrefab;
	public GameObject[] rareDecorationItemPrefab;
	public GameObject[] uniqueDecorationPrefab;
	public GameObject[] uniqueDecorationItemPrefab;
}

[System.Serializable]
public class FirstTypeMarketing
{ 
	public float bonus;
	public float price;
	public int time;
	public int failChance;
	public int zeroChance;
	public int bonusChance;
}

[System.Serializable]
public class SecondTypeMarketing
{ 
	public int price;
	public int time;
	public int failChance;
	public int oneTownChance;
	public int twoTownsChance;
	public int threeTownsChance;
}

[System.Serializable]
public class Works
{
	public int stage;
	public string name;
	public int timeSec;
	public int profit;
	public int expForStudio;
	public int expForWorker;
}

[System.Serializable]
public class OfficeLevels
{
	public int lvl;
	public Works[] works;
}
	
[System.Serializable]
public class OfficeWorklist
{
	public BuildingType officeType;
	public OfficeLevels[] levels;
}

[System.Serializable]
public class RadialMenu
{
	public GameObject menu;
	public MeshRenderer button;
	public float angleFrom;
	public float angleTo;
}

[System.Serializable]
public class AvailableGenre
{
	public FilmGenres genre;
	public int max;
}

[System.Serializable]
public class StaffInventoryItemParams
{
	public bool isSlotOpen;
	public MeshRenderer emptySlotIcon;
	public MeshRenderer lockedSlotIcon;
	public StaffInventoryItem item;
	public Collider button;
}

[System.Serializable]
public class BonusParams
{
	public BonusTypes type;
	public string itemName;
	public int ExpGainBonus;
	public int MoneyGainBonus;
	public int[] genresBonuses = new int[2];
	public int priceMoney;
	public int priceStars;
	public List<CharacterType> characters = new List<CharacterType>();
	public float useTime;
}

[System.Serializable]
public class AchievmentsProfit
{
	public string description = "";
	public int stars = 0;
	public int expToStudio = 0;
}

[System.Serializable]
public class AchievmentsTypes
{
	public string name = " ";
	public int lvl;
	public List<AchievmentsProfit> achievmentsProfit = new List<AchievmentsProfit>();
}

[System.Serializable]
public class ShopItemParams
{
	public string name;
	public string description;
	public int maxCount;
	public int price;
	public int workersCount;
	public int time;
	public int stars;
}

[System.Serializable]
public class TimeLocalization
{
	public string oneDay;
	public string manyDays;
	public string d;
	public string oneHour;
	public string manyHours;
	public string h;
	public string oneMinute;
	public string manyMinuts;
	public string m;
	public string oneSecond;
	public string manySeconds;
	public string s;
}

[System.Serializable]
public class LearningStep
{
	public string caption;
	public GameObject button;
	public Position position;
}

[System.Serializable]
public class WhatNeedsForDecoration
{
	public Setting setting;
	public RarityLevel rarity;
	public tk2dTextMesh firstItemMesh;
	public tk2dTextMesh secondItemMesh;
	public tk2dTextMesh thirdItemMesh;	
}

[System.Serializable]
public class CraftPartPrefabs
{
	public RarityLevel rarity;
	public GameObject[] prefabs;
}

[System.Serializable]
public class Costumes
{
	public Setting setting;
	public GameObject[] commonMale;
	public GameObject[] commonFemale;
	public GameObject[] rareMale;
	public GameObject[] rareFemale;
	public GameObject[] uniqueMale;
	public GameObject[] uniqueFemale;
}

[System.Serializable]
public class StaffSkills
{
	public FilmGenres genre;
	public int skill;
}

[System.Serializable]
public class SubMenu
{
	public string type;
	public GameObject subMenu;
	public GameObject tab;
	public SearchStaffPlane searchPlane;
}

[System.Serializable]
public class StageBoundingBox
{
	public BuildingType buildingType;
	public Stages stage;
	public BoundingBox[] box;
}

[System.Serializable]
public class ImportedString
{
	public string caption = "empty";
	public string text = "empty";
}

[System.Serializable]
public class HireForFilmPlane
{
	public GameObject plane;
	public GameObject[] states;
	public tk2dTextMesh[] names;
	public tk2dTextMesh[] prices;
}

[System.Serializable]
public class CostumePosition
{
	public tk2dSpriteAnimator mirror;
	public Waypoint waypoint;
	public Vector3 position;
}

[System.Serializable]
public class CraftDecorTab
{
	public Setting sett;
	public GameObject tabButton;
	public GameObject infos;
	public GameObject tab;
	public MeshRenderer activeTab;
	public CraftDecorItem[] items;
}

[System.Serializable]
public class CraftItemsIconsWithSetting
{
	public Setting setting;
	public GameObject[] iconsCommon;
	public GameObject[] iconsRare;
	public GameObject[] iconsUnique;
	public GameObject[] itemsPrefabsCommon;
	public GameObject[] itemsPrefabsRare;
	public GameObject[] itemsPrefabsUnique;
}

[System.Serializable]
public class ProgressBar
{
	public GameObject parentObj;
	public GameObject[] stars;
}

[System.Serializable]
public class CityUpgradeItem
{
	public CityUpgradeType type;
}

[System.Serializable]
public class Waypoint
{
	public Transform pointPos;
	public bool isTeleportPoint;
}

[System.Serializable]
public class TownParams
{
	public int revenuePerMin;
	public int time;
}

[System.Serializable]
public class Scene
{
	public Setting setting;
	public RarityLevel rarity;
	public UniversalMiniPlane plane;
}