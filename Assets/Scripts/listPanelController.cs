using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using SimpleSQL;

public class listPanelController : MonoBehaviour {
	
	public GameObject listPanel;
	public SimpleSQLManager dbManager;
	public Button prefabButton;


	//Simple Word class
	public class wordListItem
	{
		[PrimaryKey]
		public int id { get; set; }
		
		public string title { get; set; }
		
		public int isActive { get; set; }
	}

	// Use this for initialization
	void Start () {
		PlayerPrefs.SetInt("ShareWordList",0);
		loadList();
	}

	public void rowTapped (string id)
	{
		//Set the current wordlist
		PlayerPrefs.SetInt("ShareWordList",int.Parse (id));
		Debug.Log ("Tapped - " + id);
		//Application.LoadLevel ("Words");
		loadList ();
	}

	private void loadList ()
	{
		//Remove the old Lists
		foreach (Button child in listPanel.GetComponentsInChildren <Button>()) {
			Destroy (child.gameObject);
		}
		
		int scrollOffest = 0;
		
		List<wordListItem> myWords = getLists ();
		Debug.Log ("Word Count: " + myWords.Count.ToString ());
		foreach (wordListItem wordList in myWords) {
			Button buttonClone = (Button)Instantiate (prefabButton, transform.position, transform.rotation);
			buttonClone.transform.parent = listPanel.transform;
			buttonClone.transform.localScale = Vector3.one;
			//Change the button text to list title
			Text btnTxt = buttonClone.GetComponentInChildren<Text> ();
			btnTxt.text = wordList.title;
			buttonClone.transform.localPosition = new Vector3 (0, scrollOffest, 0);
			scrollOffest = scrollOffest - 120;
			//Dynamically adjust the scroll container height
			RectTransform scrollRect = listPanel.GetComponent<RectTransform> ();
			scrollRect.sizeDelta = new Vector2 (640, 10 * myWords.Count);
			//Get the id for later reference
			string captured = wordList.id.ToString ();
			buttonClone.onClick.AddListener (() => rowTapped (captured));
			Image[] rowImgs = buttonClone.GetComponentsInChildren<Image>();
			string curShareId = PlayerPrefs.GetInt("ShareWordList").ToString ();
			foreach (Image addImg in rowImgs){
				//Debug.Log (addImg.name);
				if(addImg.name.Equals ("Indicator")){
					addImg.enabled = false;
					if(captured.Equals(curShareId)){
						addImg.enabled = true;
					}
				}
			}
			
			//Button[] buttons = buttonClone.GetComponentsInChildren<Button> ();
			//foreach (Button btn in buttons) {
			//	if (btn.name.Equals ("DelButton")) {
			//		btn.onClick.AddListener (() => showConfirmPanel (captured));
			//	} else if (btn.name.Equals ("EditButton")) {
			//		btn.onClick.AddListener (() => showEditPanel (captured));
			//	} else if (btn.name.Equals ("AudioButton")) {
			//		btn.onClick.AddListener (() => showAudioPanel (captured));
			//	}
				//Debug.Log (btn.name);
			//}
		}
	}

	//Get all words
	private List<wordListItem> getLists ()
	{
		string sql = "SELECT * FROM SM_WordList ";
		List<wordListItem> wordLists = dbManager.Query<wordListItem> (sql);
		
		return wordLists;
	}

}
