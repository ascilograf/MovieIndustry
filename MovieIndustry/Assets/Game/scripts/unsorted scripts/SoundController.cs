using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour 
{
	public AudioClip boxOfficeOpensSound;
	public AudioClip boxOfficeClosesSound;
	public AudioClip finishMakingFilmSound;
	public AudioClip buyNewCitySound;
	public AudioClip collectMoneySound;
	public AudioClip merchClickSound;
	public AudioClip questOpenCloseSound;
	public AudioClip tapOnElementSound;
	public AudioClip mainSoundTheme;
	public AudioClip radialMenuOpenSound;
	public AudioClip radialMenuTapSound;
	public AudioClip swipeSound;
	public AudioClip usaMapSoundTheme;
	public AudioClip acceptButtonTapSound;
	public AudioClip oldTheme;
	
	public AudioSource testSound;
	public AudioSource mainTheme;
	
	public bool showMenu = false;
	// Use this for initialization
	void Start () 
	{
		mainTheme.clip = mainSoundTheme;
		mainTheme.Play();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyUp(KeyCode.Space))
		{
			showMenu = !showMenu;
			if(!showMenu)
				GlobalVars.cameraStates = CameraStates.normal;
		}
	}
	
	void OnGUI()
	{
		
		if(!showMenu)
		{
			return;
		}
		GlobalVars.cameraStates = CameraStates.menu;
		
		if(GUI.Button(new Rect(100, 100, 100, 20), "Box Opens"))
		{
			testSound.clip = boxOfficeOpensSound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(220, 100, 100, 20), "Box Closes"))
		{
			testSound.clip = boxOfficeClosesSound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(340, 100, 100, 20), "Box Lenty"))
		{
			testSound.clip = finishMakingFilmSound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(460, 100, 100, 20), "BuyNewCity"))
		{
			testSound.clip = buyNewCitySound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(580, 100, 100, 20), "Click Money"))
		{
			testSound.clip = collectMoneySound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(100, 140, 100, 20), "Click Merchand"))
		{
			testSound.clip = merchClickSound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(220, 140, 100, 20), "Error-Nomoney"))
		{
			testSound.clip = questOpenCloseSound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(340, 140, 100, 20), "Element"))
		{
			testSound.clip = tapOnElementSound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(460, 140, 100, 20), "Overworld"))
		{
			mainTheme.clip = mainSoundTheme;
			mainTheme.Play();
		}
		if(GUI.Button(new Rect(580, 140, 100, 20), "RadMenuOpen"))
		{
			testSound.clip = radialMenuOpenSound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(100, 180, 100, 20), "RadMenuTap"))
		{
			testSound.clip = radialMenuTapSound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(220, 180, 100, 20), "SwipePlenka"))
		{
			testSound.clip = swipeSound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(340, 180, 100, 20), "Map USA Theme"))
		{
			mainTheme.clip = usaMapSoundTheme;
			mainTheme.Play();
		}
		if(GUI.Button(new Rect(460, 180, 100, 20), "YesButton"))
		{
			testSound.clip = acceptButtonTapSound;
			testSound.Play();
		}
		if(GUI.Button(new Rect(580, 180, 100, 20), "Old Theme"))
		{
			mainTheme.clip = oldTheme;
			mainTheme.Play();
		}
		if(GUI.Button(new Rect(300, 220, 180, 20), "On/Off main theme"))
		{
			if(mainTheme.isPlaying)
				mainTheme.Stop();
			else
				mainTheme.Play();
		}
	}
	
	public void OpenBoxOffice()
	{
		PlaySound(boxOfficeOpensSound);
	}
	
	public void CloseBoxOffice()
	{
		PlaySound(boxOfficeClosesSound);
	}
	
	public void FinishMakingFilmWindow()
	{
		PlaySound(finishMakingFilmSound);
	}
	
	public void BuyNewCity()
	{
		PlaySound(buyNewCitySound);
	}
	
	public void MerchClick()
	{
		PlaySound(merchClickSound);
	}
	
	public void QuestOpenClose()
	{
		PlaySound(questOpenCloseSound);
	}
	
	public void ShowRadialMenu()
	{
		PlaySound(radialMenuOpenSound);
	}
	
	public void RadialMenuTap()
	{
		PlaySound (radialMenuOpenSound);
	}	
	
	public void Swipe()
	{
		PlaySound(swipeSound);
	}
	
	public void CollectMoney()
	{
		PlaySound(collectMoneySound);
	}
	
	public void PlayMapTheme()
	{
		mainTheme.clip = usaMapSoundTheme;
		mainTheme.Play();
	}
	
	public void PlayMainTheme()
	{
		mainTheme.clip = mainSoundTheme;
		mainTheme.Play();
	}
	
	void PlaySound(AudioClip clip)
	{
		testSound.clip = clip;
		testSound.Play();
	}
}
