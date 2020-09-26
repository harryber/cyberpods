using UnityEngine.Networking;
using UnityEngine;

public class TestPlayerClient : NetworkDiscovery
{

    public bool hostDiscovered
    {
        get;
        private set;
    }

    void Start()
    {
        Initialize();
        StartAsClient();
        Debug.Log("trying client");
        hostDiscovered = false;
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log("broadcast received, starting client");
        StopBroadcast();
        NetworkManager.singleton.StartClient();
        hostDiscovered = true;
    }

}