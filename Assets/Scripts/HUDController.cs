using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour {
	public Text currentWord;
	public Text currentLetter;

	// Use this for initialization
	void Start () {
		alignWordsAndLetters();
	}
	
	// Update is called once per frame
	void Update () {
		//alignWordsAndLetters();
	}

	void alignWordsAndLetters(){
		currentWord.rectTransform.position = new Vector3(320, currentWord.rectTransform.position.y ,0.0f);
		float curXPos = currentWord.rectTransform.position.x + (currentWord.rectTransform.sizeDelta.x/2);//currentWord.transform.position.x + (float)(currentWord.rectTransform.sizeDelta.x * .5);
		string word = currentWord.text;
		float offset = 0.0f;
		
		for(int i=0;i<word.Length;i++)
		{
			CharacterInfo testInfo = currentWord.font.characterInfo[i];
			
			offset = offset + testInfo.width;
		}
		currentLetter.rectTransform.position = new Vector3(curXPos - (offset/2), currentWord.rectTransform.position.y, 0.0f);
		//currentLetter.rectTransform.position = new Vector3(curXPos - (offset/2) - ((word.Length/2) * word.Length), currentWord.rectTransform.position.y, 0.0f);
		currentWord.rectTransform.position = new Vector3(currentLetter.rectTransform.position.x, currentLetter.rectTransform.position.y, 0.0f);
	}
}
