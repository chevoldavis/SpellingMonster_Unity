using UnityEngine;
using System.Collections;

public class ParentsController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void gobackHome(){
		Application.LoadLevel ("Home");
	}

	public void loadSettings(){
		Application.LoadLevel("Settings");
	}
}
