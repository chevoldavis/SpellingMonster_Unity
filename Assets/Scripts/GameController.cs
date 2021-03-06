﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleSQL;

public class wordPackage
{
	[PrimaryKey]
	public int id { get; set; }
	public string word { get; set; }
	public int wordListID { get; set; }
	public string fileName { get; set; }
}

public static class IListExtensions {
	public static void Shuffle<T>(this IList<T> ts) {
		var count = ts.Count;
		var last = count - 1;
		for (var i = 0; i < last; ++i) {
			var r = UnityEngine.Random.Range(i, count);
			var tmp = ts[i];
			ts[i] = ts[r];
			ts[r] = tmp;
		}
	}
}

public class GameController : MonoBehaviour {
	private SimpleSQLManager dbManager;
	private List<wordPackage> words;
	private int curWordIndex = 0;
	private string curWord = "";
	private int curLetterIndex = 0;
	private char curLetter;

	void Start(){
		Messenger.AddListener<string>("letter selected", letterPlayed);
		Messenger.AddListener<string>("game start", startGame);
	}
	
	void Awake() {
	}

	private List<wordPackage> getWords (int activeID)
	{	
		string sql = "SELECT w.id, w.word, w.wordListID, a.fileName FROM SM_Words w LEFT JOIN SM_WordAudio a ON w.id = a.wordID WHERE w.wordListID = " + activeID;
		List<wordPackage> wordList = dbManager.Query<wordPackage> (sql);

		return wordList;
	}

	private void startGame(string nothing)
	{
		loadWords();
	}

	public void loadWords()
	{
		dbManager = this.gameObject.GetComponentInChildren<SimpleSQLManager>();
		int activeListID = PlayerPrefs.GetInt ("ActiveWordList");
		words = getWords (activeListID);
		words.Shuffle();
		Messenger.Broadcast<string>("show new word", getNextWord());
	}

	public void getNextLetter()
	{
		if(curLetterIndex <= curWord.Length)
		{
			curLetter = curWord[curLetterIndex];
			curLetterIndex = curLetterIndex + 1;
		}
		Messenger.Broadcast<string>("new needed letter", char.ToUpper (curLetter).ToString ());
	}
	
	public string getNextWord()
	{
		string newWord = "";
		if(curWordIndex < words.Count){
			newWord = words[curWordIndex].word;
			curWord = newWord;
			getNextLetter();
			PlayerPrefs.SetInt("CurGameWord", words[curWordIndex].id);
			curWordIndex = curWordIndex + 1;
			Messenger.Broadcast<string>("word progress update", curWordIndex.ToString() + "/" + words.Count);
		}else{
			Debug.Log("**** GAME OVER ****");
			Messenger.Broadcast<int>("game end", PlayerPrefs.GetInt("CurGameNumTries"));
		}
		return newWord;
	}

	private void letterPlayed(string letter)
	{
		//Debug.Log("LOOKING FOR: " + curLetter.ToString () + " PLAYED: " + letter);
		if(letter.ToLower().Equals(curLetter.ToString().ToLower ()))
		{
			//Debug.Log("CORRECT");
			if(curLetterIndex < curWord.Length){
				getNextLetter();
				Messenger.Broadcast<string>("show new letter", letter);
			}else{
				curLetterIndex = 0;
				Messenger.Broadcast<string>("show new word", getNextWord());
			}
		}
		else
		{
			//Debug.Log("INCORRECT");
			//REMOVE STAR
			Messenger.Broadcast<string>("remove star", "");
		}
	}

}
