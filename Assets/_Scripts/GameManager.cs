using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

public class GameManager : MonoBehaviour {

	public GameObject MainPlayer;
	public Button SpellButtonPrefab;
	public RectTransform SpellsPanel;
	public RectTransform ImbuesPanel;
	public AudioClip _negate;
	public AudioClip _damage;

	public Button ImbueLeft;
	public Button ImbueRight;

	public bool ______________________________;

	public static string[] Names;
	public static GameObject[] Spells;
	public bool CustomSpellBook;
	Button[] CustomButtons;
	Button[] NormalButtons;
	Button[] ImbueButtons;

	public static AudioClip negate;
	public static AudioClip damage;
	public static AudioSource audio;

	private int spellPage;
	private int imbuePage;

	// Use this for initialization
	void Start () {
		negate = _negate;
		damage = _damage;
		Names = SpellMaker.Names;
		Spells = SpellMaker.Spells;
		CustomButtons = new Button[6];

		setupSpells ();
		setupImbues ();
		loadCustomSpells ();
		audio = GetComponent<AudioSource> ();

		spellPage = 0;
		imbuePage = 0;
	}

	/*
	 * Create the button objects for base spells
	 */
	void setupSpells() {
		NormalButtons = new Button[Player.unlockedSpells.Length];
		string[] unlockedSpells = Player.unlockedSpells;
		for (int i=0; i<unlockedSpells.Length; i++) {
			string capture = unlockedSpells[i];
			Button spellButton = Instantiate(SpellButtonPrefab);
			spellButton.GetComponentInChildren<Text>().text = capture;
			spellButton.GetComponent<Button>().onClick.AddListener( () => {
				MainPlayer.GetComponent<Player> ().CastSpell (capture);
				audio.Play();
			});
			float height = spellButton.GetComponent<RectTransform>().rect.height;
			spellButton.transform.SetParent(SpellsPanel);
			spellButton.transform.localPosition = new Vector3(0,-height * (i%6),0);
			// Limit 6 per page
//			if (i > (5*(imbuePage+1))) 
//				spellButton.gameObject.SetActive(false);
			NormalButtons[i] = spellButton;
		}
		updateSpells();
	}

	/*
	 * Create the button objects for imbuements
	 */
	void setupImbues() {
		ImbueButtons  = new Button[Player.unlockedImbues.Length];
		string[] unlockedImbues = Player.unlockedImbues;
		for (int i=0; i<unlockedImbues.Length; i++) {
			string capture = unlockedImbues[i];
			Button spellButton = Instantiate(SpellButtonPrefab);
			spellButton.GetComponentInChildren<Text>().text = "Imbue: "+capture;
			spellButton.GetComponent<Button>().onClick.AddListener( () => {
				MainPlayer.GetComponent<Player> ().Imbue (capture);
				audio.Play();
			});
			float height = spellButton.GetComponent<RectTransform>().rect.height;
			spellButton.transform.SetParent(ImbuesPanel);
			spellButton.transform.localPosition = new Vector3(0,-height*(i % 6),0);
			// Limit 6 per page
//			if (i > (5*(imbuePage+1))) 
//				spellButton.gameObject.SetActive(false);
			ImbueButtons[i] = spellButton;
		}
		updateImbues();
	}

	/*
	 * Re-compute which spell buttons should be shown
	 */
	public void updateSpells() {
		for (int i=0; i<NormalButtons.Length; i++) {
			if (i >= spellPage*6 && i<(spellPage+1)*6) {
				NormalButtons[i].gameObject.SetActive(true);
			} else {
				NormalButtons[i].gameObject.SetActive(false);
			}
		}
	}

	/*
	 * Re-compute which imbue buttons should be shown
	 */
	public void updateImbues() {
		for (int i=0; i<ImbueButtons.Length; i++) {
			if (i >= imbuePage*6 && i<(imbuePage+1)*6) {
				ImbueButtons[i].gameObject.SetActive(true);
			} else {
				ImbueButtons[i].gameObject.SetActive(false);
			}
		}
		// Add/Remove page turn buttons
		ImbueLeft.gameObject.SetActive(true);
		ImbueRight.gameObject.SetActive(true);

		if (imbuePage == 0) 
			ImbueLeft.gameObject.SetActive(false);
		
		if (imbuePage >= (ImbueButtons.Length-1)/6) 
			ImbueRight.gameObject.SetActive(false);
		
		
	}
	
	/*
	 * "Turn the page" on the spell panel. Hide the existing buttons and show 6 hidden buttons
	 * @param direction: 1=forward 1 page, -1=backward 1 page, 0=no change
	 */
	public void spellPageTurn(int direction) {
		spellPage += direction;
		updateSpells();
	}
	
	/*
	 * "Turn the page" on the imbue panel. Hide the existing buttons and show 6 hidden buttons
	 * @param direction: 1=forward 1 page, -1=backward 1 page, 0=no change
	 */
	public void imbuePageTurn(int direction) {
		imbuePage += direction;
		updateImbues();
	}

	/*
	 * Recreate the UI for spells. Used when new spells added
	 */
	public void reloadSpells() {
		foreach (Button b in NormalButtons) {
			Destroy (b.gameObject);
		}
		setupSpells();
	}
	/*
	 * Recreate the UI for imbues. Used when new imbues added
	 */
	public void reloadImbues() {
		foreach (Button b in ImbueButtons) {
			Destroy (b.gameObject);
		}
		setupImbues();
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
			//Debug.Log(Spells[i]);
			
			// Find the Spell name from the path string
			string substr = path.Split('/')[2];
			Names[i] = substr.Substring(0, substr.Length - 7);
		}
		
		// Create the buttons
		CustomButtons = new Button[6];
		for (int i=0; i<Names.Length; i++) {
			if (Names[i] == null || Names[i] == "")
				continue;
			string capture = Names[i];
			string idxcap = ""+i;
			Button spellButton = Instantiate(SpellButtonPrefab);
			spellButton.GetComponentInChildren<Text>().text = capture;
			spellButton.GetComponent<Button>().onClick.AddListener( () => {
				MainPlayer.GetComponent<Player>().CastSpell (idxcap);
				audio.Play();
			});
			float height = spellButton.GetComponent<RectTransform>().rect.height;
			spellButton.transform.SetParent(SpellsPanel);
			spellButton.transform.localPosition = new Vector3(0,-height*i,0);
			spellButton.gameObject.SetActive(CustomSpellBook);
			CustomButtons[i] = spellButton;
		}

		// Make the Clear spell
		Button btn = Instantiate(SpellButtonPrefab);
		btn.GetComponentInChildren<Text>().text = "Clear";
		btn.GetComponent<Button>().onClick.AddListener( () => {
			MainPlayer.GetComponent<Player> ().CastSpell ("Clear");
			audio.Play();
		});
		float h = btn.GetComponent<RectTransform>().rect.height;
		btn.transform.SetParent(SpellsPanel);
		btn.transform.localPosition = new Vector3(0,-h*Names.Length,0);
		btn.gameObject.SetActive(false);
		CustomButtons[5] = btn;
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
