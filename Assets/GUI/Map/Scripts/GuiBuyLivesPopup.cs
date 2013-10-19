public class GuiBuyLivesPopup : Window
{
    public UILabel LifeLabel;

    #region Event Handlers

    private void OnDisable()
    {
        RemoveEventHandlers();
    }

    private void OnEnable()
    {
        AddEventHandlers();
        OnLifesChanged(Player.Instance.Lifes.Lifes);
    }

    private void OnLifesChanged(int lifes)
    {
        LifeLabel.text = string.Format("Lifes: {0}", lifes);
    }

    public void OnAddLifeClick()
    {
        if (Economy.Instance.Spent(100))
        {
            Player.Instance.Lifes.AddLife();
            UI.Instance.ShowMap();
        }
        else
        {
            UI.Instance.ShowInAppPopup(this);
                
        }
    }

    #endregion

    private void AddEventHandlers()
    {
        RemoveEventHandlers();
        Player.Instance.Lifes.LifesChanged += OnLifesChanged;
    }

    private void RemoveEventHandlers()
    {
        Player.Instance.Lifes.LifesChanged -= OnLifesChanged;
    }
}