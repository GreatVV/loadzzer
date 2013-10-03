using System.Security.Cryptography;
using UnityEngine;
using System.Collections;
using System;

public abstract class GameMode
{
    public int Turns;
    public bool IsGameOver;
    public bool IsWin;
    public Points PointSystem;

    public GameModeDescription Description;

    protected GameMode(GameModeDescription description)
    {
        Description = description;
        Turns = StartTurns = description.Turns;
    }

    public int StartTurns;

    public abstract void HumanTurn();                              

    public virtual void Reset()
    {
        Turns = StartTurns;
        IsGameOver = false;
        IsWin = false;
        OnReset();
    }

    public abstract void OnReset();

    public event Action GameOver;
    public event Action Win;
    public event Action NoTurns;
    public event Action TurnsChanged;

    public void InvokeWin()
    {
        if (Win != null)
        {
            Win();
        }
    }

    public void InvokeGameOver()
    {
        if (GameOver != null)
        {
            GameOver();
        }
    }

    public void Check()
    {
        if (IsGameOver)
        {
            InvokeGameOver();
        }

        if (IsWin)
        {
            InvokeWin();
        }
    }

    public void SpendTurn()
    {
        Turns--;
        InvokeTurnsChanged();
        if (Turns == 0)
        {
            if (NoTurns != null)
            {
                NoTurns();
            }
            
            IsGameOver = true;
        }
    }

    public virtual void OnDestroy()
    {
        
    }

    public void InvokeTurnsChanged()
    {
        if (TurnsChanged != null)
        {
            TurnsChanged();
        }
    }
    
    public Gamefield Gamefield;

    public void Init(Gamefield gamefield)
    {   
        this.Gamefield = gamefield;
        PointSystem = Gamefield.pointSystem;
        Reset();
        OnDestroy();
        OnInit();
        InvokeTurnsChanged();
    }

    protected abstract void OnInit();
}