using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
 
//подгрузка текстовых переменных из файла с разделителями
//можно выбрать язык, поиск происходит по адресу ячейки таблицы
public class LoadFromCSV : MonoBehaviour 
{
	const int russian = 4;
	const int english = 3;
	const int male = 1;
	const int female = 2;
	
	public TextAsset csvFile; 
	public TextAsset namesFile;
	public TextAsset staffItems;
	public TextAsset achievmentsFile;
	public TextAsset achievmentsFileLoc;
	public TextAsset buildingParams;
	public TextAsset formulas;
	public TextAsset marketingParams;
	public TextAsset stats;
	
	public TextAsset mi_ui_lines;
	
	public ImportedString[] strings;
	public ImportedString[] formulasVariables;
	
	public void Awake()
	{
		Descriptions(english);
		FillLastNames(namesFile.text);
		FillNames(namesFile.text, male);
		FillNames(namesFile.text, female);
		
		//print (Textes.lastNames[4]);
		//print(Textes.namesFemale[10]);
		
		FillStaffItemsParams(english);
		FillBuildingsBuildParams();
		ImportUILines(english);
		Utils.FillTimeParameters();
		FillFormulasVariables();
		ImportMarketingParams();
		FillStats();
	}
	
	void Start()
	{
		FillAchievmentsParams(english);
	}
	
	void FillStats()
	{
		string[] lines = stats.text.Split("\n" [0]);
		for(int i = 2; i < 5; i++)
		{
			string[] columns = lines[i].Split(";" [0]);
			TownParams t = new TownParams();
			t.revenuePerMin = int.Parse(columns[1]);
			t.time = int.Parse(columns[2]);
			GlobalVars.townParams[i - 2] = t;
		}
		
		for(int i = 8; i < 11; i++)
		{
			string[] columns = lines[i].Split(";" [0]);
			Items t = new Items();
			t.price = int.Parse(columns[1]);
			t.time = int.Parse(columns[2]);
			t.val = int.Parse(columns[3]);
			GlobalVars.merchItems[i - 8] = t;
		}
		
		for(int i = 13; i < 18; i++)
		{
			string[] columns = lines[i].Split(";" [0]);
			GlobalVars.scenesTimers[i - 13] = int.Parse(columns[0]);
		}
	}
	
	void FillFormulasVariables()
	{
		string[] lines = formulas.text.Split("\n" [0]);
		Textes.formulasVariables = new ImportedString[lines.Length];
		for(int i = 1; i < lines.Length - 1; i++)
		{
			string[] columns = lines[i].Split(";" [0]);
			ImportedString imported = new ImportedString();
			if(columns[1] != "")
			{
				imported.caption = columns[3];
				imported.text = columns[4];	
			}
			formulasVariables = Textes.formulasVariables;
			Textes.formulasVariables[i - 1] = imported;
		}
	}
	
	void ImportUILines(int lang)
	{
		string[] lines = mi_ui_lines.text.Split("\n" [0]);
		Textes.mi_ui_lines = new ImportedString[lines.Length];
		for(int i = 4; i < lines.Length - 1; i++)
		{
			string[] columns = lines[i].Split(";" [0]);
			ImportedString imported = new ImportedString();
			if(!string.IsNullOrEmpty(columns[1]))
			{
				
				imported.caption = columns[1];
				imported.text = columns[lang];
				
			}
			Textes.mi_ui_lines[i - 4] = imported;
		}
		
		strings = Textes.mi_ui_lines;
	}
	
	//найти нужную фразу/слово по её положению в массиве таблицы
	public string SelectWord(string csvText, int line, int column)//, out string word)
	{
		string[] lines = csvText.Split("\n" [0]);
		string[] columns = lines[line].Split(";" [0]);
		return columns[column];
		
	}
	
	void ImportMarketingParams()
	{
		string[] lines = marketingParams.text.Split("\n" [0]);
		for(int i = 1; i < 6; i++)
		{
			string[] columns = lines[i].Split(";" [0]);
			FirstTypeMarketing m = new FirstTypeMarketing();
			float f = 0;
			float.TryParse(columns[5], out f);
			m.price = f;
			float.TryParse(columns[4], out f);
			m.bonus = f;
			m.failChance = int.Parse(columns[6]);
			m.zeroChance = int.Parse(columns[7]);
			m.bonusChance = int.Parse(columns[8]);
			GlobalVars.firstTypeMarketing[i - 1] = m;
		}
		for(int i = 7; i < 12; i++)
		{
			string[] columns = lines[i].Split(";" [0]);
			SecondTypeMarketing m = new SecondTypeMarketing();
			m.price = int.Parse(columns[5]);
			m.failChance = int.Parse(columns[6]);
			m.oneTownChance = int.Parse(columns[7]);
			m.twoTownsChance = int.Parse(columns[8]);
			m.threeTownsChance = int.Parse(columns[9]);
			GlobalVars.secondTypeMarketing[i - 7] = m;
		}
	}
	
