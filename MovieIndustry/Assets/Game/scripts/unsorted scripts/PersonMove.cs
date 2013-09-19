using UnityEngine;
using System.Collections;

//Этот скрипт не используется теперь.
public class PersonMove : MonoBehaviour 
{
	public float yMin = 0;
	public float yMax = 0;
	public float xMax = 0;
	public float xMin = 0;
	public float speed;
	Transform thisTr;
	Vector3 movement;
//	PersonController pers;
	float time = 0;
	
	
	// Use this for initialization
	void Awake()
	{
		thisTr = transform;
	}
	
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(time >= 2)
		{
			ChangeMoveDir();
			time = 0;
		}
		else
		{
			thisTr.Translate(movement * Time.deltaTime * speed);
			time += Time.deltaTime;
		}
		Vector3 tempPos = thisTr.position;
		if(tempPos.x > xMax)
		{
			tempPos.x = xMax;
			movement = Vector3.zero;
			//pers.animType = CharAnimationType.idle;
		}
		if(tempPos.x < -xMax)
		{
			tempPos.x = -xMax;
			movement = Vector3.zero;
			//pers.animType = CharAnimationType.idle;
		}
		if(tempPos.y < yMin)
		{
			tempPos.y = yMin;
			movement = Vector3.zero;
			//pers.animType = CharAnimationType.idle;
		}
		if(tempPos.y > yMax)
		{
			tempPos.y = yMax;
			movement = Vector3.zero;
			//pers.animType = CharAnimationType.idle;
		}
		thisTr.position = tempPos;
		//print (thisTr.position.magnitude);
	}
	
	void ChangeMoveDir()
	{
		
		movement = Vector3.zero;
		movement.y = Random.Range(-10 , 10);
		movement.x = Random.Range(-10, 10);
		if(movement.x > 0)
		{
			thisTr.localScale = new Vector3(-2,2,2);
		}
		else if(movement.x < 0)
		{
			thisTr.localScale = new Vector3(2,2,2);
		}
		//pers.animType = CharAnimationType.walk;
		//thisTr.Translate(movement * Time.deltaTime * speed);
	}
}
