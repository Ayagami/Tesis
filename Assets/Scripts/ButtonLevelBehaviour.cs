using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonLevelBehaviour : MonoBehaviour {

	public Text TextLabel;
	public Button BtnRef;
	private string prefId = "-1";
	
	public void Init(string text, string id){
		this.TextLabel.text = text;
		this.prefId = id;
		BtnRef.onClick.AddListener (OnButtonClick);
	}
	
	public void OnButtonClick(){
		ObjectsManagersInEditor.GetInstance ().LoadLevel (this.prefId);
	}

}
