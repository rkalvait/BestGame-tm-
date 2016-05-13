using UnityEngine;
using System.Collections;

public class Orbit : BaseImbue {
	
	public float speed  = 100.0f; // Deg per sec
	public float frameStep = 0.1f;
	public float radius = 1.5f;
	public GameObject target;
	
	private Vector3 stay = new Vector3(0,0,0);
	private float angle = 0.0f;
	
	// Update is called once per frame
	void LateUpdate () {
		// Check if the object is being held in place
		if (!gameObject.GetComponent<ScriptManager> ().shouldUpdate || gameObject.GetComponent<ScriptManager> ().target == null) {
			gameObject.GetComponent<Rigidbody2D> ().velocity = stay;
			return;
		}
		
		// MovetTowards the next position
		angle += speed*Time.deltaTime;
		Vector3 target_pos = gameObject.GetComponent<ScriptManager> ().target.transform.position;
		float x_pos = radius * Mathf.Cos(angle*Mathf.PI/180) + target_pos.x;
		float y_pos = radius * Mathf.Sin(angle*Mathf.PI/180) + target_pos.y;

		Vector3 move = new Vector3(x_pos, y_pos, 0.0f);
		float t = frameStep/Vector3.Distance(transform.position, move);
		this.transform.position = Vector3.Lerp (this.transform.position, move, t);
	}
}
