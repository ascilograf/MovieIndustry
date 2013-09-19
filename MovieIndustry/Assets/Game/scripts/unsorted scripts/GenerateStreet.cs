using UnityEngine;
using System.Collections;

public class GenerateStreet : MonoBehaviour {
	
	//части дороги
	public GameObject[] roadParts;
	public GameObject[] treesParts;
	public GameObject[] hydrantsParts;
	public GameObject[] lampPosts;
	public GameObject[] hatchParts;
	public GameObject[] fences;
	public GameObject checkPoint;
	
	//родительские объекты дорог
	public Transform roadParent;
	public Transform threeParent;
	public Transform hydrantsParent;
	public Transform lampParent;
	public Transform hatchParent;
	public Transform fenceParent;
	
	//генерируем части дороги
	void Start () 
	{
		GameObject go = Instantiate(checkPoint) as GameObject;
		go.transform.parent = hydrantsParent;
		go.transform.localPosition = new Vector3(-4000, -160, 0);
		Generate(roadParts, roadParent, 118, 1);
		Generate(treesParts, threeParent, 116, 4);
		Generate(hydrantsParts, hydrantsParent, 116, 5);
		Generate(lampPosts, lampParent, 800, 1);
		Generate(fences, fenceParent, 576, 1);
		go = Instantiate(checkPoint) as GameObject;
		go.transform.parent = hydrantsParent;
		go.transform.localPosition = new Vector3(4500, -160, 0);
	}
	
	//на вход массив объектов из которых рандомом будут выбираться части, отцовский объект, инкремент по оси Х, интервал
	void Generate(GameObject[] prefab, Transform parent, float increment, int interval)
	{
		float pos = -4500;
		
		for(int i = 0;  pos <= 5000; i++)
		{
			pos += increment;
			if(i % interval == 0)
			{
				GameObject go = Instantiate(prefab[Random.Range (0, prefab.Length)]) as GameObject;
				go.transform.parent = parent;
				go.transform.localPosition = new Vector3(pos, 0, i * -0.01f);
			}
		}
	}
}
