using UnityEngine;
using System.Collections;

//кнопка выбора сеттинга, аналогична кнопке выбора жанра
public class MarketingButton : MonoBehaviour 
{
	public MarketingTypes type;				
	public MeshRenderer checkMark;	
	public MeshRenderer tooHighForThisStage;
	public int price;
	public int marketingLvl;
	public FirstTypeMarketing firstType;
	public SecondTypeMarketing secondType;
	public tk2dTextMesh priceMesh;
	public tk2dTextMesh resultMesh;
	public tk2dTextMesh timeMesh;
	
	void Start()
	{
        //схватываем параметры из глобалок и показываем
		/*foreach(Marketing m in GlobalVars.marketing)
		{
			if(m.type == type)
			{
				price = m.price;
				result = m.result;
				time = m.time;
				//priceMesh.text = "price: " + m.price;
				//resultMesh.text = "lifetime +" + m.result + "%";
				//timeMesh.text = "time: " + m.time;
				//priceMesh.Commit();
				//resultMesh.Commit();
				//timeMesh.Commit();
			}
		}*/
		switch(type)
		{
		case MarketingTypes.FirstType:
			firstType = GlobalVars.firstTypeMarketing[marketingLvl];
			Utils.SetText(priceMesh, firstType.price + "% from revenue");
			break;
		case MarketingTypes.SecondType:
			secondType = GlobalVars.secondTypeMarketing[marketingLvl];
			Utils.SetText(priceMesh, secondType.price.ToString());
			break;
		}
	}
	
	public void SetPrice(int b)
	{
		if(type == MarketingTypes.FirstType)
		{
			price = (int)((b / 100) * firstType.bonus);
		}
		else if(type == MarketingTypes.SecondType)
		{
			price = secondType.price;
		}
	}
	
	public bool GetChoosen()
	{
		return checkMark.enabled;
	}
	
	public void SetChoosen(bool b)
	{
		checkMark.enabled = b;
	}

}
