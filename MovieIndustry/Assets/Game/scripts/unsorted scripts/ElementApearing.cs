using UnityEngine;
using System.Collections;

//появление элемента, происходит при активации объекта
//сначала происходит увеличение + выкручивание альфы с 25 до 255
//потом деформация по Х, потом по У, далее происходит возвращение в норму
public class ElementApearing : MonoBehaviour 
{
	
	float time = 0;							//время, темп-переменная		
	//tk2dSprite[] sprites;					//спрайты меню
	Vector3 baseScale;						//начальный масштаб
	public GameObject infos;				//элементы, которые будут показаны после всех деформаций, может быть пустым
	
	void Start()
	{
		
	}
	
	public void OnEnable()
	{
		baseScale = transform.localScale;
		//sprites = GetComponentsInChildren<tk2dSprite>(true);
		StartCoroutine(OnActive(150, 70, 40, 40));
	}
	
	public void OnDisable()
	{
		transform.localScale = baseScale;
	}
	
	//деформации объекта
	IEnumerator OnActive(float period,float step1, float step2, float step3)
	{
		while(true)
		{	
			baseScale = transform.localScale;
			
			
			//смена альфы + увеличение объекта
			time = 0;
			
			do 
			{
				if(infos != null)
				{
					infos.SetActive(false);
				}
				time += 0.1f;
				/*foreach(tk2dSprite s in sprites)
				{
					Color32 col = s.color;
					Color32 c = s.color;
					col.a = 255;
					c.a = 0;
					s.color = Color.Lerp(c, col, time);;
				}*/
				transform.localScale = Vector3.Lerp(Vector3.one * 0.4f, baseScale, time);
				yield return new WaitForSeconds(1/step1);
			}
			while(time < 1);
			time = 0;
			
			//растягивание по Х
			do
			{
				time += 0.4f;
				Vector3 v3 = Vector3.one;
				v3.y = Mathf.Lerp(baseScale.y - 0.1f, baseScale.y + 0.1f, time);
				v3.x = Mathf.Lerp(baseScale.x + 0.1f, baseScale.x - 0.1f, time);
				transform.localScale = v3;
				yield return new WaitForSeconds(1/step2);
			}
			while(time < 1);
			time = 0;
			
			//растягивание по У
			do
			{
				time += 0.4f;
				Vector3 v3 = Vector3.one;
				v3.y = Mathf.Lerp(baseScale.y + 0.1f, baseScale.y - 0.1f, time);
				v3.x = Mathf.Lerp(baseScale.x - 0.1f, baseScale.y + 0.1f, time);
				transform.localScale = v3;
				yield return new WaitForSeconds(1/step3);
			}
			while(time < 1);
			
			//возвращение в норму
			do
			{
				time += 0.4f;
				Vector3 v3 = Vector3.one;
				v3.y = Mathf.Lerp(baseScale.y + 0.1f, baseScale.y, time);
				v3.x = Mathf.Lerp(baseScale.x - 0.1f, baseScale.y, time);
				transform.localScale = v3;
				yield return new WaitForSeconds(2/step3);
			}
			while(time < 1);
			
			if(infos != null)
			{
				infos.SetActive(true);
			}
			Messenger.Broadcast("Menu appear");
			yield break;
		}
	}
}
