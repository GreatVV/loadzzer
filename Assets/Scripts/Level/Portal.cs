using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

    public Vector3 teleportToPosition;
    public Vector3 thisPosition;

    void Start()
    {
        thisPosition = transform.position;
    }

    void Update()
    {
        //if intersects
            //create a copy
            //update copy according parent
            //if parent fully in teleport - remove copy and teleport to position
    }
}
