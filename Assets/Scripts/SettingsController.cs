using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsController : MonoBehaviour {
	public Toggle bkgTog;
	public Toggle fxTog;
	public Toggle uppercaseTog;
	public Toggle audibleTog;
	public Toggle showWordTog;
	public Text gameSpeedTxt;


	// Use this for initialization
	void Start () {
		loadUserSettings();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void loadUserSettings()
	{
		Debug.Log ("Loading user settings");

		//Check for any BKG Music settings, if none add defaults
		if (PlayerPrefs.HasKey("BkgMusic")){
			if(PlayerPrefs.GetInt("BkgMusic") == 1){
				bkgTog.isOn = true;
			}else{
				bkgTog.isOn = false;
			}
		}else{
			PlayerPrefs.SetInt("BkgMusic",1);
			bkgTog.isOn = true;
		}

		//Check for any SoundFX settings, if none add defaults
		if (PlayerPrefs.HasKey("SoundFx")){
			if(PlayerPrefs.GetInt("SoundFx") == 1){
				fxTog.isOn = true;
			}else{
				fxTog.isOn = false;
			}
		}else{
			PlayerPrefs.SetInt("SoundFx",1);
			fxTog.isOn = true;
		}

		//Check for any Uppercase settings, if none add defaults
		if (PlayerPrefs.HasKey("Uppercase")){
			if(PlayerPrefs.GetInt("Uppercase") == 1){
				uppercaseTog.isOn = true;
			}else{
				uppercaseTog.isOn = false;
			}
		}else{
			PlayerPrefs.SetInt("Uppercase",1);
			uppercaseTog.isOn = true;
		}
		
		//Check for any Audible settings, if none add defaults
		if (PlayerPrefs.HasKey("Audible")){
			if(PlayerPrefs.GetInt("Audible") == 1){
				audibleTog.isOn = true;
			}else{
				audibleTog.isOn = false;
			}
		}else{
			PlayerPrefs.SetInt("Audible",1);
			audibleTog.isOn = true;
		}
		
		//Check for any ShowWord settings, if none add defaults
		if (PlayerPrefs.HasKey("ShowWord")){
			if(PlayerPrefs.GetInt("ShowWord") == 1){
				showWordTog.isOn = true;
			}else{
				showWordTog.isOn = false;
			}
		}else{
			PlayerPrefs.SetInt("ShowWord",1);
			showWordTog.isOn = true;
		}
		
		//Check for any GameSpeed settings, if none add defaults
		if (PlayerPrefs.HasKey("GameSpeed")){
			gameSpeedTxt.text = PlayerPrefs.GetInt("GameSpeed").ToString ();
		}else{
			PlayerPrefs.SetInt("GameSpeed",3);
			gameSpeedTxt.text = "3";
		}

	}

	public void goBack()
	{
		Application.LoadLevel ("SettingsMenu");
	}

	public void gameSpeedIncrease(){
		int curSpeed = int.Parse(gameSpeedTxt.text);
		if (curSpeed + 1 <= 3){
			curSpeed = curSpeed + 1;
			PlayerPrefs.SetInt("GameSpeed",curSpeed);
			gameSpeedTxt.text = curSpeed.ToString();
		}
	}

	public void gameSpeedDecrease(){
		int curSpeed = int.Parse(gameSpeedTxt.text);
		if (curSpeed - 1 >= 1){
			curSpeed = curSpeed - 1;
			PlayerPrefs.SetInt("GameSpeed",curSpeed);
			gameSpeedTxt.text = curSpeed.ToString();
		}
	}

	public void BkgMusicToggle(){
		if(bkgTog.isOn){
			PlayerPrefs.SetInt("BkgMusic",1);
		}else{
			PlayerPrefs.SetInt("BkgMusic",0);
		}
	}
	
	public void SoundFXToggle(){
		if(fxTog.isOn){
			PlayerPrefs.SetInt("SoundFx",1);
		}else{
			PlayerPrefs.SetInt("SoundFx",0);
		}
	}
	
	public void UppercaseToggle(){
		if(uppercaseTog.isOn){
			PlayerPrefs.SetInt("Uppercase",1);
		}else{
			PlayerPrefs.SetInt("Uppercase",0);
		}
	}
	
	public void AudibleToggle(){
		if(audibleTog.isOn){
			PlayerPrefs.SetInt("Audible",1);
		}else{
			PlayerPrefs.SetInt("Audible",0);
		}
	}
	
	public void ShowWordsToggle(){
		if(showWordTog.isOn){
			PlayerPrefs.SetInt("ShowWord",1);
		}else{
			PlayerPrefs.SetInt("ShowWord",0);
		}
	}
}
