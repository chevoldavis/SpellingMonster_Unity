using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleSQL;

public class HomeManager : MonoBehaviour {
	public static bool mouseDown;
	public float timeMouseDown;
	public Animator popInAnimator;
	public Text txtVersion;  
	public SimpleSQLManager dbManager;
	
	public class listDB
	{
		public string id { get; set; }
	}

	private bool launched = false;
	public string versionNum;

	// Use this for initialization
	void Start () {
		launched = false;
		popInAnimator.enabled = false;
		txtVersion.text = "v " + versionNum;
	}
	
	// Update is called once per frame
	void Update () {
		if(mouseDown){
			timeMouseDown += Time.deltaTime;
			if(timeMouseDown>3.0 && !launched){
				Debug.Log ("Launching past Parental Gate");
				launched=true;
				popInAnimator.Play ("PGrollOut");
			}
		}
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
		}else{
			//try and grab from the db
			int curActiveList = getActiveList();
			if(curActiveList > 0){
				activeList = curActiveList;
				PlayerPrefs.SetInt ("ActiveWordList",curActiveList);
			}else{
				Debug.Log ("User has no active lists or no lists to make active");
				MessageCenterController.Instance.displayMessage ("You have no active lists. Tap the Words button to add a list or to make a list active.");
			}
		}

		MessageCenterController.Instance.displayMessage ("Going to games screen");
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
		popInAnimator.enabled = true;
		popInAnimator.Play ("PGrollIn");
	}

	public void OnPointerUp(){
		mouseDown = false;
		timeMouseDown = 0;
		popInAnimator.Play ("PGrollOut");
	}
}
