using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelTransformBehaviour : MonoBehaviour {

	[Header("Position")]
	public InputField posX;
	public InputField posY;
	public InputField posZ;

	[Header("Scaling")]
	public InputField scaleX;
	public InputField scaleY;
	public InputField scaleZ;

	[Header("Rotation")]
	public InputField rotX;
	public InputField rotY;
	public InputField rotZ;


	private Transform currentObject;

	private void setPosition(Vector3 p){
		posX.text = p.x.ToString("0.00");
		posY.text = p.y.ToString("0.00");
		posZ.text = p.z.ToString ("0.00");
	}
	private void setRotation(Vector3 r){
		rotX.text = r.x.ToString ("0.00");
		rotY.text = r.y.ToString ("0.00");
		rotZ.text = r.z.ToString ("0.00");
	}
	private void setScale(Vector3 s){
		scaleX.text = s.x.ToString ();
		scaleY.text = s.y.ToString ();
		scaleZ.text = s.z.ToString ();
	}

	public void setCurrentTransform(Transform t){
		this.currentObject = t;

		setPosition (currentObject.transform.position);
		setRotation (currentObject.transform.rotation.eulerAngles);
		setScale    (currentObject.transform.localScale);
	}

	public void updateTransform(){
		if (!currentObject)
			return;

		setPosition (currentObject.transform.position);
		setRotation (currentObject.transform.rotation.eulerAngles);
		setScale    (currentObject.transform.localScale);
	}

	public void SetTransformToObject(){
		if (!currentObject)
			return;

		Vector3 temporalPos = currentObject.transform.position;
		temporalPos.x = float.Parse(posX.text);
		temporalPos.y = float.Parse (posY.text);
		temporalPos.z = float.Parse (posZ.text);
		currentObject.transform.position = temporalPos;

		Vector3 temporalRot = currentObject.transform.rotation.eulerAngles;
		temporalRot.x = float.Parse (rotX.text);
		temporalRot.y = float.Parse (rotY.text);
		temporalRot.z = float.Parse (rotZ.text);

		currentObject.transform.rotation = Quaternion.Euler (temporalRot);

		Vector3 temporalScale = currentObject.transform.localScale;
		Debug.Log (temporalScale);
		temporalScale.x = float.Parse (scaleX.text);
		temporalScale.y = float.Parse (scaleY.text);
		temporalScale.z = float.Parse (scaleZ.text);

		currentObject.transform.localScale = temporalScale;
	}

	public void ClearInputs(){
		currentObject = null;
		posX.text = "";
		posY.text = "";
		posZ.text = "";

		rotX.text = "";
		rotY.text = "";
		rotZ.text = "";

		scaleX.text = "";
		scaleY.text = "";
		scaleZ.text = "";

	}
}
