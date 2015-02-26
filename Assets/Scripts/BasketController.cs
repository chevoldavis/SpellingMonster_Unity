using UnityEngine;
using System.Collections;

public class BasketController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)){
			transform.position = new Vector3(Input.mousePosition.x-60,transform.position.y,0);
		}

		if (Input.touchCount > 0) {
			Vector2 touchPosition = Input.GetTouch(0).position;
			if(Input.GetTouch(0).phase == TouchPhase.Began) {
				transform.position = new Vector3(touchPosition.x-60,transform.position.y,0);
			}

			if(Input.GetTouch(0).phase == TouchPhase.Moved){
				//Debug.Log(touchPosition.x.ToString ()+"   --- " + touchDeltaPosition.x.ToString ());
				transform.position = new Vector3(touchPosition.x-60,transform.position.y,0);
			}

		}
	}
}
