using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

    public Gamefield Gamefield;

    public GameObject startGamePanel;

    public void OnStartClick()
    {
        Gamefield.StartGame();
        startGamePanel.SetActive(false);
    }
}
