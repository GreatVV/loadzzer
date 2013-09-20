using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class Gamefield : MonoBehaviour {

    public bool createNew = true;

    public GameObject[] ChuzzlePrefabs;   

    public int Width = 7;
    public int Height = 10;

    public List<Chuzzle> chuzzles;

    public LayerMask chuzzleMask;  

    bool isDrag;    
    Chuzzle draggable;
    Vector3 dragOrigin;
    Vector3 delta;
    Vector3 deltaTouch;
    bool directionChozen;
    bool isVerticalDrag;

    public Chuzzle currentChuzzle;
    public List<Chuzzle> selectedChuzzles;
    public List<Chuzzle> animatedChuzzles;

    public bool isMovingToPrevPosition;

    public GameObject portalPrefab;
    public List<Portal> portalsOnMap;

    void Awake()
    {
        var portals = new GameObject("Portals");

        var size = ChuzzlePrefabs[Random.Range(0, ChuzzlePrefabs.Length)].GetComponent<Chuzzle>().spriteScale;
        bool createVerticalPortals = false;
        if (createNew)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    CreateRandomChuzzle(i, j);

                    if (!createVerticalPortals)
                    {
                        var upPortal = (GameObject.Instantiate(portalPrefab) as GameObject).GetComponent<Portal>();
                        upPortal.name = "up" + j;
                        upPortal.thisPosition = upPortal.transform.position = new Vector3(j * size.x, Height * size.y, 0);
                        upPortal.teleportToPosition = new Vector3(j * size.x, 0 * size.y, 0);

                        var bottomPortal = (GameObject.Instantiate(portalPrefab) as GameObject).GetComponent<Portal>();
                        bottomPortal.name = "bottom" + j;
                        bottomPortal.thisPosition = bottomPortal.transform.position = upPortal.teleportToPosition;
                        bottomPortal.teleportToPosition = upPortal.thisPosition;

                        portalsOnMap.Add(upPortal);
                        portalsOnMap.Add(bottomPortal);

                        upPortal.transform.parent = portals.transform;
                        bottomPortal.transform.parent = portals.transform;
                    }
                }
                           
                var leftPortal = (GameObject.Instantiate(portalPrefab) as GameObject).GetComponent<Portal>();
                leftPortal.name = "left"+i;
                leftPortal.thisPosition = leftPortal.transform.position = new Vector3(0, i * size.y, 0);
                leftPortal.teleportToPosition = new Vector3(Width * size.x, i * size.y , 0);

                var rightPortal = (GameObject.Instantiate(portalPrefab) as GameObject).GetComponent<Portal>();
                rightPortal.name = "right"+i;
                rightPortal.thisPosition = rightPortal.transform.position = leftPortal.teleportToPosition;
                rightPortal.teleportToPosition = leftPortal.thisPosition;

                portalsOnMap.Add(leftPortal);
                portalsOnMap.Add(rightPortal);

                leftPortal.transform.parent = portals.transform;
                rightPortal.transform.parent = portals.transform;
            }

            RemoveCombinations(FindCombinations());
        }        
    }

    private Chuzzle CreateRandomChuzzle(int i, int j)
    {
        var prefab = ChuzzlePrefabs[Random.Range(0, ChuzzlePrefabs.Length)];
        var gameObject = NGUITools.AddChild(this.gameObject, prefab);
        gameObject.layer = prefab.layer;
        var chuzzle = gameObject.GetComponent<Chuzzle>();
        chuzzle.realX = chuzzle.moveToX = chuzzle.x = j;
        chuzzle.realY = chuzzle.moveToY = chuzzle.y = i;
        gameObject.transform.localPosition = new Vector3(j * gameObject.GetComponent<Chuzzle>().spriteScale.x, i * gameObject.GetComponent<Chuzzle>().spriteScale.y, 0);

        chuzzles.Add(chuzzle);
        return chuzzle;
    }

    public void CalculateRealCoordinatesFor(Chuzzle chuzzle)
    {
        chuzzle.realX = Mathf.CeilToInt(chuzzle.transform.localPosition.x/chuzzle.spriteScale.x);
        chuzzle.realY = Mathf.CeilToInt(chuzzle.transform.localPosition.y / chuzzle.spriteScale.y);
    }
                            
    void Update()
    {
        isDrag = false;             

        if (isMovingToPrevPosition)
        {         
            return;
        }

        #region Drag

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            dragOrigin = Input.mousePosition;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
             //   Debug.Log("is touch drag started");
                dragOrigin = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }


            Ray ray = Camera.main.ScreenPointToRay(dragOrigin);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, chuzzleMask))
            {
                currentChuzzle = hit.transform.gameObject.GetComponent<Chuzzle>();
            }

            return;
        }                      

        // CHECK DRAG STATE (Mouse or Touch)
        if (!Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            if (0 == Input.touchCount || Input.GetTouch(0).phase != TouchPhase.Moved)
            {
                if (currentChuzzle != null)
                {
                 //   Debug.Log("Drag dropped");
                    DropDrag();
                }
                return;
            }
        }

        if (currentChuzzle == null)
        {
            return;
        }        
  
        
        if (Input.GetMouseButton(0)) // Get Position Difference between Last and Current Touches
        {	// MOUSE
            delta = Input.mousePosition - dragOrigin;

         //   Debug.Log("Drag delta");
        }
        else
        {
            if (Input.touchCount > 0)
            { // TOUCH
                deltaTouch = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
                delta = deltaTouch - dragOrigin;
               // Debug.Log("Drag delta TOUCH");
            }
        }

        if (!directionChozen)
        {
            //chooze drag direction
            if (Mathf.Abs(delta.x) < 1.5 * Mathf.Abs(delta.y) || Mathf.Abs(delta.x) > 1.5 * Mathf.Abs(delta.y))
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
              //  Debug.Log("Direction chozen. Vertical: "+isVerticalDrag);
            }           
        }
        else
        {             

            foreach (var c in selectedChuzzles)
            {
                c.GetComponent<TeleportableEntity>().prevPosition = c.transform.localPosition;
                c.transform.localPosition += isVerticalDrag ? new Vector3(0, delta.y, 0) : new Vector3(delta.x, 0, 0);
            }
        }

        // RESET START POINT
        dragOrigin = Input.mousePosition;

        isDrag = true;

        #endregion 
                   
    }

    void LateUpdate()
    {
        #region Portals

        foreach (var c in selectedChuzzles)
        {
            var teleportable = c.GetComponent<TeleportableEntity>();
            
            if (teleportable.prevPosition != teleportable.transform.localPosition)
            {                             
                var touchTeleport = false;
                var direction = teleportable.transform.localPosition - teleportable.prevPosition;
                foreach (var portal in portalsOnMap)
                {
                    if (direction.x == 0)
                    {
                        if (direction.y > 0)
                        {
                            if (c.collider.bounds.min.x == portal.thisPosition.x &&
                             c.collider.bounds.min.y < portal.thisPosition.y && c.collider.bounds.max.y > portal.thisPosition.y &&
                             Mathf.Abs(c.collider.bounds.min.y - portal.thisPosition.y) > 10 &&
                             Vector3.Distance(teleportable.transform.position, portal.transform.position) < Vector3.Distance(teleportable.prevPosition, portal.transform.position))
                            {
                                //closer

                                if (!teleportable.teleported)
                                {
                                    var diff = portal.thisPosition - c.transform.position;
                                    c.transform.position = portal.teleportToPosition - diff;
                                    teleportable.teleported = true;
                                }
                                touchTeleport = true;
                            }
                            else
                            {
                                //far
                            }
                        }
                        else
                        {
                            if (c.collider.bounds.min.x == portal.thisPosition.x &&
                           c.collider.bounds.min.y < portal.thisPosition.y && c.collider.bounds.max.y > portal.thisPosition.y &&
                           Mathf.Abs(c.collider.bounds.min.y - portal.thisPosition.y) > 10 &&
                           Vector3.Distance(teleportable.transform.position, portal.transform.position) > Vector3.Distance(teleportable.prevPosition, portal.transform.position))
                            {
                                //closer

                                if (!teleportable.teleported)
                                {
                                    var diff = portal.thisPosition - c.transform.position;
                                    c.transform.position = portal.teleportToPosition - diff;
                                    teleportable.teleported = true;
                                }
                                touchTeleport = true;
                            }
                            else
                            {
                                //far
                            }
                        }
                    }
                    else
                    {
                        if (direction.x > 0)
                        {

                            if (c.collider.bounds.min.y == portal.thisPosition.y &&
                                c.collider.bounds.min.x < portal.thisPosition.x && c.collider.bounds.max.x > portal.thisPosition.x &&
                                Mathf.Abs(c.collider.bounds.min.x - portal.thisPosition.x) > 10 &&
                                Vector3.Distance(teleportable.transform.position, portal.transform.position) < Vector3.Distance(teleportable.prevPosition, portal.transform.position))
                            {
                                //closer

                                if (!teleportable.teleported)
                                {
                                    var diff = portal.thisPosition - c.transform.position;
                                    c.transform.position = portal.teleportToPosition - diff;
                                    teleportable.teleported = true;
                                }
                                touchTeleport = true;
                            }
                            else
                            {
                                //far
                            }
                        }
                        else
                        {
                            if (c.collider.bounds.min.y == portal.thisPosition.y &&
                               c.collider.bounds.min.x < portal.thisPosition.x && c.collider.bounds.max.x > portal.thisPosition.x &&
                               Mathf.Abs(c.collider.bounds.min.x - portal.thisPosition.x) > 10 &&
                               Vector3.Distance(teleportable.transform.position, portal.transform.position) > Vector3.Distance(teleportable.prevPosition, portal.transform.position))
                            {
                                //closer

                                if (!teleportable.teleported)
                                {
                                    var diff = portal.thisPosition - c.transform.position;
                                    c.transform.position = portal.teleportToPosition - diff;
                                    teleportable.teleported = true;
                                }
                                touchTeleport = true;
                            }
                            else
                            {
                                //far
                            }
                        }   
                    }
                    //determine horizontal or vertical
                    //teleport changing only this position


                    //if intersects
                    //create a copy
                    //update copy according parent
                    //if parent fully in teleport - remove copy and teleport to position

                }

                if (teleportable.teleported && !touchTeleport)
                {
                    teleportable.teleported = false;
                }
            }            
        }

        #endregion
    }

    void MoveToRealCoordinates()
    {

    }

    private void DropDrag()
    {
        foreach (var c in selectedChuzzles)
        {
            CalculateRealCoordinatesFor(c);
            //move all tiles to new real coordinates
            MoveToRealCoordinates();
        }
        //check new combination
        var combinations = FindCombinations();
        if (combinations.Count() > 0)
        {                                       
            //destroy combination and add new chuzzles
            RemoveCombinations(combinations);            
        }
        else
        {
            if (selectedChuzzles.Count() > 0)
            {
                //if no new combination - move to prevposition
                isMovingToPrevPosition = true;

                foreach (var c in selectedChuzzles)
                {
                    animatedChuzzles.Add(c);
                    var targetPosition = new Vector3(c.x * c.spriteScale.x, c.y * c.spriteScale.y, 0);
                    iTween.MoveTo(c.gameObject, iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f, "oncomplete", "OnCompleteTween", "oncompletetarget", gameObject, "oncompleteparams", c));
                }
                selectedChuzzles.Clear();
            }
        }                                                 
    }           

    void OnCompleteTween(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;
        chuzzle.realY = chuzzle.y = chuzzle.moveToY;
        chuzzle.realX = chuzzle.x = chuzzle.moveToX;        
      
        if (animatedChuzzles.Contains(chuzzle))
        {
            chuzzle.GetComponent<TeleportableEntity>().prevPosition = chuzzle.transform.localPosition;
            animatedChuzzles.Remove(chuzzle);
        }

        if (animatedChuzzles.Count() == 0)
        {
            isMovingToPrevPosition = false;
            currentChuzzle = null;
            directionChozen = false;

            RemoveCombinations(FindCombinations());
        }                           
        
    }

    public List<List<Chuzzle>> FindCombinations()
    {
        var combinations = new List<List<Chuzzle>>();

        //find combination
        foreach (var c in chuzzles)
        {
            if (!c.isCheckedForSearch)
            {                   
                var combination = RecursiveFind(c, new List<Chuzzle>());
              
                if (combination.Count() >= 3)
                {
                    combinations.Add(combination);
                }
            }
        }
        /*
        Debug.Log("Num of combs: " + combinations.Count());
        foreach(var com in combinations)
        {
            Debug.Log("Num of tiles: " + com.Count());
        }
                             */

        foreach (var c in chuzzles)
        {
            c.isCheckedForSearch = false;
        }

        return combinations;
    }

    public Chuzzle At(int x, int y)
    {
        return chuzzles.FirstOrDefault(c => c.x == x && c.y == y);
    }

    public void MoveTilesForDeath(List<List<Chuzzle>> combinations)
    {
        foreach (var c in chuzzles)
        {
            c.moveToX = c.x;
            c.moveToY = c.y;
        }

        var newTilesInColumns = new int[Width];

        //move animations to empty place
        foreach (var combination in combinations)
        {
          //  Debug.Log("Combination");
            foreach (var chuzzle in combination)
            {  
                var upToChuzzle = GetTopFor(chuzzle);
                while (upToChuzzle != null)
                {
                    upToChuzzle.moveToY--;
                    upToChuzzle = GetTopFor(upToChuzzle);
                }

                // Debug.Log("c: " + chuzzle.x + ":" + chuzzle.y + ":" + newTilesInColumns[chuzzle.x]);
                var newChuzzle = CreateRandomChuzzle(Height + newTilesInColumns[chuzzle.x], chuzzle.x);
                newChuzzle.moveToY = newChuzzle.moveToY - newTilesInColumns[chuzzle.x] - 1;
                newTilesInColumns[chuzzle.x]++;
            }
        }
        /*
        for (var i = 0; i < newTilesInColumns.Length; i++ )
        {
            Debug.Log("In column " + i + " are " + newTilesInColumns[i] + " new chuzzles");
        }                                      */
    }

    public void OnCompleteDeath(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;
        if (selectedChuzzles.Contains(chuzzle))
        {
            selectedChuzzles.Remove(chuzzle);
        }        

        //if all deleted
        animatedChuzzles.Remove(chuzzle);
        if (animatedChuzzles.Count() == 0)
        {
            //start tweens for new chuzzles
            foreach (var c in chuzzles)
            {
                if (c.moveToY != c.y)
                {
                    isMovingToPrevPosition = true;
                    animatedChuzzles.Add(c);
                    var targetPosition = new Vector3(c.x * c.spriteScale.x, c.moveToY * c.spriteScale.y, 0);
                    iTween.MoveTo(c.gameObject, iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f, "oncomplete", "OnCompleteTween", "oncompletetarget", gameObject, "oncompleteparams", c));
                }
            }
        }

        Destroy(chuzzle.gameObject);
    }

    public void RemoveCombinations(List<List<Chuzzle>> combinations)
    {
        MoveTilesForDeath(combinations);        

        //remove combinations
        foreach(var combination in combinations)
        {
            foreach(var chuzzle in combination)
            {                   
                chuzzles.Remove(chuzzle);
                animatedChuzzles.Add(chuzzle);
                iTween.MoveTo(chuzzle.gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "time", 0.3f));                                               
                iTween.ScaleTo(chuzzle.gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "time", 0.3f, "oncomplete", "OnCompleteDeath", "oncompletetarget", gameObject, "oncompleteparams", chuzzle));                                               
            }
        }

    }

    public Chuzzle GetLeftFor(Chuzzle c)
    {
        return chuzzles.FirstOrDefault(x => x.realX == c.realX - 1 && x.realY == c.realY);
    }

    public Chuzzle GetRightFor(Chuzzle c)
    {
        return chuzzles.FirstOrDefault(x => x.realX == c.realX + 1 && x.realY == c.realY);
    }

    public Chuzzle GetTopFor(Chuzzle c)
    {
        return chuzzles.FirstOrDefault(x => x.realX == c.realX && x.realY == c.realY + 1);      
    }

    public Chuzzle GetBottomFor(Chuzzle c)
    {
        return chuzzles.FirstOrDefault(x => x.realX == c.realX && x.realY == c.realY - 1);
    }

    public List<Chuzzle> RecursiveFind(Chuzzle chuzzle, List<Chuzzle> combination)
    {
        if (chuzzle == null || combination.Contains(chuzzle) || chuzzle.isCheckedForSearch)
        {
            return new List<Chuzzle>();
        }        
        combination.Add(chuzzle);
        chuzzle.isCheckedForSearch = true;

        var left = GetLeftFor(chuzzle);
        if (left!= null && left.Type == chuzzle.Type)
        {
            var answer = RecursiveFind(left, combination);
            foreach(var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        var right = GetRightFor(chuzzle);
        if (right!=null && chuzzle.Type == right.Type)
        {
            var answer = RecursiveFind(right, combination);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        var top = GetTopFor(chuzzle);
        if (top != null && chuzzle.Type == top.Type)
        {
            var answer = RecursiveFind(top, combination);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        var bottom = GetBottomFor(chuzzle);
        if (bottom!=null && chuzzle.Type == bottom.Type)
        {
            var answer = RecursiveFind(bottom, combination);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        return combination;
    }        
}
