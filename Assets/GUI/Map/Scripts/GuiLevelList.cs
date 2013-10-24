﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuiLevelList : Window
{
    public string LevelUrl = "https://www.dropbox.com/s/h2ejykp67vdb784/loadzerbalance.txt?dl=1";
    public JSONObject levels;
    public List<SerializedLevel> LoadedLevels;
    public GameObject Grid;
    public GameObject LevelLabelPrefab;

    public UILabel Loading;

    public TextAsset Levels;

    #region Event Handlers

    private void OnEnable()
    {
        
//#if UNITY_ANDROID
     var jsonObject = new JSONObject(Levels.text);
     var levelArray = jsonObject.GetField("levelArray").list;
     foreach (var level in levelArray)
     {
         LoadedLevels.Add(SerializedLevel.FromJson(level));
     }
     PopulateToGrid();
//#else               */
  //      Loading.text = "Loading";
    //    NGUITools.ClearChildren(Grid);
    //    StartCoroutine(DownloadLevel(LevelUrl, levels));
        //#endif
    }

    #endregion

    public IEnumerator DownloadLevel(string url, JSONObject jsonObject)
    {
        var www = new WWW(url);
        yield return www;
        if (www.isDone && www.text != "")
        {
            jsonObject = new JSONObject(www.text);
            LoadedLevels.Clear();
            var levelArray = jsonObject.GetField("levelArray").list;
            foreach (var level in levelArray)
            {
                LoadedLevels.Add(SerializedLevel.FromJson(level));
            }
            Debug.Log("Number of loaded levels: " + LoadedLevels.Count);
            PopulateToGrid();
        }

        //NGUIDebug.Log("Levels Loaded:" + jsonObject);
    }

    public void PopulateToGrid()
    {
        /*NGUITools.ClearChildren(Grid);
        foreach (var level in LoadedLevels)
        {
            var child = NGUITools.AddChild(Grid, LevelLabelPrefab);
            child.name = level.Name;
            child.GetComponent<UILabel>().text = level.Name;
            child.GetComponent<UIButtonMessage>().target = gameObject;
            child.transform.localScale = new Vector3(40, 40, 0);
            child.GetComponent<UIDragPanelContents>().draggablePanel =
                Grid.transform.parent.gameObject.GetComponent<UIDraggablePanel>();
        }
        Grid.GetComponent<UIGrid>().Reposition();*/
        Loading.text = "";
    }

    public void LoadLevel(GameObject levelGameObject)
    {
        //Debug.Log("Start Load Level: "+levelGameObject.name);
        var levelToLoad = LoadedLevels.FirstOrDefault(x => x.Name == levelGameObject.name);
        //Debug.Log("Choose to load: " + levelToLoad);
        UI.Instance.TryStartLevel(levelToLoad);
    }

    public void OnLevelClick(GameObject sender)
    {
        var mapId = sender.GetComponent<MapId>();
        if (mapId.Index < LoadedLevels.Count && mapId.Index >= 0)
        {
            UI.Instance.TryStartLevel(LoadedLevels[mapId.Index]);
        }
    }
}