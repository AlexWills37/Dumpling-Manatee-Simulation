using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdibleGrassBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject grassParticles;
    [SerializeField]
    private float amountOfGrassToRemoveOnEating;

    public void onGrassEaten()
    {
        //anything meant to happen when the grass is consumed goes here
        Destroy(grassParticles);
        cullChildren(amountOfGrassToRemoveOnEating);
    }
    private void cullChildren(float percentCulled)
    {
        for(int i=0; i<this.transform.childCount*percentCulled & i<this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
