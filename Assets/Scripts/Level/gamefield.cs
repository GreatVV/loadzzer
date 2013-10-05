using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Gamefield : MonoBehaviour
{
    public enum Direction
    {
        ToLeft,
        ToRight,
        ToTop,
        ToBottom
    };

    public Level Level;
    public List<Pair> PowerTypePrefabs;
    public List<Chuzzle> animatedChuzzles;

    public LayerMask chuzzleMask;
    public Chuzzle currentChuzzle;
    public Direction currentDirection;

    private Vector3 delta;
    private Vector3 deltaTouch;
    private bool directionChozen;
    private Vector3 dragOrigin;
    private Chuzzle draggable;
    public GameMode gameMode;
    public bool isMovingToPrevPosition;
    private bool isVerticalDrag;
    public Points pointSystem;
    public GameObject portalPrefab;
    public List<Chuzzle> selectedChuzzles;

    public event Action<List<Chuzzle>> CombinationDestroyed;

    public event Action GameStarted;

    protected virtual void InvokeGameStarted()
    {
        Action handler = GameStarted;
        if (handler != null) handler();
    }

    protected virtual void InvokeCombinationDestroyed(List<Chuzzle> combination)
    {
        var handler = CombinationDestroyed;
        if (handler != null) handler(combination);
    }

    public event Action<Chuzzle> TileDestroyed;

    private void InvokeTileDestroyed(Chuzzle destroyedChuzzle)
    {
        if (TileDestroyed != null)
        {
            TileDestroyed(destroyedChuzzle);
        }
    }

    public SerializedLevel LastSerializedLevel = null;

    public void StartGame(SerializedLevel level = null)
    {
        LastSerializedLevel = level;
        NewTilesAnimationChuzzles.Clear();
        DeathAnimationChuzzles.Clear();
        animatedChuzzles.Clear();
        selectedChuzzles.Clear();
        currentChuzzle = null;

        directionChozen = false;
        isVerticalDrag = false;
        pointSystem.Reset();

        Level.Reset();

        if (level == null)
        {
            Level.InitRandom();
            gameMode = GameModeFactory.CreateGameMode( GameModeDescription.CreateFromJson(null));
        }
        else
        {
            Level.InitFromFile(level);
        }
        NewTilesInColumns = new int[Level.Width];
        gameMode.Init(this);

        InvokeGameStarted();
        
        AnalyzeField(false);
    }

    private void Update()
    {
        isMovingToPrevPosition = animatedChuzzles.Any() || DeathAnimationChuzzles.Any() ||
                                 NewTilesAnimationChuzzles.Any() || SpecialTilesAnimated.Any();
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


            var ray = Camera.main.ScreenPointToRay(dragOrigin);
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
        {
            // MOUSE
            delta = Input.mousePosition - dragOrigin;

            //   Debug.Log("Drag delta");
        }
        else
        {
            if (Input.touchCount > 0)
            {
                // TOUCH
                deltaTouch = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
                delta = deltaTouch - dragOrigin;
                // Debug.Log("Drag delta TOUCH");
            }
        }

        if (!directionChozen)
        {
            //chooze drag direction
            if (Mathf.Abs(delta.x) < 1.5*Mathf.Abs(delta.y) || Mathf.Abs(delta.x) > 1.5*Mathf.Abs(delta.y))
            {
                if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y))
                {
                    //TODO: choose row
                    selectedChuzzles = Level.Chuzzles.Where(x => x.Current.x == currentChuzzle.Current.x).ToList();
                    isVerticalDrag = true;
                }
                else
                {
                    //TODO: choose column
                    selectedChuzzles = Level.Chuzzles.Where(x => x.Current.y == currentChuzzle.Current.y).ToList();
                    isVerticalDrag = false;
                }

                directionChozen = true;
                //  Debug.Log("Direction chozen. Vertical: "+isVerticalDrag);
            }
        }

        if (directionChozen)
        {
            if (isVerticalDrag)
            {
                if (delta.y > 0)
                {
                    currentDirection = Direction.ToTop;
                }
                else
                {
                    currentDirection = Direction.ToBottom;
                }
            }
            else
            {
                if (delta.x > 0)
                {
                    currentDirection = Direction.ToLeft;
                }
                else
                {
                    currentDirection = Direction.ToRight;
                }
            }
        }

        // RESET START POINT
        dragOrigin = Input.mousePosition;

        #endregion
    }

    private void LateUpdate()
    {
        if (selectedChuzzles.Any() && directionChozen)
        {
            foreach (var c in selectedChuzzles)
            {
                c.GetComponent<TeleportableEntity>().prevPosition = c.transform.localPosition;
                c.transform.localPosition += isVerticalDrag ? new Vector3(0, delta.y, 0) : new Vector3(delta.x, 0, 0);

                var real = ToRealCoordinates(c);
                if (Level.IsPortal(real.x, real.y))
                {
                    var difference = c.transform.localPosition - Level.ConvertXYToPosition(real.x, real.y, c.Scale);

                    var portal = Level.GetPortalAt(real.x, real.y);
                    c.transform.localPosition = Level.ConvertXYToPosition(portal.toX, portal.toY, c.Scale) + difference;
                }
                else
                {
                    if (Level.GetCellAt(real.x, real.y).Type == CellTypes.Block)
                    {
                        var currentCell = Level.GetCellAt(real.x, real.y);
                       // Debug.Log("Teleport from " + currentCell);
                        Cell targetCell = null;
                        switch (currentDirection)
                        {
                            case Direction.ToRight:
                                targetCell = currentCell.GetLeftWithType();
                           //     Debug.Log("To Right");
                                if (targetCell == null)
                                {
                                    targetCell = Level.GetCellAt(Level.Width - 1, currentCell.y);
                                    if (targetCell.Type == CellTypes.Block)
                                    {
                                        targetCell = targetCell.GetLeftWithType();
                                    }
                                }
                                break;
                            case Direction.ToLeft:
                                targetCell = currentCell.GetRightWithType();
                             //   Debug.Log("To Left");
                                if (targetCell == null)
                                {
                                    targetCell = Level.GetCellAt(0, currentCell.y);
                                    if (targetCell.Type == CellTypes.Block)
                                    {
                                        targetCell = targetCell.GetRightWithType();
                                    }
                                }
                                break;
                            case Direction.ToTop:
                                targetCell = currentCell.GetTopWithType();
                                if (targetCell == null)
                                {
                                    targetCell = Level.GetCellAt(currentCell.x, 0);
                                    if (targetCell.Type == CellTypes.Block)
                                    {
                                        targetCell = targetCell.GetTopWithType();
                                    }
                                }
                                break;
                            case Direction.ToBottom:
                                targetCell = currentCell.GetBottomWithType();
                                if (targetCell == null)
                                {
                                    targetCell = Level.GetCellAt(currentCell.x, Level.Height - 1);
                                    if (targetCell.Type == CellTypes.Block)
                                    {
                                        targetCell = targetCell.GetBottomWithType();
                                    }
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("Current direction can not be shit");
                        }
                      //  Debug.Log("Teleport to " + targetCell);

                        var difference = c.transform.localPosition - Level.ConvertXYToPosition(real.x, real.y, c.Scale);
                        c.transform.localPosition = Level.ConvertXYToPosition(targetCell.x, targetCell.y, c.Scale) +
                                                    difference;
                    }
                }
            }
        }
    }

    private void AnalyzeField(bool isHumanAction)
    {
        var hasNewTiles = CreateNew();
        if (hasNewTiles)
        {
            return;
        }

        //check new combination
        var combinations = FindCombinations();
        if (combinations.Any())
        {
            foreach (var c in Level.Chuzzles)
            {
                c.Current = c.Real;
            }
            //check can we make new specialTiles
            var newSpecials = CheckForSpecial(combinations);

            var animationForSpecials = newSpecials;
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
                gameMode.HumanTurn();
            }
        }
        else
        {
            //if no new combination - move to prevposition       
            foreach (var c in Level.Chuzzles)
            {
                c.MoveTo = c.Real = c.Current;
            }

            MoveToTargetPosition(Level.Chuzzles, "OnCompleteBackToPreviousPositionTween");
        }
    }

    public List<List<Chuzzle>> FindCombinations()
    {
        var combinations = new List<List<Chuzzle>>();

        //find combination
        foreach (var c in Level.Chuzzles)
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

        foreach (var c in Level.Chuzzles)
        {
            c.isCheckedForSearch = false;
        }

        return combinations;
    }

    public Chuzzle At(int x, int y)
    {
        return Level.Chuzzles.FirstOrDefault(c => c.Current.x == x && c.Current.y == y);
    }

    public bool MoveToTargetPosition(List<Chuzzle> targetChuzzles, string callbackOnComplete)
    {
        var isAnyTween = false;
        foreach (var c in targetChuzzles)
        {
            var targetPosition = new Vector3(c.MoveTo.x*c.Scale.x, c.MoveTo.y*c.Scale.y, 0);
            if (Vector3.Distance(c.transform.localPosition, targetPosition) > 0.1f)
            {
                isAnyTween = true;
                animatedChuzzles.Add(c);
                iTween.MoveTo(c.gameObject,
                    iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f,
                        "oncomplete", callbackOnComplete, "oncompletetarget", gameObject, "oncompleteparams", c));
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
        return Level.Chuzzles.FirstOrDefault(x => x.Real == c.Real.Left);
    }

    public Chuzzle GetRightFor(Chuzzle c)
    {
        return Level.Chuzzles.FirstOrDefault(x => x.Real == c.Real.Right);
    }

    public Chuzzle GetTopFor(Chuzzle c)
    {
        return Level.Chuzzles.FirstOrDefault(x => x.Real == c.Real.Top);
    }

    public Chuzzle GetBottomFor(Chuzzle c)
    {
        return Level.Chuzzles.FirstOrDefault(x => x.Real == c.Real.Bottom);
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
        if (left != null && left.Type == chuzzle.Type)
        {
            var answer = RecursiveFind(left, combination);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        var right = GetRightFor(chuzzle);
        if (right != null && chuzzle.Type == right.Type)
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
        if (bottom != null && chuzzle.Type == bottom.Type)
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
    ///     Remove chuzzle from game logic and add new tiles in column
    /// </summary>
    /// <param name="chuzzle">Chuzzle to remove</param>
    /// <param name="invokeEvent">Need to invoke event or not</param>
    public void RemoveChuzzle(Chuzzle chuzzle, bool invokeEvent = true)
    {
        Level.Chuzzles.Remove(chuzzle);
        NewTilesInColumns[chuzzle.Current.x]++;
        if (invokeEvent)
        {
            InvokeTileDestroyed(chuzzle);
        }
    }

    #region Control

    public IntVector2 ToRealCoordinates(Chuzzle chuzzle)
    {
        return new IntVector2(Mathf.RoundToInt(chuzzle.transform.localPosition.x/chuzzle.Scale.x),
            Mathf.RoundToInt(chuzzle.transform.localPosition.y/chuzzle.Scale.y));
    }

    public void CalculateRealCoordinatesFor(Chuzzle chuzzle)
    {
        chuzzle.Real = Level.GetCellAt(Mathf.RoundToInt(chuzzle.transform.localPosition.x/chuzzle.Scale.x),
            Mathf.RoundToInt(chuzzle.transform.localPosition.y/chuzzle.Scale.y));

        if (Level.IsPortal(chuzzle.Real.x, chuzzle.Real.y))
        {
            var difference = chuzzle.transform.localPosition -
                             Level.ConvertXYToPosition(chuzzle.Real.x, chuzzle.Real.y, chuzzle.Scale);

            var portal = Level.GetPortalAt(chuzzle.Real.x, chuzzle.Real.y);
            chuzzle.transform.localPosition = Level.ConvertXYToPosition(portal.toX, portal.toY, chuzzle.Scale) +
                                              difference;
            chuzzle.Real = Level.GetCellAt(portal.toX, portal.toY);
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

    private void MoveToRealCoordinates()
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

    private void OnTweenMoveAfterDrag(Object chuzzleObject)
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

    private void OnCompleteBackToPreviousPositionTween(Object chuzzleObject)
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

    public List<Chuzzle> SpecialTilesAnimated;

    public bool CheckForSpecial(List<List<Chuzzle>> combinations)
    {
        var isNewSpecial = false;

        for (var i = 0; i < combinations.Count; i++)
        {
            var comb = combinations[i];
            //if any tile is powerup - then don't check for new bonuses
            //or any tile has counter
            if (comb.Any(x => x.PowerType != PowerType.Usual) || comb.Any(x=>x.Counter != 0))
            {
                continue;
            }

            if (comb.Count == 4)
            {
                isNewSpecial = CreateLine(comb);
                 
            }
            else
            {
                if (comb.Count >= 5)
                {
                    isNewSpecial = CreateBomb(comb);
                    
                }
            }
        }

        return isNewSpecial;
    }

    private bool CreateBomb(List<Chuzzle> comb)
    {
        return CreateSpecialWithType(comb, PowerType.Bomb);
    }

    private void MoveToSpecialTween(Chuzzle c)
    {
        RemoveChuzzle(c);

        var targetPosition = new Vector3(c.MoveTo.x*c.Scale.x, c.MoveTo.y*c.Scale.y, 0);

        SpecialTilesAnimated.Add(c);
        // iTween.ScaleTo(c.gameObject, Vector3.zero, 1f);
        iTween.MoveTo(c.gameObject,
            iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f, "easetype",
                iTween.EaseType.easeInOutQuad, "oncomplete", "OnCreateLineTweenComplete", "oncompletetarget", gameObject,
                "oncompleteparams", c));
    }

    public bool CreateSpecialWithType(List<Chuzzle> comb,PowerType powerType)
    {
        var ordered = comb.OrderBy(x => x.Current.y).ToList();
        var targetTile = ordered.First();
        var cellForNew = ordered.First().Current;
        for (var i = 1; i < ordered.Count; i++)
        {
            var chuzzle = ordered[i];
            chuzzle.MoveTo = cellForNew;
        }

        var powerUp = PowerTypePrefabs.First(x => x.type == powerType).prefab;
        var powerUpChuzzle = Level.CreateChuzzle(targetTile.Current.x, targetTile.Current.y, powerUp);
        powerUpChuzzle.Type = targetTile.Type;
        powerUpChuzzle.PowerType = powerType;

        var color =
            Instantiate(Level.ChuzzlePrefabs.First(x => x.GetComponent<Chuzzle>().Type == targetTile.Type)) as
                GameObject;
        color.transform.parent = powerUpChuzzle.transform;
        color.transform.localPosition = Vector3.zero;
        Destroy(color.gameObject.GetComponent<Chuzzle>());
        Destroy(color.gameObject.GetComponent<BoxCollider>());
        color.GetComponent<tk2dSprite>().SortingOrder = -1;

        Destroy(targetTile.gameObject);
        Level.Chuzzles.Remove(targetTile);
        ordered.Remove(targetTile);

        foreach (var c in ordered)
        {
            if (c.PowerType == PowerType.Usual)
            {
                MoveToSpecialTween(c);
            }
        }

        return true;
    }

    private bool CreateLine(List<Chuzzle> comb)
    {
        return CreateSpecialWithType(comb, Random.Range(0, 100) > 50 ? PowerType.HorizontalLine : PowerType.VerticalLine);
    }

    private void OnCreateLineTweenComplete(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        SpecialTilesAnimated.Remove(chuzzle);
        Destroy(chuzzle.gameObject);

        if (SpecialTilesAnimated.Count == 0)
        {
            MoveTilesWhoNeedMoves();
        }
    }

    private bool KillSpecials(IEnumerable<List<Chuzzle>> combinations)
    {
        var tilesToKill = new List<Chuzzle>();
        foreach (var combination in combinations)
        {
            foreach (var chuzzle in combination)
            {
                if (chuzzle.PowerType != PowerType.Usual)
                {
                    tilesToKill.AddUniqRange(combination);
                    this.ApplyPowerUp(tilesToKill, chuzzle);
                }
            }
        }
        RemoveTiles(tilesToKill, true);

        return tilesToKill.Any();
    }

    #endregion

    #region New Tiles

    public bool CreateNew()
    {
        var hasNew = NewTilesInColumns.Any(x => x > 0);
        if (!hasNew)
        {
            return false;
        }

        //check if need create new tiles
        for (var x = 0; x < NewTilesInColumns.Length; x++)
        {
            var newInColumn = NewTilesInColumns[x];
            if (newInColumn > 0)
            {
                for (var j = 0; j < newInColumn; j++)
                {
                    //create new tiles
                    Level.CreateRandomChuzzle(x, Level.Height + j);
                }
            }
        }

        //move tiles to fill positions
        for (var x = 0; x < NewTilesInColumns.Length; x++)
        {
            var newInColumn = NewTilesInColumns[x];
            if (newInColumn > 0)
            {
                for (var y = 0; y < Level.Height; y++)
                {
                    var cell = Level.GetCellAt(x, y);
                    if (At(x, y) == null && cell.Type != CellTypes.Block)
                    {
                        while (cell != null)
                        {
                            var chuzzle = At(cell.x, cell.y);
                            if (chuzzle != null)
                            {
                                chuzzle.MoveTo = chuzzle.MoveTo.GetBottomWithType();
                                //Level.GetCellAt(chuzzle.MoveTo.x, chuzzle.MoveTo.y - 1);                                
                            }
                            cell = cell.Top;
                        }
                    }
                }
            }
        }

        NewTilesInColumns = new int[Level.Width];

        MoveTilesWhoNeedMoves();
        return hasNew;
    }

    private void MoveTilesWhoNeedMoves()
    {
        foreach (var c in Level.Chuzzles)
        {
            if (c.MoveTo.y != c.Current.y)
            {
                NewTilesAnimationChuzzles.Add(c);
                var targetPosition = new Vector3(c.Current.x*c.Scale.x, c.MoveTo.y*c.Scale.y, 0);
                iTween.MoveTo(c.gameObject,
                    iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f,
                        "oncomplete", "OnCompleteNewChuzzleTween", "oncompletetarget", gameObject, "oncompleteparams", c));
            }
        }
    }

    public void OnCompleteNewChuzzleTween(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;
        chuzzle.Real = chuzzle.Current = chuzzle.MoveTo;

        if (NewTilesAnimationChuzzles.Contains(chuzzle))
        {
            chuzzle.GetComponent<TeleportableEntity>().prevPosition = chuzzle.transform.localPosition;
            NewTilesAnimationChuzzles.Remove(chuzzle);
        }

        if (NewTilesAnimationChuzzles.Count() == 0)
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

    public GameObject Explosion;
    public List<Chuzzle> DeathAnimationChuzzles = new List<Chuzzle>();
    public List<Chuzzle> NewTilesAnimationChuzzles = new List<Chuzzle>();
    public int[] NewTilesInColumns = new int[0];

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
        InvokeCombinationDestroyed(combination);
        if (needCountPoints)
        {
            //count points
            pointSystem.CountForCombinations(combination);
        }
        foreach (var chuzzle in combination)
        {
            if (chuzzle.Counter > 0)
            {
                continue;
            }
            //remove chuzzle from game logic
            RemoveChuzzle(chuzzle);

            var explosion = Object.Instantiate(Explosion, chuzzle.transform.position, Quaternion.identity);
            Level.ScaleSprite(((GameObject) explosion).GetComponent<tk2dBaseSprite>());
            Destroy(explosion, 1f);
            //iTween.MoveTo(chuzzle.gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "time", 0.3f));
            iTween.ScaleTo(chuzzle.gameObject,
                iTween.Hash("x", 0, "y", 0, "z", 0, "time", 0.3f, "oncomplete", "OnCompleteDeath", "oncompletetarget",
                    gameObject, "oncompleteparams", chuzzle));

            DeathAnimationChuzzles.Add(chuzzle);
        }
    }

    //on tweener complete
    public void OnCompleteDeath(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        DeathAnimationChuzzles.Remove(chuzzle);

        //if all deleted
        if (DeathAnimationChuzzles.Count() == 0)
        {
            //start tweens for new chuzzles
            MoveTilesWhoNeedMoves();
        }
        Destroy(chuzzle.gameObject);
    }

    #endregion
}