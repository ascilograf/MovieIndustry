using UnityEngine;
using System.Collections;

//контроль прогресса студии
//определение процентного соотношения текущего опыта к максимальному, в зависимости от этого
//отодвигается правая часть прогресс бара, центральная часть устанавливается в середину между левой и правой, растягивается до заполнения

public class ExpProgress : MonoBehaviour {
	
	public tk2dUIProgressBar progressBar;
	public int temp;

	void Start () 
	{
		
	}
	

	void Update () 
	{
		ChangeProgress();
	}
	
	float ProgressInPercents()
	{
		int percents = GlobalVars.exp / (GlobalVars.nextLvlExp/100);
		return percents;
	}
	
	void ChangeProgress()
	{
		GlobalVars.expGain.CheckLvl();
		/*Vector3 v3 = rightPart.localPosition;
		v3.x = ProgressInPercents() * 2 + 75;
		if(v3.x < 85)
		{
			v3.x = 86;
		}
		rightPart.localPosition = v3;
		
		v3 = middlePart.localPosition;
		v3.x = Vector3.Distance(rightPart.localPosition,leftPart.localPosition)/2 + 75;
		
		middlePart.localPosition = v3;
		middlePart.localScale = new Vector3(ProgressInPercents() / 10, 1, 1);*/
		progressBar.Value = ProgressInPercents()/100;
	}
}
