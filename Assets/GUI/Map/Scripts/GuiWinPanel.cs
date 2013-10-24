#region

using UnityEngine;

#endregion

public class GuiWinPanel : Window
{
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
        TurnsLeft.text = string.Format("Turns left: {0}", numberOfTurnsLeft);
        Score.text = string.Format("Score: {0}", score);
        BestScore.text = string.Format("Best score: {0}", bestScore);
        Show();
    }
}