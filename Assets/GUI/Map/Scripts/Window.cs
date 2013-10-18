using UnityEngine;

public class Window : MonoBehaviour
{
    public Window ShowAfter;

    public void Show(Window showAfterAction = null)
    {
        ShowAfter = showAfterAction;
        gameObject.SetActive(true);
        OnShow();
    }

    protected virtual void OnShow()
    {
    }

    public void Close()
    {
        gameObject.SetActive(false);
        OnClose();
        if (ShowAfter != null)
        {
            ShowAfter.Show();
            ShowAfter = null;
        }
    }

    protected virtual void OnClose()
    {
    }
}