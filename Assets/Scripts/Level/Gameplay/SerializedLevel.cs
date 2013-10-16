using System;
using System.Collections.Generic;
using System.Linq;
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
    public List<Stage> Stages = new List<Stage>();

    public static SerializedLevel FromJson(JSONObject jsonObject)
    {
        Debug.Log("Print: \n" + jsonObject.ToString());
        var serializedLevel = new SerializedLevel();
        serializedLevel.Name = jsonObject.GetField("name").str;
        serializedLevel.Width = (int) jsonObject.GetField("width").n;
        serializedLevel.Height = (int) jsonObject.GetField("height").n;
        serializedLevel.NumberOfColors = jsonObject.HasField("numberOfColors")
            ? (int) jsonObject.GetField("numberOfColors").n
            : 6;

        serializedLevel.GameMode = GameModeDescription.CreateFromJson(jsonObject.GetField("gameMode"));

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

        serializedLevel.Stages = CreateStagesFromJsonObject(jsonObject.GetField("stages"));

        return serializedLevel;
    }

    private static List<Stage> CreateStagesFromJsonObject(JSONObject stagesJsonObject)
    {
        if (stagesJsonObject == null || stagesJsonObject.list == null || stagesJsonObject.list.First().type == JSONObject.Type.NULL)
        {
            return null;
        }

        var stages = new List<Stage>();
        foreach (var jsonObject in stagesJsonObject.list)
        {
         /*   if (jsonObject.type == JSONObject.Type.NULL)
            {
                return null;
            }*/
            var stage = new Stage()
            {
                Id = (int)jsonObject.GetField("Id").n,
                MinY = (int)jsonObject.GetField("MinY").n,
                MaxY = (int)jsonObject.GetField("MaxY").n,
                NextStage = (int)jsonObject.GetField("NextStage").n,
                WinOnComplete = jsonObject.GetField("WinOnComplete").b,
                Condition = new Condition()
                {
                    IsScore = jsonObject.GetField("Condition").GetField("IsScore").b,
                    Target = (int)jsonObject.GetField("Condition").GetField("Target").n
                }
            };
            stages.Add(stage);
        }
        return stages;
    }


    public override string ToString()
    {
        return string.Format("Name: {0} Width:{1} Height: {2}", Name, Width, Height);
    }
}