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
		Application.targetFrameRate = -1;
		QualitySettings.vSyncCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
