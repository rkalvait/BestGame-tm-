using UnityEngine;
using System.Collections;

public class TargetSelf : BaseImbue {

	
	private ScriptManager manager;
	
	void Start() {
		manager = gameObject.GetComponent<ScriptManager> ();
		manager.target = manager.owner;
	}
}
