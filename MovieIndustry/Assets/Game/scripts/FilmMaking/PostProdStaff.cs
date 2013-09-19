using UnityEngine;
using System.Collections;

//здесь содержатся доп. параметры персонала постпродакшена, 
//также этот скрипт отвечает за обновление прогрессбара Visuals
//и занятость постпрод. персонала
public class PostProdStaff : MonoBehaviour 
{
	public int visuals;						//уровень умения
	public UIProgressBar progressBar;		//прогресс бар
	public GameObject icon;					//иконка
	public GameObject avatar; 				//аватар персонажа
	public bool canBeUsed;					//может ли быть использованным
	public tk2dTextMesh price;				//префаб текстМеша
	public MeshRenderer mark;				//маркер выбора
	bool inst = false;						//инициализирован ли аватар
	public bool busy;						//занят ли персонаж
	
	void Start () 
	{
		RefreshStats();
		
		//и тут же деактивируем их, чтоб их не было видно
		/*Transform[] tr = icon.GetComponentsInChildren<Transform>();
		for(int i = 0; i < tr.Length; i++)
		{
			tr[i].gameObject.active = false;
		}*/
	}
	
	//инициализировать часть аватара (голова, тело, ноги)
	void InstPartOfAvatar(GameObject go)
	{
		GameObject g = Instantiate(go) as GameObject;
		g.transform.parent = avatar.transform;
		g.transform.localPosition = new Vector3(0,0, go.transform.localPosition.z);
		g.transform.localScale = new Vector3(2,2,2);
		g.GetComponent<tk2dAnimatedSprite>().playAutomatically = false;
		g.GetComponent<tk2dAnimatedSprite>().Play("idle");
		g.GetComponent<tk2dAnimatedSprite>().Stop();
		g.layer = 10;
	}
	
	//обновить параметры персонажа
	public void RefreshStats()
	{
		progressBar.Value = visuals * 0.01f;
		price.text = (visuals * 1000).ToString();
		price.Commit();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//если аватар не составлен, то проверяем, собрался ли весь персонаж, и если да, то строим аватар
		if(!inst)
		{
			PersonController pc = GetComponent<PersonController>();
			if(pc.head.gameObject != null && pc.body.gameObject != null & pc.legs.gameObject != null)
			{
				InstPartOfAvatar(pc.head.gameObject);
				InstPartOfAvatar(pc.body.gameObject);
				InstPartOfAvatar(pc.legs.gameObject);
				//и тут же деактивируем их, чтоб их не было видно
				Transform[] tr = icon.GetComponentsInChildren<Transform>();
				for(int i = 0; i < tr.Length; i++)
				{
					//tr[i].gameObject.active = false;
				}
				inst = true;
			}
		}
		//RefreshStats();
	}
	
	void AddStaff()
	{
		
	}
}
