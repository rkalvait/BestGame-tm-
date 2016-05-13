using UnityEngine;
using System.Collections;

public class CastSpell : BaseImbue {

	public ScriptManager manager;
	public Player owner;
	public string type;

	void Start() {
		manager = GetComponent<ScriptManager>();
		if (manager.isCustom)
			return;

		type = args[1];
		owner = manager.owner.GetComponent<Player>();
	}

	void Update () {

		if (!manager.shouldUpdate)
			return;

		// Have the owner create the spell
		// This makes it cost mana each spell cast
		GameObject spell = owner.CastSpell(type);

		// return if spellcast failed
		if (spell == null)
			return;

		spell.transform.position = transform.position;

		// Imbue only if args contains an imbue
		if (args.Length <= 2) {
			owner.ReleaseSpell();
			return;
		}

		// Recombine the args string for each imbuement
		string imbue = "";
		for (int i=2; i<args.Length; i++) {
			if (args[i] == "Imbue" && imbue != "") {
				Debug.Log(imbue);
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
	}
}
