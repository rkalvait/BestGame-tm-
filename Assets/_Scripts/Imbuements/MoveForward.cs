using UnityEngine;
using System.Collections;

public class MoveForward : BaseImbue {
	
	public float speed = 5.0f;
	public ScriptManager manager;

	void Start() {
		manager = GetComponent<ScriptManager>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Check if the object is being held in place
		if (!manager.shouldUpdate)
			return;

		// Move the x position of this Object forward
		Vector3 move = new Vector3(speed, 0, 0);
		transform.position += move * speed * Time.deltaTime;
	}
}
