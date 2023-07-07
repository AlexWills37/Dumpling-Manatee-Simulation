using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehavior2 : MonoBehaviour
{
    [SerializeField][Tooltip("How sharply the fish is capable of turning")]
    float steeringStrength; 
    [SerializeField][Tooltip("How far out to each side the fish can look (measured in degrees from the center of its vision)")]
    float coneSize;
    [SerializeField][Tooltip("How far ahead the fish can see")]
    float visionDistance;
    [SerializeField][Tooltip("How fast the fish swims forward")]
    float fishSpeed;
    [SerializeField][Tooltip("How much farther to the sides the fish looks if every path ahead is blocked (this is a multiplier ont he coneSize parameter meaning that if coneSize is 10 and extraVision is 4, it will look 40 degrees to each side when pathfinding)")]
    float extraVision;
    Rigidbody rb; //stor ethe fish's rigidbody
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.AddRelativeTorque(calculateTurningForce()*steeringStrength * Time.deltaTime); //turn the fish using the calculateTurningForce() function
        rb.AddRelativeForce(Vector3.forward * fishSpeed * Time.deltaTime); //add some forward velocity
    }

    /// <summary>
    /// This function calculates the direction the fish should turn in. it does so by casting 4 rays ahead, each offset by coneSize degrees, each ray then applies a steering force towards its direction equivelent to the distance until it hit an obstacle. this means that if all rays hit nothing, the steering forces cancel out, but if the ray pointing to the right hits something it the fish will turn to the left since the left ray will apply6 more steering force than the right. The closer the object that is hit, the less force is applied in that direction meaning that the fish turns more sharply when something is close to it than far away
    /// There is an added level to this system which I created to solve  aproblem where the fish would swim into a corner and get stuck if it approached at exactly 45 degrees since the left and right rays would cancel out even thought they were both seeing something. If any two opposite rays are hitting something, the fish adds 4 additional raycasts out to the sides in order to determine a safe path. This removed some of the behaviors where the fish got stuch, but the fish still can get stuch if their faces somehow become pressed against a flat surface (all of their raycasts will terminate at distance 0)
    /// </summary>
    /// <returns>returns a vector3 representing the direction the fish should turn</returns>
    private Vector3 calculateTurningForce()
    {
        bool[] hitCount = new bool[4];//an array of booleans to record which rays have hit something (used in the second step described above)
        RaycastHit hit; //create a raycast hit to record info about each cast
        Vector3 output = new Vector3(0, 0, 0); //create a base output vector to be modified down the line

        //cast a ray to the right
        //Debug.DrawRay(transform.position, (Quaternion.AngleAxis(coneSize, transform.up) * transform.forward * visionDistance), Color.red);
        if(!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(coneSize, transform.up) * transform.forward * visionDistance), out hit, visionDistance)))
        {
            output += new Vector3(0, visionDistance, 0); //If it hits nothing, add its maximum value (visionDistance) to the turn force
        }
        else
        {
            output += new Vector3(0, hit.distance, 0); //if it did hit something, add the distance to the hit to the turning force (so the force will be smaller when ojects are closer)
            hitCount[0] = true; //also record that  ahit was detected int he boolean array
        }

        //same as above for left
        //Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-coneSize, transform.up) * transform.forward * visionDistance), Color.yellow);
        if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(-coneSize, transform.up) * transform.forward * visionDistance), out hit, visionDistance)))
        {
            output += new Vector3(0, -visionDistance, 0);
        }
        else
        {
            output += new Vector3(0, - hit.distance, 0);
            hitCount[2] = true;
        }

        //cast a ray up
        //Debug.DrawRay(transform.position, (Quaternion.AngleAxis(coneSize, transform.right) * transform.forward * visionDistance), Color.magenta);
        if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(coneSize, transform.right) * transform.forward * visionDistance), out hit, visionDistance)))
        {
            output += new Vector3(visionDistance, 0, 0);
        }
        else
        {
            output += new Vector3(hit.distance, 0, 0);
            hitCount[1] = true;

        }

        //same as above for down
        //Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-coneSize, transform.right) * transform.forward * visionDistance), Color.cyan);
        if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(-coneSize, transform.right) * transform.forward * visionDistance), out hit, visionDistance)))
        {
            output += new Vector3(-visionDistance, 0, 0);
        }
        else
        {
            output += new Vector3(-hit.distance, 0, 0);
            hitCount[3] = true;
        }


        //now check if any two opposite rays have hit something. If so, repeat the process above using a larger value for the rotatiosn away from forward vector
        if ((hitCount[0] & hitCount[2])|(hitCount[1] & hitCount[3])|(hitCount[0] & hitCount[1] & hitCount[2] & hitCount[3]))
        {
            //Same as above but with the multiplier extravision added to the angle the ray is rotated by
            //Debug.DrawRay(transform.position, (Quaternion.AngleAxis(extraVision * coneSize, transform.up) * transform.forward * visionDistance), Color.red);
            if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(extraVision * coneSize, transform.up) * transform.forward * visionDistance), out hit, visionDistance)))
            {
                output += new Vector3(0, visionDistance, 0);
            }
            else
            {
                output += new Vector3(0, hit.distance, 0);
            }


            //Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-extraVision*coneSize, transform.up) * transform.forward * visionDistance), Color.yellow);
            if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(-extraVision * coneSize, transform.up) * transform.forward * visionDistance), out hit, visionDistance)))
            {
                output += new Vector3(0, -visionDistance, 0);
            }
            else
            {
                output += new Vector3(0, -hit.distance, 0);
            }


            //Debug.DrawRay(transform.position, (Quaternion.AngleAxis(extraVision * coneSize, transform.right) * transform.forward * visionDistance), Color.magenta);
            if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(extraVision * coneSize, transform.right) * transform.forward * visionDistance), out hit, visionDistance)))
            {
                output += new Vector3(visionDistance, 0, 0);
            }
            else
            {
                output += new Vector3(hit.distance, 0, 0);

            }


            //Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-extraVision * coneSize, transform.right) * transform.forward * visionDistance), Color.cyan);
            if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(-extraVision * coneSize, transform.right) * transform.forward * visionDistance), out hit, visionDistance)))
            {
                output += new Vector3(-visionDistance, 0, 0);
            }
            else
            {
                output += new Vector3(-hit.distance, 0, 0);
            }

        }
        //return the normalized otput (to avoid visiondistance having any impact on how quickly the fish can turn)
        return output.normalized;
    }
}
