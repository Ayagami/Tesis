using UnityEngine;
using System.Collections;

public class Gizmonizer : MonoBehaviour 
{
	public GameObject gizmoAxis;
	public float gizmoSize = 1.0f;

	private GameObject gizmoObj;
	private Gizmo gizmo;
	private GizmoHandle.Gizmo_Type gizmo_type = GizmoHandle.Gizmo_Type.Gizmo_Position;

	private static Gizmonizer instance = null;

	void Start(){
		if (instance != null) {
			instance.removeGizmo();
		}

		instance = this;
		resetGizmo ();
	}
	void Update () 
	{

		if (!ObjectsManagersInEditor.GetInstance ().IsInEditMode ())
			return;

		if (Input.GetKeyDown (KeyCode.Escape)) {
			ObjectsManagersInEditor.GetInstance().ClearCurrentObject();
			removeGizmo ();
		}

		if (gizmo) {

			if (Input.GetKeyDown (KeyCode.Delete)) {
				ObjectsManagersInEditor.GetInstance().ClearCurrentObject();
				ObjectsManagersInEditor.GetInstance().DestroyObject(this.gameObject);
			}

			if (Input.GetKeyDown(KeyCode.Alpha1)) 
			{
				gizmo_type = GizmoHandle.Gizmo_Type.Gizmo_Position;
				gizmo.setType(gizmo_type);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2)) {
				gizmo_type = GizmoHandle.Gizmo_Type.Gizmo_Rotation;
				gizmo.setType(gizmo_type);
			}
			if (Input.GetKeyDown(KeyCode.Alpha3)) {
				gizmo_type = GizmoHandle.Gizmo_Type.Gizmo_Scale;
				gizmo.setType(gizmo_type);
			}        
			if (gizmo.needUpdate) {
				resetGizmo();
			}
		}
	}

	/*
	void FixedUpdate(){
		if (gizmoObj) {
			gizmoObj.transform.localScale = gizmoAxis.transform.localScale;
			gizmoObj.transform.localScale *= gizmoSize + (Vector3.Distance(transform.position, Camera.main.transform.position)/30);
		}
	}*/

	public void OnMouseDown() 
	{
		if (!gizmoObj) {
			instance.removeGizmo();
			instance = this;
			ObjectsManagersInEditor.GetInstance().setCurrentSelectedObject(this.transform);
			resetGizmo();
		}
	}
	void removeGizmo() 
	{
		if (gizmoObj) 
		{
			gameObject.layer = 0;
			foreach (Transform child in transform) 
			{
				child.gameObject.layer = 0;
			}        
			Destroy(gizmoObj);    
			Destroy(gizmo);    
		}
	}

	void resetGizmo() {
		removeGizmo();
		gameObject.layer = 2;
		foreach (Transform child in transform) 
		{
			child.gameObject.layer = 2;
		}        
		gizmoObj = Instantiate(gizmoAxis, transform.position, transform.rotation) as GameObject;
		gizmoObj.transform.localScale *= gizmoSize;
		gizmo = gizmoObj.GetComponent<Gizmo>();
		gizmo.setParent(transform);
		gizmo.setType(gizmo_type);
	}

}
