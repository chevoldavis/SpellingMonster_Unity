using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using SimpleSQL;

public class WordDBUI : MonoBehaviour {
	public SimpleSQLManager dbManager;
	public GameObject addPanel;
	public GameObject confirmPanel;
	public GameObject editPanel;
	public InputField txtWordName;
	public InputField txtEWordName;
	public GameObject listPanel;
	public Button prefabButton;
	public Animator addListAnimator;
	public Animator confirmAnimator;
	public Animator editAnimator;
	public Text errorText;
	public Text errorWText;
	public Text listTitle;
	
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
		public string title {get; set;}
	}
	
	public class wordContent
	{
		public string word {get; set;}
	}

	//Cancel adding title
	public void cancel(){
		//Hide the Panel
		hidePanel ();
		
		//Reload the list
		loadList ();
	}

	// Use this for initialization
	void Start () {
		//Load the wordlist
		Debug.Log ("Loading List: " + PlayerPrefs.GetInt ("CurrentWordList") );
		//Load list name
		listTitle.text = loadListName(PlayerPrefs.GetInt ("CurrentWordList"));
		addPanel.SetActive (false);
		confirmPanel.SetActive (false);
		editPanel.SetActive(false);
		hidePanel ();
		loadList ();
	}
	
	public void rowTapped(string id){
		//Set the current wordlist
		//PlayerPrefs.SetInt("CurrentWordList",int.Parse (id));
		Debug.Log ("Tapped - "+ id);
		//Application.LoadLevel ("Words");
	}
	
	private void loadList(){
		//Remove the old Lists
		foreach (Button child in listPanel.GetComponentsInChildren <Button>())
		{
			Destroy (child.gameObject);
		}
		
		int scrollOffest = 0;
		
		List<wordItem> myWords = getLists ();
		Debug.Log ("Word Count: " + myWords.Count.ToString ());
		foreach (wordItem word in myWords)
		{
			Button buttonClone = (Button)Instantiate (prefabButton,transform.position,transform.rotation);
			buttonClone.transform.parent = listPanel.transform;
			buttonClone.transform.localScale = Vector3.one;
			//Change the button text to list title
			Text btnTxt =  buttonClone.GetComponentInChildren<Text> ();
			btnTxt.text = word.word;
			buttonClone.transform.localPosition = new Vector3 (0,scrollOffest,0);
			scrollOffest = scrollOffest-120;
			//Dynamically adjust the scroll container height
			RectTransform scrollRect = listPanel.GetComponent<RectTransform>();
			scrollRect.sizeDelta = new Vector2(640,120*myWords.Count);
			//Get the id for later reference
			string captured = word.id.ToString ();
			buttonClone.onClick.AddListener(() => rowTapped(captured));
			
			Button[] buttons =  buttonClone.GetComponentsInChildren<Button> ();
			foreach (Button btn in buttons) {
				if(btn.name.Equals ("DelButton")){
					btn.onClick.AddListener(() => showConfirmPanel(captured));
				}else if(btn.name.Equals ("EditButton")){
					btn.onClick.AddListener(() => showEditPanel(captured));
				}else if(btn.name.Equals ("AudioButton")){
					
				}
				//Debug.Log (btn.name);
			}
		}
	}
	//Show the Confirmation Panel
	public void showConfirmPanel(string wordID){
		confirmPanel.SetActive (true);
		
		//Show the ConfirmPanel and its children
		confirmAnimator.Play ("ConfirmFadeIn");
		
		//Set the current wordlist
		PlayerPrefs.SetInt("CurrentWord",int.Parse (wordID));
	}

	//Show the Edit Panel
	public void showEditPanel(string wordID){
		errorWText.GetComponent<Text>().color = new Color(0,0,0,0);
		editPanel.SetActive (true);

		txtEWordName.text = loadWordName(int.Parse (wordID));

		//Show the ConfirmPanel and its children
		editAnimator.Play ("EditWordFadeIn");
		
		//Set the current wordlist
		PlayerPrefs.SetInt("CurrentWord",int.Parse (wordID));
	}

	//Show the AddPanel
	public void showPanel(){
		errorText.GetComponent<Text>().color = new Color(0,0,0,0);
		addPanel.SetActive (true);
		
		//Show the AddPanel and its children
		addListAnimator.Play ("AddListTitleFadeIn");
	}

	//Hide the AddPanel
	public void hidePanel(){
		//Hide the AddPanel and its children
		errorText.GetComponent<Text>().color = new Color(0,0,0,0);
		addListAnimator.Play ("AddListTitleFadeOut");
		confirmAnimator.Play ("ConfirmFadeOut");
		editAnimator.Play ("EditWordFadeOut");
	}

	public void goBack(){
		Application.LoadLevel ("WordList");
	}

	//Get all words
	private List<wordItem> getLists() {
		string sql = "SELECT * FROM SM_Words WHERE  wordListID = " +  PlayerPrefs.GetInt("CurrentWordList");
		List<wordItem> wordLists = dbManager.Query<wordItem>(sql);
		
		return wordLists;
	}

	private string loadListName(int listID){
		string listName = "";
		bool recordExists = false;
		string sql = "SELECT * FROM SM_WordList WHERE id = " + listID;
		wordTitle myTitle = dbManager.QueryFirstRecord<wordTitle>(out recordExists, sql);
		if(recordExists){
			listName = myTitle.title;
		}

		return listName;
	}

	private string loadWordName(int wordID){
		string wordName = "";
		bool recordExists = false;
		string sql = "SELECT word FROM SM_Words WHERE id = " + wordID;
		wordContent myWord = dbManager.QueryFirstRecord<wordContent>(out recordExists, sql);
		if(recordExists){
			wordName = myWord.word;
		}
		
		return wordName;
	}

	public void updateWord(){
		//Check for length and text
		if (txtEWordName.text.Length<1) {
			errorWText.GetComponent<Text>().color = new Color(0.8f,0.16f,0.15f,255);
			Debug.Log ("No Word Entered");
		}else{
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

	public void deleteSelectedWord(){
		deleteWord ();
		hidePanel ();
		loadList ();
	}

	//Delete the word
	private void deleteWord(){
		string wordsSql = "DELETE FROM SM_Words WHERE id = ?";
		dbManager.Execute (wordsSql, PlayerPrefs.GetInt("CurrentWord"));
	}

	//Save the word 
	public void saveWord(){
		//Check for length and text
		if (txtWordName.text.Length<1) {
			errorText.GetComponent<Text>().color = new Color(0.8f,0.16f,0.15f,255);
			Debug.Log ("No Word Entered");
		}else{
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

	//Update Word Text
	private void updateWordText(){
		string sql = "UPDATE SM_Words SET word = ? WHERE id = ?";
		dbManager.Execute (sql,  txtEWordName.text, PlayerPrefs.GetInt("CurrentWord"));
	}

	//Add the word
	private void addWord() {
		string sql = "INSERT INTO SM_Words (wordListID, word) VALUES(?,?) ";
		dbManager.Execute (sql, PlayerPrefs.GetInt("CurrentWordList"), txtWordName.text);
	}
}
