using UnityEngine;

public class CameraDrag : MonoBehaviour
{
	public static bool isDrag = false;
	
	public float dragSpeed = 5; // Camera Speed Movement
	
	public Vector3 minClampPos = new Vector3(-15, 10, -15);	// MIN CLAMP Vector3
	public Vector3 maxClampPos = new Vector3(-12, 12, -12); // MAX CLAMP Vector3

    private Vector3 dragOrigin; // START Drag Point
	private Vector3 pos;		// WORLD Position for Camera Movement
	private Vector3 touch;		// Touch Position needed only for Convertation to Viewport Point
	private Vector3 move;		// Camera Movement Vector3
	
	private float startZoom; // Start Camera Zoom

    public LayerMask groundMask;
	
	void Start() {
		startZoom = GetZoom();
	}
	
	float GetZoom()
	{
	    return camera.isOrthoGraphic ? camera.orthographicSize : camera.fieldOfView;
	}

    void LateUpdate()
	{
		isDrag = false;
		
		// Don't Drag from Gui
		if (GUIUtility.hotControl != 0) return;
		
        //// If Entity Now is Drag
        //if (ControlMoveTarget.isDrag || ControlMoveTarget.needToCenterCameraOnObject) {
        //    return;
        //}

        ////if any gui opened
        //if (GuiWindow.IsOpened)
        //{
        //    return;
        //}


		// MOUSE WAS DOWN
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }
		// TOUCH WAS DOWN
		else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
			dragOrigin = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
			return;
		}
		
		// CHECK DRAG STATE (Mouse or Touch)
        if (!Input.GetMouseButton(0))
			if (0 == Input.touchCount || Input.GetTouch(0).phase != TouchPhase.Moved) return;
		
        //if (ControlMoveTarget.IsClickAble(dragOrigin)) return;
		
		// Get Viewport Position Difference between Last and Current Touches
        if (Input.GetMouseButton(0)) {	// MOUSE
			pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
		} else if (Input.touchCount > 0) { // TOUCH
			touch = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
			pos = Camera.main.ScreenToViewportPoint(touch - dragOrigin);
		}
		
		// CONSTRUCT Camera Movement Vector
        move = Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(
			transform.right.x * -pos.x * dragSpeed * (GetZoom()/startZoom), 
			transform.forward.y * pos.y * dragSpeed * (GetZoom()/startZoom),
			0
		);

        var prevPos = transform.localPosition;
		// Move Camera
        transform.localPosition += move;
        if (!CheckGround())
        {
            transform.localPosition = prevPos;
        }

		
		// RESET START POINT
		dragOrigin = Input.mousePosition;
		
		isDrag = true;
    }
	
	bool RayCast(Vector3 point, out Vector3 res) 
    {
		res = new Vector3();
		
		// Raycast
	    Ray ray = camera.ScreenPointToRay(point);
	    RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, groundMask))
	    {
			res = hit.point;
			return true;
	    }
		
		return false;
	}

    //check if we see something
    bool CheckGround()
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        return Physics.Raycast(ray, float.MaxValue, groundMask);
    }
}