using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Здесь мы берем паблик-переменные из инспектора и помещаем их в  
//статичный скрипт GlobalVars для доступа во всём проекте
public class GlobalVarsEditor : MonoBehaviour 
{
	public SoundController soundController;
	public Character[] characters;
	public Character[] premiumCharacters;
	public List<BuildingPrefabs> buildPrefabs;						
	public UnderConstruction[] underConstruction;	
	public GameObject commonPersonPrefab;						
	public int money;									
	public int stars;									
	public BoundingBox[] buildedFloorsHeght;
	public StageBoundingBox[] stageBoundingBox; 
	public BoundingBox[] underConstrFloorsHeight;		
	public BuildSubMenu[] buildSubMenu;	
	public List<FilmStaff> scripters;
	public GameObject scenarioPref;
	public PopUpCastingMenu popUpCastingMenu;
	public GameObject popUpHireForEffects;
	public PopUpHireStaff popUpHireStaff;
	public popUpNewScript hireForScript;
	public PopUpCraftDecorations popUpCraftDecorations;
	public PopUpSpeedUpAnyJob popUpSpeedUpAnyJob;
	public PopUpFinishMakingFilm popUpFinishMakingFilm;
	public PopUpStaffLevelUp popUpStaffLevelUp;
	public PopUpFinishAnyJob popUpFinishAnyJob;
	public PopUpShortCut popUpShortCut;
	public PopUpSelectMarketing popUpSelectMarketing;
	public PopUpQuests popUpQuests;
	public PopUpSelectDecoration popUpSelectDecorations;
	public QuestsMainController questMainController;
	public Decorations[] decorations;
	public CraftPartPrefabs[] craftPartsRefabs;
	public Fit[] fits;
	public GameObject filmItemPrefab;
	public MovieRental rental;
	
	public List<AvailableGenre> actorsGenres;
	public List<AvailableGenre> directorsGenres;
	public List<AvailableGenre> cameramansGenres;
	public List<AvailableGenre> scriptersGenres;
	
	public AchievmentsTypes achievments;
	
	public Camera guiCamera;
	public Camera swipeShopCamera;
	public Camera swipeCamera;
	
	public RadialMenuScripters radialScripters;
	public RadialMenuScripters radialHangar;
	public RadialMenuScripters radialPostProd;
	public RadialMenuScripters radialProducers;
	
	public GameObject placeBuildingPrefab;
	public GameObject decorIconPrefab;
	public float[] townGradePercent;
	
	void Awake () 
	{
		Application.targetFrameRate = 30;
		GlobalVars.soundController = soundController;
		GlobalVars.popUpHireStaff = popUpHireStaff;
		GlobalVars.popUpSelectMarketing = popUpSelectMarketing;
		GlobalVars.popUpHireForScript = hireForScript;
		GlobalVars.questMainController = questMainController;
		GlobalVars.popUpQuests = popUpQuests;
		GlobalVars.popUpShortCut = popUpShortCut;
		GlobalVars.popUpFinishAnyJob = popUpFinishAnyJob;
		GlobalVars.popUpStaffLevelUp = popUpStaffLevelUp; 
		GlobalVars.popUpFinishMakingFilm = popUpFinishMakingFilm;
		GlobalVars.decorIconPrefab = decorIconPrefab;
		GlobalVars.popUpSpeedUpAnyJob = popUpSpeedUpAnyJob;
		GlobalVars.placeBuildingPrefab = placeBuildingPrefab;
		GlobalVars.townGradePercent = townGradePercent;
		GlobalVars.stageBoundingBox = stageBoundingBox;
		GlobalVars.commonPersonPrefab = commonPersonPrefab;
		GlobalVars.characters = characters;
		GlobalVars.premiumCharacters = premiumCharacters;
		GlobalVars.popUpSelectDecoration = popUpSelectDecorations;
		GlobalVars.underConstr = underConstruction;
		GlobalVars.money = money;
		GlobalVars.stars = stars;
		GlobalVars.buildedFloorsHeight = buildedFloorsHeght;
		GlobalVars.underConstrFloorHeght = underConstrFloorsHeight;
		GlobalVars.buildSubMenus = buildSubMenu;
		GlobalVars.scenarioPref = scenarioPref;
		GlobalVars.popUpCastingMenu = popUpCastingMenu;
		GlobalVars.popUpCraftDecorations = popUpCraftDecorations;
		GlobalVars.decorations = decorations;
		GlobalVars.fits = fits;
		GlobalVars.filmItemPref = filmItemPrefab;
		GlobalVars.popUpHireEffects = popUpHireForEffects;
		GlobalVars.worldRental = rental;
		GlobalVars.craftPartsRefabs = craftPartsRefabs;
		GlobalVars.actorsGenres = actorsGenres;
		GlobalVars.cameramansGenres = cameramansGenres;
		GlobalVars.directorsGenres = directorsGenres;
		GlobalVars.scriptersGenres = scriptersGenres;
		
		GlobalVars.GUICamera = guiCamera;
		GlobalVars.SwipeCamera = swipeCamera;
		GlobalVars.ShopSwipeCamera = swipeShopCamera;
		
		GlobalVars.radialMenuScripters = radialScripters;
		GlobalVars.radialMenuProducers = radialProducers;
		GlobalVars.radialMenuHangar = radialHangar;
		GlobalVars.radialMenuPostProd = radialPostProd;
			
		Messenger.AddListener("Menu appear", Foo);
		
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			GlobalVars.isMobilePlatform = true;
		}
		else
		{
			GlobalVars.isMobilePlatform = false;
		}
		
		GlobalVars.tapSound = GameObject.Find("Game").audio;
		
		GlobalVars.buildingsPrefabs = buildPrefabs;
		
		float h = Screen.height;
		guiCamera.orthographicSize = h/2;
		Camera.main.orthographicSize = (h/2);//(2 + h/768));
	}
	
	void Foo()
	{
		return;
	}
	
	void Update()
	{
		GlobalVars.SwipeCamera = swipeCamera;
		
		scripters = GlobalVars.allScripters;
		//if(GlobalVars.cameraStates == CameraStates.normal)
		//{
		if(Input.GetMouseButtonUp(0) && GlobalVars.BlockInput)
		{
			Messenger.Broadcast("Finger was lifted");
			GlobalVars.BlockInput = false;
		}
		else
			Utils.GameInput();
		//}
		//if/(GlobalVars.cameraStates == CameraStates.menu)
		///{
		//	Utils.GameInput(GlobalVars.GUICamera);
		//}
	}
	
	string filmName = "";
	
	void OnGUI()
	{
		if(GlobalVars.filmsWithoutNames.Count > 0)
		{
			GlobalVars.cameraStates = CameraStates.menu;
			GUI.Box(new Rect(0,0, Screen.width, Screen.height), "");
			filmName = GUI.TextField(new Rect(Screen.width/2 - 100, Screen.height/2, 200, 20), filmName);
			if(GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 + 30 , 200, 20), "Name film"))
			{
				GlobalVars.filmsWithoutNames[0].name = filmName;
				GlobalVars.popUpFinishAnyJob.SetName(filmName);
				GlobalVars.filmsWithoutNames.Remove(GlobalVars.filmsWithoutNames[0]);
				filmName = "";
				//GlobalVars.cameraStates = CameraStates.normal;
			}
		}
	}
}
