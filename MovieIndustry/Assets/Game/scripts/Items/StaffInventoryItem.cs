using UnityEngine;
using System.Collections;

//предмет инвентаря персонажа
public class StaffInventoryItem : MonoBehaviour 
{
	
	public BonusParams bonuses;				//бонусы
	//public GameObject icon;
	// Use this for initialization
	void Start () 
	{
		//находим в массиве предметов спарсенных из файла, если произошло совпадение с выставленным на префабе типом предмета
		//то выставляем параметры
		foreach(BonusParams par in GlobalVars.staffInventoryParams)
		{
			if(par.type == bonuses.type)
			{
				bonuses = par;
			}
		}
	}
}
