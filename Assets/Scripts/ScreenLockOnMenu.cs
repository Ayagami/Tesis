using UnityEngine;
using System.Collections;

public class ScreenLockOnMenu : MonoBehaviour {
	void FixedUpdate(){
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}
