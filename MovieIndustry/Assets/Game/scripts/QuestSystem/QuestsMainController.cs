using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//на запуске заполняет все квестовые таблицы, также инициализирует либо квест-тикеты (есди у квеста нет требований), 
//либо объекты-пустышки (если у квеста есть требование для начала), на которых висит квест
//со слушателем нужд для инстанса квест-тикета

public class QuestsMainController : MonoBehaviour 
{
	public MainQuest[] allQuests;
	public TypesQuests[] allTypes;
	public List<MainQuest> activeQuests;
	public GameObject questButtonPrefab;
	public GameObject waitForStartQuestPrefab;
	public Transform lettersParent;
	public Transform waitForStartQuestParent;
	
	public TextAsset mainTableAsset;
	public TextAsset questsTableAsset;
	public TextAsset profitsTableAsset;
	public TextAsset needsTableAsset;
	
	void Awake()
	{
		FillMainTable();
		FillQuestsNeedsTable();
		FillQuestsTypesTable();
		FillProfitsTable();
		foreach(MainQuest m in QuestsTables.mainTable)
		{
			if(m.needsID == "" && m.prevQuestID == "")
			{
				InstantiateQuestWithId(m.id);
				activeQuests.Add(m);
			}
			else if(m.needsID != "")
			{
				InstantiateNeedsWithId(m.id);
			}
		}
		
		#region Quest Types listeners
		Messenger<Scenario>.AddListener("writeScript", Foo);
		Messenger<FilmItem>.AddListener("vfxFilm", Foo);
		Messenger<FilmItem>.AddListener("shootFilm", Foo);
		Messenger<FilmItem>.AddListener("releaseFilm", Foo);
		Messenger<FilmItem>.AddListener("releaseFilm", Foo);
		#endregion
		
		#region Needs Types listeners
		Messenger<Construct>.AddListener("buildUpgradeBuilding", Foo);
		#endregion
	}
	
	void Foo(FilmItem f)
	{
		
	}
	
	void Foo(Scenario s)
	{
		
	}
	
	void Foo(Construct c)
	{
		
	}
	
	void InstantiateNeedsWithId(int id)
	{
		GameObject go = Instantiate(waitForStartQuestPrefab) as GameObject;
		go.transform.parent = waitForStartQuestParent;
		go.transform.localPosition = Vector3.zero;
		go.GetComponent<WaitForStartQuest>().Activate(id);
	}
	
	public void InstantiateQuestWithId(int id)
	{
		GameObject go = Instantiate(questButtonPrefab) as GameObject;
		go.GetComponent<QuestButtonController>().ActivateQuest(id);
		go.transform.parent = lettersParent;
		go.transform.localPosition = new Vector3(0, (activeQuests.Count) * 90 - 270, 0);
		go.transform.localScale = Vector3.one;
	}
	
	void FillMainTable()
	{
		string[] lines = mainTableAsset.text.Split("\n" [0]);
		QuestsTables.mainTable = new MainQuest[lines.Length - 1];
		allQuests = new MainQuest[lines.Length];
		for(int i = 1; i < lines.Length; i++)
		{
			string[] columns = lines[i].Split(";" [0]);
			MainQuest m = new MainQuest();
			if(columns[0] != "")
			{	
				m.id = int.Parse(columns[0]);
				m.header = columns[1];
				m.quest1ID = columns[3];
				m.quest1Type = (QuestTypes)Enum.Parse(typeof(QuestTypes), columns[4]);
				m.quest2ID = columns[5];
				if(m.quest2ID != "")
				{
					m.quest2Type = (QuestTypes)Enum.Parse(typeof(QuestTypes), columns[6]);
				}
				m.quest3ID = columns[7];
				if(m.quest3ID != "")
				{
					m.quest3Type = (QuestTypes)Enum.Parse(typeof(QuestTypes), columns[8]);
				}
				m.profitID = int.Parse (columns[9]);
				m.nextQuestID = columns[11];
				m.needsID = columns[12];
				if(m.needsID != "")
				{
					m.needsType = (NeedsTypes)Enum.Parse(typeof(NeedsTypes), columns[13]);
				}
				m.prevQuestID = columns[10];
			}
			QuestsTables.mainTable[i - 1] = m;
		}
	}
	
	void FillQuestsTypesTable()
	{
		string[] lines = questsTableAsset.text.Split("\n" [0]);
		QuestsTables.typesTable = new TypesQuests[lines.Length - 4];
		for(int i = 4; i < lines.Length; i++)
		{
			string[] columns = lines[i].Split (";" [0]);
			TypesQuests t = new TypesQuests();
			int k = 0;
			if(int.TryParse(columns[0], out k))
			{			
				t.id = columns[0];
				t.param1 = columns[1];
				t.param2 = columns[2];
				t.param3 = columns[3];
			}
			QuestsTables.typesTable[i - 4] = t;
		}
	}
	
	void FillProfitsTable()
	{
		string[] lines = profitsTableAsset.text.Split("\n" [0]);
		QuestsTables.profitTable = new ProfitQuests[lines.Length - 1];
		for(int i = 1; i < lines.Length; i++)
		{
			ProfitQuests p = new ProfitQuests();
			string[] columns = lines[i].Split (";" [0]);
			if(columns[0] != "")
			{
				p.id = columns[0];
				int.TryParse(columns[1], out p.coins);
				int.TryParse(columns[2], out p.premium);
				int.TryParse(columns[3], out p.expToStudio);
			}
			QuestsTables.profitTable[i - 1] = p;
		}
	}
	
	void FillQuestsNeedsTable()
	{
		string[] lines = needsTableAsset.text.Split("\n" [0]);
		QuestsTables.needsTypesTable = new NeedsTypeQuests[lines.Length - 4];
		for(int i = 4; i < lines.Length; i++)
		{
			string[] columns = lines[i].Split (";" [0]);
			NeedsTypeQuests t = new NeedsTypeQuests();
			int k = 0;
			if(int.TryParse(columns[0], out k))
			{	
				t.id = columns[0];
				t.param1 = columns[1];
				t.param2 = columns[2];
				t.param3 = columns[3];
				t.param4 = columns[4];
			}
			QuestsTables.needsTypesTable[i - 4] = t;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		allQuests = QuestsTables.mainTable;
		allTypes = QuestsTables.typesTable;
	}
}
