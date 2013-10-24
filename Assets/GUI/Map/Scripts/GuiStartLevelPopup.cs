#region

using UnityEngine;

#endregion

public class GuiStartLevelPopup : Window
{
    public static Phrase AttemptsString = new Phrase("Попыток: {0}", "StartLevelPopup_Attempts");
    public static Phrase TaskString = new Phrase("Задача: {0}", "StartLevelPopup_Task");
    public static Phrase BestScoreString = new Phrase("Лучший: {0}", "StartLevelPopup_BestScore");
    public UILabel BestScoreLabel;
    public SerializedLevel CurrentLevel;
    public LevelInfo CurrentLevelInfo;
    public UILabel NumberOfAttemptsLabel;
    public UILabel TaskLabel;

    #region Event Handlers

    private void OnEnable()
    {
        NumberOfAttemptsLabel.text = LocalizationStrings.GetString(AttemptsString, CurrentLevelInfo.NumberOfAttempts);
        BestScoreLabel.text = LocalizationStrings.GetString(BestScoreString, CurrentLevelInfo.BestScore);
        TaskLabel.text = LocalizationStrings.GetString(TaskString, GameModeToString.GetString(GameModeFactory.CreateGameMode(CurrentLevel.GameMode)));
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

    #endregion

    public void Show(SerializedLevel level)
    {
        CurrentLevel = level;
        CurrentLevelInfo = Player.Instance.GetLevelInfo(level.Name);
        Show();
    }
}