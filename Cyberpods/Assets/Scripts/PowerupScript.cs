using UnityEngine;
using System.Collections;

public enum PowerupType
{
    none, Damage, bigDamage, ASpeed, bigASpeed, BSpeed, bigBSpeed, Speed, bigSpeed, healthKit, bigHealthKit, Length
}

public class PowerupScript : MonoBehaviour
{
    public PowerupType type;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
          
            

            for (int i = 0; i < 3; i++)
            {
                print(other.gameObject);
                string ary = "";
                foreach (PowerupType type in other.gameObject.GetComponent<PlayerProperties>().currPowerups) ary += type.ToString() + ",";
                print(ary);
                print("i: " + i);
                print(other.gameObject.GetComponent<PlayerProperties>());
                if (other.gameObject.GetComponent<PlayerProperties>().currPowerups[i] == PowerupType.none)
                {
                    
                    other.gameObject.GetComponent<PlayerProperties>().PowerupCollected(GetComponent<MeshRenderer>().material.color, type);
                    Destroy(gameObject);
                    break;
                }
            }
      
        }

    }
}