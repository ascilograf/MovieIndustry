using UnityEngine;
using System.Collections;

//Управление камерой, скрипт выясняет на каком типе устройств запущена игра и в зависимости от устроуства
//определяет управление (палец или курсор)
//если кнопка зажата, то получаем дельту движения курсора(пальца) и двигаем камеру в противоположную сторону
public class CameraMove : MonoBehaviour 
{
	public float cameraSpeed;							//скорость движения камеры
	public float androidCamSpeed;						//скорость двиежния камеры на Тач-устройствах
	public float zoomSpeed;
	public float xMax;									//границы по горзонтали (указывается одно число, отсчет будет идти направо и налево от центра экрана)
	public float yMax;									//граница по вертикали вверх от центра экрана
	public float yMin;									//граница по вертикали вниз от центра экрана
	public BoundingBox cameraFrame;
	public BoundingBox currentCameraFrame;
	
	float speed;										//промежуточная переменная скорости камеры									
	Camera cam;											//экземпляр камеры
	Vector3 pos = Vector3.zero;							//направление в котором движется курсор/палец
	Vector3 tempPos = Vector3.zero;						//позиция для проверки на заход камеры за границы экрана
	float magn;											//"множитель" для плавного замедления камеры при отпускании пальца/курсора
	Vector2 delta;										//дельта движения пальца
	public CameraStates state;
	
	float cameraSize;
	
	float dist, cameraDist;
	Touch myTouch1, myTouch2;
	//присвоение переменным их значений
	void Awake()
	{
		GlobalVars.cameraFrame = cameraFrame;
		cam = Camera.main;	
		cameraSize = Camera.main.orthographicSize;
	}
	
	void FixedUpdate () 
	{
		//если камера находится в обычном состоянии
		if(GlobalVars.cameraStates == CameraStates.normal)
		{
			//если платформа не мобильная, то скорость будет обычная
			if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || 
				Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
			{
				speed = cameraSpeed;
				
				//если нажата кнопка мыши, то получаем вектор движения курсора и выставляем множитель в единицу
				if(Input.GetMouseButton(0))
				{
					pos = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
				}
				//иначе если кнопка не нажата и если множитель больше нуля, то уменьшаем множитель до нуля с каждым кадром
				else
				{
					pos = Vector3.zero;
				}
			}
			//иначе, если плафтформа мобильная - выставляем мобильную скорость для камеры
			else
			{
				speed = androidCamSpeed;
				if(Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Began)
				{
					myTouch1 = Input.GetTouch(0);
		           	myTouch2 = Input.GetTouch(1);
			        cameraDist = Vector2.Distance(myTouch1.position, myTouch2.position);	
				}
				
				if(Input.touchCount > 1 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))
				{
					float y = cameraSize;
					myTouch1 = Input.GetTouch(0);
		           	myTouch2 = Input.GetTouch(1);
			        dist = Vector2.Distance(myTouch1.position, myTouch2.position);
					
					y /= dist/cameraDist;

					if(y > (cameraSize * 1.375f))	
					{
						y = cameraSize * 1.325f;
					}
					else if(y < (cameraSize * 0.625f))
					{
						y = cameraSize / 0.625f;
					}
					Camera.main.orthographicSize = y;
				}
				//если есть прикосновение к экрану и палец движется, то получаем вектор движения курсора и выставляем множитель в единицу
				else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) 
				{	
		            delta = Input.GetTouch(0).deltaPosition;
					pos = new Vector3(delta.x, delta.y, 0);
				}
				else if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
				{
					cameraSize = Camera.main.orthographicSize;
					cameraDist = 0;
					dist = 0;
				}
				//иначе если палец не движется, или вообще нет тача и если множитель больше нуля, то уменьшаем множитель до нуля с каждым кадром
				else
				{
					pos = Vector3.zero;
					
				}
				
			}
			
			//присвоение камере новой позиуии в соотвтствии с условиями выше
			tempPos = cam.transform.position;
			tempPos -= pos * Time.deltaTime * speed;
			
			//проверка на выход за границы экрана, если есть выход за экран - присваивание к крайним значениям по горизонтали или вертикали.
			if(tempPos.x > GlobalVars.cameraFrame.xMax)
			{
				tempPos.x = GlobalVars.cameraFrame.xMax;
			}
			if(tempPos.x < GlobalVars.cameraFrame.xMin)
			{
				tempPos.x = GlobalVars.cameraFrame.xMin;
			}
			if(tempPos.y < GlobalVars.cameraFrame.yMin)
			{
				tempPos.y = GlobalVars.cameraFrame.yMin;
			}
			if(tempPos.y > GlobalVars.cameraFrame.yMax)
			{
				tempPos.y = GlobalVars.cameraFrame.yMax;
			}
			tempPos.z = cam.transform.position.z;
			cam.transform.position = tempPos;
		}
		state = GlobalVars.cameraStates;
		CheckFrame();
	}
	
	void CheckFrame()
	{
		float modifier = 800 - Camera.main.orthographicSize ;
		GlobalVars.cameraFrame.yMin = 480 - modifier;
		GlobalVars.cameraFrame.yMax = 690 + modifier;
		GlobalVars.cameraFrame.xMin = -3100 - modifier * 1.7f;
		GlobalVars.cameraFrame.xMax = 3200 + modifier * 2;
		
		currentCameraFrame = GlobalVars.cameraFrame;
	}
}
