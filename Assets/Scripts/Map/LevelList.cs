using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class LevelList : MonoBehaviour
{

    public string LevelUrl = "https://dl.dropboxusercontent.com/u/70507866/loadzerbalance.txt";
    public JSONObject levels;
    public List<SerializedLevel> loadedLevels;
    public GameObject Grid;
    public GameObject LevelLabelPrefab;

    public UILabel Loading;

    public GameObject UI;

    void OnEnable()
    {
        /*
#if UNITY_ANDROID
     var jsonObject = new JSONObject("{name : \"New Project\", tileSize : 64, tileSetTileCount : 256, tileSetImageUrl : \"images/tile-game-1.png\", brushTile : 1, airTile : 0, paletteShortcuts : [0, 25, 50, 75, 100, 125, 150, 175, 200, 225, 250], levelArray : [{name : \"Level 1\", width : 7, height : 6, map : [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]}, {name : \"Level 2\", width : 6, height : 6, map : [1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1]}, {name : \"Level 3\", width : 6, height : 7, map : [0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0]}, {name : \"Level 4\", width : 7, height : 7, map : [0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0]}]}");
     var levelArray = jsonObject.GetField("levelArray").list;
     foreach (var level in levelArray)
     {
         loadedLevels.Add(SerializedLevel.FromJson(level));
     }
     PopulateToGrid();
#else               */
        Loading.text = "Loading";
        StartCoroutine(DownloadLevel(LevelUrl, levels, loadedLevels));
        //#endif
    }

    public IEnumerator DownloadLevel(string url, JSONObject jsonObject, List<SerializedLevel> levels)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone && www.text != "")
        {
            jsonObject = new JSONObject(www.text);

            var levelArray = jsonObject.GetField("levelArray").list;
            foreach (var level in levelArray)
            {
                levels.Add(SerializedLevel.FromJson(level));
            }
            PopulateToGrid();
        }

        Debug.Log("Levels Loaded:" + jsonObject);
    }

    public void PopulateToGrid()
    {
        NGUITools.ClearChildren(Grid);
        foreach (var level in loadedLevels)
        {
            var child = NGUITools.AddChild(Grid, LevelLabelPrefab);
            child.name = level.Name;
            child.GetComponent<UILabel>().text = level.Name;
            child.GetComponent<UIButtonMessage>().target = UI;
            child.transform.localScale = new Vector3(40, 40, 0);
            child.GetComponent<UIDragPanelContents>().draggablePanel = Grid.transform.parent.gameObject.GetComponent<UIDraggablePanel>();
        }
        Grid.GetComponent<UIGrid>().Reposition();
        Loading.text = "";
    }
}
