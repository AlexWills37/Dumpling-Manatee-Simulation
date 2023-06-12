using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassWave : MonoBehaviour
{
    SkinnedMeshRenderer[] grasses;
    int[] grassRotations;
    //float[] xPositions;
    float SideBlendAmount;
    float VerticalBlendAmount;
    float timer;
    [SerializeField]
    float bendSpeedMultiplier;
    [SerializeField]
    float bendForceMultiplier;
    [SerializeField]
    AnimationCurve bendingCurve;
    [SerializeField]
    AnimationCurve ZbendingCurve;

    void Start()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("SeaGrass");
        grasses = new SkinnedMeshRenderer[temp.Length];
        grassRotations = new int[temp.Length];
        //xPositions = new float[temp.Length];
        for (int i=0;  i<temp.Length; i++){
            grasses[i] = temp[i].GetComponent<SkinnedMeshRenderer>();
            grassRotations[i] = temp[i].GetComponent<GrassInitScript>().GetShapeKeyRotation();
            Destroy(temp[i].GetComponent<GrassInitScript>());
            //xPositions[i] = temp[i].transform.position.x;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 2/ bendSpeedMultiplier)
        {
            timer -= 2 / bendSpeedMultiplier;
        }
        SideBlendAmount = bendForceMultiplier* bendingCurve.Evaluate(timer * bendSpeedMultiplier);
        //SideBlendAmount = bendForceMultiplier * (Mathf.Sin(timer * bendSpeedMultiplier));
        // VerticalBlendAmount = bendForceMultiplier * ((Mathf.Sin((4 * timer * bendSpeedMultiplier) - (Mathf.PI / 2)) + 1) / 2);
        VerticalBlendAmount = bendForceMultiplier*ZbendingCurve.Evaluate(Mathf.Abs(bendingCurve.Evaluate(timer * bendSpeedMultiplier)));
        for (int i = 0; i < grasses.Length; i++)
        {
            if (SideBlendAmount < 0)
            {
                grasses[i].SetBlendShapeWeight(invertRotation(grassRotations[i]), -SideBlendAmount);
            }
            else
            {
                grasses[i].SetBlendShapeWeight(grassRotations[i], SideBlendAmount);
            }
            grasses[i].SetBlendShapeWeight(4, VerticalBlendAmount);
        }
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
