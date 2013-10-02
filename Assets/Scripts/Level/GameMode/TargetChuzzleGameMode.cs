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
        gamefield.TilesDestroyed += OnTileDestroyed;
        OnReset();
    }

    void OnDestroy()
    {
        gamefield.TilesDestroyed -= OnTileDestroyed;
    }

    void OnTileDestroyed(List<Chuzzle> destroyedChuzzles)
    {
        if (destroyedChuzzles.Contains(targetChuzzle))
        {
            TargetTurns -= destroyedChuzzles.Count;
        }
        Turns--;

        if (TargetTurns <= 0)
        {
            InvokeWin();
        }
        else
        {
            if (Turns == 0)
            {
                InvokeGameOver();
            }
        }
    }

    public override void Action()
    {
    }

    public override void OnReset()
    {
        StartTargetTurns = Turns;
        //TODO find target chuzzle
    }
}
