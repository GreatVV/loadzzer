using UnityEngine;
using System.Collections;

public class GuiWinPanel : MonoBehaviour {

    public UILabel turnsLeft;

    public void SetTurnsLeft(int turnsLeft)
    {
        this.turnsLeft.text = string.Format("Turns left: {0}", turnsLeft);
    }
}
