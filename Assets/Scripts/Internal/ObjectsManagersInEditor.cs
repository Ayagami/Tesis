﻿using UnityEngine;
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
	public PanelLoadLevelBehaviour panelLoadLevelHandler;

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

	public int FlagsLimit = 2;
	public int PointsLimit = 1;


	public bool IsInEditMode(){
		return isInEditMode;
	}

	public void EditMode(bool b){
		isInEditMode = b;
	}

	void Awake(){
		Instance = this;
		levels = LevelParser.LoadLevelList ();
	}

	public void ClearCurrentObject(){
		currentSelectedObject = null;
	}

	public void setCurrentSelectedObject(Transform t){
		currentSelectedObject = t;
		panelTransformHandler.setCurrentTransform (currentSelectedObject);
	}

	private List<string> levels = null;
	public static List<string> Levels(){
		return Instance.levels;
	}

	// Use this for initialization
	void Start () {
		objectsSpawned = new List<GameObject> ();

		newLevelHandler.Init (BaseMaps);
		panelLoadLevelHandler.SetLevels (this.levels);

		for (int i=0; i < Prefabs.Length; i++) {
			GameObject bt = GameObject.Instantiate(PrefabButton) as GameObject;
			ButtonPrefabBehaviour bpb = bt.GetComponent<ButtonPrefabBehaviour>();
			bt.transform.parent = WhereToPutButtons.transform;
			bpb.Init(Prefabs[i].name, i);
		}

	}
	
	public void AddPrefabObjectToScene(int id){
		if (!checkIfCanSpawn (Prefabs [id].GetComponent<PrefabsInScene> ().type))
			return;


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

	private bool checkIfCanSpawn(SceneTypePrefab type){
		if (type != SceneTypePrefab.BASE_POINT && type != SceneTypePrefab.BASE_FLAG)
			return true;

		if (objectsSpawned.Count <= 0)
			return true;
		Debug.Log ("INSIDE SWITCH");
		switch (type) {
			case SceneTypePrefab.BASE_FLAG:
				int c = 0;
				for(int i=0; i < objectsSpawned.Count; i++){
					PrefabsInScene PIS = objectsSpawned[i].GetComponent<PrefabsInScene>();
					if(PIS){
						if(PIS.type == type){
							c++;
							if(c >= FlagsLimit)
								return false;
						}
					}
				}

				Debug.Log(c);

				if(c >= FlagsLimit)
					return false;
				else 
					return true;
			break;

		case SceneTypePrefab.BASE_POINT:

			int x = 0;
			for(int i=0; i < objectsSpawned.Count; i++){
				PrefabsInScene PIS = objectsSpawned[i].GetComponent<PrefabsInScene>();
				if(PIS){
					if(PIS.type == type){
						x++;
						if(x >= PointsLimit)
							return false;
					}
				}
			}

			Debug.Log(x);
			if(x >= PointsLimit)
				return false;
			else 
				return true;

			break;
		}

		return false;
	}

	public void SaveLevel(){
		LevelParser.Save (CurrentlevelName, objectsSpawned);
	}

	public void LoadLevel(string levelName){
		panelLoadLevelHandler.Cancel ();
		Level newLevel = LevelParser.Load (levelName);

		if (newLevel != null) {
			CurrentlevelName = newLevel.LevelName;
			ParseObjectsInLevel(newLevel);
		}
	}

	private void ParseObjectsInLevel(Level theLevel){
		RemoveOldElementsFromLevel ();
		for (int i=0; i < theLevel.obj.Count; i++) {
			switch(theLevel.obj[i].prefab){
				case SceneTypePrefab.COLLISEUM:
					SpawnBaseLevel(0);
				AssignTransform(objectsSpawned[objectsSpawned.Count-1],theLevel.obj[i].pos,theLevel.obj[i].rot, theLevel.obj[i].scale);
				break;

				case SceneTypePrefab.BASE_FLAG:
					AddPrefabObjectToScene(2);
				AssignTransform(objectsSpawned[objectsSpawned.Count-1],theLevel.obj[i].pos,theLevel.obj[i].rot, theLevel.obj[i].scale);
				break;

				case SceneTypePrefab.BASE_POINT:
					AddPrefabObjectToScene(1);
				AssignTransform(objectsSpawned[objectsSpawned.Count-1],theLevel.obj[i].pos,theLevel.obj[i].rot, theLevel.obj[i].scale);
				break;

				case SceneTypePrefab.SPAWN_POINT:
					AddPrefabObjectToScene(0);
				AssignTransform(objectsSpawned[objectsSpawned.Count-1],theLevel.obj[i].pos,theLevel.obj[i].rot, theLevel.obj[i].scale);
				break;
			}
		}
	}

	private void AssignTransform(GameObject go, Vector3 pos, Vector3 rot, Vector3 scale){
		Debug.Log (pos);

		go.transform.position = pos;
		go.transform.rotation = Quaternion.Euler(rot);
		go.transform.localScale = scale;
	}

	public void UpdateLevels(List<string> levels){
		this.levels = levels;
	}
}