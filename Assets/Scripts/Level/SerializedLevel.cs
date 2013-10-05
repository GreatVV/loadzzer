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
            : 5;

        serializedLevel.GameMode = GameModeDescription.CreateFromJson(jsonObject.GetField("GameMode"));

        var array = jsonObject.GetField("map").list;
        foreach (var tile in array)
        {
            var x = array.IndexOf(tile) % serializedLevel.Width;
            var y = serializedLevel.Height - (array.IndexOf(tile) / serializedLevel.Width) - 1;

            var tileType = (int)tile.n;
            switch (tileType)
            {
                case(0): //empty
                    break;
                case(2): // place
                    serializedLevel.SpecialCells.Add(new Cell(x, y) { HasPlace = true });
                    break;
                case(3): //counter
                    serializedLevel.SpecialCells.Add(new Cell(x, y) { HasCounter = true });
                    break;
                default: // block
                    serializedLevel.SpecialCells.Add(new Cell(x, y, CellTypes.Block));
                    break;
            }
            
        }
        return serializedLevel;
    }     
}