using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

	public GameObject damagePrefab;
	public GameObject negatePrefab;
	public GameObject selector;
	public Text manaField;
	public Text hpField;
	public Text errorField;
	public Vector3 spellPosition;
	public string[] _unlockedSpells;
	public string[] _unlockedImbues;
	public GameObject persistentObject;

	public bool ____________________________________;
	public static string[] unlockedSpells;
	public static string[] unlockedImbues;
	public int health = 100;
	public int mana   = 100;
	public bool stunned = false;
	public GameObject activeSpell;
	public string  activeImbue;
	public Stack spells;
	public Stack imbues;
	private PersistentData persistentData;

	public void Awake() {
		unlockedImbues = _unlockedImbues;
		unlockedSpells = _unlockedSpells;
	}

	public void Start() {
		imbues = new Stack ();
		spells = new Stack ();
		persistentData = persistentObject.GetComponent<PersistentData>();
	}

	public void Update() {
		//if (activeSpell != null && !activeSpell.GetComponent<ScriptManager>().shouldUpdate)
		//	activeSpell.gameObject.transform.position = transform.position + spellPosition;
		updateHPBar ();
		if (Input.GetKeyDown("escape")) {
			Application.Quit();
		}
	}

	/**
	 * Create a new Spell in front of the player
	 */
	public GameObject CastSpell(string type) {

		if (stunned)
			return null;

		if (activeSpell != null)
			spells.Push (activeSpell);

		int manaCost = 0;

		switch (type) {
		case "Light Damage":
			manaCost = 10;
			// Return if too expensive
			if (manaCost > mana) {
				return null;
			}
			activeSpell = Instantiate(damagePrefab) as GameObject;
			activeSpell.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			activeSpell.GetComponent<Damage>().damage = 15;
			activeSpell.GetComponent<ScriptManager>().size = 0.5f;
			break;
		case "Damage":
			manaCost = 25;
			// Return if too expensive
			if (manaCost > mana) {
				return null;
			}
			activeSpell = Instantiate(damagePrefab) as GameObject;
			activeSpell.GetComponent<Damage>().damage = 25;
			break;
		case "Heavy Damage":
			manaCost = 75;
			// Return if too expensive
			if (manaCost > mana) {
				return null;
			}
			activeSpell = Instantiate(damagePrefab) as GameObject;
			activeSpell.transform.localScale = new Vector3(2, 2, 2);
			activeSpell.GetComponent<Damage>().damage = 50;
			activeSpell.GetComponent<ScriptManager>().size = 2f;
			break;
		case "Negation":
			manaCost = 25;
			// Return if too expensive
			if (manaCost > mana) {
				return null;
			}
			activeSpell = Instantiate(negatePrefab) as GameObject;
			activeSpell.GetComponent<ScriptManager>().size = 2f;
			break;
		case "Clear":
			reset();
			return null;
		// CUSTOM SPELLS ////////////////////////////////////////////////////////////////
		default:

			int idx = int.Parse(type);
			if (SpellMaker.Spells[idx] == null)
				return null;

			//activeSpell = Instantiate(SpellMaker.Spells[idx]) as GameObject;
			//activeSpell.gameObject.transform.position = new Vector3(0, 1, 0);

			manaCost = SpellMaker.Spells[idx].GetComponent<ScriptManager>().mana;
			// Return if too expensive
			if (manaCost > mana) {
				return null;
			}
			activeSpell = Instantiate(SpellMaker.Spells[idx]) as GameObject;
			mana -= manaCost;
			activeSpell.gameObject.transform.position = transform.position + spellPosition;
			activeSpell.GetComponent<ScriptManager> ().owner = this.gameObject;
			UpdateManabar ();
			UpdateSelector ();
			ReleaseSpell();
			return null;
		}
		mana -= manaCost;
		activeSpell.gameObject.transform.position = transform.position + spellPosition;
		activeSpell.GetComponent<ScriptManager>().mana = manaCost;
		activeSpell.GetComponent<ScriptManager> ().owner = this.gameObject;
		UpdateManabar ();
		UpdateSelector ();
		return activeSpell;
	}

	/**
	 * Allow the current spell to move
	 */
	public void ReleaseSpell() {
		if (activeSpell == null)
			return;

		activeSpell.GetComponent<ScriptManager> ().shouldUpdate = true;
		if (spells.Count > 0) {
			activeSpell = (GameObject)spells.Pop ();
		} else {
			activeSpell = null;
		}

		UpdateSelector ();
	}

	/**
	 * Attach a script to the active spell.
	 */
	public void Imbue(string imbuement) {
		if (activeSpell == null)
			return;
		imbuement = imbuement.Replace (" ", "");
		string[] tokens = imbuement.Split (':');
		imbuement = tokens [0];
		BaseImbue scriptComponent = activeSpell.GetComponent<ScriptManager>().attachScript (imbuement) as BaseImbue;
		scriptComponent.args = tokens;

		// Add the imbuementto the stack
		if (scriptComponent != null) {
			imbues.Push(activeImbue);
			activeImbue = imbuement;
			//Debug.Log (imbues.Peek());
		}
	}
	
	/*
	 * Handle collisions with spells. Currently just take damage.
	 */ 
	public void OnTriggerEnter2D(Collider2D coll) {
		//Debug.Log (coll.gameObject.name);
		if (coll.gameObject.tag == "DamageSpell" && coll.GetComponent<ScriptManager>().owner != this.gameObject) {
			health -= coll.GetComponent<Damage>().damage;
			Destroy (coll.gameObject);
			GameManager.audio.PlayOneShot(GameManager.damage);
			if (health <= 0) {
				Destroy (this.gameObject);
			}
		}
	}

	/**
	 * Store data that needs to be transferred between levels into the persistent object
	 */
	public void storePersistentData() {
		persistentData.unlockedImbues = unlockedImbues;
		persistentData.unlockedSpells = unlockedSpells;
	}

	/*
	 * Destroy all spells that have owner == this.gameobject
	 */
	private void reset() {
		GameObject[] damageSpells = GameObject.FindGameObjectsWithTag ("DamageSpell");
		GameObject[] negationSpells = GameObject.FindGameObjectsWithTag ("NegationSpell");
		foreach (GameObject o in damageSpells) {
			if (o.GetComponent<ScriptManager>().owner == this.gameObject) {
				Destroy (o);
			}
		}
		foreach (GameObject o in negationSpells) {
			if (o.GetComponent<ScriptManager>().owner == this.gameObject) {
				Destroy (o);
			}
		}
		imbues = new Stack ();
		spells = new Stack ();
		activeSpell = null;

		UpdateSelector ();
	}

	public void UpdateActiveSpell(GameObject test) {
		if (activeSpell==test) {
			ReleaseSpell();
		}
	}

	public void UpdateSelector() {
		if (activeSpell == null) {
			selector.transform.position = new Vector3 (-50, -50, -50);
		} else {
			selector.transform.position = activeSpell.transform.position;
			float size = activeSpell.GetComponent<ScriptManager>().size;
			selector.transform.localScale = new Vector3(size, size, 1f);
		}
	}

	public void UpdateManabar() {
		manaField.text = "Mana: " + mana;
	}

	public void updateHPBar() {
		hpField.text = "HP: " + health;
	}

	void OnDestroy() {
		hpField.text = "HP: 0";
	}
}
  