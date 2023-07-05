using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ManateeHeavenGameController : MonoBehaviour
{
    [SerializeField]
    private int numberOfGrassNeededToContinue;
    private int numberOfGrassEaten = 0;
    private bool breathed = false;
    private bool interacted = false;
    private PlayerManager player;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerManager>();
        player.getPlayerValuesEvent().AddListener(checkPlayerValues);
    }
    private void checkPlayerValues()
    {
        //Debug.Log("Player value Updates Received by game controller");
        this.numberOfGrassEaten = player.ateGrassNum;
        this.breathed = player.breathed;
        checkIflevelComplete();
    }

    void checkIflevelComplete()
    {
        if (numberOfGrassEaten >= numberOfGrassNeededToContinue & breathed 
            //& interacted
            )
        {
            GameObject.Find("LevelExitVolume").GetComponent<LevelExitBehav>().levelComplete();
        }
    }
}
