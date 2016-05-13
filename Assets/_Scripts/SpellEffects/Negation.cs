using UnityEngine;
using System.Collections;

public class Negation : MonoBehaviour {

	public float mana;

	void OnTriggerEnter2D(Collider2D coll) {
		//Debug.Log (coll.gameObject.name);
		if (coll.gameObject.tag == "DamageSpell" && GetComponent<ScriptManager>().owner != coll.GetComponent<ScriptManager>().owner) {
			GameManager.audio.PlayOneShot(GameManager.negate);
			Destroy (coll.gameObject);
			Destroy (this.gameObject);
		}
	}
}
