using UnityEngine;
using System.Collections;

public class InputMove : MonoBehaviour {

	public float speed = 1.0f;
	public float smoothing = 1.0f;
	//public GameObject MainCamera;

	public void FixedUpdate() {
		if (GetComponent<Player> ().stunned)
			return;
		Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
		gameObject.GetComponent<Rigidbody2D> ().velocity = move * speed;
		//transform.position += move * speed * Time.deltaTime;
		//Vector3 camPos = new Vector3 (transform.position.x, transform.position.y, -15);
		//MainCamera.transform.position = Vector3.Lerp (MainCamera.transform.position, camPos, smoothing);
	}
}
