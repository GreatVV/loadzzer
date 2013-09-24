using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class Gamefield : MonoBehaviour {

    //idea: сделать телепорты просто как указатель на какую клетку переходить при встрече нахождении на этой клетке       

    public bool createNew = true;

    public GameObject[] ChuzzlePrefabs;   

    public int Width = 7;
    public int Height = 10;

    public List<Chuzzle> chuzzles;

    public LayerMask chuzzleMask;       
   
    Chuzzle draggable;
    Vector3 dragOrigin;
    Vector3 delta;
    Vector3 deltaTouch;
    bool directionChozen;
    bool isVerticalDrag;

    public Chuzzle currentChuzzle;
    public List<Chuzzle> selectedChuzzles;
    public List<Chuzzle> animatedChuzzles;

    public List<PortalBlock> portals = new List<PortalBlock>();

    public bool isMovingToPrevPosition;

    public GameObject portalPrefab;
    public List<Portal> portalsOnMap;

    public Points pointSystem;
    public GameMode gameMode;

    void Awake()
    {
      //  StartGame();
    }

    public void Reset()
    {
        newTilesAnimationChuzzles.Clear();
        deathAnimationChuzzles.Clear();
        animatedChuzzles.Clear();
        selectedChuzzles.Clear();
        currentChuzzle = null;        
        foreach(var portal in portalsOnMap)
        {
            Destroy(portal.gameObject);
        }
        portalsOnMap.Clear();
        directionChozen = false;
        isVerticalDrag = false;
        foreach(var chuzzle in chuzzles)
        {
            Destroy(chuzzle.gameObject);
        }
        chuzzles.Clear();

        pointSystem.Reset();
        gameMode.Reset();
    }

    public void StartGame()
    {              
        var size = ChuzzlePrefabs[Random.Range(0, ChuzzlePrefabs.Length)].GetComponent<Chuzzle>().spriteScale;              
        if (createNew)
        {
            for (int i = 0; i < Height; i++)
            {                   
                for (int j = 0; j < Width; j++)
                {
                    CreateRandomChuzzle(i, j);                    
                }                        

                var leftPortalBlock = new PortalBlock();
                leftPortalBlock.x = -1;
                leftPortalBlock.y = i;
                leftPortalBlock.toX = Width-1;
                leftPortalBlock.toY = i;

                var rightPortalBlock = new PortalBlock();                
                rightPortalBlock.x = Width;
                rightPortalBlock.y = i;
                rightPortalBlock.toX = 0;
                rightPortalBlock.toY = i;

                this.portals.Add(leftPortalBlock);
                this.portals.Add(rightPortalBlock);
            }


            for (int j = 0; j < Width; j++)
            {    
                var upPortalBlock = new PortalBlock();
                upPortalBlock.x = j;
                upPortalBlock.y = Height;
                upPortalBlock.toX = j;
                upPortalBlock.toY = 0;

                var bottomPortalBlock = new PortalBlock();
                bottomPortalBlock.toX = j;
                bottomPortalBlock.toY = Height-1;
                bottomPortalBlock.x = j;
                bottomPortalBlock.y = -1;

                this.portals.Add(upPortalBlock);
                this.portals.Add(bottomPortalBlock);
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
                            
    void Update()
    {
        isMovingToPrevPosition = animatedChuzzles.Any() || deathAnimationChuzzles.Any() || newTilesAnimationChuzzles.Any();
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
        if ((!Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) && 0 == Input.touchCount)
        {
            DropDrag();
            return;
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
        
        #endregion 
                   
    }
    
    void LateUpdate()
    {
        if (selectedChuzzles.Any() && directionChozen)
        {
            foreach (var c in selectedChuzzles)
            {
                var real = ToRealCoordinates(c);
                if (IsPortal((int)real.x, (int)real.y))
                {
                    var difference = c.transform.localPosition - ConvertXYToPosition((int)real.x, (int)real.y, c.spriteScale);

                    var portal = GetPortalAt((int)real.x, (int)real.y);
                    c.transform.localPosition = ConvertXYToPosition(portal.toX, portal.toY, c.spriteScale) + difference;
                }
            }                                   
        }                      
    }
    
    #region Block and Portals

    public bool IsPortal(int x, int y)
    {
        return portals.Any(p => p.x == x && p.y == y);
    }

    public PortalBlock GetPortalAt(int x, int y)
    {
        return portals.First(p => p.x == x && p.y == y);
    }

    public Vector3 ConvertXYToPosition(int x, int y, Vector3 scale)
    {
        return new Vector3(x * scale.x, y * scale.y, 0);
    }

    #endregion

    #region Control

    public Vector2 ToRealCoordinates(Chuzzle chuzzle)
    {
        return new Vector2(Mathf.RoundToInt(chuzzle.transform.localPosition.x / chuzzle.spriteScale.x), Mathf.RoundToInt(chuzzle.transform.localPosition.y / chuzzle.spriteScale.y));
    }

    public void CalculateRealCoordinatesFor(Chuzzle chuzzle)
    {
        chuzzle.realX = Mathf.RoundToInt(chuzzle.transform.localPosition.x / chuzzle.spriteScale.x);
        chuzzle.realY = Mathf.RoundToInt(chuzzle.transform.localPosition.y / chuzzle.spriteScale.y);

        if (IsPortal(chuzzle.realX, chuzzle.realY))
        {
            var difference = chuzzle.transform.localPosition - ConvertXYToPosition(chuzzle.realX, chuzzle.realY, chuzzle.spriteScale);

            var portal = GetPortalAt(chuzzle.realX, chuzzle.realY);
            chuzzle.transform.localPosition = ConvertXYToPosition(portal.toX, portal.toY, chuzzle.spriteScale) + difference;
            chuzzle.realX = portal.toX;
            chuzzle.realY = portal.toY;
        }            
    }

    private void DropDrag()
    {
        foreach (var c in selectedChuzzles)
        {
            CalculateRealCoordinatesFor(c);
        }
        //move all tiles to new real coordinates
        MoveToRealCoordinates();

        directionChozen = false;
        currentChuzzle = null;
        selectedChuzzles.Clear();
    }

    void MoveToRealCoordinates()
    {
        foreach (var c in selectedChuzzles)
        {
            c.moveToX = c.realX;
            c.moveToY = c.realY;
        }

        MoveToTargetPosition(selectedChuzzles, "OnTweenMoveAfterDrag");    
    }                                                                               

    void OnTweenMoveAfterDrag(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        if (animatedChuzzles.Contains(chuzzle))
        {
            animatedChuzzles.Remove(chuzzle);
        }

        if (animatedChuzzles.Count() == 0)
        {
            //check new combination
            var combinations = FindCombinations();
            if (combinations.Count() > 0)
            {
                foreach (var c in chuzzles)
                {
                    c.x = c.realX;
                    c.y = c.realY;
                }
                //destroy combination and add new chuzzles
                RemoveCombinations(combinations);

                gameMode.Action();
            }
            else
            {
                //if no new combination - move to prevposition       
                foreach (var c in chuzzles)
                {   
                    c.moveToX = c.realX = c.x;
                    c.moveToY = c.realY = c.y;                    
                }

                MoveToTargetPosition(chuzzles, "OnCompleteBackToPreviousPositionTween");                
            }
        }
    }

    void OnCompleteBackToPreviousPositionTween(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        if (animatedChuzzles.Contains(chuzzle))
        {
            chuzzle.GetComponent<TeleportableEntity>().prevPosition = chuzzle.transform.localPosition;
            animatedChuzzles.Remove(chuzzle);
        }

        if (animatedChuzzles.Count() == 0)
        {   
            RemoveCombinations(FindCombinations());
        }   
    }

    #endregion

    #region Death

    private List<Chuzzle> deathAnimationChuzzles = new List<Chuzzle>();

    public void CreateNewTilesForDead(List<List<Chuzzle>> combinations)
    {
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

    public void RemoveCombinations(List<List<Chuzzle>> combinations)
    {
        CreateNewTilesForDead(combinations);

        //remove combinations
        foreach (var combination in combinations)
        {
            //count points
            pointSystem.CountForCombinations(combination);
            foreach (var chuzzle in combination)
            {
                //remove chuzzle from game logic
                chuzzles.Remove(chuzzle);                              
                
                iTween.MoveTo(chuzzle.gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "time", 0.3f));
                iTween.ScaleTo(chuzzle.gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "time", 0.3f, "oncomplete", "OnCompleteDeath", "oncompletetarget", gameObject, "oncompleteparams", chuzzle));

                deathAnimationChuzzles.Add(chuzzle);                
            }
        }

    }

    private List<Chuzzle> newTilesAnimationChuzzles = new List<Chuzzle>();

    //on tweener complete
    public void OnCompleteDeath(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;       
        
        deathAnimationChuzzles.Remove(chuzzle);

        //if all deleted
        if (deathAnimationChuzzles.Count() == 0)
        {
            //start tweens for new chuzzles
            foreach (var c in chuzzles)
            {
                if (c.moveToY != c.y)
                {                       
                    newTilesAnimationChuzzles.Add(c);
                    var targetPosition = new Vector3(c.x * c.spriteScale.x, c.moveToY * c.spriteScale.y, 0);
                    iTween.MoveTo(c.gameObject, iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f, "oncomplete", "OnCompleteNewChuzzleTween", "oncompletetarget", gameObject, "oncompleteparams", c));
                }
            }
        }

        if (selectedChuzzles.Contains(chuzzle))
        {
            selectedChuzzles.Remove(chuzzle);
        }

        Destroy(chuzzle.gameObject);
    }

    public void OnCompleteNewChuzzleTween(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;
        chuzzle.realY = chuzzle.y = chuzzle.moveToY;
        chuzzle.realX = chuzzle.x = chuzzle.moveToX;

        if (newTilesAnimationChuzzles.Contains(chuzzle))
        {
            chuzzle.GetComponent<TeleportableEntity>().prevPosition = chuzzle.transform.localPosition;
            newTilesAnimationChuzzles.Remove(chuzzle);
        }

        if (newTilesAnimationChuzzles.Count() == 0)
        {
            var combinations = FindCombinations();
            if (combinations.Count > 0)
            {
                RemoveCombinations(combinations);
            }
            else
            {
                //check gameover or win
                gameMode.Check();
            }
        }        
    }

    #endregion              

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

    public void MoveToTargetPosition(List<Chuzzle> targetChuzzles, string callbackOnComplete)
    {
        foreach (var c in targetChuzzles)
        {
            var targetPosition = new Vector3(c.moveToX * c.spriteScale.x, c.moveToY * c.spriteScale.y, 0);
            if (Vector3.Distance(c.transform.localPosition, targetPosition) > 0.1f)
            {
                animatedChuzzles.Add(c);                                                                                  
                iTween.MoveTo(c.gameObject, iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f, "oncomplete", callbackOnComplete, "oncompletetarget", gameObject, "oncompleteparams", c));
            }
            else
            {
                c.transform.localPosition = targetPosition;
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
