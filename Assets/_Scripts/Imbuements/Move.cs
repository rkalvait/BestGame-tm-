using UnityEngine;
using System.Collections;

public class Move : BaseImbue {

	int delay;
	string imbue;
	ScriptManager manager;
	public float x_speed;
	public float y_speed;
	public float speed;
	
	/*
	 * Get the spell arguments from the ScriptManager.
	 */
	void Start () {
		manager = GetComponent<ScriptManager> ();
		if (!manager.isCustom) {
			speed   = float.Parse(args [1]);
			Debug.Log(speed);
			foreach (string s in args) {
				Debug.Log(s);
			}
			// Allow for either 2 or 3 args. 3rd arg is the x- and y- speed pair
			// Separated by a comma
			// X speed default to 1, Y speed default to 0
			if (args.Length < 3) {
				x_speed = 1;
				y_speed = 0;
			} else {
				string[] speeds = args[2].Split(',');
				x_speed = float.Parse(speeds [0].Trim());
				y_speed = float.Parse(speeds [1].Trim());
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Check if the object is being held in place
		if (!manager.shouldUpdate)
			return;

		//Debug.Log("MOVING DIAGONALLY GUISE" + speed);
		// Move the x position of this Object forward
		Vector3 move = new Vector3(x_speed*speed, y_speed*speed, 0);
		transform.position += move * Time.deltaTime;
	}
}
