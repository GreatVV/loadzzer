using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;         

public class TargetChuzzleGameMode : GameMode
{

    public Gamefield gamefield;

    public Chuzzle targetChuzzle;
    public int StartTargetTurns;
    public int TargetTurns;

    void Awake()
    {
        gamefield.CombinationDestroyed += OnCombinationDestroyed;
        OnReset();
    }

    void OnDestroy()
    {
        gamefield.CombinationDestroyed -= OnCombinationDestroyed;
    }

    void OnCombinationDestroyed(List<Chuzzle> destroyedChuzzles)
    {

        if (destroyedChuzzles.Contains(targetChuzzle))
        {
            TargetTurns -= destroyedChuzzles.Count;
        }
        

        if (TargetTurns <= 0)
        {
            InvokeWin();
        }
    
    }

    public override void Action()
    {
        SpendTurn();
        if (Turns == 0)
        {
            InvokeGameOver();
        }
    }

    public override void OnReset()
    {
        StartTargetTurns = Turns;
        //TODO find target chuzzle
    }
}
