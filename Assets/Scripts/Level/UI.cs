using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

    public Gamefield Gamefield;

    public GameObject startGamePanel;
    public GameObject inGamePanel;

    public void OnStartClick()
    {
        Gamefield.StartGame();
        startGamePanel.SetActive(false);
        inGamePanel.SetActive(true);
    }

    public void OnRestartClick()
    {
        Gamefield.Reset();
        Gamefield.StartGame();
    }
}
