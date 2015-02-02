using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using SimpleSQL;
using System;
using System.IO;
using System.Net;

public class WordDBUI : MonoBehaviour
{
		public SimpleSQLManager dbManager;
		public GameObject addPanel;
		public GameObject confirmPanel;
		public GameObject editPanel;
		public GameObject listPanel;
		public GameObject audioPanel;
		public InputField txtWordName;
		public InputField txtEWordName;
		public Button prefabButton;
		public Animator addListAnimator;
		public Animator confirmAnimator;
		public Animator editAnimator;
		public Animator audioAnimator;
		public Text errorText;
		public Text errorWText;
		public Text listTitle;
		public Button playAudioButton;
		public Button recordAudioButton;
		public Sprite stopImg;
		public Sprite recordImg;
		private AudioClip wordAudio;
		public AudioSource audio;
		private bool startedRecording = false;
	
		//Simple Word class
		public class wordItem
		{
				[PrimaryKey]
				public int id { get; set; }

				public string word { get; set; }

				public int wordListID { get; set; }
		}

		public class wordTitle
		{
				public string title { get; set; }
		}
	
		public class wordContent
		{
				public string word { get; set; }
		}

		public class audioContent
		{
				public string fileName { get; set; }
		}

		//Cancel adding title
		public void cancel ()
		{
				//Hide the Panel
				hidePanel ();
		
				//Reload the list
				loadList ();
		}

		// Use this for initialization
		void Start ()
		{
				//Load the wordlist
				Debug.Log ("Loading List: " + PlayerPrefs.GetInt ("CurrentWordList"));
				//Load list name
				listTitle.text = loadListName (PlayerPrefs.GetInt ("CurrentWordList"));
				addPanel.SetActive (false);
				confirmPanel.SetActive (false);
				editPanel.SetActive (false);
				audioPanel.SetActive (false);
				hidePanel ();
				loadList ();
		}
	
		public void rowTapped (string id)
		{
				//Set the current wordlist
				//PlayerPrefs.SetInt("CurrentWordList",int.Parse (id));
				Debug.Log ("Tapped - " + id);
		}
	
		private void loadList ()
		{
				//Remove the old Lists
				foreach (Button child in listPanel.GetComponentsInChildren <Button>()) {
						Destroy (child.gameObject);
				}
		
				int scrollOffest = 0;
		
				List<wordItem> myWords = getLists ();
				Debug.Log ("Word Count: " + myWords.Count.ToString ());
				foreach (wordItem word in myWords) {
						Button buttonClone = (Button)Instantiate (prefabButton, transform.position, transform.rotation);
						buttonClone.transform.parent = listPanel.transform;
						buttonClone.transform.localScale = Vector3.one;
						//Change the button text to list title
						Text btnTxt = buttonClone.GetComponentInChildren<Text> ();
						btnTxt.text = word.word;
						buttonClone.transform.localPosition = new Vector3 (0, scrollOffest, 0);
						scrollOffest = scrollOffest - 120;
						//Dynamically adjust the scroll container height
						RectTransform scrollRect = listPanel.GetComponent<RectTransform> ();
						scrollRect.sizeDelta = new Vector2 (640, 120 * myWords.Count);
						//Get the id for later reference
						string captured = word.id.ToString ();
						buttonClone.onClick.AddListener (() => rowTapped (captured));
			
						Button[] buttons = buttonClone.GetComponentsInChildren<Button> ();
						foreach (Button btn in buttons) {
								if (btn.name.Equals ("DelButton")) {
										btn.onClick.AddListener (() => showConfirmPanel (captured));
								} else if (btn.name.Equals ("EditButton")) {
										btn.onClick.AddListener (() => showEditPanel (captured));
								} else if (btn.name.Equals ("AudioButton")) {
										btn.onClick.AddListener (() => showAudioPanel (captured));
								}
						}
				}
		}

		//Show the Confirmation Panel
		public void showConfirmPanel (string wordID)
		{
				confirmPanel.SetActive (true);
				//Show the ConfirmPanel and its children
				confirmAnimator.Play ("ConfirmFadeIn");
				//Set the current wordlist
				PlayerPrefs.SetInt ("CurrentWord", int.Parse (wordID));
		}

