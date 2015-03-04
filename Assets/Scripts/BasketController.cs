using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BasketController : MonoBehaviour {
	private bool gameRunning = false;

	// Use this for initialization
	void Start () {
		Messenger.AddListener<string>("game start", allowUpdate);
	
	}

	private void allowUpdate(string nothing)
	{
		gameRunning = true;
	}

	// Update is called once per frame
	void Update () {
		if (gameRunning){
			if(Input.GetMouseButton(0)){
				if(Input.mousePosition.y <= transform.position.y + 300){
					transform.position = new Vector3(Input.mousePosition.x-60,transform.position.y,0);
				}
			}

			if (Input.touchCount > 0) {
				Vector2 touchPosition = Input.GetTouch(0).position;
				if(Input.GetTouch(0).phase == TouchPhase.Began) {
					if(Input.mousePosition.y <= transform.position.y + 300){
						transform.position = new Vector3(touchPosition.x-60,transform.position.y,0);
					}
				}

				if(Input.GetTouch(0).phase == TouchPhase.Moved){
					if(Input.mousePosition.y <= transform.position.y + 300){
						transform.position = new Vector3(touchPosition.x-60,transform.position.y,0);
					}
				}
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if(other.gameObject.tag.Equals("Apple")){
		string letterCaught = "";
		letterCaught = other.gameObject.GetComponentInChildren<Text>().text;
		Debug.Log("Caught: " + letterCaught);
		Messenger.Broadcast<string>("letter selected", letterCaught);
		Destroy(other.gameObject);
		}
	}
}
