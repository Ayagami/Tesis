using UnityEngine;
using System.Collections;

public class ScreenLockOnMenu : MonoBehaviour {
	void FixedUpdate(){
		if (Cursor.lockState == CursorLockMode.Locked) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
