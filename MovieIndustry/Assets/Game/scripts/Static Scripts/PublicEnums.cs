using UnityEngine;
using System.Collections;

//В этом скрипте находятся все списки
public class PublicEnums : MonoBehaviour 
{
	
}

//Список типов анимаций
public enum CharAnimationType
{
	idle,
	idleSpecial,
	walk,
}

//Список типов персонажей
public enum CharacterType
{
	worker,
	cameraman,
	director,
	actor,
	producer,
	scripter,
	postProdWorker,
}

//Список типов зданий
public enum BuildingType
{
	office,
	postproduction,
	scriptWrittersOffice,
	hangar,
	trailer,
	pavillion,
	buildersHut,
}

//Список этажей
public enum Stages
{
	first,
	second,
	third,
	four,
	five,
}

public enum FilmGenres
{
	Drama,
	Comedy,
	Action,
	Romance,
	Scifi,
	Horror,
	none,
}

public enum DoorsLocation
{
	exitDoor,
	floor1,
	floor2,
	floor3,
	floor4,
	floor5,
	ground,
	
}

//Список полов персонажей
public enum Gender
{
	male,
	female,
}

//Список положений камеры
public enum CameraStates
{
	normal,
	menu,
	learning,
}

public enum Position
{
	leftTop,
	leftBottom,
	rightTop,
	rightBottom,
}

//Список типов предметов для инвентаря
public enum ItemType
{
	script,
	questItem,
	decoration,
}

public enum ProgressBarType
{
	build,
	makingScript,
	postProdProgress,
	cinemaUpgrade,
}

public enum Setting
{
	Adventure,
	War,
	Space,
	Historical,
	Fantasy,
	Modern,
	none,
}

public enum TapElementTypes
{
	button,
	checkBox,
}

public enum MarketingTypes
{
	FirstType,
	SecondType,
	none,
}

public enum BonusTypes
{
	GoldenChain,
	Turtleneck,
	Book,
	TVRemote,
}

public enum RarityLevel
{
	common,
	rare,
	unique,
	none,
}

public enum PartType
{
	firstPart,
	secondPart,
	general,
}

public enum TownGrade
{
	small,
	usual,
	huge,
}

public enum OtherActivities
{
	Postproduction,
	Marketing,
}

public enum CityUpgradeType
{
	doubleLifeTime,
	smallRevenueUp,
	bigRevenueUp,
}