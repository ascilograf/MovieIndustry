using UnityEngine;
using System.Collections;

public class UniversalMiniPlane : MonoBehaviour 
{
	public GameObject forCasting;
	public GameObject forScenes;
	public GameObject forFinish;
	
	//for casting
	public Transform avatarPlace;
	public GetTextFromFile characterType;
	public tk2dTextMesh characterName;
	public tk2dTextMesh characterPrice;
	public GameObject smileIcon;
	public FilmStaff staff;
	
	//for finish
	public tk2dUIProgressBar expBar;
	public tk2dTextMesh expValueMesh;
	public Transform lvlParent;
	public tk2dTextMesh lvlMesh;
	public GameObject lvlUpArrow;
	
	//for scenes
	public MeshRenderer normalFit;
	public MeshRenderer greatFit;	
	public Color rareColor;
	public Color uniqueColor;
	public tk2dSlicedSprite frame;
	public SettingButton scene;
	
	int tempExp = 0;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	public void SetParamsForCasting(FilmStaff s, bool isSmile)
	{
		staff = s;
		Activate(forCasting);
		GameObject g = Instantiate(staff.icon.GetComponent<CharInfo>().avatar.gameObject) as GameObject;
		g.transform.parent = avatarPlace.transform;
		g.transform.localPosition = -Vector3.forward;
		g.transform.localScale = Vector3.one;
		characterType.SetTextWithIndex(((int)staff.GetComponent<PersonController>().type) - 1);
		characterName.text = Utils.FormatStringToText(staff.name, 9);
		characterPrice.text = Utils.ToNumberWithSpaces(staff.icon.GetComponent<CharInfo>().GetPrice().ToString());
		characterPrice.gameObject.SetActive(true);
		expBar.gameObject.SetActive(false);
		lvlUpArrow.SetActive(false);
		lvlParent.gameObject.SetActive(false);
		smileIcon.SetActive(!isSmile);
		ShowRegular();
	}
	
	public void SetParamsForCasting(SettingButton sb, bool isNormal)
	{
		scene = sb;
		Activate(forScenes);
		GameObject g = null;
		switch(sb.setting)
		{
		case Setting.Adventure:
			g = sb.adventure;
			break;
		case Setting.Fantasy:
			g = sb.fantasy;
			break;
		case Setting.Historical:
			g = sb.historical;
			break;
		case Setting.Modern:
			g = sb.modern;
			break;
		case Setting.Space:
			g = sb.space;
			break;
		case Setting.War:
			g = sb.war;
			break;
		}
		if(sb.rarity == RarityLevel.rare)
			frame.color = rareColor;
		else if(sb.rarity == RarityLevel.unique)
			frame.color = uniqueColor;
		
		GameObject go = Instantiate(g) as GameObject;
		go.transform.GetChild(0).gameObject.SetActive(false);
		go.transform.parent = transform;
		go.transform.localPosition = new Vector3(0,0, -10);
		go.transform.localScale = Vector3.one;
		greatFit.enabled = !isNormal;
		normalFit.enabled = isNormal;
	}
	
	public void SetParamsForCasting(Setting setting, RarityLevel rarity, GameObject icon, bool isNormal)
	{
		//scene = sb;
		Activate(forScenes);
		GameObject g = icon;
		
		if(rarity == RarityLevel.rare)
			frame.color = rareColor;
		else if(rarity == RarityLevel.unique)
			frame.color = uniqueColor;
		
		GameObject go = Instantiate(g) as GameObject;
		go.transform.GetChild(0).gameObject.SetActive(false);
		
		go.transform.parent = transform;
		go.transform.localPosition = new Vector3(0,0, -10);
		go.transform.localScale = Vector3.one;
		greatFit.enabled = !isNormal;
		normalFit.enabled = isNormal;
		
	}
	
