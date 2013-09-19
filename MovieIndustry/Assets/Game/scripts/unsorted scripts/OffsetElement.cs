using UnityEngine;
using System.Collections;

public class OffsetElement : MonoBehaviour {
	
	//public Vector3 offset;
	// Use this for initialization
	void Awake () 
	{
		Transform tr = transform;
		float w = Screen.width;
		float h = Screen.height;
		
		if(camera != null)
		{
			camera.orthographicSize = h/2;
		}
		float aspect = w/h;
		w = w / 1024;
		h = h / 768;

		if(aspect <= 1.35f)
		{
			aspect = 0;
		}
		else if(aspect <= 1.55f)
		{
			aspect = 0.0091f;
		}
		else if(aspect <= 1.63f)
		{
			aspect = 0;
		}
		else if(aspect <= 1.7f)
		{
			aspect = 0;
		}	
		else if(aspect < 1.8f)
		{
			aspect = 0.036f;
		}	
		
		
		tr.localPosition = new Vector3((tr.localPosition.x * (w + aspect)), tr.localPosition.y * h, tr.localPosition.z);
		tr.localScale = new Vector3(transform.localScale.x * h, transform.localScale.y * h, transform.localScale.z);
		Messenger.AddListener("Finger was lifted", Poop);
		//MeshRenderer mr;
	}
	
	void Poop()
	{
		
	}
}
