using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

    public Gamefield Gamefield;
    public GameMode gameMode;

    public GameObject startGamePanel;
    public GameObject inGamePanel;
    public GameObject gameOverPanel;
    public GuiWinPanel winPanel;

    public void Awake()
    {
        gameMode.GameOver += OnGameOver;
        gameMode.Win += OnWin;
    }

    public void OnStartClick()
    {
        Gamefield.StartGame();
        DisableAllPanels();
        inGamePanel.SetActive(true);
    }

    public void OnRestartClick()
    {
        Gamefield.Reset();
        Gamefield.StartGame();
    }

    public void OnGameOverRestartClick()
    {
        DisableAllPanels();
        inGamePanel.SetActive(true);

        Gamefield.Reset();
        Gamefield.StartGame();
    }
    
    public void OnGameOver()
    {
        DisableAllPanels();
        gameOverPanel.SetActive(true);
    }

    public void OnWin()
    {
        DisableAllPanels();        
        winPanel.gameObject.SetActive(true);
        Debug.Log("Turn left:" + gameMode.Turns);
        winPanel.SetTurnsLeft(gameMode.Turns);
    }


    private void DisableAllPanels()
    {
        startGamePanel.SetActive(false);
        inGamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        winPanel.gameObject.SetActive(false);
    }

    public void OnWinRestartClick()
    {
        OnGameOverRestartClick();
    }
    
    //setup
    public void OnWidthSubmit(string text)
    {
        Gamefield.Width = int.Parse(text);
        OnRestartClick();
    }

    public void OnHeightSubmit(string text)
    {
        Gamefield.Height = int.Parse(text);
        OnRestartClick();
    }

}
