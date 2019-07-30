/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerLobby : NetworkBehaviour
{

    public GameObject player;
    public string playerName;
    private GUIScript canvas;

    // Use this for initialization
    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<GUIScript>();
        if (isLocalPlayer)
        {
            canvas.localPlayer = this;
            playerName = Random.Range(0, 100).ToString();
            CmdSetName(playerName);
            CmdSendInfo(new PlayerInfo(playerName));
            print(playerName);

        }
    }

    [Command]
    void CmdSetName(string name)
    {
        RpcSetName(name);
    }

    [ClientRpc]
    void RpcSetName(string name)
    {
        playerName = name;
    }

    [Command]
    void CmdSendInfo(PlayerInfo newInfo)
    {
        print(newInfo);

        canvas.AddPlayer(newInfo);
    }

    public void ReadyPress(bool buttonActive)
    {
        print(buttonActive);
        CmdReadyPress(buttonActive);
    }

    [Command]
    void CmdReadyPress(bool buttonActive)
    {
        print(buttonActive);
        canvas.ToggleReady(new PlayerInfo(playerName, buttonActive));
    }
}*/