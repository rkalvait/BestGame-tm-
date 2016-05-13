using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

public class SpellMaker : MonoBehaviour {

	public GameObject damagePrefab;
	public GameObject negatePrefab;
	public Button SpellButtonPrefab;

	public RectTransform SpellsPanel;
	public RectTransform ImbuesPanel;
	public InputField NameField;
	public Dropdown Menu;
	public Text MenuLabel;
	public Button DoneButton;
	public Text ErrorText;

	public bool _____________________________________;
	
	public static AudioSource audio;

	private GameObject activeSpell;
	public string  activeImbue;
	public Stack imbues;

	public static GameObject[] Spells = new GameObject[5];
	public static string[] Names = new string[5];

	public Button[] CustomButtons;
	public Button[] NormalButtons;
	public bool CustomSpellBook = false;

	private string[] unlockedSpells;
	private string[] unlockedImbues;

	// Use this for initialization
	void Start () {
		// Hide the save menu objects
		NameField.gameObject.SetActive (false);
		Menu.gameObject.SetActive (false);
		DoneButton.gameObject.SetActive (false);
		ErrorText.gameObject.SetActive (false);

		// Init some vars
		PersistentData persistentData = GameObject.Find ("Persistent Object").GetComponent<PersistentData>();
		unlockedSpells = persistentData.unlockedSpells;
		unlockedImbues = persistentData.unlockedImbues;
		imbues = new Stack ();
		CustomButtons = new Button[5];
		NormalButtons = new Button[unlockedSpells.Length];

		// Generate spellbook UI buttons
		setupSpells ();
		setupImbues ();
		loadCustomSpells ();

		// Get the audiosource
		audio = GetComponent<AudioSource> ();

	}

	/*
	 * Wait for esc key. Then go back to main game
	 */
	void Update() {
		if (Input.GetKeyDown ("escape")) {
			Application.LoadLevelAsync(1);
		}
	}

	/*
	 * Create the base spell for the user to imbue
	 */
	public void CastSpell(string type) {
		// Hide save menu items if visible
		NameField.gameObject.SetActive (false);
		Menu.gameObject.SetActive (false);
		DoneButton.gameObject.SetActive (false);
		ErrorText.gameObject.SetActive (false);

		// Destroy the old spell
		if (activeSpell != null) {
			Destroy(activeSpell);
		}

		int manaCost = 0;

		// Find the correct prefab, Instantiate
		switch (type) {
		case "Light Damage":
			manaCost = 10;

			activeSpell = Instantiate(damagePrefab) as GameObject;
			activeSpell.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			activeSpell.GetComponent<Damage>().damage = 15;
			activeSpell.GetComponent<ScriptManager>().size = 0.5f;
			break;
		case "Damage":
			manaCost = 25;

			activeSpell = Instantiate(damagePrefab) as GameObject;
			activeSpell.GetComponent<Damage>().damage = 25;
			break;
		case "Heavy Damage":
			manaCost = 75;

			activeSpell = Instantiate(damagePrefab) as GameObject;
			activeSpell.transform.localScale = new Vector3(2, 2, 2);
			activeSpell.GetComponent<Damage>().damage = 50;
			activeSpell.GetComponent<ScriptManager>().size = 2f;
			break;
		case "Negation":
			manaCost = 25;

			activeSpell = Instantiate(negatePrefab) as GameObject;
			activeSpell.GetComponent<ScriptManager>().size = 2f;
			break;
		case "Clear":
			//reset();
			return;
		// Custom spells ////////////////////////////////////////////////////////
		default:
			//Debug.Log (type);
			int idx = int.Parse(type);
			if (Spells[idx] != null) {
				activeSpell = Instantiate(Spells[idx]) as GameObject;
				activeSpell.gameObject.transform.position = new Vector3(0, 1, 0);
			}
			return;
		}

		// Move it to the right spot. Store the mana costs
		activeSpell.gameObject.transform.position = new Vector3(0, 1, 0);
		activeSpell.GetComponent<ScriptManager>().mana = manaCost;

	}

	/*
	 * Imbue the current spell
	 */
	public void Imbue(string imbuement) {
		// Hide the save menu items if visible
		NameField.gameObject.SetActive (false);
		Menu.gameObject.SetActive (false);
		DoneButton.gameObject.SetActive (false);
		ErrorText.gameObject.SetActive (false);

		if (activeSpell == null)
			return;

		// Parse the input string. Remove whitespace, split by colons
		imbuement = imbuement.Replace (" ", "");
		string[] tokens = imbuement.Split (':');
		imbuement = tokens [0];
		BaseImbue scriptComponent = activeSpell.GetComponent<ScriptManager>().attachScript (imbuement) as BaseImbue;
		// Strings split by colons will act as arguments for the imbuement script.
		scriptComponent.args = tokens;
		
		// Add the imbuement to the stack
		if (scriptComponent != null) {
			imbues.Push(activeImbue);
			activeImbue = imbuement;
			//Debug.Log (imbues.Peek());
		}
	}

	/*
	 * Enable the input field to save the current spell
	 */
	public void Save() {
		audio.Play ();
		// Don't allow saving null spells
		if (activeSpell == null)
			return;

		NameField.gameObject.SetActive (true);
		Menu.gameObject.SetActive (true);
		DoneButton.gameObject.SetActive (true);

	}

