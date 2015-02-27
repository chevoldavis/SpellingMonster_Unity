using UnityEngine;
using System.Collections;

public class LoadingController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gotoHome();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void gotoHome()
	{
		Application.LoadLevel ("Home");
	}

}
