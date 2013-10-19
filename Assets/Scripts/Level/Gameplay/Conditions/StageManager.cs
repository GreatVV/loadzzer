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
    public bool WinOnComplete;

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

    public void Init(List<Stage> stages)
    {
        Camera.transform.position = new Vector3(0, 0, -10);
        if (stages == null || stages.Count == 0)
        {
            if (CurrentStage != null)
            {
                Gamefield.PointSystem.PointChanged -= CurrentStage.OnPointsChanged;
                CurrentStage.StageComplete -= OnStageComplete;
                CurrentStage = null;
            }
            Stages.Clear();
            Gamefield.Level.UpdateActive();
            return;
        }

        Stages = stages;
        ChangeStageTo(0);
    }

  
    private void OnStageComplete()
    {
        Gamefield.PointSystem.PointChanged -= CurrentStage.OnPointsChanged;
        CurrentStage.StageComplete -= OnStageComplete;
        if (CurrentStage.NextStage == -1 )
        {
            if (CurrentStage.WinOnComplete)
            {
                Gamefield.GameMode.IsWin = true;
            }
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
                   iTween.Hash("x", targetPosition.x, "y", targetPosition.y-30, "z", -10, "time", 5f));

    }
}
