using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SimpleSQL;

public class HomeManager : MonoBehaviour {
	public Animation popInAnimation;
	public Text txtVersion;  
	public SimpleSQLManager dbManager;
	
	public class listDB
	{
		public string id { get; set; }
	}

	public class wordPackage
	{
		[PrimaryKey]
		public int id { get; set; }
		public string word { get; set; }
		public int wordListID { get; set; }
		public string fileName { get; set; }
	}
	
	private List<wordPackage> words;

	private bool launched = false;
	private bool mouseDown = false;
	private float timeMouseDown = 0.0f;
	public string versionNum;


	// Use this for initialization
	void Start () {
		Application.targetFrameRate = -1;
		QualitySettings.vSyncCount = 0;
		Time.timeScale = 1;
		launched = false;
		txtVersion.text = "v " + versionNum;
		loadUserSettings();
	}
	
	// Update is called once per frame
	void Update () {
		if(mouseDown == true){
			timeMouseDown += Time.fixedDeltaTime;
			if(timeMouseDown>3.0 && !launched){
				//Debug.Log ("Launching past Parental Gate");
				launched=true;
				popInAnimation.Play("PGrollOut");
				loadParentalGate();
			}
		}
	}

	private void loadUserSettings()
	{
		Debug.Log ("Loading user settings");
		
		//Check for any BKG Music settings, if none add defaults
		if (PlayerPrefs.HasKey("BkgMusic")){
		}else{
			PlayerPrefs.SetInt("BkgMusic",1);
		}
		
		//Check for any SoundFX settings, if none add defaults
		if (PlayerPrefs.HasKey("SoundFx")){
		}else{
			PlayerPrefs.SetInt("SoundFx",1);
		}
		
		//Check for any Uppercase settings, if none add defaults
		if (PlayerPrefs.HasKey("Uppercase")){
		}else{
			PlayerPrefs.SetInt("Uppercase",1);
		}
		
		//Check for any Audible settings, if none add defaults
		if (PlayerPrefs.HasKey("Audible")){
		}else{
			PlayerPrefs.SetInt("Audible",1);
		}
		
		//Check for any ShowWord settings, if none add defaults
		if (PlayerPrefs.HasKey("ShowWord")){
		}else{
			PlayerPrefs.SetInt("ShowWord",1);
		}
		
		//Check for any GameSpeed settings, if none add defaults
		if (PlayerPrefs.HasKey("GameSpeed")){
		}else{
			PlayerPrefs.SetInt("GameSpeed",3);
		}
	}

	private void loadParentalGate()
	{
		Application.LoadLevel ("SettingsMenu");
	}

	public void gotoWordList()
	{
		Application.LoadLevel ("WordListManager");
	}

	public void gotoGames()
	{
		int activeList = 0;
		activeList = PlayerPrefs.GetInt ("ActiveWordList");
		if(activeList>0){
			Debug.Log ("We have an active list: "+activeList);
			
			//If they have audio selection enabled, make sure all words have audio
			if(PlayerPrefs.GetInt("Audible")==1){
				words = getWords (getActiveList());
				if(allWordsHaveAudio()){
					Application.LoadLevel ("GameSelection");
				}else{
					MessageCenterController.Instance.displayMessage ("You have selected Use Audible Words, but not all words have audio.");
				}
			}else{
				Application.LoadLevel ("GameSelection");
			}
		}else{
			//try and grab from the db
			int curActiveList = getActiveList();
			if(curActiveList > 0){
				activeList = curActiveList;
				PlayerPrefs.SetInt ("ActiveWordList",curActiveList);
				//If they have audio selection enabled, make sure all words have audio
				if(PlayerPrefs.GetInt("Audible")==1){
					words = getWords (getActiveList());
					if(!allWordsHaveAudio()){
						MessageCenterController.Instance.displayMessage ("You have selected Use Audible Words, but not all words have audio.");
					}
				}
			}else{
				Debug.Log ("User has no active lists or no lists to make active");
				MessageCenterController.Instance.displayMessage ("You have no active lists. Tap the Words button to add a list or to make a list active.");
			}
		}
	}

	private List<wordPackage> getWords (int activeID)
	{	
		string sql = "SELECT w.id, w.word, w.wordListID, a.fileName FROM SM_Words w LEFT JOIN SM_WordAudio a ON w.id = a.wordID WHERE w.wordListID = " + activeID;
		List<wordPackage> wordList = dbManager.Query<wordPackage> (sql);
		
		return wordList;
	}

	private bool allWordsHaveAudio()
	{
		bool allAudio = true;
		foreach (wordPackage word in words)
		{
			if(string.IsNullOrEmpty ( word.fileName)){
				allAudio = false;
			}
		}
		return allAudio;
	}

	private int getActiveList()
	{
		bool lastInsert = false;
		int activeListID = 0;
		//listDB lastWordID = new listDB();
		string lastRowQuery = "SELECT id from SM_WordList WHERE isActive = 1 ORDER BY id DESC limit 1";
		listDB lastWordID = dbManager.QueryFirstRecord<listDB> (out lastInsert, lastRowQuery);
		if(lastInsert){
			activeListID  = int.Parse(lastWordID.id);
		}
		
		return activeListID;
	}

	public void OnPointerDown(){
		mouseDown = true;
		popInAnimation.Play("PGrollIn");
	}

	public void OnPointerUp(){
		mouseDown = false;
		timeMouseDown = 0.0f;
		popInAnimation.Play("PGrollOut");
	}
}
