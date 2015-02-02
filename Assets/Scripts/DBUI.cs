using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using SimpleSQL;

public class DBUI : MonoBehaviour {
	public SimpleSQLManager dbManager;
	public GameObject addPanel;
	public GameObject confirmPanel;
	public InputField txtListName;
	public GameObject listPanel;
	public Button prefabButton;
	public Animator addListAnimator;
	public Animator confirmAnimator;
	public Text errorText;
	
	//Simple WordList class
	public class wordList
	{
		[PrimaryKey]
		public int id { get; set; }
		public string title { get; set; }
		public int isActive { get; set; }
	}

	public class wordContent
	{
		public string id { get; set; }
	}
	
	public class audioContent
	{
		public string fileName { get; set; }
	}
	public class lastDBId
	{
		public string id { get; set; }
	}


	void Awake ()
	{
		//Application.targetFrameRate = 60;
	}

	// Use this for initialization
	void Start () {
		addPanel.SetActive (false);
		confirmPanel.SetActive (false);
		hidePanel ();
		loadList ();
	}

	public void goBack()
	{
		Application.LoadLevel ("WordListManager");
	}

	public void rowTapped(string id){
		//Set the active wordlist
		PlayerPrefs.SetInt("ActiveWordList",int.Parse (id));
		//Disable all wordlists and set this one as active
		deactivateLists ();
		setActiveList(int.Parse (id));
		loadList ();
	}

	private void loadList(){
		//Remove the old Lists
		foreach (Button child in listPanel.GetComponentsInChildren <Button>())
		{
			Destroy (child.gameObject);
		}

		int scrollOffest = 0;
		
		List<wordList> myLists = getLists ();
		Debug.Log ("WordList Count: " + myLists.Count.ToString ());
		int activeListID = PlayerPrefs.GetInt ("ActiveWordList");
		foreach (wordList list in myLists)
		{
			Button buttonClone = (Button)Instantiate (prefabButton,transform.position,transform.rotation);
			buttonClone.transform.parent = listPanel.transform;
			buttonClone.transform.localScale = Vector3.one;
			if(list.id.Equals (activeListID)){
				buttonClone.image.color = Color.green;
			}
			//Change the button text to list title
			Text btnTxt =  buttonClone.GetComponentInChildren<Text> ();
			btnTxt.text = list.title;
			buttonClone.transform.localPosition = new Vector3 (0,scrollOffest,0);
			scrollOffest = scrollOffest-120;
			//Dynamically adjust the scroll container height
			RectTransform scrollRect = listPanel.GetComponent<RectTransform>();
			scrollRect.sizeDelta = new Vector2(640,120*myLists.Count);
			//Get the id for later reference
			string captured = list.id.ToString ();
			buttonClone.onClick.AddListener(() => rowTapped(captured));
			
			Button[] buttons =  buttonClone.GetComponentsInChildren<Button> ();
			foreach (Button btn in buttons) {
				if(btn.name.Equals ("DelButton")){
					btn.onClick.AddListener(() => showConfirmPanel(captured));
				}else if(btn.name.Equals ("EditButton")){
					btn.onClick.AddListener(() => showEdit(captured));
				}
			}
		}
	}

	//Hide the AddPanel
	public void hidePanel(){
		//Hide the AddPanel and its children
		errorText.GetComponent<Text>().color = new Color(0,0,0,0);
		addListAnimator.Play ("AddListTitleFadeOut");
		confirmAnimator.Play ("ConfirmFadeOut");
	}

	//Show the AddPanel
	public void showPanel(){
		errorText.GetComponent<Text>().color = new Color(0,0,0,0);
		addPanel.SetActive (true);

		//Show the AddPanel and its children
		addListAnimator.Play ("AddListTitleFadeIn");
	}

	//Cancel adding title
	public void cancel(){
		//Hide the Panel
		hidePanel ();
		//Reload the list
		loadList ();
	}

	//Show the Confirmation Panel
	public void showConfirmPanel(string listID){
		confirmPanel.SetActive (true);

		//Show the ConfirmPanel and its children
		confirmAnimator.Play ("ConfirmFadeIn");

		//Set the current wordlist
		PlayerPrefs.SetInt("CurrentWordList",int.Parse (listID));
	}

	public void showEdit(string listID){
		//Set the current wordlist
		PlayerPrefs.SetInt("CurrentWordList",int.Parse (listID));
		Application.LoadLevel ("Words");
	}

	//Save the word list title
	public void saveTitle(){
		//Check for length and text
		if (txtListName.text.Length<1) {
			errorText.GetComponent<Text>().color = new Color(0.8f,0.16f,0.15f,255);
			Debug.Log ("No Title Entered");
		}else{
			//If all goes well add to DB
			addListTitle ();
			//Hide the Panel
			hidePanel ();
			//Reload the list
			loadList ();
			//Clear text
			txtListName.text = "";
		}
	}

	public void deleteList(){
		deleteWordList ();
		hidePanel ();
		loadList ();
	}

	private void deactivateLists(){
		string sql = "UPDATE SM_WordList SET isActive = 0 WHERE isActive = 1";
		dbManager.Execute (sql);
	}

	private void setActiveList(int listID){
		string sql = "UPDATE SM_WordList SET isActive = 1 WHERE id = ?";
		dbManager.Execute(sql,listID);
	}

	//Add the word list title
	private void addListTitle() {
		bool lastInsert = false;
		deactivateLists ();
		string sql = "INSERT INTO SM_WordList (title, isActive) VALUES(?,?) ";
		dbManager.Execute (sql, txtListName.text, 1);
		
		string lastRowQuery = "SELECT id from SM_WordList order by id DESC limit 1";
		lastDBId lastWordID = dbManager.QueryFirstRecord<lastDBId> (out lastInsert, lastRowQuery);

		PlayerPrefs.SetInt("ActiveWordList",int.Parse (lastWordID.id));
	}

	//Delete the word list
	private void deleteWordList(){
		string sql = "DELETE FROM SM_WordList WHERE id = ?";
		dbManager.Execute (sql, PlayerPrefs.GetInt("CurrentWordList"));

		string wordSql = "SELECT id FROM SM_Words WHERE wordListID = " + PlayerPrefs.GetInt("CurrentWordList");
		List<wordContent> words = dbManager.Query<wordContent> (wordSql);

		foreach(wordContent myWord in words){
			Debug.Log ("DELETING A WORD");
			//Delete any audio
			if (wordHasAudio ()) {
				string wordAudioSql = "DELETE FROM SM_WordAudio WHERE wordID = ?";
				dbManager.Execute (wordAudioSql, myWord.id);
				System.IO.File.Delete (Application.persistentDataPath + "/" + myWord.id + ".wav");
			}
			//Delete the word
			string delWordsSql = "DELETE FROM SM_Words WHERE id = ?";
			dbManager.Execute (delWordsSql, int.Parse(myWord.id));
		}
	}
	
	//Get all word lists
	private List<wordList> getLists() {
		string sql = "SELECT * FROM SM_WordList";
		List<wordList> wordLists = dbManager.Query<wordList>(sql);

		return wordLists;
	}

	private bool wordHasAudio ()
	{
		bool hasAudio = false;
		
		string sql = "SELECT fileName FROM SM_WordAudio WHERE wordID = " + PlayerPrefs.GetInt ("CurrentWord");
		audioContent myAudio = dbManager.QueryFirstRecord<audioContent> (out hasAudio, sql);
		
		return hasAudio;
	}
}
