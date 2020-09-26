using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class PlayerProperties : NetworkBehaviour
{
    public GameObject player;
    public GameObject server;

    public float mvmtSpeed;
    public float bulletSpeed;

    public float playerHealth = 1000;

    public float reloadTime = .5f;
    public float damage = 100;
    private float shootTime = 0;
    public int playerID;

    public float damageBonus;
    public float speedBonus;
    public float attackSpeedBonus;
    public float bulletSpeedBonus;
    public float healthKitBonus;

    public float bigDmgBonus;
    public float bigMSpeedBonus;
    public float bigAtkSpeedBonus;
    public float bigBSpeedBonus;
    public float bigHealthKitBonus;



    public Rigidbody rb;
    public Vector3 topleft;
    public Vector3 topright;
    public Vector3 bottomright;
    public Vector3 bottomleft;

    public Transform lookUp;
    public Transform lookDown;
    public Transform lookLeft;
    public Transform lookRight;
    public float turnSpeed;

    public Vector3 purgatory;

    public GameObject playerMinimapDot;
    public GameObject positionMarker;

    public GameObject[] colorPanels;
    public GameObject[] colorHealth;
    public GameObject[] highlightPanels;
    public GameObject[] panels;
    public GameObject[] hpPanel;
    public GameObject[] gameObjectsColor;
    public GameObject[] triggers;
    public Material[] materials;
    public Material chosenMaterial;
    public static List<Material> availableMaterials = new List<Material>();

    public GameObject cyberpodVisuals;

    public Rigidbody clone;
    public Rigidbody bullet;
    public GameObject GunShot;

    RaycastHit hit;
    public LayerMask mask;

    public PowerupType[] currPowerups;

    public float moveSpeed;
    public float ASpeed;
    public float BSpeed;
    public float Damage;
    public float spectatorSpeed;


    public float multiplier = 1;
    public float bigMultiplier = 2;

    public float crashDamageMultiplier;

    public GameObject whitePanelHealth;

    public GameObject floatingHealth;

    public bool[] disableMovement;

    public bool isSpectator;

    private float magicNumber;
    private float magicNumberHealth;
    private float magicNumberWhite;
    private int lastPowerupPressed = 0;
    public bool usingPowerup;
    public PowerupType currentPowerup;
    private Vector3 spot;
    private Vector3 moveDirection;
    private string lastkey;
    private string currkey;
    private Vector3 rotateV;
    private Vector3 tiltV;
    private bool startrotate;
    private Vector3 teleportPosition;
    private bool teleport;
    private int numCollected = 0;
    private float[] powerupPercent;
    private int[] totalPowerups;
    public float hpPercent = 1f;
    private float previousHpPercent = 1f;
    private float previousHpStored = 1f;
    private float[] powerupDuration;
    private bool coroutineRunning = false;
    private Color powerupColor;
    private int pcount;
    private bool sprinting = false;
    public float invokeRepeatRate = 0.5f;
    public GameObject gameCanvas;



    //Transferred from the old PlayerLobby Script
    public string playerName;
    public GUIScript canvas;




   
    void Start()
    {
        
        panels = new GameObject[3];
        DontDestroyOnLoad(gameObject);

        server.GetComponent<reallyimportantplayerIDscript>().AddPlayer();
        playerID = server.GetComponent<reallyimportantplayerIDscript>().playerCount;
        if (isLocalPlayer)
        {

            RoomProperties.localPlayer = gameObject;
            //CmdChooseColor();
        }

        //Transferred from the old PlayerLobby Script
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            canvas = GameObject.Find("MenuCanvas").GetComponent<GUIScript>();
            if (isLocalPlayer)
            {
                print("I MADE IT YES");
                canvas.localPlayer = this;
                playerName = playerID.ToString();
                CmdSetName(playerName);
                CmdSendInfo(new PlayerInfo(playerName));
                print(playerName);

            }
        }

        if (isServer)
        {
            print("Start");
            //InvokeRepeating("CmdPositionCorretions", 0f, invokeRepeatRate);
        }

    }

    public void GameStart()
    {
        positionMarker.transform.parent = null;

        hpPanel = GameObject.FindGameObjectsWithTag("HPPan");
        colorHealth = new GameObject[hpPanel.Length];

        isSpectator = false;


        if (isLocalPlayer)
        {

            panels = GameObject.FindGameObjectsWithTag("Panels");
            print("PANEL LENGTH" + panels.Length);
            magicNumber = panels[0].transform.GetChild(1).GetComponent<RectTransform>().localScale.y;
            magicNumberHealth = hpPanel[0].transform.GetChild(0).GetComponent<RectTransform>().localScale.x;
            magicNumberWhite = hpPanel[0].transform.GetChild(1).GetComponent<RectTransform>().localScale.x;


            colorPanels = new GameObject[panels.Length];
            disableMovement = new bool[4];
            triggers = new GameObject[4];
            colorHealth[0] = hpPanel[0].transform.GetChild(2).gameObject;
            whitePanelHealth = hpPanel[0].transform.GetChild(1).gameObject;
            highlightPanels = new GameObject[panels.Length];
            powerupPercent = new float[panels.Length];
            powerupDuration = new float[panels.Length];
            totalPowerups = new int[(int)PowerupType.Length];
            playerMinimapDot.GetComponent<MeshRenderer>().enabled = true;
            floatingHealth.SetActive(false);

            for (int i = 0; i < panels.Length; i++)
            {
                colorPanels[i] = panels[i].transform.GetChild(2).gameObject;
                highlightPanels[i] = panels[i].transform.GetChild(0).gameObject;

                powerupPercent[i] = 0;
            }

        }

        currPowerups = new PowerupType[panels.Length];
        for (int i = 0; i < panels.Length; i++)
        {
            currPowerups[i] = PowerupType.none;
        }

        if (!isLocalPlayer)
        {
            magicNumberHealth = positionMarker.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale.x;
            magicNumberWhite = positionMarker.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<RectTransform>().localScale.x;

            colorHealth[0] = positionMarker.transform.GetChild(0).GetChild(0).GetChild(2).gameObject;
            whitePanelHealth = positionMarker.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
        }
    }

    /*
        [Server]
        public void SetupRoom(float[] numbers, string name)
        {
            RpcSetupRoom(numbers, name);
        }

        [ClientRpc]
        void RpcSetupRoom(float[] numbers, string name)
        {
            GameObject.Find(name).GetComponent<RoomProperties>().SetupLootRoom(numbers);
        }
        */
    //Transferred from the old PlayerLobby Script

    public void SetPosition(Vector3 playerPosition)
    {
        teleportPosition = playerPosition;
        teleport = true;
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
    public void CmdSendInfo(PlayerInfo newInfo)
    {
        print(newInfo);
        print("canvas " + canvas);
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

    void Update()
    {
        



        if (!isSpectator) Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 35, Camera.main.transform.position.z);

        transform.position = new Vector3(transform.position.x, 5, transform.position.z);
        positionMarker.transform.position = player.transform.position;
        positionMarker.transform.rotation = new Quaternion(0, 0, 0, 0);
        if (SceneManager.GetActiveScene().name == "Game")
        {
            hpPercent = (playerHealth / 1000);
            //HP
            colorHealth[0].transform.localScale = new Vector3(hpPercent * magicNumberHealth, .2f, 1);

            if (hpPercent != previousHpPercent && !coroutineRunning && hpPercent > 0)
            {

                previousHpStored = previousHpPercent;
                StartCoroutine("lowerWhiteBar");
            }

            previousHpPercent = playerHealth / 1000;
        }


        if (isLocalPlayer && SceneManager.GetActiveScene().name == "Game")
        {
            try
            {
                player.layer = LayerMask.NameToLayer("Player");

                for (int i = 0; i < highlightPanels.Length; i++)
                {
                    highlightPanels[i].GetComponent<Image>().enabled = false;
                }

                // Powerups
                if (Input.GetButtonDown("1"))
                {
                    lastPowerupPressed = 0;
                }

                if (Input.GetButtonDown("2"))
                {
                    lastPowerupPressed = 1;
                }

                if (Input.GetButtonDown("3"))
                {
                    lastPowerupPressed = 2;
                }


                if (lastPowerupPressed < currPowerups.Length) currentPowerup = currPowerups[lastPowerupPressed];
                for (int i = 0; i < currPowerups.Length; i++)
                {
                    if (currPowerups[i] == currentPowerup && currentPowerup != PowerupType.none)
                    {
                        highlightPanels[i].GetComponent<Image>().enabled = true;
                    }

                }

                usingPowerup = Input.GetButton("Jump");
                pcount = totalPowerups[(int)currPowerups[lastPowerupPressed]];
                if (usingPowerup)
                {


                    if (pcount == 1)
                    {
                        multiplier = 1f;
                        bigMultiplier = 1.75f;
                    }
                    else if (pcount == 2)
                    {
                        multiplier = 1.75f;
                        bigMultiplier = 2.5f;
                    }
                    else if (pcount == 3)
                    {
                        bigMultiplier = 3.25f;
                    }

                    for (int i = 0; i < panels.Length; i++)
                    {
                        if (currPowerups[i] == currentPowerup)
                        {
                            powerupPercent[i] -= Time.deltaTime / powerupDuration[i];
                        }

                    }


                }

                //reset powerup slot to null if run out of powerup
                for (int i = 0; i < powerupPercent.Length && i < currPowerups.Length; i++)
                {
                    if (powerupPercent[i] <= 0)
                    {
                        currPowerups[i] = PowerupType.none;
                        powerupPercent[i] = 0;
                        if (i == lastPowerupPressed)
                        {
                            currentPowerup = PowerupType.none;
                            usingPowerup = false;
                        }


                    }
                }

                if (Input.GetButtonDown("q"))
                {
                    for (int i = 0; i < colorPanels.Length; i++)
                    {
                        if (i == lastPowerupPressed)
                        {
                            powerupPercent[i] = 0;
                        }

                    }

                }


                for (int i = 0; i < colorPanels.Length; i++)
                {
                    colorPanels[i].transform.localScale = new Vector3(0.008f, powerupPercent[i] * magicNumber, 1);
                }



            }

            catch (Exception x)
            {
                print(x);
            }


            if (teleport)
            {
                rb.isKinematic = true;
                transform.position = teleportPosition;
                teleport = false;
                spot = teleportPosition;
                Camera.main.transform.position = new Vector3(spot.x, Camera.main.transform.position.y, spot.z);
            }

            moveDirection = Vector3.zero;
            tiltV = Vector3.zero;

            moveSpeed = mvmtSpeed;
            Damage = damage;
            ASpeed = reloadTime;
            BSpeed = bulletSpeed;

            sprinting = false;
            if (usingPowerup && currentPowerup == PowerupType.Speed)
            {
                sprinting = true;
                moveSpeed = mvmtSpeed + speedBonus * multiplier;
            }
            else if (usingPowerup && currentPowerup == PowerupType.Damage)
            {
                Damage = damage + damageBonus * multiplier;
            }
            else if (usingPowerup && currentPowerup == PowerupType.ASpeed)
            {
                ASpeed = reloadTime - attackSpeedBonus * multiplier;
            }
            else if (usingPowerup && currentPowerup == PowerupType.BSpeed)
            {
                BSpeed = bulletSpeed + bulletSpeedBonus * multiplier;
            }
            else if (usingPowerup && currentPowerup == PowerupType.healthKit)
            {
                if (playerHealth >= 1000)
                {
                    usingPowerup = false;
                    playerHealth = 1000;

                }
                else
                {
                    CmdChangeHealth(healthKitBonus * multiplier * Time.deltaTime);
                }
            }
            else if (usingPowerup && currentPowerup == PowerupType.bigSpeed)
            {
                sprinting = true;
                moveSpeed = mvmtSpeed + speedBonus * bigMultiplier;
            }
            else if (usingPowerup && currentPowerup == PowerupType.bigDamage)
            {
                Damage = damage + damageBonus * bigMultiplier;
            }
            else if (usingPowerup && currentPowerup == PowerupType.bigASpeed)
            {
                ASpeed = reloadTime - attackSpeedBonus * bigMultiplier;
            }
            else if (usingPowerup && currentPowerup == PowerupType.bigBSpeed)
            {

                BSpeed = bulletSpeed + bulletSpeedBonus * bigMultiplier;
            }

            else if (usingPowerup && currentPowerup == PowerupType.bigHealthKit)
            {
                if (playerHealth >= 1000)
                {
                    usingPowerup = false;
                    playerHealth = 1000;

                }
                else
                {
                    CmdChangeHealth(bigHealthKitBonus * bigMultiplier * Time.deltaTime);
                }

            }





            //movement inputs
            rb.isKinematic = true;
            if (Input.GetButton("w"))
            {
                rb.isKinematic = false;
                moveDirection.z = 1;
                tiltV.x = 20;
            }

            if (Input.GetButtonUp("w"))
            {
                CmdPositionCorretions();
                rb.isKinematic = true;
            }
            if (Input.GetButton("s"))
            {
                rb.isKinematic = false;
                moveDirection.z = -1;
                tiltV.x = -20;
            }
            if (Input.GetButtonUp("s"))
            {
                CmdPositionCorretions();
                rb.isKinematic = true;
            }
            if (Input.GetButton("a"))
            {
                rb.isKinematic = false;
                moveDirection.x = -1;
                tiltV.z = 20;
            }
            if (Input.GetButtonUp("a"))
            {
                CmdPositionCorretions();
                rb.isKinematic = true;
            }
            if (Input.GetButton("d"))
            {
                rb.isKinematic = false;
                moveDirection.x = 1;
                tiltV.z = -20;
            }
            if (Input.GetButtonUp("d"))
            {
                CmdPositionCorretions();
                rb.isKinematic = true;
            }

            if (moveDirection != Vector3.zero)
            {
                moveDirection = moveDirection.normalized * moveSpeed;
            }


            if (Input.GetButtonUp("w") || Input.GetButtonUp("a") || Input.GetButtonUp("s") || Input.GetButtonUp("d"))
            {
                CmdMove(Vector3.zero);
                rb.velocity = Vector3.zero;
                CmdPositionCorretions();
                if (!(Input.GetButton("w") || Input.GetButton("a") || Input.GetButton("s") || Input.GetButton("d")))
                {
                    CmdPositionCorretions();
                    //spot = transform.position;

                }
            }

            if (!(Input.GetButton("w") || Input.GetButton("a") || Input.GetButton("s") || Input.GetButton("d")))
            {
                rb.isKinematic = true;
                //transform.position = spot;
                CmdMove(Vector3.zero);
                rb.velocity = Vector3.zero;
                tiltV.x = 0;
                tiltV.z = 0;

            }


            CmdRotate(new Vector3(transform.InverseTransformDirection(tiltV).x, rotateV.y, transform.InverseTransformDirection(tiltV).z));
            CmdMove(moveDirection);
            rb.velocity = moveDirection;
            //transform.position = new Vector3(transform.position.x, 5, transform.position.z);

            //shoot inputs

            /*   if (Input.GetButtonDown("up") || Input.GetButtonDown("left") || Input.GetButtonDown("down") || Input.GetButtonDown("right"))
               {
                   shootTime = Time.time;
               }*/

            if (Input.GetButton("up"))
            {
                rotateV.y = 0;
                CmdRotate(new Vector3(transform.InverseTransformDirection(tiltV).x, rotateV.y, transform.InverseTransformDirection(tiltV).z));
                if (shootTime + ASpeed <= Time.time)
                {

                    CmdShoot(new Vector3(0, 0, 3f), new Vector3(0, 0, BSpeed), playerID, Damage, ASpeed, rb.velocity, transform.position);
                    shootTime = Time.time;
                }

            }
            else if (Input.GetButton("down"))
            {
                rotateV.y = 180;
                CmdRotate(new Vector3(transform.InverseTransformDirection(tiltV).x, rotateV.y, transform.InverseTransformDirection(tiltV).z));
                if (shootTime + ASpeed <= Time.time)
                {

                    CmdShoot(new Vector3(0, 0, -3f), new Vector3(0, 0, BSpeed), playerID, Damage, ASpeed, rb.velocity, transform.position);
                    shootTime = Time.time;
                }

            }

            else if (Input.GetButton("left"))
            {
                rotateV.y = -90;
                CmdRotate(new Vector3(transform.InverseTransformDirection(tiltV).x, rotateV.y, transform.InverseTransformDirection(tiltV).z));
                if (shootTime + ASpeed <= Time.time)
                {
                    shootTime = Time.time;
                    CmdShoot(new Vector3(-3f, 0, 0), new Vector3(0, 0, BSpeed), playerID, Damage, ASpeed, rb.velocity, transform.position);
                }
            }

            else if (Input.GetButton("right"))
            {
                rotateV.y = 90;
                CmdRotate(new Vector3(transform.InverseTransformDirection(tiltV).x, rotateV.y, transform.InverseTransformDirection(tiltV).z));
                //transform.rotation = Quaternion.Lerp(transform.rotation, lookRight.rotation, turnSpeed);
                if (shootTime + ASpeed <= Time.time)
                {
                    shootTime = Time.time;
                    CmdShoot(new Vector3(3f, 0, 0), new Vector3(0, 0, BSpeed), playerID, Damage, ASpeed, rb.velocity, transform.position);
                }
            }


            if (playerHealth <= 0)
            {
                //CmdKill(player);
                
                CmdSpectatorMode(false);
            }






        }
    }


    // TAKE DMG WHEN HIT A WALL, PROB TAKE OUT LATER
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!sprinting)
            {
                if (collision.gameObject.GetComponent<PlayerProperties>().sprinting)
                {
                    CmdChangeHealth(-collision.gameObject.GetComponent<PlayerProperties>().moveSpeed * crashDamageMultiplier);
                }

            }



        }

        else if (!sprinting)
        {
            //CmdChangeHealth(-50);
        }

    }

    [Command]
    void CmdPositionCorretions()
    {
        print("HP BEFORE: " + playerHealth);
        //RpcPositionCorrections(transform.position, rb.velocity, playerHealth);
    }

    [ClientRpc]
    void RpcPositionCorrections(Vector3 position, Vector3 velocity, float health)
    {
        rb.isKinematic = true;
        //  print("RPC ALKFSJSAL:KFJL:KJ");
        // playerHealth = health;
        // transform.position = position;
        //rb.velocity = velocity;
        // print("HP AFTER: " + playerHealth);
        rb.isKinematic = false;
    }

    public void ChangeHealth(float ChangeAmount)
    {
        playerHealth += ChangeAmount;
    }

    [Command]
    public void CmdChangeHealth(float ChangeAmount)
    {
        RpcChangeHealth(ChangeAmount);
    }

    [ClientRpc]
    public void RpcChangeHealth(float ChangeAmount)
    {
        playerHealth += ChangeAmount;
    }


    IEnumerator lowerWhiteBar()
    {
        coroutineRunning = true;

        yield return new WaitForSeconds(.5f);
        for (int i = Mathf.RoundToInt(previousHpStored * 1000); i >= Mathf.RoundToInt(playerHealth) - 4; i -= 5)
        {
            if (hpPercent <= 0 || SceneManager.GetActiveScene().name != "Game") break;
            whitePanelHealth.transform.localScale = new Vector3(i / 1000f * magicNumberWhite, .2f, 1);
            yield return new WaitForSeconds(0);

        }
        coroutineRunning = false;
    }

    /*public void TakeDamage(float damage)
    {
        playerHealth -= damage;
        print(playerHealth);
    }*/

    //tells the server to run RpcMove
    [Command]
    void CmdMove(Vector3 intended)
    {
        RpcMove(intended);
    }

    [ClientRpc]
    void RpcMove(Vector3 intended)
    {
        //rb.velocity = intended;
    }

    private void OnCollision(Collision collision)
    {
        rb.isKinematic = true;
    }



    [Command]
    void CmdRotate(Vector3 angles)
    {
        RpcRotate(angles);
    }

    [ClientRpc]
    void RpcRotate(Vector3 angles)
    {
        transform.eulerAngles = angles;
    }

    //all clients run movement function

    //tells the server to run RpcShoot if you're allowed to shoot
    [Command]
    void CmdShoot(Vector3 offset, Vector3 bulletV, int playerID, float Damage, float ASpeed, Vector3 Velocity, Vector3 Position)
    {
        RpcShoot(offset, bulletV, playerID, Damage, Velocity, Position);
    }

    //all clients will run shoot function
    [ClientRpc]
    void RpcShoot(Vector3 offset, Vector3 bulletV, int playerID, float Damage, Vector3 Velocity, Vector3 Position)
    {
        if (!isSpectator)
        {
            clone = Instantiate(bullet, Position + offset, transform.rotation) as Rigidbody;
            clone.transform.localScale = new Vector3(Damage / 100, Damage / 100, Damage / 100);
            clone.GetComponent<MeshRenderer>().material = chosenMaterial;
            clone.GetComponent<BulletProperties>().shooterID = playerID;
            clone.GetComponent<BulletProperties>().player = gameObject;
            clone.GetComponent<BulletProperties>().playerProperties = this;
            clone.GetComponent<BulletProperties>().bulletDamage = Damage;
            clone.velocity = new Vector3(transform.TransformDirection(bulletV).x, 0, transform.TransformDirection(bulletV).z) + Velocity / 2;

            GameObject gunShot = Instantiate(GunShot, transform.position, transform.rotation) as GameObject;
            gunShot.transform.parent = transform;
        }
        
    }

    //tells the client to run RpcDestroy player if a player dies
    [Command]
    void CmdKill(GameObject target)
    {
        RpcKill(target);
    }

    [ClientRpc]
    void RpcKill(GameObject target)
    {
        rb.isKinematic = true;
        target.GetComponent<Rigidbody>().isKinematic = true;
        target.transform.position = purgatory;


    }


    /*
    [Command]
    void CmdChooseColor()
    {
        if (availableMaterials.Count == 0)
        {
            availableMaterials.AddRange(materials);
        }
        chosenMaterial = ;
        availableMaterials.Remove(chosenMaterial);

        RpcChooseColor(Array.IndexOf(materials, chosenMaterial));
        
        
    }


    [ClientRpc]
    void RpcChooseColor(int number)
    {
        chosenMaterial = materials[number];
        foreach (var obj in gameObjectsColor)
        {
            obj.GetComponent<MeshRenderer>().material = materials[number];
        }
    }
    */


    public void PowerupCollected(Color coloR, PowerupType type)
    {
        if (isLocalPlayer)
        {
            for (int i = 0; i < panels.Length; i++)
            {
                if (currPowerups[i] == PowerupType.none)
                {
                    currPowerups[i] = type;
                    if (type == PowerupType.bigASpeed || type == PowerupType.bigBSpeed || type == PowerupType.bigSpeed || type == PowerupType.bigDamage || type == PowerupType.bigHealthKit)
                    {
                        powerupDuration[i] = 7;
                    }

                    else
                    {
                        powerupDuration[i] = 5;
                    }
                    colorPanels[i].GetComponent<Image>().color = coloR;

                    for (int j = 0; j < panels.Length; j++)
                    {
                        if (currPowerups[j] == currPowerups[i])
                        {
                            powerupPercent[j] = 1;
                        }
                    }
                    break;
                }
            }

            for (int i = 0; i < totalPowerups.Length; i++)
            {
                totalPowerups[i] = 0;
            }
            for (int i = 0; i < currPowerups.Length; i++)
            {
                totalPowerups[(int)currPowerups[i]]++;
            }

            if (totalPowerups[(int)currPowerups[0]] == 3 && currPowerups[0] != PowerupType.none)
            {
                if (type == PowerupType.ASpeed || type == PowerupType.BSpeed || type == PowerupType.Speed || type == PowerupType.Damage)
                {
                    currPowerups[0]++;
                    powerupPercent[1] = 0;
                    powerupPercent[2] = 0;
                    print(string.Join(",", Array.ConvertAll(currPowerups, x => x.ToString())));
                    powerupDuration[0] = 7;
                }

            }


            print(string.Join(",", Array.ConvertAll(totalPowerups, x => x.ToString())));
        }


    }

    public void SetColor()
    {
        chosenMaterial = materials[UnityEngine.Random.Range(0, materials.Length)];
        foreach (var obj in gameObjectsColor)
        {
            obj.GetComponent<MeshRenderer>().material = chosenMaterial;
        }
    }

    [Command]
    void CmdSpectatorMode(bool Enabled)
    {
        RpcSpectatorMode(Enabled);
    }

    [ClientRpc]
    void RpcSpectatorMode(bool Enabled)
    {
        isSpectator = !Enabled;
        hpPercent = 1;
        player.transform.GetChild(1).gameObject.SetActive(Enabled);

        player.GetComponent<BoxCollider>().enabled = Enabled;
        
        moveSpeed = spectatorSpeed;

        if (!isLocalPlayer) positionMarker.transform.GetChild(0).gameObject.SetActive(Enabled);

        player.transform.GetChild(0).gameObject.SetActive(Enabled);

        if (SceneManager.GetActiveScene().name == "Game")
        {
            whitePanelHealth.transform.localScale = new Vector3(magicNumberWhite, .2f, 1);
            if (isLocalPlayer)
            {
                gameCanvas = GameObject.Find("Canvas");

                foreach (Transform child in gameCanvas.transform)
                {
                    child.gameObject.SetActive(Enabled);
                }
                //gameCanvas.transform.GetChild(4).gameObject.SetActive(true);

                Camera.main.transform.position = new Vector3(transform.position.x, 50, transform.position.z);
            }
        }

  
        

    }
    private void OnLevelWasLoaded(int level)
    {
        CmdSpectatorMode(true);

        if (level == 0 && isLocalPlayer)
        {
            canvas = GameObject.Find("MenuCanvas").GetComponent<GUIScript>();
            canvas.localPlayer = this;
            print(canvas.localPlayer.playerName);
            GameObject[] objects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].name == "MenuCanvas")
                {
                    canvas = objects[i].GetComponent<GUIScript>();
                    break;
                }
            }
            print(canvas);
            canvas.gameObject.SetActive(true);

            //CmdSendInfo(new PlayerInfo(playerName));
        }

        if (level == 1)
        {
           
            for (int i = 0; i < panels.Length; i++)
            {
                currPowerups[i] = PowerupType.none;
            }
            
        }
    }

    
    public void PlayerLeaveLobby()
    {
        CmdPlayerLeaveLobby();
    }

    [Command]
    public void CmdPlayerLeaveLobby()
    {
        canvas.RpcLeaveLobby(playerName);
    }

    private void OnApplicationQuit()
    {
        print("QUIT");

        if (isLocalPlayer && isServer)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
        
    }
    private void OnPlayerDisconnected(NetworkIdentity player)
    {
        print("player: " + player + " disconnected");
    }

}