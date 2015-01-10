﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LostPolygon.System.Net;
using LostPolygon.System.Net.Sockets;
using System.Text;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; 
using System.Collections.Generic;


public class ListClientServer : MonoBehaviour {

	[System.Serializable]
	public struct listPayload{
		public string listName;
		public List<string> words;
		//public AudioClip[] audibleWords;
		public List<string> audibleWords;
	}


	public Text outputTxt;
	public NetworkView networkView;
	public bool isServer = true;


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
			launchServer ();
		}else{
			StartReceivingIP();
		}
	}

	//----------------------
	//------------------------------ SERVER CODE -----------------------------------
	//----------------------

	void launchServer() {
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

		//DUMMY PAYLOAD   **NEED TO EXTEND DYNAMICALLY
		byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + "/106.wav");
		List<string> testAudio = new List<string>();
		testAudio.Add (Convert.ToBase64String (bytes));

		List<string> testWords = new List<string>();
		testWords.Add ("RED");
		testWords.Add ("BLUE");
		listPayload testPayload = new listPayload{ listName="C Test List",words=testWords,audibleWords=testAudio };

		BinaryFormatter binFormatter = new BinaryFormatter(); // Create Formatter and Stream to process our data
		MemoryStream memStream = new MemoryStream();

		binFormatter.Serialize(memStream, testPayload); // We Serialize our plInfo object using the memStream
		byte[] serializedWLInfo = memStream.ToArray(); // We convert the contents of the stream (which now contains our object) into a byte array.
		
		memStream.Close(); // Close our stream!

		//SEND PAYLOAD
		networkView.RPC ("GetPayload", client, serializedWLInfo);
		Debug.Log ("Sent Payload Via RPC!");
	}

	void OnPlayerDisconnected(NetworkPlayer client) {
		connectionCount--;
		Debug.Log("Clean up after player " + client);
		Network.RemoveRPCs(client);
		Network.DestroyPlayerObjects(client);
		outputTxt.text = outputTxt.text + "Client disconnected from " + client.ipAddress + ":" + client.port + " Client Count is now: "+ connectionCount + "\n";
	}

	public void goBack(){
		Network.Disconnect();
		Application.LoadLevel ("WordListManager");
	}

	
	//----------------------
	//------------------------------ CLIENT CODE -----------------------------------
	//----------------------
	
	public void StartReceivingIP ()
	{
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
			
			connectToServer(serverIP);
		}
	}
	
	void connectToServer(string serverIP){
		if(hasConnected == false){
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
		Debug.Log("Got List - " + wlInfo.listName);
		outputTxt.text = outputTxt.text + "\nAdded new List:\n " + wlInfo.listName;

		//GET WORDS
		foreach (string newWord in wlInfo.words) // Loop through List with foreach.
		{
			outputTxt.text = outputTxt.text + "\nNew word: " + newWord;
			Debug.Log("Got Word - " + newWord);
		}

		//GET AUDIO
		Debug.Log ("-------COUNT OF AUDIBLE WORDS: " + wlInfo.audibleWords.Count.ToString ());
		foreach (string newAudio in wlInfo.audibleWords)
		{
			byte[] bytes = System.Convert.FromBase64String(newAudio);
			//******* FILE NAME SHOULD BE DYNAMIC BASED OFF THE WORD AND WORD ID	
			File.WriteAllBytes (Application.persistentDataPath+"/9999.wav",bytes);
		}
	}
}