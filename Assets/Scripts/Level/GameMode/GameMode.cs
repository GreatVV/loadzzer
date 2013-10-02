using UnityEngine;
using System.Collections;
using System;

public abstract class GameMode
{
    public int Turns;
    public bool isGameOver;
    public bool isWin;
    public Points PointSystem;    
    
    public int StartTurns;

    public abstract void Action();                              

    public virtual void Reset()
    {
        Turns = StartTurns;
        isGameOver = false;
        isWin = false;
        OnReset();
    }

    public abstract void OnReset();

    public UILabel turnsLabel;

    public event Action GameOver;
    public event Action Win;
    public event Action NoTurns;

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
        if (isGameOver)
        {
            InvokeGameOver();
        }

        if (isWin)
        {
            InvokeWin();
        }
    }

    public void SpendTurn()
    {
        Turns--;
        TurnsChanged();
        if (Turns == 0)
        {
            if (NoTurns != null)
            {
                NoTurns();
            }
            isGameOver = true;
        }
    }

    public virtual void OnDestroy()
    {
        
    }

    public void TurnsChanged()
    {
        turnsLabel.text = string.Format("Turns: {0}", Turns);
    }
}