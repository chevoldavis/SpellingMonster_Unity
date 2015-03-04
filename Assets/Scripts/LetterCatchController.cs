using UnityEngine;
using System.Collections;

public class LetterCatchController : MonoBehaviour {
	public GameObject startPanel;

	public void goBackToGameSelection()
	{
		Application.LoadLevel ("GameSelection");
	}

	public void startGame()
	{
		startPanel.SetActive (false);
		Messenger.Broadcast<string>("game start", "");
	}
}
