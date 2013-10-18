using System.Linq;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public Gamefield Gamefield;

    public GuiGameplay GuiGameplay;

    public GuiGameOverPopup GameOverPopup;
    public GuiWinPanel WinPanel;

    public LevelList LevelList;

    public GuiInAppPopup InAppPopup;

    #region Event Handlers

    private void OnDestroy()
    {
        RemoveEventHandlers();
    }


    public void OnStartClick()
    {
        DisableAllPanels();
        GuiGameplay.gameObject.SetActive(true);
        Gamefield.StartGame();
    }

    public void OnWinRestartClick()
    {
        GameOverRestart();
    }

    #endregion

    public void Awake()
    {
        AddEventHandlers();

        Instance = this;

        //Application.RegisterLogCallback(CallBackLog);
        // Application.RegisterLogCallbackThreaded(CallBackLog);
    }

    public void Restart()
    {
        Gamefield.StartGame(Gamefield.LastLoadedLevel);
    }

    private void AddEventHandlers()
    {
        RemoveEventHandlers();
        Gamefield.GameStarted += GuiGameplay.OnGameStarted;
    }

    private void RemoveEventHandlers()
    {
        Gamefield.GameStarted -= GuiGameplay.OnGameStarted;
    }

    private void CallBackLog(string condition, string stacktrace, LogType type)
    {
        NGUIDebug.Log("\nType: " + type);
        NGUIDebug.Log(stacktrace);
        NGUIDebug.Log(condition);
    }


    public void LoadLevel(GameObject levelGameObject)
    {
        //Debug.Log("Start Load Level: "+levelGameObject.name);
        var levelToLoad = LevelList.LoadedLevels.FirstOrDefault(x => x.Name == levelGameObject.name);
        //Debug.Log("Choose to load: " + levelToLoad);
        Gamefield.StartGame(levelToLoad);
        //Debug.Log("Game created");
        DisableAllPanels();
        //Debug.Log("Panel disabled");
        GuiGameplay.gameObject.SetActive(true);
    }


    private void DisableAllPanels()
    {
        LevelList.Grid.transform.parent.gameObject.SetActive(false);
        GuiGameplay.Close();
        GameOverPopup.Close();
        WinPanel.gameObject.SetActive(false);
        InAppPopup.Close();
    }

    public void ShowMap()
    {
        DisableAllPanels();
        LevelList.Grid.transform.parent.gameObject.SetActive(true);
    }

    public void ShowWinPopup()
    {
        DisableAllPanels();
        WinPanel.gameObject.SetActive(true);
        Debug.Log("Turn left:" + Gamefield.GameMode.Turns);
        WinPanel.SetTurnsLeft(Gamefield.GameMode.Turns);
    }

    public void ShowGameoverPopup()
    {
        DisableAllPanels();
        GameOverPopup.Show();
    }

    public void AddTurns()
    {
        DisableAllPanels();
        if (Economy.Instance.Spent(60))
        {
            Gamefield.GameMode.AddTurns(5);
            Gamefield.GameMode.IsGameOver = false;
            Gamefield.IsPlaying = true;
            GuiGameplay.gameObject.SetActive(true);
        }
        else
        {
            InAppPopup.Show(GameOverPopup);
        }
    }

    public void GameOverRestart()
    {
        DisableAllPanels();
        GuiGameplay.Show();
        Restart();
    }
}