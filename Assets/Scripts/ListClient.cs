using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LostPolygon.System.Net;
using LostPolygon.System.Net.Sockets;
using System.Text;
using System;

public class ListClient : MonoBehaviour
{
    public Text outputTxt;

    private bool hasConnected = false;
    // UdpClient receiver;
    int remotePort = 19784;
    //IPEndPoint receiveIPGroup;

    // Use this for initialization
    void Start()
    {
        StartReceivingIP();
    }

    public void StartReceivingIP()
    {
        //try
        //{
        //    if (receiver == null)
        //    {
        //        receiver = new UdpClient(remotePort);
        //        receiver.BeginReceive(new AsyncCallback(ReceiveData), null);
        //    }
        //}
        //catch (SocketException e)
        //{
        //    Debug.Log(e.Message);
        //}
        //if (hasConnected == false)
        //{
        //    outputTxt.text = "Looking for servers...\n";
        //}
    }

    private void ReceiveData(IAsyncResult result)
    {
        if (hasConnected == false)
        {
            outputTxt.text = "Server found!\n";
            //  receiveIPGroup = new IPEndPoint(IPAddress.Any, remotePort);
            byte[] received;
            //if (receiver != null)
            //{
            //    received = receiver.EndReceive(result, ref receiveIPGroup);
            //}
            //else
            //{
            //    return;
            //}
            //receiver.BeginReceive(new AsyncCallback(ReceiveData), null);
            //string receivedString = Encoding.ASCII.GetString(received);

            //Get the payload
            string[] separators = { "*" };
            //string[] payload = receivedString.Split(separators, StringSplitOptions.None);
            //string serverIP = payload[1];

            //connectToServer(serverIP);
        }
    }

    void connectToServer(string serverIP)
    {
        if (hasConnected == false)
        {
            outputTxt.text = "Connecting to server at " + serverIP + "\n";
            //Network.Connect (serverIP,25000);
        }
    }

    void OnConnectedToServer()
    {
        hasConnected = true;
        Debug.Log("Connected to server");
        outputTxt.text = outputTxt.text + "Connection successful";
    }

    //void OnFailedToConnect(NetworkConnectionError error)
    //{
    //    Debug.Log("Could not connect to server: " + error);
    //    outputTxt.text = outputTxt.text + "Connected to server" + error + "\n";
    //}

    public void goBack()
    {
        //Network.Disconnect();
        //Application.LoadLevel("WordListManager");
    }
}
