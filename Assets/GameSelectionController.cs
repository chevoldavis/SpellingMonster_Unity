using UnityEngine;
using System.Collections;

public class GameSelectionController : MonoBehaviour {

	public void gotoHome()
	{
		Application.LoadLevel ("Home");
	}

	public void loadLetterCatch()
	{
		Application.LoadLevel ("LetterCatch");
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
