using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UI : MonoBehaviour {

    public string LevelUrl = "https://docs.google.com/document/d/1OHM3FcW7deuroI5D4MlI2dUpx5GoC2KkfF30brHLFsg/export?format=txt&id=1OHM3FcW7deuroI5D4MlI2dUpx5GoC2KkfF30brHLFsg&token=AC4w5Vgnw1hxB08A5gw__1fDBqOwZ2RB7Q%3A1380704843812";
    public JSONObject levels;
    public List<SerializedLevel> loadedLevels;
    public GameObject Grid;
    public GameObject LevelLabelPrefab;


    public Gamefield Gamefield;

    public GameObject startGamePanel;
    public GameObject inGamePanel;
    public GameObject gameOverPanel;
    public GuiWinPanel winPanel;

    public UILabel TurnsLabel;
    public UILabel TargetScoreLabel;

    public void Awake()
    {
        Gamefield.GameStarted += OnGameStarted;
        StartCoroutine(LoadLevels());
    }

    public static IEnumerator DownloadLevel(string url, JSONObject jsonObject, List<SerializedLevel> levels)
    {         
        WWW www = new WWW(url); 
            yield return www;
            if (www.isDone && www.text != "")
            {                   
                jsonObject = new JSONObject(www.text);

                var levelArray = jsonObject.GetField("levelArray").list;
                foreach(var level in levelArray)
                {
                    levels.Add(SerializedLevel.FromJson(level));
                }                
            }

            Debug.Log("Levels Loaded:" + jsonObject);
    }

    public IEnumerator LoadLevels()
    {
        yield return StartCoroutine(DownloadLevel(LevelUrl, levels, loadedLevels));
        PopulateToGrid();
    }

    public void PopulateToGrid()
    {
        NGUITools.ClearChildren(Grid);
        foreach(var level in loadedLevels)
        {
            var child = NGUITools.AddChild(Grid, LevelLabelPrefab);
            child.name = level.Name;
            child.GetComponent<UILabel>().text = level.Name;            
            child.GetComponent<UIButtonMessage>().target = gameObject;
            child.transform.localScale = new Vector3(40, 40, 0);
            child.GetComponent<UIDragPanelContents>().draggablePanel = Grid.transform.parent.gameObject.GetComponent<UIDraggablePanel>();
        }
        Grid.GetComponent<UIGrid>().Reposition();
    }

    public void LoadLevel(GameObject gameObject)
    {
        Debug.Log("Load Level: "+gameObject.name);
        var levelToLoad = loadedLevels.FirstOrDefault(x => x.Name == gameObject.name);
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
        Grid.transform.parent.gameObject.SetActive(true);
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
        Grid.transform.parent.gameObject.SetActive(false);
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