	//заполнение всех описаний (будет увилечиваться в размерах по мере добавления переменных)
	void Descriptions(int language)
	{
		Textes.w_dir_assign = SelectWord(csvFile.text, 1, language);
		Textes.w_script_assign = SelectWord(csvFile.text, 2, language);
	}
	
	//заполнение имен женских/мужчких, на вход идет пол.
	void FillNames(string text, int gender)
	{
		string[] lines = text.Split("\n" [0]);
		for(int i = 1; i < lines.Length - 1; i++)
		{
			string[] columns = lines[i].Split(";" [0]);
			if(gender == male)
			{	
				Textes.namesMale.Add(columns[gender]);
			}
			if(gender == female)
			{
				Textes.namesFemale.Add(columns[gender]);
			}
		}
	}
	
	//заполнение массива фамилий персонажей
	void FillLastNames(string text)
	{
		string[] lines = text.Split("\n" [0]);
		for(int i = 1; i < lines.Length - 1; i++)
		{
			string[] columns = lines[i].Split(";" [0]);
			Textes.lastNames.Add(columns[0]);
		}
	}
	
	//заполнение массива параметров предметов инвентаря
	void FillStaffItemsParams(int language)
	{
		string[] lines = staffItems.text.Split("\n" [0]);
		for(int i = 1; i < lines.Length - 1; i++)
		{
			string[] columns = lines[i].Split(";" [0]);
			BonusParams itemParams = new BonusParams();
			itemParams.type = (BonusTypes)i -1;
			itemParams.itemName = columns[language];
			itemParams.ExpGainBonus = int.Parse(columns[4]);
			itemParams.MoneyGainBonus = int.Parse(columns[5]);
			itemParams.genresBonuses[0] = int.Parse(columns[6]);
			itemParams.genresBonuses[1] = int.Parse(columns[7]);
			itemParams.priceMoney = int.Parse(columns[9]);
			itemParams.priceStars = int.Parse(columns[10]);
			itemParams.useTime = float.Parse(columns[11]);
			string[] characterTypes = columns[8].Split("," [0]);
			foreach(string s in characterTypes)
			{
				itemParams.characters.Add((CharacterType)int.Parse(s));
			}
			GlobalVars.staffInventoryParams.Add(itemParams);
		}
	}
	
	//заполнение массива достижений для студии
	void FillAchievmentsParams(int language)
	{
		string[] lines = achievmentsFile.text.Split("\n" [0]);
		string[] locLines = achievmentsFileLoc.text.Split("\n" [0]);
		int counter = 0;
		for(int i = 1; i < lines.Length - 1; i++)
		{
			AchievmentsTypes ach = new AchievmentsTypes();
			string[] columns = lines[i].Split(";" [0]);
			if(columns[0] != "")
			{	
				ach.name = columns[0];
				for(int k = 0; k < 3; k++)
				{
					AchievmentsProfit profit = new AchievmentsProfit();
					columns = lines[i + k].Split(";" [0]);
					string[] columnsLoc = locLines[i + k].Split(";" [0]);
					profit.description = columnsLoc[language];
					profit.expToStudio = int.Parse(columns[3]);
					profit.stars = int.Parse(columns[2]);
					ach.achievmentsProfit.Add(profit);
				}
				counter++;
				GlobalVars.achievments.achievments.Add(ach);
			}
		} 
	}
	
	void FillBuildingsBuildParams()
	{
		string[] lines = buildingParams.text.Split("\n" [0]);
		for(int i = 1; i < lines.Length; i++)
		{
			BuildParams par = new BuildParams();
			string[] columns = lines[i].Split(";" [0]);
			if(columns[0] != "")
			{
				print ("ASDS");
				foreach(BuildingType type in GlobalVars.buildingTypes)
				{
					if(type.ToString() == columns[0])
					{
						par.type = type;
						ShopItemParams b = new ShopItemParams();
						b.price = int.Parse(columns[9]);
						b.time = int.Parse(columns[8]);
						b.workersCount = int.Parse(columns[7]);
						par.build = b;
						for(int k = 1; k < 6; k++)
						{
							ShopItemParams upgrade = new ShopItemParams();
							columns = lines[i + k].Split(";" [0]);
							if(columns[2] != "")
							{
								upgrade.price = int.Parse (columns[4]);
								upgrade.time = int.Parse (columns[3]);
								upgrade.workersCount = int.Parse(columns[2]);
								par.upgrade.Add(upgrade);
							}
						}
						GlobalVars.buildParams.Add(par);
					}
				}
			}
		}
	}
}
