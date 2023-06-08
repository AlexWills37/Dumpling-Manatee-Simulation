using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassWave : MonoBehaviour
{
    GameObject[] grasses;
    int[] grassRotations;
    float SideBlendAmount;
    float VerticalBlendAmount;
    float timer;
    [SerializeField]
    float bendSpeedMultiplier;
    [SerializeField]
    float bendForceMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        grasses = GameObject.FindGameObjectsWithTag("SeaGrass");
        grassRotations = new int[grasses.Length];
        Debug.Log(grasses[0]);
        for (int i = 0; i < grasses.Length; i++)
        {
            grassRotations[i] = ShapeKeyRotation(grasses[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        SideBlendAmount = bendForceMultiplier * (Mathf.Sin(timer * bendSpeedMultiplier));
        VerticalBlendAmount = bendForceMultiplier * ((Mathf.Sin((4 * timer * bendSpeedMultiplier) - (Mathf.PI / 2)) + 1) / 2);
        for (int i = 0; i < grasses.Length; i++)
        {
            if (SideBlendAmount < 0)
            {
                grasses[i].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(invertRotation(grassRotations[i]), -SideBlendAmount);
            }
            else
            {
                grasses[i].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(grassRotations[i], SideBlendAmount);
            }
            grasses[i].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(4, VerticalBlendAmount);
        }
    }

    int ShapeKeyRotation(GameObject target)
    {
        int tempRotation = ((int)((target.transform.eulerAngles.y) / 90) % 4);
        Debug.Log(target);
        Debug.Log(tempRotation);
        if (tempRotation == 0)
            return 0;
        else if (tempRotation == 1)
            return 2;
        else if (tempRotation == 2)
            return 1;
        else if (tempRotation == 3)
            return 3;
        else
            throw new System.Exception("bad grass rotation");
    }
    int invertRotation(int rotationToBeInverted)
    {
        if (rotationToBeInverted == 0)
            return 1;
        else if (rotationToBeInverted == 1)
            return 0;
        else if (rotationToBeInverted == 2)
            return 3;
        else if (rotationToBeInverted == 3)
            return 2;
        else
            throw new System.Exception("bad grass rotation");
    }
}
