using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public static class Extensions
{
    public static void AddUniqRange(this List<Chuzzle> list, IEnumerable<Chuzzle> range)
    {
        foreach (var item in range)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }
}

[Serializable]
public class Pair
{
    [SerializeField]
    public PowerType type;
    [SerializeField]
    public GameObject prefab;
}

public class Gamefield : MonoBehaviour {

    //idea: сделать телепорты просто как указатель на какую клетку переходить при встрече нахождении на этой клетке       

    public bool createNew = true;
    
    public Vector3 ChuzzleSize = new Vector3(80, 80);

    public List<Pair> PowerTypePrefabs;

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

    public List<Cell> cells;

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
        if (createNew)
        {
            newTilesInColumns = new int[Width];
            for (int i = 0; i < Height; i++)
            {                   
                for (int j = 0; j < Width; j++)
                {                       
                    CreateRandomChuzzle(j, i);                    
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
        } 
        else
        {
            var level = new SerializedLevel();
            level.Width = 5;
            level.Height = 5;
            level.TilesInColumn = new int[5] { 5, 5, 4, 5, 5 };
            level.TilesInRow = new int[5] { 5, 5, 4, 5, 5 };
            level.specialCells = new List<Cell>() {               
                  new Cell(2,2,CellTypes.Block)                      
            };
            CreateLevelFrom(level);
        }

        AnalyzeField(false);
    }

    public void CreateLevelFrom(SerializedLevel level)
    {         

    }

    private Chuzzle CreateRandomChuzzle(int x, int y)
    {
        var prefab = ChuzzlePrefabs[Random.Range(0, ChuzzlePrefabs.Length)];
        return CreateChuzzle(x, y, prefab);
    }    

    private Chuzzle CreateChuzzle(int x, int y, GameObject prefab)
    {
        var gameObject = NGUITools.AddChild(this.gameObject, prefab);
        gameObject.layer = prefab.layer;
        (gameObject.collider as BoxCollider).size = ChuzzleSize;
        (gameObject.collider as BoxCollider).center = ChuzzleSize / 2;
        var chuzzle = gameObject.GetComponent<Chuzzle>();
        chuzzle.Real = chuzzle.MoveTo = chuzzle.Current = GetCellAt(x, y);

        gameObject.transform.localPosition = new Vector3(x * gameObject.GetComponent<Chuzzle>().Scale.x, y * gameObject.GetComponent<Chuzzle>().Scale.y, 0);

        chuzzles.Add(chuzzle);
        return chuzzle;
    }
                            
    Cell GetCellAt(int x, int y)
    {            
        var cell = cells.FirstOrDefault(c => c.x == x && c.y == y);
        if (cell == null)
        {
            var newCell = new Cell(x, y);
            cells.Add(newCell);
            //set left
            var left = cells.FirstOrDefault(c => c.x == x - 1 && c.y == y);
            if (left != null)
            {
                newCell.Left = left;
                left.Right = newCell;
            }

            //set right
            var right = cells.FirstOrDefault(c => c.x == x + 1 && c.y == y);
            if (right != null)
            {
                newCell.Right = right;
                right.Left = newCell;
            }

            //set top
            var top = cells.FirstOrDefault(c => c.x == x && c.y == y + 1);
            if (top != null)
            {
                newCell.Top = top;
                top.Bottom = newCell;
            }

            //set bottom
            var bottom = cells.FirstOrDefault(c => c.x == x && c.y == y - 1);
            if (bottom != null)
            {
                newCell.Bottom = bottom;
                bottom.Top = newCell;
            }

            return newCell;
        }
        return cell;
    }

    void Update()
    {
        isMovingToPrevPosition = animatedChuzzles.Any() || deathAnimationChuzzles.Any() || newTilesAnimationChuzzles.Any() || specialTilesAnimated.Any();
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
                    selectedChuzzles = chuzzles.Where(x => x.Current.x == currentChuzzle.Current.x).ToList();
                    isVerticalDrag = true;
                }
                else
                {
                    //TODO: choose column
                    selectedChuzzles = chuzzles.Where(x => x.Current.y == currentChuzzle.Current.y).ToList();
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
                    var difference = c.transform.localPosition - ConvertXYToPosition((int)real.x, (int)real.y, c.Scale);

                    var portal = GetPortalAt((int)real.x, (int)real.y);
                    c.transform.localPosition = ConvertXYToPosition(portal.toX, portal.toY, c.Scale) + difference;
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

    private void AnalyzeField(bool isHumanAction)
    {
        var hasNewTiles = CreateNew();
        if (hasNewTiles)
        {
            return;
        }

        //check new combination
        var combinations = FindCombinations();
        if (combinations.Count() > 0)
        {  
            foreach (var c in chuzzles)
            {
                c.Current = c.Real;
            }
            //check can we make new specialTiles
            bool newSpecials = CheckForSpecial(combinations);

            bool animationForSpecials = newSpecials;
            if (!newSpecials)
            {
                animationForSpecials = KillSpecials(combinations);
            }

            //if there is no specials
            if (!animationForSpecials)
            {   
                //remove combinations
                RemoveCombinations(combinations);
            }

            if (isHumanAction)
            {
                gameMode.Action();
            }
        }
        else
        {
            //if no new combination - move to prevposition       
            foreach (var c in chuzzles)
            {
                c.MoveTo = c.Real = c.Current;
            }

            MoveToTargetPosition(chuzzles, "OnCompleteBackToPreviousPositionTween");
        }
    }

    #region Control

    public IntVector2 ToRealCoordinates(Chuzzle chuzzle)
    {
        return new IntVector2(Mathf.RoundToInt(chuzzle.transform.localPosition.x / chuzzle.Scale.x), Mathf.RoundToInt(chuzzle.transform.localPosition.y / chuzzle.Scale.y));
    }

    public void CalculateRealCoordinatesFor(Chuzzle chuzzle)
    {
        chuzzle.Real = GetCellAt(Mathf.RoundToInt(chuzzle.transform.localPosition.x / chuzzle.Scale.x),Mathf.RoundToInt(chuzzle.transform.localPosition.y / chuzzle.Scale.y));        

        if (IsPortal(chuzzle.Real.x, chuzzle.Real.y))
        {
            var difference = chuzzle.transform.localPosition - ConvertXYToPosition(chuzzle.Real.x, chuzzle.Real.y, chuzzle.Scale);

            var portal = GetPortalAt(chuzzle.Real.x, chuzzle.Real.y);
            chuzzle.transform.localPosition = ConvertXYToPosition(portal.toX, portal.toY, chuzzle.Scale) + difference;
            chuzzle.Real = GetCellAt(portal.toX, portal.toY);            
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
            c.MoveTo = c.Real;            
        }

        var anyMove = MoveToTargetPosition(selectedChuzzles, "OnTweenMoveAfterDrag");            

        if (!anyMove)
        {
            AnalyzeField(true);
        }
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
            AnalyzeField(true);
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
    }

    #endregion

    #region Special Chuzzles

    public List<Chuzzle> specialTilesAnimated;

    public bool CheckForSpecial(List<List<Chuzzle>> combinations)
    {
        bool isNewSpecial = false;

        for (int i = 0; i < combinations.Count; i++ )
        {               
            var comb = combinations[i];
            //if any tile is powerup - then don't check for new bonuses
            if (comb.Any(x=>x.PowerType != PowerType.Usual))
            {
                continue;
            }
            
            if (comb.Count == 4)
            {
                CreateLine(comb);                 
                isNewSpecial = true;
            }
            else
            {
                if (comb.Count >= 5)
                {
                    CreateBomb(comb);                      
                    isNewSpecial = true;
                }
            }
        }

        return isNewSpecial;
    }

    private void CreateBomb(List<Chuzzle> comb)
    {           
        var ordered = comb.OrderBy(x => x.Current.y).ToList();
        var firstTile = ordered.First();
        var cellForNew = ordered.First().Current;        
        for (int i = 1; i < ordered.Count; i++)
        {
            var chuzzle = ordered[i];
            chuzzle.MoveTo = cellForNew;            
        }

        var powerUp = PowerTypePrefabs.First(x => x.type == PowerType.Bomb).prefab;
        var powerUpChuzzle = CreateChuzzle(firstTile.Current.x, firstTile.Current.y, powerUp);
        powerUpChuzzle.Type = firstTile.Type;
        powerUpChuzzle.PowerType = PowerType.Bomb;

        var color = GameObject.Instantiate(ChuzzlePrefabs.First(x => x.GetComponent<Chuzzle>().Type == firstTile.Type)) as GameObject;
        color.transform.parent = powerUpChuzzle.transform;
        color.transform.localPosition = Vector3.zero;
        color.gameObject.GetComponent<Chuzzle>().enabled = false;
        color.gameObject.GetComponent<BoxCollider>().enabled = false;
        color.GetComponent<tk2dSprite>().SortingOrder = -1;

        Destroy(firstTile.gameObject);
        chuzzles.Remove(firstTile);
        ordered.Remove(firstTile);


        foreach (var c in ordered)
        {
            if (c.PowerType == PowerType.Usual)
            {
                MoveToSpecialTween(c);
            }
        }        
    }

    private void MoveToSpecialTween(Chuzzle c)
    {
        RemoveChuzzle(c);        

        var targetPosition = new Vector3(c.MoveTo.x * c.Scale.x, c.MoveTo.y * c.Scale.y, 0);
        
        specialTilesAnimated.Add(c);
        iTween.ScaleTo(c.gameObject, Vector3.zero, 1f);
        iTween.MoveTo(c.gameObject, iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 1f, "easetype", iTween.EaseType.linear, "oncomplete", "OnCreateLineTweenComplete", "oncompletetarget", gameObject, "oncompleteparams", c));

    }

    private void CreateLine(List<Chuzzle> comb)
    {
        var ordered = comb.OrderBy(x => x.Current.y).ToList();
        var firstTile = ordered.First();
        var cellForNew = ordered.First().Current;
        for (int i = 1; i < ordered.Count; i++)
        {
            var chuzzle = ordered[i];
            chuzzle.MoveTo = cellForNew;            
        }

        var targetType = Random.Range(0, 100) > 50 ? PowerType.HorizontalLine : PowerType.VerticalLine;

        var powerUp = PowerTypePrefabs.First(x => x.type == targetType).prefab;
        var powerUpChuzzle = CreateChuzzle(firstTile.Current.x, firstTile.Current.y, powerUp);
        powerUpChuzzle.Type = firstTile.Type;
        powerUpChuzzle.PowerType = targetType;

        var color = GameObject.Instantiate(ChuzzlePrefabs.First(x => x.GetComponent<Chuzzle>().Type == firstTile.Type)) as GameObject;                
        color.transform.parent = powerUpChuzzle.transform;
        color.transform.localPosition = Vector3.zero;
        color.gameObject.GetComponent<Chuzzle>().enabled = false;
        color.gameObject.GetComponent<BoxCollider>().enabled = false;
        color.GetComponent<tk2dSprite>().SortingOrder = -1;

        Destroy(firstTile.gameObject);
        chuzzles.Remove(firstTile);
        ordered.Remove(firstTile);

        foreach (var c in ordered)
        {
            if (c.PowerType == PowerType.Usual)
            {
                MoveToSpecialTween(c);
            }
        }        
    }
    
    private void OnCreateLineTweenComplete(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;
               
        specialTilesAnimated.Remove(chuzzle);
        Destroy(chuzzle.gameObject);

        if (specialTilesAnimated.Count == 0)
        {
            MoveTilesWhoNeedMoves();           
        }
    }

    private bool KillSpecials(List<List<Chuzzle>> combinations)
    {
        var tilesToKill = new List<Chuzzle>();
        foreach(var combination in combinations)
        {
             foreach(var chuzzle in combination)
             {
                 if (chuzzle.PowerType != PowerType.Usual)
                 {
                     tilesToKill.AddUniqRange(combination);
                     this.ApplyPowerUp(tilesToKill, chuzzle);
                 }
             }
        }          
        RemoveTiles(tilesToKill, false);

        return tilesToKill.Any();
    }

    #endregion

    #region New Tiles

    public bool CreateNew()
    {
        var hasNew = newTilesInColumns.Any(x => x > 0);
        if (!hasNew)
        {
            return false;
        }        

        //check if need create new tiles
        for (int x = 0; x < newTilesInColumns.Length; x++)
        {
            var newInColumn = newTilesInColumns[x];
            if (newInColumn > 0)
            {                   
                for (int j = 0; j < newInColumn; j++)
                {
                    //create new tiles
                    CreateRandomChuzzle(x, Height + j);
                }
            }
        }
        
        //move tiles to fill positions
        for (int x = 0; x < newTilesInColumns.Length; x++)
        {
            var newInColumn = newTilesInColumns[x];
            if (newInColumn > 0)
            {
                for (var y = 0; y < Height; y++)
                {
                    if (At(x,y) == null)
                    {
                        var cell = GetCellAt(x, y);                        
                        while (cell != null)
                        {   
                            var chuzzle = At(cell.x, cell.y);
                            if (chuzzle != null)
                            {
                                chuzzle.MoveTo = GetCellAt(chuzzle.MoveTo.x, chuzzle.MoveTo.y - 1);                                
                            }
                            cell = cell.Top;
                        }
                    }
                }
               
            }
        }

        newTilesInColumns = new int[Width];

        MoveTilesWhoNeedMoves();                   
        return hasNew;
    }

    private void MoveTilesWhoNeedMoves()
    {
        foreach (var c in chuzzles)
        {
            if (c.MoveTo.y != c.Current.y)
            {
                newTilesAnimationChuzzles.Add(c);
                var targetPosition = new Vector3(c.Current.x * c.Scale.x, c.MoveTo.y * c.Scale.y, 0);
                iTween.MoveTo(c.gameObject, iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f, "oncomplete", "OnCompleteNewChuzzleTween", "oncompletetarget", gameObject, "oncompleteparams", c));
            }
        }
    }

