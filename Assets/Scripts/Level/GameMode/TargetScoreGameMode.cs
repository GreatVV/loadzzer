public class TargetScoreGameMode : GameMode
{
    public int TargetScore;
    public Points pointSystem;
    public UILabel targetScore;

    private void Awake()
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

    public override void Action()
    {
        SpendTurn();
    }
}