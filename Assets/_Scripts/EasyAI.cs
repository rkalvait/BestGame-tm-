using UnityEngine;
using System.Collections;

public class EasyAI : MonoBehaviour {

	public float speed;
	private bool up = true;
	private Player playerScript;

	// Use this for initialization
	void Start () {
		playerScript = this.GetComponent<Player> ();
		CastNegateAndWait ();
	}

	void FixedUpdate() {
		Vector3 velocity;
		if (up) {
			velocity = new Vector3 (0, 1, 0);
		} else {
			velocity = new Vector3 (0, -1, 0);
		}

		GetComponent<Rigidbody2D> ().velocity = velocity * speed;
	}

	void OnCollisionEnter2D(Collision2D coll) {
		up = !up;
	}

	void CastNegateAndWait() {
		playerScript.CastSpell ("Negation");
		playerScript.ReleaseSpell ();

		Invoke ("CastDamageAndWait", 3);
	}

	void CastDamageAndWait() {
		playerScript.CastSpell ("Damage");
		playerScript.Imbue ("Magnetism");
		playerScript.Imbue ("TargetNearest:Player");
		playerScript.ReleaseSpell ();

		Invoke ("CastNegateAndWait", 3);
	}
}
