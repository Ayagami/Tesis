
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_Level : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		Level data = (Level)obj;
		// Add your writer.Write calls here.
		writer.Write(data.obj);
		writer.Write(data.LevelName);

	}
	
	public override object Read(ES2Reader reader)
	{
		Level data = new Level();
		// Add your reader.Read calls here and return your object.
		data.obj = reader.ReadList<SceneObject>();
		data.LevelName = reader.Read<System.String>();

		return data;
	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_Level():base(typeof(Level)){}
}
