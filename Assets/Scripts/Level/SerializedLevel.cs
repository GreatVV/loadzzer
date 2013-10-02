using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SerializedLevel {

    public GameMode gameMode;

    public int Width;
    public int Height;
    public string Name;

    public int NumberOfColors = -1;

    public List<Cell> specialCells = new List<Cell>();

    public static SerializedLevel FromJson(JSONObject jsonObject)
    {           
        Debug.Log("Print: \n" + jsonObject.ToString());
        var serializedLevel = new SerializedLevel();
        serializedLevel.Name = jsonObject.GetField("name").str;
        serializedLevel.Width = (int)jsonObject.GetField("width").n;
        serializedLevel.Height = (int)jsonObject.GetField("height").n;
        serializedLevel.NumberOfColors = jsonObject.HasField("NumberOfColor")? (int)jsonObject.GetField("NumberOfColors").n : 8;

        var array = jsonObject.GetField("map").list;
        foreach(var tile in array)
        {
            if (tile.n != 0)
            {
                var x = array.IndexOf(tile)%serializedLevel.Width;
                var y = serializedLevel.Height - (array.IndexOf(tile) / serializedLevel.Width) - 1 ;
                serializedLevel.specialCells.Add(new Cell(x, y, CellTypes.Block));
            }
        }   
        return serializedLevel;
    }
}
