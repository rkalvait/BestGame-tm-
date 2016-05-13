using UnityEngine;
using System.Collections;

public class Negate_All : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D coll) {
		Destroy (coll.gameObject);
	}
}
