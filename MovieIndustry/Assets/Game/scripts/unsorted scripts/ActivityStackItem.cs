using UnityEngine;
using System.Collections;

//Итем активити стэка, управление прогрессом выполнения тех или иных задач, 
//каждый кадр считывает текущий прогресс по сравнению с максимальным в процентах
//и выставляет значение прогресса в соотвтствии с этим (от 0 до 1, где 0 = 0%, а 1 = 100%).
public class ActivityStackItem : MonoBehaviour 
{
	public UIProgressBar pb;									//прогресс бар	
	public ProgressBarType type;								//тип прогресс бара
	public GameObject parentObj;								//откуда берется время										
	public Construct constr;									//ссылка на конструктор, если идет строительство
	public ScriptersOfficeStage scriptWrContr;			//ссылка на СкриптОффис, если идет написание сценария
	public PostProdOfficeStage ppOffice;
	public MovieRentalItem rentalItem;

	
	//Начать использование прогрессбара, в зависимости от типа стэка
	public void StartProgressBar(float time)
	{
		switch (type)
		{
		case ProgressBarType.build:
			StartCoroutine(BuildProgress(time));
			break;
		case ProgressBarType.makingScript:
			StartCoroutine(ScriptProgress(time));
			break;
		case ProgressBarType.postProdProgress:
			StartCoroutine(PostProdProgress(time));
			break;
		case ProgressBarType.cinemaUpgrade:
			StartCoroutine(CinemaUpgradeProgress(time));
			break;	
		}
	}

	//Корутина отслеживания прогресса строительства
	IEnumerator BuildProgress(float time)
	{
		while(true)
		{
			float k = constr.timeToBuild/(time/100);
			pb.Value = k / 100;
			//print(pb.Value);
			//если время вышло, то удаляем итем из списка и уничтожаем объект
			if(pb.Value <= 0)
			{
				GlobalVars.ActionStack.RemoveItem(GetComponent<UIListItem>(), true);
				yield break;
			}
			yield return 0;
		}
	}
	
	//Корутина отслеживания прогресса написания сценария
	IEnumerator ScriptProgress(float time)
	{
		while(true)
		{
			float k = scriptWrContr.time/(time/100);
			pb.Value = k / 100;
			//print(pb.Value);
			//если время вышло, то удаляем итем из списка и уничтожаем объект
			if(pb.Value <= 0)
			{
				GlobalVars.ActionStack.RemoveItem(GetComponent<UIListItem>(), true);
				yield break;
			}
			yield return 0;
		}
	}
	
	//корутина отслеживания прогресса наложения эффектов на фильм
	IEnumerator PostProdProgress(float time)
	{
		while(true)
		{
			float k = ppOffice.time/(time/100);
			pb.Value = k / 100;
			//print(pb.Value);
			//если время вышло, то удаляем итем из списка и уничтожаем объект
			if(pb.Value <= 0)
			{
				GlobalVars.ActionStack.RemoveItem(GetComponent<UIListItem>(), true);
				yield break;
			}
			yield return 0;
		}
	}
	
	//корутина отслеживания прогресса покупки кинотеатра на участке
	IEnumerator CinemaUpgradeProgress(float time)
	{
		while(true)
		{
			float k = rentalItem.tempTime/(time/100);
			pb.Value = k / 100;
			//print(pb.Value);
			//если время вышло, то удаляем итем из списка и уничтожаем объект
			if(pb.Value <= 0)
			{
				GlobalVars.ActionStack.RemoveItem(GetComponent<UIListItem>(), true);
				yield break;
			}
			yield return 0;
		}
	}
}
