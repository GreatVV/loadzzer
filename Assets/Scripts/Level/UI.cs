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
        Gamefield.PointSystem.PointChanged += OnPointsChanged;

        //Application.RegisterLogCallback(CallBackLog);
       // Application.RegisterLogCallbackThreaded(CallBackLog);
      
    }

    private void CallBackLog(string condition, string stacktrace, LogType type)
    {
        NGUIDebug.Log("\nType: "+type);
        NGUIDebug.Log(stacktrace);
        NGUIDebug.Log(condition);
    }

    private void OnPointsChanged(int obj)
    {
        PointsLabel.text = string.Format("Points: {0}", Gamefield.PointSystem.CurrentPoints);
    }

    void OnDestroy()
    {
        Gamefield.GameStarted -= OnGameStarted;
        Gamefield.PointSystem.PointChanged -= OnPointsChanged;
    }

    public void LoadLevel(GameObject gameObject)
    {
        //Debug.Log("Start Load Level: "+gameObject.name);
        var levelToLoad = LevelList.LoadedLevels.FirstOrDefault(x => x.Name == gameObject.name);
        //Debug.Log("Choose to load: " + levelToLoad);
        Gamefield.StartGame(levelToLoad);
        //Debug.Log("Game created");
        DisableAllPanels();
        //Debug.Log("Panel disabled");
        inGamePanel.SetActive(true);
    }

    public void OnStartClick()
    {
        DisableAllPanels();
        inGamePanel.SetActive(true);
        Gamefield.StartGame();
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

        OnRestartClick();
    }
    
    public void OnGameOver()
    {
        DisableAllPanels();
        gameOverPanel.SetActive(true);
    }

    public void OnGameStarted()
    {
        Gamefield.GameMode.GameOver -= OnGameOver;
        Gamefield.GameMode.Win -= OnWin;

        Gamefield.GameMode.GameOver += OnGameOver;
        Gamefield.GameMode.Win += OnWin;

        Gamefield.GameMode.TurnsChanged -= OnTurnsChanged;
        Gamefield.GameMode.TurnsChanged += OnTurnsChanged;

        if (Gamefield.GameMode is TargetScoreGameMode)
        {
            TargetScoreLabel.text = string.Format("Game mode: Target Score. Target score: {0}",
                (Gamefield.GameMode as TargetScoreGameMode).TargetScore);
        }
        else
        {
            TargetScoreLabel.text = string.Format("Game mode: {0}",Gamefield.GameMode.ToString());
        }

        OnTurnsChanged();
    }

    private void OnTurnsChanged()
    {       
        TurnsLabel.text = string.Format("Turns: {0}", Gamefield.GameMode.Turns);  
    }

    public void OnWin()
    {
        DisableAllPanels();        
        winPanel.gameObject.SetActive(true);
        Debug.Log("Turn left:" + Gamefield.GameMode.Turns);
        winPanel.SetTurnsLeft(Gamefield.GameMode.Turns);
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
        (Gamefield.GameMode as TargetScoreGameMode).TargetScore = int.Parse(text);        
        OnRestartClick();
    }

    public void OnSetTST(string text)
    {
        (Gamefield.GameMode as TargetScoreGameMode).StartTurns = int.Parse(text);
        OnRestartClick();
    }

}
