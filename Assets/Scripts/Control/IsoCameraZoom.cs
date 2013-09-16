using UnityEngine;
using System.Collections;

public class IsoCameraZoom : MonoBehaviour
{

    public float minZoom = 20;
    public float maxZoom = 5;
    public float zoomSpeed = 5;

    // Current zoom
    float zoom;

    // Touch parameters
    float minPinchSpeed = 5.0F; 
    float varianceInDistances = 5.0F;  
    Vector2 prevDist = new Vector2(0,0); 
    Vector2 curDist = new Vector2(0,0);

    public static IsoCameraZoom instance;

    // Use this for initialization
    void Start()
    {
        zoom = camera.orthographicSize;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //if (GuiWindow.IsOpened)
        //{
        //    return;
        //}

        float direction = 0;

        // Mouse Zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            direction = Input.GetAxis("Mouse ScrollWheel");
        }

        // Touch zoom
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {

            curDist = Input.GetTouch(0).position - Input.GetTouch(1).position; //current distance between finger touches
            prevDist = ((Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition) - (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition)); //difference in previous locations using delta positions
            float touchDelta = curDist.magnitude - prevDist.magnitude;
            float speedTouch0 = Input.GetTouch(0).deltaPosition.magnitude / Input.GetTouch(0).deltaTime;
            float speedTouch1 = Input.GetTouch(1).deltaPosition.magnitude / Input.GetTouch(1).deltaTime;

            if ((touchDelta + varianceInDistances <= 1) && (speedTouch0 > minPinchSpeed) && (speedTouch1 > minPinchSpeed)) direction = -0.1f;
            if ((touchDelta + varianceInDistances > 1) && (speedTouch0 > minPinchSpeed) && (speedTouch1 > minPinchSpeed)) direction = 0.1f;
        }

        if (direction != 0)
        {
           zoom = Mathf.Clamp(zoom - direction * zoomSpeed, maxZoom, minZoom);
        }

        if(zoom != camera.orthographicSize) camera.orthographicSize = Mathf.Lerp(zoom, camera.orthographicSize, 1 - Time.deltaTime * 5);

    }

    public static void SetZoom(float newZoom)
    {
        instance.zoom = instance.camera.orthographicSize = newZoom;
    }

}