	/*
	 * Store the spell in the array, save the prefab for future use.
	 */
	public void Store() {
		string name = NameField.text;
		// No empty names. No names that are already in use.
		if (name == "" || name == null || System.Array.IndexOf(Names, name) != -1) {
			ErrorText.gameObject.SetActive (true);
			return;
		}

		activeSpell.GetComponent<ScriptManager> ().isCustom = true;
		int index = Menu.value;

		// Create a new asset file if there is not already one.
		if (Spells [index] == null) {
			activeSpell.GetComponent<ScriptManager> ().isCustom = true;
			Object prefab = PrefabUtility.CreateEmptyPrefab ("Assets/Temporary/" + name + ".prefab");
			GameObject spell = PrefabUtility.ReplacePrefab (activeSpell, prefab, ReplacePrefabOptions.ConnectToPrefab);
			Spells [index] = spell;
			Names [index] = name;
		// If an asset file already exists, replace it.
		} else {
			string path = "Assets/Temporary/"+Names[index]+".prefab";
			Debug.Log (AssetDatabase.RenameAsset(path, name)); // Print any errors
			Object prefab = AssetDatabase.LoadAssetAtPath<Object>("Assets/Temporary/"+name+".prefab");
			GameObject spell = PrefabUtility.ReplacePrefab (activeSpell, prefab, ReplacePrefabOptions.ConnectToPrefab);
			Spells [index] = spell;
			Names [index] = name;
		}

		string menuString = ""+(index+1)+": "+name;

		Menu.options [index].text = menuString;
		MenuLabel.text = menuString;

		// Hide the save menu objects. Reload the custom spells portion of the spellbook.
		NameField.gameObject.SetActive (false);
		Menu.gameObject.SetActive (false);
		DoneButton.gameObject.SetActive (false);
		ErrorText.gameObject.SetActive (false);
		loadCustomSpells ();
	}

	/*
	 * Generate the spells and imbues menu
	 */
	void setupSpells() {
		for (int i=0; i<unlockedSpells.Length; i++) {
			string capture = unlockedSpells[i]; // Prevent threading issues
			Button spellButton = Instantiate(SpellButtonPrefab);
			spellButton.GetComponentInChildren<Text>().text = capture;
			spellButton.GetComponent<Button>().onClick.AddListener( () => {
				CastSpell (capture);
				audio.Play();
			});
			float height = spellButton.GetComponent<RectTransform>().rect.height;
			spellButton.transform.SetParent(SpellsPanel);
			spellButton.transform.localPosition = new Vector3(0,-height*i,0);
			NormalButtons[i] = spellButton;
		}
	}
	void setupImbues() {
		for (int i=0; i<unlockedImbues.Length; i++) {
			string capture = unlockedImbues[i]; // Prevent threading issues
			Debug.Log(capture);
			Button spellButton = Instantiate(SpellButtonPrefab);
			spellButton.GetComponentInChildren<Text>().text = "Imbue: "+capture;
			spellButton.GetComponent<Button>().onClick.AddListener( () => {
				Imbue (capture);
				audio.Play();
			});
			float height = spellButton.GetComponent<RectTransform>().rect.height;
			spellButton.transform.SetParent(ImbuesPanel);
			spellButton.transform.localPosition = new Vector3(0,-height*i,0);
		}
	}

	/*
	 * Load all the prefabs from the Temporary folder and store them in the Spells array
	 */
	void loadCustomSpells() {

		// First, destroy the old buttons
		foreach (Button b in CustomButtons) {
			if (b != null)
				Destroy(b.gameObject);
		}

		// Find all the prefabs and store them
		string[] lookFor = new string[] {"Assets/Temporary"};
		string[] guids = AssetDatabase.FindAssets ("", lookFor);
		for (int i=0; i<guids.Length; i++) {
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			Spells[i] = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
			Debug.Log(Spells[i]);

			// Find the Spell name from the path string
			string substr = path.Split('/')[2];
			Names[i] = substr.Substring(0, substr.Length - 7);
			string menuString = ""+(i+1)+": "+Names[i];
			Menu.options[i].text = menuString;
			if (Menu.value == i) {
				MenuLabel.text = menuString;;
			}
		}

		// Create the buttons
		for (int i=0; i<Names.Length; i++) {
			if (Names[i] == null || Names[i] == "")
				continue;
			string capture = Names[i];
			string idxcap = ""+i;
			Button spellButton = Instantiate(SpellButtonPrefab);
			spellButton.GetComponentInChildren<Text>().text = capture;
			spellButton.GetComponent<Button>().onClick.AddListener( () => {
				CastSpell (idxcap);
				audio.Play();
			});
			float height = spellButton.GetComponent<RectTransform>().rect.height;
			spellButton.transform.SetParent(SpellsPanel);
			spellButton.transform.localPosition = new Vector3(0,-height*i,0);
			spellButton.gameObject.SetActive(CustomSpellBook);
			CustomButtons[i] = spellButton;
		}
	}

	/*
	 * Tobble between Normal/Custom spells
	 */
	public void ToggleSpellBook() {
		CustomSpellBook = ! CustomSpellBook;
		foreach (Button b in NormalButtons) {
			if (b != null)
				b.gameObject.SetActive(!CustomSpellBook);
		}
		foreach (Button b in CustomButtons) {
			if (b != null)
				b.gameObject.SetActive(CustomSpellBook);
		}
	}
}
