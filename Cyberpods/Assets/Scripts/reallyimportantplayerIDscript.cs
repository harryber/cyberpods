using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reallyimportantplayerIDscript : MonoBehaviour {

    public int playerCount = 0;

    public void AddPlayer()
    {
        playerCount++;
        print(playerCount);
    }
}
