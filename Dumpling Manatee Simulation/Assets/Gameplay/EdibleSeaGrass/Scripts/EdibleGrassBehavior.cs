using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EdibleGrassBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject grassParticles;
    [SerializeField]
    private float amountOfGrassToRemoveOnEating;
    private bool alreadyEaten = false;
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

    private void OnTriggerEnter(Collider other) {
        if (!alreadyEaten && other.CompareTag("Player")) {
            PlayerManager player = other.gameObject.transform.parent.GetComponent<PlayerManager>();
            this.onGrassEaten();
            player.OnGrassEaten();
            alreadyEaten = true;
        }
    }
}
