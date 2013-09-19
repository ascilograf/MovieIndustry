using UnityEngine;
using System.Collections;

//Плавная прокрутка оюъекта по движению курсора или пальца
public class SwipeButtons : MonoBehaviour 
{
	public float swipeSpeed;			//скорость прокрутки
	public float xMax;					//макс. Х	
	public float xMin;					//мин. Х
	
	void Start () 
	{
		xMin = transform.localPosition.x;	
	}
	
	
	void Update () 
	{
		float x = 0;
		Vector3 pos = transform.localPosition;
		// Если платформа не мобильная, то используется GetAxis - изменение движения курсора
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || 
				Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			if(Input.GetMouseButton(0))
			{
				x = Input.GetAxis("Mouse X");
				pos = transform.localPosition;	
			}
		}
		//иначе - происходит проверка на фазу тача
		else
		{
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) 
			{	
		        x = Input.GetTouch(0).deltaPosition.x;
			}
		}
		
		
		pos.x += x * swipeSpeed/2 * Time.deltaTime;
		//проверка на заход за границы и присвоение нового значения позиции объекта.
		if(pos.x < xMax)
		{
			pos.x = xMax;
		}
		if(pos.x > xMin)
		{
			pos.x = xMin;
		}
		transform.localPosition = pos;
	}
}
