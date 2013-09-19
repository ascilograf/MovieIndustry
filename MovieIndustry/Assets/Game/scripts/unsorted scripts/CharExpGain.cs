using UnityEngine;
using System.Collections;

//в скрипте находится список функций по увеличению опыта персонажей
//в паблик переменных находятся модификаторы, либо фиксированные награды за действия
public class CharExpGain : MonoBehaviour {
	
	//модификаторы
	public int modMakeScript = 10;				
	public int modActorMakeFilm = 10;
	public int modDirectorMakeFilm = 5;
	public int modCameramanMakeFilm = 4;
	public int modAddVisuals = 5;
	public int modMatketing = 2;

	void Awake()
	{
		GlobalVars.charExpGain = this;
	}
	
	//за создание сценария
	public void MakeScriptExpGain(FilmStaff scripter)
	{
		scripter.exp += scripter.lvl * modMakeScript + (((scripter.lvl * modMakeScript)/100) * scripter.bonuses.ExpGainBonus);
	}
	
	//за съемки актеру
	public void ActorMakeFilmExpGain(FilmStaff staff)
	{
		staff.exp += staff.lvl * modActorMakeFilm + ((staff.lvl * modActorMakeFilm)/100) * staff.bonuses.ExpGainBonus;
	}
	
	//за съемки режиссеру
	public void DirectorMakeFilmExpGain(FilmStaff staff)
	{
		staff.exp += staff.lvl * modDirectorMakeFilm + ((staff.lvl * modDirectorMakeFilm)/100) * staff.bonuses.ExpGainBonus;
	}
	
	//за съемки оператору
	public void CameramanMakeFilmExpGain(FilmStaff staff)
	{
		staff.exp += staff.lvl * modCameramanMakeFilm + ((staff.lvl * modCameramanMakeFilm)/100) * staff.bonuses.ExpGainBonus;
	}
	
	//за добавление визуальных спецэффектов
	public void AddVisualsExpGain(FilmStaff staff)
	{
		staff.exp += staff.lvl * modAddVisuals + ((staff.lvl * modAddVisuals)/100) * staff.bonuses.ExpGainBonus;
	}
	
	//за маркетинг
	public void MatketingExpGain(FilmStaff staff)
	{
		staff.exp += staff.lvl * modMatketing + ((staff.lvl * modMatketing)/100) * staff.bonuses.ExpGainBonus;
	}
}
