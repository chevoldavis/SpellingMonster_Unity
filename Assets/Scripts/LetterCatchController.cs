using UnityEngine;
using System.Collections;

public class LetterCatchController : MonoBehaviour {
	public GameObject startPanel;

	public void goBackToGameSelection()
	{
		Application.LoadLevel ("GameSelection");
	}

	public void pauseGame()
	{
		Messenger.Broadcast<string>("game pause", "");
	}

	public void startGame()
	{
		startPanel.SetActive (false);
		Messenger.Broadcast<string>("game start", "");
	}
}
