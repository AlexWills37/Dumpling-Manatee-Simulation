using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Repositions the OVR CameraRig at the start of the scene, to offer control
/// over what direction the player starts off facing.
/// Help from https://forum.unity.com/threads/help-with-oculus-quest-reorientation-on-levelload.759626/#post-5066975
/// 
/// @author Alex Wills
/// @date 18 October 2023
/// </summary>
public class RecenterOVR : MonoBehaviour
{

    [Tooltip("Camera Rig object OVR prefab to reorient")]
    [SerializeField] private OVRCameraRig cameraRig;

    [Tooltip("The rotation to reorient the camera at the beginning of the scene")]
    [SerializeField] private float yRotation = 0;
    void Start()
    {
        // Use a coroutine to delay this method until the end of the frame
        StartCoroutine(ResetCameraRotation());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Resets the camera position to match the rotation specified in the inspector.
    /// This coroutine only takes 1 frame.
    /// </summary>
    /// <returns> IEnumerator representation of this coroutine </returns>
    private IEnumerator ResetCameraRotation() {
        
        // Wait until the end of the frame, after the Camera Rig has adjusted itself to the scene
        yield return new WaitForEndOfFrame();

        // Figure out how much the camera rig needs to be rotated
        Transform centerEye = cameraRig.centerEyeAnchor;
        float yDifference = yRotation - centerEye.eulerAngles.y;

        // Rotate the camera rig to meet the target rotation
        cameraRig.transform.Rotate(0, yDifference, 0);
        
    }
}
