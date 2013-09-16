using UnityEngine;
using System.Collections;

public class CheckButtons : MonoBehaviour {

    public LayerMask buttonsMask;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        var levelName = string.Empty;       
        if (Input.GetMouseButtonDown(0) && RayCast(Input.mousePosition, out levelName))
        {
            Application.LoadLevel(levelName);
        }
	}

    bool RayCast(Vector3 point, out string levelName)
    {
        levelName = string.Empty;

        // Raycast
        Ray ray = camera.ScreenPointToRay(point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, buttonsMask))
        {            
            levelName = hit.transform.gameObject.GetComponent<LinkToLevel>().levelName;
            Debug.Log("Button clicked: " + levelName);
            return true;
        }

        return false;
    }
}
