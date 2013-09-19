using UnityEngine;
using System.Collections;

//пейдж контрол, сюда передаются значения выбран, или нет персонаж, указанный как parent, 
//если да, то закрашиваем кружок.
public class PageControl : MonoBehaviour 
{
	public GameObject parent;				//объект слежения
	public bool isChecked;					//выбран/Нет
	public MeshRenderer mesh;				//меш кружка
	void Start () {
	
	}
	
	void Update () 
	{
		if(isChecked)
		{
			mesh.enabled = true;
		}
		else
		{
			mesh.enabled = false;
		}
	}
}
