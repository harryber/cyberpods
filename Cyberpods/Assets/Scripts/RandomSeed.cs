using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSeed : MonoBehaviour {

    public int seed;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
