using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using System.Net.Sockets;
//using System.Net;
using LostPolygon.System.Net;
using LostPolygon.System.Net.Sockets;
using System.Text;

public class ListServer : MonoBehaviour
{
    public Text outputTxt;
    private int connectionCount = 0;
    //UdpClient sender;

    int remotePort = 19784;
    int localPort = 19785;

    // Use this for initialization
    void Start()
    {
        launchServer();
    }

    void launchServer()
    {
        outputTxt.text = "Starting Server...\n";
        //bool useNat = !Network.HavePublicAddress();
        //Network.InitializeServer(32, 25000, useNat);
        startBroadcast();
    }

    void startBroadcast()
    {
        //sender = new UdpClient(localPort, AddressFamily.InterNetwork);
        //sender.EnableBroadcast = true;
        //IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, remotePort);
        //sender.Connect(groupEP);

        InvokeRepeating("SendData", 0, 5f);
    }

    void SendData()
    {
        string localIP = getIPAddress();
        string customMessage = "Spelling Monster*" + localIP;

        if (customMessage != "")
        {
            // sender.Send(Encoding.ASCII.GetBytes(customMessage), customMessage.Length);
        }
        Debug.Log("UDP Broadcast - " + customMessage);
    }

    string getIPAddress()
    {
        //IPHostEntry host;
        string localIP = "?";
        //host = Dns.GetHostEntry(Dns.GetHostName());
        //foreach (IPAddress ip in host.AddressList)
        //{
        //    if (ip.AddressFamily == AddressFamily.InterNetwork)
        //    {
        //        localIP = ip.ToString();
        //    }
        //}
        return localIP;
    }

    void OnServerInitialized()
    {
        outputTxt.text = "Server Initialized and waiting for clients...\n";
    }

    //void OnPlayerConnected(NetworkPlayer player) {
    //	connectionCount++;
    //	Debug.Log("Client " + connectionCount + " connected from " + player.ipAddress + ":" + player.port);
    //	outputTxt.text = outputTxt.text + "Client " + connectionCount + " connected from " + player.ipAddress + ":" + player.port + "\n";
    //}

    //void OnPlayerDisconnected(NetworkPlayer player)
    //{
    //    Debug.Log("Clean up after player " + player);

    //    Network.RemoveRPCs(player);
    //    Network.DestroyPlayerObjects(player);
    //    outputTxt.text = outputTxt.text + "Client disconnected from " + player.ipAddress + ":" + player.port + " Client Count is now: " + connectionCount-- + "\n";
    //}

    public void goBack()
    {
        //Network.Disconnect();
        //Application.LoadLevel("WordListManager");
    }
}
