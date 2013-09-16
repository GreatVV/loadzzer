using UnityEngine;
using System.Collections;

public class IsoCameraFollow : MonoBehaviour {

    // The target we are following
    public Transform target;

    // The distance in the x-z plane to the target
    public float distance;

    // the height we want the camera to be above the target
    public float height = 5.0f;

    public static IsoCameraFollow instance;

    // Constructor
    public void Awake()
    {
        instance = this;
        //camera.transparencySortMode = TransparencySortMode.Orthographic;
        if (target) SetTarget(target);
    }

    // Set new camera target
    public static void SetTarget(Transform newTarget) 
    {
        instance.target = newTarget;
    }

    void LateUpdate()
    {
        // Early out if we don't have a target
        if (!target) return;

        // Set the height of the camera
        transform.position = target.position + new Vector3(-distance, target.position.y + height, -distance);
    }
}