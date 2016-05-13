using UnityEngine;
using System.Collections;

public class ImbueLater : BaseImbue {

	public int delay;
	public string imbue;
	public ScriptManager manager;
	public string[] nextArgs;
	private bool shouldCast = true;

	/*
	 * Get the spell arguments from the ScriptManager.
	 */
	void Start () {
		manager = GetComponent<ScriptManager> ();
		if (!manager.isCustom) {
			imbue = args [1];
			nextArgs = new string[args.Length-1];
			for (int i=0; i<nextArgs.Length; i++) {
				nextArgs[i] = args[i+1];
			}
			delay = int.Parse (args [2]);
		}
	}

	/*
	 * Set the timer after release
	 */
	void Update() {
		if (shouldCast && manager.shouldUpdate) {
			Invoke ("MakeImbue", delay);
			shouldCast = false;
		}
	}

	/*
	 * Attach the imbument script
	 */
	void MakeImbue() {
		BaseImbue imbueComponent = manager.attachScript (imbue) as BaseImbue;
		imbueComponent.args = nextArgs;
	}

}
