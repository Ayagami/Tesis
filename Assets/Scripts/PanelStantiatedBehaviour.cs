using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelStantiatedBehaviour : MonoBehaviour {

	private List<GameObject> elementsSpawned;
	public Transform container;
	public GameObject buttonElementPrefab;

	public void Init(List<GameObject> gos){
		elementsSpawned = gos;
		UpdateChildrens ();
	}

	public void UpdateChildrens(){
		Clean ();
		Fill ();
	}
	void Clean(){
		var children = new List<GameObject>();
		foreach (Transform child in container)
			children.Add(child.gameObject);
		children.ForEach(child => Destroy(child));
	}
	
	void Fill(){
		for (int i=1; i < this.elementsSpawned.Count; i++) {
			GameObject go = Instantiate(buttonElementPrefab) as GameObject;
			go.GetComponent<ButtonInstantiatedBehaviour>().Init(this.elementsSpawned[i].name, i);
			go.transform.parent = container;
		}
	}
}
