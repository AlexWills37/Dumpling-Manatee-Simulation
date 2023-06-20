using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controls the waving behgavior of grasses in the scene. It gets all of the objects taged "SeaGrass" and updates them every frame with the appropriate values for their shape keys
/// </summary>
public class GrassWave : MonoBehaviour
{
    private SkinnedMeshRenderer[] grassMeshRenderers; //Stores the skinnedMeshRendereres of every grass
    private int[] grassRotations; //stores the rotations of the grasses (in order to move the appropriate shape keys
    private float SideBlendAmount; //the amount of bending side to side to apply
    private float VerticalBlendAmount; //the amount of vertical bend (applied to shape key 4 the Z-axis stretch)
    private float timer; //keeps track of the current time (used to calculate how the grass should bend)
    [SerializeField]
    private float bendSpeedMultiplier; //how quickly the grasses should move back and forth
    [SerializeField]
    private float bendForceMultiplier; //how strongly the grasses should bend
    [SerializeField]
    private AnimationCurve sideBendCurve; //the curve which the side-to-side bending follows
    [SerializeField]
    private AnimationCurve zBendingAmount; // the curve which the vertical bending follows

    /// <summary>
    /// The start function gathers data on all of the various sea grasses and stores them in this script to save expensive FindGameObjectsWithTag calls down the line. once it has all of the objects with the tag seagrass it stores the SkinnedMeshRenderer components and gets their rotations from the GrassInitScript. It then destroys the GrassInitScript to save memory.
    /// This is why setting the right value in the GrassInitScript is so critical. The grassWave knows what axis to animate them on only by this value.
    /// This method of figuring out their rotation is really clumsy. My initial attempt was to have this script automatically figure out their rotation, but bevcause of euler representations it was not possible for me to get a concrete idea of their rotation and would often break if they were rotated on another axis (ie if they sat at an angle on a slope). Hence this clumsy method was chose. If a better approach can be found I strongly encourage its use (although if you have time to develop a furthur shape key grass bending system you should give up and figure out how to write a shader since its the correct way to do this)
    /// </summary>
    void Start()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("SeaGrass"); //get all the sea grass objects
        grassMeshRenderers = new SkinnedMeshRenderer[temp.Length]; //initialize the storage arrays to have the right length
        grassRotations = new int[temp.Length]; //iterate through the seagrasses and store their rotations and meshrenderers.
        for (int i=0;  i<temp.Length; i++){ //crucially this happens in order so that grassMeshRenderers[1] and grassRotations[1] refer to the same object
            grassMeshRenderers[i] = temp[i].GetComponent<SkinnedMeshRenderer>();
            grassRotations[i] = temp[i].GetComponent<GrassInitScript>().GetShapeKeyRotation();
            Destroy(temp[i].GetComponent<GrassInitScript>()); //remove the initialization script to save space in memory
        }
    }

    /// <summary>
    /// The update function runs a calculation to figure out what the bend amounts should be the iterates through every grass to update it. If the result of the function is negative it uses the invertRotation function to figure out what the correct shape key to change is 
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime; //add to the timer
        if (timer > 2/ bendSpeedMultiplier)
        {
            timer -= 2 / bendSpeedMultiplier; //remove 2 from the timer since that is the period of the wave function. keeps timer value from getting crazy large. I think this approach is more effecient than doing timer = (timer+Time.deltaTime)%2 every frame
        }
        //use the curves to evaluate the correct values for the shape keys.

        SideBlendAmount = sideBendCurve.Evaluate(timer * bendSpeedMultiplier); //set the sideBendAmount to the timer * bendSpeedMultiplier (nedspeedmultiplier effects the frequency of the wave)(avoid changing the power of the wave here so that the value can be reused for calculating the VerticalBendAmount)

        VerticalBlendAmount = bendForceMultiplier*zBendingAmount.Evaluate(Mathf.Abs(SideBlendAmount)); //The zBendingAmount is evaluated based off of the result of the previous wave since I wanted it to reach its maximum while the other curve was at 0. This interaction of waves creates an effect such that the grass followes a less straight path than normal shape key changes.

        SideBlendAmount = bendForceMultiplier * SideBlendAmount; //now add the amplitude change into sideBlendAmoutn 

        for (int i = 0; i < grassMeshRenderers.Length; i++) //iterate through each grass, setting its values appropriately
        {
            if (SideBlendAmount < 0) //if the sideBendAmpoutn is negative, invert the rotations (get the opposite shape key), but still give it a positive value since shape keys dont play nicely with negatives
            {
                grassMeshRenderers[i].SetBlendShapeWeight(invertRotation(grassRotations[i]), -SideBlendAmount);
            }
            else
            {
                grassMeshRenderers[i].SetBlendShapeWeight(grassRotations[i], SideBlendAmount);
            }
            grassMeshRenderers[i].SetBlendShapeWeight(4, VerticalBlendAmount); //always use index 4 since the vertical one never changes
        }
    }

    /// <summary>
    /// The invert rotation function takes a shape key index and returns the inverse (x becomes -x, y becomes -y). This is done by hand since there is rewally no concrete rule possible.
    /// </summary>
    /// <param name="rotationToBeInverted">the index of the shape key that you want to opposite of</param>
    /// <returns>returns the index of the opposite shape key</returns>
    int invertRotation(int rotationToBeInverted)
    {
        switch (rotationToBeInverted)
        {
            case 0:
                return 1;
            case 1:
                return 0;
            case 2:
                return 3;
            case 3:
                return 2;
            default:
                 throw new System.Exception("bad grass rotation"); //if it gets a shape key rotation that is wrong it throws a grassRotation error
        }

           
    }
}
