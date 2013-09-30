using UnityEngine;
using System.Collections;

public class AutoBackgroundChanger : MonoBehaviour {

    public string[] spriteNameList;
    public tk2dSprite sprite;
    public float time = 10f;

    public void Awake()
    {
        ChangeBackground();
    }

    void ChangeBackground()
    {
        sprite.SetSprite(spriteNameList[Random.Range(0, spriteNameList.Length)]);
        Invoke("ChangeBackground", time);
    }

}
