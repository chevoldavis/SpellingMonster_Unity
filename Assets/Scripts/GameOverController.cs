using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverController : MonoBehaviour {
	public GameObject Star1;
	public GameObject Star2;
	public GameObject Star3;
	public GameObject passBkg;
	public GameObject failBkg;
	public Text wordsText;
	public Text encouragementText;

	private CanvasGroup cg;
	private string[] promo1 = {"Excellent Job!","Perfect!","Way To Go!","Excellent!","Amazing!","Wow!","Super!","Superb!","Wonderful!","Fantastic!","Tremendous!","Outstanding!","Incredible!","Fabulous!","Remarkable","Impressive","Spectacular"};
	private string[] promo2 = {"Great Job!","Well Done!","Great","Nice Going","Way To Go","Terrific","Sensational","Congratulations","Terrific","Cool","Dynamite","Very Good","Super Job"};
	private string[] promo3 = {"Good Job","Not Bad","Good","Good Work","OK Job","Getting Better"};
	private string[] promo4 = {"Nice Try"};

	// Use this for initialization
	void Start () {
		Messenger.AddListener<int> ("game end", showGameOver);
	}

	private void showGameOver(int numStars) {
		Time.timeScale = 0;
		cg = gameObject.GetComponent<CanvasGroup>();
		cg.alpha = 1.0f;
		cg.interactable = true;
		cg.blocksRaycasts = true;

		switch(numStars)
		{
		case 0:
			Star1.SetActive(false);
			Star2.SetActive(false);
			Star3.SetActive(false);
			passBkg.SetActive(false);
			encouragementText.text = promo4[Random.Range(0,promo4.Length)];
			break;
		case 1:
			Star1.SetActive(false);
			Star3.SetActive(false);
			failBkg.SetActive(false);
			encouragementText.text = promo3[Random.Range(0,promo3.Length)];
			break;
		case 2:
			Star3.SetActive(false);
			failBkg.SetActive(false);
			encouragementText.text = promo2[Random.Range(0,promo2.Length)];
			break;
		case 3:
			failBkg.SetActive(false);
			encouragementText.text = promo1[Random.Range(0,promo1.Length)];
			break;
		}
		//Messenger.Broadcast<string>("game pause", "");
	}
	
	public void hideGameOver(){
		Time.timeScale = 1;
		cg = gameObject.GetComponent<CanvasGroup>();
		cg.alpha = 0.0f;
		cg.interactable = false;
		cg.blocksRaycasts = false;
		
		Messenger.Broadcast<string>("game resume", "");
	}

	public void replay(){
		Application.LoadLevel("LetterCatch");
	}

	public void gotoMainMenu(){
		Application.LoadLevel ("GameSelection");
	}
}
