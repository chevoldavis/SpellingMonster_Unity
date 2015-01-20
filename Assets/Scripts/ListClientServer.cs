using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.UI;
using LostPolygon.System.Net;
using LostPolygon.System.Net.Sockets;
using System.Text;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; 
using System.Collections.Generic;
using SimpleSQL;


public class ListClientServer : MonoBehaviour {
	[System.Serializable]
	public struct wordPackage
	{
		public string word {get; set;}
		public string audio {get; set;}
	}

	[System.Serializable]
	public struct listPayload{
		public string listName;
		public List<wordPackage> wordsPackage;
	}


	public Text outputTxt;
	public NetworkView networkView;
	public bool isServer = true;
	public GameObject instPanel;
	public Animator instAnimator;
	public GameObject listSelectPanel;
	public Animator listSelAnimator;
	public SimpleSQLManager dbManager;

	public class wordListTitle
	{
		public string title { get; set; }
	}
	
	public class wordContent
	{
		public int id { get; set; }
		public string word { get; set; }
	}

	public class audioContent
	{
		public string fileName { get; set; }
	}

	public class lastDBId
	{
		public string id { get; set; }
	}

	private int connectionCount = 0;
	UdpClient sender;
	int remotePort = 19784;
	int localPort = 19785;
	private bool hasConnected = false;
	UdpClient receiver;
	IPEndPoint receiveIPGroup;



	// Use this for initialization
	void Start () {
		if(isServer){
			listSelectPanel.SetActive(false);
		}
		showPanel ();
	}

	public void showPanel(){
		if(isServer){
			instAnimator.Play ("SInstPanelFadeIn");
		}else{
			instAnimator.Play ("CInstPanelFadeIn");
		}
	}

	//----------------------
	//------------------------------ SERVER CODE -----------------------------------
	//----------------------

	public void launchServer() {
		hidePanel ();
		outputTxt.text = "Starting Server...\n";
		bool useNat = !Network.HavePublicAddress();
		Network.InitializeServer(32, 25000, useNat);
		startBroadcast ();
	}

	void startBroadcast(){
		sender = new UdpClient(localPort, AddressFamily.InterNetwork);
		sender.EnableBroadcast = true;
		IPEndPoint groupEP = new IPEndPoint (IPAddress.Broadcast, remotePort);
		sender.Connect (groupEP);

		InvokeRepeating("SendData",0,5f);
	}

	void SendData ()
	{
		string localIP = getIPAddress();
		string customMessage = "Spelling Monster*" + localIP;
		
		if (customMessage != "") {
			sender.Send (Encoding.ASCII.GetBytes (customMessage), customMessage.Length);
		}
		Debug.Log ("UDP Broadcast - " + customMessage);
	}

	string getIPAddress(){
		IPHostEntry host;
		string localIP = "?";
		localIP = Network.player.ipAddress.ToString ();
		return localIP;
	}

	void OnServerInitialized() {
		outputTxt.text = "Server Initialized and waiting for clients...\n";
	}

	void OnPlayerConnected(NetworkPlayer client) {
		connectionCount++;
		Debug.Log("Client " + connectionCount + " connected from " + client.ipAddress + ":" + client.port);
		outputTxt.text = outputTxt.text + "Client " + connectionCount + " connected from " + client.ipAddress + ":" + client.port + "\n";

		//WORDLIST NAME
		string listname = getListName ();

		//AUDIO
		List<wordPackage> myWordPkg = new List<wordPackage>();


		List<wordContent> myWords = getWords ();
		foreach (wordContent word in myWords) {
			wordPackage newPkg = new wordPackage();
			newPkg.word = word.word;
			newPkg.audio = "";
			//Add audio
			if(wordHasAudio (word.id.ToString ())){
				byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + "/" + word.id + ".wav");
				newPkg.audio = Convert.ToBase64String (bytes);
			}
			myWordPkg.Add(newPkg);
		}

		listPayload testPayload = new listPayload{ listName=listname,wordsPackage=myWordPkg };

		BinaryFormatter binFormatter = new BinaryFormatter(); // Create Formatter and Stream to process our data
		MemoryStream memStream = new MemoryStream();

		binFormatter.Serialize(memStream, testPayload); // We Serialize our plInfo object using the memStream
		byte[] serializedWLInfo = memStream.ToArray(); // We convert the contents of the stream (which now contains our object) into a byte array.
		
		memStream.Close(); // Close our stream!

