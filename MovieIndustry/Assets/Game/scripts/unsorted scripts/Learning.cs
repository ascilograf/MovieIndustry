using UnityEngine;
using System.Collections;

//контроль туториала, в массиве указывается объект, после вызова метода NextLearningStep()
//все объекты становятся неактивными, кроме этого объекта, после выполняется FocusCircleOnButton() и LocateMatker()
//т.е. происходит фокусировка видимой части на объекте и перемещение маркера.
public class Learning : MonoBehaviour 
{
	public LearningStep[] steps;			//шаги туториала, в них указывается объект, надпись и положение маркера
	
	public GameObject marker;				//маркер
	public tk2dTextMesh textMesh;			//тект маркера
	public int stepIndex;					//индекс шага
	  
	float temp;
	
	void Start () 
	{
		if(Screen.width > 2000)
		{
			transform.localScale = Vector3.one * 2;
		}
		Messenger<GameObject>.AddListener("Tap on target", CheckTapOnGUI);
		FocusCircleOnButton();
	}
	
	//по нажатию на выбранный объект происходит переход на след. шаг
	void CheckTapOnGUI(GameObject go)
	{
		if(stepIndex < steps.Length)
		{
			if(go == steps[stepIndex].button)
			{
				NextLearningStep();
			}
		}
	}
	
	//выделение следующей кнопки в массиве
	void NextLearningStep()
	{
		stepIndex++;
		if(stepIndex >= steps.Length)
		{
			GlobalVars.buttonForLearning = null;
			gameObject.SetActive(false);
			return;
		}
		else
		{
			FocusCircleOnButton();
		}
		
	}
	
	//фокусировка на объекте
	void FocusCircleOnButton()
	{
		GlobalVars.cameraStates = CameraStates.learning;
		
		textMesh.text = steps[stepIndex].caption;
		textMesh.Commit();
		GlobalVars.buttonForLearning = steps[stepIndex].button;
		Vector3 v3 = transform.position;
		v3.x = steps[stepIndex].button.transform.position.x;
		v3.y = steps[stepIndex].button.transform.position.y;
		transform.position = v3;
		
		LocateMarker();
	}
	
	//определение положения маркера
	void LocateMarker()
	{
		switch(steps[stepIndex].position)
		{
		case Position.leftTop:
			marker.transform.localPosition = new Vector3(-100, 100, -1);
			marker.transform.localEulerAngles = new Vector3(0,0, 315);
			textMesh.transform.localPosition = new Vector3(-100, 200, -1);
			break;
		case Position.rightTop:
			marker.transform.localPosition = new Vector3(100, 100, -1);
			marker.transform.localEulerAngles = new Vector3(0,0, 225);
			textMesh.transform.localPosition = new Vector3(-100, 200, -1);
			break;
		case Position.leftBottom:
			marker.transform.localPosition = new Vector3(-100, -100, -1);
			marker.transform.localEulerAngles = new Vector3(0,0, 45);
			textMesh.transform.localPosition = new Vector3(-100, -200, -1);
			break;
		case Position.rightBottom:
			marker.transform.localPosition = new Vector3(100, -100, -1);
			marker.transform.localEulerAngles = new Vector3(0,0, 135);
			textMesh.transform.localPosition = new Vector3(-100, -200, -1);;
			break;
		}
	}
	
	
	void Update () 
	{
		if(temp >= 0.01f)
		{
			FocusCircleOnButton();
		}
		else
		{
			temp += Time.deltaTime;
		}
	}
}
