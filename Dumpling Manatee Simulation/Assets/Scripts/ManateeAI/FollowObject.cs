using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Force this game object to copy the position of another game object.
/// 
/// @author Alex Wills
/// Updated 6/15/2023
/// </summary>
public class FollowObject : MonoBehaviour
{
    [Tooltip("The game object to follow")]
    [SerializeField] private Transform toFollow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = toFollow.position;
        this.transform.eulerAngles = toFollow.eulerAngles;    
    }
}
