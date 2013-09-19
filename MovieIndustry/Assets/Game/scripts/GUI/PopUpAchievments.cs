using UnityEngine;
using System.Collections;

//отслеживает прогресс по ачивкам.
public class PopUpAchievments : MonoBehaviour 
{
	public GameObject[] progress;			//список пернт-объектов с прогрессбарами и надписями
	
	//проверка ачивок, для каждого прогрессбара, присваиваются названия и степень выполнения ачивки.
	void CheckAllAchievments()
	{
		for(int i = 0; i < GlobalVars.achievments.achievments.Count; i++)
		{
			tk2dTextMesh text = progress[i].GetComponentInChildren<tk2dTextMesh>();
			UIProgressBar bar = progress[i].GetComponentInChildren<UIProgressBar>();
			int index = GlobalVars.achievments.achievments[i].lvl;
			if(index < 0)
			{
				index = 0;
			}
			if(index > 2)
			{
				index = 2;
			}
			text.text = GlobalVars.achievments.achievments[i].achievmentsProfit[index].description;
			text.Commit();
			bar.Value = (GlobalVars.achievments.achievments[i].lvl * 0.333f);
		} 
	}
	
	void Update () 
	{
		//если объект активен - каждый кадр проверяем прогресс, если нажали на ЛКМ - закрываем меню.
		CheckAllAchievments();
		if(Input.GetMouseButtonDown(0))
		{
			gameObject.SetActive(false);
			GlobalVars.cameraStates = CameraStates.normal;
		}
	}
}
