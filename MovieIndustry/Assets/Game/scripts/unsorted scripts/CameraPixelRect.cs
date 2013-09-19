using UnityEngine;
using System.Collections;

//регулировка окна свайп-камеры
public class CameraPixelRect : MonoBehaviour 
{
	//боксы/окна ограничений камер по разным аспектам
	public BoundingBox aspect4_3;			
	public BoundingBox aspect16_9;
	public BoundingBox aspect16_10;
	public BoundingBox aspect5_3;
	public BoundingBox aspect3_2;
	
	public BoundingBox box;
	
	public bool editable = false;
	public bool isVertical;	//вертикальный ли свайп
	public bool doNotChangeSize = false;
	BoundingBox bb = new BoundingBox();		//временный бокс
	
	
	
	void Start()
	{
		if(!editable)
		{
			float h = Screen.height;
			float w = Screen.width;
			
			//определение аспекта
			float aspect = 0;
			aspect = w/h;
			
			//нахождение нужного бокса под полученный аспект
			if(aspect <= 1.35f)
			{
				bb = aspect4_3;
				Place (1024, 768);
			}
			else if(aspect <= 1.55f)
			{
				bb = aspect3_2;
				Place (960, 640);
			}
			else if(aspect <= 1.63f)
			{
				bb = aspect16_10;
				Place(1280, 800);
			}
			else if(aspect <= 1.7f)
			{
				bb = aspect5_3;
				Place (1280, 768);
			}	
			else if(aspect >= 1.77f)
			{
				bb = aspect16_9;
				Place (1280, 720);
			}	
		}
	}
		
	//установка новых параметров окна рендеринга камеры
	void Place(float width,	float height)
	{
		float w = Screen.width;
		float h = Screen.height;
		
		w = w / width;
		h = h / height;
		
		//определение размера камеры
		if(!doNotChangeSize)
		{
			if(isVertical)
			{
				camera.orthographicSize = Screen.height / 4;
			}
			else
			{
				camera.orthographicSize = Screen.height / 2;
			}
		}
		
		
		//масштабирование окна
		bb.xMin *= w;
		bb.xMax *= w;
		bb.yMax *= h;
		bb.yMin *= h;
		
		//присваивание новых значений окна камере, к которой прикреплен этот компонент
		Rect rect = camera.pixelRect;
		rect.xMax = bb.xMax;
		rect.xMin = bb.xMin;
		rect.yMax = bb.yMax;
		rect.yMin = bb.yMin;
		camera.pixelRect = rect;
	}
	
	void Update()
	{
		if(editable)
		{
			Rect rect = camera.pixelRect;
			rect.xMax = box.xMax;
			rect.xMin = box.xMin;
			rect.yMax = box.yMax;
			rect.yMin = box.yMin;
			camera.pixelRect = rect;
		}
	}
	
	/* это для тестов
	void OnGUI()
	{
		float h = Screen.height;
		float w = Screen.width;
		GUI.TextArea(new Rect(100,100,100, 20), "w = " + w);
		GUI.TextArea(new Rect(100,150,100, 20), "h = " + h);
		GUI.TextArea(new Rect(100,200,100, 20), "a = " + w/h);
		GUI.TextArea(new Rect(100,250,100, 20), "xMax = " + camera.pixelRect.xMax);
		GUI.TextArea(new Rect(100,300,100, 20), "yMax = " + camera.pixelRect.yMax);
		GUI.TextArea(new Rect(100,350,100, 20), "xMin = " + camera.pixelRect.xMin);
		GUI.TextArea(new Rect(100,400,100, 20), "yMin = " + camera.pixelRect.yMin);
	}*/
}
