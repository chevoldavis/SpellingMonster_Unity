using UnityEngine;
using System.Collections;
using SimpleSQL;

public class WordLIstManagerController : MonoBehaviour {
	
	public SimpleSQLManager dbManager;

	public class listDB
	{
		public string id { get; set; }
	}

	void Start(){
		//PlayerPrefs.DeleteKey ("ActiveWordList");
		//Check for an active list
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
			}
		}
	}

	public void goHome()
	{
		Application.LoadLevel ("Home");
	}

	public void gotoCreatEditList()
	{
		Application.LoadLevel ("WordList");
	}

	public void gotoShareList()
	{
		Application.LoadLevel ("ListServer");
	}

	public void gotoGetList()
	{
		Application.LoadLevel ("ListClient");
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
}
