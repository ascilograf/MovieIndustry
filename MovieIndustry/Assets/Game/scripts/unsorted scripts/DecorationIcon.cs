using UnityEngine;
using System.Collections;

//содержит иконки собранных декораций
//используется для отображения в меню завершения сцены
//на вход идет сеттинг, рарность и фит, определяется иконка для показа и фит для показа

public class DecorationIcon : MonoBehaviour 
{
	public GameObject[] adventureSetting = new GameObject[3];
	public GameObject[] spaceSetting = new GameObject[3];
	public GameObject[] modernSetting = new GameObject[3];
	public GameObject[] warSetting = new GameObject[3];
	public GameObject[] historicalSetting = new GameObject[3];
	public GameObject[] fantasySetting = new GameObject[3];
	public GameObject[] fits;
	
	public Setting setting;
	public RarityLevel rarity;
	public float fit;
	 
	void OnEnable()
	{
		gameObject.SetActive(false);
		switch(setting)
		{
		case Setting.Adventure:
			adventureSetting[(int)rarity].SetActive(true);
			break;
		case Setting.War:
			warSetting[(int)rarity].SetActive(true);
			break;
		case Setting.Historical:
			historicalSetting[(int)rarity].SetActive(true);
			break;
		case Setting.Modern:
			modernSetting[(int)rarity].SetActive(true);
			break;
		case Setting.Space:
			spaceSetting[(int)rarity].SetActive(true);
			break;
		case Setting.Fantasy:
			fantasySetting[(int)rarity].SetActive(true);
			break;
		}
		if(fit >= 1 && fit < 1.5f)
			fits[1].SetActive(true);
		else if(fit < 1f)
			fits[0].SetActive(true);
		else if(fit >= 1.5f)
			fits[2].SetActive(true);
	}
	
	public void SetIcon(Setting sett, RarityLevel rar, float f)
	{
		setting = sett;
		rarity = rar;
		fit = f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
