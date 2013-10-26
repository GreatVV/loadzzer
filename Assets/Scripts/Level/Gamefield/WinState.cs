using System;

[Serializable]
public class WinState : GamefieldState
{
    public WinState(Gamefield gamefield = null)
        : base(gamefield)
    {
    }

    #region Event Handlers

    public override void OnEnter()
    {
        var levelInfo = Player.Instance.GetLevelInfo(Gamefield.Level.Serialized.Name);
        if (Gamefield.PointSystem.CurrentPoints > levelInfo.BestScore)
        {
            levelInfo.BestScore = Gamefield.PointSystem.CurrentPoints;
        }

        levelInfo.IsCompleted = true;
        levelInfo.NumberOfAttempts++;
    }

    public override void OnExit()
    {
    }

    #endregion

    public override void Update()
    {
    }

    public override void LateUpdate()
    {
    }
}