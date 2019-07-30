using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomHelper : MonoBehaviour {
    public GameObject hq;
    private int seed;
    private float number;

	// Use this for initialization
	void Awake () {

        try
        {
            seed = GameObject.Find("RandomSeed").GetComponent<RandomSeed>().seed;
        }
        catch (System.Exception x)
        {
            print("PLAY IT FROM THE MENU");
            SceneManager.LoadScene(0);
            return;
        }
        Random.InitState(seed);
        //number = Random.value;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players) player.GetComponent<PlayerProperties>().SetColor();

        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");

        for (int i = 0; i < players.Length; i++)
        {
            RoomProperties r = rooms[Random.Range(0, rooms.Length)].GetComponent<RoomProperties>();
            if (r.room == Rooms.empty)
            {
                r.room = Rooms.hq;
                players[i].GetComponent<PlayerProperties>().SetPosition(r.lootSpawnPoints[0] + r.transform.position);
                players[i].GetComponent<PlayerProperties>().GameStart();
                GameObject hqClone = Instantiate(hq, r.transform.position, Quaternion.identity, r.transform);
                hqClone.transform.GetChild(0).GetComponent<Light>().color = players[i].GetComponent<PlayerProperties>().chosenMaterial.color;
                print(r.lootSpawnPoints[0] + r.transform.position);
            }
            else
            {
                i--;
            }
        }
        
        int numLootRooms = Mathf.RoundToInt(Random.Range(3, 3 + players.Length));

        for (int i = 0; i < numLootRooms; i++)
        {
            RoomProperties r = rooms[Random.Range(0, rooms.Length)].GetComponent<RoomProperties>();
            if (r.room == Rooms.empty)
            {
                r.room = Rooms.loot;
            }
            else
            {
                i--;
            }
        }
        
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(50, Screen.height - 100, 300, 50), seed.ToString());
    }
}
