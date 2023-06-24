using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimTo : AbstractAction
{
    private float movementSpeed, rotationSpeed;
    protected bool isSwimming = false;
    protected IEnumerator swimmingCoroutine;
    public SwimTo(ManateeBehavior manatee, float movementSpeed, float rotationSpeed) : base(manatee) {
        this.movementSpeed = movementSpeed;
        this.rotationSpeed = rotationSpeed;
    }

    /// <summary>
    /// Force the manatee to stop swimming.
    /// 
    /// The manatee will still drift forward for a bit.
    /// </summary>
    protected override void ForceEnd()
    {
        // Add drag to slow the manatee down
        manateeRb.drag = 1;
        if (isSwimming) {
            manatee.StopCoroutine(swimmingCoroutine);
        }
        manateeAnimator.SetBool("isSwimming", false);
    }


    /// <summary>
    /// Swim for a random amount of time.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator ActionCoroutine() {

        bool swimBackwards = (Random.Range(0, 2) < 1);

        manateeAnimator.SetBool("isSwimming", true);
        
        swimmingCoroutine = SwimToCoroutine(GameObject.Find("Seagrass(Tasty)").transform.position);
        manatee.StartCoroutine(swimmingCoroutine);
        Debug.Log("Swimming to destination");
        while (isSwimming) {
            yield return null;
        }
        Debug.Log("Finished swimming");
        manateeAnimator.SetBool("isSwimming", false);
        
        
        
        // // Set velocity forward for a bit of time
        // manateeRb.velocity = manatee.transform.forward * movementSpeed;
        // manateeRb.drag = 0;

        // if (swimBackwards) {
        //     manateeRb.velocity = manatee.transform.forward * movementSpeed * -1;
        // }

        // // Swim for a random amount of time
        // yield return new WaitForSeconds(Random.Range(1, 5));

        // // Come to a slow stop by adding drag for a bit of time
        // manateeAnimator.SetBool("isSwimming", false);
        // manateeRb.drag = 1;
        // yield return new WaitForSeconds(4);
        this.OnComplete();
    }

    protected IEnumerator SwimToCoroutine(Vector3 destination) {
        isSwimming = true;
        // Rotate to face the destination (only rotate on Y and X axis)
        Vector3 endDirection = (destination - manatee.transform.position).normalized;   // Our forward direction should match this direction

        // manatee.transform.forward = endDirection;    // This snaps the manatee to look in the right direction, but we want to get here gradually

        
        // Rotation on y axis = arctan(z / x), plus/minus some depending on the quadrant
        // If x == 0, we cannot divide, so just set yaw to 0
        float yawGoal = endDirection.x != 0 ?
            90 - Mathf.Atan(endDirection.z / endDirection.x) * Mathf.Rad2Deg
            : (endDirection.z < 0 ? 180 : 0);   // If x = 0, yaw = 180 if z < 0, or 0 if z >= 0
        if (endDirection.x < 0) {
            yawGoal += 180;
        }

        float pitchGoal = endDirection.z != 0 ?
            Mathf.Atan(endDirection.y / endDirection.z) * Mathf.Rad2Deg
            : (endDirection.y < 0 ? 90 : -90);   // If z = 0, pitch = 90 if y < 0, or -90 if y >= 0
        if (endDirection.z > 0) {
            pitchGoal *= -1; 
        }
        if (endDirection.y > 0) {
            pitchGoal += 360;
        }

        
        // Variables for gradually rotating up or down
        float endChangeInX = pitchGoal - manatee.transform.eulerAngles.x;
        if (endChangeInX > 180) {
            endChangeInX -= 360;
        } else if (endChangeInX < -180) {
            endChangeInX += 360;
        }
        float elapsedChangeInX = 0;
        float deltaX;
        
        // Variables for gradually rotating on the Y axis
        float endChangeInY = CalculateDeltaY(manatee.transform.eulerAngles.y, yawGoal);
        float elapsedChangeInY = 0;
        float deltaY;
        bool changingY, changingX;

        do {
            // Check if we still need to rotate
            changingY = elapsedChangeInY < Mathf.Abs(endChangeInY);
            changingX = elapsedChangeInX < Mathf.Abs(endChangeInX);

            // Rotate on the Y axis (yaw)
            if (changingY) {
                deltaY = Mathf.Sign(endChangeInY) * Time.deltaTime * rotationSpeed;
                elapsedChangeInY += Mathf.Abs(deltaY);
                manatee.transform.Rotate(0, deltaY, 0, Space.World);
            }

            // Rotate on the X axis (pitch)
            if (changingX) {
                deltaX = Mathf.Sign(endChangeInX) * Time.deltaTime * rotationSpeed * 0.2f;
                elapsedChangeInX += Mathf.Abs(deltaX);
                manatee.transform.Rotate(deltaX, 0, 0, Space.Self);
            }

            yield return null;
        } while (changingY || changingX);

        

        // Swim forward until the destination is reached
        manatee.transform.forward = (destination - manatee.transform.position).normalized;
        manateeRb.velocity = manatee.transform.forward * movementSpeed;
        manateeRb.drag = 0;
        float distanceToGoal = (destination - manatee.transform.position).magnitude;
        float distanceTraveled = 0;
        Vector3 previousLocation = manatee.transform.position;
        while (distanceTraveled < distanceToGoal) {
            distanceTraveled += (manatee.transform.position - previousLocation).magnitude;
            previousLocation = manatee.transform.position;
            yield return null;
        }
        manateeRb.drag = 2;

        // Flatten out rotation


        // yield return new WaitForSecondsRealtime(5);
        isSwimming = false;
    }

    /// <summary>
    /// Calculate the shortest way to rotate from currentY to targetY.
    /// This method takes into consideration the possibility of wrapping around and rotating past 0 / 360 degrees.
    /// 
    /// Note: There might be a more efficient math-y way to do this method, but this approach makes sense to me (alex).
    /// </summary>
    /// <param name="currentY"> the starting Y rotation </param>
    /// <param name="targetY"> the ending Y rotation </param>
    /// <returns> shortest change in degrees of rotation to get from currentY to targetY </returns>
    private float CalculateDeltaY(float currentY, float targetY)
    {
        float deltaY;

        // If you take a circle representing the possible angles, and lay the circle into a line from 0 to 360,
        // there are two points: currentY and targetY. We need to determine if it is shorter to wrap around and go
        // through 0/360 degrees, or if it is shorter to not wrap around.
        float shortestDistance = Mathf.Abs(targetY - currentY);

        bool wrapAroundZero = false;
        if(360 - shortestDistance < shortestDistance)
        {
            wrapAroundZero = true;
            shortestDistance = 360 - shortestDistance;
        }

        // If we are not wrapping around 0, change in rotation is just (end - start)
        if (!wrapAroundZero)
        {
            deltaY = targetY - currentY;
        } else
        {
            // We are wrapping around 0

            // We need to rotate counterclockwise / anticlockwise / in the negative direction if the target is greater than where we currently are
            if(targetY > currentY)
            {
                deltaY = shortestDistance * -1;

            // If the target is less than the current rotation, we will rotate clockwise / in the positive direction to wrap around and get to the target
            } else
            {
                deltaY = shortestDistance;
            }

        }

        return deltaY;
    }
}
