using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AppleController : MonoBehaviour {
	public GameObject applePrefab;
	public int neededLetterFrequency = 0;
	private int spawnCount = 0;
	private string neededLetter = "A";
	private bool gameRunning = false;

	string[] UppercaseAlphabet = new string[26] {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
	string[] LowercaseAlphabet = new string[26] {"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};

	void Awake(){
	}

	// Use this for initialization
	void Start () {
		Messenger.AddListener<string>("new needed letter", setNeededLetter);
		Messenger.AddListener<string>("game start", startDroppingApples);
		Messenger.AddListener<string> ("game pause", pauseDropping);
		Messenger.AddListener<string> ("game resume", startDroppingApples);
		InvokeRepeating("SpawnApple", 1, 3);
	}

	private void setNeededLetter(string newNeededLetter)
	{
		if(PlayerPrefs.GetInt("Uppercase") == 1){
			neededLetter = newNeededLetter.ToUpper();
		}else{
			neededLetter = newNeededLetter.ToLower();
		}

	}

	private void pauseDropping(string nothing)
	{
		gameRunning = false;
	}

	private void startDroppingApples(string nothing)
	{
		gameRunning = true;
	}

	void SpawnApple () {
		if (gameRunning){
			GameObject apple = Instantiate(applePrefab,  new Vector3 (transform.position.x, 400, 0), transform.rotation) as GameObject;
			apple.transform.parent = gameObject.transform;
			apple.transform.position = new Vector3 (Random.Range(30.0f, 640.0f), transform.position.y, 0);
			Text letter = apple.GetComponentInChildren<Text>();
			string randomLetter;
			if(PlayerPrefs.GetInt("Uppercase") == 1){
				randomLetter = UppercaseAlphabet[Random.Range(0, UppercaseAlphabet.Length)];
			}else{
				randomLetter = LowercaseAlphabet[Random.Range(0, LowercaseAlphabet.Length)];
			}

			apple.GetComponent<Rigidbody2D>().gravityScale = 20.0f;

			if(spawnCount >= neededLetterFrequency){
				letter.text = neededLetter;
				spawnCount = 0;
			}else{
				letter.text = randomLetter;
				spawnCount = spawnCount+1;
			}
		}
	}
}
