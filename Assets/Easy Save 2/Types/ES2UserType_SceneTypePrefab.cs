
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_SceneTypePrefab : ES2Type {
	public override void Write(object obj, ES2Writer writer) {
		int data = (int) obj;
		// Add your writer.Write calls here.
		writer.Write(data);

	}
	
	public override object Read(ES2Reader reader) {
		SceneTypePrefab data = (SceneTypePrefab) reader.Read<System.Int32>();
		// Add your reader.Read calls here and return your object.
		return data;
	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_SceneTypePrefab():base(typeof(SceneTypePrefab)){}
}
