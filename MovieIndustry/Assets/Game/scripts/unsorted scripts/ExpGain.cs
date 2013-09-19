using UnityEngine;
using System.Collections;

//в скрипте находится список функций по увеличению опыта студии
//в паблик переменных находятся модификаторы, либо фиксированные награды за действия
public class ExpGain : MonoBehaviour 
{
	public int studioLvl;						//текущий уровень студии
	public int currExp;							//текущий опыт
	public int nextLvlExp;						//следующий уровень будет при опыте равном этой переменной
	
	//модификаторы опыта
	public float modHireStaff;					//для найма персонажа					
	public float modBuild;						//для постройки здания
	public float modScriptWriting;				//для написания сценария
	public float modVisuals;					//для добавления эффектов фильму
	public float modBuildNewCinema;				//для добавления нового кинотеатра на участок
	public float modFinishingFilm;				//для окончания фильма
	public float modStudioLvl;					//для увдечиния предела набора опыта
	
	public int expForRental;					//опыт за прокат
	public int expForDecorChanging;				//опыт за смену декорация
	public int expForBuyState;					//опыт за покупку нового штата (участка с кинотеатрами)
	
	public GameObject popUpLvlUp;	
	
	public GameObject lvlExpProgress;
	tk2dTextMesh expToLvl;// = new tk2dTextMesh();
	tk2dTextMesh lvl;// = new tk2dTextMesh();

	void Awake () 
	{
		GlobalVars.expGain = this;
		GlobalVars.studioLvl = studioLvl;
		
	}
	
	void Start()
	{
		GameObject go = lvlExpProgress.transform.FindChild("expProgress").gameObject;
		expToLvl = go.GetComponent<tk2dTextMesh>();
		print (go.name);
		go = lvlExpProgress.transform.FindChild("LvLNumber").gameObject;
		lvl = go.GetComponent<tk2dTextMesh>();
		print (go.name);
		CheckLvl();
	}
	
	void Update () 
	{
		studioLvl = GlobalVars.studioLvl;
		currExp = GlobalVars.exp;
		nextLvlExp = GlobalVars.nextLvlExp;
		
	}
	
	#region Функции повышения уровня, все они работают по принципу: на вход идет переменная, применяется формула, увеличивается опыт 
	
	
	public void gainForHireStaff(int price)
	{
		int k = (int)(price/modHireStaff);
		if(k <= 0)
		{
			k = 1;
		}
		StartCoroutine(addExpSmooth(k));
		CheckLvl();
	}
	
	public void gainForBuild(int time)
	{
		int k = (int)(time/modBuild);
		if(k <= 0)
		{
			k = 1;
		}
		StartCoroutine(addExpSmooth(k));
		CheckLvl();
	}
	
	public void gainForScriptWriting(int story)
	{
		int k = (int)(story/modScriptWriting);
		if(k <= 0)
		{
			k = 1;
		}
		StartCoroutine(addExpSmooth(k));
		CheckLvl();
	}
	
	public void gainForDecorChanging()
	{
		StartCoroutine(addExpSmooth(expForDecorChanging));
		CheckLvl();
	}
	
	public void gainForPostProd(int visuals)
	{
		StartCoroutine(addExpSmooth((int)(visuals/modVisuals)));
		CheckLvl();
	}
	
	public void gainForRental()
	{
		StartCoroutine(addExpSmooth((int)(expForRental)));
		CheckLvl();
	}
	
	public void gainForBuildNewCinema(int price)
	{
		StartCoroutine(addExpSmooth((int)(price/modBuildNewCinema)));
		CheckLvl();
	}
	
	public void gainForNewState()
	{
		StartCoroutine(addExpSmooth(expForBuyState));
		CheckLvl();
	}
	
	public void gainForWork(int e)
	{
		StartCoroutine(addExpSmooth(e));
		CheckLvl();
	}
	
	public void gainForFinishingFilm(FilmItem item)
	{
		StartCoroutine(addExpSmooth((int)((item.acting + item.direction + item.cinematography)/modFinishingFilm)));
		CheckLvl();
	}
	
	#endregion
	
	//проверка текущего уровня, если набран порог повышения уровня - увеличиваем уровень, повышаем порог набора.
	public void CheckLvl()
	{
		if(GlobalVars.exp >= GlobalVars.nextLvlExp)
		{
			GlobalVars.studioLvl++;
			GlobalVars.exp = GlobalVars.exp - GlobalVars.nextLvlExp;
			GlobalVars.nextLvlExp = (int)(modStudioLvl*(GlobalVars.studioLvl - 1));
			//Utils.ShowHideObject(popUpLvlUp, true);
		}
		SetProgressBarsToValue();
	}
	
	void SetProgressBarsToValue()
	{
		lvl.text = GlobalVars.studioLvl.ToString();
		lvl.Commit();
		
		expToLvl.text = GlobalVars.exp + " / " + GlobalVars.nextLvlExp;
		expToLvl.Commit();
	}
	
	
	IEnumerator addExpSmooth(int exp, float t = 2)
	{
		while(true)
		{
			yield return new WaitForSeconds(0.1f * Time.deltaTime);
			SetProgressBarsToValue();
			if(t <= 0)
			{
				yield break;
			}
			GlobalVars.exp += (int)(exp/20f);
			t -= 0.1f;
			yield return 0;
		}
	}
}
