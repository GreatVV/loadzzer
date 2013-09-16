using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class Gamefield : MonoBehaviour {

    public GameObject ChuzzlePrefab;
    public Vector3 ChuzzleScale = new Vector3(1,1,1);

    public int Width = 7;
    public int Height = 10;

    public List<Chuzzle> chuzzles;

    public LayerMask chuzzleMask;

    void Awake()
    {
        for (int i = 0; i < Height; i++)
        {           
            for (int j = 0; j < Width; j++)
            {
                var gameObject = NGUITools.AddChild(this.gameObject, ChuzzlePrefab);
                gameObject.layer = ChuzzlePrefab.layer;
                var chuzzle = gameObject.GetComponent<Chuzzle>();
                chuzzle.x = j;
                chuzzle.y = i;
                gameObject.transform.localScale = ChuzzleScale;
                gameObject.transform.localPosition = new Vector3(j * ChuzzleScale.x, i*ChuzzleScale.y, 0);
                gameObject.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));

                chuzzles.Add(chuzzle);
            }
        }
    }

    bool isDrag;    
    Chuzzle draggable;
    Vector3 dragOrigin;
    Vector3 delta;
    Vector3 deltaTouch;
    bool directionChozen;
    bool isVerticalDrag;

    public Chuzzle currentChuzzle;
    public List<Chuzzle> selectedChuzzles;

    void Update()
    {
        isDrag = false;

        // MOUSE WAS DOWN
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(dragOrigin);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, chuzzleMask))
            {
                currentChuzzle = hit.transform.gameObject.GetComponent<Chuzzle>();
            }

            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            DropDrag();
            return;
        }

        if (currentChuzzle == null)
        {
            return;
        }

        // TOUCH WAS DOWN
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            dragOrigin = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
            return;
        }

        // CHECK DRAG STATE (Mouse or Touch)
        if (!Input.GetMouseButton(0))
            if (0 == Input.touchCount || Input.GetTouch(0).phase != TouchPhase.Moved)
            {
                DropDrag();
                return;
            }
  
        // Get Viewport Position Difference between Last and Current Touches
        if (Input.GetMouseButton(0))
        {	// MOUSE
            delta = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        }
        else
        {
            if (Input.touchCount > 0)
            { // TOUCH
                deltaTouch = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
                delta = Camera.main.ScreenToViewportPoint(deltaTouch - dragOrigin);
            }
        }

        if (!directionChozen)
        {
            if (Mathf.Abs(delta.x) < 3 * Mathf.Abs(delta.y) || Mathf.Abs(delta.x) > 3 * Mathf.Abs(delta.y))
            {
                if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y))
                {
                    //TODO: choose row
                    selectedChuzzles = chuzzles.Where(x => x.x == currentChuzzle.x).ToList();
                    isVerticalDrag = true;
                }
                else
                {
                    //TODO: choose column
                    selectedChuzzles = chuzzles.Where(x => x.y == currentChuzzle.y).ToList();
                    isVerticalDrag = false;
                }

                directionChozen = true;
            }           
        }
        else
        {
            foreach (var c in selectedChuzzles)
            {
                c.transform.localPosition += isVerticalDrag ? new Vector3(0, delta.y, 0) * 10 : new Vector3(delta.x, 0, 0) * 10;
            }
        }

        // RESET START POINT
        dragOrigin = Input.mousePosition;

        isDrag = true;
    }

    private void DropDrag()
    {
        selectedChuzzles.Clear();
        currentChuzzle = null;
        directionChozen = false;
    }
}
