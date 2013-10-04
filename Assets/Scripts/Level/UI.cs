using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UI : MonoBehaviour {

   


    public Gamefield Gamefield;

    public GameObject startGamePanel;
    public GameObject inGamePanel;
    public GameObject gameOverPanel;
    public GuiWinPanel winPanel;

    public UILabel TurnsLabel;
    public UILabel TargetScoreLabel;
    public UILabel PointsLabel;

    public LevelList LevelList;

    public void Awake()
    {
        Gamefield.GameStarted += OnGameStarted;
        Gamefield.pointSystem.PointChanged += OnPointsChanged;
      
    }

    private void OnPointsChanged(int obj)
    {
        PointsLabel.text = string.Format("Points: {0}", Gamefield.pointSystem.CurrentPoints);
    }

    void OnDestroy()
    {
        Gamefield.GameStarted -= OnGameStarted;
        Gamefield.pointSystem.PointChanged -= OnPointsChanged;
    }

   
    
   

    public void LoadLevel(GameObject gameObject)
    {
        Debug.Log("Load Level: "+gameObject.name);
        var levelToLoad = LevelList.loadedLevels.FirstOrDefault(x => x.Name == gameObject.name);
        Gamefield.StartGame(levelToLoad);
        DisableAllPanels();
        inGamePanel.SetActive(true);
    }

    public void OnStartClick()
    {
        Gamefield.StartGame();
        DisableAllPanels();
        inGamePanel.SetActive(true);
    }

    public void OnRestartClick()
    {   
        Gamefield.StartGame(Gamefield.LastSerializedLevel);
    }

    public void ChoseLevel()
    {
        DisableAllPanels();
        LevelList.Grid.transform.parent.gameObject.SetActive(true);
    }

    public void OnGameOverRestartClick()
    {
        DisableAllPanels();
        inGamePanel.SetActive(true);
        
        Gamefield.StartGame();
    }
    
    public void OnGameOver()
    {
        DisableAllPanels();
        gameOverPanel.SetActive(true);
    }

    public void OnGameStarted()
    {
        Gamefield.gameMode.GameOver -= OnGameOver;
        Gamefield.gameMode.Win -= OnWin;

        Gamefield.gameMode.GameOver += OnGameOver;
        Gamefield.gameMode.Win += OnWin;

        Gamefield.gameMode.TurnsChanged -= OnTurnsChanged;
        Gamefield.gameMode.TurnsChanged += OnTurnsChanged;

        if (Gamefield.gameMode is TargetScoreGameMode)
        {
            TargetScoreLabel.text = string.Format("Target score: {0}",
                (Gamefield.gameMode as TargetScoreGameMode).TargetScore);
        }
    }

    private void OnTurnsChanged()
    {       
        TurnsLabel.text = string.Format("Turns: {0}", Gamefield.gameMode.Turns);  
    }

    public void OnWin()
    {
        DisableAllPanels();        
        winPanel.gameObject.SetActive(true);
        Debug.Log("Turn left:" + Gamefield.gameMode.Turns);
        winPanel.SetTurnsLeft(Gamefield.gameMode.Turns);
    }


    private void DisableAllPanels()
    {
        LevelList.Grid.transform.parent.gameObject.SetActive(false);
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
        Gamefield.Level.Width = int.Parse(text);
        Gamefield.Level.ChuzzleSize = new Vector3(480, 480, 0) / Gamefield.Level.Width;
        OnRestartClick();
    }

    public void OnHeightSubmit(string text)
    {
        Gamefield.Level.Height = int.Parse(text);
        Gamefield.Level.ChuzzleSize = new Vector3(480, 480, 0) / Gamefield.Level.Width;
        OnRestartClick();
    }

    public void OnSetTS(string text)
    {
        (Gamefield.gameMode as TargetScoreGameMode).TargetScore = int.Parse(text);        
        OnRestartClick();
    }

    public void OnSetTST(string text)
    {
        (Gamefield.gameMode as TargetScoreGameMode).StartTurns = int.Parse(text);
        OnRestartClick();
    }

}
