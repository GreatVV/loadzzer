using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class Gamefield : MonoBehaviour
{
    #region Direction enum

    public enum Direction
    {
        ToLeft,
        ToRight,
        ToTop,
        ToBottom
    };

    #endregion

    #region Set in editor

    public GameObject Explosion;
    public GameObject PortalPrefab;

    public LayerMask ChuzzleMask;

    #endregion

    public Level Level = new Level();

    public SpecialCreationUtility SpecialCreation;
    
    public GameMode GameMode = GameModeFactory.CreateGameMode(GameModeDescription.CreateFromJson(null));
    
    public Points PointSystem = new Points();
    
    public SerializedLevel LastLoadedLevel = null;

    public List<Chuzzle> AnimatedChuzzles = new List<Chuzzle>();

    public Chuzzle CurrentChuzzle;
    public Direction CurrentDirection;
    public List<Chuzzle> DeathAnimationChuzzles = new List<Chuzzle>();

    public bool IsMovingToPrevPosition;

    public List<Chuzzle> NewTilesAnimationChuzzles = new List<Chuzzle>();
    public int[] NewTilesInColumns = new int[0];

    public List<Chuzzle> SelectedChuzzles = new List<Chuzzle>();
    

    private Vector3 _delta;
    private Vector3 _deltaTouch;
    private bool _directionChozen;
    private Vector3 _dragOrigin;
    private Chuzzle _draggable;
    private bool _isVerticalDrag;

    #region Events

    public event Action<List<Chuzzle>> CombinationDestroyed;

    public event Action GameStarted;

    public event Action<Chuzzle> TileDestroyed;

    #endregion

    #region Event Invokators

    protected virtual void InvokeCombinationDestroyed(List<Chuzzle> combination)
    {
        var handler = CombinationDestroyed;
        if (handler != null) handler(combination);
    }

    protected virtual void InvokeGameStarted()
    {
        var handler = GameStarted;
        if (handler != null) handler();
    }

    private void InvokeTileDestroyed(Chuzzle destroyedChuzzle)
    {
        if (TileDestroyed != null)
        {
            TileDestroyed(destroyedChuzzle);
        }
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
        var combinations = GamefieldUtility.FindCombinations(Level.Chuzzles);
        if (combinations.Any())
        {
            foreach (var c in Level.Chuzzles)
            {
                c.Current = c.Real;
            }
            //check can we make new specialTiles
            var newSpecials = SpecialCreation.CheckForSpecial(combinations);

            var animationForSpecials = newSpecials;
            if (!newSpecials)
            {
                animationForSpecials = SpecialCreation.KillSpecials(combinations);
            }

            //if there is no specials
            if (!animationForSpecials)
            {
                //remove combinations
                RemoveCombinations(combinations);
            }

            if (isHumanAction)
            {
                GameMode.HumanTurn();
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

    public Chuzzle At(int x, int y)
    {
        return Level.Chuzzles.FirstOrDefault(c => c.Current.x == x && c.Current.y == y);
    }

    private void LateUpdate()
    {
        if (SelectedChuzzles.Any() && _directionChozen)
        {
            foreach (var c in SelectedChuzzles)
            {
                c.GetComponent<TeleportableEntity>().prevPosition = c.transform.localPosition;
                c.transform.localPosition += _isVerticalDrag ? new Vector3(0, _delta.y, 0) : new Vector3(_delta.x, 0, 0);

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
                        switch (CurrentDirection)
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

    public bool MoveToTargetPosition(List<Chuzzle> targetChuzzles, string callbackOnComplete)
    {
        var isAnyTween = false;
        foreach (var c in targetChuzzles)
        {
            var targetPosition = new Vector3(c.MoveTo.x*c.Scale.x, c.MoveTo.y*c.Scale.y, 0);
            if (Vector3.Distance(c.transform.localPosition, targetPosition) > 0.1f)
            {
                isAnyTween = true;
                AnimatedChuzzles.Add(c);
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

    public void StartGame(SerializedLevel level = null)
    {
        LastLoadedLevel = level;

        NewTilesAnimationChuzzles.Clear();

        DeathAnimationChuzzles.Clear();

        AnimatedChuzzles.Clear();

        SelectedChuzzles.Clear();

        CurrentChuzzle = null;

        SpecialCreation.SpecialTilesAnimated.Clear();

        _directionChozen = false;

        _isVerticalDrag = false;

        PointSystem.Reset();

        Level.Reset();

        if (level == null)
        {
            Level.InitRandom();
            GameMode = GameModeFactory.CreateGameMode(GameModeDescription.CreateFromJson(null));
        }
        else
        {
            Level.InitFromFile(level);
        }

        NewTilesInColumns = new int[Level.Width];

        InvokeGameStarted();

        AnalyzeField(false);
    }

    private void Update()
    {
        if (LastLoadedLevel == null)
        {
            Debug.Log("Cry");
            return;
        }

        IsMovingToPrevPosition = AnimatedChuzzles.Any() || DeathAnimationChuzzles.Any() ||
                                 NewTilesAnimationChuzzles.Any() || SpecialCreation.SpecialTilesAnimated.Any();
        if (IsMovingToPrevPosition)
        {
            return;
        }

        #region Drag

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            _dragOrigin = Input.mousePosition;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //   Debug.Log("is touch drag started");
                _dragOrigin = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }


            var ray = Camera.main.ScreenPointToRay(_dragOrigin);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, ChuzzleMask))
            {
                CurrentChuzzle = hit.transform.gameObject.GetComponent<Chuzzle>();
            }

            return;
        }

        // CHECK DRAG STATE (Mouse or Touch)
        if ((!Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) && 0 == Input.touchCount)
        {
            DropDrag();
            return;
        }

        if (CurrentChuzzle == null)
        {
            return;
        }


        if (Input.GetMouseButton(0)) // Get Position Difference between Last and Current Touches
        {
            // MOUSE
            _delta = Input.mousePosition - _dragOrigin;

            //   Debug.Log("Drag delta");
        }
        else
        {
            if (Input.touchCount > 0)
            {
                // TOUCH
                _deltaTouch = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
                _delta = _deltaTouch - _dragOrigin;
                // Debug.Log("Drag delta TOUCH");
            }
        }

        if (!_directionChozen)
        {
            //chooze drag direction
            if (Mathf.Abs(_delta.x) < 1.5*Mathf.Abs(_delta.y) || Mathf.Abs(_delta.x) > 1.5*Mathf.Abs(_delta.y))
            {
                if (Mathf.Abs(_delta.x) < Mathf.Abs(_delta.y))
                {
                    //TODO: choose row
                    SelectedChuzzles = Level.Chuzzles.Where(x => x.Current.x == CurrentChuzzle.Current.x).ToList();
                    _isVerticalDrag = true;
                }
                else
                {
                    //TODO: choose column
                    SelectedChuzzles = Level.Chuzzles.Where(x => x.Current.y == CurrentChuzzle.Current.y).ToList();
                    _isVerticalDrag = false;
                }

                _directionChozen = true;
                //  Debug.Log("Direction chozen. Vertical: "+isVerticalDrag);
            }
        }

        if (_directionChozen)
        {
            if (_isVerticalDrag)
            {
                if (_delta.y > 0)
                {
                    CurrentDirection = Direction.ToTop;
                }
                else
                {
                    CurrentDirection = Direction.ToBottom;
                }
            }
            else
            {
                if (_delta.x > 0)
                {
                    CurrentDirection = Direction.ToLeft;
                }
                else
                {
                    CurrentDirection = Direction.ToRight;
                }
            }
        }

        // RESET START POINT
        _dragOrigin = Input.mousePosition;

        #endregion
    }

    #region Control

    #region Event Handlers

    private void OnCompleteBackToPreviousPositionTween(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        if (AnimatedChuzzles.Contains(chuzzle))
        {
            chuzzle.GetComponent<TeleportableEntity>().prevPosition = chuzzle.transform.localPosition;
            AnimatedChuzzles.Remove(chuzzle);
        }
    }

    private void OnTweenMoveAfterDrag(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        if (AnimatedChuzzles.Contains(chuzzle))
        {
            AnimatedChuzzles.Remove(chuzzle);
        }

        if (AnimatedChuzzles.Count() == 0)
        {
            AnalyzeField(true);
        }
    }

    #endregion

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
        foreach (var c in SelectedChuzzles)
        {
            CalculateRealCoordinatesFor(c);
        }
        //move all tiles to new real coordinates
        MoveToRealCoordinates();

        _directionChozen = false;
        CurrentChuzzle = null;
        SelectedChuzzles.Clear();
    }

    private void MoveToRealCoordinates()
    {
        foreach (var c in SelectedChuzzles)
        {
            c.MoveTo = c.Real;
        }

        var anyMove = MoveToTargetPosition(SelectedChuzzles, "OnTweenMoveAfterDrag");

        if (!anyMove)
        {
            AnalyzeField(true);
        }
    }

    public IntVector2 ToRealCoordinates(Chuzzle chuzzle)
    {
        return new IntVector2(Mathf.RoundToInt(chuzzle.transform.localPosition.x/chuzzle.Scale.x),
            Mathf.RoundToInt(chuzzle.transform.localPosition.y/chuzzle.Scale.y));
    }

    #endregion

    #region Special Chuzzles

    private void OnCreateLineTweenComplete(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        SpecialCreation.SpecialTilesAnimated.Remove(chuzzle);
        Destroy(chuzzle.gameObject);

        if (SpecialCreation.SpecialTilesAnimated.Count == 0)
        {
            MoveTilesWhoNeedMoves();
        }
    }

    #endregion

    #region New Tiles

    #region Event Handlers

    public void OnCompleteNewChuzzleTween(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;
        chuzzle.Real = chuzzle.Current = chuzzle.MoveTo;

        if (NewTilesAnimationChuzzles.Contains(chuzzle))
        {
            chuzzle.GetComponent<TeleportableEntity>().prevPosition = chuzzle.transform.localPosition;
            NewTilesAnimationChuzzles.Remove(chuzzle);
        }

        if (!NewTilesAnimationChuzzles.Any())
        {
            var combinations = GamefieldUtility.FindCombinations(Level.Chuzzles);
            if (combinations.Count > 0)
            {
                AnalyzeField(false);
            }
            else
            {
                //check gameover or win
                GameMode.Check();
            }
        }
    }

    #endregion

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

    public void MoveTilesWhoNeedMoves()
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

    #endregion

    #region Death

    #region Event Handlers

    public void OnCompleteDeath(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        DeathAnimationChuzzles.Remove(chuzzle);

        //if all deleted
        if (!DeathAnimationChuzzles.Any())
        {
            //start tweens for new chuzzles
            MoveTilesWhoNeedMoves();
        }
        Destroy(chuzzle.gameObject);
    }

    #endregion

    public void RemoveCombinations(List<List<Chuzzle>> combinations)
    {
        //remove combinations
        foreach (var combination in combinations)
        {
            RemoveTiles(combination, true);
        }
    }

    public void RemoveTiles(List<Chuzzle> combination, bool needCountPoints)
    {
        InvokeCombinationDestroyed(combination);
        if (needCountPoints)
        {
            //count points
            PointSystem.CountForCombinations(combination);
        }
        foreach (var chuzzle in combination)
        {
            if (chuzzle.Counter > 0)
            {
                continue;
            }
            //remove chuzzle from game logic
            RemoveChuzzle(chuzzle);
            Debug.Log("1");
            var explosion = Instantiate(Explosion, chuzzle.transform.position, Quaternion.identity);
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

    #endregion
}