using UnityEngine;
using System.Collections;

public class leafScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.y < 0)
		{
			Destroy(this.gameObject);
		}
	}
}
