using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AppleController : MonoBehaviour {
	public GameObject applePrefab;

	string[] Alphabet = new string[26] {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};

	// Use this for initialization
	void Start () {
		InvokeRepeating("SpawnApple", 1, 3);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void SpawnApple () {
		GameObject apple = Instantiate(applePrefab,  new Vector3 (transform.position.x, 400, 0), transform.rotation) as GameObject;
		apple.transform.parent = gameObject.transform;
		apple.transform.position = new Vector3 (Random.Range(30.0f, 640.0f), transform.position.y, 0);
		Text letter = apple.GetComponentInChildren<Text>();
		string randomLetter = Alphabet[Random.Range(0, Alphabet.Length)];
		apple.rigidbody2D.gravityScale = 20.0f;
		Debug.Log(randomLetter);
		letter.text = randomLetter;
	}
}
