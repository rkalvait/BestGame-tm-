using UnityEngine;
using System.Collections;

public class Book : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.tag == "Player") {
			coll.GetComponent<Player>().storePersistentData();
			Application.LoadLevelAsync("_SpellMaker");
		}
	}
}
