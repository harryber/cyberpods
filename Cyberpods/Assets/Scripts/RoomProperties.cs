using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public enum Rooms
{
    loot, cp, obstacle, empty, hq, teleporter, poison, heal
}

public class RoomProperties : MonoBehaviour {


    public Vector3 bottomLeft;
    public Vector3 topRight;
    public GameObject ItemBox;
    public GameObject Chest;
    public GameObject ObstacleBox;
    public GameObject CapturePoint;
    public GameObject Teleporter;
    public GameObject Trap;
    public GameObject HealthPack;
    public GameObject Poison;
    public static GameObject localPlayer;
    public Vector3[] lootSpawnPoints;
    public GameObject doors;

    public Rooms room;

    private bool lerping = false;
    private float number;
    private GameObject[] players;
    private int numPlayersInRoom = 0;
    private Rect boundaries;
    private float time = 0;

    void Start()
    {

        SetupRoom();
        players = GameObject.FindGameObjectsWithTag("Player");
        boundaries = new Rect(transform.position.x + bottomLeft.x, transform.position.z + bottomLeft.z, topRight.x - bottomLeft.x, topRight.z - bottomLeft.z);

    }
    private void Update()
    {
        numPlayersInRoom = 0;
        foreach (var player in players)
        {
            if (boundaries.Contains(new Vector2(player.transform.position.x, player.transform.position.z)))
            {
                if (!player.gameObject.GetComponent<PlayerProperties>().isSpectator)
                {
                    numPlayersInRoom++;
                }
                
            }
        }

        if (numPlayersInRoom >= 2 && time < 1)
        {
            time += 0.02f;
            doors.transform.localPosition = Vector3.Lerp(new Vector3(0, -10.1f, 0), new Vector3(0, 0, 0), time);
        }

        if (numPlayersInRoom < 2 && time > 0)
        {
            time -= 0.02f;
            doors.transform.localPosition = Vector3.Lerp(new Vector3(0, -10.1f, 0), new Vector3(0, 0, 0), time);
        }
    }

    void SetupRoom()
    {
        if (room == Rooms.loot)
        {
            float[] numbers = new float[lootSpawnPoints.Length];
            int numSpawned = 0;
            while (numSpawned == 0)
            {
                for (int i = 0; i < numbers.Length; i++)
                {
                    numbers[i] = Random.Range(0f, 1f);
                    if (numbers[i] > .5f)
                    {
                        numSpawned++;
                    }
                }
            }

            //localPlayer.GetComponent<PlayerProperties>().SetupRoom(numbers, name);
            SetupLootRoom(numbers);

        }
    }

    public void SetupLootRoom(float[] numbers)
    {

        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] >= 0f && numbers[i] < .8f)
            {
                Instantiate(ItemBox, transform.position + lootSpawnPoints[i], Quaternion.identity);
            }

            else if (numbers[i] >= .8f)
            {
                Instantiate(Chest, transform.position + lootSpawnPoints[i], Quaternion.identity);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == localPlayer && !other.GetComponent<PlayerProperties>().isSpectator)
        {
            Vector3 target = new Vector3(transform.position.x, 35, transform.position.z);
            if (Camera.main.transform.position != target && !lerping) StartCoroutine("MoveCamera", target);
        }
    }

    private IEnumerator MoveCamera(Vector3 targetPosition)
    {
        lerping = true;
        Vector3 startPosition = Camera.main.transform.position;
        for (float t = 0; t <= 1.1; t += 0.04f)
        {
            Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return new WaitForSeconds(0.01f);
        }
        lerping = false;
    }


    
}
