using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour {
	public static bool mouseDown;
	public float timeMouseDown;
	public Animator popInAnimator;
	public Text txtVersion;  

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
