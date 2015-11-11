using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {
	public float speed;

	void FixedUpdate(){
		transform.Rotate ( Vector3.up * speed * Time.deltaTime);
	}
}
