using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleSQL;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;

public class HUDController : MonoBehaviour
{
		public SimpleSQLManager dbManager;
		public Text currentWord;
		public Text currentLetter;
		public Text currentProgress;
		public GameObject star1;
		public GameObject star2;
		public GameObject star3;
		public GameObject audibleBtn;
		public AudioSource audio;
		private int numTries = 3;
		private int wordProgress = 0;
		private bool gameRunning = false;
	
		public class audioContent
		{
			public string fileName { get; set; }
		}


		// Use this for initialization
		void Start ()
		{
				Messenger.AddListener<string> ("show new word", displayWord);
				Messenger.AddListener<string> ("show new letter", displayLetter);
				Messenger.AddListener<string> ("word progress update", displayProgress);
				Messenger.AddListener<string> ("remove star", removeStar);
				numTries = 3;
				wordProgress = 0;
				PlayerPrefs.SetInt("CurGameNumTries",3);
				if(PlayerPrefs.GetInt("Audible") == 1){
					if(PlayerPrefs.GetInt("ShowWord") == 0){
						Color clearColor = new Color();
						clearColor.a = 0.0f;
						currentWord.color = clearColor;
					}
					audibleBtn.SetActive(true);
				}else{	
					audibleBtn.SetActive(false);
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
		}

		public void playAudio(){
			if(wordHasAudio()){
				StartCoroutine (playClip ());
			}
		}

		IEnumerator playClip ()
		{
			//get audio file	
			WWW AudioToLoadPath = new WWW ("file://" + Application.persistentDataPath + "/" + PlayerPrefs.GetInt ("CurGameWord").ToString () + ".wav");
			
			yield return AudioToLoadPath;
			
			if (WebFileExists ("file://" + Application.persistentDataPath + "/" + PlayerPrefs.GetInt ("CurGameWord").ToString () + ".wav")) {
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

		private bool wordHasAudio ()
		{
			bool hasAudio = false;
		
			string sql = "SELECT fileName FROM SM_WordAudio WHERE wordID = " + PlayerPrefs.GetInt ("CurGameWord");
			audioContent myAudio = dbManager.QueryFirstRecord<audioContent> (out hasAudio, sql);

			return hasAudio;
		}

		private void displayProgress (string progress)
		{
			if(PlayerPrefs.GetInt("Uppercase") == 1){
				currentProgress.text = progress.ToUpper ();
			}else{
				currentProgress.text = progress.ToLower ();
			}
		}

		private void displayWord (string newWord)
		{
			if(PlayerPrefs.GetInt("Uppercase") == 1){
				currentWord.text = newWord.ToUpper();
			}else{
				currentWord.text = newWord.ToLower();
			}
				currentLetter.text = "";
				wordProgress = 0;
				alignWordsAndLetters ();
		}

		private void displayLetter (string newLetter)
		{
			if(PlayerPrefs.GetInt("Uppercase") == 1){
				currentLetter.text = currentWord.text.Substring(0,wordProgress + 1).ToUpper();
			}else{
				currentLetter.text = currentWord.text.Substring(0,wordProgress + 1).ToLower();
			}
			
			alignWordsAndLetters ();
			wordProgress = wordProgress + 1;
		}

		private void removeStar (string nothing)
		{
				switch (numTries) {
				case 1:
						star1.SetActive (false);
						Debug.Log ("GAME OVER - NO MORE TRIES");
						PlayerPrefs.SetInt("CurGameNumTries",0);
						Messenger.Broadcast<int>("game end", 0);
						break;
				case 2:
						star2.SetActive (false);
						PlayerPrefs.SetInt("CurGameNumTries",1);
						break;
				case 3:
						star3.SetActive (false);
						PlayerPrefs.SetInt("CurGameNumTries",2);
						break;
				default:
						break;
				}
				numTries = numTries - 1;
		}

		void alignWordsAndLetters ()
		{
				currentWord.rectTransform.position = new Vector3 (Screen.width / 2, currentWord.rectTransform.position.y, 0.0f);
				float curXPos = currentWord.rectTransform.position.x + (currentWord.rectTransform.sizeDelta.x / 2);//currentWord.transform.position.x + (float)(currentWord.rectTransform.sizeDelta.x * .5);
				//string word = currentWord.text;
				float offset = 0.0f;
				//Debug.Log("---- HERE ---- " + currentWord.font.characterInfo.Length.ToString ());
		
				//for(int i=0;i<word.Length;i++)
				//for(int i=0;i<currentWord.font.characterInfo.Length;i++)
				//{
				//	CharacterInfo testInfo = currentWord.font.characterInfo[i];
				//	offset = offset + testInfo.advance;
				//}
				offset = currentWord.preferredWidth;
				currentLetter.rectTransform.position = new Vector3 (curXPos - (offset / 2), currentWord.rectTransform.position.y, 0.0f);
				//currentLetter.rectTransform.position = new Vector3(curXPos - (offset/2) - ((word.Length/2) * word.Length), currentWord.rectTransform.position.y, 0.0f);
				currentWord.rectTransform.position = new Vector3 (currentLetter.rectTransform.position.x, currentLetter.rectTransform.position.y, 0.0f);
		}

}
