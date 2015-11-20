
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_SceneObject : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		SceneObject data = (SceneObject)obj;
		// Add your writer.Write calls here.
		writer.Write(data.prefab);
		writer.Write(data.pos);
		writer.Write(data.rot);
		writer.Write(data.scale);

	}
	
	public override object Read(ES2Reader reader)
	{
		SceneObject data = new SceneObject();
		// Add your reader.Read calls here and return your object.
		data.prefab = reader.Read<SceneTypePrefab>();
		data.pos = reader.Read<UnityEngine.Vector3>();
		data.rot = reader.Read<UnityEngine.Vector3>();
		data.scale = reader.Read<UnityEngine.Vector3>();

		return data;
	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_SceneObject():base(typeof(SceneObject)){}
}
