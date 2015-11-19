using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewLevelOnEditor : MonoBehaviour {

	private GameObject[] Base_levels;

	public InputField LevelName;

	public Dropdown baseLevelsGraphic;
	public Text baseLevelLabel;

	public void Init(GameObject[] levels){
		Base_levels = levels;
		UpdateLevels ();
	}



	// Use this for initialization
	void Start () {
		 //this.gameObject.SetActive (false);
	}

	void UpdateLevels(){
		for(int i=0; i < Base_levels.Length; i++){
			Dropdown.OptionData option = new Dropdown.OptionData (Base_levels[i].name);
			baseLevelsGraphic.options.Add( option );
		}
	}

	public void OnBaseLevelChanged(){
		baseLevelLabel.text = Base_levels [baseLevelsGraphic.value].name;
	}

	public void CreateLevel(){
		if (ObjectsManagersInEditor.GetInstance ().CreateNewLevel (LevelName.text, baseLevelsGraphic.value))
			Cancel ();
		else {
			Debug.Log("HUBO UN ERROR PROCESANDO TU NUEVO NIVEL");
		}
	}

	public void Show(){
		ObjectsManagersInEditor.GetInstance ().EditMode (false);
		LevelName.text = "";
		baseLevelsGraphic.value = 0;
		baseLevelLabel.text = Base_levels [0].name;
		this.gameObject.SetActive (true);

	}

	public void Cancel(){
		this.gameObject.SetActive (false);
		ObjectsManagersInEditor.GetInstance ().EditMode (true);
	}
}
