using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelLoadLevelBehaviour : MonoBehaviour {

	public Transform container;
	public GameObject buttonLevelPrefab;

	private List<string> levels;

	public void SetLevels(List<string> levels){
		this.levels = levels;
	}

	public void Show(){
		ObjectsManagersInEditor.GetInstance ().EditMode (false);
		Clean ();
		Fill ();
		this.gameObject.SetActive (true);
	}
	
	public void Cancel(){
		this.gameObject.SetActive (false);
		ObjectsManagersInEditor.GetInstance ().EditMode (true);
	}


	void Clean(){
		var children = new List<GameObject>();
		foreach (Transform child in container)
			children.Add(child.gameObject);
		children.ForEach(child => Destroy(child));
	}

	void Fill(){
		for (int i=0; i < this.levels.Count; i++) {
			GameObject go = Instantiate(buttonLevelPrefab) as GameObject;
			go.GetComponent<ButtonLevelBehaviour>().Init(this.levels[i], this.levels[i]);
			go.transform.parent = container;
		}
	}
}
