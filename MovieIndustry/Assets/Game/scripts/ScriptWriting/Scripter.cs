using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//доп. скрипт который вешается только на сценаристов и управляет цветом иконок, расположением иконок, а также содержит параметры сценариста
public class Scripter : MonoBehaviour 
{
	public List<FilmSkills> skills;			//умения по жанрам
	public int lvl;							//уровень
	public int exp;							//кол-во опыта
	public GameObject icon;					//иконка
	public GameObject avatar; 				//аватар персонажа
	public bool canBeUsed;					//может ли быть использованным
	public tk2dTextMesh price;				//префаб текстМеша
	public MeshRenderer mark;
	
	
	
	void Start () 
	{
		RefreshStats();
		PersonController pc = GetComponent<PersonController>();
		InstPartOfAvatar(pc.head.gameObject);
		InstPartOfAvatar(pc.body.gameObject);
		InstPartOfAvatar(pc.legs.gameObject);
		//и тут же деактивируем их, чтоб их не было видно
		Transform[] tr = icon.GetComponentsInChildren<Transform>();
		for(int i = 0; i < tr.Length; i++)
		{
			tr[i].gameObject.SetActive(false);
		}
	}
	
	//инициализация автара
	void InstPartOfAvatar(GameObject go)
	{
		GameObject g = Instantiate(go) as GameObject;
		g.transform.parent = avatar.transform;
		g.transform.localPosition = new Vector3(0,0, -10);
		g.transform.localScale = new Vector3(2,2,2);
		g.GetComponent<tk2dAnimatedSprite>().playAutomatically = false;
		g.layer = 10;
		g.GetComponent<tk2dAnimatedSprite>().Stop();
	}
	
	//обновление значений в прогрессбарах
    public void RefreshStats()
	{
		for(int i = 0; i < skills.Count; i++)
		{
			//skills[i].progressBar.Value = skills[i].skill * 0.01f;
		}
		price.text = (lvl * 1000).ToString();
		price.Commit();
	}

	void Update () 
	{
	//	icon.transform.localScale = new Vector3(transform.localScale.x / 4, 0.5f, 0.5f);
		RefreshStats();
	}
}
