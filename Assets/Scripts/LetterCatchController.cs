using UnityEngine;
using System.Collections;

public class LetterCatchController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void goBackToGameSelection()
	{
		Application.LoadLevel ("GameSelection");
	}
}
