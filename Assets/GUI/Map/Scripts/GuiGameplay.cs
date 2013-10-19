using UnityEngine;

public class GuiGameplay : Window
{
    public UILabel PointsLabel;
    public UILabel TargetScoreLabel;
    public UILabel TurnsLabel;

    #region Event Handlers

    public void OnChoseLevelClick()
    {
        UI.Instance.ShowMap();
    }

    public void OnGameOver()
    {
        UI.Instance.ShowGameoverPopup();
    }

    public void OnGameStarted(Gamefield gamefield)
    {
        AddEventHandlers(gamefield);

        TargetScoreLabel.text = string.Format("Target: {0}", gamefield.GameMode);

        OnTurnsChanged(gamefield.GameMode.Turns);
    }

    private void OnPointsChanged(int points)
    {
        PointsLabel.text = string.Format("Points: {0}", points);
    }

    public void OnRestartClick()
    {
        UI.Instance.Restart();
    }

    void OnPauseClick()
    {
        UI.Instance.ShowPausePopup();
    }

    private void OnTurnsChanged(int turns)
    {
        TurnsLabel.text = string.Format("Turns: {0}", turns);
    }

    public void OnWin()
    {
        UI.Instance.ShowWinPopup();
    }

    #endregion

    public void AddEventHandlers(Gamefield gamefield)
    {
        RemoveEventHandlers(gamefield);
        gamefield.PointSystem.PointChanged += OnPointsChanged;
        gamefield.GameMode.GameOver += OnGameOver;
        gamefield.GameMode.Win += OnWin;
        gamefield.GameMode.TurnsChanged += OnTurnsChanged;
    }

    private void RemoveEventHandlers(Gamefield gamefield)
    {
        gamefield.PointSystem.PointChanged -= OnPointsChanged;
        gamefield.GameMode.GameOver -= OnGameOver;
        gamefield.GameMode.Win -= OnWin;
        gamefield.GameMode.TurnsChanged -= OnTurnsChanged;
    }
}