using UnityEngine;
using System.Collections;

public class PersistentData : MonoBehaviour {

	public string[] unlockedSpells;
	public string[] unlockedImbues;

	void Awake() {
		Debug.Log (GameObject.Find("Persistent Object"));
	}

	// Use this for initialization
	void Start () {
		Object.DontDestroyOnLoad(this);
		unlockedSpells = Player.unlockedSpells;
		unlockedImbues = Player.unlockedImbues;
	}
}
