using UnityEngine;
using System.Collections;

public class tk2dUIDemo5Controller : tk2dUIBaseDemoController {

	public tk2dUILayout[] windows = new tk2dUILayout[0];

	public tk2dUIScrollableArea scrollableArea;
	public tk2dUILayout prefabItem;
	public tk2dUILayout lastListItem;

	void CustomizeListObject( Transform contentRoot ) {
		string[] firstPart = { "Ba", "Po", "Re", "Zu", "Meh", "Ra'", "B'k", "Adam", "Ben", "George" };
		string[] secondPart = { "Hoopler", "Hysleria", "Yeinydd", "Nekmit", "Novanoid", "Toog1t", "Yboiveth", "Resaix", "Voquev", "Yimello", "Oleald", "Digikiki", "Nocobot", "Morath", "Toximble", "Rodrup", "Chillaid", "Brewtine", "Surogou", "Winooze", "Hendassa", "Ekcle", "Noelind", "Animepolis", "Tupress", "Jeren", "Yoffa", "Acaer" };
		string name = firstPart[Random.Range(0, firstPart.Length)] + " " + secondPart[Random.Range(0, secondPart.Length)];
 		Color color = new Color32((byte)Random.Range(192, 255), (byte)Random.Range(192, 255), (byte)Random.Range(192, 255), 255);
		contentRoot.Find("Name").GetComponent<tk2dTextMesh>().text = name;
		contentRoot.Find("HP").GetComponent<tk2dTextMesh>().text = "HP: " + Random.Range(100, 512).ToString();
		contentRoot.Find("MP").GetComponent<tk2dTextMesh>().text = "MP: " + (Random.Range(2, 40) * 10).ToString();
		contentRoot.Find("Portrait").GetComponent<tk2dBaseSprite>().color = color;
	}

	void Start () {
		foreach (tk2dUILayout window in windows) {
			RegisterWindow(window.transform);
		}
	
		// disable the prefab item
		// don't want it visible when the game is running, as it is in the scene
		prefabItem.transform.parent = null;
		DoSetActive( prefabItem.transform, false );

		// Add a bunch of items to the list
		float x = 0;
		float w = (prefabItem.GetMaxBounds() - prefabItem.GetMinBounds()).x;
		for (int i = 0; i < 10; ++i) {
			tk2dUILayout layout = Instantiate(prefabItem) as tk2dUILayout;
			layout.transform.parent = scrollableArea.contentContainer.transform;
			layout.transform.localPosition = new Vector3(x, 0, 0);
			DoSetActive( layout.transform, true );
			CustomizeListObject( layout.transform );
			x += w;
		}
		lastListItem.transform.localPosition = new Vector3(x, lastListItem.transform.localPosition.y, 0);
		x += (lastListItem.GetMaxBounds() - lastListItem.GetMinBounds()).x;
		scrollableArea.ContentLength = x;

		ShowWindow( windows[0].transform );
	}

	IEnumerator AddSomeItems() {
		float x = lastListItem.transform.localPosition.x;
		float w = (prefabItem.GetMaxBounds() - prefabItem.GetMinBounds()).x;
		int numToAdd = Random.Range(1, 5);
		for (int i = 0; i < numToAdd; ++i) {
			tk2dUILayout layout = Instantiate(prefabItem) as tk2dUILayout;
			layout.transform.parent = scrollableArea.contentContainer.transform;
			layout.transform.localPosition = new Vector3(x, 0, 0);
			DoSetActive( layout.transform, true );
			CustomizeListObject( layout.transform );
			x += w;

			lastListItem.transform.localPosition = new Vector3(x, lastListItem.transform.localPosition.y, 0);
			scrollableArea.ContentLength = x + (lastListItem.GetMaxBounds() - lastListItem.GetMinBounds()).x;

			yield return new WaitForSeconds(0.2f);
		}
	}
	
	void NextButtonPressed() {
	}
}
