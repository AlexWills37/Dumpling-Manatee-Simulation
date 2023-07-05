using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManateeHeavenGameController : MonoBehaviour
{
    [SerializeField]
    private int numberOfGrassNeededToContinue;
    private int numberOfGrassEaten = 0;
    private bool breathed = false;
    private bool interacted = false;
    public void grassEaten()
    {
        numberOfGrassEaten += 1;
        checkIflevelComplete();
    }
    public void manateesInteracted()
    {
        interacted = true;
        checkIflevelComplete();
    }
    public void playerBreathes()
    {
        breathed = true;
        checkIflevelComplete();
    }
    void checkIflevelComplete()
    {
        if (numberOfGrassEaten >= numberOfGrassNeededToContinue 
            //& breathed & interacted
            )
        {
            GameObject.Find("LevelExitVolume").GetComponent<LevelExitBehav>().levelComplete();
        }
    }
}
