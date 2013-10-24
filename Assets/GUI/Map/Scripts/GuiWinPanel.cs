#region

using UnityEngine;

#endregion

public class GuiWinPanel : Window
{
    public static Phrase TurnsLeftString = new Phrase("Ходов осталось: {0}", "WinPopup_TurnsLeft");
    public static Phrase ScoreString = new Phrase("Счет: {0}", "WinPopup_Score");
    public static Phrase BestScoreString = new Phrase("Лучший: {0}", "WinPopup_BestScore");
    public UILabel BestScore;
    public UILabel Score;
    public UILabel TurnsLeft;

    #region Event Handlers

    private void OnMapClick()
    {
        UI.Instance.ShowMap();
    }

    private void OnEnable()
    {
        transform.localPosition = new Vector3(0, -800, 0);
        iTween.MoveTo(gameObject, new Vector3(0, 0, 0), 0.5f);
    }

    protected override bool OnClose()
    {
        Debug.Log("onclose");
        iTween.MoveTo(gameObject,
            iTween.Hash("x", 0, "y", 2, "z", 0, "time", 0.5f,
                "oncomplete", "OnCloseAnimationComplete", "oncompletetarget", gameObject, "oncompleteparams", 0));

        return false;
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
    }

    #endregion

    public void Show(int numberOfTurnsLeft, int score, int bestScore)
    {
        TurnsLeft.text = LocalizationStrings.GetString(TurnsLeftString, numberOfTurnsLeft);
        Score.text = LocalizationStrings.GetString(ScoreString, score);
        BestScore.text = LocalizationStrings.GetString(BestScoreString, bestScore);
        Show();
    }
}