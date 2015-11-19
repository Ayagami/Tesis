using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ObjectsManagersInEditor : MonoBehaviour {
	public GameObject[] Prefabs;

	public GameObject UIReference;

	public GameObject PrefabButton;

	public GameObject WhereToPutButtons;
	
	public NewLevelOnEditor newLevelHandler;
	public PanelTransformBehaviour panelTransformHandler;

	private List<GameObject> objectsSpawned;

	[Header("BaseMaps")]
	public GameObject[] BaseMaps;

	[Header("ObjectsSpawneables")]
	public GameObject[] ObjectsSpawneables;

	private static ObjectsManagersInEditor Instance;

	public static ObjectsManagersInEditor GetInstance(){
		return Instance;
	}

	private bool isInEditMode = true;


	[Header("LevelAttributes")]
	private string CurrentlevelName;
	private int CurrentBaseLevel;



	[Header("Other Stuffs")]
	public Transform currentSelectedObject = null;


	public GameObject gizmoPrefab;

	public bool IsInEditMode(){
		return isInEditMode;
	}

	public void EditMode(bool b){
		isInEditMode = b;
	}

	void Awake(){
		Instance = this;
	}

	public void ClearCurrentObject(){
		currentSelectedObject = null;
	}

	// Use this for initialization
	void Start () {
		objectsSpawned = new List<GameObject> ();

		newLevelHandler.Init (BaseMaps);
		for (int i=0; i < Prefabs.Length; i++) {
			GameObject bt = GameObject.Instantiate(PrefabButton) as GameObject;
			ButtonPrefabBehaviour bpb = bt.GetComponent<ButtonPrefabBehaviour>();
			bt.transform.parent = WhereToPutButtons.transform;
			bpb.Init(Prefabs[i].name, i);
		}
	}
	
	public void AddPrefabObjectToScene(int id){
		GameObject go = Instantiate (Prefabs [id], Vector3.zero, Quaternion.identity) as GameObject;
		go.transform.localScale = Prefabs [id].transform.localScale;
		objectsSpawned.Add (go);
		currentSelectedObject = go.transform;
		panelTransformHandler.setCurrentTransform(go.transform);

		Gizmonizer g = go.AddComponent<Gizmonizer> ();
		g.gizmoAxis = gizmoPrefab;
		g.gizmoSize = 0.5f;

	}

	public bool CreateNewLevel(string cLevel, int baseMapRef){

		if (cLevel == "")
			return false;
		if (baseMapRef < 0 || baseMapRef > BaseMaps.Length - 1)
			return false;


		CurrentlevelName = cLevel;
		CurrentBaseLevel = baseMapRef;

		RemoveOldElementsFromLevel ();
		SpawnBaseLevel (CurrentBaseLevel);

		return true;
	}

	private void RemoveOldElementsFromLevel(){
		for (int i=0; i < objectsSpawned.Count; i++) {
			Destroy(objectsSpawned[i]);
		}

		objectsSpawned.Clear ();
		ClearCurrentObject ();
		panelTransformHandler.ClearInputs ();
	}

	public void setTransformToObject(){
		panelTransformHandler.updateTransform ();
	}

	private void SpawnBaseLevel(int baseLevel){
		GameObject go = Instantiate (BaseMaps [baseLevel], Vector3.zero, Quaternion.identity) as GameObject;
		go.transform.localScale = BaseMaps [baseLevel].transform.localScale;

		objectsSpawned.Add (go);
		currentSelectedObject = go.transform;
	}

}