using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//инвентарь, здесь содержатся все предметы 
//а также контроль построения меню и показ его на экране
public class Inventory : MonoBehaviour 
{
	public List<InventoryItem> items;						//список вещей в инвентаре
	public bool showInventory;								//показать/не показать инвентарь
	Vector2 inventoryScroll;								//скроллинг инвентаря
	
	void Awake()
	{
		GlobalVars.inventory = this;
	}
	/*
	void Update()
	{
		//по нажатию на пробел показываем инвентарь
		if(Input.GetKeyDown(KeyCode.Space) && GlobalVars.cameraStates == CameraStates.normal)
		{
			showInventory = !showInventory;
		}
		if(showInventory)
		{
			GlobalVars.cameraStates = CameraStates.menu;
		}
	}
	
	// отображение инвентаря
    public void OnGUI()
    {
        if (showInventory)
        {
            if(GUI.Button(new Rect(10, 10, 150, 25), "Hide Inventory"))
            {
				GlobalVars.cameraStates = CameraStates.normal;
                showInventory = !showInventory;
            }

            GUILayout.BeginArea(new Rect(Screen.width-500, 10, 300, Screen.height-20), GUI.skin.box);
            {
                inventoryScroll = GUILayout.BeginScrollView(inventoryScroll, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));

                // рисуем каждую вещь инвентаря
                foreach (InventoryItem item in items)
                {
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.Label(item.itemName); // Название
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(item.inventoryTexture); // Иконка
                    GUILayout.BeginVertical();
                    if (GUILayout.Button("Use"))  // Кнопка "использовать"
                    {
						if(item.GetComponent<Scenario>() != null)
						{
							if(GameObject.FindGameObjectWithTag("Pavillion") != null)
							{
								GameObject go =	Instantiate(GlobalVars.popUpHireForFilm) as GameObject;
								go.transform.parent = Camera.main.transform;
								go.transform.localPosition = new Vector3(0,900, 500);
								go.GetComponent<HireForFilm>().script = item.GetComponent<Scenario>();
								go.GetComponent<HireForFilm>().price = item.GetComponent<Scenario>().price;
								go.GetComponent<HireForFilm>().priceMesh.text = (item.GetComponent<Scenario>().price).ToString();
								go.GetComponent<HireForFilm>().priceMesh.Commit();
								items.Remove(item);
								showInventory = false;
								GlobalVars.cameraStates = CameraStates.normal;
								break;
							}
						}
						if(item.GetComponent<FilmItem>() != null)
						{
							if(GameObject.FindGameObjectWithTag("postProdOffice") != null)
							{
								GameObject go =	Instantiate(GlobalVars.popUpHireEffects) as GameObject;
								go.transform.parent = Camera.main.transform;
								go.transform.localPosition = new Vector3(0,0, 100);
								go.GetComponent<AddVisualEffects>().film = item.GetComponent<FilmItem>();
								items.Remove(item);
								showInventory = false;
								GlobalVars.cameraStates = CameraStates.menu;
								break;
							}
						}
                    }
                    if (GUILayout.Button("Sell"))  // Кнопка "продать"
					{
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }


                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();

        }
	}*/
}
