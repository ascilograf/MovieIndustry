using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//чарт фильмов, здесь происходит обновление списка фильмов, сортировка, заполнение праметров.

public class MapFilmsChart : MonoBehaviour 
{
	public GameObject tabButton;
	public Camera cam;
	public float maxLength = -720;
	public float minLength = -1070;
	public Transform chartInfosParent;
	public MapChartItem[] filmItemsInfo;
	public MovieRental rental;
	public Camera swipeCam;
	public Camera masksCamera;
	public tk2dUIScrollableArea scrollArea;
	
	float yMax;
	Rect r;
	
	bool isOpen = false;
	
	// Use this for initialization
	void Start () 
	{
		/*float h = Screen.height;
		float w = Screen.width;
		w = w/1024;
		h = h / 768;
		transform.localPosition = new Vector3(transform.localPosition.x * ( w + 0.036f), transform.localPosition.y, transform.localPosition.z);
		transform.localScale = new Vector3(transform.localScale.x * h, transform.localScale.y * h, transform.localScale.z);
		minLength *= w;//transform.localPosition.x;
		maxLength *= w;*/
		
		
		float w = Screen.width;
		w = (w / 1024) ;
		
		maxLength *= w;
		minLength *= w;
		OffsetChart();
	}
	
	void OffsetChart()
	{
		Transform tr = transform;
		float w = Screen.width;
		float h = Screen.height;
		
		w = w / 1024;
		h = h / 768;
		
		tr.localPosition = new Vector3(tr.localPosition.x * w, tr.localPosition.y * h, tr.localPosition.z);
		tr.localScale = new Vector3(transform.localScale.x * w, transform.localScale.y * h, transform.localScale.z);
	}
	
	void OnEnable()
	{
		Messenger<GameObject>.AddListener("Tap on target", CheckTap);	
	}
	
	void OnDisable()
	{
		MinimizeTab();
		Messenger<GameObject>.RemoveListener("Tap on target", CheckTap);
	}
	
	void CheckTap(GameObject g)
	{
		if(g == tabButton)
		{
			if(isOpen)
				GlobalVars.soundController.CloseBoxOffice();
			else
				GlobalVars.soundController.OpenBoxOffice();
			StartCoroutine(MoveTab(transform.localPosition.x));
		}
	}
	
	public void RefreshFilmList()
	{	
		for(int i = 0; i < filmItemsInfo.Length; i++)
		{
			if(i < rental.films.Count)
			{
				filmItemsInfo[i].RefreshParams();
				if(filmItemsInfo[i].film != null && !filmItemsInfo[i].isMoving)
				{
					
					if(filmItemsInfo[i].film != rental.films[i])
					{
						for(int k = 0; k < rental.films.Count; k++)
						{
							if(rental.films[k] == filmItemsInfo[i].film)
							{
								Vector3 v3 = filmItemsInfo[i].transform.localPosition;
								v3.z = k * 10 - 5;
								filmItemsInfo[i].transform.localPosition = v3;
								v3.y = (k * -166) + 170;
								filmItemsInfo[i].isMoving = true;
								Coroutiner.StartCoroutine(Utils.SwitchFilmPlanes(filmItemsInfo[i], k, filmItemsInfo[i].transform.localPosition, v3));
							}
						}
						//filmItemsInfo[i].SetParams(rental.films[i]);
					}
				}
				else if(filmItemsInfo[i].film == null)
				{
					filmItemsInfo[i].SetParams(rental.films[i]);
				}
			}
			else
			{
				filmItemsInfo[i].RefreshParams();
			}
			SetTextInChildWithName(filmItemsInfo[i].gameObject, "filmN", (i + 1).ToString());
			yMax = rental.films.Count * 140 - 213;
		}
		scrollArea.ContentLength = yMax;
		SortFilmList();
	}
	
	void SortFilmList()
	{
		rental.films.Sort(delegate(FilmItem item1, FilmItem item2)
		{
			return item2.budgetTaken.CompareTo(item1.budgetTaken);
		});
	}
	
	void SetTextInChildWithName(GameObject target, string childName, string currency)
	{
		tk2dTextMesh mesh = target.transform.FindChild(childName).GetComponent<tk2dTextMesh>();
		mesh.text = currency;
	}
	
	public void MaximizeTab()
	{
		Vector3 v3 = transform.localPosition;
		v3.x = maxLength;
		transform.localPosition = v3;
	}
	
	public void MinimizeTab()
	{
		Vector3 v3 = transform.localPosition;
		v3.x = minLength;
		transform.localPosition = v3;
	}
	
	public void ToBottom()
	{
		scrollArea.Value = 1;
	}
	
	
	IEnumerator MoveTab(float fromPos, float t = 0)
	{
		while(true)
		{
			Vector3 v3 = Vector3.zero;
			print ("!!!");
			if(t >= 0.5f || Input.GetMouseButtonDown(0))
			{
				v3 = transform.localPosition;
				if(isOpen)
				{
					v3.x = minLength;
				}
				else
				{
					v3.x = maxLength;
				}
				transform.localPosition = v3;
				isOpen = !isOpen;
				yield break;
			}
			v3 = transform.localPosition;
			if(isOpen)
			{
				v3.x = Mathf.Lerp(fromPos, minLength, t * 2);
			}
			else
			{
				v3.x = Mathf.Lerp(fromPos, maxLength, t * 2);
			}
			transform.localPosition = v3;
			t += Time.deltaTime;
			yield return 0;
		}
	}
}
