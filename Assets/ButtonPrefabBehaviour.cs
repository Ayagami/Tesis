using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class ButtonPrefabBehaviour : MonoBehaviour {

	public Text TextLabel;
	public Button BtnRef;


	private int prefId = -1;

	public void Init(string text, int id){
		this.TextLabel.text = text;
		this.prefId = id;
		BtnRef.onClick.AddListener (OnButtonClick);
	}

	public void OnButtonClick(){
		ObjectsManagersInEditor.GetInstance ().AddPrefabObjectToScene (this.prefId);
	}
}
