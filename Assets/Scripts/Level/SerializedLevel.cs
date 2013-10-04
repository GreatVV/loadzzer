using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedLevel
{
    public int Height;
    public string Name;

    public int NumberOfColors = -1;
    public int Width;
    public GameModeDescription GameMode;

    public List<Cell> SpecialCells = new List<Cell>();

    public static SerializedLevel FromJson(JSONObject jsonObject)
    {
        //Debug.Log("Print: \n" + jsonObject.ToString());
        var serializedLevel = new SerializedLevel();
        serializedLevel.Name = jsonObject.GetField("name").str;
        serializedLevel.Width = (int) jsonObject.GetField("width").n;
        serializedLevel.Height = (int) jsonObject.GetField("height").n;
        serializedLevel.NumberOfColors = jsonObject.HasField("NumberOfColor")
            ? (int) jsonObject.GetField("NumberOfColors").n
            : 8;

        serializedLevel.GameMode = GameModeDescription.CreateFromJson(jsonObject.GetField("GameMode"));

        var array = jsonObject.GetField("map").list;
        foreach (var tile in array)
        {
            if (tile.n == 0)
                continue;
            
            var x = array.IndexOf(tile) % serializedLevel.Width;
            var y = serializedLevel.Height - (array.IndexOf(tile) / serializedLevel.Width) - 1;
            // 2 - place for target delete
            //else - block
            if (Math.Abs(tile.n - 2) < 0.01f)
            {
              //  Debug.Log("Place at " + x + ": " + y);
                serializedLevel.SpecialCells.Add(new Cell(x,y) {HasPlace = true});
            }
            else
            {        
                serializedLevel.SpecialCells.Add(new Cell(x, y, CellTypes.Block));
            }
        }
        return serializedLevel;
    }     
}