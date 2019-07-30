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
    int countTime = 0;

    private float number;
    void Start()
    {
        buttonText.text = "READY";
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
        if (isServer) RpcSeed(UnityEngine.Random.Range(Int32.MinValue, Int32.MaxValue));
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
        UnityEngine.Random.InitState(seed);
        number = UnityEngine.Random.value;
        GameObject.Find("RandomSeed").GetComponent<RandomSeed>().seed = seed;
    }


    void Update()
    {
        if (isServer)
        {
            playersConnected = NetworkServer.connections.Count;


        }
        

        if (playersReady == playersConnected && playersConnected > 0)
        {
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
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
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

    private void OnGUI()
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
            GUI.Box(new Rect(Screen.width/2 - 150, 40 * (p+1), 300, 30), playerInfoList[p].name);
        }
        if (countdown) GUI.Label(new Rect(50, Screen.height - 100, 300, 50), countTime.ToString());
        GUI.Label(new Rect(50, Screen.height - 50, 300, 50), number.ToString());
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
