using UnityEngine;
using System.Collections;

//менюшка квеста, может быть в двух состояниях: инфо квеста и сдача квеста
//в состоянии инфо активна кнопка Do It! Она отправляет на к зданию и вызывает меню, если здание свободно
//в состоянии сдачи активна кнопка сдать квест, также начисляются денюжки и прочее.

public class PopUpQuests : MonoBehaviour 
{
	[System.Serializable]
	public class QuestSection
	{
		public Transform parentObj;
		public GameObject buttonDoIt;
		public tk2dTextMesh buttonText;
		public MeshRenderer checkMark;
		public tk2dTextMesh questTextMesh;
		public QuestTypes questType;
	};
	
	public QuestSection[] questSections;
	//public GameObject[] questSections;
	public GameObject buttonCollect;
	public GameObject buttonDoIt;
	public MeshRenderer completeMark;
	
	//meshes
	public tk2dTextMesh headerMesh;
	public tk2dTextMesh descriptionMesh;
	public tk2dTextMesh questsMesh;
	public tk2dTextMesh coinsRewardMesh;
	public tk2dTextMesh expRewardMesh;
	public tk2dTextMesh premiumRewardMesh;
	
	public QuestTypes type;
	public QuestButtonController questButton;
	
	GameObject stage = null;
	// Use this for initialization
	void Start () 
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject g)
	{
		if(g == gameObject)
		{
			gameObject.SetActive(false);
			GlobalVars.cameraStates = CameraStates.normal;
		}
		else if(g == buttonCollect)
		{
			Utils.ChangeMoney(questButton.profit.coins);
			GlobalVars.stars += questButton.profit.premium;
			Coroutiner.StartCoroutine(Utils.ChangeExpBalance(GlobalVars.exp, GlobalVars.exp + questButton.profit.expToStudio));
			gameObject.SetActive(false);
			GlobalVars.cameraStates = CameraStates.normal;
			questButton.DestroyThisQuest();
		}
		else if(g.name == "buttonDoIt")
		{
			foreach(QuestSection q in questSections)
			{
				if(q.buttonDoIt == g)
				{
					type = q.questType;
					DoIt();
				}
			}
				
		}
	}
	
	public void SetParamsForStart(QuestButtonController button, string header, string description, string quest, string rewardCoins, string rewardExp, string rewardPremium)
	{
		questButton = button;
		gameObject.SetActive(true);
		FillQuestObjectives();
		buttonCollect.SetActive(false);
		GlobalVars.cameraStates = CameraStates.menu;
		Utils.SetText(headerMesh, header);
		Utils.SetText(descriptionMesh, Utils.FormatStringToText(description, 40));
		
		//Utils.SetText(questsMesh, Utils.FormatStringToText(quest, 33));
		SetReward(rewardCoins, rewardPremium, rewardExp);
	}
	
	public void SetParamsForFinish(QuestButtonController button, string header, string description, string quest, string rewardCoins, string rewardExp, string rewardPremium)
	{
		questButton = button;
		gameObject.SetActive(true);
		FillQuestObjectives();
		buttonCollect.SetActive(true);
		GlobalVars.cameraStates = CameraStates.menu;
		Utils.SetText(headerMesh, header);
		Utils.SetText(descriptionMesh, Utils.FormatStringToText(description, 40));
		SetReward(rewardCoins, rewardPremium, rewardExp);
	}
	
	void FillQuestObjectives()
	{
		foreach(QuestSection q in questSections)
		{
			q.parentObj.gameObject.SetActive(false);
		}
		switch(questButton.quests.Count)
		{
		case 1:
			questSections[0].parentObj.gameObject.SetActive(true);
			questSections[0].parentObj.localPosition = new Vector3(0,-4,-5);
			questSections[0].questType = questButton.quests[0].type;
			if(questButton.quests[0].isComplete)
			{
				questSections[0].checkMark.enabled = true;
				questSections[0].buttonDoIt.SetActive(false);
			}
			else
			{
				questSections[0].checkMark.enabled = false;
				questSections[0].buttonDoIt.SetActive(true);
			}
			SetButtonText(questButton.quests[0].type, questSections[0].buttonText);
			Utils.SetText(questSections[0].questTextMesh, Utils.FormatStringToText(questButton.quests[0].text, 40));
			break;
		case 2:
			questSections[0].parentObj.localPosition = new Vector3(0,24,-5);
			questSections[0].parentObj.gameObject.SetActive(true);
			questSections[0].questType = questButton.quests[0].type;
			if(questButton.quests[0].isComplete)
			{
				questSections[0].checkMark.enabled = true;
				questSections[0].buttonDoIt.SetActive(false);
			}
			else
			{
				questSections[0].checkMark.enabled = false;
				questSections[0].buttonDoIt.SetActive(true);
			}
			Utils.SetText(questSections[0].questTextMesh, Utils.FormatStringToText(questButton.quests[0].text, 40));
			SetButtonText(questButton.quests[0].type, questSections[0].buttonText);
			questSections[1].parentObj.localPosition = new Vector3(0,-35,-5);
			questSections[1].parentObj.gameObject.SetActive(true);
			questSections[1].questType = questButton.quests[1].type;
			if(questButton.quests[1].isComplete)
			{
				questSections[1].checkMark.enabled = true;
				questSections[1].buttonDoIt.SetActive(false);
			}
			else
			{
				questSections[1].checkMark.enabled = false;
				questSections[1].buttonDoIt.SetActive(true);
			}
			Utils.SetText(questSections[1].questTextMesh, Utils.FormatStringToText(questButton.quests[1].text, 40));
			SetButtonText(questButton.quests[0].type, questSections[1].buttonText);
			break;
		case 3:
			break;
		}
	}
	
	void SetButtonText(QuestTypes type, tk2dTextMesh mesh)
	{
		switch(type)
		{
		case QuestTypes.releaseFilmBudget:
			Utils.SetText(mesh, "Release!");
			break;
		case QuestTypes.shootFilmGenres:
			Utils.SetText(mesh, "Shoot!");
			break;
		case QuestTypes.shootFilmScenes:
			Utils.SetText(mesh, "Shoot!");
			break;
		case QuestTypes.vfxFilmGenres:
			Utils.SetText(mesh, "Do vfx!");
			break;
		case QuestTypes.writeScriptGenres:
			Utils.SetText(mesh, "Write!");
			break;
		}
	}
	
	void SetReward(string coins, string premium, string exp)
	{
		if(premium == "0" || premium == "")
			premiumRewardMesh.gameObject.SetActive(false);
		else
			Utils.SetText(premiumRewardMesh, premium);
		if(coins == "0" || coins == "")
			coinsRewardMesh.gameObject.SetActive(false);
		else
			Utils.SetText(coinsRewardMesh, coins);
		if(exp == "0" || exp == "")
			expRewardMesh.gameObject.SetActive(false);
		else
			Utils.SetText(expRewardMesh, exp);
	}
	
	
	void DoIt()
	{
		gameObject.SetActive(false);	
		GlobalVars.cameraStates = CameraStates.normal;
		GameObject[] stages;
		Vector3 v3 = Camera.main.transform.position;
		switch(type)
		{
		case QuestTypes.releaseFilmBudget:
			this.stage = GameObject.FindGameObjectWithTag("Offices");
			v3.x = this.stage.transform.position.x;
			break;
		case QuestTypes.releaseFilmGenres:
			this.stage = GameObject.FindGameObjectWithTag("Offices");
			v3.x = this.stage.transform.position.x;
			break;
		case QuestTypes.shootFilmGenres:
			stages = GameObject.FindGameObjectsWithTag("Pavillion");
			foreach(GameObject stage in stages)
			{
				if(!stage.GetComponent<StageWork>().isStageBusy && !stage.GetComponent<FilmMaking>().busy)
				{
					this.stage = stage;
					gameObject.SetActive(true);
					GlobalVars.cameraStates = CameraStates.menu;
					break;
				}
				else if(stage.GetComponent<StageWork>().isStageBusy)
				{
					this.stage = stage;
					gameObject.SetActive(true);
					GlobalVars.cameraStates = CameraStates.menu;
				}
				else if(stage.GetComponent<FilmMaking>().busy)
				{
					this.stage = stage;
					gameObject.SetActive(true);
					GlobalVars.cameraStates = CameraStates.menu;
				}
			}
			v3.x = this.stage.transform.position.x;
			break;
		case QuestTypes.shootFilmScenes:
			stages = GameObject.FindGameObjectsWithTag("Pavillion");
			foreach(GameObject stage in stages)
			{
				if(!stage.GetComponent<StageWork>().isStageBusy && !stage.GetComponent<FilmMaking>().busy)
				{
					this.stage = stage;
					GlobalVars.cameraStates = CameraStates.menu;
					break;
				}
				else if(stage.GetComponent<StageWork>().isStageBusy)
				{
					this.stage = stage;
					GlobalVars.cameraStates = CameraStates.menu;
				}
				else if(stage.GetComponent<FilmMaking>().busy)
				{
					this.stage = stage;
					GlobalVars.cameraStates = CameraStates.menu;
				}
			}
			v3.x = this.stage.transform.position.x;
			break;
		case QuestTypes.vfxFilmGenres:
			stages = GameObject.FindGameObjectsWithTag("postProdOffice");
			foreach(GameObject stage in stages)
			{
				if(!stage.GetComponent<PostProdOfficeStage>().busy && !stage.GetComponent<StageWork>().isStageBusy)
				{
					this.stage = stage;
					GlobalVars.cameraStates = CameraStates.menu;
					break;
				}
				else if(stage.GetComponent<PostProdOfficeStage>().busy || stage.GetComponent<StageWork>().isStageBusy)
				{
					this.stage = stage;
					GlobalVars.cameraStates = CameraStates.menu;
				}	
			}
			v3.x = this.stage.transform.position.x;
			break;
		case QuestTypes.writeScriptGenres:
			stages = GameObject.FindGameObjectsWithTag("ScriptWritters");
			foreach(GameObject stage in stages)
			{
				if(!stage.GetComponent<ScriptersOfficeStage>().isStageBusy && !stage.GetComponent<StageWork>().isStageBusy)
				{
					this.stage = stage;
					GlobalVars.cameraStates = CameraStates.menu;
					break;
				}
				else
				{
					this.stage = stage;
					GlobalVars.cameraStates = CameraStates.menu;
				}
				
			}
			break;
		}
		v3.x = this.stage.transform.position.x;
		Coroutiner.StartCoroutine(Utils.MoveCameraTo(Camera.main.transform.position, v3, this));
	}
	
	public void CallMenu()
	{
		switch(type)
		{
		case QuestTypes.releaseFilmBudget:
			MarketingOfficeStage mStage = this.stage.GetComponent<MarketingOfficeStage>();
			GlobalVars.popUpSelectMarketing.SetParams(mStage);
			break;
		case QuestTypes.shootFilmGenres:
			FilmMaking filmMaking = stage.GetComponent<FilmMaking>();
			if(filmMaking.boostIcon.activeSelf)
			{
				GlobalVars.cameraStates = CameraStates.normal;
			}
			else if(filmMaking.GetComponent<StageWork>().worker != null)
			{
				StageWork s = filmMaking.GetComponent<StageWork>();
				GlobalVars.popUpStageWork.ShowInfo(s.worker, (int)s.time, s);	
			}
			else if(filmMaking.busy && filmMaking.staffCount == filmMaking.staff.Count)
			{
				GlobalVars.popUpSpeedUpAnyJob.SetParamsForFinish(filmMaking);	
			}
			else if(!filmMaking.busy)
			{
				GlobalVars.cameraStates = CameraStates.menu;
				GlobalVars.popUpCastingMenu.SetParams(filmMaking);	
			}
			break;
		case QuestTypes.shootFilmScenes:
			filmMaking = stage.GetComponent<FilmMaking>();
			if(filmMaking.boostIcon.activeSelf)
			{
				GlobalVars.cameraStates = CameraStates.normal;
			}
			else if(filmMaking.GetComponent<StageWork>().worker != null)
			{
				StageWork s = filmMaking.GetComponent<StageWork>();
				GlobalVars.popUpStageWork.ShowInfo(s.worker, (int)s.time, s);	
			}
			else if(filmMaking.busy && filmMaking.staffCount == filmMaking.staff.Count)
			{
				GlobalVars.popUpSpeedUpAnyJob.SetParamsForFinish(filmMaking);	
			}
			else if(!filmMaking.busy)
			{
				GlobalVars.cameraStates = CameraStates.menu;
				GlobalVars.popUpCastingMenu.SetParams(filmMaking);	
			}
			break;
		case QuestTypes.releaseFilmGenres:
			mStage = stage.GetComponent<MarketingOfficeStage>();
			GlobalVars.popUpSelectMarketing.SetParams(mStage);
			break;
		case QuestTypes.vfxFilmGenres:
			PostProdOfficeStage vfxStage = stage.GetComponent<PostProdOfficeStage>();
			if(vfxStage.GetComponent<StageWork>().worker != null)
			{
				StageWork s = vfxStage.GetComponent<StageWork>();
				GlobalVars.popUpStageWork.ShowInfo(s.worker, (int)s.time, s);
			}
			else if(vfxStage.busy)
			{
				GlobalVars.popUpSpeedUpAnyJob.SetParamsForFinish(vfxStage);		
			}
			else if(!vfxStage.busy)
			{
				GlobalVars.cameraStates = CameraStates.menu;
				GlobalVars.popUpHireEffects.GetComponent<AddVisualEffects>().ShowMenu(vfxStage);
			}
			break;
		case QuestTypes.writeScriptGenres:
				ScriptersOfficeStage scriptersStage = stage.GetComponent<ScriptersOfficeStage>();
				if(!scriptersStage.isStageBusy && !scriptersStage.GetComponent<StageWork>().isStageBusy)
				{
					GlobalVars.cameraStates = CameraStates.menu;
					GlobalVars.popUpHireForScript.SetParams(scriptersStage);
					GlobalVars.popUpHireForScript.SwitchState(PopUpNewScriptState.selectGenres);
					return;
				}
				else if(scriptersStage.isStageBusy)
				{
					GlobalVars.popUpSpeedUpAnyJob.SetParamsForFinish(scriptersStage);
				}
				else if(scriptersStage.GetComponent<StageWork>().isStageBusy)
				{
					GlobalVars.popUpStageWork.ShowInfo(scriptersStage.GetComponent<StageWork>().worker, (int)scriptersStage.GetComponent<StageWork>().time, scriptersStage.GetComponent<StageWork>());
				}
			break;
		}
	}
}
