using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class Gamefield : MonoBehaviour
{
    #region Set in editor

    public GameObject Explosion;
    public GameObject PortalPrefab;

    #endregion

    public Level Level = new Level();

    public Field Field = new Field();

    public StageManager StageManager = new StageManager();

    public SpecialCreationUtility SpecialCreation;

    public GameMode GameMode = GameModeFactory.CreateGameMode(GameModeDescription.CreateFromJson(null));

    public Points PointSystem = new Points();

    public SerializedLevel LastLoadedLevel = null;

    public List<Chuzzle> AnimatedChuzzles = new List<Chuzzle>();

    public List<Chuzzle> DeathAnimationChuzzles = new List<Chuzzle>();

    public bool IsMovingToPrevPosition;

    public List<Chuzzle> NewTilesAnimationChuzzles = new List<Chuzzle>();
    public int[] NewTilesInColumns = new int[0];
    public float TimeFromTip = 0;

    public bool IsPlaying;

    #region Events

    public event Action<List<Chuzzle>> CombinationDestroyed;

    public event Action<Gamefield> GameStarted;

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
        if (handler != null) handler(this);
    }

    private void InvokeTileDestroyed(Chuzzle destroyedChuzzle)
    {
        if (TileDestroyed != null)
        {
            TileDestroyed(destroyedChuzzle);
        }
    }

    #endregion

    #region Event Handlers

    private void OnDestroy()
    {
        Field.DragDrop -= OnDragDrop;
    }

    private void OnDragDrop()
    {
        //move all tiles to new real coordinates
        MoveToRealCoordinates();

        //drop shining
        foreach (var chuzzle in Level.ActiveChuzzles)
        {
            chuzzle.Shine = false;
        }
    }

    #endregion

    private void Awake()
    {
        Field.DragDrop += OnDragDrop;
    }

    private void AnalyzeField(bool isHumanAction)
    {
        var hasNewTiles = CreateNew();
        if (hasNewTiles)
        {
            return;
        }

        //check new combination
        var combinations = GamefieldUtility.FindCombinations(Level.ActiveChuzzles);
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

    private void LateUpdate()
    {
        Field.LateUpdate(Level.ActiveCells);
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
        if (Level.ActiveChuzzles.Contains(chuzzle))
        {
            Level.ActiveChuzzles.Remove(chuzzle);
        }
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

        SpecialCreation.SpecialTilesAnimated.Clear();

        Field.Reset();

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
            StageManager.Init(level.Stages);
        }

        NewTilesInColumns = new int[Level.Width];

        AddEventHandlers();

        InvokeGameStarted();

        AnalyzeField(false);

        IsPlaying = true;
    }

    private void AddEventHandlers()
    {
        RemoveEventHandlers();
        GameMode.Win += OnWin;
        GameMode.GameOver += OnGameOver;
    }

    private void OnGameOver()
    {
        Player.Instance.Lifes.SpentLife();
        IsPlaying = false;
    }

    private void OnWin()
    {
        IsPlaying = false;
    }

    private void RemoveEventHandlers()
    {
        GameMode.Win -= OnWin;
        GameMode.GameOver -= OnGameOver;
    }

    private void Update()
    {
        if (LastLoadedLevel == null || !IsPlaying)
        {
            Debug.Log("No level loaded");
            return;
        }

        IsMovingToPrevPosition = AnimatedChuzzles.Any() || DeathAnimationChuzzles.Any() ||
                                 NewTilesAnimationChuzzles.Any() || SpecialCreation.SpecialTilesAnimated.Any();
        if (IsMovingToPrevPosition )
        {
            return;
        }

        TimeFromTip += Time.deltaTime;
        if (TimeFromTip > 1)
        {
            var list = GamefieldUtility.Tip(Level.ActiveChuzzles);
            if (list.Any())
            {
                foreach (var chuzzle in list)
                {
                    chuzzle.Shine = true;
                }
            }
            TimeFromTip = 0;
        }

        Field.Update(Level.ActiveChuzzles);
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

        if (!AnimatedChuzzles.Any())
        {
            AnalyzeField(true);
        }
    }

    #endregion

    public void CalculateRealCoordinatesFor(Chuzzle chuzzle)
    {
        chuzzle.Real = Level.GetCellAt(Mathf.RoundToInt(chuzzle.transform.localPosition.x/chuzzle.Scale.x),
            Mathf.RoundToInt(chuzzle.transform.localPosition.y/chuzzle.Scale.y), false);
    }

    private void MoveToRealCoordinates()
    {
        foreach (var c in Field.SelectedChuzzles)
        {
            CalculateRealCoordinatesFor(c);
        }

        foreach (var c in Level.Chuzzles)
        {
            c.MoveTo = c.Real;
        }

        var anyMove = MoveToTargetPosition(Level.Chuzzles, "OnTweenMoveAfterDrag");

        if (!anyMove)
        {
            AnalyzeField(true);
        }
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
            Level.UpdateActive();

            var combinations = GamefieldUtility.FindCombinations(Level.ActiveChuzzles);
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
                    var chuzzle = Level.CreateRandomChuzzle(x, Level.Height + j, true);
                    chuzzle.Current.IsTemporary = true;
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
                    var cell = Level.GetCellAt(x, y, false);
                    if (Level.At(x, y) == null && cell.Type != CellTypes.Block)
                    {
                        while (cell != null)
                        {
                            var chuzzle = Level.At(cell.x, cell.y);
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
        return true;
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

        if (!NewTilesAnimationChuzzles.Any())
        {
            AnalyzeField(false);
        }
    }

    #endregion

    #region Death

    #region Event Handlers

    public void OnCompleteDeath(Object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        DeathAnimationChuzzles.Remove(chuzzle);
        Destroy(chuzzle.gameObject);

        //if all deleted
        if (DeathAnimationChuzzles.Count() == 0)
        {
            //start tweens for new chuzzles
            MoveTilesWhoNeedMoves();
        }
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