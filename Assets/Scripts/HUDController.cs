using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour
{
		public Text currentWord;
		public Text currentLetter;
		public Text currentProgress;
		public GameObject star1;
		public GameObject star2;
		public GameObject star3;
		private int numTries = 3;
		private int wordProgress = 0;
		private bool gameRunning = false;

		// Use this for initialization
		void Start ()
		{
				Messenger.AddListener<string> ("show new word", displayWord);
				Messenger.AddListener<string> ("show new letter", displayLetter);
				Messenger.AddListener<string> ("word progress update", displayProgress);
				Messenger.AddListener<string> ("remove star", removeStar);
				numTries = 3;
				wordProgress = 0;
		}
	
		// Update is called once per frame
		void Update ()
		{
		}

		private void displayProgress (string progress)
		{
				currentProgress.text = progress;
		}

		private void displayWord (string newWord)
		{
				currentWord.text = newWord;
				currentLetter.text = "";
				wordProgress = 0;
				alignWordsAndLetters ();
		}

		private void displayLetter (string newLetter)
		{
		currentLetter.text = currentWord.text.Substring(0,wordProgress + 1);
				alignWordsAndLetters ();
				wordProgress = wordProgress + 1;
		}

		private void removeStar (string nothing)
		{
				switch (numTries) {
				case 1:
						star1.SetActive (false);
						Debug.Log ("GAME OVER - NO MORE TRIES");
						break;
				case 2:
						star2.SetActive (false);
						break;
				case 3:
						star3.SetActive (false);
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
