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
                //UI.Instance.ShowMap();
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
        AddLifeButton.SetActive(Player.Instance.Lifes.IsRegenerating);
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