using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoToEditorButton : MonoBehaviour {

	public Button btn;

	// Use this for initialization
	void Start () {
		btn.onClick.AddListener (GoToEditor);
	}
	
	public void GoToEditor(){
		Destroy (GuiLobbyManager.s_Singleton.gameObject);
		Application.LoadLevel (2);
	}
}
