using UnityEngine;
using System.Collections;
using System;

public class TargetScoreGameMode : GameMode {

    public UILabel turnsLabel;
    public UILabel targetScore;

    public Points pointSystem;
    
    public int TargetScore;

    public event Action NoTurns;

    void Awake()
    {
        Turns = StartTurns;
        pointSystem.PointChanged += OnPointChanged;
        TurnsChanged();
        targetScore.text = string.Format("Target score: {0}", TargetScore);
    }

    public void OnPointChanged(int points)
    {
        if (points >= TargetScore)
        {
            isWin = true;
        }
    }

    public override void OnReset()
    {
        TurnsChanged(); 
        targetScore.text = string.Format("Target score: {0}", TargetScore);
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

    public void TurnsChanged()
    {
        turnsLabel.text = string.Format("Turns: {0}", Turns);
    }

    public override void Action()
    {
        SpendTurn();
    }
}
