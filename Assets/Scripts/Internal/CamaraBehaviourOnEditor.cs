using UnityEngine;
using System.Collections;

public class CamaraBehaviourOnEditor : MonoBehaviour {
	[Header("Basic Settings")]
	[SerializeField]
	Vector3 LookAtAwake = Vector3.zero;

	[Header("BasicRotation Settings")]
	public float horizontalSpeed = 1f;
	public float verticalSpeed = 1f;

	[Header("PivotRotation Settings")]
	public float pivotHorizontalSpeed = 1f;
	public float pivotVerticalSpeed = 1f;

	private Vector3 InitialPosition = Vector3.zero;

	void Awake(){
		InitialPosition = transform.position;
		transform.LookAt (LookAtAwake);
	}

	// Use this for initialization
	void Start () {
	
	}

	void FixedUpdate () {
		if (!ObjectsManagersInEditor.GetInstance ().IsInEditMode ())
			return;

		ScrollWheelBehaviour ();
		MovementBehaviour ();
		RotationBehaviour ();
		OtherInputs ();
	}

	void OnDrawGizmosSelected(){
		Gizmos.DrawLine (transform.position, getTarget());
	}

	void ScrollWheelBehaviour(){
		float scrollWheel = Input.GetAxis ("Mouse ScrollWheel");
		transform.Translate ( transform.forward * scrollWheel );
	}

	void MovementBehaviour(){
		Vector3 translation = Vector3.right * Input.GetAxis ("Horizontal") + Vector3.up * Input.GetAxis ("Vertical");
		transform.Translate (translation);
	}

	void RotationFromPivot(){
		transform.RotateAround (getTarget(), transform.up,    Input.GetAxis ("Mouse X") * Time.deltaTime * pivotVerticalSpeed);
		transform.RotateAround (getTarget(), transform.right, Input.GetAxis ("Mouse Y") * Time.deltaTime * pivotHorizontalSpeed);
	}

	void BasicRotation(){
		float h = horizontalSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
		float v = verticalSpeed   * Input.GetAxis("Mouse Y") * Time.deltaTime;
		transform.Rotate(v, h, 0);
	}

	void RotationBehaviour(){
		if (Input.GetMouseButton(1)) {
			if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
				RotationFromPivot();
			}else{
				BasicRotation();
			}
		}
	}

	void ResetToDefault(){
		transform.position = InitialPosition;
		transform.rotation = Quaternion.identity;
		LookAtTarget (getTarget());
	}

	void OtherInputs(){
		if (Input.GetKeyDown (KeyCode.Space))
			ResetToDefault ();
		if (Input.GetKeyDown (KeyCode.LeftAlt))
			LookAtTarget (getTarget());
	}

	void LookAtTarget(Vector3 target){
		transform.LookAt (target);
	}

	Vector3 getTarget(){
		if (ObjectsManagersInEditor.GetInstance ().currentSelectedObject == null)
			return Vector3.zero;
		else
			return ObjectsManagersInEditor.GetInstance ().currentSelectedObject.transform.position;
	}
}
