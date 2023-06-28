using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehavior : MonoBehaviour
{
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
    //Rigidbody rb;
    private float timeCount;
    // Start is called before the first frame update
    void Start()
    {
        //rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, visionDistance))
        {
            transform.rotation = transform.rotation * findNewValidAngle();
        }
        //rb.AddForce(Vector3.forward * fishSpeed);
        transform.Translate(Vector3.forward * fishSpeed*Time.deltaTime);     
    }

    private Quaternion findNewValidAngle()
    {
        for(float i=0; i < 360; i += coneSize)
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position, Quaternion.Euler(0, i, 0) * (transform.forward * visionDistance));
            if (! (Physics.Raycast(transform.position, Quaternion.Euler(0, i, 0) * (transform.forward), visionDistance)))
            {
                Debug.DrawRay(transform.position, Quaternion.Euler(0, i, 0) * (transform.forward * visionDistance), Color.red, 1);
                return Quaternion.Euler(0, i, 0);
            }

            Debug.DrawRay(transform.position, Quaternion.Euler(0, -i, 0) * (transform.forward * visionDistance));
            if (! (Physics.Raycast(transform.position, Quaternion.Euler(0, -i, 0) * (transform.forward), visionDistance)))
            {
                Debug.DrawRay(transform.position, Quaternion.Euler(0, -i, 0) * (transform.forward * visionDistance), Color.red, 1);
                return Quaternion.Euler(0, -i, 0);
            }
            Debug.DrawRay(transform.position, Quaternion.Euler(i, 0, 0) * (transform.forward * visionDistance));
            if (!Physics.Raycast(transform.position, Quaternion.Euler(i, 0, 0) * (transform.forward), out hit, visionDistance))
            {
                Debug.DrawRay(transform.position, Quaternion.Euler(i, 0, 0) * (transform.forward * visionDistance), Color.red, 1);
                return Quaternion.Euler(i, 0, 0);
            }

            Debug.DrawRay(transform.position, Quaternion.Euler(-i, 0, 0) * (transform.forward * visionDistance));
            if (!Physics.Raycast(transform.position, Quaternion.Euler(-i, 0, 0) * (transform.forward), out hit, visionDistance))
            {
                Debug.DrawRay(transform.position, Quaternion.Euler(-i, 0, 0) * (transform.forward * visionDistance), Color.red, 1);
                return Quaternion.Euler(-i, 0, 0);
            }

        }
        //Debug.Log("failed to find path forward");
        return Quaternion.Euler(180, 0, 0);
    }
   
    private IEnumerator rotateToNewAngle(Quaternion newAngle, Quaternion startingAngle)
    {
        transform.rotation = Quaternion.Slerp(startingAngle, newAngle, timeCount);
        timeCount += .1f;
        yield return new WaitForSeconds(.01f);
    }

    public float getFishSpeed()
    {
        return fishSpeed;
    }
    public float getSchoolSize()
    {
        return schoolSize;
    }

    public float getVisionDistance()
    {
        return (visionDistance);
    }

    public float getforceFieldRadius()
    {
        return forceFieldRadius;
    }
}
