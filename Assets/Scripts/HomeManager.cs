using UnityEngine;
using System.Collections;

public class HomeManager : MonoBehaviour {
	public static bool mouseDown;
	public float timeMouseDown;
	public Animator popInAnimator;

	private bool launched = false;

	// Use this for initialization
	void Start () {
		launched = false;
		//popInAnimator.Play ("3SecRollOut");
		popInAnimator.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(mouseDown){
			timeMouseDown += Time.deltaTime;
			//Debug.Log ("Held For: " + timeMouseDown.ToString());
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
