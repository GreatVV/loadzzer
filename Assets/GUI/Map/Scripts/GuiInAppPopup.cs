public class GuiInAppPopup : Window
{
    public UILabel MoneyLabel;

    #region Event Handlers

    public void OnAdd100()
    {
        Economy.Instance.Add(100);
        if (ShowAfter != null)
        {
            Close();
        }
    }

    public void OnAdd1000()
    {
        Economy.Instance.Add(1000);
        if (ShowAfter != null)
        {
            Close();
        }
    }

    public void OnAdd10000()
    {
        Economy.Instance.Add(10000);
        if (ShowAfter != null)
        {
            Close();
        }
    }

    private void OnDestroy()
    {
        RemoveEventHandlers();
    }

    private void OnDisable()
    {
        RemoveEventHandlers();
    }

    private void OnEnable()
    {
        AddEventHandlers();
        OnMoneyChanged(Economy.Instance.CurrentMoney);
    }

    private void OnMoneyChanged(int money)
    {
        MoneyLabel.text = string.Format("Money: {0}", money);
    }

    #endregion

    public void AddEventHandlers()
    {
        RemoveEventHandlers();
        Economy.Instance.MoneyChanged += OnMoneyChanged;
    }

    private void RemoveEventHandlers()
    {
        Economy.Instance.MoneyChanged -= OnMoneyChanged;
    }

    private void OnCloseButton()
    {
        Close();
    }
}