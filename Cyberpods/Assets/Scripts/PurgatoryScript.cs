using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PurgatoryScript : MonoBehaviour {

    public Vector3 bottomLeft;
    public Vector3 topRight;

    private GameObject[] players;
    private int numPlayersInRoom;
    private Rect boundaries;
    private bool countdown;
    private int countTime = 0;
    // Use this for initialization
    void Start ()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        boundaries = new Rect(transform.position.x + bottomLeft.x, transform.position.z + bottomLeft.z, topRight.x - bottomLeft.x, topRight.z - bottomLeft.z);
    }

    // Update is called once per frame
    void Update()
    {
        numPlayersInRoom = 0;
        foreach (var player in players)
        {
            if (boundaries.Contains(new Vector2(player.transform.position.x, player.transform.position.z)))
            {
                numPlayersInRoom++;
            }
        }

        if (players.Length != 1)
        {
            if (numPlayersInRoom >= players.Length - 1)
            {
                print("DEATH");
                StartCoroutine("ChangeScene", 0);
            }
        }
        

        //print(numPlayersInRoom);
    }

    IEnumerator ChangeScene(int waitTime)
    {
        countdown = true;
        for (int i = waitTime; i > 0; i--)
        {
            print(i);
            countTime = i;
            yield return new WaitForSeconds(1);
        }
        countdown = false;
        print("ASFOIJASLFIJAPFJOAGJLEA:KJ");
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
