using UnityEngine;
using System.Collections;

//ворклисты на каждое здание, на старте закидываем их в глобальные переменные
public class SWOfficeWorklist : MonoBehaviour {
	
	public OfficeLevels[] scriptersWorklist;								
	public OfficeLevels[] hangarWorklist;						
	public OfficeLevels[] postProdWorklist;					
	public OfficeLevels[] producersWorklist;				
	// Use this for initialization
	void Awake () 
	{
		GlobalVars.scriptersWorklist = scriptersWorklist;
		GlobalVars.hangarWorklist = hangarWorklist;
		GlobalVars.postProdWorklist = postProdWorklist;
		GlobalVars.producersWorklist = producersWorklist;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
