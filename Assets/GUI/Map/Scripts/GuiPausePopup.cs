﻿using AnimationOrTween;
using UnityEngine;

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
        transform.localPosition = new Vector3(0, -800, 0);
        iTween.MoveTo(gameObject, new Vector3(0,0,0), 0.5f);
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