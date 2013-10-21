public class GuiPausePopup : Window
{
    public UILabel TaskLabel;

    #region Event Handlers

    private void OnContinueClick()
    {
        Close();
        UI.Instance.Gamefield.IsPlaying = true;
    }

    private void OnEnable()
    {
        TaskLabel.text = UI.Instance.Gamefield.GameMode.ToString();
    }

    private void OnMapClick()
    {
        UI.Instance.ShowMap();
        Player.Instance.Lifes.SpentLife();
    }

    private void OnRestartClick()
    {
        UI.Instance.Restart();
        Player.Instance.Lifes.SpentLife();
    }

    #endregion
}