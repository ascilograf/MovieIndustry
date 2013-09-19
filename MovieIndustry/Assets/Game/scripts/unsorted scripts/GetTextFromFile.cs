using UnityEngine;
using System.Collections;

//подгрузка текста для текстмеша из локкита
//поиск по названию переменной в списке импортированных текстов, при совпадении - грабит текст
//для нескольких значений заполняется массив значений по массиву имен переменных

public class GetTextFromFile : MonoBehaviour 
{
	public string caption = "";
	
	public string addedAfterText;
	public string addedBeforeText;
	public int maxLength;
	
	public string[] manyCaptions;
	
	public string[] values = new string[0];
	tk2dTextMesh mesh = null;
	
	void Start()
	{
		mesh = GetComponent<tk2dTextMesh>();
		foreach(ImportedString s in Textes.mi_ui_lines)
		{
			if(s.caption == caption)
			{
				if(maxLength > 0)
				{
					mesh.text = Utils.FormatStringToText(s.text, maxLength);
				}
				else
				{
					mesh.text = Utils.FormatString(s.text);
				}
				mesh.text = addedBeforeText + mesh.text;
				mesh.text += addedAfterText;
				mesh.maxChars = s.text.Length;				//если будет виснуть - убрать это и выставить размеры на самих мешах
				return;
			}
		}
		
		if(manyCaptions.Length > 0)
		{
			values = new string[manyCaptions.Length];
			for(int i = 0; i < manyCaptions.Length; i++)
			{
				foreach(ImportedString str in Textes.mi_ui_lines)
				{
					if(str.caption.Equals(manyCaptions[i]))
					{
						values[i] = str.text;
						break;
					}
				}
			}
		}
	}
	
	//вызов определенного текста из массива, есть возможность дописать текст (например дописать 0/3, или что-то в этом духе)
	public void SetTextWithIndex(int index, string addedText = " ")
	{
		if(manyCaptions.Length > 0)
		{
			if(values.Length == 0)
			{
				values = new string[manyCaptions.Length];
				for(int i = 0; i < manyCaptions.Length; i++)
				{
					for(int j = 0; j < Textes.mi_ui_lines.Length - 1;j++)
					{
						if(Textes.mi_ui_lines[j] != null)
						{
							if(Textes.mi_ui_lines[j].caption.Equals(manyCaptions[i]))
							{
								values[i] = Textes.mi_ui_lines[j].text;
								break;
							}
						}
					}
				}
			}
			mesh = GetComponent<tk2dTextMesh>();
			if(addedText != " ")
			{
				mesh.maxChars = values[index].Length + addedText.Length;	
			}
			else
			{
				mesh.maxChars = values[index].Length;
			}
			
			if(maxLength > 0)
			{
				mesh.text = Utils.FormatStringToText(values[index] + addedText, maxLength);
			}
			else
			{
				mesh.text = Utils.FormatString(values[index]) + addedText;
			}	
		}
		
	}
}
