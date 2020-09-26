using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public struct PlayerInfo
{
    public string name;
    public bool ready;

    public PlayerInfo(string name)
    {
        this.name = name;
        ready = false;
    }

    public PlayerInfo(string name, bool ready)
    {
        this.name = name;
        this.ready = ready;
    }
}

public class GUIScript : NetworkBehaviour {

    public int playersConnected;

    public int playersReady;
    public Button readyButton;
    public Text readyText;
    public Text buttonText;

    public List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
    public PlayerProperties localPlayer;
    private bool buttonActive;
    bool countdown = false;
    public bool allready = false;
    int countTime = 0;

    private float number;
    void Start()
    {
        buttonText.text = "READY";
        DontDestroyOnLoad(gameObject);
    }

    [Server]
    public void ToggleReady(PlayerInfo player)
    {
        print("ToggleReady " + player.ready + ", name " + player.name);
        PlayerInfo[] array = playerInfoList.ToArray();
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].name == player.name)
            {
                array[i].ready = player.ready;
                print(array[i].ToString());
            }
            
        }
        
        RpcSyncPlayerInfo(array);
    }

    [Server]
    public void AddPlayer(PlayerInfo newPlayer)
    {
        print(newPlayer.ToString() + " 2");
        playerInfoList.Add(newPlayer);
        RpcSyncPlayerInfo(playerInfoList.ToArray());
    }

    [ClientRpc]
    void RpcSyncPlayerInfo(PlayerInfo[] array)
    {
        playerInfoList = new List<PlayerInfo>(array);
        playersConnected = array.Length;
        //if (isServer) RpcSeed(UnityEngine.Random.Range(Int32.MinValue, Int32.MaxValue));
        playersReady = 0;
        foreach (var player in playerInfoList)
        {
            if (player.ready) playersReady++;
            print("name: " + player.name + ", ready: " + player.ready);
        }
    }


    [ClientRpc]
    void RpcSeed(int seed)
    {
        GameObject.Find("RandomSeed").GetComponent<RandomSeed>().seed = seed;
        UnityEngine.Random.InitState(seed);
        number = UnityEngine.Random.value;
        
    }


    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            gameObject.GetComponent<Canvas>().enabled = false;
        }

        if (isServer)
        {
            playersConnected = playerInfoList.Count;


        }
        

        if (playersReady == playersConnected && playersReady > 0 && !allready)
        {
            allready = true;
            if (isServer) RpcSeed(UnityEngine.Random.Range(Int32.MinValue, Int32.MaxValue));
            StartCoroutine("ChangeScene", 3);
        }

        readyText.text = playersReady.ToString() + " / " + playersConnected.ToString();
        //if (playersReady == playersConnected)
        if (playersReady == playersConnected)
        {
            
            
        }
    }

    IEnumerator ChangeScene(int waitTime)
    {
        countdown = true;
        for (int i = waitTime; i > 0; i-- )
        {
            countTime = i;
            yield return new WaitForSeconds(1);
        }
        countdown = false;
        if (buttonActive == true) ReadyUp();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerProperties>().playerHealth = 1000;
            player.GetComponent<PlayerProperties>().hpPercent = 1000;
            player.GetComponent<PlayerProperties>().isSpectator = false;
        }
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
        buttonActive = false;
    }

    public void ReadyUp()
    {
        
        if (buttonActive)
        {
            buttonActive = false;
            playersReady--;
            buttonText.text = "READY";
        }
        else
        {
            buttonActive = true;
            playersReady++;
            buttonText.text = "UNREADY";
        }
        print(buttonActive);
        localPlayer.ReadyPress(buttonActive);
        
     
    }

    public void LeaveLobby()
    {
        if (isServer)
        {
            //do something
        }
        else localPlayer.PlayerLeaveLobby();

        NetworkManager.singleton.gameObject.GetComponent<NetDiscovery>().showButton = true;
    }

    [ClientRpc]
    public void RpcLeaveLobby(string name)
    {
        for (int p = 0; p < playerInfoList.Count; p++)
        {
            if (playerInfoList[p].name == name)
            {
                playerInfoList.RemoveAt(p);
                break;
            }
        }
        if (localPlayer.playerName == name) NetworkManager.singleton.StopClient();
    }

    private void OnGUI()
    {
        if (GetComponent<Canvas>().enabled)
        {
            GUIStyle centeredText = new GUIStyle();
            centeredText.alignment = TextAnchor.MiddleCenter;
            for (int p = 0; p < playerInfoList.Count; p++)
            {
                if (playerInfoList[p].ready)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUI.Box(new Rect(Screen.width / 2 - 150, 40 * (p + 1), 300, 30), playerInfoList[p].name);
            }
            if (countdown) GUI.Label(new Rect(50, Screen.height - 100, 300, 50), countTime.ToString());
            GUI.Label(new Rect(50, Screen.height - 50, 300, 50), GameObject.Find("RandomSeed").GetComponent<RandomSeed>().seed.ToString());
        }
        
    }

    public void Quit()
    {
        Application.Quit();
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