	public void SetParamsForfinishFilm(FilmStaff s, int expAdd)
	{
		staff = s;
		Activate(forCasting);
		GameObject g = Instantiate(staff.icon.GetComponent<CharInfo>().avatar.gameObject) as GameObject;
		g.transform.parent = avatarPlace.transform;
		g.transform.localPosition = -Vector3.forward;
		g.transform.localScale = Vector3.one;
		characterType.SetTextWithIndex(((int)staff.GetComponent<PersonController>().type) - 1);
		characterName.text = Utils.FormatStringToText(staff.name, 9);
		characterPrice.gameObject.SetActive(false);
		expBar.gameObject.SetActive(true);
		smileIcon.SetActive(false);
		lvlParent.gameObject.SetActive(true);

		if(staff.freeSkillPoints > 0)
		{
			lvlUpArrow.SetActive(true);
		}
		else
		{
			lvlUpArrow.SetActive(false);
		}
		
		ShowRegular();
		StartCoroutine(ChangeProgressAnimation(staff.exp, staff.exp + expAdd, staff.lvl));
	}
	
	void Activate(GameObject currState)
	{
		forCasting.SetActive(false);
		forScenes.SetActive(false);
		//forFinish.SetActive(false);
		
		currState.SetActive(true);
	}
	
	public void ShowForFinalScreen()
	{
		characterName.transform.localPosition = new Vector3(-19, 10, -1);
		characterType.gameObject.SetActive(false);
	}
	
	public void ShowRegular()
	{
		characterName.transform.localPosition = new Vector3(-19, 0, -1);
		characterType.gameObject.SetActive(true);
	}
	
	IEnumerator ChangeProgressAnimation(int curr, int next, int lvl, float t = 0)
	{
		while(true)
		{
			if(t >= 2 || Input.GetMouseButtonDown(0))
			{
				tempExp = next;
				if((staff.lvl + 1) * 100 < staff.exp)
				{
					next = (curr + (next - curr)) - (staff.lvl + 1) * 100;
					curr = 0;
					staff.lvl++;
					staff.freeSkillPoints++;
					lvlUpArrow.SetActive(true);
					StartCoroutine(AnimateLvl());	
				}
				staff.exp = next;
				ChangeProgress(staff.exp, staff.lvl);
				
				lvlMesh.text = staff.lvl.ToString();
				yield break;
			}
			
			staff.exp = (int)Mathf.Lerp(curr, next, t / 2);
			if((staff.lvl + 1) * 100 < staff.exp)
			{
				next = (curr + (next - curr)) - (staff.lvl + 1) * 100;
				curr = 0;
				t = 0;
				staff.freeSkillPoints++;
				lvlUpArrow.SetActive(true);
				StartCoroutine(AnimateLvl());
				staff.lvl++;
			}
			lvlMesh.text = staff.lvl.ToString();
			t += Time.deltaTime;
			ChangeProgress(staff.exp, staff.lvl);
			yield return 0;
		}
	}
	
	//определение прогресса в процентах
	float ProgressInPercents(int e, int l)
	{
		int percents = e / ((100*(l + 1))/100);
		return percents;
	}
	
	//изменение прогрессбара опыта
	void ChangeProgress(int e, int l)
	{
		expValueMesh.text = Utils.ToNumberWithSpaces(staff.exp.ToString()) + " / " + Utils.ToNumberWithSpaces((100*(staff.lvl + 1)).ToString());
		expBar.Value = ProgressInPercents(staff.exp, staff.lvl) / 100;
	}
	
	IEnumerator AnimateLvl(float t = 0)
	{
		while(true)
		{
			if(t >= 2 || Input.GetMouseButton(0))
			{
				lvlParent.localScale = Vector3.one;
				lvlUpArrow.transform.localScale = Vector3.one;
				yield break;
			}
			lvlUpArrow.transform.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.one, t/2);
			lvlParent.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.one, t/2);
			t += Time.deltaTime;
			yield return 0;
		}
	}
}
