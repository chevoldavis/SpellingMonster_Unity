using UnityEngine;
using System.Collections;

public class PauseController : MonoBehaviour {
	// Use this for initialization
	public string currentLevel;
	private CanvasGroup cg;


	void Awake (){
		//gameObject.SetActive(false);
	}

	void Start () {
		Messenger.AddListener<string> ("game pause", displayPause);
	}

	private void displayPause(string nothing) {
		Time.timeScale = 0;
		cg = gameObject.GetComponent<CanvasGroup>();
		cg.alpha = 1.0f;
		cg.interactable = true;
		cg.blocksRaycasts = true;
	}

	public void hidePause(){
		Time.timeScale = 1;
		cg = gameObject.GetComponent<CanvasGroup>();
		cg.alpha = 0.0f;
		cg.interactable = false;
		cg.blocksRaycasts = false;

		Messenger.Broadcast<string>("game resume", "");
	}

	public void restart(){
		Time.timeScale = 1;
		Application.LoadLevel(currentLevel);
	}

	public void gotoMainMenu() {
		Application.LoadLevel ("GameSelection");
	}
	
}