		//Show the Edit Panel
		public void showEditPanel (string wordID)
		{
				errorWText.GetComponent<Text> ().color = new Color (0, 0, 0, 0);
				editPanel.SetActive (true);

				txtEWordName.text = loadWordName (int.Parse (wordID));

				//Show the ConfirmPanel and its children
				editAnimator.Play ("EditWordFadeIn");
		
				//Set the current wordlist
				PlayerPrefs.SetInt ("CurrentWord", int.Parse (wordID));
		}

		//Show the Audio Panel
		public void showAudioPanel (string wordID)
		{
				audioPanel.SetActive (true);
		
				//Show the AudioPanel and its children
				audioAnimator.Play ("AudioFadeIn");

				//Set the current wordlist
				PlayerPrefs.SetInt ("CurrentWord", int.Parse (wordID));

				wordAudio = new AudioClip ();
				startedRecording = false;
				if (wordHasAudio ()) {
						playAudioButton.interactable = true;
				} else {
						playAudioButton.interactable = false;
				}
		}

		//Show the AddPanel
		public void showPanel ()
		{
				errorText.GetComponent<Text> ().color = new Color (0, 0, 0, 0);
				addPanel.SetActive (true);
		
				//Show the AddPanel and its children
				addListAnimator.Play ("AddListTitleFadeIn");
		}

		//Hide the AddPanel
		public void hidePanel ()
		{
				//Hide the AddPanel and its children
				errorText.GetComponent<Text> ().color = new Color (0, 0, 0, 0);
				addListAnimator.Play ("AddListTitleFadeOut");
				confirmAnimator.Play ("ConfirmFadeOut");
				editAnimator.Play ("EditWordFadeOut");
				audioAnimator.Play ("AudioFadeOut");
		}

		//Record Audio
		public void recordAudio ()
		{
				recordAudioButton.enabled = false;
				recordAudioButton.image.sprite = recordImg;

				if (startedRecording) {
						startedRecording = false;
						SavWav.Save (PlayerPrefs.GetInt ("CurrentWord").ToString (), wordAudio);
						if (wordHasAudio ()) {
						} else {
								saveWordAudio (PlayerPrefs.GetInt ("CurrentWord").ToString () + ".wav");
						}
						playAudioButton.interactable = true;
				} else {
						startedRecording = true;
						recordAudioButton.image.sprite = stopImg;
						//IF FILE EXISTS THEN DELETE FIRST 
						if (wordHasAudio ()) {
								System.IO.File.Delete (Application.persistentDataPath + "/" + PlayerPrefs.GetInt ("CurrentWord").ToString () + ".wav");
						}
						wordAudio = Microphone.Start (null, false, 2, 44100);
				}
		
				recordAudioButton.enabled = true;
		}

		public void playAudio ()
		{
				if (wordHasAudio ()) {
						StartCoroutine (playClip ());
				}
		}

		IEnumerator playClip ()
		{
				//get audio file	
				WWW AudioToLoadPath = new WWW ("file://" + Application.persistentDataPath + "/" + PlayerPrefs.GetInt ("CurrentWord").ToString () + ".wav");

				yield return AudioToLoadPath;

				if (WebFileExists ("file://" + Application.persistentDataPath + "/" + PlayerPrefs.GetInt ("CurrentWord").ToString () + ".wav")) {
						audio.clip = AudioToLoadPath.GetAudioClip (false);
			
						AudioListener.volume = 1.0f;
						audio.ignoreListenerVolume = true;
						audio.volume = 1.0f;
						audio.Play ();
						Debug.Log ("Playing File");
				}
		}
	
		static public bool WebFileExists (string uri)
		{
				long fileLength = -1;
				WebRequest request = HttpWebRequest.Create (uri);
				request.Method = "HEAD";
				WebResponse resp = null;
				try {
						resp = request.GetResponse ();
				} catch {
						resp = null;
				}
				if (resp != null) {
						long.TryParse (resp.Headers.Get ("Content-Length"), out fileLength);
				}
				return fileLength > 0;
		}

