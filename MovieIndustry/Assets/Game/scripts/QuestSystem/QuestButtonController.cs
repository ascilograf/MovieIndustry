using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//контроль квест-тикета, по активации находит id квеста в основной таблице квестов
//определяет тип квеста и в зависимости от него запускает нужного слушателя и заполняет нужные параметры
//при получении сообщения либо накручивает счетчик, либо просто выполняет задание
//при накручивании счетчика происходит проверка на выполнение, если выполнение произошло, то иконка меняется на галку
//и при вызове бабла квеста он будет бабл на выполнение, после прожатия на бабле собрать лут - тикет уничтожается
//если у квеста было продолжение в другом - перед уничтожением происходит создание следующего тикета

public class QuestButtonController : MonoBehaviour 
{
	public List<QuestStatus> quests;
	public MeshRenderer activeIcon;
	public MeshRenderer finishedIcon;
	
	public int questId;
	
	public ReleaseFilmBudgetQuest releaseFilmBudget;
	public ReleaseFilmGenresQuest releaseFilmGenres;
	public ShootFilmScenesQuest shootFilmScenes;
	public VfxFilmGenresQuest vfxFilmGenres;
	public WriteScriptGenreQuest writeScriptGenres;
	
	public MainQuest mainQuest;
	public ProfitQuests profit;
	
