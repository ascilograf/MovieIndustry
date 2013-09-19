using UnityEngine;
using System.Collections;

public class Starfield : MonoBehaviour 
{
	private Vector2 lastScreenSize = new Vector2();
	public Camera starFieldCamera;
	public Material starField;
	
	public float backgroundDistance = 10000;
	public float smallStarsDistance = 5000;
	public float mediumStarsDistance = 2500;
	public float bigStarsDistance = 1000;
	
	void Update()
	{
		if(Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
		{
			UpdateSize();
		}
	}
	
	private void UpdateSize()
	{
		/*lastScreenSize.x = Screen.width;
		lastScreenSize.y = Screen.height;
		
		float maxSize = lastScreenSize.y > lastScreenSize.x ? lastScreenSize.y : lastScreenSize.x;
		maxSize /= 10;
		transform.localScale = new Vector3(maxSize * 6, 1, maxSize);*/
	}
	
	void LateUpdate()
	{
		Vector3 camPos = starFieldCamera.transform.position;
		starField.SetTextureOffset("_MainTex", new Vector2(camPos.x / backgroundDistance, 0));//camPos.y / smallStarsDistance));
		
		//starField.SetTextureOffset("_SmallStars", new Vector2(camPos.x / smallStarsDistance, camPos.y / smallStarsDistance));
		//starField.SetTextureOffset("_MediumStars", new Vector2(camPos.x / mediumStarsDistance, camPos.y / mediumStarsDistance));
		//starField.SetTextureOffset("_BigStars", new Vector2(camPos.x / bigStarsDistance, camPos.y / bigStarsDistance));
	}
}