		public void goBack ()
		{
				Application.LoadLevel ("WordList");
		}
		

		//Get all words
		private List<wordItem> getLists ()
		{
				string sql = "SELECT * FROM SM_Words WHERE  wordListID = " + PlayerPrefs.GetInt ("CurrentWordList");
				List<wordItem> wordLists = dbManager.Query<wordItem> (sql);
		
				return wordLists;
		}

		private string loadListName (int listID)
		{
				string listName = "";
				bool recordExists = false;
				string sql = "SELECT * FROM SM_WordList WHERE id = " + listID;
				wordTitle myTitle = dbManager.QueryFirstRecord<wordTitle> (out recordExists, sql);
				if (recordExists) {
						listName = myTitle.title;
				}

				return listName;
		}

		private string loadWordName (int wordID)
		{
				string wordName = "";
				bool recordExists = false;
				string sql = "SELECT word FROM SM_Words WHERE id = " + wordID;
				wordContent myWord = dbManager.QueryFirstRecord<wordContent> (out recordExists, sql);
				if (recordExists) {
						wordName = myWord.word;
				}
		
				return wordName;
		}

		public void updateWord ()
		{
				//Check for length and text
				if (txtEWordName.text.Length < 1) {
						errorWText.GetComponent<Text> ().color = new Color (0.8f, 0.16f, 0.15f, 255);
						Debug.Log ("No Word Entered");
				} else {
						//If all goes well add to DB
						updateWordText ();
						//Hide the Panel
						hidePanel ();
						//Reload the list
						loadList ();
						//Clear text
						txtEWordName.text = "";
				}
		}

		public void deleteSelectedWord ()
		{
				deleteWord ();
				hidePanel ();
				loadList ();
		}

		//Delete the word
		private void deleteWord ()
		{
				string wordsSql = "DELETE FROM SM_Words WHERE id = ?";
				dbManager.Execute (wordsSql, PlayerPrefs.GetInt ("CurrentWord"));

				//IF FILE EXISTS THEN DELETE IT 
				if (wordHasAudio ()) {
						string wordAudioSql = "DELETE FROM SM_WordAudio WHERE wordID = ?";
						dbManager.Execute (wordAudioSql, PlayerPrefs.GetInt ("CurrentWord"));
						System.IO.File.Delete (Application.persistentDataPath + "/" + PlayerPrefs.GetInt ("CurrentWord").ToString () + ".wav");
				}
		}
	
		//Save the word 
		public void saveWord ()
		{
				//Check for length and text
				if (txtWordName.text.Length < 1) {
						errorText.GetComponent<Text> ().color = new Color (0.8f, 0.16f, 0.15f, 255);
						Debug.Log ("No Word Entered");
				} else {
						//If all goes well add to DB
						addWord ();
						//Hide the Panel
						hidePanel ();
						//Reload the list
						loadList ();
						//Clear text
						txtWordName.text = "";
				}
		}

		private bool wordHasAudio ()
		{
				bool hasAudio = false;

				string sql = "SELECT fileName FROM SM_WordAudio WHERE wordID = " + PlayerPrefs.GetInt ("CurrentWord");
				audioContent myAudio = dbManager.QueryFirstRecord<audioContent> (out hasAudio, sql);

				return hasAudio;
		}
	
		private void saveWordAudio (string fileName)
		{
				string sql = "INSERT INTO SM_WordAudio (wordID, fileName) VALUES(?,?) ";
				dbManager.Execute (sql, PlayerPrefs.GetInt ("CurrentWord").ToString (), fileName);
		}

		//Update Word Text
		private void updateWordText ()
		{
				string sql = "UPDATE SM_Words SET word = ? WHERE id = ?";
				dbManager.Execute (sql, txtEWordName.text, PlayerPrefs.GetInt ("CurrentWord"));
		}

		//Add the word
		private void addWord ()
		{
				string sql = "INSERT INTO SM_Words (wordListID, word) VALUES(?,?) ";
				dbManager.Execute (sql, PlayerPrefs.GetInt ("CurrentWordList"), txtWordName.text);
		}
}
