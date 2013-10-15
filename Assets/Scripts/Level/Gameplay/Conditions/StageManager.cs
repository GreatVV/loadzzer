using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

[Serializable]
public class Stage
{
    public int Id;
    public int NextStage = -1;

    public int MinY;
    public int MaxY;
    
    public Condition Condition;
    public event Action StageComplete;

    protected virtual void InvokeStageComplete()
    {
        Action handler = StageComplete;
        if (handler != null) handler();
    }

    public void OnPointsChanged(int points)
    {
        if (Condition.IsScore)
        {
            if (Condition.Target < points)
            {
                InvokeStageComplete();
            }
        }
    }

}

[Serializable]
public class StageManager
{
    public List<Stage> Stages = new List<Stage>();

    public Stage CurrentStage;

    public Gamefield Gamefield;

    public GameObject Camera;

    public void Init(Gamefield gamefield)
    {
        Gamefield = gamefield;
        Stages.Clear();
        Stages.Add(
            new Stage()
            {
                Id = 0,
                Condition = new Condition()
                {
                    IsScore = true,
                    Target = 1000,
                },
                NextStage = 1,
                MaxY = 23,
                MinY = 16
            });

        Stages.Add(
            new Stage()
            {
                Id = 1,
                Condition = new Condition()
                {
                    IsScore = true,
                    Target = 2000,
                },
                NextStage = 2,
                MaxY = 15,
                MinY = 8
            });

        Stages.Add(
           new Stage()
           {
               Id = 2,
               Condition = new Condition()
               {
                   IsScore = true,
                   Target = 3000,
               },
               NextStage = -1,
               MaxY = 7,
               MinY = 0
           });

        ChangeStageTo(0);
    }

    private void OnStageComplete()
    {
        Gamefield.PointSystem.PointChanged -= CurrentStage.OnPointsChanged;
        CurrentStage.StageComplete -= OnStageComplete;
        if (CurrentStage.NextStage == -1)
        {
            //HACK use other way to win (???)
            Gamefield.GameMode.IsWin = true;
        }
        else
        {
            ChangeStageTo(CurrentStage.NextStage);
        }
    }

    public void ChangeStageTo(int id)
    {
        CurrentStage = Stages.First(x => x.Id == id);

        CurrentStage.StageComplete += OnStageComplete;
        Gamefield.PointSystem.PointChanged += CurrentStage.OnPointsChanged;

        Gamefield.Level.ChoseFor(CurrentStage.MinY, CurrentStage.MaxY);

        var targetPosition = GamefieldUtility.ConvertXYToPosition(0, CurrentStage.MinY, Gamefield.Level.ChuzzleSize);

        iTween.MoveTo(Camera,
                   iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", -10, "time", 5f));

    }
}