    public void OnCompleteNewChuzzleTween(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;
        chuzzle.Real = chuzzle.Current = chuzzle.MoveTo;

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
                AnalyzeField(false);
            }
            else
            {
                //check gameover or win
                gameMode.Check();
            }
        }
    }
    #endregion

    #region Death

    private int[] newTilesInColumns = new int[0];
    public List<Chuzzle> deathAnimationChuzzles = new List<Chuzzle>();    

    public void RemoveCombinations(List<List<Chuzzle>> combinations)
    {           
        //remove combinations
        foreach (var combination in combinations)
        {
            RemoveTiles(combination, true);
        }

    }

    private void RemoveTiles(List<Chuzzle> combination, bool needCountPoints)
    {
        if (needCountPoints)
        {
            //count points
            pointSystem.CountForCombinations(combination);
        }
        foreach (var chuzzle in combination)
        {
            //remove chuzzle from game logic
            RemoveChuzzle(chuzzle);             

            iTween.MoveTo(chuzzle.gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "time", 0.3f));
            iTween.ScaleTo(chuzzle.gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "time", 0.3f, "oncomplete", "OnCompleteDeath", "oncompletetarget", gameObject, "oncompleteparams", chuzzle));

            deathAnimationChuzzles.Add(chuzzle);
        }
    }

    public List<Chuzzle> newTilesAnimationChuzzles = new List<Chuzzle>();

    //on tweener complete
    public void OnCompleteDeath(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;       
        
        deathAnimationChuzzles.Remove(chuzzle);

        //if all deleted
        if (deathAnimationChuzzles.Count() == 0)
        {
            //start tweens for new chuzzles
            MoveTilesWhoNeedMoves();
        }     
        Destroy(chuzzle.gameObject);
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

        foreach (var c in chuzzles)
        {
            c.isCheckedForSearch = false;
        }

        return combinations;
    }

    public Chuzzle At(int x, int y)
    {
        return chuzzles.FirstOrDefault(c => c.Current.x == x && c.Current.y == y);
    }         

    public bool MoveToTargetPosition(List<Chuzzle> targetChuzzles, string callbackOnComplete)
    {
        bool isAnyTween = false;
        foreach (var c in targetChuzzles)
        {
            var targetPosition = new Vector3(c.MoveTo.x * c.Scale.x, c.MoveTo.y * c.Scale.y, 0);
            if (Vector3.Distance(c.transform.localPosition, targetPosition) > 0.1f)
            {
                isAnyTween = true;
                animatedChuzzles.Add(c);                                                                                  
                iTween.MoveTo(c.gameObject, iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f, "oncomplete", callbackOnComplete, "oncompletetarget", gameObject, "oncompleteparams", c));
            }
            else
            {
                c.transform.localPosition = targetPosition;
            }

        }

        return isAnyTween;
    }

    public Chuzzle GetLeftFor(Chuzzle c)
    {
        var leftCell = c.Real.Left;
        while (leftCell !=null && leftCell.type == CellTypes.Block)
        {
            leftCell = leftCell.Left;
        }                                                             
        return chuzzles.FirstOrDefault(x=>x.Real == leftCell);
    }

    public Chuzzle GetRightFor(Chuzzle c)
    {
        var right = c.Real.Right;
        while (right != null && right.type == CellTypes.Block)
        {
            right = right.Right;
        }
        return chuzzles.FirstOrDefault(x => x.Real == right);
    }

    public Chuzzle GetTopFor(Chuzzle c)
    {
        var top = c.Real.Top;
        while (top != null && top.type == CellTypes.Block)
        {
            top = top.Top;
        }
        return chuzzles.FirstOrDefault(x => x.Real == top);
    }

    public Chuzzle GetBottomFor(Chuzzle c)
    {
        var bottom = c.Real.Bottom;
        while (bottom != null && bottom.type == CellTypes.Block)
        {
            bottom = bottom.Bottom;
        }
        return chuzzles.FirstOrDefault(x => x.Real == bottom);
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

    /// <summary>
    /// Remove chuzzle from game logic and add new tiles in column
    /// </summary>
    /// <param name="chuzzle"></param>
    public void RemoveChuzzle(Chuzzle chuzzle)
    {
        chuzzles.Remove(chuzzle);
        newTilesInColumns[chuzzle.Current.x]++;
    }
}
