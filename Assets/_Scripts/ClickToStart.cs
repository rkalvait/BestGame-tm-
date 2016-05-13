using UnityEngine;
using System.Collections;

public class ClickToStart : MonoBehaviour {
	
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0)) {
			Application.LoadLevel(1);
		}
	}
}
