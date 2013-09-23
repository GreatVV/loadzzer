using UnityEngine;
using System.Collections;
using System;

public abstract class GameMode : MonoBehaviour
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
    

    public event Action GameOver;
    public event Action Win;

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
}