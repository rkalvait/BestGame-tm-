using UnityEngine;
using System.Collections;

public class Magnetism : BaseImbue {
	
	public float speed = 5.0f;
	public GameObject target;
	public ScriptManager manager;

	private Vector3 stay = new Vector3(0,0,0);

	void Start() {
		manager = GetComponent<ScriptManager>();
	}

	// Update is called once per frame
	void LateUpdate () {
		// Check if the object is being held in place
		if (!manager.shouldUpdate || manager.target == null) {
			gameObject.GetComponent<Rigidbody2D> ().velocity = stay;
			return;
		}
		
		// Move the x position of this Object forward
		GameObject target = manager.target;
		Vector3 move = Vector3.Normalize (target.transform.position - transform.position) * speed;
		gameObject.GetComponent<Rigidbody2D> ().velocity = move;
	}
}
