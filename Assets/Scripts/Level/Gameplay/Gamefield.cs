#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class Gamefield : MonoBehaviour
{
    #region Set in editor

    public GameObject Explosion;
    public GameObject PortalPrefab;
    public LayerMask ChuzzleMask;

    #endregion

    public CheckSpecialState CheckSpecial;
    public CreateNewState CreateNew;
    public GamefieldState CurrentState;
    public GameMode GameMode = GameModeFactory.CreateGameMode(GameModeDescription.CreateFromJson(null));
    public GameOverState GameOverState;
    public SerializedLevel LastLoadedLevel = null;
    public Level Level = new Level();

    public int[] NewTilesInColumns = new int[0];
    public Points PointSystem = new Points();
    public RemoveCombinationState RemoveState;
    public SpecialCreationUtility SpecialCreation;
    public StageManager StageManager = new StageManager();
    public float TimeFromTip = 0;
    public WinState WinState;
    public Field FieldState;

    #region Events

    public event Action<List<Chuzzle>> CombinationDestroyed;

    public event Action<Gamefield> GameStarted;

    public event Action<Chuzzle> TileDestroyed;
    public PauseState PauseState;

    #endregion

    #region Event Handlers

    private void OnGameOver()
    {
        SwitchStateTo(GameOverState);
        Player.Instance.Lifes.SpentLife();
    }

    private void OnWin()
    {
        SwitchStateTo(WinState);
    }

    #endregion

    #region Event Invokators

    public virtual void InvokeCombinationDestroyed(List<Chuzzle> combination)
    {
        var handler = CombinationDestroyed;
        if (handler != null) handler(combination);
    }

    public virtual void InvokeGameStarted()
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

    public void Start()
    {
        CheckSpecial = new CheckSpecialState(this);
        CreateNew = new CreateNewState(this);
        RemoveState = new RemoveCombinationState(this);
        GameOverState = new GameOverState(this);
        WinState = new WinState(this);
        FieldState = new Field(this);
        PauseState = new PauseState(this);
    }


    private void LateUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.LateUpdate();
        }
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
        CurrentState = new InitState(this);
        CurrentState.OnEnter();
    }

    public void AddEventHandlers()
    {
        RemoveEventHandlers();
        GameMode.Win += OnWin;
        GameMode.GameOver += OnGameOver;
    }

    private void RemoveEventHandlers()
    {
        GameMode.Win -= OnWin;
        GameMode.GameOver -= OnGameOver;
    }

    private void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }

    public void SwitchStateTo(GamefieldState newState)
    {
        CurrentState.OnExit();
        CurrentState = newState;
        Debug.Log("Switch to: "+CurrentState);
        CurrentState.OnEnter();
    }       
}