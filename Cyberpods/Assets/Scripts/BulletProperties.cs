using UnityEngine;
using System.Collections;

public class BulletProperties : MonoBehaviour
{
    public GameObject player;
    public GameObject box;

    public PlayerProperties playerProperties;

    public int shooterID;
    public float bulletDamage;
    public GameObject bullet;

    void Start()
    {
        //Physics.IgnoreCollision(GetComponent<Collider>(), bullet.GetComponent<Collider>());
        playerProperties = player.GetComponent<PlayerProperties>();
    }


    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PlayerProperties>().playerID != shooterID)
            {
                other.GetComponent<PlayerProperties>().TakeDamage(bulletDamage);
                Destroy(gameObject);
            }
            
        }

        else if (other.gameObject.tag == "Box")
        {
            other.gameObject.GetComponent<BoxProperties>().boxHealth -= bulletDamage;
            Destroy(gameObject);
        }

        else if (other.gameObject.tag == "Chest")
        {
            other.gameObject.GetComponent<BoxProperties>().chestHealth -= bulletDamage;
            Destroy(gameObject);
        }

        else if (other.gameObject.tag == "Bullet")
        {
            return;
        }


        else if (other.gameObject.layer == 11)
        {
            return;
        }

        else
        {
            Destroy(gameObject);
        }
        

    }


}