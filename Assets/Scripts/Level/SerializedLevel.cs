using System;
using System.Collections.Generic;

[Serializable]
public class SerializedLevel
{
    public int Height;
    public string Name;

    public int NumberOfColors = -1;
    public int Width;
    public GameMode gameMode;

    public List<Cell> specialCells = new List<Cell>();

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

        serializedLevel.gameMode =
            CreateGameModeFromString(jsonObject.HasField("GameMode")
                ? jsonObject.GetStringField("GameMode")
                : "TargetScore");


        var array = jsonObject.GetField("map").list;
        foreach (var tile in array)
        {
            var x = array.IndexOf(tile) % serializedLevel.Width;
            var y = serializedLevel.Height - (array.IndexOf(tile) / serializedLevel.Width) - 1;
            // 2 - place for target delete
            //else - block
            if (tile.n == 2)
            {
                var targetPlaceGameMode = serializedLevel.gameMode as TargetPlaceGameMode;
                targetPlaceGameMode.PlaceCoordinates.Add(new IntVector2(x,y));
            }
            else
            {        
                serializedLevel.specialCells.Add(new Cell(x, y, CellTypes.Block));
            }
        }
        return serializedLevel;
    }

    public static GameMode CreateGameModeFromString(string gameMode)
    {
        switch (gameMode)
        {
            case ("TargetScore"):
                return new TargetScoreGameMode();
            case ("TargetPlace"):
                return new TargetPlaceGameMode();
            case ("TargetChuzzle"):
                return new TargetChuzzleGameMode();
            default:
                throw new ArgumentOutOfRangeException("Not correct gammode" + gameMode);
        }
    }
}