using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// MESSSAGECENTER USAGE INSTRUCTIONS:
// ADD PREAFB TO SCENE Xpos = 0 Ypos = -450
// CALL MESSAGECENTER FROM SCENE CODE LIKE THIS:
// MessageCenterController.Instance.displayMessage ("Some message to display to the user goes here");


// SIMPLE SINGLETON PATTER SO ONLY 1 INSTANCE IS EVER CREATED
public class MessageCenterController : MonoBehaviour {
	Animator anim;
	private static MessageCenterController instance = null;
	public static MessageCenterController Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
		Debug.Log ("MESSAGE CENTER ACTIVATED");
		anim = GetComponentInParent<Animator>();
		anim.enabled = false;
		gameObject.GetComponentInChildren<Text>().text = "";
	}

	public void displayMessage(string msg){
		Debug.Log ("MESSAGE CENTER - " + msg);
		gameObject.GetComponentInChildren<Text>().text = msg;
		anim.enabled = true;
		anim.Play ("msgSlideDown");
		StartCoroutine(WaitToDismiss(3.0F));
	}
	IEnumerator WaitToDismiss(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		hideMessage();
	}

	void hideMessage(){
		anim.Play ("msgSlideUp");
	}

}
