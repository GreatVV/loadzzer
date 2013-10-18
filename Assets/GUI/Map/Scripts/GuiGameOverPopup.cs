public class GuiGameOverPopup : Window
{
    public UILabel MoneyLabel;

    #region Event Handlers

    public void OnAddTurns()
    {
        UI.Instance.AddTurns();
    }

    private void OnEnable()
    {
        MoneyLabel.text = string.Format("Money: {0}", Economy.Instance.CurrentMoney);
    }

    public void OnGameOverRestartClick()
    {
        UI.Instance.GameOverRestart();
    }

    #endregion
}