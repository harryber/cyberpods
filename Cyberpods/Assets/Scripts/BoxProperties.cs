using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class BoxProperties : NetworkBehaviour
{
    public GameObject box;
    public GameObject powerup;
    public GameObject openedChest;

    public GameObject atkSpeedUp;
    public GameObject dmgUp;
    public GameObject bSpeedUp;
    public GameObject mSpeedUp;
    public GameObject health;

    public GameObject bigAtkSpeedUp;
    public GameObject bigDmgUp;
    public GameObject bigBSpeedUp;
    public GameObject bigMSpeedUp;
    public GameObject bigHealth;

    public float boxHealth = 300;
    public float chestHealth = 600;
    private int powerupChoice;


    void Awake()
    {
        //powerupChoice = 2;
        powerupChoice = UnityEngine.Random.Range(0, 5);
    }
        
    void Update()
    {
        if (boxHealth <= 0)
        {
            if (powerupChoice == 0) powerup = Instantiate(atkSpeedUp, transform.position + new Vector3(0, 1, 0), transform.rotation) as GameObject;
            else if (powerupChoice == 1) powerup = Instantiate(dmgUp, transform.position + new Vector3(0, 1, 0), transform.rotation) as GameObject;
            else if (powerupChoice == 2) powerup = Instantiate(bSpeedUp, transform.position + new Vector3(0, 1, 0), transform.rotation) as GameObject;
            else if (powerupChoice == 3) powerup = Instantiate(mSpeedUp, transform.position + new Vector3(0, 1, 0), transform.rotation) as GameObject;
            else if (powerupChoice == 4) powerup = Instantiate(health, transform.position + new Vector3(0, 1, 0), transform.rotation) as GameObject;
            Destroy(box);
        }

        if (chestHealth <= 0)
        {
            openedChest = Instantiate(openedChest, transform.position, transform.rotation) as GameObject;
            if (powerupChoice == 0) powerup = Instantiate(bigAtkSpeedUp, transform.position + new Vector3(4.5f, 1, -2.5f), transform.rotation) as GameObject;
            else if (powerupChoice == 1) powerup = Instantiate(bigDmgUp, transform.position + new Vector3(4.5f, 1, -2.5f), transform.rotation) as GameObject;
            else if (powerupChoice == 2) powerup = Instantiate(bigBSpeedUp, transform.position + new Vector3(4.5f, 1, -2.5f), transform.rotation) as GameObject;
            else if (powerupChoice == 3) powerup = Instantiate(bigMSpeedUp, transform.position + new Vector3(4.5f, 1, -2.5f), transform.rotation) as GameObject;
            else if (powerupChoice == 4) powerup = Instantiate(bigHealth, transform.position + new Vector3(4.5f, 1, -2.5f), transform.rotation) as GameObject;
            Destroy(box);
        }
        
    }

    [Command]
    void CmdChoosePwr(float powerupChoice)
    {
        RpcChoosePwr(powerupChoice);
    }

    [ClientRpc]
    void RpcChoosePwr(float powerupChoice)
    {
        
    }


}
