using UnityEngine;

public class GuiBuyLivesPopup : Window
{
    public UILabel LifeLabel;
    public GameObject AddLifeButton;

    #region Event Handlers

    public void OnAddLifeClick()
    {
        if (Player.Instance.Lifes.IsRegenerating)
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
    }

    private void OnCloseButton()
    {
        Close();
    }

    private void OnDisable()
    {
        RemoveEventHandlers();
    }

    private void OnEnable()
    {
        AddEventHandlers();
        AddLifeButton.SetActive(Player.Instance.Lifes.IsRegenerating);
        OnLifesChanged(Player.Instance.Lifes.Lifes);
    }

    private void OnLifesChanged(int lifes)
    {
        if (!Player.Instance.Lifes.IsRegenerating)
        {
            LifeLabel.text = string.Format("Lifes: {0},\n and it's a maximum", lifes);
        }
        else
        {
            LifeLabel.text = string.Format("Lifes: {0}", lifes);
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