		//SEND PAYLOAD
		networkView.RPC ("GetPayload", client, serializedWLInfo);
		Debug.Log ("Sent Payload Via RPC!");
	}

	private string getListName(){
		string curShareId = PlayerPrefs.GetInt("ShareWordList").ToString ();
		string listName = "";
		bool recordExists = false;
		string sql = "SELECT * FROM SM_WordList WHERE id = " + curShareId;
		wordListTitle myTitle = dbManager.QueryFirstRecord<wordListTitle> (out recordExists, sql);
		if (recordExists) {
			listName = myTitle.title;
		}
		
		return listName;
	}

	private List<wordContent> getWords ()
	{
		string curShareId = PlayerPrefs.GetInt("ShareWordList").ToString ();
		string sql = "SELECT id, word FROM SM_Words WHERE  wordListID = " + curShareId;
		List<wordContent> words = dbManager.Query<wordContent> (sql);
		
		return words;
	}

	private bool wordHasAudio (string wordID)
	{
		bool hasAudio = false;
		string sql = "SELECT fileName FROM SM_WordAudio WHERE wordID = " + wordID;
		audioContent myAudio = dbManager.QueryFirstRecord<audioContent> (out hasAudio, sql);
		
		return hasAudio;
	}


	void OnPlayerDisconnected(NetworkPlayer client) {
		connectionCount--;
		Debug.Log("Clean up after player " + client);
		Network.RemoveRPCs(client);
		Network.DestroyPlayerObjects(client);
		outputTxt.text = outputTxt.text + "Client disconnected from " + client.ipAddress + ":" + client.port + " Client Count is now: "+ connectionCount + "\n";
	}

	public void showSelPanel(){
		if(isServer){
			listSelectPanel.SetActive(true);
			instAnimator.Play ("SInstPanelFadeOut");
			listSelAnimator.Play ("ShareListSelFadeIn");
		}
	}

	public void goBack(){
		Network.Disconnect();
		Application.LoadLevel ("WordListManager");
	}

	//Hide the AddPanel
	public void hidePanel ()
	{
		if(isServer){
			instAnimator.Play ("SInstPanelFadeOut");
			listSelAnimator.Play ("ShareListSelFadeOut");
		}else{
			instAnimator.Play ("CInstPanelFadeOut");
		}
	}
	
	//----------------------
	//------------------------------ CLIENT CODE -----------------------------------
	//----------------------
	
	public void StartReceivingIP ()
	{
		hidePanel ();
		try {
			if (receiver == null) {
				receiver = new UdpClient (remotePort);
				receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
			}
		} catch (SocketException e) {
			Debug.Log (e.Message);
		}
		if (hasConnected == false){
			outputTxt.text = "Looking for servers...\n";
		}
	}
	
	private void ReceiveData (IAsyncResult result)
	{
		if (hasConnected == false){
			outputTxt.text = "Server found!\n";
			receiveIPGroup = new IPEndPoint (IPAddress.Any, remotePort);
			byte[] received;
			if (receiver != null) {
				received = receiver.EndReceive (result, ref receiveIPGroup);
			} else {
				return;
			}
			receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
			string receivedString = Encoding.ASCII.GetString (received);
			
			//Get the payload
			string[] separators = {"*"};
			string[] payload = receivedString.Split (separators,StringSplitOptions.None);
			string serverIP = payload[1];
			
			outputTxt.text = "Connecting to server at " + serverIP + "\n";
			Network.Connect (serverIP,25000);
		}
	}
	
	void OnConnectedToServer() {
		hasConnected  = true;
		if(!hasConnected){
			Debug.Log("Connected to server");
			outputTxt.text = outputTxt.text + "Connection successful\n";
		}
	}
	
	void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("Could not connect to server: " + error);
		outputTxt.text = outputTxt.text + "Could not connect to server" + error + "\n";
	}
	
	
	[RPC]    
	void GetPayload(byte[] serializedWLInfo)
	{
		BinaryFormatter binFormatter = new BinaryFormatter(); // Create Formatter and Stream to process our data
		MemoryStream memStream = new MemoryStream();
		
		/* This line will write the byte data we received into the stream The second parameter specifies the offset, since we want to start at the beginning of the stream we set this to 0.
    	* The third   parameter specifies the maximum number of bytes to be written into the stream, so we use the amount of bytes that our data contains by passing the length of our byte array. */
		memStream.Write(serializedWLInfo,0,serializedWLInfo.Length); 
		
		/* After writing our data, our streams internal "reader" is now at the last position of the stream. We shift it back to the beginning of our stream so we can start reading from the very    
     	*  beginning */
		memStream.Seek(0, SeekOrigin.Begin); 
		
		listPayload wlInfo = (listPayload)binFormatter.Deserialize(memStream); // Deserialize our data and Cast it into a PlayerInfo object

		//GET LIST NAME
		outputTxt.text = outputTxt.text + "\nDownloading List\n";
		
		//INSERT LIST NAME
		bool lastInsert = false;
		string lastRowQuery = "SELECT id from SM_WordList order by id DESC limit 1";//"last_insert_rowid()";
		string sql = "INSERT INTO SM_WordList (title, isActive) VALUES(?,1) ";
		dbManager.Execute (sql, wlInfo.listName);
		lastDBId lastID = dbManager.QueryFirstRecord<lastDBId> (out lastInsert, lastRowQuery);

		//GET WORD PACKAGE
		foreach (wordPackage newPkg in wlInfo.wordsPackage)
		{
			string wordSql = "INSERT INTO SM_Words (wordListID, word) VALUES(?,?) ";
			dbManager.Execute (wordSql, int.Parse ( lastID.id ), newPkg.word);

			lastRowQuery = "SELECT id from SM_Words order by id DESC limit 1";
			lastDBId lastWordID = dbManager.QueryFirstRecord<lastDBId> (out lastInsert, lastRowQuery);
			
			if(!newPkg.audio.Equals (""))
			{
				byte[] bytes = System.Convert.FromBase64String(newPkg.audio);
				File.WriteAllBytes (Application.persistentDataPath+"/" + lastWordID.id + ".wav",bytes);
				string wordAudioSql = "INSERT INTO SM_WordAudio (wordID, fileName) VALUES(?,?) ";
				dbManager.Execute (wordAudioSql, lastWordID.id, lastWordID.id + ".wav");
			}
		}
		
		Debug.Log("Got List - " + wlInfo.listName);
		outputTxt.text = outputTxt.text + "\nAdded new List:\n" + wlInfo.listName;
		Network.Disconnect();
	}
}
