using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehavior2 : MonoBehaviour
{
    [SerializeField]
    float steeringStrength;
    [SerializeField]
    float coneSize;
    [SerializeField]
    float visionDistance;
    [SerializeField]
    float fishSpeed;
    [SerializeField]
    float schoolSize;
    [SerializeField]
    float forceFieldRadius;
    [SerializeField]
    float extraVision;
    //Rigidbody rb;
    private float timeCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        this.GetComponent<Rigidbody>().AddRelativeTorque(calculateForce()*steeringStrength * Time.deltaTime);
        this.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * fishSpeed * Time.deltaTime);
        //RaycastHit hit;

        //if (Physics.Raycast(transform.position, transform.forward, out hit, visionDistance))
        // {
        //    transform.rotation = transform.rotation * findNewValidAngle();
        //}
        //rb.AddForce(Vector3.forward * fishSpeed);
        //transform.Translate(Vector3.forward * fishSpeed * Time.deltaTime);
    }

    private Vector3 calculateForce()
    {
        bool[] hitCount = new bool[4];
        RaycastHit hit;
        Vector3 output = new Vector3(0, 0, 0);

        Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-coneSize, transform.up) * transform.forward * visionDistance), Color.blue);

        Debug.DrawRay(transform.position, (Quaternion.AngleAxis(coneSize, transform.up) * transform.forward * visionDistance), Color.red);
        if(!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(coneSize, transform.up) * transform.forward * visionDistance), out hit, visionDistance)))
        {
            output += new Vector3(0, visionDistance, 0);
        }
        else
        {
            output += new Vector3(0, hit.distance, 0);
            hitCount[0] = true;
        }


        Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-coneSize, transform.up) * transform.forward * visionDistance), Color.yellow);
        if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(-coneSize, transform.up) * transform.forward * visionDistance), out hit, visionDistance)))
        {
            output += new Vector3(0, -visionDistance, 0);
        }
        else
        {
            output += new Vector3(0, - hit.distance, 0);
            hitCount[2] = true;
        }


        Debug.DrawRay(transform.position, (Quaternion.AngleAxis(coneSize, transform.right) * transform.forward * visionDistance), Color.magenta);
        if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(coneSize, transform.right) * transform.forward * visionDistance), out hit, visionDistance)))
        {
            output += new Vector3(visionDistance, 0, 0);
        }
        else
        {
            output += new Vector3(hit.distance, 0, 0);
            hitCount[1] = true;

        }


        Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-coneSize, transform.right) * transform.forward * visionDistance), Color.cyan);
        if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(-coneSize, transform.right) * transform.forward * visionDistance), out hit, visionDistance)))
        {
            output += new Vector3(-visionDistance, 0, 0);
        }
        else
        {
            output += new Vector3(-hit.distance, 0, 0);
            hitCount[3] = true;
        }


        if ((hitCount[0] & hitCount[2])|(hitCount[1] & hitCount[3])|(hitCount[0] & hitCount[1] & hitCount[2] & hitCount[3]))
        {
            Debug.DrawRay(transform.position, (Quaternion.AngleAxis(extraVision * coneSize, transform.up) * transform.forward * visionDistance), Color.red);
            if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(extraVision * coneSize, transform.up) * transform.forward * visionDistance), out hit, visionDistance)))
            {
                output += new Vector3(0, visionDistance, 0);
            }
            else
            {
                output += new Vector3(0, hit.distance, 0);
            }


            Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-extraVision*coneSize, transform.up) * transform.forward * visionDistance), Color.yellow);
            if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(-extraVision * coneSize, transform.up) * transform.forward * visionDistance), out hit, visionDistance)))
            {
                output += new Vector3(0, -visionDistance, 0);
            }
            else
            {
                output += new Vector3(0, -hit.distance, 0);
            }


            Debug.DrawRay(transform.position, (Quaternion.AngleAxis(extraVision * coneSize, transform.right) * transform.forward * visionDistance), Color.magenta);
            if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(extraVision * coneSize, transform.right) * transform.forward * visionDistance), out hit, visionDistance)))
            {
                output += new Vector3(visionDistance, 0, 0);
            }
            else
            {
                output += new Vector3(hit.distance, 0, 0);

            }


            Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-extraVision * coneSize, transform.right) * transform.forward * visionDistance), Color.cyan);
            if (!(Physics.Raycast(transform.position, (Quaternion.AngleAxis(-extraVision * coneSize, transform.right) * transform.forward * visionDistance), out hit, visionDistance)))
            {
                output += new Vector3(-visionDistance, 0, 0);
            }
            else
            {
                output += new Vector3(-hit.distance, 0, 0);
            }

        }
        return output.normalized;
    }
}
