using UnityEngine;
using System.Collections;

//Слушатель остальных команд, подобен GUIListener, но создан для остальных задач
public class OtherListeners : MonoBehaviour {

	//хапускаем слушателей команд
	void Start () 
	{
		Messenger.AddListener("Refresh Scripters List", RefreshScriptersList);
		Messenger<GameObject, float>.AddListener("New Activity Stack", CreateActvityStackItem);
		Messenger<MovieRentalItem>.AddListener("Start Cinema Upgrade", StartCinemaUpgrade);
	}
	
	//обновить список сценаристов через поиск по всем сценаристам на сцене
	void RefreshScriptersList()
	{
		GlobalVars.allScripters.Clear();
		GameObject[] go = GameObject.FindGameObjectsWithTag("scripter");
		for(int i = 0; i < go.Length; i++)
		{
			if(go[i].GetComponent<FilmStaff>() != null)
			{
				GlobalVars.allScripters.Add(go[i].GetComponent<FilmStaff>());
			}
		}
	}
	
	//создать новый итем активити стэка, в зависимости от парент-объекта - выставляем нужный тип итему и запускаем прогрессбар на итеме
	void CreateActvityStackItem(GameObject parent, float time)
	{
		GameObject go = Instantiate(GlobalVars.activityStackItem) as GameObject;
		ActivityStackItem item = go.GetComponent<ActivityStackItem>();
		item.parentObj = parent;
		
		GlobalVars.ActionStack.AddItem(go);
		if(parent.GetComponent<Construct>() != null)
		{
			item.constr = parent.GetComponent<Construct>();
			item.type = ProgressBarType.build;
		}
		else if(parent.GetComponent<ScriptersOfficeStage>() != null)
		{
			item.scriptWrContr = parent.GetComponent<ScriptersOfficeStage>();
			item.type = ProgressBarType.makingScript;
		}
		else if(parent.GetComponent<PostProdOfficeStage>() != null)
		{
			item.ppOffice = parent.GetComponent<PostProdOfficeStage>();
			item.type = ProgressBarType.postProdProgress;
		}
		else if(parent.GetComponent<MovieRentalItem>() != null)
		{
			print ("QWEQWE");
			item.rentalItem = parent.GetComponent<MovieRentalItem>();
			item.type = ProgressBarType.cinemaUpgrade;
		}
		item.StartProgressBar(time);
	}
	
	public void StartCinemaUpgrade(MovieRentalItem item)
	{
		StartCoroutine(CinemaUpgrade(item));
	}
	
	IEnumerator CinemaUpgrade(MovieRentalItem item)
	{
		while(true)
		{
			if(item.tempTime <= 0)
			{
				item.timeToUpgrade *= item.timeToUpgradeModifier;
				item.iconBuyCinema.collider.enabled = true;
				item.cinemaCount++;
				GlobalVars.cinemasRevenue += 5;
				yield break;
			}
			item.tempTime -= Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
			yield return 0;
		}
	}
}
