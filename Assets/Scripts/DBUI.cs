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
		
		List<wordList> myLists = getLists ();
		Debug.Log ("WordList Count: " + myLists.Count.ToString ());
		foreach (wordList list in myLists)
		{
			Button buttonClone = (Button)Instantiate (prefabButton,transform.position,transform.rotation);
			buttonClone.transform.parent = listPanel.transform;
			buttonClone.transform.localScale = Vector3.one;
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
				//Debug.Log (btn.name);
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

	//Add the word list title
	private void addListTitle() {
		string sql = "INSERT INTO SM_WordList (title, isActive) VALUES(?,?) ";
		dbManager.Execute (sql, txtListName.text, 1);
	}

	//Delete the word list
	private void deleteWordList(){
		string sql = "DELETE FROM SM_WordList WHERE id = ?";
		dbManager.Execute (sql, PlayerPrefs.GetInt("CurrentWordList"));

		//Also remove any words related to that list
		string wordsSql = "DELETE FROM SM_Words WHERE wordListID = ?";
		dbManager.Execute (wordsSql, PlayerPrefs.GetInt("CurrentWordList"));
	}
	
	//Get all word lists
	private List<wordList> getLists() {
		string sql = "SELECT * FROM SM_WordList";
		List<wordList> wordLists = dbManager.Query<wordList>(sql);

		return wordLists;
	}
}
