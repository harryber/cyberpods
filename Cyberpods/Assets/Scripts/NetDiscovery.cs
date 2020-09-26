using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetDiscovery : NetworkManager
{
    public NetworkClient myClient;
    public bool showButton = true;
    private bool received = false;
    public string broadcast;
    private UdpClient udp;

    public string[] data;
    public bool mrcvd;
    public string address;
    public int port;

    private byte num = 0;
    private IPEndPoint e;
    private UdpClient u;
    private bool messageReceived = false;
    public struct UdpState
    {
        public UdpClient u;
        public IPEndPoint e;

        public UdpState(IPEndPoint e, UdpClient u)
        {
            this.u = u;
            this.e = e;
        }
    }


    private void Start()
    {
        udp = new UdpClient("255.255.255.255", 8888);
        string host = Dns.GetHostName();
        IPAddress[] ips = Dns.GetHostEntry(host).AddressList;
        broadcast = "Cyberpods:";
        foreach (IPAddress ip in ips) if (ip.AddressFamily == AddressFamily.InterNetwork) broadcast += ip.ToString();
        broadcast += ":7777";

        e = new IPEndPoint(IPAddress.Any, 8888); 
        u = new UdpClient(e);
        UdpState s = new UdpState(e, u);
        //u.BeginReceive(new AsyncCallback(ReceiveCallback), s); //<-- Original Code from Nathan, took it out because couldn't connect clients. Now that it's gone Clients can connect (Maybe will cause other problems?).
        u.BeginReceive(ReceiveCallback, s);
    }

    void Update()
    {
        if (messageReceived)
        {
            messageReceived = false;
            //u.BeginReceive(new AsyncCallback(ReceiveCallback), new UdpState(e, u));//<-- Original Code from Nathan, took it out because couldn't connect clients. Now that it's gone Clients can connect (Maybe will cause other problems?).
            u.BeginReceive((ReceiveCallback), new UdpState(e, u));
        }
    }

    void Broadcast()
    {
        byte[] bytes = Encoding.ASCII.GetBytes(broadcast + ":" + (++num));
        udp.Send(bytes, bytes.Length);
        if (SceneManager.GetActiveScene().name == "Game")
        {
            CancelInvoke();
        }
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        mrcvd = true;
        UdpClient u = ((UdpState)(ar.AsyncState)).u;
        IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

        byte[] receiveBytes = u.EndReceive(ar, ref e);
        string receiveString = Encoding.ASCII.GetString(receiveBytes);

        print(receiveString);
        messageReceived = true;

        data = receiveString.Split(':');
        if (data[0] == "Cyberpods")
        {
            address = data[1];
            port = Int32.Parse(data[2]);
            networkAddress = address;
            networkPort = port;
            received = true;

        }
    }


    private void OnGUI()
    {
        if (mrcvd) GUI.Label(RectPercent(.9f, .1f, .6f, .1f), "RecievedCallback");
        try
        {
            GUI.Label(RectPercent(.9f, .1f, .1f, .6f), data[1]);
        }
        catch
        {
            GUI.Label(RectPercent(.9f, .1f, .1f, .6f), "No Data");
        }
       

        if (showButton)
        {
            if (received)
            {
                
                if (GUI.Button(RectPercent(.5f, .1f, .35f), "JOIN LAN GAME"))
                {
                    showButton = false;
                    StartClient();

                }
            }
            else
            {

                if (GUI.Button(RectPercent(.5f, .1f, .35f), "HOST LAN GAME"))
                {
                    showButton = false;
                    NetworkServer.Reset(); //<-- THIS COULD CAUSE BROADCAST DATA PROBLEMS
                    StartHost();
                    InvokeRepeating("Broadcast", 0, 1);
                }
            }
            
        }

        GUI.Label(RectPercent(.1f, .1f, .35f), address);
        GUI.Label(RectPercent(.2f, .1f, .35f), port.ToString());

    }

    private Rect RectPercent(float fromTop, float height, float fromSides)
    {
        float x = Screen.width * fromSides;
        float y = Screen.height * fromTop;
        float w = Screen.width * (1 - 2 * fromSides);
        float h = Screen.height * height;

        return new Rect(x, y, w, h);
    }

    private Rect RectPercent(float fromTop, float height, float fromLeft, float fromRight)
    {
        float x = Screen.width * fromLeft;
        float y = Screen.height * fromTop;
        float w = Screen.width * (1 - (fromLeft + fromRight));
        float h = Screen.height * height;

        return new Rect(x, y, w, h);
    }
}
