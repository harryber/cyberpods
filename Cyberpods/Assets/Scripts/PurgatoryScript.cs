using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PurgatoryScript : MonoBehaviour {

    public Vector3 bottomLeft;
    public Vector3 topRight;
    public GameObject canvas;

    private GameObject[] players;
    private int numPlayersDead;
    private Rect boundaries;
    private bool countdown;
    private int countTime = 0;
    // Use this for initialization
    void Start ()
    {
        print("PurgatoryStart");
        players = GameObject.FindGameObjectsWithTag("Player");
        boundaries = new Rect(transform.position.x + bottomLeft.x, transform.position.z + bottomLeft.z, topRight.x - bottomLeft.x, topRight.z - bottomLeft.z);
    }

    // Update is called once per frame
    void Update()
    {
        numPlayersDead = 0;
        foreach (var player in players)
        {
            PlayerProperties playerProperties = player.gameObject.GetComponent<PlayerProperties>();
            print("player" + playerProperties.playerID + " health:" + playerProperties.playerHealth);
            print("player" + playerProperties.playerID + " spec:" + playerProperties.isSpectator);
            if (player.gameObject.GetComponent<PlayerProperties>().isSpectator)
            {
                print("found spectator");
                numPlayersDead++;
            }
        }

        if (players.Length != 1)
        {
            if (numPlayersDead >= players.Length - 1)
            {
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

        print(numPlayersDead);
        canvas = GameObject.Find("MenuCanvas");
        canvas.GetComponent<GUIScript>().allready = false;
        canvas.GetComponent<Canvas>().enabled = true;
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }   

   
}
