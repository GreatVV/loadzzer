﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

[Serializable]
public class TargetChuzzleGameMode : GameMode
{
    public int TargetAmount;
    public int Amount;
    public Chuzzle TargetChuzzle;

    public TargetChuzzleGameMode(GameModeDescription description) : base(description)
    {
        Amount = TargetAmount = description.Amount;
    }

    protected override void OnInit()
    {
        //TODO find chuzzle (it's special type)
        TargetChuzzle = Gamefield.Level.Chuzzles.FirstOrDefault(x => x.Counter > 0);
        if (TargetChuzzle == null)
        {
           Debug.Log("No target chuzzle");
            return;
        }
        Debug.Log("Fucking magic");

        Gamefield.CombinationDestroyed -= OnCombinationDestroyed;
        Gamefield.CombinationDestroyed += OnCombinationDestroyed;
        
    }


    public override void OnDestroy()
    {
        Gamefield.CombinationDestroyed -= OnCombinationDestroyed;
    }

    private void OnCombinationDestroyed(List<Chuzzle> destroyedChuzzles)
    {
        Debug.Log("destroy");
        if (destroyedChuzzles.Contains(TargetChuzzle))
        {
            Amount -= destroyedChuzzles.Count-1;
            if (Amount < 0)
            {
                Amount = 0;
            }
            TargetChuzzle.GetComponentInChildren<tk2dTextMesh>().text = Amount.ToString(CultureInfo.InvariantCulture);
        }


        if (Amount <= 0)
        {
            IsWin = true;
        }
    }

    public override void HumanTurn()
    {
        SpendTurn();     
    }

    public override void OnReset()
    {
        Amount = TargetAmount;
        //TODO find target chuzzle
    }
}