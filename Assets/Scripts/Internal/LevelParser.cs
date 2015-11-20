using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level {
	public List<SceneObject> obj;
	public string LevelName;
}

public enum SceneTypePrefab {
	BASE_FLAG,
	BASE_POINT,
	SPAWN_POINT,
	DEST1,
	DEST2,
	DEST3,
	COLLISEUM,
	Count
}

public class SceneObject{
	public SceneTypePrefab prefab;
	public Vector3 pos;
	public Vector3 rot;
	public Vector3 scale;
}

public class LevelParser{
	private static string token        = "87859b6921509e0Au9sjR4ep8H9T1FED0g2JH65E";
	private static string saveFile             = "tesis.txt";
	private static string levelFile			   = "Levels.txt";
	private static string NameTag              = "levels";
	public  static bool useEncrypt             = true;

	private static string calculateString(string tag, string sFile){
		string str = "";
		str += sFile + "?tag=";
		str += tag;
		
		if (useEncrypt){
			str += "&encrypt=true&password=";
			str += token;
		}

		return str;
	}

	public static List<string> LoadLevelList(){

		bool test = ES2.Exists (saveFile);
		if (test){
			return ES2.LoadList< string > ( calculateString(NameTag, saveFile) );
		}
		else
			return (new List<string>()) ;
	}

	public static void SaveLevelList(List<string> levelList){
		ES2.Save (levelList, calculateString(NameTag, saveFile));
	}

	public static void Save(string levelName, List<GameObject> objects){

		Debug.Log ("Trying to save " + levelName);

		Level newLevel = new Level ();
		newLevel.LevelName = levelName;

		List<SceneObject> objectsInScene = new List<SceneObject>();

		for (int i=0; i < objects.Count; i++) {
				SceneObject obj = new SceneObject();
				
				obj.prefab = objects[i].GetComponent<PrefabsInScene>().type;
				obj.pos = objects[i].transform.position;
				obj.scale = objects[i].transform.localScale;
				obj.rot = objects[i].transform.rotation.eulerAngles;

				objectsInScene.Add(obj);
		}

		newLevel.obj = objectsInScene;

		ES2.Save (newLevel, calculateString (levelName, levelFile));

		if(ObjectsManagersInEditor.Levels().IndexOf(levelName) == -1)
			ObjectsManagersInEditor.Levels ().Add (levelName);

		SaveLevelList (ObjectsManagersInEditor.Levels ());
	}

	public static Level Load(string levelName){

		if( ES2.Exists( calculateString(levelName, levelFile) ) ){
			return ES2.Load<Level>(calculateString(levelName, levelFile));
		}

		Debug.Log ("NO LVL " + levelName);

		return null;
	}

	private static string Base64Encode(string plainText) {
		var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
		return System.Convert.ToBase64String(plainTextBytes);
	}
	
	private static string Base64Decode(string base64EncodedData) {
		var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
		return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
	}
}