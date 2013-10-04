using System;
using System.Collections.Generic;

[Serializable]
public class TargetChuzzleGameMode : GameMode
{
    public int TargetAmount;
    public int Amount;
    public Chuzzle targetChuzzle;

    public TargetChuzzleGameMode(GameModeDescription description) : base(description)
    {
        TargetAmount = description.Amount;
    }

    protected override void OnInit()
    {
        Gamefield.CombinationDestroyed += OnCombinationDestroyed;
        //TODO find chuzzle (it's special type)
    }


    public override void OnDestroy()
    {
        Gamefield.CombinationDestroyed -= OnCombinationDestroyed;
    }

    private void OnCombinationDestroyed(List<Chuzzle> destroyedChuzzles)
    {
        if (destroyedChuzzles.Contains(targetChuzzle))
        {
            Amount -= destroyedChuzzles.Count;
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
        TargetAmount = Turns;
        //TODO find target chuzzle
    }
}