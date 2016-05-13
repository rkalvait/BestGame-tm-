using UnityEngine;
using System.Collections;

public class LastBreath : BaseImbue {

	public Player owner;

	// Use this for initialization
	void Start () {
		owner = GetComponent<ScriptManager>().owner.GetComponent<Player>();
	}
	
	void OnDestroy () {
		Debug.Log(owner);
		GameObject spell = owner.CastSpell(args[1]);

		if (spell==null) {
			Debug.Log("Cast failed");
			return;
		}

		spell.transform.position = transform.position;
		Debug.Log (args[1]);
		
		// Imbue only if args contains an imbue
		if (args.Length <= 2) {
			owner.ReleaseSpell();
			return;
		}
		
		// Recombine the args string for each imbuement
		string imbue = "";
		for (int i=2; i<args.Length; i++) {
			if (args[i] == "Imbue" && imbue != "") {
				Debug.Log(owner);
				owner.Imbue(imbue);
				imbue = "";
			} else if(args[i] == "Imbue") {
				// Do nothing
			} else {
				if (imbue == "") {
					imbue += args[i];
				} else {
					imbue += ":"+args[i];
				}
			}
		}
		
		if (imbue != "")
			owner.Imbue(imbue);
		
		owner.ReleaseSpell();
		Debug.Log ("END DEST");
	}
}
