using UnityEngine;
using System.Collections;

//двигаем задний фон по слоям каждый кадр с разной скоростью

public class BackgroundScroller : MonoBehaviour 
{
	public Transform layerSky;
	public Transform layerHills;
	public Transform layerSkyscrappers;
	
	public float speedSky;
	public float speedHills;
	public float speedSkyscrappers;
	
	void Awake()
	{
		gameObject.SetActive(true);
	}
	
	void Update () 
	{
		MoveLayer(layerSky, speedSky);
		MoveLayer(layerHills, speedHills);
		MoveLayer(layerSkyscrappers, speedSkyscrappers);
	}
	
	//сдвиг бэкграунда
	void MoveLayer(Transform layer, float speed)
	{
		Vector3 v3 = Camera.main.transform.position;
		v3.x /= speed;
		v3.z = layer.transform.position.z;
		v3.y = layer.transform.position.y;
		layer.transform.position = v3;
	}
}
