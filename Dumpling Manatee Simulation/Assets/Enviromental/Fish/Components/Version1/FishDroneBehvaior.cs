using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishDroneBehvaior : MonoBehaviour
{
    [SerializeField]
    GameObject leader;
    float schoolSize;
    float fishSpeed;
    float visionDistance;
    float coneSize = 2;
    bool tooClose = true;
    float forceFieldRadius;
    float distanceToLeader;
    float lastDistanceToLeader;
    bool turningTowardsLeader = false;
    Quaternion startRotation;
    Quaternion destinationRotation;
    float counter;
    void Start()
    {
        FishBehavior leaderBehvaior = leader.GetComponent<FishBehavior>();
        fishSpeed = leaderBehvaior.getFishSpeed();
        schoolSize = leaderBehvaior.getSchoolSize();
        visionDistance = leaderBehvaior.getVisionDistance() - 1;
        forceFieldRadius = leaderBehvaior.getforceFieldRadius();
    }

    void Update()
    {
        lastDistanceToLeader = distanceToLeader;
        distanceToLeader = (transform.position - leader.transform.position).magnitude;

        if (Physics.Raycast(transform.position, transform.forward, visionDistance) )
        {
            transform.rotation = transform.rotation * findNewValidAngle();
            if (turningTowardsLeader)
            {
                turningTowardsLeader = false;
            }
        }
        else
        {
            if (distanceToLeader > schoolSize)
            {
                tooClose = false;
                if (distanceToLeader>lastDistanceToLeader)
                {
                    //Debug.DrawRay(transform.position,  leader.transform.position - transform.position);
                    RaycastHit hit;
                    Physics.Raycast(transform.position, leader.transform.position - transform.position, out hit);
                    if (hit.collider.gameObject.name == leader.name) 
                    {
                        if (! turningTowardsLeader)
                        {
                            //Debug.Log("starting turn towards leader");
                            startRotation = transform.rotation;
                            turningTowardsLeader = true;
                            destinationRotation = Quaternion.LookRotation(leader.transform.position - transform.position);
                            counter = 0;
                        }
                        else
                        {
                            //Debug.Log(counter);
                            counter += Time.deltaTime*8;
                            //Debug.DrawRay(transform.position, leader.transform.position - transform.position);
                            transform.rotation = Quaternion.Slerp(startRotation, destinationRotation, counter);
                            if (counter >= 1)
                            {
                                turningTowardsLeader = false;
                                //Debug.Log("Finished turn towards leader");
                            }
                        }
                        //transform.rotation = Quaternion.LookRotation(leader.transform.position - transform.position);
                    }
                }
            }
            if (distanceToLeader < forceFieldRadius)
            {
                tooClose = true;
            }
        }
        if (tooClose)
        {
            transform.Translate(Vector3.forward * (fishSpeed - .5f)*Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.forward * (fishSpeed + .5f) * Time.deltaTime);
        }
        
    }

    private Quaternion findNewValidAngle()
    {
        for (float i = 0; i < 360; i += coneSize)
        {
            RaycastHit hit;
            //Debug.DrawRay(transform.position, Quaternion.Euler(0, i, 0) * (transform.forward * visionDistance));
            if (!(Physics.Raycast(transform.position, Quaternion.Euler(0, i, 0) * (transform.forward), visionDistance)))
            {
               // Debug.DrawRay(transform.position, Quaternion.Euler(0, i, 0) * (transform.forward * visionDistance), Color.red, 1);
                return Quaternion.Euler(0, i, 0);
            }

            //Debug.DrawRay(transform.position, Quaternion.Euler(0, -i, 0) * (transform.forward * visionDistance));
            if (!(Physics.Raycast(transform.position, Quaternion.Euler(0, -i, 0) * (transform.forward), visionDistance)))
            {
                //Debug.DrawRay(transform.position, Quaternion.Euler(0, -i, 0) * (transform.forward * visionDistance), Color.red, 1);
                return Quaternion.Euler(0, -i, 0);
            }
            //Debug.DrawRay(transform.position, Quaternion.Euler(i, 0, 0) * (transform.forward * visionDistance));
            if (!Physics.Raycast(transform.position, Quaternion.Euler(i, 0, 0) * (transform.forward), out hit, visionDistance))
            {
                //Debug.DrawRay(transform.position, Quaternion.Euler(i, 0, 0) * (transform.forward * visionDistance), Color.red, 1);
                return Quaternion.Euler(i, 0, 0);
            }

            //Debug.DrawRay(transform.position, Quaternion.Euler(-i, 0, 0) * (transform.forward * visionDistance));
            if (!Physics.Raycast(transform.position, Quaternion.Euler(-i, 0, 0) * (transform.forward), out hit, visionDistance))
            {
                //Debug.DrawRay(transform.position, Quaternion.Euler(-i, 0, 0) * (transform.forward * visionDistance), Color.red, 1);
                return Quaternion.Euler(-i, 0, 0);
            }

        }
        Debug.Log("failed to find path forward");
        return Quaternion.Euler(180, 0, 0);
    }
}