	void Start()
	{
		GlobalVars.soundController.QuestOpenClose();
	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void OnDisable()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void OnDestroy()
	{
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject g)
	{
		if(g == gameObject && GlobalVars.cameraStates == CameraStates.normal)
		{
			activeIcon.enabled = false;
			GetQuestText();
			if(!Complete())
				GlobalVars.popUpQuests.SetParamsForStart(this, mainQuest.header, "", 
				"", profit.coins.ToString(), profit.expToStudio.ToString(), profit.premium.ToString());
			else
				GlobalVars.popUpQuests.SetParamsForFinish(this, mainQuest.header, "", 
				"", profit.coins.ToString(), profit.expToStudio.ToString(), profit.premium.ToString());
		}
	}
	
	public void ActivateQuest(int id)
	{
		finishedIcon.enabled = false;
		activeIcon.enabled = true;
		foreach(MainQuest m in QuestsTables.mainTable)
		{
			if(m.id == id)
			{
				mainQuest = m;
				QuestStatus quest = new QuestStatus();
				switch(m.quest1Type)
				{
				case QuestTypes.releaseFilmBudget:
					ReleaseFilmBudgetStart(m.quest1ID, quest);
					break;
				case QuestTypes.releaseFilmGenres:
					ReleaseFilmGenresStart(m.quest1ID, quest);
					break;
				case QuestTypes.shootFilmScenes:
					ShootFilmScenesStart(m.quest1ID, quest);
					break;
				case QuestTypes.vfxFilmGenres:
					VfxFilmGenresStart(m.quest1ID, quest);
					break;
				case QuestTypes.writeScriptGenres:
					WriteScriptGenresStart(m.quest1ID, quest);
					break;
					
				}
				quests.Add(quest);
				
				if(m.quest2ID != "")
				{
					quest = new QuestStatus();
					switch(m.quest2Type)
					{
					case QuestTypes.releaseFilmBudget:
						ReleaseFilmBudgetStart(m.quest2ID, quest);
						break;
					case QuestTypes.releaseFilmGenres:
						ReleaseFilmGenresStart(m.quest2ID, quest);
						break;
					case QuestTypes.shootFilmScenes:
						ShootFilmScenesStart(m.quest2ID, quest);
						break;
					case QuestTypes.vfxFilmGenres:
						VfxFilmGenresStart(m.quest2ID, quest);
						break;
					case QuestTypes.writeScriptGenres:
						WriteScriptGenresStart(m.quest2ID, quest);
						break;
					}
					quests.Add(quest);
				}
				
				if(m.quest3ID != "")
				{
					quest = new QuestStatus();	
					switch(m.quest3Type)
					{
					case QuestTypes.releaseFilmBudget:
						ReleaseFilmBudgetStart(m.quest3ID, quest);
						break;
					case QuestTypes.releaseFilmGenres:
						ReleaseFilmGenresStart(m.quest3ID, quest);
						break;
					case QuestTypes.shootFilmScenes:
						ShootFilmScenesStart(m.quest3ID, quest);
						break;
					case QuestTypes.vfxFilmGenres:
						VfxFilmGenresStart(m.quest3ID, quest);
						break;
					case QuestTypes.writeScriptGenres:
						WriteScriptGenresStart(m.quest3ID, quest);
						break;
					}
					quests.Add(quest);
				}
				SetProfit(id);
			}
		}
	}
	
	void GetQuestText()
	{
		for(int i = 0; i < quests.Count; i++)
		{
			switch(quests[i].type)
			{
			case QuestTypes.releaseFilmBudget:
				quests[i].text = "Release a movie with " + releaseFilmBudget.budgetTaken + " budget taken";
				break;
			case QuestTypes.releaseFilmGenres:
				quests[i].text ="Release " + releaseFilmGenres.releasedFilmsCount + " " + releaseFilmGenres.genres.ToString() + " " + releaseFilmGenres.setting + " movies." 
								+ " (" + quests[i].counter + "/" + releaseFilmGenres.releasedFilmsCount + ")";
				break;
			case QuestTypes.shootFilmScenes:
				quests[i].text ="Shoot " + shootFilmScenes.filmToShootCount + " " + shootFilmScenes.genres + " " + shootFilmScenes.setting + " movies."
								+ " (" + quests[i].counter + "/" + shootFilmScenes.filmToShootCount + ")";
				break;
			case QuestTypes.vfxFilmGenres:
				quests[i].text ="Use postproduction on " + vfxFilmGenres.moviesToVfxCount + " " + vfxFilmGenres.genres + " " + vfxFilmGenres.setting + " movies." 
								+ " (" + quests[i].counter + "/" + vfxFilmGenres.moviesToVfxCount + ")";
				break;
			case QuestTypes.writeScriptGenres:
				quests[i].text ="Write " + writeScriptGenres.scriptsCount + " " + writeScriptGenres.genres.ToString() + 
								" scripts with " + (writeScriptGenres.storyLvl / 20) + " star rating." + " (" + quests[i].counter + 
								"/" + writeScriptGenres.scriptsCount + ")";
				break;
			}
		}
	}
	
	void SetProfit(int id)
	{
		foreach(ProfitQuests p in QuestsTables.profitTable)
		{
			if(p.id == id.ToString())
			{
				profit.coins = p.coins;
				profit.expToStudio = p.expToStudio;
				profit.premium = p.premium;
			}
		}
	}
	
	bool Complete()
	{
		foreach(QuestStatus q in quests)
		{
			if(!q.isComplete)
				return false;
		}
		return true;
	}
	
	void CheckStatus()
	{
		foreach(QuestStatus q in quests)
		{
			if(!q.isComplete)
				return;
		}
		for(int i = 0; i < quests.Count; i++)
		{
			switch(quests[i].type)
			{
			case QuestTypes.releaseFilmBudget:
				Messenger<FilmItem>.RemoveListener("releaseFilm", ReleaseFilmBudgetFinish);
				break;
			case QuestTypes.releaseFilmGenres:
				Messenger<FilmItem>.RemoveListener("releaseFilm", ReleaseFilmGenresFinish);
				break;
			case QuestTypes.shootFilmGenres:
				Messenger<FilmItem>.RemoveListener("shootFilm", ReleaseFilmGenresFinish);
				break;
			case QuestTypes.shootFilmScenes:
				break;
			case QuestTypes.vfxFilmGenres:
				Messenger<FilmItem>.RemoveListener("vfxFilm", ReleaseFilmGenresFinish);
				break;
			case QuestTypes.writeScriptGenres:
				Messenger<Scenario>.RemoveListener("writeScript", WriteScriptGenresFinish);
				break;
			}
		}
		activeIcon.enabled = false;
	    finishedIcon.enabled = true;
	}
	
	public void DestroyThisQuest()
	{
		GlobalVars.soundController.QuestOpenClose();
		GlobalVars.questMainController.activeQuests.Remove(mainQuest);
		if(mainQuest.nextQuestID != "")
		{
			GlobalVars.questMainController.InstantiateQuestWithId(int.Parse(mainQuest.nextQuestID));
		}
		Destroy (this.gameObject);
	}
	
	#region ReleaseFilmGenres
	void ReleaseFilmGenresStart(string id, QuestStatus quest)
	{
		quest.type = QuestTypes.releaseFilmGenres;
		foreach(TypesQuests t in QuestsTables.typesTable)
		{
			if(t.id == id)
			{
				releaseFilmGenres.id = quests.Count - 1;
				releaseFilmGenres.genres = (FilmGenres)Enum.Parse(typeof(FilmGenres), t.param1);
				releaseFilmGenres.setting = (Setting)Enum.Parse(typeof(Setting), t.param2);
				releaseFilmGenres.releasedFilmsCount = int.Parse(t.param3);
				Messenger<FilmItem>.AddListener("releaseFilm", ReleaseFilmGenresFinish);
			}
		}
	}
	
	void ReleaseFilmGenresFinish(FilmItem film)
	{
		for(int i = 0; i < film.genres.Count; i++)
		{
			if(film.genres[i] == releaseFilmGenres.genres)
			{
				break;
			}
			else if(i == film.genres.Count - 1)
			{
				return;
			}
		}
		for(int i = 0; i < film.settings.Count; i++)
		{
			if(film.settings[i] == releaseFilmGenres.setting)
			{
				break;
			}
			else if(i == film.genres.Count - 1)
			{
				return;
			}
		}
		foreach(QuestStatus q in quests)
		{
			if(q.type == QuestTypes.releaseFilmGenres)
			{
				q.counter++;
				if(q.counter >= releaseFilmGenres.releasedFilmsCount)
				{
					q.isComplete = true;
					CheckStatus();
				}
			}
		}
	}	
	#endregion
	
	#region ReleaseFilmBudget
	void ReleaseFilmBudgetStart(string id, QuestStatus quest)
	{
		quest.type = QuestTypes.releaseFilmBudget;
		foreach(TypesQuests t in QuestsTables.typesTable)
		{
			if(t.id == id)
			{
				releaseFilmBudget.id = quests.Count - 1;
				releaseFilmBudget.budgetTaken = int.Parse(t.param1);
				Messenger<FilmItem>.AddListener("releaseFilm", ReleaseFilmBudgetFinish);
			}
		}
	}
	
	void ReleaseFilmBudgetFinish(FilmItem film)
	{
		if(releaseFilmBudget.budgetTaken > film.budgetTaken)
		{
			return;
		}
		foreach(QuestStatus q in quests)
		{
			if(q.type == QuestTypes.releaseFilmBudget)
			{
				q.isComplete = true;
				CheckStatus();
			}
		}
	}	
	#endregion
	
	#region ShootFilmScenes
	void ShootFilmScenesStart(string id, QuestStatus quest)
	{
		quest.type = QuestTypes.shootFilmScenes;
		foreach(TypesQuests t in QuestsTables.typesTable)
		{
			if(t.id == id)
			{
				shootFilmScenes.id = quests.Count - 1;
				shootFilmScenes.genres = (FilmGenres)Enum.Parse(typeof(FilmGenres), t.param1);
				shootFilmScenes.setting = (Setting)Enum.Parse(typeof(Setting), t.param2);
				shootFilmScenes.filmToShootCount = int.Parse(t.param3);
				Messenger<FilmItem>.AddListener("shootFilm", ShootFilmScenesFinish);
			}
		}
	}
	
	void ShootFilmScenesFinish(FilmItem film)
	{
		for(int i = 0; i < film.genres.Count; i++)
		{
			if(film.genres[i] == shootFilmScenes.genres)
			{
				break;
			}
			else if(i == film.genres.Count - 1)
			{
				return;
			}
		}
		for(int i = 0; i < film.settings.Count; i++)
		{
			if(film.settings[i] == shootFilmScenes.setting)
			{
				break;
			}
			else if(i == film.genres.Count - 1)
			{
				return;
			}
		}
		foreach(QuestStatus q in quests)
		{
			if(q.type == QuestTypes.shootFilmScenes)
			{
				q.counter++;
				if(q.counter >= shootFilmScenes.filmToShootCount)
				{
					q.isComplete = true;
					CheckStatus();
				}
			}
		}
	}	
	#endregion
	
	#region VfxFilmGenres
	void VfxFilmGenresStart(string id, QuestStatus quest)
	{
		quest.type = QuestTypes.vfxFilmGenres;
		foreach(TypesQuests t in QuestsTables.typesTable)
		{
			if(t.id == id)
			{
				vfxFilmGenres.id = quests.Count - 1;
				vfxFilmGenres.genres = (FilmGenres)Enum.Parse(typeof(FilmGenres), t.param1);
				vfxFilmGenres.setting = (Setting)Enum.Parse(typeof(Setting), t.param2);
				vfxFilmGenres.moviesToVfxCount = int.Parse(t.param3);
				Messenger<FilmItem>.AddListener("vfxFilm", VfxFilmGenresFinish);
			}
		}
	}
	
	void VfxFilmGenresFinish(FilmItem film)
	{
		for(int i = 0; i < film.genres.Count; i++)
		{
			if(film.genres[i] == vfxFilmGenres.genres)
			{
				break;
			}
			else if(i == film.genres.Count - 1)
			{
				return;
			}
		}
		for(int i = 0; i < film.settings.Count; i++)
		{
			if(film.settings[i] == vfxFilmGenres.setting)
			{
				break;
			}
			else if(i == film.genres.Count - 1)
			{
				return;
			}
		}
		foreach(QuestStatus q in quests)
		{
			if(q.type == QuestTypes.vfxFilmGenres)
			{
				q.counter++;
				if(q.counter >= vfxFilmGenres.moviesToVfxCount)
				{
					q.isComplete = true;
					CheckStatus();
				}
			}
		}
	}	
	#endregion
	
	#region WriteScriptGenres
	void WriteScriptGenresStart(string id, QuestStatus quest)
	{
		quest.type = QuestTypes.writeScriptGenres;
		foreach(TypesQuests t in QuestsTables.typesTable)
		{
			if(t.id == id)
			{
				writeScriptGenres.id = quests.Count - 1;
				writeScriptGenres.genres = (FilmGenres)Enum.Parse(typeof(FilmGenres), t.param1);
				writeScriptGenres.storyLvl = int.Parse(t.param2);
				writeScriptGenres.scriptsCount = int.Parse(t.param3);
				Messenger<Scenario>.AddListener("writeScript", WriteScriptGenresFinish);
			}
		}
	}
	
	void WriteScriptGenresFinish(Scenario scenario)
	{
		for(int i = 0; i < scenario.genres.Count; i++)
		{
			if(scenario.genres[i] == writeScriptGenres.genres)
			{
				break;
			}
			else if(i == scenario.genres.Count - 1)
			{
				return;
			}
		}
		foreach(QuestStatus q in quests)
		{
			if(q.type == QuestTypes.writeScriptGenres)
			{
				q.counter++;
				if(q.counter >= writeScriptGenres.scriptsCount)
				{
					q.isComplete = true;
					CheckStatus();
				}
			}
		}
	}	
	#endregion
}
