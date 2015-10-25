using UnityEngine;
using System.Collections;

public class Player_CameraManager : MonoBehaviour {

	public CamaraJugador CJ;
	public SimpleSmoothMouseLook SSL;

	public CameraState currentState = CameraState.CAMERA_MOVEMENT;

	[SerializeField]
	private Vector3 prevStateBeforeChange = Vector3.zero;
	[SerializeField]
	private Quaternion prevRotBeforeChange = Quaternion.identity;


	private bool ShouldChange = false;

	public Transform PlayerTarget;

	void Awake(){
		currentState = CameraState.CAMERA_MOVEMENT;
	}
	// Use this for initialization
	void Start () {
	
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.P)) {
			if(!ShouldChange){
				ChangeState( currentState == CameraState.CAMERA_MOVEMENT ? CameraState.CAMERA_SHOOTING : CameraState.CAMERA_MOVEMENT);
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (ShouldChange) {
			if(currentState == CameraState.CAMERA_SHOOTING){
				transform.position = Vector3.Lerp(transform.position, SSL.WhereShouldBe.position, Time.deltaTime * 10f);
				transform.rotation = Quaternion.Lerp(transform.rotation, SSL.WhereShouldBe.rotation, Time.deltaTime * 30f);

				if( (transform.position - SSL.WhereShouldBe.transform.position).magnitude < 0.3f ) {
					transform.position = SSL.WhereShouldBe.transform.position;
					transform.rotation = SSL.WhereShouldBe.transform.rotation;

					SSL.enabled = true;
					ShouldChange = false;
				}
			}else{

				/*transform.position = Vector3.Lerp(transform.position, prevStateBeforeChange - PlayerTarget.position, Time.deltaTime * 10f);
				transform.rotation = Quaternion.Lerp(transform.rotation, prevRotBeforeChange, Time.deltaTime * 30f);

				if( (transform.position - (prevStateBeforeChange - PlayerTarget.position)).magnitude < 0.3f ) {
					transform.position = prevStateBeforeChange - PlayerTarget.position;
					transform.rotation = prevRotBeforeChange;
					
					CJ.enabled = true;
					ShouldChange = false;
				}*/
				CJ.enabled = true;
				ShouldChange = false;
			}
		}
	}

	void ChangeState(CameraState newState){
		currentState = newState;
		switch (currentState) {
		case CameraState.CAMERA_MOVEMENT:
			SSL.enabled = false;
			ShouldChange = true;
			break;
		case CameraState.CAMERA_SHOOTING:
			CJ.enabled = false;
			prevStateBeforeChange = PlayerTarget.position + transform.position;
			prevRotBeforeChange  = transform.rotation;
			ShouldChange = true;
			break;
		default:
			Debug.Log("NO DEBERIA LLEGAR ACA");
			break;
		}
	}
}

public enum CameraState{
	CAMERA_MOVEMENT,
	CAMERA_SHOOTING
}