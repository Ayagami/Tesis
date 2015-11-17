using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level {
	public List<SceneObject> obj;
	public string LevelName;
}

public enum SceneTypePrefab {
	SIMPLE,
	COMP
}

public class SceneObject{
	public SceneTypePrefab prefab;
	public Transform 	   _transform;
}