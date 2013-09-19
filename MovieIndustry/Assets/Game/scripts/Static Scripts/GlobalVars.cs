
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Список глобальных статичных переменных для повсеместного использования
public static class GlobalVars 
{
	public static SoundController soundController;
	public static Character[] characters;								//Префабы и параметры персонажей
	public static GameObject commonPersonPrefab;
	public static Character[] premiumCharacters;						//префабы и параметры премиумным персонажей
	public static List<BuildParams> buildParams = 						//Префабы и параметры построенных зданий
				  new List<BuildParams>();				
	public static List<BuildingPrefabs> buildingsPrefabs;
	public static UnderConstruction[] underConstr;						//Префабы и параметры строящихся зданий
	public static GameObject hireMenu;									//меню наёма
	public static GameObject buildMenu;									//меню строительства
	public static CharacterType hirePerson;								//тип персонажа для наёма
	public static BoundingBox[] buildedFloorsHeight;					//Высоты и ширина этажей построенных зданий
	public static StageBoundingBox[] stageBoundingBox;
	public static BoundingBox[] underConstrFloorHeght;					//Высоты и ширина этажей не построенных зданий	
	public static BuildSubMenu[] buildSubMenus;							//Подменю строений
	public static TownParams[] townParams = new TownParams[3];
	public static Items[] merchItems = new Items[3];
	public static int[] scenesTimers = new int[5];
	
	public static int money;											//деньги
	public static int stars;											//звезды
	public static GameObject currMenu;									//Выбранное меню
	public static CameraStates cameraStates = CameraStates.normal;		//стейты камеры
	public static GameObject buildingToConstruct;						//строение, которое надо построить
	public static List<FilmStaff> allScripters = new List<FilmStaff>();	//все сценаристы
	public static GameObject scenarioPref;								//префаб сценария
	public static GameObject filmItemPref;								//префаб фильма
	public static Inventory inventory;
	public static UIScrollList ActionStack;
	public static GameObject activityStackItem;
	public static UIScrollList ScriptWrittersPopUp;
	public static GameObject scriptWrittersPopUpItem;
	public static GameObject pageControlItem;							//префаб pageControl'а
	public static GameObject popUpHireEffects;
	public static Decorations[] decorations;
	public static CraftPartPrefabs[] craftPartsRefabs;
	public static Fit[] fits; 											//фиты
	public static FirstTypeMarketing[] firstTypeMarketing = new FirstTypeMarketing[5];				//типы маркетинга и их параметры
	public static SecondTypeMarketing[] secondTypeMarketing = new SecondTypeMarketing[5];
	public static MovieRental worldRental;								//прокат фильмов по миру
	public static int cinemasRevenue = 5;								//доп. выручка от кол-ва кинотеатров
	public static float[] townGradePercent;								//проценты от городов в ревенью, от мЕньшего к бОльшему
	public static ExpGain expGain;
	public static CharExpGain charExpGain;
	public static int exp = 0 ;
	public static int nextLvlExp = 0;
	public static int studioLvl = 0;
	
	public static BoundingBox cameraFrame;
	
	public static BuildingType[] buildingTypes = 	{BuildingType.buildersHut, BuildingType.hangar, BuildingType.office, BuildingType.pavillion,
													BuildingType.postproduction, BuildingType.scriptWrittersOffice, BuildingType.trailer}; 
	
	public static Camera GUICamera;
	public static Camera SwipeCamera;
	public static Camera ShopSwipeCamera;
	
	public static AudioSource tapSound;
	public static GameObject defaultLayerTarget;
	public static GameObject guiLayerTarget;
	public static GameObject decorIconPrefab;
	
	public static bool isMobilePlatform;
	
	public static OfficeLevels[] scriptersWorklist;
	public static OfficeLevels[] producersWorklist;
	public static OfficeLevels[] hangarWorklist;
	public static OfficeLevels[] postProdWorklist;
	
	public static RadialMenuScripters radialMenuScripters;
	public static RadialMenuScripters radialMenuProducers;
	public static RadialMenuScripters radialMenuHangar;
	public static RadialMenuScripters radialMenuPostProd;
	public static PopUpCastingMenu popUpCastingMenu;
	public static popUpStageWork popUpStageWork;
	public static PopUpIncrStaffLvl popUpIncrStafLvl;
	public static PopUpHireStaff popUpHireStaff;
	public static PopUpBreakthrough popUpBreakthrough;
	public static PopUpSelectDecoration popUpSelectDecoration;
	public static PopUpCraftDecorations popUpCraftDecorations;
	public static PopUpSpeedUpAnyJob popUpSpeedUpAnyJob;
	public static PopUpFinishMakingFilm popUpFinishMakingFilm;
	public static PopUpStaffLevelUp popUpStaffLevelUp;
	public static PopUpFinishAnyJob popUpFinishAnyJob;
	public static PopUpShortCut popUpShortCut;
	public static popUpNewScript popUpHireForScript;
	public static PopUpSelectMarketing popUpSelectMarketing;
	public static PopUpQuests popUpQuests;
	public static QuestsMainController questMainController;
	
	public static List<AvailableGenre> scriptersGenres = new List<AvailableGenre>(); 
	public static List<AvailableGenre> actorsGenres = new List<AvailableGenre>(); 
	public static List<AvailableGenre> directorsGenres = new List<AvailableGenre>(); 
	public static List<AvailableGenre> cameramansGenres = new List<AvailableGenre>(); 
	public static List<AvailableGenre> producersGenres = new List<AvailableGenre>(); 
	public static GroundWaypoint[] allGroundWaypoints;
	
	public static List<BonusParams> staffInventoryParams = new List<BonusParams>();
	public static Achievments achievments;
	
	public static List<Scenario> filmsWithoutNames = new List<Scenario>();
	public static GameObject buttonForLearning;
	
	public static float tapTremble = 50;
	
	public static GameObject placeBuildingPrefab;
	
	//константы
	public const float STAFF_ANIMATIONS_SPEED = 10;
	public const int BUILDING_LAYER = 149;							//слой бэков зданий
	public const int DECOR1_LAYER = -1;								//первый слой декораций, не перекрывают персонажей, находятся за персонажами 
	public const int DECOR2_LAYER = -50;							//второй слой декораций, перекрывают персонажей, находятся перед персонажами
	public const int CHARACTER_IN_BUILDING_LAYER = 125;				//слой персонажей в здании
	public const int CHARACTER_FREE_LAYER = -100;						//слой персонажей вне здания	
	public const int SEEKER_DISTANCE = 3;
	
	static bool blockInput;
	
	public static bool BlockInput
	{
		get{	return blockInput;}
		set{	blockInput = value;}
	}
}
