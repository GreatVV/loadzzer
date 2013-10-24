#region

using UnityEngine;

#endregion

public class GuiStartLevelPopup : Window
{
    public UILabel BestScoreLabel;
    public SerializedLevel CurrentLevel;
    public LevelInfo CurrentLevelInfo;
    public UILabel NumberOfAttemptsLabel;
    public UILabel TaskLabel;

    #region Event Handlers

    private void OnEnable()
    {
        NumberOfAttemptsLabel.text = string.Format("Attempts: {0}", CurrentLevelInfo.NumberOfAttempts);
        BestScoreLabel.text = string.Format("Best score: {0}", CurrentLevelInfo.BestScore);
        TaskLabel.text = string.Format("Task: {0}", GameModeFactory.CreateGameMode(CurrentLevel.GameMode).ToString());
        transform.localPosition = new Vector3(0, -800, -5);
        iTween.MoveTo(gameObject, new Vector3(0, 0, -0.01f), 0.5f);
    }

    protected override bool OnClose()
    {
        Debug.Log("onclose");
        iTween.MoveTo(gameObject,
            iTween.Hash("x", 0, "y", 2, "z", -0.01f, "time", 0.5f,
                "oncomplete", "OnCloseAnimationComplete", "oncompletetarget", gameObject, "oncompleteparams", 0));

        return false;
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
    }

    private void OnCloseButton()
    {
        Close();
    }

    public void OnStartButton()
    {
        UI.Instance.TryStartLevel(CurrentLevel);
    }

    public void Show(SerializedLevel level)
    {
        CurrentLevel = level;
        CurrentLevelInfo = Player.Instance.GetLevelInfo(level.Name);
        Show();
    }

    #endregion
}