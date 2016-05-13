using UnityEngine;

public class ScriptManager : MonoBehaviour {

	public bool shouldUpdate = false;
	public bool isCustom = false;
	public GameObject owner = null;
	public GameObject target = null;
	public float size = 1;
	public int mana;
	public string[] args;

	/*
	 * Attach a script with the given name to the parent GameObject
	 * Only one of each type of sript may be attached.
	 */
	public Component attachScript(string script) {
		if (this.GetComponent(script) == null)
			return UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(gameObject, "Assets/_Scripts/ScriptManager.cs (12,3)", script);
		return null;
	}

	/*
	 * Return mana to the owner. Update the Active Spell and indicator as needed
	 */
	public void OnDestroy() { 
		if (owner == null)
			return;
		owner.GetComponent<Player> ().mana += this.mana;
		owner.GetComponent<Player>().UpdateManabar ();
		owner.GetComponent<Player>().UpdateActiveSpell (gameObject);
		owner.GetComponent<Player>().UpdateSelector ();
	}
}