using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour {

    public GameObject player;
    public PlayerProperties playerProperties;
    public bool[] side = new bool[4];

    private void Start()
    {
        playerProperties = player.GetComponent<PlayerProperties>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerProperties.isLocalPlayer)
        {
            print("Other: " + other);
            //if (other.gameObject.tag == "Player")
            //{
                for (int i = 0; i < side.Length; i++)
                {
                    if (side[i])
                    {
                        playerProperties.disableMovement[i] = true;
                    }
                }
            //}
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerProperties.isLocalPlayer)
        {
            print("Other: " + other);
            //if (other.gameObject.tag == "Player")
            //{
            for (int i = 0; i < side.Length; i++)
                {
                    if (side[i])
                    {
                        playerProperties.disableMovement[i] = false;
                    }
                }
            //}
        }
        
    }

}
