using UnityEngine;
using System.Collections;

//опрделеяем, изменилась ли строка, если да - то после каждого 3-го символа вставляем пробел, отсчет с конца.

public class AddSpacesToString : MonoBehaviour 
{
	public tk2dTextMesh mesh;
	public string text = "";
	
	void Start()
	{
		mesh = GetComponent<tk2dTextMesh>();
	}
	
	void FixedUpdate()
	{
		if(mesh.text != text && mesh != null)
		{
			mesh.text = Utils.ToNumberWithSpaces(mesh.text);
			text = mesh.text;
			mesh.Commit();
		}
	}
}
